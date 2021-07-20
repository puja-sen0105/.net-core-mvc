using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdeToFood
{
    public interface ILog
    {
        void info(string str);
    }

    public class MyConsoleLogger : ILog
    {
        public void info(string str)
        {
            Console.WriteLine(str);
        }
    }
}
