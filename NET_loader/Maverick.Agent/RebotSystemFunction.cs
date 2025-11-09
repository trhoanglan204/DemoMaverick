using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    public static class RebotSystemFunction
    {
        public static void PerformRebotSystem()
        {
            System.Diagnostics.Process.Start("shutdown", "/r /f /t 0");
        }
    }
}
