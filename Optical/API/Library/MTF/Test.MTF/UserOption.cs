using System.ComponentModel;
using System.Drawing;

namespace Optical.API.Library
{
    public class UserOptions
    {
        private const int defaultMinThreshold = 50;
        private const int defaultMaxThreshold = 200;
        private const int defaultBitDepth = 8;

        public UserOptions()
        {
            MinThreshold = defaultMinThreshold;
            MaxThreshold = defaultMaxThreshold;
            BitDepth = defaultBitDepth;

            ROI = new Rectangle(31, 30, 336, 336);
            SpatialFrequency = 0.2;
        }

        [Category("Image")]
        [Description("Bit depth of the image.")]
        [DefaultValue(defaultBitDepth)]
        public int BitDepth { get; set; }

        [Category("Edge Detection")]
        [Description("Edge detection minimum threshold.")]
        [DefaultValue(defaultMinThreshold)]
        public int MinThreshold { get; set; }

        [Category("Edge Detection")]
        [Description("Edge detection maximum threshold.")]
        [DefaultValue(defaultMaxThreshold)]
        public int MaxThreshold { get; set; }

        [Category("Image")]
        [Description("MTF calculation target area.")]
        public Rectangle ROI { get; set; }

        [Category("MTF")]
        [Description("Spatial frequency[cycles/pixel]")]
        public double SpatialFrequency { get; set; }
    }
}
