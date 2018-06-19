using System;
using System.Collections.Generic;
using System.Text;

namespace CrossSolar.Tests
{
    public static class DbHelper
    {
        static string _constring = "data source=VIMAL;initial catalog=CrossSolarDB;integrated security=True;multipleactiveresultsets=True;App=EntityFramework;";

        public static string GetConnectionString()
        {
            return _constring;
        }
    }
}
