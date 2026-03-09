namespace WavefrontTester.Views
{
    partial class WavefrontMapView
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxColorBar = new System.Windows.Forms.PictureBox();
            this.zernikeView = new System.Windows.Forms.PictureBox();
            this.groupBoxImageSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericImageResolution = new System.Windows.Forms.NumericUpDown();
            this.signInversion = new System.Windows.Forms.CheckBox();
            this.aberrationUpper = new System.Windows.Forms.NumericUpDown();
            this.aberrationLower = new System.Windows.Forms.NumericUpDown();
            this.legendreView = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zernikeView)).BeginInit();
            this.groupBoxImageSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericImageResolution)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aberrationUpper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aberrationLower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.legendreView)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxColorBar
            // 
            this.pictureBoxColorBar.BackColor = System.Drawing.Color.DimGray;
            this.pictureBoxColorBar.Location = new System.Drawing.Point(32, 36);
            this.pictureBoxColorBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBoxColorBar.Name = "pictureBoxColorBar";
            this.pictureBoxColorBar.Size = new System.Drawing.Size(35, 512);
            this.pictureBoxColorBar.TabIndex = 7;
            this.pictureBoxColorBar.TabStop = false;
            // 
            // zernikeView
            // 
            this.zernikeView.BackColor = System.Drawing.Color.DimGray;
            this.zernikeView.Location = new System.Drawing.Point(6, 23);
            this.zernikeView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.zernikeView.Name = "zernikeView";
            this.zernikeView.Size = new System.Drawing.Size(512, 512);
            this.zernikeView.TabIndex = 6;
            this.zernikeView.TabStop = false;
            // 
            // groupBoxImageSettings
            // 
            this.groupBoxImageSettings.Controls.Add(this.label1);
            this.groupBoxImageSettings.Controls.Add(this.numericImageResolution);
            this.groupBoxImageSettings.Controls.Add(this.signInversion);
            this.groupBoxImageSettings.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBoxImageSettings.Location = new System.Drawing.Point(91, 561);
            this.groupBoxImageSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxImageSettings.Name = "groupBoxImageSettings";
            this.groupBoxImageSettings.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxImageSettings.Size = new System.Drawing.Size(256, 82);
            this.groupBoxImageSettings.TabIndex = 11;
            this.groupBoxImageSettings.TabStop = false;
            this.groupBoxImageSettings.Text = "Image Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Resolution:";
            // 
            // numericImageResolution
            // 
            this.numericImageResolution.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.numericImageResolution.Location = new System.Drawing.Point(98, 25);
            this.numericImageResolution.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericImageResolution.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericImageResolution.Name = "numericImageResolution";
            this.numericImageResolution.Size = new System.Drawing.Size(72, 24);
            this.numericImageResolution.TabIndex = 0;
            this.numericImageResolution.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericImageResolution.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // signInversion
            // 
            this.signInversion.AutoSize = true;
            this.signInversion.Location = new System.Drawing.Point(15, 56);
            this.signInversion.Name = "signInversion";
            this.signInversion.Size = new System.Drawing.Size(116, 21);
            this.signInversion.TabIndex = 14;
            this.signInversion.Text = "Sign Inversion";
            this.signInversion.UseVisualStyleBackColor = true;
            // 
            // aberrationUpper
            // 
            this.aberrationUpper.DecimalPlaces = 4;
            this.aberrationUpper.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aberrationUpper.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.aberrationUpper.Location = new System.Drawing.Point(9, 4);
            this.aberrationUpper.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.aberrationUpper.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.aberrationUpper.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.aberrationUpper.Name = "aberrationUpper";
            this.aberrationUpper.Size = new System.Drawing.Size(74, 24);
            this.aberrationUpper.TabIndex = 15;
            this.aberrationUpper.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aberrationUpper.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // aberrationLower
            // 
            this.aberrationLower.DecimalPlaces = 4;
            this.aberrationLower.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aberrationLower.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.aberrationLower.Location = new System.Drawing.Point(9, 556);
            this.aberrationLower.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.aberrationLower.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.aberrationLower.Minimum = new decimal(new int[] {
            65535,
            0,
            0,
            -2147483648});
            this.aberrationLower.Name = "aberrationLower";
            this.aberrationLower.Size = new System.Drawing.Size(74, 24);
            this.aberrationLower.TabIndex = 16;
            this.aberrationLower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.aberrationLower.Value = new decimal(new int[] {
            10,
            0,
            0,
            -2147418112});
            // 
            // legendreView
            // 
            this.legendreView.BackColor = System.Drawing.Color.DimGray;
            this.legendreView.Location = new System.Drawing.Point(6, 23);
            this.legendreView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.legendreView.Name = "legendreView";
            this.legendreView.Size = new System.Drawing.Size(512, 512);
            this.legendreView.TabIndex = 17;
            this.legendreView.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.zernikeView);
            this.groupBox2.Location = new System.Drawing.Point(91, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(524, 541);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Zernike";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.legendreView);
            this.groupBox3.Location = new System.Drawing.Point(621, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(524, 541);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Legendre";
            // 
            // WavefrontMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.aberrationLower);
            this.Controls.Add(this.aberrationUpper);
            this.Controls.Add(this.groupBoxImageSettings);
            this.Controls.Add(this.pictureBoxColorBar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "WavefrontMapView";
            this.Size = new System.Drawing.Size(1161, 653);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zernikeView)).EndInit();
            this.groupBoxImageSettings.ResumeLayout(false);
            this.groupBoxImageSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericImageResolution)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aberrationUpper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aberrationLower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.legendreView)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxColorBar;
        private System.Windows.Forms.PictureBox zernikeView;
        private System.Windows.Forms.GroupBox groupBoxImageSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericImageResolution;
        private System.Windows.Forms.CheckBox signInversion;
        private System.Windows.Forms.NumericUpDown aberrationUpper;
        private System.Windows.Forms.NumericUpDown aberrationLower;
        private System.Windows.Forms.PictureBox legendreView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
