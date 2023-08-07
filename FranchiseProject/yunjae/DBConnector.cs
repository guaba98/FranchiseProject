using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
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

        private void loadDB()
        {
            string query = $"SELECT * FROM public.\"TB_LOCATION\"";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTIONSTRING))
                {
                    conn.Open();
                    NpgsqlCommand dbCommand = new NpgsqlCommand(query, conn);
                    NpgsqlDataReader dbReader = dbCommand.ExecuteReader();

                    List<object> myData = new List<object>();
                    while (dbReader.Read())
                    {
                        var item = new
                        {
                            LOC_ID = dbReader["LOC_ID"],
                            LOC_GU = dbReader["LOC_GU"],
                            LOC_DONG = dbReader["LOC_DONG"],
                            LOC_NAME = dbReader["LOC_NAME"],
                            LOC_ADDR = dbReader["LOC_ADDR"],
                            LOC_X = dbReader["LOC_X"],
                            LOC_Y = dbReader["LOC_Y"]
                        };

                        myData.Add(item);
                    }


                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string jsonData = serializer.Serialize(myData);
                    Console.WriteLine(jsonData);
                    string htmlTemplate = File.ReadAllText("kakaoMap.html");
                    string finalHtml = htmlTemplate.Replace("{{JSON_DATA}}", jsonData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
