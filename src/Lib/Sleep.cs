using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class Sleep
    {
        public static void Until(DateTimeOffset end, int step = 1000)
        {
            while (DateTimeOffset.Now <= end)
            {
                System.Threading.Thread.Sleep(step);
            }
        }
    }
}
