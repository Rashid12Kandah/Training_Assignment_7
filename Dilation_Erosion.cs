using System;
using System.Drawing;
using System.Drawing.Imaging;
using dipp;
using conversion;

namespace dip
{
    public class Morphology
{
    public static Bitmap To_8Bit(Bitmap image){
        if(image.PixelFormat != PixelFormat.Format1bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb ){
            throw new Exception("Image format is not supported only 24-bit and 1-bit images are supported");
        }
        if(image.PixelFormat == PixelFormat.Format1bppIndexed){
            return Converter.bit_2_eight(image);
        }else{
            return GrayScaleConvert.Conv_24to8bits(image);
        }
    }

    public static unsafe Bitmap AddPadding(Bitmap _image)
    {
        int width = _image.Width, height = _image.Height;

        Bitmap image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

        if(_image.PixelFormat != PixelFormat.Format8bppIndexed)
        {
            image = To_8Bit(_image);
        }
        Bitmap paddedImage = new Bitmap(width + 2, height + 2, PixelFormat.Format8bppIndexed);

        ColorPalette grayscalePalette = paddedImage.Palette;
        for (int i = 0; i < 256; i++)
        {
            grayscalePalette.Entries[i] = Color.FromArgb(i, i, i);
        }
        paddedImage.Palette = grayscalePalette;
        
        BitmapData imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
        BitmapData paddedImageData = paddedImage.LockBits(new Rectangle(0, 0, width + 2, height + 2), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        byte* imagePtr = (byte*)imageData.Scan0.ToPointer();
        byte* paddedImagePtr = (byte*)paddedImageData.Scan0.ToPointer();

        int imageStride = imageData.Stride;
        int paddedImageStride = paddedImageData.Stride;

        for (int i = 0; i < height + 2; i++)
        {
            byte* currentPaddedImagePtr = paddedImagePtr + i * paddedImageStride;
            for (int j = 0; j < width + 2; j++)
            {
                currentPaddedImagePtr[j] = 0;
            }
        }

        for (int i = 0; i < height; i++)
        {
            byte* currentImagePtr = imagePtr + i * imageStride;
            byte* currentPaddedImagePtr = paddedImagePtr + (i + 1) * paddedImageStride + 1;
            for (int j = 0; j < width; j++)
            {
                currentPaddedImagePtr[j] = currentImagePtr[j];
            }
        }

        image.UnlockBits(imageData);
        paddedImage.UnlockBits(paddedImageData);

        return paddedImage;
    }
///////////////////////////
    public static Bitmap Dilation(Bitmap paddedImg, int [,] kernel)
    {
        int width = paddedImg.Width, height = paddedImg.Height;
        int originalW = paddedImg.Width - 2, originalH = paddedImg.Height - 2;
        int kernelW = kernel.GetLength(1), kernelH = kernel.GetLength(0); 
        Bitmap dilatedImg = new Bitmap(originalW, originalH, PixelFormat.Format8bppIndexed);

        ColorPalette grayscalePalette = dilatedImg.Palette;
        for (int i = 0; i < 256; i++)
        {
            grayscalePalette.Entries[i] = Color.FromArgb(i, i, i);
        }
        dilatedImg.Palette = grayscalePalette;


        BitmapData paddedImgData = paddedImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
        BitmapData dilatedImgData = dilatedImg.LockBits(new Rectangle(0, 0, originalW, originalH), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        int paddedImgStride = paddedImgData.Stride;
        int dilatedImgStride = dilatedImgData.Stride;

        unsafe
        {
            byte* paddedImgPtr = (byte*)paddedImgData.Scan0.ToPointer();
            byte* dilatedImgPtr = (byte*)dilatedImgData.Scan0.ToPointer();
            for (int i = 1; i < originalH; i++)
            {
                byte* currentDilatedImagePtr = dilatedImgPtr + i * dilatedImgStride;
                for (int j = 1; j < originalW; j++)
                {
                    int max = 0;
                    for (int k = 0; k < kernelH; k++)
                    {
                        for (int l = 0; l < kernelW; l++)
                        {
                            int pixel = paddedImgPtr[(i + k) * paddedImgStride + j + l];
                            if (pixel > max)
                            {
                                max = pixel;
                            }
                        }
                    }
                    currentDilatedImagePtr[j] = (byte)max;
                    }
                }
            }
            paddedImg.UnlockBits(paddedImgData);
            dilatedImg.UnlockBits(dilatedImgData);

            return dilatedImg;
        }

        public static Bitmap Erosion(Bitmap paddedImg, int[,] kernel)
        {
            int width = paddedImg.Width, height = paddedImg.Height;
            int originalW = paddedImg.Width - 2, originalH = paddedImg.Height - 2;
            int kernelW = kernel.GetLength(1), kernelH = kernel.GetLength(0);
            int kSize = 0;
            for (int i = 0; i < kernelH; i++)
            {
                for (int j = 0; j < kernelW; j++)
                {
                    if (kernel[i, j] == 255)
                    {
                        kSize++;
                    }
                }
            }

            Bitmap erodedImg = new Bitmap(originalW, originalH, PixelFormat.Format8bppIndexed);
            ColorPalette grayscalePalette = erodedImg.Palette;
            for (int i = 0; i < 256; i++)
            {
                grayscalePalette.Entries[i] = Color.FromArgb(i, i, i);
            }
            erodedImg.Palette = grayscalePalette;

            BitmapData paddedImgData = paddedImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData erodedImgData = erodedImg.LockBits(new Rectangle(0, 0, originalW, originalH), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int paddedImgStride = paddedImgData.Stride;
            int erodedImgStride = erodedImgData.Stride;

            unsafe
            {
                byte* paddedImgPtr = (byte*)paddedImgData.Scan0.ToPointer();
                byte* erodedImgPtr = (byte*)erodedImgData.Scan0.ToPointer();
                for(int i = 1; i < originalH-1; i++)
                {
                    for(int j = 1; j < originalW-1; j++)
                    {
                        int min = 255;
                        for(int k = 0; k < kernelH; k++)
                        {
                            for(int l = 0; l < kernelW; l++)
                            {
                                int pixel = paddedImgPtr[(i + k) * paddedImgStride + j + l];
                                int value = pixel - kernel[k, l];
                                if(value < min)
                                {
                                    min = value;
                                }
                            }
                        }
                        erodedImgPtr[i * erodedImgStride + j] = (byte)min;
                    }
                }
            }
            paddedImg.UnlockBits(paddedImgData);
            erodedImg.UnlockBits(erodedImgData);

            return erodedImg;
        }


        public static void Main(string[] args){
            string method = args[0];
            string path = args[1];
            Bitmap image = new Bitmap(path);
            Console.WriteLine("Image Size: " + image.Width + "x" + image.Height);
            Console.WriteLine("Image Pixel Format: " + image.PixelFormat);
            int[,] kernel = new int[,]{
                {0, 255, 0},
                {255, 255, 255},
                {0, 255, 0}
            };
            Bitmap output = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);
            if(method == "dilate")
            {
                Bitmap paddedImage = AddPadding(image);
                output = Dilation(paddedImage, kernel);
                Console.WriteLine("Image Size: " + output.Width + "x" + output.Height);
                Console.WriteLine("Image Pixel Format: " + output.PixelFormat);
                output.Save($"{method}.png", ImageFormat.Png);
            }else if(method == "erode")
            {
                Bitmap paddedImage = AddPadding(image);
                output = Erosion(paddedImage, kernel);
                Console.WriteLine("Image Size: " + output.Width + "x" + output.Height);
                Console.WriteLine("Image Pixel Format: " + output.PixelFormat);
                output.Save($"{method}.png", ImageFormat.Png);
            }
            else if(method == "pad")
            {
                output = AddPadding(image);
                Console.WriteLine("Image Size: " + output.Width + "x" + output.Height);
                Console.WriteLine("Image Pixel Format: " + output.PixelFormat);
                output.Save($"{method}.png", ImageFormat.Png);
            }
    }
}
}
