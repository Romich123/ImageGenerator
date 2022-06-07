using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageGenerator
{
	public static class BitmapExtensions
	{
		public static byte[] GetBytes(this Bitmap bmp)
		{
			byte[] result = new byte[bmp.Width * bmp.Height * Image.GetPixelFormatSize(bmp.PixelFormat)];

			var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

			int bytesLength = Math.Abs(bmpData.Stride) * bmp.Height;
			Marshal.Copy(bmpData.Scan0, result, 0, bytesLength);

			bmp.UnlockBits(bmpData);

			return result;
		}
	}
}
