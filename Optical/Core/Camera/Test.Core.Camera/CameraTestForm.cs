using System;
using System.Linq;
using System.Windows.Forms;

namespace Test.Core.Camera
{
    public partial class cameraTestForm : Form
    {
        #region Methods
        public cameraTestForm()
        {
            InitializeComponent();
            initializeDeviceList();
        }

        private void cameraTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            close();
        }

        private void cameraProfilePictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (camera.IsGrabbing)
            {
                coordinateDisplayLabel.Text = e.Location.ToString();

                positionX = e.Location.X;
                positionY = e.Location.Y;
                // 輝度出力
                if (camera.TriggerMode)
                {
                    outputDisplayBrightness();
                }
            }
        }

        private void cameraProfilePictureBox_MouseLeave(object sender, EventArgs e)
        {
            coordinateDisplayLabel.ResetText();
            brightnessDisplayLabel.ResetText();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            close();
            clearDataBindings();
            deviceListView.SelectedItems.Clear();
        }

        private void deviceListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectDevice();
        }

        private void startButton_Click(object sender, EventArgs e) => start();

        private void openButton_Click(object sender, EventArgs e)
        {
            open();
        }

        private void snapShotButton_Click(object sender, EventArgs e)
        {
            snapShot();
        }

        private void upDateDviceListButton_Click(object sender, EventArgs e)
        {
            updateDeviceList();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stop();
        }

        // PropertyEventMethods
        private void autoExposureComboBox_DropDownClosed(object sender, EventArgs e)
        {
            autoExposureComboBox.Text = Convert.ToString(autoExposureComboBox.SelectedItem);
            changedPropertyItem();
        }

        private void autoExposureLowerTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void autoExposureLowerTextBox_Validated(object sender, EventArgs e)
        {
            getAutoExposureRangen();
        }

        private void autoExposureUpperTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void autoExposureUpperTextBox_Validated(object sender, EventArgs e)
        {
            getAutoExposureRangen();
        }

        private void autoGainComboBox_DropDownClosed(object sender, EventArgs e)
        {
            autoGainComboBox.Text = Convert.ToString(autoGainComboBox.SelectedItem);
            changedPropertyItem();
        }

        private void autoGainLowerTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void autoGainLowerTextBox_Validated(object sender, EventArgs e)
        {
            getAutoGainRange();
        }

        private void autoGainUpperTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void autoGainUpperTextBox_Validated(object sender, EventArgs e)
        {
            getAutoGainRange();
        }

        private void binningTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void binningModeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            binningModeComboBox.Text = Convert.ToString(binningModeComboBox.SelectedItem);
            changedPropertyItem();
        }

        private void bitDepthTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            statusMessageLabel.ResetText();
            if (camera.PixelFormatList.Contains($"Mono{bitDepthTextBox.Text}"))
            {
                changedPropertyItem();
            }
            else
            {
                showStatusMessage("Please Set the value corresponding to the 「PixelFormat」.");
            }
        }

        private void bitDepthTextBox_Leave(object sender, EventArgs e)
        {
            statusMessageLabel.ResetText();
            if (!camera.PixelFormatList.Contains($"Mono{bitDepthTextBox.Text}"))
            {
                bitDepthTextBox.Focus();
                showStatusMessage("Please Set the value corresponding to the 「PixelFormat」.");
            }
            else
            {
                changedPropertyItem();
            }
        }

        private void blackLevelTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void exposureTimeTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void frameRateTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void frameRateEnabledComboBox_DropDownClosed(object sender, EventArgs e)
        {
            frameRateEnabledComboBox.Text = Convert.ToString(frameRateEnabledComboBox.SelectedItem);
            changedPropertyItem();
        }

        private void gainTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                changedPropertyItem();
            }
        }

        private void messageAndPropertySplitContainer_Panel2_Click(object sender, EventArgs e)
        {
            changedPropertyItem();
        }

        private void pixelFormatComboBox_DropDownClosed(object sender, EventArgs e)
        {
            pixelFormatComboBox.Text = Convert.ToString(pixelFormatComboBox.SelectedItem);
            changedPropertyItem();
        }

        private void formatListButton_Click(object sender, EventArgs e)
        {
            pixelFormatComboBox.Enabled = true;
            pixelFormatComboBox.Items.Clear();

            var formatList = camera.PixelFormatList;
            for (int index = 0; index < formatList.Length; index++)
            {
                pixelFormatComboBox.Items.Add(formatList[index]);
            }
        }

        private void triggerModeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            triggerModeComboBox.Text = Convert.ToString(triggerModeComboBox.SelectedItem);
            changedPropertyItem();
        }

        private void triggerTypeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            triggerTypeComboBox.Text = Convert.ToString(triggerTypeComboBox.SelectedItem);
            changedPropertyItem();
        }
        #endregion // Methods
    }
}
