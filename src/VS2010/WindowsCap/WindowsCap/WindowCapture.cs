using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace WindowsCap
{
	public static class WindowCapture
	{
		public static Bitmap CaptureScreen()
		{
			return CaptureWindow(User32.GetDesktopWindow());
		}

		public static Bitmap CaptureWindow(IntPtr handle)
		{
			// get te hDC of the target window
			IntPtr hdcSrc = User32.GetWindowDC(handle);
			// get the size
			User32.RECT windowRect = new User32.RECT();
			User32.GetWindowRect(handle, ref windowRect);
			int width = windowRect.right - windowRect.left;
			int height = windowRect.bottom - windowRect.top;
			// create a device context we can copy to
			IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
			// create a bitmap we can copy it to,
			// using GetDeviceCaps to get the width/height
			IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
			// select the bitmap object
			IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
			// bitblt over
			GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
			// restore selection
			GDI32.SelectObject(hdcDest, hOld);
			// clean up 
			GDI32.DeleteDC(hdcDest);
			User32.ReleaseDC(handle, hdcSrc);

			// get a .NET image object for it
			Bitmap img = Image.FromHbitmap(hBitmap);
			// free up the Bitmap object
			GDI32.DeleteObject(hBitmap);

			return img;
		}

		public static IList<Bitmap> CaptureProcessWindows(Process process)
		{
			List<Bitmap> images = new List<Bitmap>();
			foreach (ProcessThread thread in process.Threads)
			{
				List<IntPtr> windows = GetThreadWindows(thread.Id);

				foreach (IntPtr windowHandle in windows)
				{
					string getText = WinUtil.GetText(windowHandle);

					if (User32.IsIconic(windowHandle) || !User32.IsWindowVisible(windowHandle))
					{
						continue;
					}

					images.Add(CaptureWindow(windowHandle));
				}
			}

			return images;
		}

		public static List<IntPtr> GetThreadWindows(int threadId)
		{
			List<IntPtr> windowPointers = new List<IntPtr>();

			User32.EnumThreadWindows(threadId, delegate(IntPtr hWnd, IntPtr lParam)
			{
				windowPointers.Add(hWnd);
				return 1;
			}, 0);

			return windowPointers;
		}
	}
}
