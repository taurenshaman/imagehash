using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Vision.Tools {
  public static class SkiaSharpUtility {

    public static SKPaint CreateDefaultPaint() {
      var paint = new SKPaint();
      paint.IsAntialias = true;
      paint.FilterQuality = SKFilterQuality.High;
      return paint;
    }

    public static SKImage CropSurfaceToImage(SKSurface skSurface, int imageCanvasSize, int destSize, SKPaint paint) {
      SKImage skImage = null;
      var srcImage = skSurface.Snapshot();
      using (var newSurface = SKSurface.Create( new SKImageInfo( destSize, destSize ) )) {
        newSurface.Canvas.DrawImage( srcImage, SKRect.Create( 0, 0, destSize, destSize ), SKRect.Create( 0, 0, destSize, destSize ), paint );
        newSurface.Canvas.Flush();
        skImage = newSurface.Snapshot();
      }
      return skImage;
    }

    public static SKImage ScaleSurfaceToImage(SKSurface skSurface, int imageCanvasSize, int destSize, SKPaint paint) {
      SKImage skImage = null;
      var canvas = skSurface.Canvas;
      // scale
      if (destSize == imageCanvasSize)
        skImage = skSurface.Snapshot();
      else {
        // 先缩放；然后绘制到小图上面，即相当于Crop了
        canvas.Scale( 1.0f * destSize / imageCanvasSize );
        var srcImage = skSurface.Snapshot();

        using (var newSurface = SKSurface.Create( new SKImageInfo( destSize, destSize ) )) {
          newSurface.Canvas.DrawImage( srcImage, SKRect.Create( 0, 0, imageCanvasSize, imageCanvasSize ), SKRect.Create( 0, 0, destSize, destSize ), paint );
          newSurface.Canvas.Flush();
          skImage = newSurface.Snapshot();
        }
      } // scale
      return skImage;
    }

    public static SKData EncodeImageToSKData(SKImage skImage, string format, int quality = 90) {
      SKEncodedImageFormat skFormat = SKEncodedImageFormat.Png;
      switch (format) {
        case "jpg":
          skFormat = SKEncodedImageFormat.Jpeg;
          break;
        case "jpeg":
          skFormat = SKEncodedImageFormat.Jpeg;
          break;
        case "gif":
          skFormat = SKEncodedImageFormat.Gif;
          break;
        case "bmp":
          skFormat = SKEncodedImageFormat.Bmp;
          break;
        case "webp":
          skFormat = SKEncodedImageFormat.Webp;
          break;
        default:
          skFormat = SKEncodedImageFormat.Png;
          break;
      }
      SKData skData = skImage.Encode( skFormat, quality );
      return skData;
    }

    public static void DrawImageToCanvas(SKCanvas canvas, string imgPath, SKRect dest, SKPaint paint = null) {
      var skBitmap = SKBitmap.Decode( imgPath );
      canvas.DrawBitmap( skBitmap, SKRect.Create( 0, 0, skBitmap.Width, skBitmap.Height ), dest, paint );
      skBitmap.Dispose();
    }

    public static void CircleStroke(SKCanvas canvas, SKPaint paint, SKColor strokeColor, float centerX, float centerY, float radius) {
      paint.Style = SKPaintStyle.Stroke;
      paint.Color = strokeColor;
      canvas.DrawCircle( centerX, centerY, radius, paint );
    }

    public static void CircleFill(SKCanvas canvas, SKPaint paint, SKColor fillColor, float centerX, float centerY, float radius) {
      paint.Style = SKPaintStyle.Fill;
      paint.Color = fillColor;
      canvas.DrawCircle( centerX, centerY, radius, paint );
    }

    public static void PathStroke(SKCanvas canvas, SKPaint paint, SKColor strokeColor, SKPath path) {
      paint.Style = SKPaintStyle.Stroke;
      paint.Color = strokeColor;
      canvas.DrawPath( path, paint );
    }

    public static void PathFill(SKCanvas canvas, SKPaint paint, SKColor fillColor, SKPath path) {
      paint.Style = SKPaintStyle.Fill;
      paint.Color = fillColor;
      canvas.DrawPath( path, paint );
    }

    public static void DrawTriangle(SKCanvas canvas, SKPaint paint, SKPoint p1, SKPoint p2, SKPoint p3) {
      var path = new SKPath { FillType = SKPathFillType.EvenOdd };
      path.MoveTo( p1 );
      path.LineTo( p2 );
      path.LineTo( p3 );
      path.LineTo( p1 );
      path.Close();

      canvas.DrawPath( path, paint );
    }

    public static SKColor CreateRGBColor(byte red, byte green, byte blue) {
      SKColor color = new SKColor(red, green, blue);
      return color;
    }

    public static SKColor CreateRGBAColor(byte red, byte green, byte blue, byte alpha) {
      SKColor color = new SKColor( red, green, blue, alpha );
      return color;
    }

    public static SKImage ConvertPixelsToImage(byte[] data, int width, int height) {
      SKImage img;
      using (var ms = new MemoryStream( data )) {
        SKImageInfo ii = new SKImageInfo( width, height );
        SKData sKData = SKData.Create( ms );
        img = SKImage.FromPixels( ii, sKData );
      }
      return img;
    }

    public static SKBitmap ResizeBitmap(SKBitmap imgSource, int newWidth, int newHeight) {
      //SKBitmap imgDest = new SKBitmap( newWidth, newHeight );
      //bool r = imgSource.ScalePixels( imgDest, SKFilterQuality.High );
      //if (!r)
      //  return null;

      SKImageInfo ii = new SKImageInfo( newWidth, newHeight );
      SKBitmap imgDest = imgSource.Resize( ii, SKFilterQuality.High );
      return imgDest;
    }

    //public static bool TryGetImageFromBytes(byte[] data, out System.Drawing.Image image) {
    //  try {
    //    using (var ms = new MemoryStream( data )) {
    //      image = System.Drawing.Image.FromStream( ms );
    //    }
    //  }
    //  catch (ArgumentException) {
    //    image = null;
    //    return false;
    //  }

    //  return true;
    //}
    //public static bool TryGetImageFromBytes(byte[] data, out System.Drawing.Bitmap image) {
    //  try {
    //    using (var ms = new MemoryStream( data )) {
    //      image = (Bitmap)Bitmap.FromStream( ms );
    //    }
    //  }
    //  catch (ArgumentException) {
    //    image = null;
    //    return false;
    //  }

    //  return true;
    //}

    //public static Bitmap Copy32BPPBitmapSafe(Bitmap srcBitmap) {
    //  Bitmap result = new Bitmap( srcBitmap.Width, srcBitmap.Height, PixelFormat.Format32bppArgb );

    //  Rectangle bmpBounds = new Rectangle( 0, 0, srcBitmap.Width, srcBitmap.Height );
    //  BitmapData srcData = srcBitmap.LockBits( bmpBounds, ImageLockMode.ReadOnly, srcBitmap.PixelFormat );
    //  BitmapData resData = result.LockBits( bmpBounds, ImageLockMode.WriteOnly, result.PixelFormat );

    //  Int64 srcScan0 = srcData.Scan0.ToInt64();
    //  Int64 resScan0 = resData.Scan0.ToInt64();
    //  int srcStride = srcData.Stride;
    //  int resStride = resData.Stride;
    //  int rowLength = Math.Abs( srcData.Stride );
    //  try {
    //    byte[] buffer = new byte[rowLength];
    //    for (int y = 0; y < srcData.Height; y++) {
    //      Marshal.Copy( new IntPtr( srcScan0 + y * srcStride ), buffer, 0, rowLength );
    //      Marshal.Copy( buffer, 0, new IntPtr( resScan0 + y * resStride ), rowLength );
    //    }
    //  }
    //  finally {
    //    srcBitmap.UnlockBits( srcData );
    //    result.UnlockBits( resData );
    //  }

    //  return result;
    //}

    //public static Bitmap CopyBitmapSafe(Bitmap srcBitmap) {
    //  Bitmap result = new Bitmap( srcBitmap.Width, srcBitmap.Height, srcBitmap.PixelFormat );
    //  result.SetResolution( srcBitmap.HorizontalResolution, srcBitmap.VerticalResolution );

    //  Rectangle bmpBounds = new Rectangle( 0, 0, srcBitmap.Width, srcBitmap.Height );
    //  BitmapData srcData = srcBitmap.LockBits( bmpBounds, ImageLockMode.ReadOnly, srcBitmap.PixelFormat );
    //  BitmapData resData = result.LockBits( bmpBounds, ImageLockMode.WriteOnly, result.PixelFormat );

    //  Int64 srcScan0 = srcData.Scan0.ToInt64();
    //  Int64 resScan0 = resData.Scan0.ToInt64();
    //  int srcStride = srcData.Stride;
    //  int resStride = resData.Stride;
    //  int rowLength = System.Math.Abs( srcData.Stride );
    //  try {
    //    byte[] buffer = new byte[rowLength];
    //    for (int y = 0; y < srcData.Height; y++) {
    //      Marshal.Copy( new IntPtr( srcScan0 + y * srcStride ), buffer, 0, rowLength );
    //      Marshal.Copy( buffer, 0, new IntPtr( resScan0 + y * resStride ), rowLength );
    //    }
    //  }
    //  finally {
    //    srcBitmap.UnlockBits( srcData );
    //    result.UnlockBits( resData );
    //  }

    //  return result;
    //}


    public static float[,] GetPixelValuesTo2DArray(SKBitmap image) {
      var pixels = image.Pixels;
      
      float[,] matrix = new float[image.Height, image.Width];
      int rs = 256 ^ 2;
      long offset = 0;
      for (int i = 0; i < image.Height; i++) {
        for (int j = 0; j < image.Width; j++) {
          var color = pixels[offset];
          // https://stackoverflow.com/questions/52626779/calculating-a-single-representative-pixel-value-from-a-rgb-value
          // 1: (r+g+b)/2
          matrix[i, j] = ( color.Red + color.Green + color.Blue ) / 3f;
          // 2: r*256^2 + g*256 + b
          //matrix[i, j] = color.Red * rs + color.Green * 256 + color.Blue;

          offset++;
        }
      }
      return matrix;
    }

    /// <summary>
    /// Accord库有bitmap.SetGrayscalePalette扩展方法，但是无法保证它修改Palette与ITU-R 601-2等价。故重新实现了L模式。
    /// Use the ITU-R 601-2 luma transform: L = R * 299/1000 + G * 587/1000 + B * 114/1000
    /// </summary>
    public static SKBitmap ConvertToLMode(SKBitmap source) {
      SKBitmap bm = new SKBitmap( source.Width, source.Height );
      for (int y = 0; y < bm.Height; y++) {
        for (int x = 0; x < bm.Width; x++) {
          var c = source.GetPixel( x, y );
          byte luma = (byte)( c.Red * 0.299 + c.Green * 0.587 + c.Blue * 0.114 );
          SKColor grayscaleColor = new SKColor( luma, luma, luma );
          bm.SetPixel( x, y, grayscaleColor );
        }
      }
      return bm;
    }

    //public static float[,] GetGrayArray2D(Bitmap srcBmp, Rectangle rect) {
    //  int width = rect.Width;
    //  int height = rect.Height;

    //  BitmapData srcBmpData = srcBmp.LockBits( rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

    //  IntPtr srcPtr = srcBmpData.Scan0;

    //  int scanWidth = width * 3;
    //  int src_bytes = scanWidth * height;
    //  //int srcStride = srcBmpData.Stride;  
    //  byte[] srcRGBValues = new byte[src_bytes];
    //  float[,] grayValues = new float[height, width];
    //  //RGB[] rgb = new RGB[srcBmp.Width * rows];  
    //  //复制GRB信息到byte数组  
    //  Marshal.Copy( srcPtr, srcRGBValues, 0, src_bytes );
    //  //解锁位图  
    //  srcBmp.UnlockBits( srcBmpData );
    //  //灰度化处理  
    //  int m = 0, i = 0, j = 0;  //m表示行，j表示列  
    //  int k = 0;
    //  float gray;

    //  for (i = 0; i < height; i++)  //只获取图片的rows行像素值  
    //  {
    //    for (j = 0; j < width; j++) {
    //      //只处理每行中图像像素数据,舍弃未用空间  
    //      //注意位图结构中RGB按BGR的顺序存储  
    //      k = 3 * j;
    //      gray = (float)( srcRGBValues[i * scanWidth + k + 2] * 0.299
    //           + srcRGBValues[i * scanWidth + k + 1] * 0.587
    //           + srcRGBValues[i * scanWidth + k + 0] * 0.114 );

    //      grayValues[m, j] = gray;  //将灰度值存到double的数组中  
    //    }
    //    m++;
    //  }

    //  return grayValues;
    //}

  }

}
