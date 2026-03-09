using Optical.API.Library.Device;
using Optical.Core.Camera;
using Optical.Enums;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test.Core.Camera
{
    public partial class cameraTestForm : Form
    {
        #region Fields
        private IReadOnlyList<CameraComponents> cameraList;
        private ISensorCamera camera;
        private ListViewItem selectDeviceName;
        private static Dictionary<string, CameraComponents> cameraTable;
        private ImageContainer frameContainer;
        private int[] colorMap;
        private int positionX;
        private int positionY;
        private SemaphoreSlim drawingSemaphore = new SemaphoreSlim(1, 1);
        #endregion // Fields

        #region Methods
        private void changedPropertyItem()
        {
            // データバインドのコントロールプロパティ検証操作
            ProcessTabKey(true);
        }

        private void clearDataBindings()
        {
            autoExposureComboBox.DataBindings.Clear();
            autoExposureLowerTextBox.DataBindings.Clear();
            autoExposureLowerRangenMinimumValueLabel.DataBindings.Clear();
            autoExposureLowerRangenMaximumValueLabel.DataBindings.Clear();
            autoExposureUpperTextBox.DataBindings.Clear();
            autoExposureUpperRangenMinimumValueLabel.DataBindings.Clear();
            autoExposureUpperRangenMaximumValueLabel.DataBindings.Clear();
            autoGainComboBox.DataBindings.Clear();
            autoGainLowerTextBox.DataBindings.Clear();
            autoGainLowerRangenMinimumValueLabel.DataBindings.Clear();
            autoGainLowerRangenMaximumValueLabel.DataBindings.Clear();
            autoGainUpperTextBox.DataBindings.Clear();
            autoGainUpperRangenMinimumValueLabel.DataBindings.Clear();
            autoGainUpperRangenMaximumValueLabel.DataBindings.Clear();
            binningTextBox.DataBindings.Clear();
            binningModeComboBox.DataBindings.Clear();
            bitDepthTextBox.DataBindings.Clear();
            blackLevelTextBox.DataBindings.Clear();
            exposureTimeTextBox.DataBindings.Clear();
            exposureTimeRangeMinimumValueLabel.DataBindings.Clear();
            exposureTimeRangeMaximumValueLabel.DataBindings.Clear();
            frameRateTextBox.DataBindings.Clear();
            frameRateEnabledComboBox.DataBindings.Clear();
            frameRateRangeMinimumValueLabel.DataBindings.Clear();
            frameRateRangeMaximumValueLabel.DataBindings.Clear();
            gainTextBox.DataBindings.Clear();
            gainRangeMinimumValueLabel.DataBindings.Clear();
            gainRangeMaximumValueLabel.DataBindings.Clear();
            heightValueLabel.DataBindings.Clear();
            isGrabbingStateLabel.DataBindings.Clear();
            isOpenedStateLabel.DataBindings.Clear();
            pixelFormatComboBox.DataBindings.Clear();
            triggerModeComboBox.DataBindings.Clear();
            triggerTypeComboBox.DataBindings.Clear();
            widthValueLabel.DataBindings.Clear();
        }

        private void close()
        {
            if (camera == null)
            {
                return;
            }

            if (camera.IsOpened)
            {
                camera.Close();
                updateViewControls(true, false);
                isOpenedStateLabel.Text = "Flase";
            }

            camera = null;
            pixelDisplayLabel.ResetText();
            coordinateDisplayLabel.ResetText();
        }

        private void start()
        {
            if (camera == null)
            {
                showStatusMessage("Please select a device and Open.");
                MessageBox.Show("Please select a device and Open.");
                return;
            }

            if (camera.IsGrabbing)
            {
                return;
            }

            camera.Start();
            //updatePropertyControls(false);
            isGrabbingStateLabel.Text = $"{camera.IsGrabbing}";
            pixelDisplayLabel.Text = $"{camera.Width} x {camera.Height}";
            showStatusMessage("Acquisition Start...");
        }

        private void getAutoExposureRangen()
        {
            autoExposureLowerRangenMinimumValueLabel.Text = $"{camera.AutoExposureLowerRange.Minimum}";
            autoExposureLowerRangenMaximumValueLabel.Text = $"{camera.AutoExposureLowerRange.Maximum}";
            autoExposureUpperRangenMinimumValueLabel.Text = $"{camera.AutoExposureUpperRange.Minimum}";
            autoExposureUpperRangenMaximumValueLabel.Text = $"{camera.AutoExposureUpperRange.Maximum}";
        }

        private void getAutoGainRange()
        {
            autoGainLowerRangenMinimumValueLabel.Text = $"{camera.AutoGainLowerRange.Minimum}";
            autoGainLowerRangenMaximumValueLabel.Text = $"{camera.AutoGainLowerRange.Maximum}";
            autoGainUpperRangenMinimumValueLabel.Text = $"{camera.AutoGainUpperRange.Minimum}";
            autoGainUpperRangenMaximumValueLabel.Text = $"{camera.AutoGainUpperRange.Maximum}";
        }

        private string generateCameraInfo(CameraComponents cameraInfo)
        {
            string deviceName = $"{cameraInfo.VendorName},{cameraInfo.ModelName} (DeviceId:{cameraInfo.DeviceId})(SerialNumber:{cameraInfo.SerialNumber})";
            return deviceName;
        }

        private void Camera_NewFrame(object sender, ImageContainer e)
        {
            if (!drawingSemaphore.Wait(0))
            {
                return;
            }

            frameContainer = e;
            Task.Run(() =>
            {
                var bitmap = new Bitmap(frameContainer.Width, frameContainer.Height, PixelFormat.Format32bppRgb);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

                // 最小有効Byte抽出
                int bytesPerPixel = Bit.ToByte(camera.BitDepth);

                // 1byteデータ変換用ビットシフト抽出
                int shiftBit = camera.BitDepth - 8;
                if (shiftBit <= 0)
                {
                    shiftBit = 0;
                }

                colorMap = new int[frameContainer.Width * frameContainer.Height];

                switch (bytesPerPixel)
                {
                    case 1:
                        colorMap = getPixelConvertedValue((byte[])frameContainer.RawData.Pixels, shiftBit);
                        break;
                    case 2:
                        ushort[] viewPixels2ByteBuffer = new ushort[frameContainer.Width * frameContainer.Height];
                        colorMap = getPixelConvertedValue(viewPixels2ByteBuffer, shiftBit);
                        break;
                    case 4:
                        uint[] viewPixels4ByteBuffer = new uint[frameContainer.Width * frameContainer.Height];
                        colorMap = getPixelConvertedValue(viewPixels4ByteBuffer, shiftBit);
                        break;
                    case 8:
                        ulong[] viewPixels8ByteBuffer = new ulong[frameContainer.Width * frameContainer.Height];
                        colorMap = getPixelConvertedValue(viewPixels8ByteBuffer, shiftBit);
                        break;
                }

                Marshal.Copy(colorMap, 0, bitmapData.Scan0, colorMap.Length);
                bitmap.UnlockBits(bitmapData);

                Invoke((MethodInvoker)(() =>
                {
                    var bitmapOld = cameraProfilePictureBox.Image as Bitmap;
                    cameraProfilePictureBox.Image = bitmap;
                    bitmapOld?.Dispose();

                    // 輝度値出力 
                    if (!camera.TriggerMode)
                    {
                        outputDisplayBrightness();
                    }

                    frameDisplayLabel.Text = $"[Frame] {frameContainer.FrameNumber}";
                }));

                drawingSemaphore.Release();
            });
        }

        private int[] getPixelConvertedValue<T>(T[] viewPixelsBuffer, int shiftBit)
        {
            Buffer.BlockCopy(frameContainer.RawData.Pixels, 0, viewPixelsBuffer, 0, frameContainer.RawData.Pixels.Length);

            Parallel.For(0, frameContainer.Height, row =>
            {
                for (int index = row * frameContainer.Width; index < ((row + 1) * frameContainer.Width); index++)
                {
                    ulong pixelValue = Convert.ToUInt64(viewPixelsBuffer[index]) >> shiftBit;
                    int gray = Convert.ToInt32(pixelValue);
                    colorMap[index] = (int)(0xff000000 + (gray << 16) + (gray << 8) + gray);
                }
            });

            return colorMap;
        }

        private void initializeDataBindings()
        {
            autoExposureComboBox.DataBindings.Add(nameof(autoExposureComboBox.Text), camera, nameof(camera.AutoExposure));
            autoExposureLowerTextBox.DataBindings.Add(nameof(autoExposureLowerTextBox.Text), camera, nameof(camera.AutoExposureLower));
            autoExposureLowerRangenMinimumValueLabel.DataBindings.Add(nameof(autoExposureLowerRangenMinimumValueLabel.Text), camera.AutoExposureLowerRange, nameof(camera.AutoExposureLowerRange.Minimum));
            autoExposureLowerRangenMaximumValueLabel.DataBindings.Add(nameof(autoExposureLowerRangenMaximumValueLabel.Text), camera.AutoExposureLowerRange, nameof(camera.AutoExposureLowerRange.Maximum));
            autoExposureUpperTextBox.DataBindings.Add(nameof(autoExposureUpperTextBox.Text), camera, nameof(camera.AutoExposureUpper));
            autoExposureUpperRangenMinimumValueLabel.DataBindings.Add(nameof(autoExposureUpperRangenMinimumValueLabel.Text), camera.AutoExposureUpperRange, nameof(camera.AutoExposureUpperRange.Minimum));
            autoExposureUpperRangenMaximumValueLabel.DataBindings.Add(nameof(autoExposureUpperRangenMaximumValueLabel.Text), camera.AutoExposureUpperRange, nameof(camera.AutoExposureUpperRange.Maximum));
            autoGainComboBox.DataBindings.Add(nameof(autoGainComboBox.Text), camera, nameof(camera.AutoGain));
            autoGainLowerTextBox.DataBindings.Add(nameof(autoGainLowerTextBox.Text), camera, nameof(camera.AutoGainLower));
            autoGainLowerRangenMinimumValueLabel.DataBindings.Add(nameof(autoGainLowerRangenMinimumValueLabel.Text), camera.AutoGainLowerRange, nameof(camera.AutoGainLowerRange.Minimum));
            autoGainLowerRangenMaximumValueLabel.DataBindings.Add(nameof(autoGainLowerRangenMaximumValueLabel.Text), camera.AutoGainLowerRange, nameof(camera.AutoGainLowerRange.Maximum));
            autoGainUpperTextBox.DataBindings.Add(nameof(autoGainUpperTextBox.Text), camera, nameof(camera.AutoGainUpper));
            autoGainUpperRangenMinimumValueLabel.DataBindings.Add(nameof(autoGainUpperRangenMinimumValueLabel.Text), camera.AutoGainUpperRange, nameof(camera.AutoGainUpperRange.Minimum));
            autoGainUpperRangenMaximumValueLabel.DataBindings.Add(nameof(autoGainUpperRangenMaximumValueLabel.Text), camera.AutoGainUpperRange, nameof(camera.AutoGainUpperRange.Maximum));
            binningTextBox.DataBindings.Add(nameof(binningTextBox.Text), camera, nameof(camera.Binning));
            binningModeComboBox.DataBindings.Add(nameof(binningModeComboBox.Text), camera, nameof(camera.BinningMode));
            bitDepthTextBox.DataBindings.Add(nameof(bitDepthTextBox.Text), camera, nameof(camera.BitDepth));
            blackLevelTextBox.DataBindings.Add(nameof(blackLevelTextBox.Text), camera, nameof(camera.BlackLevel));
            exposureTimeTextBox.DataBindings.Add(nameof(exposureTimeTextBox.Text), camera, nameof(camera.ExposureTime));
            exposureTimeRangeMinimumValueLabel.DataBindings.Add(nameof(exposureTimeRangeMinimumValueLabel.Text), camera.ExposureTimeRange, nameof(camera.ExposureTimeRange.Minimum));
            exposureTimeRangeMaximumValueLabel.DataBindings.Add(nameof(exposureTimeRangeMaximumValueLabel.Text), camera.ExposureTimeRange, nameof(camera.ExposureTimeRange.Maximum));
            frameRateTextBox.DataBindings.Add(nameof(frameRateTextBox.Text), camera, nameof(camera.FrameRate));
            frameRateEnabledComboBox.DataBindings.Add(nameof(frameRateEnabledComboBox.Text), camera, nameof(camera.FrameRateEnabled));
            frameRateRangeMinimumValueLabel.DataBindings.Add(nameof(frameRateRangeMinimumValueLabel.Text), camera.FrameRateRange, nameof(camera.FrameRateRange.Minimum));
            frameRateRangeMaximumValueLabel.DataBindings.Add(nameof(frameRateRangeMaximumValueLabel.Text), camera.FrameRateRange, nameof(camera.FrameRateRange.Maximum));
            gainTextBox.DataBindings.Add(nameof(gainTextBox.Text), camera, nameof(camera.Gain));
            gainRangeMinimumValueLabel.DataBindings.Add(nameof(gainRangeMinimumValueLabel.Text), camera.GainRange, nameof(camera.GainRange.Minimum));
            gainRangeMaximumValueLabel.DataBindings.Add(nameof(gainRangeMaximumValueLabel.Text), camera.GainRange, nameof(camera.GainRange.Maximum));
            heightValueLabel.DataBindings.Add(nameof(heightValueLabel.Text), camera, nameof(camera.Height));
            isGrabbingStateLabel.DataBindings.Add(nameof(isGrabbingStateLabel.Text), camera, nameof(camera.IsGrabbing));
            isOpenedStateLabel.DataBindings.Add(nameof(isOpenedStateLabel.Text), camera, nameof(camera.IsOpened));
            pixelFormatComboBox.DataBindings.Add(nameof(pixelFormatComboBox.Text), camera, nameof(camera.PixelFormat));
            triggerModeComboBox.DataBindings.Add(nameof(triggerModeComboBox.Text), camera, nameof(camera.TriggerMode));
            var triggerTypeBind = new Binding(nameof(triggerTypeComboBox.Text), camera, nameof(camera.TriggerType));
            triggerTypeBind.Parse += stringToTriggerType;
            triggerTypeComboBox.DataBindings.Add(triggerTypeBind);
            widthValueLabel.DataBindings.Add(nameof(widthValueLabel.Text), camera, nameof(camera.Width));
        }

        private void initializeDeviceList()
        {
            cameraTable = new Dictionary<string, CameraComponents>();
            updateDeviceList();
            showStatusMessage("Please select a Device.");
        }

        private void open()
        {
            if (selectDeviceName == null)
            {
                showStatusMessage("Please select a Device.");
                MessageBox.Show("Please select a Device.");
                return;
            }

            if (camera != null)
            {
                close();
            }

            string displayShowDevice = null;
            string serialNumber = string.Empty;
            foreach (var cameraInfo in cameraTable)
            {
                if (cameraInfo.Key == selectDeviceName.Text)
                {
                    var classType = Type.GetType(cameraInfo.Value.Identifier);
                    camera = (ISensorCamera)Activator.CreateInstance(classType, cameraInfo.Value);
                    camera.NewFrame += Camera_NewFrame;

                    displayShowDevice = cameraInfo.Key;
                    serialNumber = cameraInfo.Value.SerialNumber;
                }
            }

            camera.Open();

            initializeDataBindings();
            updateViewControls(false, true);
            showStatusMessage($"{displayShowDevice}\nOpen successfull.");
        }

        private void outputDisplayBrightness()
        {
            if (colorMap == null)
            {
                return;
            }

            if (coordinateDisplayLabel.Text != "")
            {
                int x = positionX;
                int y = positionY;
                int bytes = Bit.ToByte(frameContainer.RawData.BitDepth);
                int imageWidth = frameContainer.Width * bytes;
                int brightness = 0;
                for (int shift = 0; shift < bytes; shift++)
                {
                    brightness += frameContainer.RawData.Pixels[(imageWidth * y) + (x * bytes) + shift] << (8 * shift);
                }

                brightnessDisplayLabel.Text = $"[Brightness] {brightness}";
            }
        }

        private void selectDevice()
        {
            if (camera != null)
            {
                close();
            }

            if (deviceListView.SelectedItems.Count > 0)
            {
                selectDeviceName = deviceListView.SelectedItems[0];
                showStatusMessage("Perform the 'Operation'.");
            }
            else
            {
                selectDeviceName = null;
                showStatusMessage("Please select a Device.");
            }
        }

        private void showStatusMessage(string message)
        {
            statusMessageLabel.Text = message;
        }

        private void snapShot()
        {
            if (camera == null)
            {
                showStatusMessage("Please select a device and Open.");
                MessageBox.Show("Please select a device and Open.");
                return;
            }

            if ((camera.TriggerMode != true) || (camera.TriggerType != TriggerInput.Software))
            {
                showStatusMessage("Set 「TriggerMode」 to \"True\" and 「TriggerType」to \"Software\".");
                MessageBox.Show("Set 「TriggerMode」 to \"True\" and 「TriggerType」to \"Software\".");
            }

            camera.TakeSnapshot();
            showStatusMessage("SnapShot.");
        }

        private void stop()
        {
            if (camera == null)
            {
                return;
            }

            camera.Stop();
            //updatePropertyControls(true);
            isGrabbingStateLabel.Text = $"{camera.IsGrabbing}";
            showStatusMessage("Stop.");
        }

        private void stringToTriggerType(object sender, ConvertEventArgs e)
        {
            if (e.DesiredType != typeof(TriggerInput))
            {
                return;
            }

            e.Value = Enum.Parse(typeof(TriggerInput), e.Value.ToString());
        }

        private void updateDeviceList()
        {
            deviceListView.Items.Clear();
            cameraTable.Clear();

            cameraList = CameraSearch.EnumerateDevice();
            foreach (CameraComponents cameraInfo in cameraList)
            {
                string deviceName = generateCameraInfo(cameraInfo);
                deviceListView.Items.Add(deviceName);
                cameraTable.Add(deviceName, cameraInfo);
            }
        }

        private void updatePropertyControls(bool enabledControls)
        {
            binningTextBox.Enabled = enabledControls;
            bitDepthTextBox.Enabled = enabledControls;
            formatListButton.Enabled = enabledControls;
            pixelFormatComboBox.Enabled = enabledControls;
        }

        private void updateViewControls(bool enabledListControls, bool enabledGrabControls)
        {
            deviceListView.Enabled = enabledListControls;
            upDateDviceListButton.Enabled = enabledListControls;
            cameraProfilePictureBox.Enabled = enabledGrabControls;
            messageAndPropertySplitContainer.Panel2.Enabled = enabledGrabControls;
        }
        #endregion // Methods
    }
}
