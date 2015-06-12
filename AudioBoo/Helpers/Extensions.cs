using ImageTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AudioBoo.Helpers
{
    public static class Extensions
    {
        public static BitmapImage ToBitmapImage(this byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);
                return bitmapImage;
            }
        }

        public static Image ToImage(this Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                var bytes = binaryReader.ReadBytes((int)stream.Length);
                BitmapImage bi = bytes.ToBitmapImage();
                Image i = new Image();
                i.Source = bi;
                return i;
            }

        }

        public static byte[] ToByteArray(this ExtendedImage extendedImage)
        {
            using (Stream outStream = extendedImage.ToStream())
            {
                using (BinaryReader binaryReader2 = new BinaryReader(outStream))
                {
                    var bytesOut = binaryReader2.ReadBytes((int)outStream.Length);
                    return bytesOut;
                }
            }
        }
    }
}
