namespace WavefrontTester
{
    partial class MainForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.loadOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateDefaultCalibration = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDefaultCalibration = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDefaultCalibration = new System.Windows.Forms.ToolStripMenuItem();
            this.userCalibrationMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.generateUserCalibration = new System.Windows.Forms.ToolStripMenuItem();
            this.saveUserCalibration = new System.Windows.Forms.ToolStripMenuItem();
            this.loadUserCalibration = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawSpotPairMap = new System.Windows.Forms.ToolStripMenuItem();
            this.drawSpotProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.drawPairingArea = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.stopDrawing = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlViews = new System.Windows.Forms.TabControl();
            this.tabPageSpotView = new System.Windows.Forms.TabPage();
            this.tabPageIntensityView = new System.Windows.Forms.TabPage();
            this.tabPageWavefrontView = new System.Windows.Forms.TabPage();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.resultView = new System.Windows.Forms.TabControl();
            this.zernikeTab = new System.Windows.Forms.TabPage();
            this.zernikeView = new System.Windows.Forms.DataGridView();
            this.captionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.enabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.zernikeEnabledMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zernikeCheckAll = new System.Windows.Forms.ToolStripMenuItem();
            this.zernikeUncheckAll = new System.Windows.Forms.ToolStripMenuItem();
            this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zernikeComponentsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.legendreTab = new System.Windows.Forms.TabPage();
            this.legendreView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.legendreEnabledMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.legendreCheckAll = new System.Windows.Forms.ToolStripMenuItem();
            this.legendreUncheckAll = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.legendreComponentsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.seidelTab = new System.Windows.Forms.TabPage();
            this.seidelPanel = new System.Windows.Forms.Panel();
            this.astigmatismAngleValue = new System.Windows.Forms.Label();
            this.astigmatismMagnitudeValue = new System.Windows.Forms.Label();
            this.comaAngleValue = new System.Windows.Forms.Label();
            this.comaMagnitudeValue = new System.Windows.Forms.Label();
            this.sphericalAberrationValue = new System.Windows.Forms.Label();
            this.defocusValue = new System.Windows.Forms.Label();
            this.tiltAngleValue = new System.Windows.Forms.Label();
            this.tiltMagnitudeValue = new System.Windows.Forms.Label();
            this.astigmatismLabel = new System.Windows.Forms.Label();
            this.comaLabel = new System.Windows.Forms.Label();
            this.sphericalAberraLabel = new System.Windows.Forms.Label();
            this.defocusLabel = new System.Windows.Forms.Label();
            this.astigmatismAngleLabel = new System.Windows.Forms.Label();
            this.comaAngleLabel = new System.Windows.Forms.Label();
            this.tiltAngleLabel = new System.Windows.Forms.Label();
            this.astigmatismMagnitudeLabel = new System.Windows.Forms.Label();
            this.comaMagnitudeLabel = new System.Windows.Forms.Label();
            this.tiltMagnitudeLabel = new System.Windows.Forms.Label();
            this.tiltLabel = new System.Windows.Forms.Label();
            this.totalTab = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rhoYValue = new System.Windows.Forms.Label();
            this.rhoYLabel = new System.Windows.Forms.Label();
            this.rhoXLabel = new System.Windows.Forms.Label();
            this.rocYValue = new System.Windows.Forms.Label();
            this.rocYLabel = new System.Windows.Forms.Label();
            this.rocXLabel = new System.Windows.Forms.Label();
            this.strehlRatioLabel = new System.Windows.Forms.Label();
            this.pvLabel = new System.Windows.Forms.Label();
            this.incidentAngleSyntheticValue = new System.Windows.Forms.Label();
            this.incidentAngleSyntheticLabel = new System.Windows.Forms.Label();
            this.strehlRatioValue = new System.Windows.Forms.Label();
            this.rmsValue = new System.Windows.Forms.Label();
            this.pvValue = new System.Windows.Forms.Label();
            this.rhoXValue = new System.Windows.Forms.Label();
            this.rocXValue = new System.Windows.Forms.Label();
            this.powerValue = new System.Windows.Forms.Label();
            this.incidentAngleYValue = new System.Windows.Forms.Label();
            this.incidentAngleXValue = new System.Windows.Forms.Label();
            this.rmsLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.powerLabel = new System.Windows.Forms.Label();
            this.incidentAngleYLabel = new System.Windows.Forms.Label();
            this.incidentAngleXLabel = new System.Windows.Forms.Label();
            this.incidentAngleLabel = new System.Windows.Forms.Label();
            this.optionTab = new System.Windows.Forms.TabPage();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.statusTab = new System.Windows.Forms.TabPage();
            this.statusGrid = new System.Windows.Forms.PropertyGrid();
            this.logTab = new System.Windows.Forms.TabPage();
            this.logPanel = new System.Windows.Forms.Panel();
            this.filePathLabel = new System.Windows.Forms.Label();
            this.saveContinuous = new System.Windows.Forms.RadioButton();
            this.loggingCount = new System.Windows.Forms.NumericUpDown();
            this.logDescription = new System.Windows.Forms.DataGridView();
            this.Caption = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Contents = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filePathButton = new System.Windows.Forms.Button();
            this.saveOnce = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.saveLogButton = new System.Windows.Forms.Button();
            this.loggingInterval = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.filePath = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.framesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.pairCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.resultIntervalLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.calibrationFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.deviceListPanel = new System.Windows.Forms.Panel();
            this.deviceListView = new System.Windows.Forms.DataGridView();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.checkMeasure = new System.Windows.Forms.CheckBox();
            this.checkCalibration = new System.Windows.Forms.CheckBox();
            this.checkOpen = new System.Windows.Forms.CheckBox();
            this.deviceButton = new System.Windows.Forms.Button();
            this.snapshotButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.menuStripMain.SuspendLayout();
            this.tabControlViews.SuspendLayout();
            this.resultView.SuspendLayout();
            this.zernikeTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zernikeView)).BeginInit();
            this.zernikeEnabledMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zernikeComponentsBindingSource)).BeginInit();
            this.legendreTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.legendreView)).BeginInit();
            this.legendreEnabledMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.legendreComponentsBindingSource)).BeginInit();
            this.seidelTab.SuspendLayout();
            this.seidelPanel.SuspendLayout();
            this.totalTab.SuspendLayout();
            this.panel1.SuspendLayout();
            this.optionTab.SuspendLayout();
            this.statusTab.SuspendLayout();
            this.logTab.SuspendLayout();
            this.logPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loggingCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loggingInterval)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.deviceListPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.deviceListView)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.calibrationToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1664, 24);
            this.menuStripMain.TabIndex = 2;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveOptions,
            this.loadOptions});
            this.optionsToolStripMenuItem.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // saveOptions
            // 
            this.saveOptions.Name = "saveOptions";
            this.saveOptions.Size = new System.Drawing.Size(103, 22);
            this.saveOptions.Text = "Save";
            this.saveOptions.Click += new System.EventHandler(this.saveOptionsToolStripMenuItem_Click);
            // 
            // loadOptions
            // 
            this.loadOptions.Name = "loadOptions";
            this.loadOptions.Size = new System.Drawing.Size(103, 22);
            this.loadOptions.Text = "Load";
            this.loadOptions.Click += new System.EventHandler(this.loadOptions_Click);
            // 
            // calibrationToolStripMenuItem
            // 
            this.calibrationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultToolStripMenuItem,
            this.userCalibrationMenu});
            this.calibrationToolStripMenuItem.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.calibrationToolStripMenuItem.Name = "calibrationToolStripMenuItem";
            this.calibrationToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.calibrationToolStripMenuItem.Text = "Calibration";
            // 
            // defaultToolStripMenuItem
            // 
            this.defaultToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateDefaultCalibration,
            this.saveDefaultCalibration,
            this.loadDefaultCalibration});
            this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            this.defaultToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.defaultToolStripMenuItem.Text = "Default";
            // 
            // generateDefaultCalibration
            // 
            this.generateDefaultCalibration.Name = "generateDefaultCalibration";
            this.generateDefaultCalibration.Size = new System.Drawing.Size(128, 22);
            this.generateDefaultCalibration.Text = "Generate";
            this.generateDefaultCalibration.Click += new System.EventHandler(this.generateDefaultCalibration_Click);
            // 
            // saveDefaultCalibration
            // 
            this.saveDefaultCalibration.Name = "saveDefaultCalibration";
            this.saveDefaultCalibration.Size = new System.Drawing.Size(128, 22);
            this.saveDefaultCalibration.Text = "Save";
            this.saveDefaultCalibration.Click += new System.EventHandler(this.saveDefaultCalibration_Click);
            // 
            // loadDefaultCalibration
            // 
            this.loadDefaultCalibration.Name = "loadDefaultCalibration";
            this.loadDefaultCalibration.Size = new System.Drawing.Size(128, 22);
            this.loadDefaultCalibration.Text = "Load";
            this.loadDefaultCalibration.Click += new System.EventHandler(this.loadDefaultCalibration_Click);
            // 
            // userCalibrationMenu
            // 
            this.userCalibrationMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateUserCalibration,
            this.saveUserCalibration,
            this.loadUserCalibration});
            this.userCalibrationMenu.Name = "userCalibrationMenu";
            this.userCalibrationMenu.Size = new System.Drawing.Size(116, 22);
            this.userCalibrationMenu.Text = "User";
            // 
            // generateUserCalibration
            // 
            this.generateUserCalibration.Name = "generateUserCalibration";
            this.generateUserCalibration.Size = new System.Drawing.Size(128, 22);
            this.generateUserCalibration.Text = "Generate";
            this.generateUserCalibration.Click += new System.EventHandler(this.generateUserCalibration_Click);
            // 
            // saveUserCalibration
            // 
            this.saveUserCalibration.Name = "saveUserCalibration";
            this.saveUserCalibration.Size = new System.Drawing.Size(128, 22);
            this.saveUserCalibration.Text = "Save";
            this.saveUserCalibration.Click += new System.EventHandler(this.saveUserCalibration_Click);
            // 
            // loadUserCalibration
            // 
            this.loadUserCalibration.Name = "loadUserCalibration";
            this.loadUserCalibration.Size = new System.Drawing.Size(128, 22);
            this.loadUserCalibration.Text = "Load";
            this.loadUserCalibration.Click += new System.EventHandler(this.loadUserCalibration_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spotToolStripMenuItem,
            this.toolStripSeparator1,
            this.stopDrawing});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // spotToolStripMenuItem
            // 
            this.spotToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawSpotPairMap,
            this.drawSpotProfile,
            this.drawPairingArea});
            this.spotToolStripMenuItem.Name = "spotToolStripMenuItem";
            this.spotToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.spotToolStripMenuItem.Text = "Spot";
            // 
            // drawSpotPairMap
            // 
            this.drawSpotPairMap.CheckOnClick = true;
            this.drawSpotPairMap.Name = "drawSpotPairMap";
            this.drawSpotPairMap.Size = new System.Drawing.Size(138, 22);
            this.drawSpotPairMap.Text = "Pair Map";
            this.drawSpotPairMap.Click += new System.EventHandler(this.drawSpotPairMap_Click);
            // 
            // drawSpotProfile
            // 
            this.drawSpotProfile.CheckOnClick = true;
            this.drawSpotProfile.Name = "drawSpotProfile";
            this.drawSpotProfile.Size = new System.Drawing.Size(138, 22);
            this.drawSpotProfile.Text = "Profile";
            this.drawSpotProfile.Click += new System.EventHandler(this.drawSpotProfile_Click);
            // 
            // drawPairingArea
            // 
            this.drawPairingArea.CheckOnClick = true;
            this.drawPairingArea.Name = "drawPairingArea";
            this.drawPairingArea.Size = new System.Drawing.Size(138, 22);
            this.drawPairingArea.Text = "Pairing Area";
            this.drawPairingArea.Click += new System.EventHandler(this.drawPairingArea_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(141, 6);
            // 
            // stopDrawing
            // 
            this.stopDrawing.CheckOnClick = true;
            this.stopDrawing.Name = "stopDrawing";
            this.stopDrawing.Size = new System.Drawing.Size(144, 22);
            this.stopDrawing.Text = "Stop drawing";
            this.stopDrawing.Click += new System.EventHandler(this.stopDrawing_Click);
            // 
            // tabControlViews
            // 
            this.tabControlViews.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlViews.Controls.Add(this.tabPageSpotView);
            this.tabControlViews.Controls.Add(this.tabPageIntensityView);
            this.tabControlViews.Controls.Add(this.tabPageWavefrontView);
            this.tabControlViews.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabControlViews.Location = new System.Drawing.Point(472, 27);
            this.tabControlViews.Name = "tabControlViews";
            this.tabControlViews.SelectedIndex = 0;
            this.tabControlViews.Size = new System.Drawing.Size(1180, 805);
            this.tabControlViews.TabIndex = 5;
            // 
            // tabPageSpotView
            // 
            this.tabPageSpotView.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageSpotView.Location = new System.Drawing.Point(4, 26);
            this.tabPageSpotView.Name = "tabPageSpotView";
            this.tabPageSpotView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSpotView.Size = new System.Drawing.Size(1172, 775);
            this.tabPageSpotView.TabIndex = 0;
            this.tabPageSpotView.Text = "Spot";
            // 
            // tabPageIntensityView
            // 
            this.tabPageIntensityView.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageIntensityView.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPageIntensityView.Location = new System.Drawing.Point(4, 26);
            this.tabPageIntensityView.Name = "tabPageIntensityView";
            this.tabPageIntensityView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIntensityView.Size = new System.Drawing.Size(1172, 775);
            this.tabPageIntensityView.TabIndex = 1;
            this.tabPageIntensityView.Text = "Intensity";
            // 
            // tabPageWavefrontView
            // 
            this.tabPageWavefrontView.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageWavefrontView.Location = new System.Drawing.Point(4, 26);
            this.tabPageWavefrontView.Name = "tabPageWavefrontView";
            this.tabPageWavefrontView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWavefrontView.Size = new System.Drawing.Size(1172, 775);
            this.tabPageWavefrontView.TabIndex = 2;
            this.tabPageWavefrontView.Text = "Wavefront";
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // resultView
            // 
            this.resultView.Controls.Add(this.zernikeTab);
            this.resultView.Controls.Add(this.legendreTab);
            this.resultView.Controls.Add(this.seidelTab);
            this.resultView.Controls.Add(this.totalTab);
            this.resultView.Controls.Add(this.optionTab);
            this.resultView.Controls.Add(this.statusTab);
            this.resultView.Controls.Add(this.logTab);
            this.resultView.Font = new System.Drawing.Font("Meiryo UI", 10F);
            this.resultView.Location = new System.Drawing.Point(12, 228);
            this.resultView.Name = "resultView";
            this.resultView.SelectedIndex = 0;
            this.resultView.Size = new System.Drawing.Size(454, 604);
            this.resultView.TabIndex = 15;
            // 
            // zernikeTab
            // 
            this.zernikeTab.BackColor = System.Drawing.SystemColors.Control;
            this.zernikeTab.Controls.Add(this.zernikeView);
            this.zernikeTab.ForeColor = System.Drawing.SystemColors.ControlText;
            this.zernikeTab.Location = new System.Drawing.Point(4, 26);
            this.zernikeTab.Name = "zernikeTab";
            this.zernikeTab.Padding = new System.Windows.Forms.Padding(3);
            this.zernikeTab.Size = new System.Drawing.Size(446, 574);
            this.zernikeTab.TabIndex = 0;
            this.zernikeTab.Text = "Zernike";
            // 
            // zernikeView
            // 
            this.zernikeView.AllowUserToAddRows = false;
            this.zernikeView.AllowUserToDeleteRows = false;
            this.zernikeView.AllowUserToResizeRows = false;
            this.zernikeView.AutoGenerateColumns = false;
            this.zernikeView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.zernikeView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.captionDataGridViewTextBoxColumn,
            this.enabledDataGridViewCheckBoxColumn,
            this.valueDataGridViewTextBoxColumn});
            this.zernikeView.DataSource = this.zernikeComponentsBindingSource;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Meiryo UI", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.Format = "#0.000000";
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.zernikeView.DefaultCellStyle = dataGridViewCellStyle1;
            this.zernikeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zernikeView.Location = new System.Drawing.Point(3, 3);
            this.zernikeView.Name = "zernikeView";
            this.zernikeView.RowHeadersVisible = false;
            this.zernikeView.RowTemplate.Height = 21;
            this.zernikeView.Size = new System.Drawing.Size(440, 568);
            this.zernikeView.TabIndex = 0;
            // 
            // captionDataGridViewTextBoxColumn
            // 
            this.captionDataGridViewTextBoxColumn.DataPropertyName = "Caption";
            this.captionDataGridViewTextBoxColumn.HeaderText = "Caption";
            this.captionDataGridViewTextBoxColumn.Name = "captionDataGridViewTextBoxColumn";
            this.captionDataGridViewTextBoxColumn.ReadOnly = true;
            this.captionDataGridViewTextBoxColumn.Width = 180;
            // 
            // enabledDataGridViewCheckBoxColumn
            // 
            this.enabledDataGridViewCheckBoxColumn.ContextMenuStrip = this.zernikeEnabledMenuStrip;
            this.enabledDataGridViewCheckBoxColumn.DataPropertyName = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.HeaderText = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.Name = "enabledDataGridViewCheckBoxColumn";
            // 
            // zernikeEnabledMenuStrip
            // 
            this.zernikeEnabledMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zernikeCheckAll,
            this.zernikeUncheckAll});
            this.zernikeEnabledMenuStrip.Name = "zernikeEnabledMenuStrip";
            this.zernikeEnabledMenuStrip.Size = new System.Drawing.Size(138, 48);
            // 
            // zernikeCheckAll
            // 
            this.zernikeCheckAll.Name = "zernikeCheckAll";
            this.zernikeCheckAll.Size = new System.Drawing.Size(137, 22);
            this.zernikeCheckAll.Text = "Check All";
            this.zernikeCheckAll.Click += new System.EventHandler(this.zernikeCheckAll_Click);
            // 
            // zernikeUncheckAll
            // 
            this.zernikeUncheckAll.Name = "zernikeUncheckAll";
            this.zernikeUncheckAll.Size = new System.Drawing.Size(137, 22);
            this.zernikeUncheckAll.Text = "Uncheck All";
            this.zernikeUncheckAll.Click += new System.EventHandler(this.zernikeUncheckAll_Click);
            // 
            // valueDataGridViewTextBoxColumn
            // 
            this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
            this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
            this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
            this.valueDataGridViewTextBoxColumn.ReadOnly = true;
            this.valueDataGridViewTextBoxColumn.Width = 120;
            // 
            // zernikeComponentsBindingSource
            // 
            this.zernikeComponentsBindingSource.DataSource = typeof(WavefrontTester.MainForm.ZernikeComponents);
            // 
            // legendreTab
            // 
            this.legendreTab.BackColor = System.Drawing.SystemColors.Control;
            this.legendreTab.Controls.Add(this.legendreView);
            this.legendreTab.Location = new System.Drawing.Point(4, 26);
            this.legendreTab.Name = "legendreTab";
            this.legendreTab.Padding = new System.Windows.Forms.Padding(3);
            this.legendreTab.Size = new System.Drawing.Size(446, 574);
            this.legendreTab.TabIndex = 3;
            this.legendreTab.Text = "Legendre";
            // 
            // legendreView
            // 
            this.legendreView.AllowUserToAddRows = false;
            this.legendreView.AllowUserToDeleteRows = false;
            this.legendreView.AllowUserToResizeRows = false;
            this.legendreView.AutoGenerateColumns = false;
            this.legendreView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.legendreView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewCheckBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.legendreView.DataSource = this.legendreComponentsBindingSource;
            this.legendreView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.legendreView.Location = new System.Drawing.Point(3, 3);
            this.legendreView.Name = "legendreView";
            this.legendreView.RowHeadersVisible = false;
            this.legendreView.RowTemplate.Height = 21;
            this.legendreView.Size = new System.Drawing.Size(440, 568);
            this.legendreView.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Caption";
            this.dataGridViewTextBoxColumn1.HeaderText = "Caption";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.ContextMenuStrip = this.legendreEnabledMenuStrip;
            this.dataGridViewCheckBoxColumn1.DataPropertyName = "Enabled";
            this.dataGridViewCheckBoxColumn1.HeaderText = "Enabled";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            // 
            // legendreEnabledMenuStrip
            // 
            this.legendreEnabledMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.legendreCheckAll,
            this.legendreUncheckAll});
            this.legendreEnabledMenuStrip.Name = "zernikeEnabledMenuStrip";
            this.legendreEnabledMenuStrip.Size = new System.Drawing.Size(138, 48);
            // 
            // legendreCheckAll
            // 
            this.legendreCheckAll.Name = "legendreCheckAll";
            this.legendreCheckAll.Size = new System.Drawing.Size(137, 22);
            this.legendreCheckAll.Text = "Check All";
            this.legendreCheckAll.Click += new System.EventHandler(this.legendreCheckAll_Click);
            // 
            // legendreUncheckAll
            // 
            this.legendreUncheckAll.Name = "legendreUncheckAll";
            this.legendreUncheckAll.Size = new System.Drawing.Size(137, 22);
            this.legendreUncheckAll.Text = "Uncheck All";
            this.legendreUncheckAll.Click += new System.EventHandler(this.legendreUncheckAll_Click);
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Value";
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 200;
            // 
            // legendreComponentsBindingSource
            // 
            this.legendreComponentsBindingSource.DataSource = typeof(WavefrontTester.MainForm.LegendreComponents);
            // 
            // seidelTab
            // 
            this.seidelTab.Controls.Add(this.seidelPanel);
            this.seidelTab.Location = new System.Drawing.Point(4, 26);
            this.seidelTab.Name = "seidelTab";
            this.seidelTab.Padding = new System.Windows.Forms.Padding(3);
            this.seidelTab.Size = new System.Drawing.Size(446, 574);
            this.seidelTab.TabIndex = 8;
            this.seidelTab.Text = "Seidel";
            // 
            // seidelPanel
            // 
            this.seidelPanel.Controls.Add(this.astigmatismAngleValue);
            this.seidelPanel.Controls.Add(this.astigmatismMagnitudeValue);
            this.seidelPanel.Controls.Add(this.comaAngleValue);
            this.seidelPanel.Controls.Add(this.comaMagnitudeValue);
            this.seidelPanel.Controls.Add(this.sphericalAberrationValue);
            this.seidelPanel.Controls.Add(this.defocusValue);
            this.seidelPanel.Controls.Add(this.tiltAngleValue);
            this.seidelPanel.Controls.Add(this.tiltMagnitudeValue);
            this.seidelPanel.Controls.Add(this.astigmatismLabel);
            this.seidelPanel.Controls.Add(this.comaLabel);
            this.seidelPanel.Controls.Add(this.sphericalAberraLabel);
            this.seidelPanel.Controls.Add(this.defocusLabel);
            this.seidelPanel.Controls.Add(this.astigmatismAngleLabel);
            this.seidelPanel.Controls.Add(this.comaAngleLabel);
            this.seidelPanel.Controls.Add(this.tiltAngleLabel);
            this.seidelPanel.Controls.Add(this.astigmatismMagnitudeLabel);
            this.seidelPanel.Controls.Add(this.comaMagnitudeLabel);
            this.seidelPanel.Controls.Add(this.tiltMagnitudeLabel);
            this.seidelPanel.Controls.Add(this.tiltLabel);
            this.seidelPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.seidelPanel.Location = new System.Drawing.Point(3, 3);
            this.seidelPanel.Name = "seidelPanel";
            this.seidelPanel.Size = new System.Drawing.Size(440, 568);
            this.seidelPanel.TabIndex = 0;
            // 
            // astigmatismAngleValue
            // 
            this.astigmatismAngleValue.AutoSize = true;
            this.astigmatismAngleValue.Location = new System.Drawing.Point(309, 346);
            this.astigmatismAngleValue.Name = "astigmatismAngleValue";
            this.astigmatismAngleValue.Size = new System.Drawing.Size(82, 18);
            this.astigmatismAngleValue.TabIndex = 12;
            this.astigmatismAngleValue.Text = "-0.000000";
            // 
            // astigmatismMagnitudeValue
            // 
            this.astigmatismMagnitudeValue.AutoSize = true;
            this.astigmatismMagnitudeValue.Location = new System.Drawing.Point(309, 311);
            this.astigmatismMagnitudeValue.Name = "astigmatismMagnitudeValue";
            this.astigmatismMagnitudeValue.Size = new System.Drawing.Size(82, 18);
            this.astigmatismMagnitudeValue.TabIndex = 11;
            this.astigmatismMagnitudeValue.Text = "-0.000000";
            // 
            // comaAngleValue
            // 
            this.comaAngleValue.AutoSize = true;
            this.comaAngleValue.Location = new System.Drawing.Point(309, 263);
            this.comaAngleValue.Name = "comaAngleValue";
            this.comaAngleValue.Size = new System.Drawing.Size(82, 18);
            this.comaAngleValue.TabIndex = 10;
            this.comaAngleValue.Text = "-0.000000";
            // 
            // comaMagnitudeValue
            // 
            this.comaMagnitudeValue.AutoSize = true;
            this.comaMagnitudeValue.Location = new System.Drawing.Point(309, 228);
            this.comaMagnitudeValue.Name = "comaMagnitudeValue";
            this.comaMagnitudeValue.Size = new System.Drawing.Size(82, 18);
            this.comaMagnitudeValue.TabIndex = 9;
            this.comaMagnitudeValue.Text = "-0.000000";
            // 
            // sphericalAberrationValue
            // 
            this.sphericalAberrationValue.AutoSize = true;
            this.sphericalAberrationValue.Location = new System.Drawing.Point(309, 169);
            this.sphericalAberrationValue.Name = "sphericalAberrationValue";
            this.sphericalAberrationValue.Size = new System.Drawing.Size(82, 18);
            this.sphericalAberrationValue.TabIndex = 8;
            this.sphericalAberrationValue.Text = "-0.000000";
            // 
            // defocusValue
            // 
            this.defocusValue.AutoSize = true;
            this.defocusValue.Location = new System.Drawing.Point(309, 115);
            this.defocusValue.Name = "defocusValue";
            this.defocusValue.Size = new System.Drawing.Size(82, 18);
            this.defocusValue.TabIndex = 7;
            this.defocusValue.Text = "-0.000000";
            // 
            // tiltAngleValue
            // 
            this.tiltAngleValue.AutoSize = true;
            this.tiltAngleValue.Location = new System.Drawing.Point(309, 60);
            this.tiltAngleValue.Name = "tiltAngleValue";
            this.tiltAngleValue.Size = new System.Drawing.Size(82, 18);
            this.tiltAngleValue.TabIndex = 6;
            this.tiltAngleValue.Text = "-0.000000";
            // 
            // tiltMagnitudeValue
            // 
            this.tiltMagnitudeValue.AutoSize = true;
            this.tiltMagnitudeValue.Location = new System.Drawing.Point(309, 25);
            this.tiltMagnitudeValue.Name = "tiltMagnitudeValue";
            this.tiltMagnitudeValue.Size = new System.Drawing.Size(82, 18);
            this.tiltMagnitudeValue.TabIndex = 5;
            this.tiltMagnitudeValue.Text = "-0.000000";
            // 
            // astigmatismLabel
            // 
            this.astigmatismLabel.AutoSize = true;
            this.astigmatismLabel.Location = new System.Drawing.Point(73, 311);
            this.astigmatismLabel.Name = "astigmatismLabel";
            this.astigmatismLabel.Size = new System.Drawing.Size(91, 18);
            this.astigmatismLabel.TabIndex = 4;
            this.astigmatismLabel.Text = "Astigmatism";
            // 
            // comaLabel
            // 
            this.comaLabel.AutoSize = true;
            this.comaLabel.Location = new System.Drawing.Point(118, 228);
            this.comaLabel.Name = "comaLabel";
            this.comaLabel.Size = new System.Drawing.Size(46, 18);
            this.comaLabel.TabIndex = 4;
            this.comaLabel.Text = "Coma";
            // 
            // sphericalAberraLabel
            // 
            this.sphericalAberraLabel.AutoSize = true;
            this.sphericalAberraLabel.Location = new System.Drawing.Point(15, 169);
            this.sphericalAberraLabel.Name = "sphericalAberraLabel";
            this.sphericalAberraLabel.Size = new System.Drawing.Size(149, 18);
            this.sphericalAberraLabel.TabIndex = 4;
            this.sphericalAberraLabel.Text = "Spherical Aberration";
            // 
            // defocusLabel
            // 
            this.defocusLabel.AutoSize = true;
            this.defocusLabel.Location = new System.Drawing.Point(102, 115);
            this.defocusLabel.Name = "defocusLabel";
            this.defocusLabel.Size = new System.Drawing.Size(62, 18);
            this.defocusLabel.TabIndex = 3;
            this.defocusLabel.Text = "Defocus";
            // 
            // astigmatismAngleLabel
            // 
            this.astigmatismAngleLabel.AutoSize = true;
            this.astigmatismAngleLabel.Location = new System.Drawing.Point(190, 346);
            this.astigmatismAngleLabel.Name = "astigmatismAngleLabel";
            this.astigmatismAngleLabel.Size = new System.Drawing.Size(46, 18);
            this.astigmatismAngleLabel.TabIndex = 2;
            this.astigmatismAngleLabel.Text = "Angle";
            // 
            // comaAngleLabel
            // 
            this.comaAngleLabel.AutoSize = true;
            this.comaAngleLabel.Location = new System.Drawing.Point(190, 263);
            this.comaAngleLabel.Name = "comaAngleLabel";
            this.comaAngleLabel.Size = new System.Drawing.Size(46, 18);
            this.comaAngleLabel.TabIndex = 2;
            this.comaAngleLabel.Text = "Angle";
            // 
            // tiltAngleLabel
            // 
            this.tiltAngleLabel.AutoSize = true;
            this.tiltAngleLabel.Location = new System.Drawing.Point(190, 60);
            this.tiltAngleLabel.Name = "tiltAngleLabel";
            this.tiltAngleLabel.Size = new System.Drawing.Size(46, 18);
            this.tiltAngleLabel.TabIndex = 2;
            this.tiltAngleLabel.Text = "Angle";
            // 
            // astigmatismMagnitudeLabel
            // 
            this.astigmatismMagnitudeLabel.AutoSize = true;
            this.astigmatismMagnitudeLabel.Location = new System.Drawing.Point(190, 311);
            this.astigmatismMagnitudeLabel.Name = "astigmatismMagnitudeLabel";
            this.astigmatismMagnitudeLabel.Size = new System.Drawing.Size(80, 18);
            this.astigmatismMagnitudeLabel.TabIndex = 1;
            this.astigmatismMagnitudeLabel.Text = "Magnitude";
            // 
            // comaMagnitudeLabel
            // 
            this.comaMagnitudeLabel.AutoSize = true;
            this.comaMagnitudeLabel.Location = new System.Drawing.Point(190, 228);
            this.comaMagnitudeLabel.Name = "comaMagnitudeLabel";
            this.comaMagnitudeLabel.Size = new System.Drawing.Size(80, 18);
            this.comaMagnitudeLabel.TabIndex = 1;
            this.comaMagnitudeLabel.Text = "Magnitude";
            // 
            // tiltMagnitudeLabel
            // 
            this.tiltMagnitudeLabel.AutoSize = true;
            this.tiltMagnitudeLabel.Location = new System.Drawing.Point(190, 25);
            this.tiltMagnitudeLabel.Name = "tiltMagnitudeLabel";
            this.tiltMagnitudeLabel.Size = new System.Drawing.Size(80, 18);
            this.tiltMagnitudeLabel.TabIndex = 1;
            this.tiltMagnitudeLabel.Text = "Magnitude";
            // 
            // tiltLabel
            // 
            this.tiltLabel.AutoSize = true;
            this.tiltLabel.Location = new System.Drawing.Point(134, 25);
            this.tiltLabel.Name = "tiltLabel";
            this.tiltLabel.Size = new System.Drawing.Size(30, 18);
            this.tiltLabel.TabIndex = 0;
            this.tiltLabel.Text = "Tilt";
            // 
            // totalTab
            // 
            this.totalTab.Controls.Add(this.panel1);
            this.totalTab.Location = new System.Drawing.Point(4, 26);
            this.totalTab.Name = "totalTab";
            this.totalTab.Padding = new System.Windows.Forms.Padding(3);
            this.totalTab.Size = new System.Drawing.Size(446, 574);
            this.totalTab.TabIndex = 9;
            this.totalTab.Text = "Total";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rhoYValue);
            this.panel1.Controls.Add(this.rhoYLabel);
            this.panel1.Controls.Add(this.rhoXLabel);
            this.panel1.Controls.Add(this.rocYValue);
            this.panel1.Controls.Add(this.rocYLabel);
            this.panel1.Controls.Add(this.rocXLabel);
            this.panel1.Controls.Add(this.strehlRatioLabel);
            this.panel1.Controls.Add(this.pvLabel);
            this.panel1.Controls.Add(this.incidentAngleSyntheticValue);
            this.panel1.Controls.Add(this.incidentAngleSyntheticLabel);
            this.panel1.Controls.Add(this.strehlRatioValue);
            this.panel1.Controls.Add(this.rmsValue);
            this.panel1.Controls.Add(this.pvValue);
            this.panel1.Controls.Add(this.rhoXValue);
            this.panel1.Controls.Add(this.rocXValue);
            this.panel1.Controls.Add(this.powerValue);
            this.panel1.Controls.Add(this.incidentAngleYValue);
            this.panel1.Controls.Add(this.incidentAngleXValue);
            this.panel1.Controls.Add(this.rmsLabel);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.powerLabel);
            this.panel1.Controls.Add(this.incidentAngleYLabel);
            this.panel1.Controls.Add(this.incidentAngleXLabel);
            this.panel1.Controls.Add(this.incidentAngleLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(440, 568);
            this.panel1.TabIndex = 1;
            // 
            // rhoYValue
            // 
            this.rhoYValue.AutoSize = true;
            this.rhoYValue.Location = new System.Drawing.Point(309, 270);
            this.rhoYValue.Name = "rhoYValue";
            this.rhoYValue.Size = new System.Drawing.Size(82, 18);
            this.rhoYValue.TabIndex = 22;
            this.rhoYValue.Text = "-0.000000";
            // 
            // rhoYLabel
            // 
            this.rhoYLabel.AutoSize = true;
            this.rhoYLabel.Location = new System.Drawing.Point(190, 270);
            this.rhoYLabel.Name = "rhoYLabel";
            this.rhoYLabel.Size = new System.Drawing.Size(17, 18);
            this.rhoYLabel.TabIndex = 21;
            this.rhoYLabel.Text = "Y";
            // 
            // rhoXLabel
            // 
            this.rhoXLabel.AutoSize = true;
            this.rhoXLabel.Location = new System.Drawing.Point(190, 235);
            this.rhoXLabel.Name = "rhoXLabel";
            this.rhoXLabel.Size = new System.Drawing.Size(17, 18);
            this.rhoXLabel.TabIndex = 20;
            this.rhoXLabel.Text = "X";
            // 
            // rocYValue
            // 
            this.rocYValue.AutoSize = true;
            this.rocYValue.Location = new System.Drawing.Point(309, 200);
            this.rocYValue.Name = "rocYValue";
            this.rocYValue.Size = new System.Drawing.Size(82, 18);
            this.rocYValue.TabIndex = 19;
            this.rocYValue.Text = "-0.000000";
            // 
            // rocYLabel
            // 
            this.rocYLabel.AutoSize = true;
            this.rocYLabel.Location = new System.Drawing.Point(190, 200);
            this.rocYLabel.Name = "rocYLabel";
            this.rocYLabel.Size = new System.Drawing.Size(17, 18);
            this.rocYLabel.TabIndex = 18;
            this.rocYLabel.Text = "Y";
            // 
            // rocXLabel
            // 
            this.rocXLabel.AutoSize = true;
            this.rocXLabel.Location = new System.Drawing.Point(190, 165);
            this.rocXLabel.Name = "rocXLabel";
            this.rocXLabel.Size = new System.Drawing.Size(17, 18);
            this.rocXLabel.TabIndex = 17;
            this.rocXLabel.Text = "X";
            // 
            // strehlRatioLabel
            // 
            this.strehlRatioLabel.AutoSize = true;
            this.strehlRatioLabel.Location = new System.Drawing.Point(75, 375);
            this.strehlRatioLabel.Name = "strehlRatioLabel";
            this.strehlRatioLabel.Size = new System.Drawing.Size(89, 18);
            this.strehlRatioLabel.TabIndex = 16;
            this.strehlRatioLabel.Text = "Strehl Ratio";
            // 
            // pvLabel
            // 
            this.pvLabel.AutoSize = true;
            this.pvLabel.Location = new System.Drawing.Point(138, 305);
            this.pvLabel.Name = "pvLabel";
            this.pvLabel.Size = new System.Drawing.Size(26, 18);
            this.pvLabel.TabIndex = 15;
            this.pvLabel.Text = "PV";
            // 
            // incidentAngleSyntheticValue
            // 
            this.incidentAngleSyntheticValue.AutoSize = true;
            this.incidentAngleSyntheticValue.Location = new System.Drawing.Point(309, 95);
            this.incidentAngleSyntheticValue.Name = "incidentAngleSyntheticValue";
            this.incidentAngleSyntheticValue.Size = new System.Drawing.Size(82, 18);
            this.incidentAngleSyntheticValue.TabIndex = 14;
            this.incidentAngleSyntheticValue.Text = "-0.000000";
            // 
            // incidentAngleSyntheticLabel
            // 
            this.incidentAngleSyntheticLabel.AutoSize = true;
            this.incidentAngleSyntheticLabel.Location = new System.Drawing.Point(190, 95);
            this.incidentAngleSyntheticLabel.Name = "incidentAngleSyntheticLabel";
            this.incidentAngleSyntheticLabel.Size = new System.Drawing.Size(72, 18);
            this.incidentAngleSyntheticLabel.TabIndex = 13;
            this.incidentAngleSyntheticLabel.Text = "Synthetic";
            // 
            // strehlRatioValue
            // 
            this.strehlRatioValue.AutoSize = true;
            this.strehlRatioValue.Location = new System.Drawing.Point(309, 375);
            this.strehlRatioValue.Name = "strehlRatioValue";
            this.strehlRatioValue.Size = new System.Drawing.Size(82, 18);
            this.strehlRatioValue.TabIndex = 12;
            this.strehlRatioValue.Text = "-0.000000";
            // 
            // rmsValue
            // 
            this.rmsValue.AutoSize = true;
            this.rmsValue.Location = new System.Drawing.Point(309, 340);
            this.rmsValue.Name = "rmsValue";
            this.rmsValue.Size = new System.Drawing.Size(82, 18);
            this.rmsValue.TabIndex = 11;
            this.rmsValue.Text = "-0.000000";
            // 
            // pvValue
            // 
            this.pvValue.AutoSize = true;
            this.pvValue.Location = new System.Drawing.Point(309, 305);
            this.pvValue.Name = "pvValue";
            this.pvValue.Size = new System.Drawing.Size(82, 18);
            this.pvValue.TabIndex = 10;
            this.pvValue.Text = "-0.000000";
            // 
            // rhoXValue
            // 
            this.rhoXValue.AutoSize = true;
            this.rhoXValue.Location = new System.Drawing.Point(309, 235);
            this.rhoXValue.Name = "rhoXValue";
            this.rhoXValue.Size = new System.Drawing.Size(82, 18);
            this.rhoXValue.TabIndex = 9;
            this.rhoXValue.Text = "-0.000000";
            // 
            // rocXValue
            // 
            this.rocXValue.AutoSize = true;
            this.rocXValue.Location = new System.Drawing.Point(309, 165);
            this.rocXValue.Name = "rocXValue";
            this.rocXValue.Size = new System.Drawing.Size(82, 18);
            this.rocXValue.TabIndex = 8;
            this.rocXValue.Text = "-0.000000";
            // 
            // powerValue
            // 
            this.powerValue.AutoSize = true;
            this.powerValue.Location = new System.Drawing.Point(309, 130);
            this.powerValue.Name = "powerValue";
            this.powerValue.Size = new System.Drawing.Size(82, 18);
            this.powerValue.TabIndex = 7;
            this.powerValue.Text = "-0.000000";
            // 
            // incidentAngleYValue
            // 
            this.incidentAngleYValue.AutoSize = true;
            this.incidentAngleYValue.Location = new System.Drawing.Point(309, 60);
            this.incidentAngleYValue.Name = "incidentAngleYValue";
            this.incidentAngleYValue.Size = new System.Drawing.Size(82, 18);
            this.incidentAngleYValue.TabIndex = 6;
            this.incidentAngleYValue.Text = "-0.000000";
            // 
            // incidentAngleXValue
            // 
            this.incidentAngleXValue.AutoSize = true;
            this.incidentAngleXValue.Location = new System.Drawing.Point(309, 25);
            this.incidentAngleXValue.Name = "incidentAngleXValue";
            this.incidentAngleXValue.Size = new System.Drawing.Size(82, 18);
            this.incidentAngleXValue.TabIndex = 5;
            this.incidentAngleXValue.Text = "-0.000000";
            // 
            // rmsLabel
            // 
            this.rmsLabel.AutoSize = true;
            this.rmsLabel.Location = new System.Drawing.Point(125, 340);
            this.rmsLabel.Name = "rmsLabel";
            this.rmsLabel.Size = new System.Drawing.Size(39, 18);
            this.rmsLabel.TabIndex = 4;
            this.rmsLabel.Text = "RMS";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(104, 235);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 18);
            this.label12.TabIndex = 4;
            this.label12.Text = "ρ (Rho)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(99, 165);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 18);
            this.label13.TabIndex = 4;
            this.label13.Text = "RoC [m]";
            // 
            // powerLabel
            // 
            this.powerLabel.AutoSize = true;
            this.powerLabel.Location = new System.Drawing.Point(114, 130);
            this.powerLabel.Name = "powerLabel";
            this.powerLabel.Size = new System.Drawing.Size(50, 18);
            this.powerLabel.TabIndex = 3;
            this.powerLabel.Text = "Power";
            // 
            // incidentAngleYLabel
            // 
            this.incidentAngleYLabel.AutoSize = true;
            this.incidentAngleYLabel.Location = new System.Drawing.Point(190, 60);
            this.incidentAngleYLabel.Name = "incidentAngleYLabel";
            this.incidentAngleYLabel.Size = new System.Drawing.Size(17, 18);
            this.incidentAngleYLabel.TabIndex = 2;
            this.incidentAngleYLabel.Text = "Y";
            // 
            // incidentAngleXLabel
            // 
            this.incidentAngleXLabel.AutoSize = true;
            this.incidentAngleXLabel.Location = new System.Drawing.Point(190, 25);
            this.incidentAngleXLabel.Name = "incidentAngleXLabel";
            this.incidentAngleXLabel.Size = new System.Drawing.Size(17, 18);
            this.incidentAngleXLabel.TabIndex = 1;
            this.incidentAngleXLabel.Text = "X";
            // 
            // incidentAngleLabel
            // 
            this.incidentAngleLabel.AutoSize = true;
            this.incidentAngleLabel.Location = new System.Drawing.Point(56, 25);
            this.incidentAngleLabel.Name = "incidentAngleLabel";
            this.incidentAngleLabel.Size = new System.Drawing.Size(108, 18);
            this.incidentAngleLabel.TabIndex = 0;
            this.incidentAngleLabel.Text = "Incident Angle";
            // 
            // optionTab
            // 
            this.optionTab.BackColor = System.Drawing.SystemColors.Control;
            this.optionTab.Controls.Add(this.propertyGrid);
            this.optionTab.Location = new System.Drawing.Point(4, 26);
            this.optionTab.Name = "optionTab";
            this.optionTab.Padding = new System.Windows.Forms.Padding(3);
            this.optionTab.Size = new System.Drawing.Size(446, 574);
            this.optionTab.TabIndex = 4;
            this.optionTab.Text = "Options";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(440, 568);
            this.propertyGrid.TabIndex = 0;
            // 
            // statusTab
            // 
            this.statusTab.Controls.Add(this.statusGrid);
            this.statusTab.Location = new System.Drawing.Point(4, 26);
            this.statusTab.Name = "statusTab";
            this.statusTab.Padding = new System.Windows.Forms.Padding(3);
            this.statusTab.Size = new System.Drawing.Size(446, 574);
            this.statusTab.TabIndex = 6;
            this.statusTab.Text = "Status";
            this.statusTab.UseVisualStyleBackColor = true;
            // 
            // statusGrid
            // 
            this.statusGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusGrid.Location = new System.Drawing.Point(3, 3);
            this.statusGrid.Name = "statusGrid";
            this.statusGrid.Size = new System.Drawing.Size(440, 568);
            this.statusGrid.TabIndex = 0;
            // 
            // logTab
            // 
            this.logTab.Controls.Add(this.logPanel);
            this.logTab.Location = new System.Drawing.Point(4, 26);
            this.logTab.Name = "logTab";
            this.logTab.Padding = new System.Windows.Forms.Padding(3);
            this.logTab.Size = new System.Drawing.Size(446, 574);
            this.logTab.TabIndex = 7;
            this.logTab.Text = "Log";
            this.logTab.UseVisualStyleBackColor = true;
            // 
            // logPanel
            // 
            this.logPanel.Controls.Add(this.filePathLabel);
            this.logPanel.Controls.Add(this.saveContinuous);
            this.logPanel.Controls.Add(this.loggingCount);
            this.logPanel.Controls.Add(this.logDescription);
            this.logPanel.Controls.Add(this.filePathButton);
            this.logPanel.Controls.Add(this.saveOnce);
            this.logPanel.Controls.Add(this.label1);
            this.logPanel.Controls.Add(this.saveLogButton);
            this.logPanel.Controls.Add(this.loggingInterval);
            this.logPanel.Controls.Add(this.label2);
            this.logPanel.Controls.Add(this.filePath);
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.Location = new System.Drawing.Point(3, 3);
            this.logPanel.Margin = new System.Windows.Forms.Padding(0);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(440, 568);
            this.logPanel.TabIndex = 13;
            // 
            // filePathLabel
            // 
            this.filePathLabel.AutoSize = true;
            this.filePathLabel.Location = new System.Drawing.Point(7, 9);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(75, 18);
            this.filePathLabel.TabIndex = 3;
            this.filePathLabel.Text = "ファイルパス:";
            // 
            // saveContinuous
            // 
            this.saveContinuous.AutoSize = true;
            this.saveContinuous.Location = new System.Drawing.Point(10, 203);
            this.saveContinuous.Name = "saveContinuous";
            this.saveContinuous.Size = new System.Drawing.Size(54, 22);
            this.saveContinuous.TabIndex = 12;
            this.saveContinuous.Text = "連続";
            this.saveContinuous.UseVisualStyleBackColor = true;
            // 
            // loggingCount
            // 
            this.loggingCount.Location = new System.Drawing.Point(159, 204);
            this.loggingCount.Name = "loggingCount";
            this.loggingCount.Size = new System.Drawing.Size(90, 24);
            this.loggingCount.TabIndex = 7;
            this.loggingCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.loggingCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // logDescription
            // 
            this.logDescription.AllowUserToAddRows = false;
            this.logDescription.AllowUserToDeleteRows = false;
            this.logDescription.AllowUserToResizeColumns = false;
            this.logDescription.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.logDescription.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Caption,
            this.Contents});
            this.logDescription.Location = new System.Drawing.Point(10, 70);
            this.logDescription.Name = "logDescription";
            this.logDescription.RowHeadersVisible = false;
            this.logDescription.RowTemplate.Height = 21;
            this.logDescription.Size = new System.Drawing.Size(276, 102);
            this.logDescription.TabIndex = 0;
            // 
            // Caption
            // 
            this.Caption.HeaderText = "Caption";
            this.Caption.Name = "Caption";
            this.Caption.ReadOnly = true;
            this.Caption.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Caption.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Contents
            // 
            this.Contents.HeaderText = "Contents";
            this.Contents.Name = "Contents";
            this.Contents.Width = 150;
            // 
            // filePathButton
            // 
            this.filePathButton.Location = new System.Drawing.Point(395, 30);
            this.filePathButton.Name = "filePathButton";
            this.filePathButton.Size = new System.Drawing.Size(42, 24);
            this.filePathButton.TabIndex = 4;
            this.filePathButton.Text = "...";
            this.filePathButton.UseVisualStyleBackColor = true;
            this.filePathButton.Click += new System.EventHandler(this.filePathButton_Click);
            // 
            // saveOnce
            // 
            this.saveOnce.AutoSize = true;
            this.saveOnce.Checked = true;
            this.saveOnce.Location = new System.Drawing.Point(10, 178);
            this.saveOnce.Name = "saveOnce";
            this.saveOnce.Size = new System.Drawing.Size(54, 22);
            this.saveOnce.TabIndex = 11;
            this.saveOnce.TabStop = true;
            this.saveOnce.Text = "一回";
            this.saveOnce.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 206);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 18);
            this.label1.TabIndex = 8;
            this.label1.Text = "回数:";
            // 
            // saveLogButton
            // 
            this.saveLogButton.Location = new System.Drawing.Point(292, 70);
            this.saveLogButton.Name = "saveLogButton";
            this.saveLogButton.Size = new System.Drawing.Size(75, 41);
            this.saveLogButton.TabIndex = 1;
            this.saveLogButton.Text = "Save";
            this.saveLogButton.UseVisualStyleBackColor = true;
            this.saveLogButton.Click += new System.EventHandler(this.saveLogButton_Click);
            // 
            // loggingInterval
            // 
            this.loggingInterval.Location = new System.Drawing.Point(159, 232);
            this.loggingInterval.Maximum = new decimal(new int[] {
            86400000,
            0,
            0,
            0});
            this.loggingInterval.Name = "loggingInterval";
            this.loggingInterval.Size = new System.Drawing.Size(90, 24);
            this.loggingInterval.TabIndex = 9;
            this.loggingInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.loggingInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 234);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 18);
            this.label2.TabIndex = 10;
            this.label2.Text = "間隔(ms):";
            // 
            // filePath
            // 
            this.filePath.Location = new System.Drawing.Point(10, 30);
            this.filePath.Name = "filePath";
            this.filePath.Size = new System.Drawing.Size(379, 24);
            this.filePath.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.framesLabel,
            this.pairCountLabel,
            this.resultIntervalLabel,
            this.calibrationFileName});
            this.statusStrip1.Location = new System.Drawing.Point(0, 839);
            this.statusStrip1.Margin = new System.Windows.Forms.Padding(3);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1664, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 18;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // framesLabel
            // 
            this.framesLabel.Name = "framesLabel";
            this.framesLabel.Size = new System.Drawing.Size(55, 17);
            this.framesLabel.Text = "Frames: 0";
            // 
            // pairCountLabel
            // 
            this.pairCountLabel.Name = "pairCountLabel";
            this.pairCountLabel.Size = new System.Drawing.Size(44, 17);
            this.pairCountLabel.Text = "Pairs: 0";
            // 
            // resultIntervalLabel
            // 
            this.resultIntervalLabel.Name = "resultIntervalLabel";
            this.resultIntervalLabel.Size = new System.Drawing.Size(116, 17);
            this.resultIntervalLabel.Text = "Result Interval: 0[ms]";
            // 
            // calibrationFileName
            // 
            this.calibrationFileName.Name = "calibrationFileName";
            this.calibrationFileName.Size = new System.Drawing.Size(85, 17);
            this.calibrationFileName.Text = "Not calibrated.";
            // 
            // deviceListPanel
            // 
            this.deviceListPanel.Controls.Add(this.deviceListView);
            this.deviceListPanel.Location = new System.Drawing.Point(12, 27);
            this.deviceListPanel.Name = "deviceListPanel";
            this.deviceListPanel.Size = new System.Drawing.Size(454, 100);
            this.deviceListPanel.TabIndex = 19;
            // 
            // deviceListView
            // 
            this.deviceListView.AllowUserToAddRows = false;
            this.deviceListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.deviceListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceListView.Location = new System.Drawing.Point(0, 0);
            this.deviceListView.MultiSelect = false;
            this.deviceListView.Name = "deviceListView";
            this.deviceListView.ReadOnly = true;
            this.deviceListView.RowHeadersVisible = false;
            this.deviceListView.RowTemplate.Height = 21;
            this.deviceListView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.deviceListView.Size = new System.Drawing.Size(454, 100);
            this.deviceListView.TabIndex = 0;
            // 
            // buttonPanel
            // 
            this.buttonPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.buttonPanel.Controls.Add(this.checkMeasure);
            this.buttonPanel.Controls.Add(this.checkCalibration);
            this.buttonPanel.Controls.Add(this.checkOpen);
            this.buttonPanel.Controls.Add(this.deviceButton);
            this.buttonPanel.Controls.Add(this.snapshotButton);
            this.buttonPanel.Controls.Add(this.stopButton);
            this.buttonPanel.Controls.Add(this.startButton);
            this.buttonPanel.Controls.Add(this.closeButton);
            this.buttonPanel.Controls.Add(this.openButton);
            this.buttonPanel.Location = new System.Drawing.Point(12, 133);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(454, 89);
            this.buttonPanel.TabIndex = 21;
            // 
            // checkMeasure
            // 
            this.checkMeasure.AutoCheck = false;
            this.checkMeasure.AutoSize = true;
            this.checkMeasure.Location = new System.Drawing.Point(352, 58);
            this.checkMeasure.Name = "checkMeasure";
            this.checkMeasure.Size = new System.Drawing.Size(85, 19);
            this.checkMeasure.TabIndex = 8;
            this.checkMeasure.Text = "Measuring";
            this.checkMeasure.UseVisualStyleBackColor = true;
            // 
            // checkCalibration
            // 
            this.checkCalibration.AutoCheck = false;
            this.checkCalibration.AutoSize = true;
            this.checkCalibration.Location = new System.Drawing.Point(352, 33);
            this.checkCalibration.Name = "checkCalibration";
            this.checkCalibration.Size = new System.Drawing.Size(85, 19);
            this.checkCalibration.TabIndex = 7;
            this.checkCalibration.Text = "Calibrated";
            this.checkCalibration.UseVisualStyleBackColor = true;
            // 
            // checkOpen
            // 
            this.checkOpen.AutoCheck = false;
            this.checkOpen.AutoSize = true;
            this.checkOpen.Location = new System.Drawing.Point(352, 8);
            this.checkOpen.Name = "checkOpen";
            this.checkOpen.Size = new System.Drawing.Size(70, 19);
            this.checkOpen.TabIndex = 6;
            this.checkOpen.Text = "Opened";
            this.checkOpen.UseVisualStyleBackColor = true;
            // 
            // deviceButton
            // 
            this.deviceButton.Location = new System.Drawing.Point(219, 3);
            this.deviceButton.Name = "deviceButton";
            this.deviceButton.Size = new System.Drawing.Size(88, 37);
            this.deviceButton.TabIndex = 5;
            this.deviceButton.Text = "Device";
            this.deviceButton.UseVisualStyleBackColor = true;
            this.deviceButton.Click += new System.EventHandler(this.deviceButton_Click);
            // 
            // snapshotButton
            // 
            this.snapshotButton.Location = new System.Drawing.Point(219, 46);
            this.snapshotButton.Name = "snapshotButton";
            this.snapshotButton.Size = new System.Drawing.Size(88, 37);
            this.snapshotButton.TabIndex = 4;
            this.snapshotButton.Text = "Snapshot";
            this.snapshotButton.UseVisualStyleBackColor = true;
            this.snapshotButton.Click += new System.EventHandler(this.snapshotButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(97, 46);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(88, 37);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(3, 46);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(88, 37);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(97, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(88, 37);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(3, 3);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(88, 37);
            this.openButton.TabIndex = 0;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1664, 861);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.deviceListPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.resultView);
            this.Controls.Add(this.tabControlViews);
            this.Controls.Add(this.menuStripMain);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wavefront Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.tabControlViews.ResumeLayout(false);
            this.resultView.ResumeLayout(false);
            this.zernikeTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zernikeView)).EndInit();
            this.zernikeEnabledMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zernikeComponentsBindingSource)).EndInit();
            this.legendreTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.legendreView)).EndInit();
            this.legendreEnabledMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.legendreComponentsBindingSource)).EndInit();
            this.seidelTab.ResumeLayout(false);
            this.seidelPanel.ResumeLayout(false);
            this.seidelPanel.PerformLayout();
            this.totalTab.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.optionTab.ResumeLayout(false);
            this.statusTab.ResumeLayout(false);
            this.logTab.ResumeLayout(false);
            this.logPanel.ResumeLayout(false);
            this.logPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loggingCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loggingInterval)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.deviceListPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.deviceListView)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControlViews;
        private System.Windows.Forms.TabPage tabPageSpotView;
        private System.Windows.Forms.TabPage tabPageIntensityView;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.TabPage tabPageWavefrontView;
        private System.Windows.Forms.TabControl resultView;
        private System.Windows.Forms.TabPage zernikeTab;
        private System.Windows.Forms.TabPage legendreTab;
        private System.Windows.Forms.TabPage optionTab;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel framesLabel;
        private System.Windows.Forms.ToolStripStatusLabel resultIntervalLabel;
        private System.Windows.Forms.ToolStripMenuItem calibrationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userCalibrationMenu;
        private System.Windows.Forms.Panel deviceListPanel;
        private System.Windows.Forms.DataGridView deviceListView;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button snapshotButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button deviceButton;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.DataGridView zernikeView;
        private System.Windows.Forms.BindingSource zernikeComponentsBindingSource;
        private System.Windows.Forms.ContextMenuStrip zernikeEnabledMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem zernikeCheckAll;
        private System.Windows.Forms.ToolStripMenuItem zernikeUncheckAll;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawSpotPairMap;
        private System.Windows.Forms.ToolStripMenuItem drawSpotProfile;
        private System.Windows.Forms.DataGridView legendreView;
        private System.Windows.Forms.BindingSource legendreComponentsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn captionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabledDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateDefaultCalibration;
        private System.Windows.Forms.ToolStripMenuItem saveDefaultCalibration;
        private System.Windows.Forms.ToolStripMenuItem generateUserCalibration;
        private System.Windows.Forms.ToolStripMenuItem saveUserCalibration;
        private System.Windows.Forms.ToolStripMenuItem saveOptions;
        private System.Windows.Forms.ToolStripMenuItem loadOptions;
        private System.Windows.Forms.ContextMenuStrip legendreEnabledMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem legendreCheckAll;
        private System.Windows.Forms.ToolStripMenuItem legendreUncheckAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView logDescription;
        private System.Windows.Forms.Button saveLogButton;
        private System.Windows.Forms.TextBox filePath;
        private System.Windows.Forms.Label filePathLabel;
        private System.Windows.Forms.Button filePathButton;
        private System.Windows.Forms.NumericUpDown loggingCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown loggingInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage statusTab;
        private System.Windows.Forms.PropertyGrid statusGrid;
        private System.Windows.Forms.ToolStripMenuItem loadDefaultCalibration;
        private System.Windows.Forms.RadioButton saveContinuous;
        private System.Windows.Forms.RadioButton saveOnce;
        private System.Windows.Forms.TabPage logTab;
        private System.Windows.Forms.ToolStripMenuItem loadUserCalibration;
        private System.Windows.Forms.ToolStripMenuItem drawPairingArea;
        private System.Windows.Forms.CheckBox checkMeasure;
        private System.Windows.Forms.CheckBox checkCalibration;
        private System.Windows.Forms.CheckBox checkOpen;
        private System.Windows.Forms.Panel logPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Caption;
        private System.Windows.Forms.DataGridViewTextBoxColumn Contents;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem stopDrawing;
        private System.Windows.Forms.ToolStripStatusLabel calibrationFileName;
        private System.Windows.Forms.ToolStripStatusLabel pairCountLabel;
        private System.Windows.Forms.TabPage seidelTab;
        private System.Windows.Forms.Panel seidelPanel;
        private System.Windows.Forms.Label tiltAngleLabel;
        private System.Windows.Forms.Label tiltMagnitudeLabel;
        private System.Windows.Forms.Label tiltLabel;
        private System.Windows.Forms.Label defocusLabel;
        private System.Windows.Forms.Label sphericalAberraLabel;
        private System.Windows.Forms.Label comaLabel;
        private System.Windows.Forms.Label comaAngleLabel;
        private System.Windows.Forms.Label comaMagnitudeLabel;
        private System.Windows.Forms.Label astigmatismLabel;
        private System.Windows.Forms.Label astigmatismAngleValue;
        private System.Windows.Forms.Label astigmatismMagnitudeValue;
        private System.Windows.Forms.Label comaAngleValue;
        private System.Windows.Forms.Label comaMagnitudeValue;
        private System.Windows.Forms.Label sphericalAberrationValue;
        private System.Windows.Forms.Label defocusValue;
        private System.Windows.Forms.Label tiltAngleValue;
        private System.Windows.Forms.Label tiltMagnitudeValue;
        private System.Windows.Forms.Label astigmatismAngleLabel;
        private System.Windows.Forms.Label astigmatismMagnitudeLabel;
        private System.Windows.Forms.TabPage totalTab;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label incidentAngleSyntheticValue;
        private System.Windows.Forms.Label incidentAngleSyntheticLabel;
        private System.Windows.Forms.Label strehlRatioValue;
        private System.Windows.Forms.Label rmsValue;
        private System.Windows.Forms.Label pvValue;
        private System.Windows.Forms.Label rhoXValue;
        private System.Windows.Forms.Label rocXValue;
        private System.Windows.Forms.Label powerValue;
        private System.Windows.Forms.Label incidentAngleYValue;
        private System.Windows.Forms.Label incidentAngleXValue;
        private System.Windows.Forms.Label rmsLabel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label powerLabel;
        private System.Windows.Forms.Label incidentAngleYLabel;
        private System.Windows.Forms.Label incidentAngleXLabel;
        private System.Windows.Forms.Label incidentAngleLabel;
        private System.Windows.Forms.Label pvLabel;
        private System.Windows.Forms.Label strehlRatioLabel;
        private System.Windows.Forms.Label rhoYValue;
        private System.Windows.Forms.Label rhoYLabel;
        private System.Windows.Forms.Label rhoXLabel;
        private System.Windows.Forms.Label rocYValue;
        private System.Windows.Forms.Label rocYLabel;
        private System.Windows.Forms.Label rocXLabel;
    }
}

