using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Piet.Core
{
    public enum ArrangementMode
    {
        Column,
        Row
    }

    public enum HandlingType
    {
        Omit,
        CutOff
    }

    public class Grid
    {
        private Measure laneMaxMeasure;
        private ArrangementMode arrangementMode;
        private HandlingType handlingType;
        private List<String> imageFilePaths;
        private Lane[] lanes;
        private const int RowCutOffWidthExpand = 30;
        private int lanesNum;
        private int maxWidth;
        private int maxHeight;
        private int offsetSize;
        private int[] offsetColor;
        private int cornerRadius;

        public Grid(int lanesNum, ArrangementMode arrangementMode, HandlingType handlingType, List<String> imageFilePaths, int offsetSize, int[] offsetColor, int cornerRadius)
        {
            this.lanesNum = lanesNum;
            this.arrangementMode = arrangementMode;
            this.imageFilePaths = imageFilePaths;
            this.offsetSize = offsetSize;
            this.offsetColor = offsetColor;
            this.handlingType = handlingType;
            this.cornerRadius = cornerRadius;
            (maxWidth, maxHeight) = GetMaxScreenSize();
            this.laneMaxMeasure = arrangementMode == ArrangementMode.Column
                ? new Measure((maxWidth - (lanesNum + 1) * offsetSize) / lanesNum, maxHeight)
                : new Measure(maxWidth, (maxHeight - (lanesNum + 1) * offsetSize) / lanesNum);
            lanes = InitializeLanes();
            imageFilePaths.Shuffle();
        }

        public void SetBackground()
        {
            FirstFit();
            String imageFilepath = ExportImage();
            Wallpaper.Set(imageFilepath);
        }

        private void FirstFit()
        {
            List<int> laneTracker = new List<int>(Enumerable.Range(0, lanes.Length));
            int loops = 0;
            foreach (String imageFilePath in imageFilePaths)
            {
                if (!laneTracker.Any()) break;
                try
                {
                    using (var image = System.Drawing.Image.FromFile(imageFilePath))
                    {
                        for (int i = 0; i < laneTracker.Count; i++)
                        {
                            loops += 1;
                            if (lanes[laneTracker[i]].AddImage(imageFilePath, GetResizedHeight(image)))
                            {
                                if (lanes[laneTracker[i]].IsFull())
                                {
                                    laneTracker.Remove(laneTracker[i]);
                                }
                                break;
                            }
                        }
                    }
                }
                catch (OutOfMemoryException)
                {
                    // Failed to read an image, just continue
                }
            }
        }

        public String ExportImage()
        {
            Bitmap fullImage = arrangementMode == ArrangementMode.Column ? PackColumns() : PackRows();
            String imageFilepath = SaveImage(fullImage);
            fullImage.Dispose();
            return imageFilepath;
        }

        private Bitmap PackColumns()
        {
            Bitmap fullImage = new Bitmap(maxWidth, maxHeight);
            using (Graphics g = Graphics.FromImage(fullImage))
            {
                g.Clear(Color.FromArgb(offsetColor[0], offsetColor[1], offsetColor[2]));
                int x = offsetSize;
                foreach (Lane lane in lanes)
                {
                    using (var laneImage = lane.MergeImages())
                    {
                        g.DrawImage(laneImage, x, offsetSize, laneImage.Width, laneImage.Height);
                        x += laneImage.Width + offsetSize * (3 / 2);
                    }
                }
            }
            return fullImage;
        }

        private Bitmap PackRows()
        {
            Bitmap fullImage = new Bitmap(maxWidth, maxHeight);
            using (Graphics g = Graphics.FromImage(fullImage))
            {
                g.Clear(Color.FromArgb(offsetColor[0], offsetColor[1], offsetColor[2]));
                int y = offsetSize;
                foreach (Lane lane in lanes)
                {
                    using (var laneImage = lane.MergeImages())
                    {
                        g.DrawImage(laneImage, offsetSize, y, laneImage.Width, laneImage.Height);
                        y += laneImage.Height + offsetSize * (3 / 2);
                    }
                }
            }
            if (handlingType == HandlingType.CutOff)
            {
                return fullImage.Clone(new Rectangle(RowCutOffWidthExpand, 0, maxWidth - RowCutOffWidthExpand, maxHeight), fullImage.PixelFormat);
            }
            return fullImage;
        }

        private String SaveImage(Bitmap image)
        {
            ;
            String filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "piet_tmp_img.jpeg");
            if (File.Exists(filepath))
                File.Delete(filepath);
            image.Save(filepath, ImageFormat.Jpeg);
            image.Dispose();
            File.SetAttributes(filepath, FileAttributes.Hidden);
            return filepath;
        }

        private Lane[] InitializeLanes()
        {
            Lane[] lanes = new Lane[lanesNum];
            for (int i = 0; i < lanesNum; i++)
            {
                lanes[i] = new Lane(0, 0, laneMaxMeasure, offsetSize, arrangementMode, cornerRadius, handlingType);
            }
            return lanes;
        }

        private (int, int) GetMaxScreenSize()
        {
            Screen[] screens = Screen.AllScreens;
            int maxWidth = 0;
            int maxHeight = 0;
            foreach (Screen screen in screens)
            {
                maxWidth = screen.Bounds.Width > maxWidth
                    ? screen.Bounds.Width
                    : maxWidth;
                maxHeight = screen.Bounds.Height > maxHeight
                    ? screen.Bounds.Height
                    : maxHeight;
            }
            if (arrangementMode == ArrangementMode.Row && handlingType == HandlingType.CutOff)
            {
                // We add additional width so we can later crop it to get
                // the symmetrical look for row-cutoff combination
                maxWidth += RowCutOffWidthExpand;
            }
            return (maxWidth, maxHeight);
        }

        private Measure GetResizedHeight(System.Drawing.Image image)
        {
            if (arrangementMode == ArrangementMode.Column)
            {
                decimal scaleFactor = Decimal.Divide(laneMaxMeasure.width, image.Width);
                int height = Convert.ToInt32(image.Height * scaleFactor);
                return new Measure(laneMaxMeasure.width, height);
            }
            else
            {
                decimal scaleFactor = Decimal.Divide(laneMaxMeasure.height, image.Height);
                int width = Convert.ToInt32(image.Width * scaleFactor);
                return new Measure(width, laneMaxMeasure.height);
            }

        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
