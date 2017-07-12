using System;
using System.Drawing;

namespace ISHIHARA
{
    public class ImageManager
    {
        private Bitmap _bitmap;
        public Bitmap Bitmap {
            get {
                return _bitmap;
            }
        }

        public ImageManager(Bitmap bitmap)
        {
            _bitmap = bitmap;
        }

        private static Image ScaleImage(Image oldBitmap, int newWidth, int newHeight)
        {
            float sx = (float)newWidth / (float)oldBitmap.Width;
            float sy = (float)newHeight / (float)oldBitmap.Height;

            var newBitmap = new Bitmap(newWidth, newHeight);
            var graphics = Graphics.FromImage(newBitmap);

            graphics.ScaleTransform(sx, sy);
            graphics.DrawImage(oldBitmap, new Rectangle(0, 0, oldBitmap.Width, oldBitmap.Height));

            return newBitmap;
        }

        private Rectangle GetDotRectangle(Point dotSelectedPoint)
        {
            var x = dotSelectedPoint.X;
            var y = dotSelectedPoint.Y;

            Color color = _bitmap.GetPixel(x, y);

            do
            {
                x--;
                if (x == 0)
                    break;

                color = _bitmap.GetPixel(x, y);
            } while (color.R == Color.Black.R ||  color.G == Color.Black.G ||  color.B == Color.Black.B);
            var startX = x;
            x = dotSelectedPoint.X;
            do
            {
                x++;
                if (x == _bitmap.Width)
                {
                    x--;
                    break;
                }

                color = _bitmap.GetPixel(x, y);
            } while (color.R == Color.Black.R || color.G == Color.Black.G ||  color.B == Color.Black.B);
            var endX = x;
            var centerX = (endX - startX) / 2 + startX;
            do
            {
                y--;
                if (y == 0)
                    break;

                color = _bitmap.GetPixel(centerX, y);
            } while (color.R == Color.Black.R ||  color.G == Color.Black.G || color.B == Color.Black.B);
            var startY = y;
            y = dotSelectedPoint.Y;
            do
            {
                y++;
                if (y == _bitmap.Height)
                {
                    y--;
                    break;
                }

                color = _bitmap.GetPixel(centerX, y);
            } while (color.R == Color.Black.R ||  color.G == Color.Black.G ||  color.B == Color.Black.B);
            var endY = y;
            var centerY = (endY - startY) / 2 + startY;

            x = centerX;
            do
            {
                x--;
                if (x == 0)
                    break;

                color = _bitmap.GetPixel(x, centerY);
            } while (color.R == Color.Black.R || color.G == Color.Black.G || color.B == Color.Black.B);
            startX = x;

            x = centerX;
            do
            {
                x++;
                if (x == _bitmap.Width)
                {
                    x--;
                    break;
                }

                color = _bitmap.GetPixel(x, centerY);
            } while (color.R == Color.Black.R || color.G == Color.Black.G ||  color.B == Color.Black.B);
            endX = x;


            return new Rectangle(startX, startY, endX - startX, endY - startY);
        }


        public void ColorDot(Rectangle dotArea, Color newColor)
        {
            for (int x = dotArea.X; x <= dotArea.X + dotArea.Width; x++)
            {
                for (int y = dotArea.Y; y <= dotArea.Y + dotArea.Height; y++)
                {
                    Color pixel = _bitmap.GetPixel(x, y);

                    if (pixel.R == Color.Black.R && pixel.G == Color.Black.G &&  pixel.B == Color.Black.B) {
                        _bitmap.SetPixel(x, y, newColor);
                    }
                }
            }
        }

        public void ColorTextInDotImage(Bitmap textImage, Color textColor, Color backgroundColor)
        {
            if (textImage.Height != _bitmap.Height || textImage.Width != _bitmap.Width)
                textImage = new Bitmap(ScaleImage(textImage, _bitmap.Width, _bitmap.Height));

            var h = _bitmap.Height;
            var w = _bitmap.Width;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color textPixel = textImage.GetPixel(x, y);
                    Color bitmapPixel = _bitmap.GetPixel(x, y);
                    if (bitmapPixel.R == Color.Black.R && bitmapPixel.G == Color.Black.G && bitmapPixel.B == Color.Black.B)
                    {

                        if (textPixel.R == Color.Black.R && textPixel.G == Color.Black.G && textPixel.B == Color.Black.B)
                        {
                            var rectangle = GetDotRectangle(new Point(x, y));
                            ColorDot(rectangle, textColor);
                            x += rectangle.Width;
                        }
                        else
                        {
                            var rectangle = GetDotRectangle(new Point(x, y));
                            ColorDot(rectangle, backgroundColor);
                            x += rectangle.Width;
                        }
                    }
                }
            }
        }
    }
}
