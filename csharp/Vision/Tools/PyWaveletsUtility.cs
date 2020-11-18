using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Tools {
  public static class PyWaveletsUtility {

    // F:\python-packages\pywt\_multilevel.py
    // def wavedec2(data, wavelet, mode='symmetric', level=None, axes=(-2, -1)):
    public static void wavedec2(float[,] data, string mode = "symmetric", int level = 0, Tuple<int, int> axes = null) {
      if (axes == null)
        axes = Tuple.Create( -2, -1 );

      // data = numpy.asarray(data)
      // if data.ndim < 2:
      //    raise ValueError("Expected input data to have at least 2 dimensions.")
      

      // axes = tuple(axes)
      // if len( axes ) != 2:
      //   raise ValueError("Expected 2 axes")
      // if len( axes ) != len( set( axes ) ):
      //   raise ValueError("The axes passed to wavedec2 must be unique.")

      // axes_sizes = [data.shape[ax] for ax in axes]
    }

  }

}
