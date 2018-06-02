using System;
using System.IO;
using System.Threading.Tasks;

#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif

#if __ANDROID__
using Android.Graphics;
#endif


namespace MsorLi.Utilities
{
    public static class ImageResizer
    {
        static ImageResizer()
        {
        }

        public static byte[] ResizeImage(byte[] imageData, float width, float height)
        {
#if __IOS__
            return ResizeImageIOS(imageData, width, height);
#endif
#if __ANDROID__
            return ResizeImageAndroid(imageData, width, height);
#endif
        }


#if __IOS__
        public static byte[] ResizeImageIOS(byte[] imageData, float width, float height)
        {
            UIImage originalImage = ImageFromByteArray(imageData);

            originalImage = iOSScaleAndRotateImage.ScaleAndRotateImage(originalImage, originalImage.Orientation);
                                
           // UIImageOrientation orientation = originalImage.Orientation;


            return originalImage.AsPNG().ToArray();


            //create a 24bit RGB image
            //using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
            //                                     (int)width, (int)height, 8,
            //                                     4 * (int)width, CGColorSpace.CreateDeviceRGB(),
            //                                     CGImageAlphaInfo.PremultipliedFirst))
            //{

            //    RectangleF imageRect = new RectangleF(0, 0, width, height);

            //    // draw the image
            //    context.DrawImage(imageRect, originalImage.CGImage);

            //    UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

            //    // save the image as a png
            //    return resizedImage.AsPNG().ToArray();
            //}
        }        

        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }


#endif

#if __ANDROID__

        public static byte[] ResizeImageAndroid(byte[] imageData, float width, float height)
        {
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, true);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                resizedImage.Recycle();
                return ms.ToArray();
            }
        }
#endif
    }
#if __IOS__

    public static class iOSScaleAndRotateImage
    {
        public static UIImage ScaleAndRotateImage(UIImage imageIn, UIImageOrientation orIn)
        {
            int kMaxResolution = 720;

            CGImage imgRef = imageIn.CGImage;
            float width = imgRef.Width;
            float height = imgRef.Height;
            CGAffineTransform transform = CGAffineTransform.MakeIdentity();
            RectangleF bounds = new RectangleF(0, 0, width, height);

            if (width > kMaxResolution || height > kMaxResolution)
            {
                float ratio = width / height;

                if (ratio > 1)
                {
                    bounds.Width = kMaxResolution;
                    bounds.Height = bounds.Width / ratio;
                }
                else
                {
                    bounds.Height = kMaxResolution;
                    bounds.Width = bounds.Height * ratio;
                }
            }

            float scaleRatio = bounds.Width / width;
            SizeF imageSize = new SizeF(width, height);
            UIImageOrientation orient = orIn;
            float boundHeight;

            switch (orient)
            {
                case UIImageOrientation.Up:                                        //EXIF = 1
                    transform = CGAffineTransform.MakeIdentity();
                    break;



                case UIImageOrientation.Down:                                      //EXIF = 3
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
                    break;


                case UIImageOrientation.Left:                                      //EXIF = 6
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(0.0f, imageSize.Width);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                    break;



                case UIImageOrientation.Right:                                     //EXIF = 8
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                    break;

                default:
                    throw new Exception("Invalid image orientation");
                    break;
            }

            UIGraphics.BeginImageContext(bounds.Size);

            CGContext context = UIGraphics.GetCurrentContext();

            if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left)
            {
                context.ScaleCTM(-scaleRatio, scaleRatio);
                context.TranslateCTM(-height, 0);
            }
            else
            {
                context.ScaleCTM(scaleRatio, -scaleRatio);
                context.TranslateCTM(0, -height);
            }

            context.ConcatCTM(transform);
            context.DrawImage(new RectangleF(0, 0, width, height), imgRef);

            UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return imageCopy;
        }
    }
    #endif
}