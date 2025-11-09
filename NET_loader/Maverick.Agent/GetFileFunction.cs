using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    public static class GetFileFunction
    {
        public static byte[] PerformReadFile(string name)
        {
            try
            {
                return File.ReadAllBytes(name);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
