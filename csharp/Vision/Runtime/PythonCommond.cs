using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Runtime {
  public static class PythonCommond {
    static string PythonExecutePath = @"D:\Python27\python.exe";


    public static string Excute(int millisecondsWaitForExit, string pythonScriptPath, params string[] parameters) {
      StringBuilder sbParams = new StringBuilder();
      if(parameters != null) {
        foreach (string param in parameters)
          sbParams.Append( " " + param );
      }

      string output = "";

      ProcessStartInfo startInfo = new ProcessStartInfo();
      startInfo.FileName = PythonExecutePath;
      startInfo.Arguments = string.Format( "{0} {1}", pythonScriptPath, sbParams.ToString() ); ;
      startInfo.UseShellExecute = false;
      startInfo.RedirectStandardInput = true;
      startInfo.RedirectStandardOutput = true;
      startInfo.CreateNoWindow = true;

      using (Process process = new Process()) {
        process.StartInfo = startInfo;
        //process.OutputDataReceived += (s, e) => {
        //  System.Diagnostics.Debug.WriteLine( e.Data );
        //};
        try {
          if (process.Start())
          {
            if (millisecondsWaitForExit == 0)
              process.WaitForExit();
            else
              process.WaitForExit( millisecondsWaitForExit );
            output = process.StandardOutput.ReadToEnd();
          }
        }
        catch (Exception ex) {
          Debug.WriteLine( ex.Message );
        }
        finally {
          if (process != null)
            process.Close();
        }
      }

      return output;
    }

    public static string ComputeImageWHash(string imagePath, int millisecondsWaitForExit = 0) {
      string output = Excute( millisecondsWaitForExit, @"E:\Github\imagehash\python\computeimagehash.py", imagePath );
      return output;
    }

  }

}
