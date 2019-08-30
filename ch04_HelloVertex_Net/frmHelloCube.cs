using ch03_HelloCube_Net;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public partial class frmHelloCube : Form
{
   #region Members
    private SLMat4f   m_viewMatrix;        // view matrix
    private SLMat4f   m_projectionMatrix;  // projection matrix
    private SLMat4f   m_viewportMatrix;    // viewport matrix
    private SLMat4f   m_rotationMatrix;
    private SLVec3f   m_cam;
    private float     m_camZ;              // z-distance of camera
    private float     m_rotAngleUp;          // angle of cube rotation
    private float     m_rotAngleSide;          // angle of cube rotation
    private SLVec3f   add_rotAxis;
    private float     tForce;
    private float     scrollForce = 200.0f;
    private SLVec3f preCursorPosition;
    private SLVec3f cursorPosition;
    private bool isDown;
    private ZBuffer zBuffer;
    private List<Mesh> meshes;
    private SLLight light;
    private bool phongActive;
    private bool xWireframeActive;
    private bool zShowActive;
    private Stopwatch sw;
    private TimeSpan second;
    private float fps;

    private SLVec3f colorGreen = new SLVec3f(0, 255, 0);
    private SLVec3f colorBlue = new SLVec3f(0, 0, 255);
    private SLVec3f colorRed = new SLVec3f(255, 0, 0);

    private SLVec3f colorLightBlue = new SLVec3f(0, 255, 255);
    private SLVec3f colorViolet = new SLVec3f(255, 0, 255);
    private SLVec3f colorYellow = new SLVec3f(255, 255, 0);

    private SLVec3f colorWhite = new SLVec3f(255, 255, 255);
    private SLVec3f colorBlack = new SLVec3f(0, 0, 0);




    #endregion

    /// <summary>
    /// We initialize the matrices and the vertices for the wire frame cube
    /// </summary>
    public frmHelloCube()
    {
        // Create matrices
        m_viewMatrix       = new SLMat4f();
        m_projectionMatrix = new SLMat4f();
        m_viewportMatrix   = new SLMat4f();
        m_rotationMatrix   = new SLMat4f();
 

        light = new SLLight();
        phongActive = false;
        xWireframeActive = false;
        m_camZ = -5.5f;      // backwards movement of the camera

        m_cam = new SLVec3f(0, 0, m_camZ);

        add_rotAxis = new SLVec3f();
        tForce = 0.1f;
        preCursorPosition = new SLVec3f();
        cursorPosition = new SLVec3f();

        sw = new Stopwatch();
        sw.Start();
        second = new TimeSpan(0, 0, 1);
        fps = 0;


        meshes = new List<Mesh>();
        Cube c1 = new Cube(1f);
        c1.modelMatrix.Translate(-.5f, 0, 0);
        c1.color = colorGreen;

        Sphere s1 = new Sphere(1.5f, 15, 15);
        s1.modelMatrix.Translate(.5f, 0, 0);
        s1.color = colorRed;

        meshes.Add(c1);
        meshes.Add(s1);

        // Without double buffering it would flicker
        this.DoubleBuffered = true;
        InitializeComponent();
    }


    /// <summary>
    /// The forms load handler is used to call resize before the first paint
    /// </summary>
    private void frmHelloCube_Load(object sender, EventArgs e)
   {  
        this.Text = "Hello Cube with .NET";
        Console.WriteLine("");
        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine("Spinning cube without with .Net ...");
        this.KeyPreview = true;




        frmHelloCube_Resize(null, null);
   }

   /// <summary>
   /// The forms resize handler get called whenever the form is resized.
   /// When the form resizes we have to redefine the projection matrix
   /// as well as the viewport matrix.
   /// </summary>
   private void frmHelloCube_Resize(object sender, EventArgs e)
   {  
        float aspect = (float)ClientRectangle.Width / (float)ClientRectangle.Height;
        m_projectionMatrix.Perspective(50, aspect, 1.0f, 3.0f);
        m_viewportMatrix.Viewport(0, 0, 
                                ClientRectangle.Width, 
                                ClientRectangle.Height, 
                                0, 1);

        // set up zBuffer with the size of the window, near and far settup
        zBuffer = new ZBuffer(ClientRectangle.Width, ClientRectangle.Height, 1.0f, 1.4f);

        controlBox.Update();
        this.Invalidate();
        
    }


    /// <summary>
    /// The forms paint routine where all drawing happens.
    /// </summary>
    private void frmHelloCube_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        #region graphicsSetup

        g.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

        #endregion

        addFps();
        zBuffer.Reset();

        #region cameraMovement
        // start with identity every frame
        m_viewMatrix.Identity();

        // view transform: move the coordinate system away from the camera
        m_viewMatrix.Translate(m_cam);

        // add new Rotations for camera
        m_viewMatrix.Rotate(m_rotAngleUp + (cursorPosition.y - preCursorPosition.y), new SLVec3f(1, 0, 0));
        m_viewMatrix.Rotate(m_rotAngleSide + (cursorPosition.x - preCursorPosition.x), new SLVec3f(0, 1, 0));
        #endregion

        using (BmpG bmpGraphics = new BmpG(ClientRectangle.Width, ClientRectangle.Height, zBuffer, light))
        {
            #region graphicsMode
            bmpGraphics.phong = phongActive;
            bmpGraphics.wireframe = xWireframeActive;
            bmpGraphics.showZ = zShowActive;
            #endregion

            foreach (Mesh mesh in meshes)
            {
                // all transformed vertecies of the mesh are temporary saved in vertex2
                List<SLVertex> vertex2 = new List<SLVertex>();

                // Vertex Shader
                #region transformPipeline

                SLMat4f mv = new SLMat4f(m_viewMatrix);
                mv.Multiply(mesh.modelMatrix);

                SLMat3f nm = new SLMat3f(mv.InverseTransposed());

                // build combined matrix out of viewport, projection & modelview matrix
                SLMat4f mvp = new SLMat4f();
                mvp.Multiply(m_viewportMatrix); // screen
                mvp.Multiply(m_projectionMatrix); // projektion
                mvp.Multiply(mv); // kamera & view (cube)

                for (int n = 0; n < mesh.vertices.Length; n++)
                {
                    vertex2.Add(new SLVertex(mvp.Multiply(mesh.vertices[n].position),
                                              nm.Multiply(mesh.vertices[n].normale),
                                              mesh.color,
                                              mv.Multiply(mesh.vertices[n].position)));
                
                }
                #endregion

                // Fragment Shader
                drawVertices(vertex2, mesh.indices,m_cam, bmpGraphics);

            }
            // Pixel output
            g.DrawImageUnscaled(bmpGraphics.Result(), 0, 0);
        }


        // Tell the system that the window should be repaint again
        this.Invalidate();
   }

   /// <summary>Handles the mouse down event</summary>
   private void frmHelloCube_MouseDown(object sender, MouseEventArgs e)
   {

        isDown = true;

        preCursorPosition.Set(PerDeg(e.X, ClientRectangle.Width), PerDeg(e.Y, ClientRectangle.Height), 0);
        cursorPosition.Set(PerDeg(e.X, ClientRectangle.Width), PerDeg(e.Y, ClientRectangle.Height), 0);

        this.Invalidate();
  


    }
   
   /// <summary>Handles the mouse move event</summary>
   private void frmHelloCube_MouseMove(object sender, MouseEventArgs e)
   {
        if(isDown)
        {
            // cursor position
            cursorPosition.Set(PerDeg(e.X, ClientRectangle.Width), PerDeg(e.Y,ClientRectangle.Height), 0);
            this.Invalidate();

        }


    }

    /// <summary>Handles the mouse up event</summary>
    private void frmHelloCube_MouseUp(object sender, MouseEventArgs e)
   {
        m_rotAngleSide += cursorPosition.x - preCursorPosition.x;
        m_rotAngleUp += cursorPosition.y - preCursorPosition.y;

        cursorPosition.Set(SLVec3f.Zero);
        preCursorPosition.Set(SLVec3f.Zero);

        isDown = false;

        this.Invalidate();
    }
    
    /// <summary>Handles the mouse wheel event</summary>
    private void frmHelloCube_MouseWheel(object sender, MouseEventArgs e)
   {
        m_camZ += e.Delta / scrollForce;
        transformActive(0, 0, e.Delta / scrollForce);

    }

    private SLVec3f trackBallVec(float x, float y)
    {

        SLVec3f vec = new SLVec3f();
        float r;
        if (ClientRectangle.Width < ClientRectangle.Height)
        {
            r = (float)(ClientRectangle.Width / 2) * 0.88f;
        }
        else
        {
            r = (float)(ClientRectangle.Height / 2) * 0.88f;
        }
        vec.x = (x - ClientRectangle.Width / 2) / r;
        vec.y = (y - ClientRectangle.Height / 2) /  r * -1;

        float d2 = vec.x * vec.x + vec.y * vec.y;

        if (d2 < 1f)
        {

            vec.z = (float)Math.Sqrt(1f - d2); 

        }else
        {
            vec.z = 0f;
            vec.Normalize();
        }
        return vec;
    }
    private float trackBallRadi()
    {
        float r;
        if (ClientRectangle.Width < ClientRectangle.Height)
        {
            r = (float)(ClientRectangle.Width / 2) * 0.88f;
        }
        else
        {
            r = (float)(ClientRectangle.Height / 2) * 0.88f;
        }
        return r;
    }
    private float RadToDeg(float angle)
    {
        return angle * (180.0f / (float)Math.PI);
    }
    private float PerDeg(float value, float max)
    {
        return (value / max) * 180;
    }

    /// <summary>
    /// Calls DrawPolygon for the vertecies with the same indices
    /// backface culling calculations
    /// </summary>
    /// <param name="v">vertecies</param>
    /// <param name="index">indecies</param>
    /// <param name="cam">camera for backfaceculling</param>
    /// <param name="bmp">bitmap to draw on</param>
    private void drawVertices(List<SLVertex> v, int[] index,SLVec3f cam, BmpG bmp)
    {
        for (int i = 0; i < index.Length; i += 3)
        {
            if (backfaceCulling(v[index[i]], 
                                v[index[i + 1]], 
                                v[index[i + 2]], 
                                cam))
            {
                bmp.DrawPolygon(v[index[i]], 
                                v[index[i + 1]], 
                                v[index[i + 2]]);

            }

        }
    }

    /// <summary>
    /// if vertecies are in inverse order or not
    /// </summary>
    /// <param name="v0">vertex 1</param>
    /// <param name="v1">vertex 2</param>
    /// <param name="v2">vertex 3</param>
    /// <param name="cam">relative to the camera position</param>
    /// <returns></returns>
    private bool  backfaceCulling(SLVertex v0, SLVertex v1, SLVertex v2, SLVec3f cam)
    {
        SLVec3f face = SLVec3f.CrossProduct((v1.position - v0.position), (v2.position - v0.position));
        return (SLVec3f.DotProduct(face, cam) >= 0 || xWireframeActive);
    }

    /// <summary>
    /// After everything is loaded, the UI is update for the transparent background
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmHelloCube_Shown(object sender, EventArgs e)
    {
        controlBox.Update();
    }

    /// <summary>
    /// if the checkbox phon ist active set phong bool active
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void phongCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        phongActive = phongCheckBox.Checked;
        light.isPhong = phongCheckBox.Checked;
    }

    /// <summary>
    /// calls updateSphere method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void stackUpDown_ValueChanged(object sender, EventArgs e)
    {
        updateSphere((int)stackUpDown.Value, (int)slicesUpDown.Value);
    }
    /// <summary>
    /// calls updateSphere method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void slicesUpDown_ValueChanged(object sender, EventArgs e)
    {
        updateSphere((int)stackUpDown.Value, (int)slicesUpDown.Value);
    }

    /// <summary>
    /// Updates the sphere with the new slices and stacks
    /// </summary>
    /// <param name="stacks">new stacks</param>
    /// <param name="slices">new slices</param>
    private void updateSphere(int stacks, int slices)
    {
        for(int i = 0; i < meshes.Count; i++)
        {
            if (meshes[i] is Sphere)
            {
                Mesh oldMesh = meshes[i];
                meshes[i] = new Sphere(1.5f, stacks, slices);
                meshes[i].color = oldMesh.color;
                meshes[i].modelMatrix = oldMesh.modelMatrix;
                break;
            }
        }




    }

    /// <summary>
    /// updates and adds fps to the fps counter
    /// </summary>
    private void addFps()
    {
        if(sw.Elapsed > second)
        {
            fpsLabel.Text = fps.ToString("0.0") + " fps";
            fpsLabel.Update();
            fps = 0;
            sw.Reset();
            sw.Start();
        }
        fps++;
    }

    /// <summary>
    /// shift key is left, scroll force goes back to normal
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmHelloCube_KeyUp(object sender, KeyEventArgs e)
    {
        if(e.KeyCode == Keys.ShiftKey)
        {
            scrollForce = 200.0f;
        }
    }

    /// <summary>
    /// if wireframe checkbox ist active or not
    /// increaces diffuse
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void wireframeBox_CheckedChanged(object sender, EventArgs e)
    {
        xWireframeActive = wireframeBox.Checked;
        if (xWireframeActive)
        {
            light.diffuse *= 10;
        }
        else
        {
            light.diffuse /= 10;
        }
    }


    /// <summary>
    /// sets zBuffer from chekbox active or not
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void zBufferBox_CheckedChanged(object sender, EventArgs e)
    {
        zShowActive = zBufferBox.Checked;
    }


    /// <summary>
    /// moves active Controll Object in the input w a s d
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmHelloCube_KeyDown(object sender, KeyEventArgs e)
    {
        switch(e.KeyCode)
        {
            case (Keys.S):
                transformActive(0,-tForce,0);
                break;
            case (Keys.W):
                transformActive(0, tForce, 0);
                break;
            case (Keys.A):
                transformActive(-tForce,0, 0);
                break;
            case (Keys.D):
                transformActive(tForce, 0, 0);
                break;
            case (Keys.ShiftKey):
                scrollForce = 1000.0f;
                break;
            default:
                break;
        }
           
                

    }

    /// <summary>
    /// transforms active controll object
    /// </summary>
    /// <param name="x">x directon</param>
    /// <param name="y">y directon</param>
    /// <param name="z">z directon</param>
    private void transformActive( float x, float y, float z)
    {

        if (controllCam.Checked)
        {
            m_cam.y += y;
            m_cam.x += x;
            m_cam.z += z;
            return;
        }
        if(controllSphere.Checked)
        {
            if(meshes.Count > 1)
            {
                meshes[1].modelMatrix.Translate(x, y, z);
                return;
            }

        }
        if(controllCube.Checked)
        {
            meshes[0].modelMatrix.Translate(x, y, z);
            return;
        }
    }


    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
   static void Main()
   {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      frmHelloCube form = new frmHelloCube();
      Application.Run(form);

      
   }
}

