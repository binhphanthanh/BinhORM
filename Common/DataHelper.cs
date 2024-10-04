using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace SystemFramework.Common
{
    public class DataHelper
    {
        public static byte[] ImageToByteArray(Image image)
        {
            if (image != null)
            {
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
            else
            {
                return null;
            }
        }

        public static Image ByteArrayToImage(byte[] bytes)
        {
            if (bytes != null)
            {
                MemoryStream stream = new MemoryStream(bytes);
                return Bitmap.FromStream(stream);
            }
            else
            {
                return null;
            }
        }

        public static DateTime RemoveTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static int GetRightNumber(string sourceString, int numberLenght)
        {
            int lastNumber = 0;
            if (!string.IsNullOrEmpty(sourceString))
            {
                try
                {
                    lastNumber = int.Parse(sourceString.Substring(sourceString.Length - numberLenght, numberLenght));
                }
                catch (Exception)
                {
                    lastNumber = 0;
                }
            }
            return lastNumber;
        }
    }
}
