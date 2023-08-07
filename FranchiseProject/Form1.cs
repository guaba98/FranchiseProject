//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using Newtonsoft.Json;
//using System.Diagnostics;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        // 특정 테이블의 여러 컬럼 값을 반환할 
        public static List<Dictionary<string, object>> GetValuesFromMultipleColumns(string tableName, List<string> columnNames, string criteria = null, bool distinct = false)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string columns = string.Join(", ", columnNames.Select(c => $"\"{c}\""));
                string distinctClause = distinct ? "DISTINCT" : ""; // distinct 값에 따라 쿼리 조각 결정
                string query = $"SELECT {distinctClause} {columns} FROM \"{tableName}\"";

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


        }

        // 체크박스의 상태(선택/해제)에 따라 지도 상에 마커를 표시하거나 삭제
        private void show_checkbox_markers(System.Windows.Forms.CheckBox checkBox)
        {
            List<Dictionary<string, object>> facility_rows = GetFacilitiesByTypeAndLocation(checkBox, comboBox1, comboBox2);

            StringBuilder jsCode = new StringBuilder(); // JavaScript 코드를 동적으로 생성하기 위한 StringBuilder
            string facilityType = checkBox.Tag.ToString();  //체크박스의 태그 값을 사용하여 시설 유형을 가져옴 -> ui에서 수정함

            // 체크박스 선택되었을 때
            if (checkBox.Checked)
            {
                jsCode.AppendLine($"add_markers('{facilityType}', [");
                foreach (var row in facility_rows)
                {
                    string name = row["FACILITY_NAME"].ToString(); // 업체명
                    string addr = row["FACILITY_ADDR"].ToString();  // 주소 
                    string x = row["FACILITY_X"].ToString(); //x좌표
                    string y = row["FACILITY_Y"].ToString(); //y좌표
                    Console.WriteLine(name + addr + x + y); // 확인용

                    // // 각 시설의 정보를 바탕으로 JavaScript 코드를 추가
                    jsCode.AppendLine($"{{ title: '{name}', addr: '{addr}', latlng: new kakao.maps.LatLng({x}, {y}) }},");
                }
                jsCode.AppendLine("]);");
            }
            else // 체크박스 해제되었을 때 마커 삭제 명령 이동
            {
                jsCode.AppendLine($"remove_markers('{facilityType}');");
            }

            // 생성된 JavaScript 코드를 웹 브라우저 컨트롤을 통해 실행
            webBrowser1.Document.InvokeScript("eval", new object[] { jsCode.ToString() });
        }



        // 체크박스의 이름을 참조해서 db에서 값을 가져온다.(인자: 체크박스, 구 콤보박스, 동 콤보박스)
        private List<Dictionary<string, object>> GetFacilitiesByTypeAndLocation(CheckBox checkBox, ComboBox guComboBox, ComboBox dongComboBox)
        {
            // 데이터 가져옴
            string facilityType = checkBox.Tag.ToString(); // Tag에서 시설 타입 가져오기
            string gu = guComboBox.Text;
            string dong = dongComboBox.Text;
            string condition = "";

            // 편의시설 합친 것 때문에 수정해줌
            if (facilityType == "음식점")
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" IN ('음식점', '패스트푸드', '피자', '제빵', '음식점', '치킨', '분식', '술집')";
            }
            else if (facilityType == "쇼핑몰")
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" IN ('쇼핑몰', '할인점')";
            }
            else if (facilityType == "중/고등학교")
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" IN ('중학교', '고등학교')";
            }
            else if (facilityType == "문화시설")
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" IN ('문화시설', '영화관')";
            }
            else
            {
                condition = $"\"FACILITY_GU\" = '{gu}' AND \"FACILITY_DONG\" = '{dong}' AND \"FACILITY_TYPE\" = '{facilityType}'";
            }

            return GetAllRowsFromTable("TB_FACILITY", condition);
        }




        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //편의점
            show_checkbox_markers(sender as CheckBox);

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //카페
            show_checkbox_markers(sender as CheckBox);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            //은행
            show_checkbox_markers(sender as CheckBox);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            //쇼핑몰
            show_checkbox_markers(sender as CheckBox);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            //병원
            show_checkbox_markers(sender as CheckBox);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            //음식점
            show_checkbox_markers(sender as CheckBox);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            //공용주차장
            show_checkbox_markers(sender as CheckBox);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            // 중 고등학교
            show_checkbox_markers(sender as CheckBox);
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            // 대학교
            show_checkbox_markers(sender as CheckBox);
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            //문화시설
            show_checkbox_markers(sender as CheckBox);
        }
    }
}
