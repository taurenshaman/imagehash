using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Math {
  public class Matrix {
    public static void Scale( ref float[,] source, int rows, int columns, float scale) {
      for(int i = 0; i< rows; i++) {
        for(int j = 0; j < columns; j++) {
          source[i, j] = source[i, j] * scale;
        }
      }
    }

  }

}
