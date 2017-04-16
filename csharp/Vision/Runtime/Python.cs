using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Runtime {
  public class Python {
    static ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();

    static Python() {
      var paths = engine.GetSearchPaths();
      paths.Add( @"D:\Python27\Lib" );
      paths.Add( @"D:\Python27\Lib\site-packages" );
      paths.Add( @"D:\Python27\Lib\site-packages\numpy" );
      paths.Add( @"D:\Python27\Lib\site-packages\PIL" );
      paths.Add( @"D:\Python27\Lib\site-packages\pywt" );
      engine.SetSearchPaths( paths );
    }

    public static string ComputeImageWHash( string imagePath ) {
      // PIL include many _imaging files, which are all .pyd format. Not supported by IronPython.
      var source = engine.CreateScriptSourceFromFile( @"E:\Github\imagehash\python\imagehash.py", Encoding.UTF8, Microsoft.Scripting.SourceCodeKind.File );
      ScriptScope scope = engine.CreateScope();
      source.Execute( scope );
      Func<string, string> compute_whash = scope.GetVariable<Func<string, string>>( "compute_whash" );
      string hash = compute_whash( imagePath );
      return hash;
    }

  }

}
