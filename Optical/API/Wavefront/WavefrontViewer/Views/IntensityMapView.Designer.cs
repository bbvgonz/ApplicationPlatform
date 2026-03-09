namespace WavefrontTester.Views
{
    partial class IntensityMapView
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
            this.intensityView = new System.Windows.Forms.PictureBox();
            this.pictureBoxColorBar = new System.Windows.Forms.PictureBox();
            this.labelIntensityMin = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.intensityView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorBar)).BeginInit();
            this.SuspendLayout();
            // 
            // intensityView
            // 
            this.intensityView.BackColor = System.Drawing.Color.DimGray;
            this.intensityView.Location = new System.Drawing.Point(0, 0);
            this.intensityView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.intensityView.Name = "intensityView";
            this.intensityView.Size = new System.Drawing.Size(1024, 768);
            this.intensityView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.intensityView.TabIndex = 4;
            this.intensityView.TabStop = false;
            // 
            // pictureBoxColorBar
            // 
            this.pictureBoxColorBar.BackColor = System.Drawing.Color.DimGray;
            this.pictureBoxColorBar.Location = new System.Drawing.Point(1039, 28);
            this.pictureBoxColorBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBoxColorBar.Name = "pictureBoxColorBar";
            this.pictureBoxColorBar.Size = new System.Drawing.Size(35, 450);
            this.pictureBoxColorBar.TabIndex = 5;
            this.pictureBoxColorBar.TabStop = false;
            // 
            // labelIntensityMin
            // 
            this.labelIntensityMin.AutoSize = true;
            this.labelIntensityMin.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelIntensityMin.Location = new System.Drawing.Point(1036, 482);
            this.labelIntensityMin.Name = "labelIntensityMin";
            this.labelIntensityMin.Size = new System.Drawing.Size(17, 18);
            this.labelIntensityMin.TabIndex = 6;
            this.labelIntensityMin.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(1036, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 18);
            this.label1.TabIndex = 7;
            this.label1.Text = "4095";
            // 
            // IntensityMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelIntensityMin);
            this.Controls.Add(this.pictureBoxColorBar);
            this.Controls.Add(this.intensityView);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "IntensityMapView";
            this.Size = new System.Drawing.Size(1100, 787);
            ((System.ComponentModel.ISupportInitialize)(this.intensityView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColorBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox intensityView;
        private System.Windows.Forms.PictureBox pictureBoxColorBar;
        private System.Windows.Forms.Label labelIntensityMin;
        private System.Windows.Forms.Label label1;
    }
}
