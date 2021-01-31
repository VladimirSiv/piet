using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Piet.Core
{
    public class Image
    {
        public static Bitmap ResizeImage(String imageFile, Measure measure, int cornerRadius)
        {
            using (var image = System.Drawing.Image.FromFile(imageFile))
            {
                // Resizing
                //
                Rectangle destRect = new Rectangle(0, 0, measure.width, measure.height);
                Bitmap resizedImage = new Bitmap(measure.width, measure.height);
                resizedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (Graphics graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                // Corner - We have to fill path again to take care of anti-aliasing
                //
                if (cornerRadius != 0)
                {
                    cornerRadius *= 2;
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
                    gp.AddArc(0 + measure.width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
                    gp.AddArc(0 + measure.width - cornerRadius, 0 + measure.height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    gp.AddArc(0, 0 + measure.height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                    Bitmap corneredImage = new Bitmap(measure.width, measure.height);
                    using (Graphics graphics = Graphics.FromImage(corneredImage))
                    {
                        graphics.Clear(Color.Transparent);
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        Brush brush = new TextureBrush(resizedImage);
                        graphics.FillPath(brush, gp);
                    }
                    return corneredImage;
                }
                return resizedImage;
            }
        }
    }
}
