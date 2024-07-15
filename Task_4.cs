using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace conversion
{
    public class Converter{
    public static Bitmap bit_2_eight(Bitmap bitmap1bit){
        int width = bitmap1bit.Width, height = bitmap1bit.Height;
        Bitmap bitmap8bit = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

        ColorPalette palette = bitmap8bit.Palette;

        for(int i = 0; i<255; i++){
            palette.Entries[i] = Color.FromArgb(i,i,i);
        }

        bitmap8bit.Palette = palette;

        BitmapData bitmapData1bit = bitmap1bit.LockBits(new Rectangle(0,0,width, height), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
        BitmapData bitmapData8bit = bitmap8bit.LockBits(new Rectangle(0,0,width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        unsafe{
            byte* ptr1bit = (byte*)(void*)bitmapData1bit.Scan0;
            byte* ptr8bit = (byte*)(void*)bitmapData8bit.Scan0;

            for(int y=0; y<height; y++){
                // temp = y*stride
                for(int x=0; x<width; x++){     // 1000 0000                       1000 0000
                    byte bit = (byte)(ptr1bit[y * bitmapData1bit.Stride + x/8] & (0x80 >> (x%8)));
                    ptr8bit[y * bitmapData8bit.Stride + x] = (byte)(bit > 0 ? 255:0);
                }
            }
        }

        bitmap1bit.UnlockBits(bitmapData1bit);
        bitmap8bit.UnlockBits(bitmapData8bit);


        return bitmap8bit;
    }

    public static void Verify(Bitmap bitmap){
        
            Console.WriteLine($"Image Width: {bitmap.Width}");
            Console.WriteLine($"Image Height: {bitmap.Height}");
            Console.WriteLine($"Pixel Format: {bitmap.PixelFormat}");

            bool is8Bit = bitmap.PixelFormat == PixelFormat.Format8bppIndexed;
            if (is8Bit)
            {
                Console.WriteLine("The image is 8-bit.");
            }
            else
            {
                Console.WriteLine("The image is not 8-bit.");
            }
        
    }
    static void Main(string[] args){

        if(args.Length < 1){
            Console.WriteLine("Usage: mono Task_4.exe <path/to/1-bit/img>");
            return;
        }

        string inputPath = args[0];
        string uuid = Guid.NewGuid().ToString();
        string outputPath = $"8_bit_{uuid}.jpeg";
        Bitmap bitmap1bit = new Bitmap(inputPath);
        Bitmap result = bit_2_eight(bitmap1bit);
        result.Save(outputPath, ImageFormat.Jpeg);
        Console.WriteLine("Verifying Correct Conversion...");
        Verify(new Bitmap(inputPath));
        Console.WriteLine("-----------------------------------------------------------");
        Verify(result);
    }
}
}
