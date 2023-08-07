//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using Newtonsoft.Json;
//using System.Diagnostics;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace FranchiseProject
{
    public partial class Form1 : Form
    {
        // 지역명, 위도, 경도  (ex. "문정동", "37.412412", "124.512512")
        List<Tuple<string, double, double>> tuples = new List<Tuple<string, double, double>>();

        // DB 불러오기
        private const string ConnectionString = "Host=10.10.20.103;Username=postgres;Password=1234;Database=franchise";

        // 생성자
        public Form1()
        {
            InitializeComponent();
            InitializeComboBoxes();
        }

        // DB
        // 특정 테이블에서 특정 칼럼의 값을 반환하는 함수
        public static List<string> GetValuesFromTable(string tableName, string columnName, string criteria = null, bool distinct = false)
        {
            List<string> results = new List<string>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string distinctClause = distinct ? "DISTINCT" : ""; // distinct 값에 따라 쿼리 조각 결정
                string query = $"SELECT {distinctClause} \"{columnName}\" FROM \"{tableName}\"";

                if (!string.IsNullOrWhiteSpace(criteria))
                {
                    query += $" WHERE {criteria}";
                }

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return results;
        }

        // 특정 테이블의 모든 행의 값을 반환하는 함수
        public static List<Dictionary<string, object>> GetAllRowsFromTable(string tableName, string criteria = null)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string query = $"SELECT * FROM \"{tableName}\"";

                if (!string.IsNullOrWhiteSpace(criteria))
                {
                    query += $" WHERE {criteria}";
                }

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }

        // 콤보박스
        private void InitializeComboBoxes()
        {
            //콤보박스
            string[] data = { "북구", "서구", "동구", "남구", "광산구" };
            comboBox1.Items.AddRange(data); // 콤보박스에 자료 넣기
            comboBox1.SelectedIndex = 0; // 첫번째 아이템 선택
        }
        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

            // 첫 번째 콤보박스의 선택에 따라 두 번째 콤보박스의 항목을 설정
            string selectedGu = comboBox1.SelectedItem.ToString();
            update_combobox2(selectedGu);
            comboBox2.SelectedIndex = 0;

        }
        private void update_combobox2(string guName)
        {
            // 두 번째 콤보박스의 항목을 초기화
            comboBox2.Items.Clear();
            List<string> DongNames = GetValuesFromTable("TB_DONG", "H_DONG_NAME", $"\"GU_NAME\" = '{guName}' ORDER BY \"H_DONG_NAME\" ", true);

            foreach (string dong in DongNames)
            {
                comboBox2.Items.Add(dong); // ComboBox에 d를 추가합니다.
            }
        }

        // 지도
        private void Form1_Load(object sender, EventArgs e)
        {
            // 로드될 때 생성
            // WebBrowser 컨트롤에 "kakaoMap.html" 을 표시한다. 
            Version ver = webBrowser1.Version;
            string name = webBrowser1.ProductName;
            string str = webBrowser1.ProductVersion;
            string html = "kakaoMap.html";
            string dir = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(dir, html);
            webBrowser1.Navigate(path);
        }

        public void Search(string area) // 지역 검색
        {
            // 요청을 보낼 url 
            string site = "https://dapi.kakao.com/v2/local/search/address.json";
            string query = string.Format("{0}?query={1}", site, area);
            WebRequest request = WebRequest.Create(query); // 요청 생성. 
            string api_key = "106e805bafc9548f37b878db306c0484"; // API 인증키 입력. (각자 발급한 API 인증키를 입력하자)
            string header = "KakaoAK " + api_key;


            request.Headers.Add("Authorization", header); // HTTP 헤더 "Authorization" 에 header 값 설정. 
            WebResponse response = request.GetResponse(); // 요청을 보내고 응답 객체를 받는다. 
            Stream stream = response.GetResponseStream(); // 응답객체의 결과물


            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            String json = reader.ReadToEnd(); // JOSN 포멧 문자열
            //Console.WriteLine("결과물" + json);

            JavaScriptSerializer js = new JavaScriptSerializer(); // (Reference 에 System.Web.Extensions.dll 을 추가해야한다)
            var dob = js.Deserialize<dynamic>(json);

            var docs = dob["documents"];
            object[] buf = docs;
            int length = buf.Length;

            for (int i = 0; i < length; i++) // 지역명, 위도, 경도 읽어오기. 
            {
                string address_name = docs[i]["address_name"];
                double x = double.Parse(docs[i]["x"]); // 위도
                double y = double.Parse(docs[i]["y"]); // 경도
                tuples.Add(new Tuple<string, double, double>(address_name, x, y));
                Console.WriteLine("저장한주소값: " + address_name + x + y);
            }
        }


        // 지도 확대 축소
        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.InvokeScript("zoomIn"); // 줌인
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.InvokeScript("zoomOut"); // 줌아웃
        }

        // 검색 버튼 눌렀을 때 연결
        private void button4_Click(object sender, EventArgs e)
        {
            //정보 불러오기
            tuples.Clear();
            string gu = comboBox1.Text;
            string dong = comboBox2.Text;
            string new_addr = "광주광역시 " + gu + dong;

            // 튜플에 값 넣기
            Search(new_addr);
            var sel = tuples[0];

            // 위도, 경도 불러와서 이동
            object[] arr = new object[] { sel.Item3, sel.Item2 }; // 위도, 경도
            object res = webBrowser1.Document.InvokeScript("panTo", arr);


            //// 특정 구, 동에서 편의시설 값 불러오기(GetAllRowsFromTable 함수 사용)
            //string condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}'";
            //List<Dictionary<string, object>> facility_rows = GetAllRowsFromTable("TB_FACILITY", condition);

            //// facility_data 딕셔너리에 값 넣기
            //foreach (var row in facility_rows)
            //{
            //    var facility_data = new Dictionary<string, object>
            //    {
            //        { "FACILITY_NAME", row["FACILITY_NAME"] },
            //        { "FACILITY_X", row["FACILITY_X"] },
            //        { "FACILITY_Y", row["FACILITY_Y"] }
            //     };

            //    string jsonData = JsonConvert.SerializeObject(facility_data);
            //    Console.WriteLine(jsonData);
            //    webBrowser1.Document.InvokeScript("set_marker", new object[] { jsonData });
            //}
            // 특정 구, 동에서 편의시설 값 불러오기(GetAllRowsFromTable 함수 사용)
            string condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}'";
            List<Dictionary<string, object>> facility_rows = GetAllRowsFromTable("TB_FACILITY", condition);

            // JavaScript 코드를 동적으로 작성하기
            StringBuilder jsCode = new StringBuilder();
            jsCode.AppendLine("set_marker([");
            foreach (var row in facility_rows)
            {
                string name = row["FACILITY_NAME"].ToString();
                string x = row["FACILITY_X"].ToString();
                string y = row["FACILITY_Y"].ToString();

                jsCode.AppendLine($"{{ title: '{name}', latlng: new kakao.maps.LatLng({x}, {y}) }},");
            }
            jsCode.AppendLine("]);");

            // 이제 'jsCode' 문자열을 웹 브라우저 컨트롤에 주입하여 실행
            webBrowser1.Document.InvokeScript("eval", new object[] { jsCode.ToString() });




            //// 마커찍기
            //webBrowser1.Document.InvokeScript("set_marker");

        }

    }
}
