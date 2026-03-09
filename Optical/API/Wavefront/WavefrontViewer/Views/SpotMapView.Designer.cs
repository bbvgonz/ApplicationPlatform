namespace WavefrontTester.Views
{
    partial class SpotMapView
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
            this.spotMapPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.spotMapPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // spotMapPicture
            // 
            this.spotMapPicture.BackColor = System.Drawing.Color.DimGray;
            this.spotMapPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spotMapPicture.Location = new System.Drawing.Point(0, 0);
            this.spotMapPicture.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.spotMapPicture.Name = "spotMapPicture";
            this.spotMapPicture.Size = new System.Drawing.Size(1024, 768);
            this.spotMapPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.spotMapPicture.TabIndex = 3;
            this.spotMapPicture.TabStop = false;
            // 
            // SpotMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.spotMapPicture);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SpotMapView";
            this.Size = new System.Drawing.Size(1024, 768);
            ((System.ComponentModel.ISupportInitialize)(this.spotMapPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox spotMapPicture;
    }
}
