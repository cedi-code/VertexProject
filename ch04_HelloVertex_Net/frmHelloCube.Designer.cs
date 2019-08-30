
partial class frmHelloCube
{
   /// <summary>
   /// Required designer variable.
   /// </summary>
   private System.ComponentModel.IContainer components = null;

   /// <summary>
   /// Clean up any resources being used.
   /// </summary>
   /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
   protected override void Dispose(bool disposing)
   {
      if (disposing && (components != null))
      {
         components.Dispose();
      }
      base.Dispose(disposing);
   }

   #region Windows Form Designer generated code

   /// <summary>
   /// Required method for Designer support - do not modify
   /// the contents of this method with the code editor.
   /// </summary>
   private void InitializeComponent()
   {
            this.phongCheckBox = new System.Windows.Forms.CheckBox();
            this.controlBox = new System.Windows.Forms.GroupBox();
            this.controlls = new System.Windows.Forms.GroupBox();
            this.controllCube = new System.Windows.Forms.RadioButton();
            this.controllSphere = new System.Windows.Forms.RadioButton();
            this.controllCam = new System.Windows.Forms.RadioButton();
            this.zBufferBox = new System.Windows.Forms.CheckBox();
            this.wireframeBox = new System.Windows.Forms.CheckBox();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.circleControll = new System.Windows.Forms.GroupBox();
            this.slicesUpDown = new System.Windows.Forms.NumericUpDown();
            this.slicesLabel = new System.Windows.Forms.Label();
            this.stackUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.controlBox.SuspendLayout();
            this.controlls.SuspendLayout();
            this.circleControll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slicesUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stackUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // phongCheckBox
            // 
            this.phongCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.phongCheckBox.AutoSize = true;
            this.phongCheckBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.phongCheckBox.Location = new System.Drawing.Point(6, 32);
            this.phongCheckBox.Name = "phongCheckBox";
            this.phongCheckBox.Size = new System.Drawing.Size(57, 17);
            this.phongCheckBox.TabIndex = 0;
            this.phongCheckBox.Text = "Phong";
            this.phongCheckBox.UseVisualStyleBackColor = true;
            this.phongCheckBox.CheckedChanged += new System.EventHandler(this.phongCheckBox_CheckedChanged);
            // 
            // controlBox
            // 
            this.controlBox.BackColor = System.Drawing.Color.Transparent;
            this.controlBox.Controls.Add(this.controlls);
            this.controlBox.Controls.Add(this.zBufferBox);
            this.controlBox.Controls.Add(this.wireframeBox);
            this.controlBox.Controls.Add(this.fpsLabel);
            this.controlBox.Controls.Add(this.circleControll);
            this.controlBox.Controls.Add(this.phongCheckBox);
            this.controlBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.controlBox.ForeColor = System.Drawing.Color.White;
            this.controlBox.Location = new System.Drawing.Point(283, 0);
            this.controlBox.Name = "controlBox";
            this.controlBox.Size = new System.Drawing.Size(80, 311);
            this.controlBox.TabIndex = 1;
            this.controlBox.TabStop = false;
            this.controlBox.Text = "Controlls";
            // 
            // controlls
            // 
            this.controlls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlls.Controls.Add(this.controllCube);
            this.controlls.Controls.Add(this.controllSphere);
            this.controlls.Controls.Add(this.controllCam);
            this.controlls.ForeColor = System.Drawing.Color.White;
            this.controlls.Location = new System.Drawing.Point(7, 104);
            this.controlls.Name = "controlls";
            this.controlls.Size = new System.Drawing.Size(67, 96);
            this.controlls.TabIndex = 5;
            this.controlls.TabStop = false;
            this.controlls.Text = "Controlls";
            // 
            // controllCube
            // 
            this.controllCube.AutoSize = true;
            this.controllCube.Location = new System.Drawing.Point(6, 66);
            this.controllCube.Name = "controllCube";
            this.controllCube.Size = new System.Drawing.Size(49, 17);
            this.controllCube.TabIndex = 2;
            this.controllCube.TabStop = true;
            this.controllCube.Text = "cube";
            this.controllCube.UseVisualStyleBackColor = true;
            // 
            // controllSphere
            // 
            this.controllSphere.AutoSize = true;
            this.controllSphere.Location = new System.Drawing.Point(6, 42);
            this.controllSphere.Name = "controllSphere";
            this.controllSphere.Size = new System.Drawing.Size(59, 17);
            this.controllSphere.TabIndex = 1;
            this.controllSphere.TabStop = true;
            this.controllSphere.Text = "Sphere";
            this.controllSphere.UseVisualStyleBackColor = true;
            // 
            // controllCam
            // 
            this.controllCam.AutoSize = true;
            this.controllCam.Checked = true;
            this.controllCam.Location = new System.Drawing.Point(6, 19);
            this.controllCam.Name = "controllCam";
            this.controllCam.Size = new System.Drawing.Size(60, 17);
            this.controllCam.TabIndex = 0;
            this.controllCam.TabStop = true;
            this.controllCam.Text = "camera";
            this.controllCam.UseVisualStyleBackColor = true;
            // 
            // zBufferBox
            // 
            this.zBufferBox.AutoSize = true;
            this.zBufferBox.Location = new System.Drawing.Point(6, 81);
            this.zBufferBox.Name = "zBufferBox";
            this.zBufferBox.Size = new System.Drawing.Size(61, 17);
            this.zBufferBox.TabIndex = 4;
            this.zBufferBox.Text = "show Z";
            this.zBufferBox.UseVisualStyleBackColor = true;
            this.zBufferBox.CheckedChanged += new System.EventHandler(this.zBufferBox_CheckedChanged);
            // 
            // wireframeBox
            // 
            this.wireframeBox.AutoSize = true;
            this.wireframeBox.Location = new System.Drawing.Point(6, 57);
            this.wireframeBox.Name = "wireframeBox";
            this.wireframeBox.Size = new System.Drawing.Size(71, 17);
            this.wireframeBox.TabIndex = 3;
            this.wireframeBox.Text = "wireframe";
            this.wireframeBox.UseVisualStyleBackColor = true;
            this.wireframeBox.CheckedChanged += new System.EventHandler(this.wireframeBox_CheckedChanged);
            // 
            // fpsLabel
            // 
            this.fpsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.Location = new System.Drawing.Point(44, 16);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(24, 13);
            this.fpsLabel.TabIndex = 2;
            this.fpsLabel.Text = " fps";
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // circleControll
            // 
            this.circleControll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.circleControll.Controls.Add(this.slicesUpDown);
            this.circleControll.Controls.Add(this.slicesLabel);
            this.circleControll.Controls.Add(this.stackUpDown);
            this.circleControll.Controls.Add(this.label1);
            this.circleControll.ForeColor = System.Drawing.Color.White;
            this.circleControll.Location = new System.Drawing.Point(6, 205);
            this.circleControll.Name = "circleControll";
            this.circleControll.Size = new System.Drawing.Size(67, 100);
            this.circleControll.TabIndex = 2;
            this.circleControll.TabStop = false;
            this.circleControll.Text = "Circle";
            // 
            // slicesUpDown
            // 
            this.slicesUpDown.Location = new System.Drawing.Point(6, 74);
            this.slicesUpDown.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.slicesUpDown.Name = "slicesUpDown";
            this.slicesUpDown.Size = new System.Drawing.Size(54, 20);
            this.slicesUpDown.TabIndex = 2;
            this.slicesUpDown.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.slicesUpDown.ValueChanged += new System.EventHandler(this.slicesUpDown_ValueChanged);
            // 
            // slicesLabel
            // 
            this.slicesLabel.AutoSize = true;
            this.slicesLabel.Location = new System.Drawing.Point(5, 58);
            this.slicesLabel.Name = "slicesLabel";
            this.slicesLabel.Size = new System.Drawing.Size(33, 13);
            this.slicesLabel.TabIndex = 2;
            this.slicesLabel.Text = "slices";
            // 
            // stackUpDown
            // 
            this.stackUpDown.Location = new System.Drawing.Point(8, 36);
            this.stackUpDown.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.stackUpDown.Name = "stackUpDown";
            this.stackUpDown.Size = new System.Drawing.Size(54, 20);
            this.stackUpDown.TabIndex = 1;
            this.stackUpDown.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.stackUpDown.ValueChanged += new System.EventHandler(this.stackUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "stacks";
            // 
            // frmHelloCube
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 311);
            this.Controls.Add(this.controlBox);
            this.Name = "frmHelloCube";
            this.Text = "Hello Cube in C#";
            this.Load += new System.EventHandler(this.frmHelloCube_Load);
            this.Shown += new System.EventHandler(this.frmHelloCube_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmHelloCube_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmHelloCube_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmHelloCube_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmHelloCube_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmHelloCube_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmHelloCube_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.frmHelloCube_MouseWheel);
            this.Resize += new System.EventHandler(this.frmHelloCube_Resize);
            this.controlBox.ResumeLayout(false);
            this.controlBox.PerformLayout();
            this.controlls.ResumeLayout(false);
            this.controlls.PerformLayout();
            this.circleControll.ResumeLayout(false);
            this.circleControll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slicesUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stackUpDown)).EndInit();
            this.ResumeLayout(false);

   }

    #endregion

    private System.Windows.Forms.CheckBox phongCheckBox;
    private System.Windows.Forms.GroupBox controlBox;
    private System.Windows.Forms.GroupBox circleControll;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.NumericUpDown stackUpDown;
    private System.Windows.Forms.NumericUpDown slicesUpDown;
    private System.Windows.Forms.Label slicesLabel;
    private System.Windows.Forms.Label fpsLabel;
    private System.Windows.Forms.CheckBox wireframeBox;
    private System.Windows.Forms.CheckBox zBufferBox;
    private System.Windows.Forms.GroupBox controlls;
    private System.Windows.Forms.RadioButton controllCube;
    private System.Windows.Forms.RadioButton controllSphere;
    private System.Windows.Forms.RadioButton controllCam;
}

