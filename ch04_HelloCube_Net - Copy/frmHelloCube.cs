using ch03_HelloCube_Net;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public partial class frmHelloCube : Form
{
   #region Members
   private SLMat4f   m_modelMatrix;       // model matrix
   private SLMat4f   m_viewMatrix;        // view matrix
   private SLMat4f   m_projectionMatrix;  // projection matrix
   private SLMat4f   m_viewportMatrix;    // viewport matrix
    private SLMat4f m_rotationMatrix;
   private SLVec3f[] m_v;                 // array for vertices for the cube
   private SLVec3f   preTrackBallVec;         //trackball start
   private SLVec3f   currTrackBallVec;       // current position on track ball
    private SLVec3f m_cam;
   private float     m_camZ;              // z-distance of camera
   private float     m_rotAngleUp;          // angle of cube rotation
   private float     m_rotAngleSide;          // angle of cube rotation
   private float     m_rotAngle;             // for the trackball
   private float     add_rotAngle;
   private SLVec3f   m_rotAxis;           // for the trackball
   private SLVec3f   add_rotAxis;
   private float     zoomForce;
   private SLVec3f preCursorPosition;
   private SLVec3f cursorPosition;
   private bool isDown;

    private SLVec3f colorGreen = new SLVec3f(0, 255, 0);
    private SLVec3f colorBlue = new SLVec3f(0, 0, 255);
    private SLVec3f colorRed = new SLVec3f(255, 0, 0);

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
        m_rotationMatrix = new SLMat4f();
        // define the 8 vertices of a cube
        m_v    = new SLVec3f[8];
      m_v[0] = new SLVec3f(-0.5f,-0.5f, 0.5f); // front lower left
      m_v[1] = new SLVec3f( 0.5f,-0.5f, 0.5f); // front lower right
      m_v[2] = new SLVec3f( 0.5f, 0.5f, 0.5f); // front upper right
      m_v[3] = new SLVec3f(-0.5f, 0.5f, 0.5f); // front upper left
      m_v[4] = new SLVec3f(-0.5f,-0.5f,-0.5f); // back lower left
      m_v[5] = new SLVec3f( 0.5f,-0.5f,-0.5f); // back lower right
      m_v[6] = new SLVec3f( 0.5f, 0.5f,-0.5f); // back upper left
      m_v[7] = new SLVec3f(-0.5f, 0.5f,-0.5f); // back upper right

      m_camZ = -4.5f;      // backwards movement of the camera
      zoomForce = m_camZ;

        m_cam = new SLVec3f(0, 0, m_camZ);
      preTrackBallVec = new SLVec3f();
      currTrackBallVec = new SLVec3f();


      add_rotAngle = 0;
      add_rotAxis = new SLVec3f();

      
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


        this.Invalidate();
   }

   /// <summary>
   /// The forms paint routine where all drawing happens.
   /// </summary>
   private void frmHelloCube_Paint(object sender, PaintEventArgs e)
   {   
      // start with identity every frame
      m_viewMatrix.Identity();
   
      // view transform: move the coordinate system away from the camera
      m_viewMatrix.Translate(m_cam);
   
      // model transform: rotate the coordinate system increasingly
      m_modelMatrix.Identity();

      // add previous Rotations
      m_modelMatrix.Multiply(m_rotationMatrix);
      // add new Rotations
      m_modelMatrix.Rotate(add_rotAngle, add_rotAxis);


      m_modelMatrix.Scale(2, 2, 2);
      
      // build combined matrix out of viewport, projection & modelview matrix
      SLMat4f m = new SLMat4f();
      m.Multiply(m_viewportMatrix); // ??
      m.Multiply(m_projectionMatrix); // projektion
      m.Multiply(m_viewMatrix); // kamera
      m.Multiply(m_modelMatrix); // cube

      // transform all vertices into screen space (x & y in pixels and z as the depth) 
      SLVec3f[] v2 = new SLVec3f[8];
      for (int i=0; i < m_v.Length; ++i)
      {  v2[i] = m.Multiply(m_v[i]);
      }


        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;



        BmpG bmp = new BmpG(ClientRectangle.Width, ClientRectangle.Height);

        //drawCube(v2, bmp);

        bmp.DrawPolygon(v2[0], colorRed, v2[1], colorGreen, v2[2], colorBlue);

        g.DrawImageUnscaled(bmp.Result(), 0, 0);


        float r = trackBallRadi();
        g.DrawEllipse(Pens.White, (ClientRectangle.Width / 2) - r, (ClientRectangle.Height / 2) - r, r*2, r*2);

        
     
      // Tell the system that the window should be repaint again
      this.Invalidate();
   }

   /// <summary>Handles the mouse down event</summary>
   private void frmHelloCube_MouseDown(object sender, MouseEventArgs e)
   {
        isDown = true;
        preTrackBallVec.Set(trackBallVec(e.X, e.Y));
        
        currTrackBallVec.Set(trackBallVec(e.X, e.Y));
        this.Invalidate();
  


    }
   
   /// <summary>Handles the mouse move event</summary>
   private void frmHelloCube_MouseMove(object sender, MouseEventArgs e)
   {
        if(isDown)
        {

            

            // cursor position
            currTrackBallVec.Set(trackBallVec(e.X, e.Y));

            // on what angle
            float dot = SLVec3f.DotProduct( currTrackBallVec, preTrackBallVec);
            add_rotAngle = RadToDeg((float)Math.Acos(dot));

            // on witch acces
            add_rotAxis = SLVec3f.CrossProduct(preTrackBallVec, currTrackBallVec);


            this.Invalidate();

        }


    }

    /// <summary>Handles the mouse up event</summary>
    private void frmHelloCube_MouseUp(object sender, MouseEventArgs e)
   {
        currTrackBallVec.Set(trackBallVec(e.X, e.Y));

        preTrackBallVec.Normalize();
        currTrackBallVec.Normalize();
  

        // Degrees
        float dot = SLVec3f.DotProduct(preTrackBallVec, currTrackBallVec);
        add_rotAngle = RadToDeg((float)Math.Acos(dot));
        
        // on wich acces
        add_rotAxis = SLVec3f.CrossProduct(preTrackBallVec, currTrackBallVec);

        // save rotation in Matrix
        m_rotationMatrix.Rotate(add_rotAngle, add_rotAxis);

        // reset so they dont add up
        add_rotAngle = 0;
        add_rotAxis.Set(SLVec3f.Zero);

        isDown = false;
        this.Invalidate();



        
    }
    

    /// <summary>Handles the mouse wheel event</summary>
    private void frmHelloCube_MouseWheel(object sender, MouseEventArgs e)
   {
        // TODO cap on m_camZ setzen
        //if ((zoomForce <= -1f && zoomForce >= -10f) ||
        //    ((float)e.Delta / Math.Abs(e.Delta) ==+1 && zoomForce <= -10f) ||
        //    ((float)e.Delta / Math.Abs(e.Delta) == -1 && zoomForce >= -1f))
        //{
        //    zoomForce += ((float)e.Delta / Math.Abs(e.Delta)) / 10;
        //    m_camZ = zoomForce;
        //    m_cam.z = zoomForce;
        //}
        //Console.WriteLine(m_camZ);
        
       

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

    private void drawCube(SLVec3f[] v2, BmpG bmp)
    {
        // if the triangle is frontig forward
        SLVec3f frontPol1 = SLVec3f.CrossProduct((v2[1] - v2[0]), (v2[2] - v2[0]));
        SLVec3f backPol1 = SLVec3f.CrossProduct((v2[4] - v2[5]), (v2[6] - v2[5]));
        SLVec3f rightPol1 = SLVec3f.CrossProduct((v2[5] - v2[1]), (v2[6] - v2[1]));
        SLVec3f leftPol1 = SLVec3f.CrossProduct((v2[3] - v2[0]), (v2[4] - v2[0]));
        SLVec3f topPol1 = SLVec3f.CrossProduct((v2[2] - v2[3]), (v2[6] - v2[3]));
        SLVec3f bottomPol1 = SLVec3f.CrossProduct((v2[1] - v2[5]), (v2[0] - v2[5]));

        // if the triangle in front of the camera
        bool visibleFront = SLVec3f.DotProduct(frontPol1, m_cam) >= 0;
        bool visibleBack = SLVec3f.DotProduct(backPol1, m_cam) >= 0;
        bool visibleRight = SLVec3f.DotProduct(rightPol1, m_cam) >= 0;
        bool visibleLeft = SLVec3f.DotProduct(leftPol1, m_cam) >= 0;
        bool visibleTop = SLVec3f.DotProduct(topPol1, m_cam) >= 0;
        bool visibleBottom = SLVec3f.DotProduct(bottomPol1, m_cam) >= 0;

        if (visibleFront)
        {
            // front
            bmp.DrawPolygon(v2[0], colorRed, v2[1], colorBlue, v2[2], colorGreen);
            bmp.DrawPolygon(v2[2], colorGreen, v2[3], colorBlue, v2[0], colorRed);
        }
        if (visibleBack)
        {
            bmp.DrawPolygon(v2[5], colorRed, v2[4], colorBlue, v2[6], colorGreen);
            bmp.DrawPolygon(v2[6], colorGreen, v2[4], colorBlue, v2[7], colorRed);
            // back
        }
        if (visibleRight)
        {
            // right side
            bmp.DrawPolygon(v2[1], colorBlue, v2[5], colorRed, v2[6], colorGreen);
            bmp.DrawPolygon(v2[6], colorGreen, v2[2], colorGreen, v2[1], colorBlue);
        }

        if (visibleTop)
        {
            // top
            //g.FillPolygon(new SolidBrush(Color.Blue), new PointF[] { new PointF(v2[3].x, v2[3].y), new PointF(v2[2].x, v2[2].y), new PointF(v2[6].x, v2[6].y) });
            //g.FillPolygon(new SolidBrush(Color.Blue), new PointF[] { new PointF(v2[6].x, v2[6].y), new PointF(v2[7].x, v2[7].y), new PointF(v2[3].x, v2[3].y) });

            //g.DrawPolygon(Pens.Black, new PointF[] { new PointF(v2[3].x, v2[3].y), new PointF(v2[2].x, v2[2].y), new PointF(v2[6].x, v2[6].y) });
            //g.DrawPolygon(Pens.Black, new PointF[] { new PointF(v2[6].x, v2[6].y), new PointF(v2[7].x, v2[7].y), new PointF(v2[3].x, v2[3].y) });
        }

        if (visibleLeft)
        {
            //  left side
            bmp.DrawPolygon(v2[0], colorRed, v2[3], colorBlue, v2[4], colorBlue);
            bmp.DrawPolygon(v2[4], colorBlue, v2[3], colorBlue, v2[7], colorRed);
        }

        if (visibleBottom)
        {
            //botom
            //g.FillPolygon(new SolidBrush(Color.Blue), new PointF[] { new PointF(v2[5].x, v2[5].y), new PointF(v2[1].x, v2[1].y), new PointF(v2[0].x, v2[0].y) });
            //g.FillPolygon(new SolidBrush(Color.Blue), new PointF[] { new PointF(v2[4].x, v2[4].y), new PointF(v2[5].x, v2[5].y), new PointF(v2[0].x, v2[0].y) });

            //g.DrawPolygon(Pens.Black, new PointF[] { new PointF(v2[5].x, v2[5].y), new PointF(v2[1].x, v2[1].y), new PointF(v2[0].x, v2[0].y) });
            //g.DrawPolygon(Pens.Black, new PointF[] { new PointF(v2[4].x, v2[4].y), new PointF(v2[5].x, v2[5].y), new PointF(v2[0].x, v2[0].y) });
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
      Application.Run(new frmHelloCube());
   }
}

