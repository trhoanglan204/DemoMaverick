using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    public static class SendFileFunction
    {
        public static void PerformWriteFile(string name, byte[] data)
        {
            try
            {
                System.IO.File.WriteAllBytes(name, data);
            }
            catch (Exception)
            {
                // Handle exceptions as needed
            }
        }
    }
}
