using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace dipp
{
    public class GrayScaleConvert{

//     public static bool ConvertToGrayScale(Bitmap b){
        
//         Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
//         BitmapData BData = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

//         IntPtr ptr = BData.Scan0;
// //
//         int bytes = Math.Abs(BData.Stride) * b.Height;
//         byte[] rgbValues = new byte[bytes];

//         System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

//         for(int i = 0; i < rgbValues.Length;i+=3){
//             byte gray = (byte)(0.299 * rgbValues[i] + 0.587 * rgbValues[i+1] + 0.114 * rgbValues[i+2]);

//             rgbValues[i] = gray;
//             rgbValues[i+1] = gray;
//             rgbValues[i+2] = gray;
//         }

//         System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

        
//         b.UnlockBits(BData);

//         return true;
//     }

    public static Bitmap Conv_24to8bits(Bitmap img)
    {
        int width = img.Width;
        int height = img.Height;

        Bitmap newImg = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

        ColorPalette palette = newImg.Palette;
        for(int i = 0; i < 256; i++){
            palette.Entries[i] = Color.FromArgb(i, i, i);
        }
        newImg.Palette = palette;

        BitmapData imgData = img.LockBits(new Rectangle(0,0,width,height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData newImgData = newImg.LockBits(new Rectangle(0,0,width,height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
        
        int imgStride = imgData.Stride;
        int imgOffset = imgStride - (width*3);

        int newStride = newImgData.Stride;
        int newOffset = newStride - width;


        unsafe
        {
            byte* imgPtr = (byte*)(void*)imgData.Scan0;
            byte* newImgPtr = (byte*)(void*)newImgData.Scan0;

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    byte blue = imgPtr[0];
                    byte green = imgPtr[1];
                    byte red = imgPtr[2];

                    newImgPtr[0] = (byte)(0.299 * red + 0.587 * green + 0.114 * blue);


                    imgPtr += 3;
                    newImgPtr += 1;
                    
                }
                imgPtr += imgOffset;
                newImgPtr += newOffset;
            }
        }
        img.UnlockBits(imgData);
        newImg.UnlockBits(newImgData);

        return newImg;
    }

    public static void Main(string[] args){

        if(args.Length != 1){
            Console.WriteLine("Usage: Color_To_GS_Conv.exe <input_file> ");
            return;
        }

        string InputPath = args[0];
        string uuid = Guid.NewGuid().ToString();
        string OutputPath = $"ParrotsGrayScale_{uuid}.jpeg";

        Bitmap b = new Bitmap(InputPath);
        Bitmap output = Conv_24to8bits(b);
        output.Save(OutputPath, ImageFormat.Jpeg);
    }
}
}
