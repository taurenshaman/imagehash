using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Accord.Imaging;
using Accord.Imaging.Filters;

namespace Vision.Tools {
  public class ImageHelper {
    public static Bitmap ReGenerate(byte[] data, int width, int height) {
      Bitmap img = null;
      using (var ms = new MemoryStream( data )) {
        System.Drawing.Image tmpImg = System.Drawing.Image.FromStream( ms );
        img = (Bitmap)tmpImg.Clone();
      }
      return img;
    }

    public static Bitmap ResizeImage(Bitmap imgSource, int newWidth, int newHeight) {
      double ratio = 0d;
      double myThumbWidth = 0d;
      double myThumbHeight = 0d;
      int x = 0;
      int y = 0;

      if (( imgSource.Width / Convert.ToDouble( newWidth ) ) > ( imgSource.Height / Convert.ToDouble( newHeight ) ))
        ratio = Convert.ToDouble( imgSource.Width ) / Convert.ToDouble( newWidth );
      else
        ratio = Convert.ToDouble( imgSource.Height ) / Convert.ToDouble( newHeight );
      myThumbHeight = System.Math.Ceiling( imgSource.Height / ratio );
      myThumbWidth = System.Math.Ceiling( imgSource.Width / ratio );

      //Size thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);
      Size thumbSize = new Size( newWidth, newHeight );
      Bitmap imgResult = new Bitmap( newWidth, newHeight );
      x = ( newWidth - thumbSize.Width ) / 2;
      y = ( newHeight - thumbSize.Height );
      // Had to add System.Drawing class in front of Graphics ---
      System.Drawing.Graphics g = Graphics.FromImage( imgResult );
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      g.PixelOffsetMode = PixelOffsetMode.HighQuality;
      Rectangle rect = new Rectangle( x, y, thumbSize.Width, thumbSize.Height );
      g.DrawImage( imgSource, rect, 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel );

      g.Dispose();

      return imgResult;
    }

    public static Bitmap ResizeImage(Bitmap imgSource, Size newSize) {
      return ResizeImage( imgSource, newSize.Width, newSize.Height );
    }

    public static bool TryGetImageFromBytes(byte[] data, out System.Drawing.Image image) {
      try {
        using (var ms = new MemoryStream( data )) {
          image = System.Drawing.Image.FromStream( ms );
        }
      }
      catch (ArgumentException) {
        image = null;
        return false;
      }

      return true;
    }
    public static bool TryGetImageFromBytes(byte[] data, out System.Drawing.Bitmap image) {
      try {
        using (var ms = new MemoryStream( data )) {
          image = (Bitmap)Bitmap.FromStream( ms );
        }
      }
      catch (ArgumentException) {
        image = null;
        return false;
      }

      return true;
    }

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

    public static Bitmap CopyBitmapSafe(Bitmap srcBitmap) {
      Bitmap result = new Bitmap( srcBitmap.Width, srcBitmap.Height, srcBitmap.PixelFormat );
      result.SetResolution( srcBitmap.HorizontalResolution, srcBitmap.VerticalResolution );
      
      Rectangle bmpBounds = new Rectangle( 0, 0, srcBitmap.Width, srcBitmap.Height );
      BitmapData srcData = srcBitmap.LockBits( bmpBounds, ImageLockMode.ReadOnly, srcBitmap.PixelFormat );
      BitmapData resData = result.LockBits( bmpBounds, ImageLockMode.WriteOnly, result.PixelFormat );

      Int64 srcScan0 = srcData.Scan0.ToInt64();
      Int64 resScan0 = resData.Scan0.ToInt64();
      int srcStride = srcData.Stride;
      int resStride = resData.Stride;
      int rowLength = System.Math.Abs( srcData.Stride );
      try {
        byte[] buffer = new byte[rowLength];
        for (int y = 0; y < srcData.Height; y++) {
          Marshal.Copy( new IntPtr( srcScan0 + y * srcStride ), buffer, 0, rowLength );
          Marshal.Copy( buffer, 0, new IntPtr( resScan0 + y * resStride ), rowLength );
        }
      }
      finally {
        srcBitmap.UnlockBits( srcData );
        result.UnlockBits( resData );
      }

      return result;
    }

    /// <summary>
    /// 将位图压缩到流。移除exif信息+调整Encoder.Quality
    /// </summary>
    /// <param name="srcBitmap"></param>
    /// <param name="imageQuality"></param>
    /// <returns></returns>
    public static MemoryStream CompressBitmapToStream(Bitmap srcBitmap, long imageQuality = 90) {
      Bitmap bmp = Vision.Tools.ImageHelper.CopyBitmapSafe( srcBitmap );

      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
      ImageCodecInfo ici = null;

      foreach (ImageCodecInfo codec in codecs) {
        if (codec.MimeType == "image/jpeg")
          ici = codec;
      }

      EncoderParameters ep = new EncoderParameters();
      ep.Param[0] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, imageQuality );

      MemoryStream ms = new MemoryStream();
      bmp.Save( ms, ici, ep );
      bmp.Dispose();
      return ms;
    }

    public static float[,] GetPixelValuesTo2DArray(Bitmap image) {
      BitmapData bmpData = image.LockBits( ImageLockMode.ReadOnly );
      IntPtr srcPtr = bmpData.Scan0;
      int length = image.Width * image.Height;
      int scanWidth = image.Width * 3;

      byte[] srcRGBValues = new byte[length];
      float[,] matrix = new float[image.Height, image.Width];

      Marshal.Copy( srcPtr, srcRGBValues, 0, length );
      //解锁位图  
      image.UnlockBits( bmpData );

      int row = 0;
      for (int i = 0; i < image.Height; i++) {
        for (int j = 0; j < image.Width; j++) {
          //只处理每行中图像像素数据,舍弃未用空间  
          //注意位图结构中RGB按BGR的顺序存储  
          int k = 3 * j;
          matrix[row, j] = ( srcRGBValues[i * scanWidth + k + 2]
               + srcRGBValues[i * scanWidth + k + 1]
               + srcRGBValues[i * scanWidth + k + 0] ) / 3f;
        }
        row++;
      }
      return matrix;
    }

    /// <summary>
    /// Accord库有bitmap.SetGrayscalePalette扩展方法，但是无法保证它修改Palette与ITU-R 601-2等价。故重新实现了L模式。
    /// Use the ITU-R 601-2 luma transform: L = R * 299/1000 + G * 587/1000 + B * 114/1000
    /// </summary>
    public static Bitmap ConvertToLMode(Bitmap source) {
      Bitmap bm = new Bitmap( source.Width, source.Height );
      for (int y = 0; y < bm.Height; y++) {
        for (int x = 0; x < bm.Width; x++) {
          Color c = source.GetPixel( x, y );
          int luma = (int)( c.R * 0.299 + c.G * 0.587 + c.B * 0.114 );
          bm.SetPixel( x, y, Color.FromArgb( luma, luma, luma ) );
        }
      }
      return bm;
    }

    public static float[,] GetGrayArray2D(Bitmap srcBmp, Rectangle rect) {
      int width = rect.Width;
      int height = rect.Height;

      BitmapData srcBmpData = srcBmp.LockBits( rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

      IntPtr srcPtr = srcBmpData.Scan0;

      int scanWidth = width * 3;
      int src_bytes = scanWidth * height;
      //int srcStride = srcBmpData.Stride;  
      byte[] srcRGBValues = new byte[src_bytes];
      float[,] grayValues = new float[height, width];
      //RGB[] rgb = new RGB[srcBmp.Width * rows];  
      //复制GRB信息到byte数组  
      Marshal.Copy( srcPtr, srcRGBValues, 0, src_bytes );
      //解锁位图  
      srcBmp.UnlockBits( srcBmpData );
      //灰度化处理  
      int m = 0, i = 0, j = 0;  //m表示行，j表示列  
      int k = 0;
      float gray;

      for (i = 0; i < height; i++)  //只获取图片的rows行像素值  
      {
        for (j = 0; j < width; j++) {
          //只处理每行中图像像素数据,舍弃未用空间  
          //注意位图结构中RGB按BGR的顺序存储  
          k = 3 * j;
          gray = (float)( srcRGBValues[i * scanWidth + k + 2] * 0.299
               + srcRGBValues[i * scanWidth + k + 1] * 0.587
               + srcRGBValues[i * scanWidth + k + 0] * 0.114 );

          grayValues[m, j] = gray;  //将灰度值存到double的数组中  
        }
        m++;
      }

      return grayValues;
    }

    /// <summary>
    /// not finished.
    /// TODO: pywt.wavedec2 is too complicated.
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="hash_size">@hash_size must be a power of 2 and less than @image_scale.</param>
    /// <param name="image_scale">@image_scale must be power of 2 and less than image size. By default is equal to max power of 2 for an input image.</param>
    /// <param name="mode">'haar' - Haar wavelets, by default; 'db4' - Daubechies wavelets</param>
    /// <param name="remove_max_haar_ll">whether remove the lowest low level (LL) frequency using Haar wavelet.</param>
    public static void WHash(Bitmap bmp, int hash_size = 8, int image_scale = 16, string mode = "haar", bool remove_max_haar_ll = true) {
      // assert image_scale & (image_scale - 1) == 0, "image_scale is not power of 2"
      if (( image_scale & ( image_scale - 1 ) ) == 0)
        throw new Exception( "image_scale is not power of 2" );
      // image_scale = 2**int(numpy.log2(min(image.size)))
      // image.size: im.size ⇒ (width, height)
      // Image size, in pixels.The size is given as a 2 - tuple( width, height ).
      // **	幂 - 返回x的y次幂
      int minWH = System.Math.Min( bmp.Width, bmp.Height );
      int b = Accord.Math.Tools.Log2( minWH );
      image_scale = (int)System.Math.Pow( 2, b );
      // ll_max_level = int(numpy.log2(image_scale))
      int ll_max_level = Accord.Math.Tools.Log2( image_scale );

      // assert hash_size & (hash_size-1) == 0, "hash_size is not power of 2"
      if (( hash_size & ( hash_size - 1 ) ) == 0)
        throw new Exception( "hash_size is not power of 2" );
      // level = int(numpy.log2(hash_size))
      int level = (int)( System.Math.Log( hash_size, 2 ) );
      //assert level <= ll_max_level, "hash_size in a wrong range"
      if( level <= ll_max_level)
        throw new Exception( "hash_size in a wrong range" );
      // dwt_level = ll_max_level - level
      int dwt_level = ll_max_level - level;

      // image = image.convert("L").resize((image_scale, image_scale), Image.ANTIALIAS)
      // L (8-bit pixels, black and white)
      // im.resize(size) ⇒ image
      // Returns a resized copy of an image. The size argument gives the requested size in pixels, as a 2-tuple: (width, height).
      var imgLMode = ConvertToLMode( bmp );
      var imgResized = ResizeImage( imgLMode, image_scale, image_scale );

      // pixels = numpy.array( image.getdata(), dtype = numpy.float ).reshape( (image_scale, image_scale) )
      var pixels = GetPixelValuesTo2DArray( imgResized );
      // pixels /= 255
      Vision.Math.Matrix.Scale( ref pixels, imgResized.Height, imgResized.Width, 1f / 255 );
      // 上面应该是像素值进行了归一化处理。


      //# Remove low level frequency LL(max_ll) if @remove_max_haar_ll using haar filter
      //if remove_max_haar_ll:
      //    coeffs = pywt.wavedec2( pixels, 'haar', level = ll_max_level )
      //    coeffs = list( coeffs )
      //    coeffs[0] *= 0
      //    pixels = pywt.waverec2( coeffs, 'haar' )
      if (mode == "haar" && remove_max_haar_ll) {
        // 2D multilevel decomposition using wavedec2
        WaveletTransform wtHaar = new WaveletTransform( new Accord.Math.Wavelets.Haar( ll_max_level ) ); // 只能处理Bitmap
        

        Vision.Math.DiscreteWaveletTransformation.FWT( pixels, ll_max_level );
        
      }

      //WaveletTransform wt = new WaveletTransform( new Accord.Math.Wavelets.Haar( 1 ) );
    }

  }

}
