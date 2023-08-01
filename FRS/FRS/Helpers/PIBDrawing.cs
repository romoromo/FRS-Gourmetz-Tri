using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.Helpers
{
    public class PIBDrawing
    {
        public static Image FixedSize(Image imgPhoto, int Width, int Height, bool makeTransparent, bool isAddFooter = false, int fHeight = 20, string fText = null, Font fFont = null, SolidBrush fBrush = null)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = null;

            if (isAddFooter)
            {
                bmPhoto = new Bitmap(Width, Height + fHeight);
            }
            else
            {
                bmPhoto = new Bitmap(Width, Height); //, PixelFormat.Format32bppArgb
            }
            //if (makeTransparent)
            //{
            //    bmPhoto.MakeTransparent();
            //}
            //bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
            //                 imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //grPhoto.Clear(Color.White);
            //grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            var desRect = new Rectangle(destX, destY, destWidth, destHeight);
            grPhoto.DrawImage(imgPhoto,
                desRect,
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            if (isAddFooter)
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                float charWidth = grPhoto.MeasureString("Y", fFont).Width;

                grPhoto.DrawString(fText, fFont, fBrush, destX + (desRect.Width / 2), destHeight + 10, sf);
            }

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static int SafeGet(object obj)
        {
            if (obj == null) return Int32.MaxValue;
            return Convert.ToInt32(obj);
        }

        public static Font GetFont(string fontFamily, float fontSize, FontStyle fontStyle)
        {
            Font drawFont = new Font("Calibri", fontSize, fontStyle);
            switch (fontFamily)
            {
                case "Tahoma":
                    drawFont = new Font("Tahoma", fontSize, fontStyle);
                    break;
                case "Calibri":
                    drawFont = new Font("Calibri", fontSize);
                    break;
                case "Arial":
                    drawFont = new Font("Arial", fontSize);
                    break;
                case "Times New Roman":
                    drawFont = new Font("Times New Roman", fontSize);
                    break;
                    //case "Roboto": break;
            }

            return drawFont;
        }

        public static Font GetAdjustedFont(Graphics g, string graphicString, Font originalFont, int containerWidth, int maxFontSize, int minFontSize, bool smallestOnFail)
        {
            Font testFont = null;
            // We utilize MeasureString which we get via a control instance           
            for (int adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);

                // Test the string with the new size
                SizeF adjustedSizeNew = g.MeasureString(graphicString, testFont);

                if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width))
                {
                    // Good font, return it
                    return testFont;
                }
            }

            // If you get here there was no fontsize that worked
            // return minimumSize or original?
            if (smallestOnFail)
            {
                return testFont;
            }
            else
            {
                return originalFont;
            }
        }

        public static Font GetAdjustedFontWrap(Graphics graphic, string str, Font originalFont, Size containerSize)
        {
            // We utilize MeasureString which we get via a control instance           
            for (int adjustedSize = (int)originalFont.Size; adjustedSize >= 1; adjustedSize--)
            {
                var testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style, GraphicsUnit.Pixel);

                // Test the string with the new size
                var adjustedSizeNew = graphic.MeasureString(str, testFont, containerSize.Width);

                if (containerSize.Height > Convert.ToInt32(adjustedSizeNew.Height))
                {
                    // Good font, return it
                    return testFont;
                }
            }

            return new Font(originalFont.Name, 1, originalFont.Style, GraphicsUnit.Pixel);
        }
    }
}
