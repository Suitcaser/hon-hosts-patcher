using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Principal;
using System.IO;
using System.Diagnostics;

namespace InternationalServerHoNPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\drivers\etc\hosts";
            string path = Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\drivers\etc";
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal currentPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;

            List<string> result = new List<string>();
            foreach (var str in System.IO.File.ReadAllLines(filePath))
            {
                if (!(str.Contains("masterserver.hon.s2games.com") || str.Contains("masterserver.naeu.heroesofnewerth.com") || str.StartsWith("#Patched hosts file")))
                {
                    result.Add(str.Trim());
                }
            }
            result.Add("104.131.237.217 masterserver.naeu.heroesofnewerth.com");
            result.Add("104.131.237.217 masterserver.hon.s2games.com");

            //System.Diagnostics.Process process = new System.Diagnostics.Process();
            //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo.FileName = "cmd.exe";
            //startInfo.Arguments = "echo 0.0.0.0 websitename.com >> %WINDIR%\\System32\\Drivers\\Etc\\Hosts";
            //process.StartInfo = startInfo;
            //process.Start();

            //ExecuteCommandAsAdmin(@"/c attrib -R -S -H hosts",path);
            ////ExecuteCommandAsAdmin(@"/c echo 0.0.0.0 websitename.com >> D:\LOL.txt");
            //string comm = @"/c echo " + string.Join("\n",result) + @" > " + filePath;
            //ExecuteCommandAsAdmin(comm, path);
            //ExecuteCommandAsAdmin(@"/c attrib +S hosts",path);

            var batContents = @"@echo off" + Environment.NewLine +
                @"cd /d ""%windir%\system32\drivers""" + Environment.NewLine +
                @"cd etc" + Environment.NewLine +
                @"attrib -R -S -H hosts" + Environment.NewLine +
                @"echo #Patched hosts file > hosts" + Environment.NewLine +
                @"echo " + string.Join(@" >> hosts" + Environment.NewLine + @"echo ", result) + @" >> hosts" + Environment.NewLine + 
                @"attrib +R +S hosts" + Environment.NewLine +
                @"ipconfig /flushdns" + Environment.NewLine +
                @"goto :eof";


            var batPath = Environment.GetEnvironmentVariable("SystemRoot") + @"\tmpPatch.bat";
            using (StreamWriter file = new System.IO.StreamWriter(batPath))
            {
                file.Write(batContents);
            }

            System.Diagnostics.Process.Start(batPath);

            File.Delete(batPath);
        }

        public static void ExecuteCommandAsAdmin(string path = @"C:\")
        {

            ProcessStartInfo procStartInfo = new ProcessStartInfo()
            {
                Verb = "runas",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = path,
                //Arguments = command
            };

            using (Process proc = new Process())
            {
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
            }
        }
    }
}
