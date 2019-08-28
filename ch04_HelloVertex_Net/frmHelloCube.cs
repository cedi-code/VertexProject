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
    private List<SLVertex> slVertices;
    private List<int> vNeighbour;
    private SLVec3f front, back, left, right, top, bottom;
    private SLLight light;
    private bool phongActive;
    private bool xRayActive;
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
   
        slVertices = new List<SLVertex>();
        vNeighbour = new List<int>();


        buildSphere(1f, 15, 15, ref slVertices, ref vNeighbour);
        // setUpCube(ref slVertices, ref vNeighbour);


        // Without double buffering it would flicker
        this.DoubleBuffered = true;
        InitializeComponent();
    }
    private void setUpCube(ref List<SLVertex> vertices, ref List<int> indices)
   {

        m_v = new SLVec3f[8];
        m_v[0] = new SLVec3f(-1f, -1f, 1f); // front lower left
        m_v[1] = new SLVec3f(1f, -1f, 1f); // front lower right
        m_v[2] = new SLVec3f(1f, 1f, 1f); // front upper right
        m_v[3] = new SLVec3f(-1f, 1f, 1f); // front upper left
        m_v[4] = new SLVec3f(-1f, -1f, -1f); // back lower left
        m_v[5] = new SLVec3f(1f, -1f, -1f); // back lower right
        m_v[6] = new SLVec3f(1f, 1f, -1f); // back upper left
        m_v[7] = new SLVec3f(-1f, 1f, -1f); // back upper right

        front = new SLVec3f(0, 0, 1);
        back = new SLVec3f(0, 0, -1);
        left = new SLVec3f(-1, 0, 0);
        right = new SLVec3f(1, 0, 0);
        top = new SLVec3f(0, 1, 0);
        bottom = new SLVec3f(0, -1, 0);

        vertices.Add(new SLVertex(m_v[0], front , colorRed));
        vertices.Add(new SLVertex(m_v[1], front , colorRed));
        vertices.Add(new SLVertex(m_v[2], front , colorRed));
        vertices.Add(new SLVertex(m_v[3], front , colorRed));
        vertices.Add(new SLVertex(m_v[0], left  , colorRed));
        vertices.Add(new SLVertex(m_v[4], left  , colorRed));
        vertices.Add(new SLVertex(m_v[7], left  , colorRed));
        vertices.Add(new SLVertex(m_v[3], left  , colorRed));
        vertices.Add(new SLVertex(m_v[4], back  , colorRed));
        vertices.Add(new SLVertex(m_v[5], back  , colorRed));
        vertices.Add(new SLVertex(m_v[6], back  , colorRed));
        vertices.Add(new SLVertex(m_v[7], back  , colorRed));
        vertices.Add(new SLVertex(m_v[1], right , colorRed));
        vertices.Add(new SLVertex(m_v[5], right , colorRed));
        vertices.Add(new SLVertex(m_v[6], right , colorRed));
        vertices.Add(new SLVertex(m_v[2], right , colorRed));
        vertices.Add(new SLVertex(m_v[2], top   , colorRed));
        vertices.Add(new SLVertex(m_v[6], top   , colorRed));
        vertices.Add(new SLVertex(m_v[7], top   , colorRed));
        vertices.Add(new SLVertex(m_v[3], top   , colorRed));
        vertices.Add(new SLVertex(m_v[0], bottom, colorRed));
        vertices.Add(new SLVertex(m_v[4], bottom, colorRed));
        vertices.Add(new SLVertex(m_v[5], bottom, colorRed));
        vertices.Add(new SLVertex(m_v[1], bottom, colorRed));



        int start = 0;
        int second = 0;
        int up = 1;
        for(int i = 0; i < 36;i+=6)
        {
            for(int s = 0; s < 4; s+=3)
            {
                for (int d = 1; d < 3; d++)
                {
                    indices.Add(up);
                    up++;
                }
                up--;
                indices.Add(start);
            }
            start += 4;
            up = start + 1;
        }

    }

    void buildSphere(float radius, int stacks, int slices, ref List<SLVertex> vertices, ref List<int> indices )
    {

        // create vertex array
        int _numV = (stacks + 1) * (slices + 1);
        addVerteciesToList(ref vertices, _numV);

        float theta, dtheta; // angles around x-axis
        float phi, dphi;     // angles around z-axis
        int i, j;          // loop counters
        int iv = 0;

        // init start values
        theta = 0.0f;
        dtheta = (float) Math.PI / stacks;
        dphi =  2.0f * (float) Math.PI / slices;

        // Define vertex position & normals by looping through all stacks
        for (i = 0; i <= stacks; ++i)
        {
            float sin_theta = (float) Math.Sin(theta);
            float cos_theta = (float) Math.Cos(theta);
            phi = 0.0f;

            // Loop through all slices
            for (j = 0; j <= slices; ++j)
            {
                if (j == slices) { phi = 0.0f; }

                // is unnecessary
                //vertices[iv] = new SLVertex();
                vertices[iv].setColor(colorRed);
                // define first the normal with length 1
                vertices[iv].normale.x = sin_theta * (float) Math.Cos(phi);
                vertices[iv].normale.y = sin_theta * (float) Math.Sin(phi);
                vertices[iv].normale.z = cos_theta;

                // set the vertex position w. the scaled normal
                vertices[iv].position.x = radius * vertices[iv].normale.x;
                vertices[iv].position.y = radius * vertices[iv].normale.y;
                vertices[iv].position.z = radius * vertices[iv].normale.z;

                // set the texture coords.
                //vertices[iv].t.x = 0; // ???
                //vertices[iv].t.y = 0; // ???

                phi += dphi;
                iv++;
            }
            theta += dtheta;
        }

        // create Index array x
        // neighbors
        int _numI = (int)(slices * stacks * 2 * 3);
        // indices = new int[_numI];
        int ii = 0, iV1, iV2;

        for (i = 0; i < stacks; ++i)
        {
            // index of 1st & 2nd vertex of stack
            iV1 = i * (slices + 1);
            iV2 = iV1 + slices + 1;

            for (j = 0; j < slices; ++j)
            { // 1st triangle ccw
                indices.Add(iV1 + j);
                indices.Add(iV2 + j);
                indices.Add(iV2 + j + 1);
                // 2nd triangle ccw
                indices.Add(iV1 + j);
                indices.Add(iV2 + j + 1);
                indices.Add(iV1 + j + 1);
            }
        }

    }

    private void addVerteciesToList(ref List<SLVertex> l, int size)
    {
        for(int s = 0; s < size; s++)
        {
            l.Add(new SLVertex());
        }
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
        zBuffer = new ZBuffer(ClientRectangle.Width, ClientRectangle.Height, 1.0f, 3.0f);

        m_cam = m_rotationMatrix * m_cam;
     
        this.Invalidate();
        
    }

    /// <summary>
    /// The forms paint routine where all drawing happens.
    /// </summary>
    private void frmHelloCube_Paint(object sender, PaintEventArgs e)
    {
        addFps();
        zBuffer.Reset();
        // start with identity every frame
        m_viewMatrix.Identity();
        m_modelMatrix.Identity();


        // view transform: move the coordinate system away from the camera
        m_viewMatrix.Translate(m_cam);

        m_viewMatrix.Multiply(m_rotationMatrix);
        // add new Rotations
        m_viewMatrix.Rotate(m_rotAngleUp + (cursorPosition.y - preCursorPosition.y), new SLVec3f(1, 0, 0));
        m_viewMatrix.Rotate(m_rotAngleSide + (cursorPosition.x - preCursorPosition.x), new SLVec3f(0, 1, 0));

        // m_modelMatrix.Scale(2, 2, 2);


        SLMat4f mv = new SLMat4f(m_viewMatrix);
        mv.Multiply(m_modelMatrix);
        SLMat3f nm = new SLMat3f(mv.InverseTransposed());
        
        
        // build combined matrix out of viewport, projection & modelview matrix
        SLMat4f mvp = new SLMat4f();
        mvp.Multiply(m_viewportMatrix); // screen
        mvp.Multiply(m_projectionMatrix); // projektion
        mvp.Multiply(mv); // kamera & view (cube)

        // transform all vertices into screen space (x & y in pixels and z as the depth) 
        // TODO keine array sondern liste machen!
        List<SLVertex> vertex2 = new List<SLVertex>();
        for (int n = 0; n < slVertices.Count; n++)
        {
            vertex2.Add(new SLVertex(mvp.Multiply(slVertices[n].position), 
                                      nm.Multiply(slVertices[n].normale),
                                      colorRed,
                                      mv.Multiply(slVertices[n].position)));

        }
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;



        using (BmpG bmpGraphics = new BmpG(ClientRectangle.Width, ClientRectangle.Height, zBuffer, light))
        {

            //showNormale(v2, vertex2, bmpGraphics);
            // showVectors(v2, bmpGraphics);
            bmpGraphics.phong = phongActive;
            bmpGraphics.xRay = xRayActive;
            //drawDiffuseCube(vertex2, bmpGraphics);
            drawVertex(vertex2, bmpGraphics);
            

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
        m_camZ += e.Delta / 100;
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

            vec.z = (float)Math.Sqrt(1f - d2); // why? how?

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

    private void drawVertex(List<SLVertex> vertex, BmpG bmp)
    {
        // TODO stacks berechnen
        for (int i = 0; i < vNeighbour.Count; i += 6)
        {
            SLVec3f face = SLVec3f.CrossProduct((vertex[vNeighbour[i+1]].position - vertex[vNeighbour[i]].position), (vertex[vNeighbour[i + 2]].position - vertex[vNeighbour[i]].position));
            // if backface culling
            if (SLVec3f.DotProduct(face, m_cam) >= 0 || xRayActive)
            {
                bmp.DrawPolygon(vertex[vNeighbour[i]], vertex[vNeighbour[i + 1]], vertex[vNeighbour[i + 2]]);
                
            }
            // second polygon in square
            SLVec3f face2 = SLVec3f.CrossProduct((vertex[vNeighbour[i + 4]].position - vertex[vNeighbour[i + 3]].position), (vertex[vNeighbour[i + 5]].position - vertex[vNeighbour[i + 3]].position));
            if (SLVec3f.DotProduct(face2, m_cam) >= 0 || xRayActive)
            {
                bmp.DrawPolygon(vertex[vNeighbour[i + 3]], vertex[vNeighbour[i + 4]], vertex[vNeighbour[i + 5]]);
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
        // TODO cube erhalten!
        slVertices.Clear();
        vNeighbour.Clear();
        buildSphere(1.5f, (int)stackUpDown.Value, (int)slicesUpDown.Value, ref slVertices, ref vNeighbour);
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
                m_cam.y -= tForce;
                break;
            case (Keys.W):
                m_cam.y += tForce;
                break;
            case (Keys.A):
                m_cam.x -= tForce;
                break;
            case (Keys.D):
                m_cam.x += tForce;
                break;
            default:
                break;
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

    private void drawDiffuseCube(SLVertex[] vertex, BmpG bmp)
    {

        // TODO für jedes poligon machen!
        SLVec3f[] pol = new SLVec3f[6];
        // front
        pol[0] = SLVec3f.CrossProduct((vertex[1].position - vertex[0].position), (vertex[2].position - vertex[0].position));
        // left
        pol[1] = SLVec3f.CrossProduct((vertex[6].position - vertex[4].position), (vertex[5].position - vertex[4].position));
        // back
        pol[2] = SLVec3f.CrossProduct((vertex[8].position - vertex[9].position), (vertex[10].position - vertex[9].position));
        // right
        pol[3] = SLVec3f.CrossProduct((vertex[13].position - vertex[12].position), (vertex[14].position - vertex[12].position));
        // top
        pol[4] = SLVec3f.CrossProduct((vertex[16].position - vertex[19].position), (vertex[17].position - vertex[19].position));
        // bottom
        pol[5] = SLVec3f.CrossProduct((vertex[23].position - vertex[22].position), (vertex[20].position - vertex[22].position));

        bool[] visible = new bool[6];
        // if the triangle in front of the camera
        for(int s = 0; s < 6; s++)
        {
            visible[s] = SLVec3f.DotProduct(pol[s], m_cam) >= 0;
        }


        for (int n = 0; n < visible.Length; n++)
        {
            if(visible[n])
            {
                for (int i = n*6; i < (n+1)*6; i+=3)
                {
                    bmp.DrawPolygon(vertex[vNeighbour[i]], vertex[vNeighbour[i + 1]], vertex[vNeighbour[i + 2]]);
                }
            }

        }

    }


    private void CalcNormals()
    {
        for(int i = 0; i < vNeighbour.Count; i+= 3)
        {
            SLVec3f e1, e2, n;

            e1 = slVertices[vNeighbour[i + 1]].position - slVertices[vNeighbour[i + 2]].position;
            e2 = slVertices[vNeighbour[i + 1]].position - slVertices[vNeighbour[i]].position;

            n = SLVec3f.CrossProduct(e1, e2);
            slVertices[vNeighbour[i]].normale = n;
            slVertices[vNeighbour[i + 1]].normale = n;
            slVertices[vNeighbour[i + 2]].normale = n;

        }
        for(int vid = 0; vid < slVertices.Count; vid++)
        {
            slVertices[vid].normale.Normalize();
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

