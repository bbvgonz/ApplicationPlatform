namespace Optical.API.Library
{
    partial class TestForm
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imagePictureBox = new System.Windows.Forms.PictureBox();
            this.imageFilePath = new System.Windows.Forms.TextBox();
            this.openImageFileButton = new System.Windows.Forms.Button();
            this.calculationButton = new System.Windows.Forms.Button();
            this.userOptionPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.readImageGroup = new System.Windows.Forms.GroupBox();
            this.drawImageHeightUnit = new System.Windows.Forms.Label();
            this.drawImageWidthUnit = new System.Windows.Forms.Label();
            this.drawImageHeightValue = new System.Windows.Forms.Label();
            this.drawImageHeightLabel = new System.Windows.Forms.Label();
            this.drawImageWidthValue = new System.Windows.Forms.Label();
            this.drawImageWidthLabel = new System.Windows.Forms.Label();
            this.syntheticLsfProfilePictureBox = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mtfResultGroup = new System.Windows.Forms.GroupBox();
            this.lsfPeakLabel = new System.Windows.Forms.Label();
            this.lsfPeakValue = new System.Windows.Forms.Label();
            this.mtfLabel = new System.Windows.Forms.Label();
            this.mtfValue = new System.Windows.Forms.Label();
            this.slantedEdgeAngleUnit = new System.Windows.Forms.Label();
            this.slantedEdgeAngleValue = new System.Windows.Forms.Label();
            this.syntheticLsfProfileGroup = new System.Windows.Forms.GroupBox();
            this.esfProfileGroup = new System.Windows.Forms.GroupBox();
            this.esfProfilePictureBox = new System.Windows.Forms.PictureBox();
            this.imagePathLabel = new System.Windows.Forms.Label();
            this.edgeImageGroup = new System.Windows.Forms.GroupBox();
            this.edgeImagePictureBox = new System.Windows.Forms.PictureBox();
            this.mtfProfileGroup = new System.Windows.Forms.GroupBox();
            this.mtfProfilePictureBox = new System.Windows.Forms.PictureBox();
            this.resultDataPictureBox = new System.Windows.Forms.PictureBox();
            this.mtfBackGroundPictureBox = new System.Windows.Forms.PictureBox();
            this.stopwatchLabel = new System.Windows.Forms.Label();
            this.rawImagePictureBox = new System.Windows.Forms.PictureBox();
            this.rawImageGroup = new System.Windows.Forms.GroupBox();
            this.roiImageGroup = new System.Windows.Forms.GroupBox();
            this.roiImagePictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cannyMethodRadioButton = new System.Windows.Forms.RadioButton();
            this.peterBurnsMethodRadioButton = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.imagePictureBox)).BeginInit();
            this.readImageGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.syntheticLsfProfilePictureBox)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.mtfResultGroup.SuspendLayout();
            this.syntheticLsfProfileGroup.SuspendLayout();
            this.esfProfileGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.esfProfilePictureBox)).BeginInit();
            this.edgeImageGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edgeImagePictureBox)).BeginInit();
            this.mtfProfileGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mtfProfilePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultDataPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mtfBackGroundPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rawImagePictureBox)).BeginInit();
            this.rawImageGroup.SuspendLayout();
            this.roiImageGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.roiImagePictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imagePictureBox
            // 
            this.imagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imagePictureBox.Location = new System.Drawing.Point(0, 0);
            this.imagePictureBox.Name = "imagePictureBox";
            this.imagePictureBox.Size = new System.Drawing.Size(480, 480);
            this.imagePictureBox.TabIndex = 0;
            this.imagePictureBox.TabStop = false;
            // 
            // imageFilePath
            // 
            this.imageFilePath.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.imageFilePath.Location = new System.Drawing.Point(100, 488);
            this.imageFilePath.Name = "imageFilePath";
            this.imageFilePath.ReadOnly = true;
            this.imageFilePath.Size = new System.Drawing.Size(334, 23);
            this.imageFilePath.TabIndex = 1;
            // 
            // openImageFileButton
            // 
            this.openImageFileButton.Location = new System.Drawing.Point(440, 486);
            this.openImageFileButton.Name = "openImageFileButton";
            this.openImageFileButton.Size = new System.Drawing.Size(40, 25);
            this.openImageFileButton.TabIndex = 2;
            this.openImageFileButton.Text = "...";
            this.openImageFileButton.UseVisualStyleBackColor = true;
            this.openImageFileButton.Click += new System.EventHandler(this.openImageFileButton_Click);
            // 
            // calculationButton
            // 
            this.calculationButton.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.calculationButton.Location = new System.Drawing.Point(405, 586);
            this.calculationButton.Name = "calculationButton";
            this.calculationButton.Size = new System.Drawing.Size(75, 75);
            this.calculationButton.TabIndex = 3;
            this.calculationButton.Text = ">>";
            this.calculationButton.UseVisualStyleBackColor = true;
            this.calculationButton.Click += new System.EventHandler(this.calculationButton_Click);
            // 
            // userOptionPropertyGrid
            // 
            this.userOptionPropertyGrid.Location = new System.Drawing.Point(204, 517);
            this.userOptionPropertyGrid.Name = "userOptionPropertyGrid";
            this.userOptionPropertyGrid.Size = new System.Drawing.Size(188, 262);
            this.userOptionPropertyGrid.TabIndex = 4;
            this.userOptionPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.userOptionPropertyGrid_PropertyValueChanged);
            // 
            // readImageGroup
            // 
            this.readImageGroup.Controls.Add(this.drawImageHeightUnit);
            this.readImageGroup.Controls.Add(this.drawImageWidthUnit);
            this.readImageGroup.Controls.Add(this.drawImageHeightValue);
            this.readImageGroup.Controls.Add(this.drawImageHeightLabel);
            this.readImageGroup.Controls.Add(this.drawImageWidthValue);
            this.readImageGroup.Controls.Add(this.drawImageWidthLabel);
            this.readImageGroup.Location = new System.Drawing.Point(10, 541);
            this.readImageGroup.Name = "readImageGroup";
            this.readImageGroup.Size = new System.Drawing.Size(188, 75);
            this.readImageGroup.TabIndex = 5;
            this.readImageGroup.TabStop = false;
            this.readImageGroup.Text = "Read Image";
            // 
            // drawImageHeightUnit
            // 
            this.drawImageHeightUnit.AutoSize = true;
            this.drawImageHeightUnit.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.drawImageHeightUnit.Location = new System.Drawing.Point(116, 45);
            this.drawImageHeightUnit.Name = "drawImageHeightUnit";
            this.drawImageHeightUnit.Size = new System.Drawing.Size(46, 16);
            this.drawImageHeightUnit.TabIndex = 1;
            this.drawImageHeightUnit.Text = "[pixel]";
            // 
            // drawImageWidthUnit
            // 
            this.drawImageWidthUnit.AutoSize = true;
            this.drawImageWidthUnit.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.drawImageWidthUnit.Location = new System.Drawing.Point(116, 24);
            this.drawImageWidthUnit.Name = "drawImageWidthUnit";
            this.drawImageWidthUnit.Size = new System.Drawing.Size(46, 16);
            this.drawImageWidthUnit.TabIndex = 1;
            this.drawImageWidthUnit.Text = "[pixel]";
            // 
            // drawImageHeightValue
            // 
            this.drawImageHeightValue.AutoSize = true;
            this.drawImageHeightValue.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.drawImageHeightValue.Location = new System.Drawing.Point(70, 45);
            this.drawImageHeightValue.Name = "drawImageHeightValue";
            this.drawImageHeightValue.Size = new System.Drawing.Size(39, 16);
            this.drawImageHeightValue.TabIndex = 1;
            this.drawImageHeightValue.Text = "0000";
            this.drawImageHeightValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drawImageHeightLabel
            // 
            this.drawImageHeightLabel.AutoSize = true;
            this.drawImageHeightLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.drawImageHeightLabel.Location = new System.Drawing.Point(6, 45);
            this.drawImageHeightLabel.Name = "drawImageHeightLabel";
            this.drawImageHeightLabel.Size = new System.Drawing.Size(57, 16);
            this.drawImageHeightLabel.TabIndex = 0;
            this.drawImageHeightLabel.Text = "Height :";
            // 
            // drawImageWidthValue
            // 
            this.drawImageWidthValue.AutoSize = true;
            this.drawImageWidthValue.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.drawImageWidthValue.Location = new System.Drawing.Point(70, 24);
            this.drawImageWidthValue.Name = "drawImageWidthValue";
            this.drawImageWidthValue.Size = new System.Drawing.Size(39, 16);
            this.drawImageWidthValue.TabIndex = 1;
            this.drawImageWidthValue.Text = "0000";
            this.drawImageWidthValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // drawImageWidthLabel
            // 
            this.drawImageWidthLabel.AutoSize = true;
            this.drawImageWidthLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.drawImageWidthLabel.Location = new System.Drawing.Point(6, 24);
            this.drawImageWidthLabel.Name = "drawImageWidthLabel";
            this.drawImageWidthLabel.Size = new System.Drawing.Size(52, 16);
            this.drawImageWidthLabel.TabIndex = 0;
            this.drawImageWidthLabel.Text = "Width :";
            // 
            // syntheticLsfProfilePictureBox
            // 
            this.syntheticLsfProfilePictureBox.ContextMenuStrip = this.contextMenuStrip1;
            this.syntheticLsfProfilePictureBox.Location = new System.Drawing.Point(6, 18);
            this.syntheticLsfProfilePictureBox.Name = "syntheticLsfProfilePictureBox";
            this.syntheticLsfProfilePictureBox.Size = new System.Drawing.Size(500, 200);
            this.syntheticLsfProfilePictureBox.TabIndex = 6;
            this.syntheticLsfProfilePictureBox.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportCSVToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(132, 26);
            // 
            // exportCSVToolStripMenuItem
            // 
            this.exportCSVToolStripMenuItem.Name = "exportCSVToolStripMenuItem";
            this.exportCSVToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.exportCSVToolStripMenuItem.Text = "Export CSV";
            this.exportCSVToolStripMenuItem.Click += new System.EventHandler(this.exportCSVToolStripMenuItem_Click);
            // 
            // mtfResultGroup
            // 
            this.mtfResultGroup.Controls.Add(this.lsfPeakLabel);
            this.mtfResultGroup.Controls.Add(this.lsfPeakValue);
            this.mtfResultGroup.Controls.Add(this.mtfLabel);
            this.mtfResultGroup.Controls.Add(this.mtfValue);
            this.mtfResultGroup.Location = new System.Drawing.Point(486, 7);
            this.mtfResultGroup.Name = "mtfResultGroup";
            this.mtfResultGroup.Size = new System.Drawing.Size(731, 60);
            this.mtfResultGroup.TabIndex = 7;
            this.mtfResultGroup.TabStop = false;
            this.mtfResultGroup.Text = "Result";
            // 
            // lsfPeakLabel
            // 
            this.lsfPeakLabel.AutoSize = true;
            this.lsfPeakLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lsfPeakLabel.Location = new System.Drawing.Point(221, 23);
            this.lsfPeakLabel.Name = "lsfPeakLabel";
            this.lsfPeakLabel.Size = new System.Drawing.Size(85, 16);
            this.lsfPeakLabel.TabIndex = 3;
            this.lsfPeakLabel.Text = "LSF Peak:";
            // 
            // lsfPeakValue
            // 
            this.lsfPeakValue.AutoSize = true;
            this.lsfPeakValue.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lsfPeakValue.Location = new System.Drawing.Point(313, 18);
            this.lsfPeakValue.Name = "lsfPeakValue";
            this.lsfPeakValue.Size = new System.Drawing.Size(99, 24);
            this.lsfPeakValue.TabIndex = 2;
            this.lsfPeakValue.Text = "0.000000";
            // 
            // mtfLabel
            // 
            this.mtfLabel.AutoSize = true;
            this.mtfLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mtfLabel.Location = new System.Drawing.Point(16, 23);
            this.mtfLabel.Name = "mtfLabel";
            this.mtfLabel.Size = new System.Drawing.Size(44, 16);
            this.mtfLabel.TabIndex = 1;
            this.mtfLabel.Text = "MTF:";
            // 
            // mtfValue
            // 
            this.mtfValue.AutoSize = true;
            this.mtfValue.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mtfValue.Location = new System.Drawing.Point(63, 18);
            this.mtfValue.Name = "mtfValue";
            this.mtfValue.Size = new System.Drawing.Size(99, 24);
            this.mtfValue.TabIndex = 0;
            this.mtfValue.Text = "0.000000";
            // 
            // slantedEdgeAngleUnit
            // 
            this.slantedEdgeAngleUnit.AutoSize = true;
            this.slantedEdgeAngleUnit.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.slantedEdgeAngleUnit.Location = new System.Drawing.Point(133, 224);
            this.slantedEdgeAngleUnit.Name = "slantedEdgeAngleUnit";
            this.slantedEdgeAngleUnit.Size = new System.Drawing.Size(45, 16);
            this.slantedEdgeAngleUnit.TabIndex = 4;
            this.slantedEdgeAngleUnit.Text = "[deg]";
            // 
            // slantedEdgeAngleValue
            // 
            this.slantedEdgeAngleValue.AutoSize = true;
            this.slantedEdgeAngleValue.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.slantedEdgeAngleValue.Location = new System.Drawing.Point(36, 221);
            this.slantedEdgeAngleValue.Name = "slantedEdgeAngleValue";
            this.slantedEdgeAngleValue.Size = new System.Drawing.Size(91, 19);
            this.slantedEdgeAngleValue.TabIndex = 3;
            this.slantedEdgeAngleValue.Text = "-00.0000";
            // 
            // syntheticLsfProfileGroup
            // 
            this.syntheticLsfProfileGroup.Controls.Add(this.syntheticLsfProfilePictureBox);
            this.syntheticLsfProfileGroup.Location = new System.Drawing.Point(704, 303);
            this.syntheticLsfProfileGroup.Name = "syntheticLsfProfileGroup";
            this.syntheticLsfProfileGroup.Size = new System.Drawing.Size(513, 224);
            this.syntheticLsfProfileGroup.TabIndex = 8;
            this.syntheticLsfProfileGroup.TabStop = false;
            this.syntheticLsfProfileGroup.Text = "Synthetic LSF(Line Spread Function) Profile";
            // 
            // esfProfileGroup
            // 
            this.esfProfileGroup.Controls.Add(this.esfProfilePictureBox);
            this.esfProfileGroup.Location = new System.Drawing.Point(704, 73);
            this.esfProfileGroup.Name = "esfProfileGroup";
            this.esfProfileGroup.Size = new System.Drawing.Size(513, 224);
            this.esfProfileGroup.TabIndex = 9;
            this.esfProfileGroup.TabStop = false;
            this.esfProfileGroup.Text = "ESF(Edge Spread Function) Profile";
            // 
            // esfProfilePictureBox
            // 
            this.esfProfilePictureBox.ContextMenuStrip = this.contextMenuStrip1;
            this.esfProfilePictureBox.Location = new System.Drawing.Point(6, 18);
            this.esfProfilePictureBox.Name = "esfProfilePictureBox";
            this.esfProfilePictureBox.Size = new System.Drawing.Size(500, 200);
            this.esfProfilePictureBox.TabIndex = 0;
            this.esfProfilePictureBox.TabStop = false;
            // 
            // imagePathLabel
            // 
            this.imagePathLabel.AutoSize = true;
            this.imagePathLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.imagePathLabel.Location = new System.Drawing.Point(7, 491);
            this.imagePathLabel.Name = "imagePathLabel";
            this.imagePathLabel.Size = new System.Drawing.Size(86, 16);
            this.imagePathLabel.TabIndex = 10;
            this.imagePathLabel.Text = "Image Path:";
            // 
            // edgeImageGroup
            // 
            this.edgeImageGroup.Controls.Add(this.slantedEdgeAngleUnit);
            this.edgeImageGroup.Controls.Add(this.edgeImagePictureBox);
            this.edgeImageGroup.Controls.Add(this.slantedEdgeAngleValue);
            this.edgeImageGroup.Location = new System.Drawing.Point(486, 533);
            this.edgeImageGroup.Name = "edgeImageGroup";
            this.edgeImageGroup.Size = new System.Drawing.Size(212, 246);
            this.edgeImageGroup.TabIndex = 11;
            this.edgeImageGroup.TabStop = false;
            this.edgeImageGroup.Text = "Edge Detection Image";
            // 
            // edgeImagePictureBox
            // 
            this.edgeImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.edgeImagePictureBox.Location = new System.Drawing.Point(6, 18);
            this.edgeImagePictureBox.Name = "edgeImagePictureBox";
            this.edgeImagePictureBox.Size = new System.Drawing.Size(200, 200);
            this.edgeImagePictureBox.TabIndex = 0;
            this.edgeImagePictureBox.TabStop = false;
            // 
            // mtfProfileGroup
            // 
            this.mtfProfileGroup.Controls.Add(this.mtfProfilePictureBox);
            this.mtfProfileGroup.Controls.Add(this.resultDataPictureBox);
            this.mtfProfileGroup.Controls.Add(this.mtfBackGroundPictureBox);
            this.mtfProfileGroup.Location = new System.Drawing.Point(704, 533);
            this.mtfProfileGroup.Name = "mtfProfileGroup";
            this.mtfProfileGroup.Size = new System.Drawing.Size(618, 407);
            this.mtfProfileGroup.TabIndex = 12;
            this.mtfProfileGroup.TabStop = false;
            this.mtfProfileGroup.Text = "MTF(Modulated Transfer Function) Profile";
            // 
            // mtfProfilePictureBox
            // 
            this.mtfProfilePictureBox.BackColor = System.Drawing.Color.Transparent;
            this.mtfProfilePictureBox.Location = new System.Drawing.Point(0, 0);
            this.mtfProfilePictureBox.Name = "mtfProfilePictureBox";
            this.mtfProfilePictureBox.Size = new System.Drawing.Size(600, 380);
            this.mtfProfilePictureBox.TabIndex = 0;
            this.mtfProfilePictureBox.TabStop = false;
            // 
            // resultDataPictureBox
            // 
            this.resultDataPictureBox.Location = new System.Drawing.Point(0, 0);
            this.resultDataPictureBox.Name = "resultDataPictureBox";
            this.resultDataPictureBox.Size = new System.Drawing.Size(600, 380);
            this.resultDataPictureBox.TabIndex = 16;
            this.resultDataPictureBox.TabStop = false;
            // 
            // mtfBackGroundPictureBox
            // 
            this.mtfBackGroundPictureBox.ContextMenuStrip = this.contextMenuStrip1;
            this.mtfBackGroundPictureBox.Location = new System.Drawing.Point(6, 18);
            this.mtfBackGroundPictureBox.Name = "mtfBackGroundPictureBox";
            this.mtfBackGroundPictureBox.Size = new System.Drawing.Size(600, 380);
            this.mtfBackGroundPictureBox.TabIndex = 16;
            this.mtfBackGroundPictureBox.TabStop = false;
            // 
            // stopwatchLabel
            // 
            this.stopwatchLabel.AutoSize = true;
            this.stopwatchLabel.Location = new System.Drawing.Point(403, 669);
            this.stopwatchLabel.Name = "stopwatchLabel";
            this.stopwatchLabel.Size = new System.Drawing.Size(48, 12);
            this.stopwatchLabel.TabIndex = 13;
            this.stopwatchLabel.Text = "0.00[ms]";
            // 
            // rawImagePictureBox
            // 
            this.rawImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rawImagePictureBox.Location = new System.Drawing.Point(6, 18);
            this.rawImagePictureBox.Name = "rawImagePictureBox";
            this.rawImagePictureBox.Size = new System.Drawing.Size(200, 200);
            this.rawImagePictureBox.TabIndex = 14;
            this.rawImagePictureBox.TabStop = false;
            // 
            // rawImageGroup
            // 
            this.rawImageGroup.Controls.Add(this.rawImagePictureBox);
            this.rawImageGroup.Location = new System.Drawing.Point(486, 73);
            this.rawImageGroup.Name = "rawImageGroup";
            this.rawImageGroup.Size = new System.Drawing.Size(212, 224);
            this.rawImageGroup.TabIndex = 15;
            this.rawImageGroup.TabStop = false;
            this.rawImageGroup.Text = "RAW Image";
            // 
            // roiImageGroup
            // 
            this.roiImageGroup.Controls.Add(this.roiImagePictureBox);
            this.roiImageGroup.Location = new System.Drawing.Point(486, 303);
            this.roiImageGroup.Name = "roiImageGroup";
            this.roiImageGroup.Size = new System.Drawing.Size(212, 224);
            this.roiImageGroup.TabIndex = 15;
            this.roiImageGroup.TabStop = false;
            this.roiImageGroup.Text = "RoI Image";
            // 
            // roiImagePictureBox
            // 
            this.roiImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.roiImagePictureBox.Location = new System.Drawing.Point(6, 18);
            this.roiImagePictureBox.Name = "roiImagePictureBox";
            this.roiImagePictureBox.Size = new System.Drawing.Size(200, 200);
            this.roiImagePictureBox.TabIndex = 14;
            this.roiImagePictureBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cannyMethodRadioButton);
            this.groupBox1.Controls.Add(this.peterBurnsMethodRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(10, 634);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 84);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MTF Calculation Method";
            // 
            // cannyMethodRadioButton
            // 
            this.cannyMethodRadioButton.AutoSize = true;
            this.cannyMethodRadioButton.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cannyMethodRadioButton.Location = new System.Drawing.Point(15, 20);
            this.cannyMethodRadioButton.Name = "cannyMethodRadioButton";
            this.cannyMethodRadioButton.Size = new System.Drawing.Size(60, 17);
            this.cannyMethodRadioButton.TabIndex = 1;
            this.cannyMethodRadioButton.TabStop = true;
            this.cannyMethodRadioButton.Text = "Canny";
            this.cannyMethodRadioButton.UseVisualStyleBackColor = true;
            this.cannyMethodRadioButton.CheckedChanged += new System.EventHandler(this.cannyMethodRadioButton_CheckedChanged);
            // 
            // peterBurnsMethodRadioButton
            // 
            this.peterBurnsMethodRadioButton.AutoSize = true;
            this.peterBurnsMethodRadioButton.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.peterBurnsMethodRadioButton.Location = new System.Drawing.Point(15, 52);
            this.peterBurnsMethodRadioButton.Name = "peterBurnsMethodRadioButton";
            this.peterBurnsMethodRadioButton.Size = new System.Drawing.Size(93, 17);
            this.peterBurnsMethodRadioButton.TabIndex = 2;
            this.peterBurnsMethodRadioButton.TabStop = true;
            this.peterBurnsMethodRadioButton.Text = "Peter.Burns";
            this.peterBurnsMethodRadioButton.UseVisualStyleBackColor = true;
            this.peterBurnsMethodRadioButton.CheckedChanged += new System.EventHandler(this.peterBurnsMethodRadioButton_CheckedChanged);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1342, 957);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mtfProfileGroup);
            this.Controls.Add(this.roiImageGroup);
            this.Controls.Add(this.rawImageGroup);
            this.Controls.Add(this.stopwatchLabel);
            this.Controls.Add(this.edgeImageGroup);
            this.Controls.Add(this.imagePathLabel);
            this.Controls.Add(this.esfProfileGroup);
            this.Controls.Add(this.syntheticLsfProfileGroup);
            this.Controls.Add(this.mtfResultGroup);
            this.Controls.Add(this.readImageGroup);
            this.Controls.Add(this.userOptionPropertyGrid);
            this.Controls.Add(this.calculationButton);
            this.Controls.Add(this.openImageFileButton);
            this.Controls.Add(this.imageFilePath);
            this.Controls.Add(this.imagePictureBox);
            this.Name = "TestForm";
            this.Text = "MTF Debug";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.imagePictureBox)).EndInit();
            this.readImageGroup.ResumeLayout(false);
            this.readImageGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.syntheticLsfProfilePictureBox)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.mtfResultGroup.ResumeLayout(false);
            this.mtfResultGroup.PerformLayout();
            this.syntheticLsfProfileGroup.ResumeLayout(false);
            this.esfProfileGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.esfProfilePictureBox)).EndInit();
            this.edgeImageGroup.ResumeLayout(false);
            this.edgeImageGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edgeImagePictureBox)).EndInit();
            this.mtfProfileGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mtfProfilePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultDataPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mtfBackGroundPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rawImagePictureBox)).EndInit();
            this.rawImageGroup.ResumeLayout(false);
            this.roiImageGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.roiImagePictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imagePictureBox;
        private System.Windows.Forms.TextBox imageFilePath;
        private System.Windows.Forms.Button openImageFileButton;
        private System.Windows.Forms.Button calculationButton;
        private System.Windows.Forms.PropertyGrid userOptionPropertyGrid;
        private System.Windows.Forms.GroupBox readImageGroup;
        private System.Windows.Forms.Label drawImageHeightUnit;
        private System.Windows.Forms.Label drawImageWidthUnit;
        private System.Windows.Forms.Label drawImageHeightValue;
        private System.Windows.Forms.Label drawImageHeightLabel;
        private System.Windows.Forms.Label drawImageWidthValue;
        private System.Windows.Forms.Label drawImageWidthLabel;
        private System.Windows.Forms.PictureBox syntheticLsfProfilePictureBox;
        private System.Windows.Forms.GroupBox mtfResultGroup;
        private System.Windows.Forms.GroupBox syntheticLsfProfileGroup;
        private System.Windows.Forms.GroupBox esfProfileGroup;
        private System.Windows.Forms.PictureBox esfProfilePictureBox;
        private System.Windows.Forms.Label mtfValue;
        private System.Windows.Forms.Label imagePathLabel;
        private System.Windows.Forms.Label slantedEdgeAngleUnit;
        private System.Windows.Forms.Label slantedEdgeAngleValue;
        private System.Windows.Forms.Label mtfLabel;
        private System.Windows.Forms.GroupBox edgeImageGroup;
        private System.Windows.Forms.PictureBox edgeImagePictureBox;
        private System.Windows.Forms.GroupBox mtfProfileGroup;
        private System.Windows.Forms.PictureBox mtfProfilePictureBox;
        private System.Windows.Forms.Label stopwatchLabel;
        private System.Windows.Forms.PictureBox rawImagePictureBox;
        private System.Windows.Forms.GroupBox rawImageGroup;
        private System.Windows.Forms.GroupBox roiImageGroup;
        private System.Windows.Forms.PictureBox roiImagePictureBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportCSVToolStripMenuItem;
        private System.Windows.Forms.Label lsfPeakLabel;
        private System.Windows.Forms.Label lsfPeakValue;
        private System.Windows.Forms.PictureBox resultDataPictureBox;
        private System.Windows.Forms.PictureBox mtfBackGroundPictureBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton cannyMethodRadioButton;
        private System.Windows.Forms.RadioButton peterBurnsMethodRadioButton;
    }
}

