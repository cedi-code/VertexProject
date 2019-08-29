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
    private SLMat4f   m_modelMatrix;       // model matrix
    private SLMat4f   m_viewMatrix;        // view matrix
    private SLMat4f   m_projectionMatrix;  // projection matrix
    private SLMat4f   m_viewportMatrix;    // viewport matrix
    private SLMat4f   m_rotationMatrix;
    private SLVec3f[] m_v;                 // array for vertices for the cube
    private SLVec3f   preTrackBallVec;         //trackball start
    private SLVec3f   currTrackBallVec;       // current position on track ball
    private SLVec3f   m_cam;
    private SLVec3f   old_cam;
    private float     m_camZ;              // z-distance of camera
    private float     m_rotAngleUp;          // angle of cube rotation
    private float     m_rotAngleSide;          // angle of cube rotation
    private float     m_rotAngle;             // for the trackball
    private float     add_rotAngle;
    private SLVec3f   m_rotAxis;           // for the trackball
    private SLVec3f   add_rotAxis;
    private float     tForce;
    private SLVec3f preCursorPosition;
    private SLVec3f cursorPosition;
    private bool isDown;
    private ZBuffer zBuffer;
    private List<Mesh> meshes;
    private SLVec3f front, back, left, right, top, bottom;
    private SLLight light;
    private bool phongActive;
    private bool xRayActive;
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
        m_modelMatrix      = new SLMat4f();
        m_viewMatrix       = new SLMat4f();
        m_projectionMatrix = new SLMat4f();
        m_viewportMatrix   = new SLMat4f();
        m_rotationMatrix   = new SLMat4f();
          // define the 8 vertices of a cube


        light = new SLLight();
        phongActive = false;
        xRayActive = false;
        m_camZ = -5.5f;      // backwards movement of the camera

        m_cam = new SLVec3f(0, 0, m_camZ);
      //preTrackBallVec = new SLVec3f();
      //currTrackBallVec = new SLVec3f();

        add_rotAngle = 0;
        add_rotAxis = new SLVec3f();
        tForce = 0.1f;
        m_rotAngleUp = 0;
        m_rotAngle = 0;
        preCursorPosition = new SLVec3f();
        cursorPosition = new SLVec3f();

        sw = new Stopwatch();
        sw.Start();
        second = new TimeSpan(0, 0, 1);
        fps = 0;


        // setUpCube(ref slVertices, ref vNeighbour);
        // buildSphere(1.5f, 15, 15, ref slVertices, ref vNeighbour);
        //mesh = new Sphere(1.5f, 15, 15);
        meshes = new List<Mesh>();
        meshes.Add(new Cube(1f));
        meshes.Add(new Sphere(1.5f, 15, 15));

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
        // TODO söt eigentlech 3 si u ni 1.3 :(
        // (-2 * f * n) / (f - n)
        zBuffer = new ZBuffer(ClientRectangle.Width, ClientRectangle.Height, 1.0f, 1.3f);

        m_cam = m_rotationMatrix * m_cam;
     
        this.Invalidate();
        
    }

    /// <summary>
    /// The forms paint routine where all drawing happens.
    /// </summary>
    private void frmHelloCube_Paint(object sender, PaintEventArgs e)
    {

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;


        addFps();
        zBuffer.Reset();


        // start with identity every frame
        m_viewMatrix.Identity();


        // view transform: move the coordinate system away from the camera

        m_viewMatrix.Translate(m_cam);

        m_viewMatrix.Multiply(m_rotationMatrix);
        // add new Rotations
        m_viewMatrix.Rotate(m_rotAngleUp + (cursorPosition.y - preCursorPosition.y), new SLVec3f(1, 0, 0));
        m_viewMatrix.Rotate(m_rotAngleSide + (cursorPosition.x - preCursorPosition.x), new SLVec3f(0, 1, 0));


        using (BmpG bmpGraphics = new BmpG(ClientRectangle.Width, ClientRectangle.Height, zBuffer, light))
        {

            bmpGraphics.phong = phongActive;
            bmpGraphics.xRay = xRayActive;
            bmpGraphics.showZ = zShowActive;
  
            // ball.draw(bmpGraphics, m_cam);



            for (int o = 0; o < meshes.Count; o++)
            {

                SLMat4f mv = new SLMat4f(m_viewMatrix);
                mv.Multiply(meshes[o].m_modelMatrix);
                SLMat3f nm = new SLMat3f(mv.InverseTransposed());

                List<SLVertex> vertex2 = new List<SLVertex>();
                // build combined matrix out of viewport, projection & modelview matrix
                SLMat4f mvp = new SLMat4f();
                mvp.Multiply(m_viewportMatrix); // screen
                mvp.Multiply(m_projectionMatrix); // projektion
                mvp.Multiply(mv); // kamera & view (cube)

                // transform all vertices into screen space (x & y in pixels and z as the depth) 
                // TODO keine array sondern liste machen!

                for (int n = 0; n < meshes[o].vertices.Length; n++)
                {


                    vertex2.Add(new SLVertex(mvp.Multiply(meshes[o].vertices[n].position),
                                              nm.Multiply(meshes[o].vertices[n].normale),
                                              meshes[o].color,
                                              mv.Multiply(meshes[o].vertices[n].position)));
                
                }

                drawVertex(vertex2, meshes[o].indices, bmpGraphics);

            }
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
        m_camZ += e.Delta / 200.0f;
        // Console.WriteLine(e.Delta / 200.0f);
        if(m_camZ > -3)
        {
            m_camZ = -3;
        }
        m_cam.z = m_camZ;

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

    private void drawVertex(List<SLVertex> vertex, int[] indices, BmpG bmp)
    {
        for (int i = 0; i < indices.Length; i += 3)
        {
            SLVec3f face = SLVec3f.CrossProduct((vertex[indices[i + 1]].position - vertex[indices[i]].position), (vertex[indices[i + 2]].position - vertex[indices[i]].position));
            if (SLVec3f.DotProduct(face, m_cam) >= 0 || xRayActive)
            {
                bmp.DrawPolygon(vertex[indices[i]], vertex[indices[i + 1]], vertex[indices[i + 2]]);
            }

        }
    }

    private void frmHelloCube_Shown(object sender, EventArgs e)
    {
        controlBox.Update();
    }

    private void phongCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        phongActive = phongCheckBox.Checked;
        light.isPhong = phongCheckBox.Checked;
    }

    private void stackUpDown_ValueChanged(object sender, EventArgs e)
    {
        updateObjects();
    }
    private void updateObjects()
    {
        if(meshes.Count > 1)
        {
            meshes[1] = new Sphere(1.5f, (int)stackUpDown.Value, (int)slicesUpDown.Value);
        }else
        {
            meshes[0] = new Sphere(1.5f, (int)stackUpDown.Value, (int)slicesUpDown.Value);
        }


    }

    private void slicesUpDown_ValueChanged(object sender, EventArgs e)
    {
        updateObjects();
    }
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

    private void zBufferBox_CheckedChanged(object sender, EventArgs e)
    {
        zShowActive = zBufferBox.Checked;
    }


    /// <summary>
    /// moves camera in the input w a s d
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmHelloCube_KeyDown(object sender, KeyEventArgs e)
    {
        switch(e.KeyCode)
        {
            case (Keys.S):
                transformActive(0,-tForce);
                break;
            case (Keys.W):
                transformActive(0, tForce);
                break;
            case (Keys.A):
                transformActive(-tForce,0);
                break;
            case (Keys.D):
                transformActive(tForce, 0);
                break;
            default:
                break;
        }
           
                

    }


    private void transformActive( float x, float y)
    {

        if (controllCam.Checked)
        {
            m_cam.y += y;
            m_cam.x += x;
            return;
        }
        if(controllSphere.Checked)
        {
            if(meshes.Count > 1)
            {
                meshes[1].m_modelMatrix.Translate(x, y, 0);
                return;
            }

        }
        if(controllCube.Checked)
        {
            meshes[0].m_modelMatrix.Translate(x, y, 0);
            return;
        }
    }


    private void xRayBox_CheckedChanged(object sender, EventArgs e)
    {
        xRayActive = xRayBox.Checked;
        if(xRayActive)
        {
            light.diffuse *= 10;
        }else
        {
            light.diffuse /= 10;
        }

    }



    //private void CalcNormals()
    //{
    //    for(int i = 0; i < vNeighbour.Count; i+= 3)
    //    {
    //        SLVec3f e1, e2, n;

    //        e1 = slVertices[vNeighbour[i + 1]].position - slVertices[vNeighbour[i + 2]].position;
    //        e2 = slVertices[vNeighbour[i + 1]].position - slVertices[vNeighbour[i]].position;

    //        n = SLVec3f.CrossProduct(e1, e2);
    //        slVertices[vNeighbour[i]].normale = n;
    //        slVertices[vNeighbour[i + 1]].normale = n;
    //        slVertices[vNeighbour[i + 2]].normale = n;

    //    }
    //    for(int vid = 0; vid < slVertices.Count; vid++)
    //    {
    //        slVertices[vid].normale.Normalize();
    //    }
    //}

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

