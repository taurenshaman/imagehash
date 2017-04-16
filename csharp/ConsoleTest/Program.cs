using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace ConsoleTest {
  class Program {
    static void Main(string[] args) {
      testImageHashUsingCommand();


      Console.WriteLine( "done." );
      Console.ReadKey();
    }
    
    // failed: IronPython cannot process .pyd files in PIL
    static void testImageHashUsingIronPython() {
      string dir = @"E:\Github\imagehash\images\";
      string file1 = dir + "imagehash.png";
      string file2 = dir + "lenna.png";
      string file3 = dir + "lenna1.jpg";
      string file4 = dir + "lenna2.jpg";

      Console.WriteLine( file1 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash1 = Vision.Runtime.Python.ComputeImageWHash( file1 );
      Console.WriteLine( hash1 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file2 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash2 = Vision.Runtime.Python.ComputeImageWHash( file2 );
      Console.WriteLine( hash2 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file3 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash3 = Vision.Runtime.Python.ComputeImageWHash( file3 );
      Console.WriteLine( hash3 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file4 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash4 = Vision.Runtime.Python.ComputeImageWHash( file4 );
      Console.WriteLine( hash4 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
    }

    // worked @ 2017-4-15
    static void testImageHashUsingCommand() {
      string dir = @"E:\Github\imagehash\images\";
      string file1 = dir + "imagehash.png";
      string file2 = dir + "lenna.png";
      string file3 = dir + "lenna1.jpg";
      string file4 = dir + "lenna2.jpg";

      Console.WriteLine( file1 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash1 = Vision.Runtime.PythonCommond.ComputeImageWHash( file1 );
      Console.WriteLine( hash1 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file2 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash2 = Vision.Runtime.PythonCommond.ComputeImageWHash( file2 );
      Console.WriteLine( hash2 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file3 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash3 = Vision.Runtime.PythonCommond.ComputeImageWHash( file3 );
      Console.WriteLine( hash3 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file4 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash4 = Vision.Runtime.PythonCommond.ComputeImageWHash( file4 );
      Console.WriteLine( hash4 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
    }

  }

}
