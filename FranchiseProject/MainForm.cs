//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using Newtonsoft.Json;
//using System.Diagnostics;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace FranchiseProject
{
    public partial class MainForm : Form
    {
        // 지역명, 위도, 경도  (ex. "문정동", "37.412412", "124.512512")
        List<Tuple<string, double, double>> tuples = new List<Tuple<string, double, double>>();
        // DB 불러오기
        private const string ConnectionString = "Host=10.10.20.103;Username=postgres;Password=1234;Database=franchise";


        public MainForm()
        {
            FontLoad();
            InitializeComponent();
            InitializeComboBoxes();
        }

        // 폰트 불러오는 함수
        public static void FontLoad()
        {
            // 폰트 경로를 배열로 저장 후 부모경로를 통해 상대경로를 뽑아냄
            string[] fontPaths = { @"font\Maplestory_Bold.ttf", @"font\Maplestory_Light.ttf" };
            string baseDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            // 객체를 생성
            PrivateFontCollection privateFonts = new PrivateFontCollection();

            // 폰트를 가져온 후 추가
            foreach (string fontPath in fontPaths)
            {
                string fontFilePath = Path.Combine(baseDirectory, fontPath);
                privateFonts.AddFontFile(fontFilePath);
            }
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
                Console.WriteLine(query);
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
            flatComboBox1.Items.AddRange(data); // 콤보박스에 자료 넣기
            flatComboBox1.SelectedIndex = 0; // 첫번째 아이템 선택
        }

        //private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        //{
        //    // 첫 번째 콤보박스의 선택에 따라 두 번째 콤보박스의 항목을 설정
        //    string selectedGu = comboBox1.SelectedItem.ToString();
        //    update_combobox2(selectedGu);
        //    comboBox2.SelectedIndex = 0;
        //}

        private void flatComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 첫 번째 콤보박스의 선택에 따라 두 번째 콤보박스의 항목을 설정
            string selectedGu = flatComboBox1.SelectedItem.ToString();
            update_combobox2(selectedGu);
            flatComboBox2.SelectedIndex = 0;
        }


        private void update_combobox2(string guName)
        {
            // 두 번째 콤보박스의 항목을 초기화
            flatComboBox2.Items.Clear();
            List<string> DongNames = GetValuesFromTable("TB_DONG", "H_DONG_NAME", $"\"GU_NAME\" = '{guName}' ORDER BY \"H_DONG_NAME\" ", true);

            foreach (string dong in DongNames)
            {
                flatComboBox2.Items.Add(dong); // ComboBox에 d를 추가합니다.
            }
        }

        private void update_tabpage(string guName, string dongName)
        {
            // 현재 comboBox2에서 선택된 동(Dong) 이름을 가져옴
            string dong = flatComboBox2.Text;

            // DB로부터 가져올 칼럼 이름들을 리스트로 정의
            var columns = new List<string> { "DEAL_TYPE", "DEAL_USE", "DEAL_GU", "DEAL_DONG", "DEAL_ADDR", "DEAL_DEPOSIT", "DEAL_PRICE", "DEAL_RENT_PRICE", "DEAL_SPACE" };

            // DB로부터 지정된 조건의 레코드들을 가져옴
            var data = GetValuesFromMultipleColumns("TB_DEAL", columns, $"\"DEAL_DONG\" = '{dongName}'");

            // 기존 리스트 뷰 아이템을 모두 지움
            listView1.Items.Clear();
            listView2.Items.Clear();

            // 가져온 각 행(레코드)에 대해 아래의 작업을 수행
            foreach (var row in data)
            {
                // 해당 행에서 필요한 데이터를 가져옴
                string dealType = row["DEAL_TYPE"].ToString();
                string dealUse = row["DEAL_USE"].ToString();
                string dealDeposit = row["DEAL_DEPOSIT"].ToString();
                string dealPrice = row["DEAL_PRICE"].ToString();
                string dealRentprice = row["DEAL_RENT_PRICE"].ToString();
                string dealSpace = row["DEAL_SPACE"].ToString();

                // 거래 유형이 '매매'인 경우
                if (dealType == "매매")
                {
                    // listView1에 해당 아이템을 추가
                    var item = new ListViewItem(dealType);
                    item.SubItems.Add($"{dealUse}");
                    item.SubItems.Add($"{dealPrice}");
                    item.SubItems.Add($"{dealSpace}");
                    listView1.Items.Add(item);
                }

                // 거래 유형이 '월세'인 경우
                else if (dealType == "월세")
                {
                    // listView2에 해당 아이템을 추가
                    var item = new ListViewItem(dealType);
                    item.SubItems.Add($"{dealUse}");
                    item.SubItems.Add($"{dealDeposit}");
                    item.SubItems.Add($"{dealRentprice}");
                    item.SubItems.Add($"{dealSpace}");
                    listView2.Items.Add(item);
                }

                // 출력 확인용: 현재 행의 모든 열(칼럼) 데이터를 콘솔에 출력
                foreach (var keyValuePair in row)
                {
                    Console.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value}");
                }
            }
        }

        private void update_picturebox(string guName, string dongName)
        {
            // 필요한 열 이름들을 리스트에 저장
            var columns = new List<string> { "GU_NAME", "H_DONG_NAME" };

            // GetValuesFromMultipleColumns 메서드를 사용하여 데이터베이스에서 해당 조건에 맞는 데이터를 가져옴
            var data = GetValuesFromMultipleColumns("TB_DONG", columns, $" \"GU_NAME\" = '{guName}' and \"H_DONG_NAME\" = '{dongName}' ");

            // 가져온 데이터가 없으면 함수를 종료
            if (data.Count == 0)
            {
                return;
            }

            // 첫 번째 행의 데이터를 가져옴
            var row = data[0];
            string dongName_ = row["H_DONG_NAME"].ToString();
            string guName_ = row["GU_NAME"].ToString();

            // 이미지 파일들이 저장된 폴더 경로들을 배열에 저장
            string[] folderNames = {
                @"graph\00_동별_다중이용시설",
                @"graph\01_동별_인구비율",
                @"graph\02_동별_면적범위별_평균보증금_임대료_pastel",
                @"graph\03_구별_1030인구대비_월평균추정매출",
                @"graph\04_구별_월평균추정매출_경쟁업체",
                @"graph\05_전역_광주광역시_화장품상가_분포도"
            };

            string currentDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            // 각 폴더에서 해당 구와 동 이름을 포함하는 이미지 파일들의 경로를 가져옴
            for (int idx = 0; idx < folderNames.Length; idx++)
            {
                string folderPath = Path.Combine(currentDirectory, folderNames[idx]);
                List<string> imageFiles = GetMatchingImageFiles(folderPath, guName, dongName);

                // 이미지 파일이 없는 경우 none_data 이미지로 설정
                if (imageFiles.Count == 0)
                {
                    string noneDataImagePath = Path.Combine(folderPath, "none_data.png");
                    Image noneDataImage = Image.FromFile(noneDataImagePath);

                    // 현재 폴더 인덱스에 따라 해당 PictureBox에 none_data를 설정
                    if (idx == 0)
                    {
                        facPictureBox.Image = noneDataImage;
                    }
                    else if (idx == 1)
                    {
                        popPictureBox.Image = noneDataImage;
                    }
                    else if (idx == 2)
                    {
                        pricePictureBox.Image = noneDataImage;
                    }
                    else if (idx == 3)
                    {
                        guPictureBox1.Image = noneDataImage;
                    }
                    else if (idx == 4)
                    {
                        guPictureBox2.Image = noneDataImage;
                    }
                }
                else
                {
                    // 이미지 파일이 있는 경우, 해당 이미지를 설정
                    string imagePath = imageFiles[0];
                    Image image = Image.FromFile(imagePath);

                    // 현재 폴더 인덱스에 따라 해당 PictureBox에 이미지를 설정
                    if (idx == 0)
                    {
                        facPictureBox.Image = image;
                    }
                    else if (idx == 1)
                    {
                        popPictureBox.Image = image;
                    }
                    else if (idx == 2)
                    {
                        pricePictureBox.Image = image;
                    }
                    else if (idx == 3)
                    {
                        guPictureBox1.Image = image;
                    }
                    else if (idx == 4)
                    {
                        guPictureBox2.Image = image;
                    }
                }

                // 광주광역시 전체 데이터는 직접 경로와 파일명을 가져와서 pictureBox에 저장
                rivalPictureBox1.Image = Image.FromFile(GetImagePath(folderNames[5], $"광주광역시_전체상가_지도.png"));
                rivalPictureBox2.Image = Image.FromFile(GetImagePath(folderNames[5], $"광주광역시_화장품상가_지도.png"));
            }
        }

        // 주어진 폴더 경로와 파일 이름을 기반으로 이미지 파일의 전체 경로를 생성하는 함수
        string GetImagePath(string folderPath, string fileName)
        {
            // 상대 경로를 가져옴
            string currentDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            // Path.Combine 메서드를 사용하여 현재 디렉토리, 폴더 경로, 파일 이름을 결합하여 이미지 파일의 전체 경로를 반환함
            return Path.Combine(currentDirectory, folderPath, fileName);
        }


        // 주어진 기본 폴더 경로(baseFolderPath), 구이름(guName), 동이름(dongName)에 맞는 이미지 파일들을 찾아서 리스트로 반환
        private List<string> GetMatchingImageFiles(string baseFolderPath, string guName, string dongName)
        {
            // 결과로 반환할 이미지 파일들을 담을 리스트 생성
            List<string> imageFiles = new List<string>();

            // 지정된 폴더 경로에서 특정 패턴에 맞는 파일들을 찾아옴 ('구이름_동이름.*' 형태)
            string[] filesWithDong = Directory.GetFiles(baseFolderPath, $"{guName}_{dongName}.*");
            string[] filesWithoutDong = Directory.GetFiles(baseFolderPath, $"{guName}.*");

            // '구이름_동이름.*' 형태의 파일이 없을 경우 '구이름.*' 형태의 파일들을 가져옴
            if (filesWithDong.Length == 0)
            {
                imageFiles.AddRange(filesWithoutDong);
            }

            else
            {
                // '구이름_동이름.*' 형태의 파일이 있을 경우 해당 파일들을 반환할 리스트에 추가
                imageFiles.AddRange(filesWithDong);
            }

            // 결과 리스트 반환
            return imageFiles;
        }

        // 지도
        private void MainForm_Load(object sender, EventArgs e)

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
            string gu = flatComboBox1.Text;
            string dong = flatComboBox2.Text;
            string new_addr = "광주광역시 " + gu + dong;

            // 튜플에 값 넣기
            Search(new_addr);
            var sel = tuples[0];

            // 위도, 경도 불러와서 이동
            object[] arr = new object[] { sel.Item3, sel.Item2 }; // 위도, 경도
            object res = webBrowser1.Document.InvokeScript("panTo", arr);
            update_tabpage(gu, dong);
            update_picturebox(gu, dong);

            // 올리브영 위치 찍기
            var columns = new List<string> { "LOC_NAME", "LOC_ADDR", "LOC_X", "LOC_Y" };
            var condition = $"\"LOC_GU\" = '{gu}' AND \"LOC_DONG\" = \'{dong}\'";

            var data = GetValuesFromMultipleColumns("TB_LOCATION", columns, condition, false);
            StringBuilder jsCode = new StringBuilder();
            jsCode.AppendLine($"remove_markers('olive_young');");

            if (data != null && data.Count > 0)
            {
                jsCode.AppendLine($"add_markers('olive_young', [");
                foreach (var row in data)
                {
                    string name = row["LOC_NAME"].ToString(); // 업체명
                    string addr = row["LOC_ADDR"].ToString();  // 주소 
                    string x = row["LOC_X"].ToString(); //x좌표
                    string y = row["LOC_Y"].ToString(); //y좌표
                    Console.WriteLine(name + addr + x + y); // 확인용

                    // 각 시설의 정보를 바탕으로 JavaScript 코드를 추가
                    jsCode.AppendLine($"{{ title: '{name}', addr: '{addr}', latlng: new kakao.maps.LatLng({x}, {y}) }},");
                }
                jsCode.AppendLine("]);");
                Console.WriteLine(jsCode.ToString());

                // 생성된 JavaScript 코드를 웹 브라우저 컨트롤을 통해 실행
                webBrowser1.Document.InvokeScript("eval", new object[] { jsCode.ToString() });
            }


        }

        // 체크박스의 상태(선택/해제)에 따라 지도 상에 마커를 표시하거나 삭제
        private void show_checkbox_markers(System.Windows.Forms.CheckBox checkBox)
        {
            List<Dictionary<string, object>> facility_rows = GetFacilitiesByTypeAndLocation(checkBox, flatComboBox1, flatComboBox2);

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



        // 다중이용시설 체크박스 이벤트 연결
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

        private void resultButton_Click(object sender, EventArgs e)
        {

        }

    }
}
