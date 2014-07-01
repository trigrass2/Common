using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;

namespace Common
{
    class Utils
    {
        public static List<FileInfo> GetFiles(string dir, string pattern, List<FileInfo> files = null)
        {
            if (files == null)
                files = new List<FileInfo>();
            string[] filetypes = new string[] { "3gp", "avi", "dat", "mp4", "wmv", 
                                                     "mov", "mpg", "flv",  };
            foreach (string file in Directory.GetFiles(dir, pattern,
                                                   SearchOption.TopDirectoryOnly))
            {
                files.Add(new FileInfo(file));
            }
            foreach (string subDir in Directory.GetDirectories(dir))
            {
                try
                {
                    GetFiles(subDir, pattern, files);
                }
                catch
                {
                }
            }
            return files;
        }
        public static int RunCommand(string command, string arguments, out string[] output)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = arguments;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            List<String> outputList = new List<string>();
            while (!p.StandardOutput.EndOfStream)
                outputList.Add(p.StandardOutput.ReadLine());
            p.WaitForExit();
            output = outputList.ToArray();
            return p.ExitCode;
        }

        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

    }

}
