using System;
using System.Collections.Generic;
using System.Drawing;

namespace Piet.Core
{
    internal class Lane
    {
        private Measure laneMaxMeasure;
        private ArrangementMode arrangementMode;
        private HandlingType handlingType;
        private List<String> imageFiles = new List<String>();
        private List<Measure> imagesMeasure = new List<Measure>();
        private int x;
        private int y;
        private int offsetSize;
        private int height;
        private int width;
        private int cornerRadius;

        public Lane(int x, int y, Measure laneMaxMeasure, int offsetSize, ArrangementMode arrangementMode, int cornerRadius, HandlingType handlingType)
        {
            this.x = x;
            this.y = y;
            this.laneMaxMeasure = laneMaxMeasure;
            this.offsetSize = offsetSize;
            this.arrangementMode = arrangementMode;
            this.cornerRadius = cornerRadius;
            this.handlingType = handlingType;
        }

        public bool AddImage(String imageFilePath, Measure measure)
        {
            return (handlingType == HandlingType.Omit
                ? OmitHandling(imageFilePath, measure)
                : CutOffHandling(imageFilePath, measure));
        }

        public bool IsFull()
        {
            if (arrangementMode == ArrangementMode.Column)
            {
                if (handlingType == HandlingType.CutOff)
                {
                    return height > laneMaxMeasure.height;
                }
                else
                {
                    return (double)height / laneMaxMeasure.height >= 0.95;
                }

            }
            else
            {
                if (handlingType == HandlingType.CutOff)
                {
                    return width > laneMaxMeasure.width;
                }
                else
                {
                    return (double)width / laneMaxMeasure.width >= 0.95;
                }
            }
        }

        public Bitmap MergeImages()
        {
            Bitmap bitmap = new Bitmap(laneMaxMeasure.width, laneMaxMeasure.height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (arrangementMode == ArrangementMode.Column) PackColumn(g); else PackRow(g);
            }
            return bitmap;
        }

        private bool OmitHandling(String imageFilePath, Measure measure)
        {
            if (arrangementMode == ArrangementMode.Column)
            {
                if (height + measure.height < laneMaxMeasure.height)
                {
                    imageFiles.Add(imageFilePath);
                    imagesMeasure.Add(measure);
                    height += measure.height + offsetSize;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (width + measure.width < laneMaxMeasure.width)
                {
                    imageFiles.Add(imageFilePath);
                    imagesMeasure.Add(measure);
                    width += measure.width + offsetSize;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool CutOffHandling(String imageFilePath, Measure measure)
        {
            if (arrangementMode == ArrangementMode.Column)
            {
                if (height < laneMaxMeasure.height)
                {
                    imageFiles.Add(imageFilePath);
                    imagesMeasure.Add(measure);
                    height += measure.height + offsetSize;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (width < laneMaxMeasure.width)
                {
                    imageFiles.Add(imageFilePath);
                    imagesMeasure.Add(measure);
                    width += measure.width + offsetSize;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        private void PackColumn(Graphics g)
        {
            int localHeight = y;
            for (int i = 0; i < imageFiles.Count; i++)
            {
                using (var resizedImage = Image.ResizeImage(imageFiles[i], imagesMeasure[i], cornerRadius))
                {
                    g.DrawImage(resizedImage, x, localHeight, resizedImage.Width, resizedImage.Height);
                    localHeight += resizedImage.Height + offsetSize;
                }
            }
        }

        private void PackRow(Graphics g)
        {
            int localWidth = x;
            for (int i = 0; i < imageFiles.Count; i++)
            {
                using (var resizedImage = Image.ResizeImage(imageFiles[i], imagesMeasure[i], cornerRadius))
                {
                    g.DrawImage(resizedImage, localWidth, y, resizedImage.Width, resizedImage.Height);
                    localWidth += resizedImage.Width + offsetSize;
                }
            }
        }
    }
}
