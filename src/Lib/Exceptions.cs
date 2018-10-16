using System;

namespace Lib
{
    public static class Exceptions
    {
        public static T Log<T>(Func<T> fn)
        {
            try
            {
                return fn();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static void Log(Action fn)
        {
            try
            {
                fn();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
