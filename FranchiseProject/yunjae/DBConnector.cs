using Npgsql;
using System;
using System.Windows.Forms;

namespace FranchiseProject.yunjae
{
    internal class DBConnector
    {

        // 생성자
        public DBConnector() { }
        public NpgsqlConnection conn { get; set; }

        private static string HOST = "10.10.20.103";
        private static string PORT = "5432";
        private static string DATABASENAME = "franchise";
        private static string USERNAME = "postgres";
        private static string USERPASSWORD = "1234";
        private static string CONNECTIONSTRING = $"HOST={HOST}; PORT={PORT}; DATABASE={DATABASENAME}; USERNAME={USERNAME}; PASSWORD={USERPASSWORD}";

        public NpgsqlConnection ConnectDB()
        {
            try
            {
                conn = new NpgsqlConnection(CONNECTIONSTRING);
                conn.Open();
                return conn;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

    }
}
