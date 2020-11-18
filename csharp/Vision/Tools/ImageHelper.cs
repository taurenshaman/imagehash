using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Accord.Imaging;
using Accord.Imaging.Filters;
using SkiaSharp;

namespace Vision.Tools {
  public class ImageHelper {
    
    /// <summary>
    /// not finished.
    /// TODO: pywt.wavedec2 is too complicated.
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="hash_size">@hash_size must be a power of 2 and less than @image_scale.</param>
    /// <param name="image_scale">@image_scale must be power of 2 and less than image size. By default is equal to max power of 2 for an input image.</param>
    /// <param name="mode">'haar' - Haar wavelets, by default; 'db4' - Daubechies wavelets</param>
    /// <param name="remove_max_haar_ll">whether remove the lowest low level (LL) frequency using Haar wavelet.</param>
    public static void WHash(SKBitmap bmp, int hash_size = 8, int image_scale = 16, string mode = "haar", bool remove_max_haar_ll = true) {
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
      var imgLMode = SkiaSharpUtility.ConvertToLMode( bmp );
      var imgResized = SkiaSharpUtility.ResizeBitmap( imgLMode, image_scale, image_scale );

      // pixels = numpy.array( image.getdata(), dtype = numpy.float ).reshape( (image_scale, image_scale) )
      var pixels = SkiaSharpUtility.GetPixelValuesTo2DArray( imgResized );
      // pixels /= 255
      Vision.Math.Matrix.Scale( ref pixels, imgResized.Height, imgResized.Width, 1f / 255 );
      // 上面应该是像素值进行了归一化处理。


      //# Remove low level frequency LL(max_ll) if @remove_max_haar_ll using haar filter
      //if remove_max_haar_ll:
      //    coeffs = pywt.wavedec2( pixels, 'haar', level = ll_max_level )
      //    coeffs = list( coeffs )
      //    coeffs[0] *= 0
      //    pixels = pywt.waverec2( coeffs, 'haar' )
      if (remove_max_haar_ll) {
        // 2D multilevel decomposition using wavedec2
        WaveletTransform wtHaar = new WaveletTransform( new Accord.Math.Wavelets.Haar( ll_max_level ) ); // 只能处理Bitmap
        

        Vision.Math.DiscreteHaarWaveletTransformation.FWT( pixels, ll_max_level );
        
      }

      //WaveletTransform wt = new WaveletTransform( new Accord.Math.Wavelets.Haar( 1 ) );
    }

  }

}
