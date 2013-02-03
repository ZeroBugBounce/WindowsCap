using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using WindowsCap;
using System.Drawing;
using System;

namespace ConsoleTestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var chromeImgs = WindowCapture.CaptureProcessWindows(
				Process.GetProcessesByName("chrome").First());

			// everything below here is just for saving:

			// Encoder parameter for image quality 
			EncoderParameter qualityParam =
				new EncoderParameter(Encoder.Quality, 90L);
			// Jpeg image codec 
			ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

			EncoderParameters encoderParams = new EncoderParameters(1);
			encoderParams.Param[0] = qualityParam;

			foreach (Bitmap image in chromeImgs)
			{
				image.Save(@"C:\temp\chrome" + new Random().Next().ToString() + ".jpg", jpegCodec, encoderParams);
			}
		}

		public static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			// Get image codecs for all image formats 
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			// Find the correct image codec 
			for (int i = 0; i < codecs.Length; i++)
				if (codecs[i].MimeType == mimeType)
					return codecs[i];
			return null;
		}
	}
}
