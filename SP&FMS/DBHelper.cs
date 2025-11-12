using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SP_FMS
{
    public static class DBHelper
    {
        // ✅ Update your password here
       private static string connString = "server=localhost;user=root;database=sp_fms;port=3306;password=;";


        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connString);
        }
    }
}
