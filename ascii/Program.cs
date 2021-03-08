using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Text;
namespace ascii
{
    class Program
    {
	[DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);
        private static void Maximize() => Task.Run(() => ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 3));
        static void Main(string[] args)
        {
	    Maximize();
            Console.Title = "Ascii_GIF_Player";
            Bitmap img = null;
            if (args.Length == 1)
            {
                try
                {
                    img = new Bitmap(@$"{args[0]}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
            else
            {
                try
                {
                    Console.WriteLine("please paste image path\n");
                    img = new Bitmap(@$"{Console.ReadLine()}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }

	    Stopwatch sw = new Stopwatch();
	    double fps = 0;
	    List<double> frameTimes = new List<double>();
            for (int i = 0; i < 15; i++) //arbitrary gif repeat
            {
                if (img != null)
                {
					
                    foreach (Bitmap map in GetGifFrames(img))
                    {
			sw.Start();
                        WriteImage(map);
			sw.Stop();
			fps = sw.ElapsedMilliseconds;
			frameTimes.Add(fps);
			Console.Title = $"Ascii_GIF_Player - {1000d / fps:n2} FPS";
			sw.Reset();
                        Console.SetCursorPosition(0, 0);
                    }
                }
            }
            Console.ReadLine();
        }

        static double GetPixelBrightness(Color color) => 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
        static IEnumerable<char> ConvertSingleImage(Bitmap image)
        {
            char[] greyScale = ".:-= +*#%@".ToCharArray();
            Array.Reverse(greyScale);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    double brightness = GetPixelBrightness(image.GetPixel(x, y));
                    yield return brightness >= 1 && 255 / (int)brightness < greyScale.Length - 1 ? greyScale[(int)Math.Floor(255 / brightness) - 1] : greyScale[greyScale.Length - 1];
                }
            }
        }
        static void WriteImage(Bitmap img)
        {
			
            int count = 0;
            List<char> buffer = new List<char>();
            foreach (char c in ConvertSingleImage(img))
            {
                buffer.Add(c);
                buffer.Add(' ');
                count++;

                if (count == img.Width)
                {
                    count = 0;
                    DumpCharListToConsole(ref buffer);
                }
            }
        }
        static IEnumerable<Bitmap> GetGifFrames(Bitmap gifToSplice)
        {
            int frameCount = gifToSplice.GetFrameCount(FrameDimension.Time);
            for (int i = 0; i < frameCount; i++)
            {
                gifToSplice.SelectActiveFrame(FrameDimension.Time, i);
                yield return (Bitmap)gifToSplice.Clone();
            }
        }

        static void DumpCharListToConsole(ref List<char> chars)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars.Count; i++)
            {
                sb.Append(chars[i]);
            }
            Console.WriteLine(sb);
            chars.Clear();
        }
    }
}
