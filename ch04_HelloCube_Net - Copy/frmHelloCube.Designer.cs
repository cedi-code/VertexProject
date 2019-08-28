
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
            this.fpsLabel = new System.Windows.Forms.Label();
            this.circleControll = new System.Windows.Forms.GroupBox();
            this.slicesUpDown = new System.Windows.Forms.NumericUpDown();
            this.slicesLabel = new System.Windows.Forms.Label();
            this.stackUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.xRayBox = new System.Windows.Forms.CheckBox();
            this.controlBox.SuspendLayout();
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
            this.controlBox.Controls.Add(this.xRayBox);
            this.controlBox.Controls.Add(this.fpsLabel);
            this.controlBox.Controls.Add(this.circleControll);
            this.controlBox.Controls.Add(this.phongCheckBox);
            this.controlBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.controlBox.ForeColor = System.Drawing.Color.White;
            this.controlBox.Location = new System.Drawing.Point(304, 0);
            this.controlBox.Name = "controlBox";
            this.controlBox.Size = new System.Drawing.Size(80, 262);
            this.controlBox.TabIndex = 1;
            this.controlBox.TabStop = false;
            this.controlBox.Text = "Controlls";
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
            this.circleControll.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.circleControll.Controls.Add(this.slicesUpDown);
            this.circleControll.Controls.Add(this.slicesLabel);
            this.circleControll.Controls.Add(this.stackUpDown);
            this.circleControll.Controls.Add(this.label1);
            this.circleControll.ForeColor = System.Drawing.Color.White;
            this.circleControll.Location = new System.Drawing.Point(6, 80);
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
            this.slicesUpDown.TabIndex = 3;
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
            // xRayBox
            // 
            this.xRayBox.AutoSize = true;
            this.xRayBox.Location = new System.Drawing.Point(6, 57);
            this.xRayBox.Name = "xRayBox";
            this.xRayBox.Size = new System.Drawing.Size(48, 17);
            this.xRayBox.TabIndex = 3;
            this.xRayBox.Text = "x-ray";
            this.xRayBox.UseVisualStyleBackColor = true;
            this.xRayBox.CheckedChanged += new System.EventHandler(this.xRayBox_CheckedChanged);
            // 
            // frmHelloCube
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 262);
            this.Controls.Add(this.controlBox);
            this.Name = "frmHelloCube";
            this.Text = "Hello Cube in C#";
            this.Load += new System.EventHandler(this.frmHelloCube_Load);
            this.Shown += new System.EventHandler(this.frmHelloCube_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmHelloCube_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmHelloCube_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmHelloCube_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmHelloCube_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmHelloCube_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.frmHelloCube_MouseWheel);
            this.Resize += new System.EventHandler(this.frmHelloCube_Resize);
            this.controlBox.ResumeLayout(false);
            this.controlBox.PerformLayout();
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
    private System.Windows.Forms.CheckBox xRayBox;
}

