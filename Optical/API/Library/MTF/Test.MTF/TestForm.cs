using System;
using System.Windows.Forms;

namespace Optical.API.Library
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            initializeParameter();
        }

        private bool isDragging { get; set; }

        private void openImageFileButton_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.bmp;*.png;*.jpg)|*.bmp;*.png;*.jpg"
            };
            DialogResult result = openDialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            imageFilePath.Text = openDialog.FileName;
            drawImage(openDialog.FileName);
            drawRoi();
        }

        private void calculationButton_Click(object sender, EventArgs e)
        {
            calculateMTF();
        }

        private void userOptionPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e is null)
            {
                return;
            }

            // プロパティの名前を取得する
            string propertyName = e.ChangedItem?.PropertyDescriptor?.Name ?? string.Empty;

            // プロパティの値を取得する
            switch (propertyName)
            {
                case nameof(userOption.ROI.X):
                case nameof(userOption.ROI.Y):
                case nameof(userOption.ROI.Width):
                case nameof(userOption.ROI.Height):
                    drawRoi();
                    break;
                default:
                    break;
            }
        }

        private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            loadImage?.Dispose();
        }

        private void roiPictureBox_MouseDown(object? sender, MouseEventArgs e)
        {
            isDragging = true;
            startDragRoi(e.Location);
        }

        private void roiPictureBox_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!isDragging)
            {
                return;
            }

            isDragging = false;
            updateRoi(e.Location);
            endDragRoi();
            drawRoi();
        }

        private void roiPictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!isDragging)
            {
                return;
            }

            drawDragRoi(e.Location);
        }

        private void exportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dialogCaption = string.Empty;
            switch (contextMenuStrip1.SourceControl?.Name)
            {
                case nameof(esfProfilePictureBox):
                    dialogCaption = "Export ESF Profile";
                    exportEsfProfile();
                    break;
                case nameof(syntheticLsfProfilePictureBox):
                    dialogCaption = "Export Synthetic LSF Profile";
                    exportSyntheticLsfProfile();
                    break;
                case nameof(mtfBackGroundPictureBox):
                    dialogCaption = "Export MTF Profile";
                    exportMtfProfile();
                    break;
                default:
                    break;
            }
        }

        private void cannyMethodRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            mtfMethod = slantedEdgeMethod.Canny;
        }

        private void peterBurnsMethodRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            mtfMethod = slantedEdgeMethod.PeterBurns;
        }
    }
}
