using System;
using System.Drawing;
using System.Windows.Forms;

namespace FranchiseProject
{
    public partial class DialogForm : Form
    {
        MainForm mainForm = new MainForm();

        // 마우스 드래그를 위한 offset 변수
        private Point offset;

        // 생성자
        public DialogForm(string guName,  // 구 이름
                          string dongName,  // 동 이름
                          string minCost,  // 최소 비용
                          string maxCost,  // 최대 비용
                          string salesIncome,  // 월 추정 매출
                          string salesPeople,  // 유동 인구 수
                          string facilityCnt,  // 다중 이용 시설 수
                          string resultRate,  // 적합 등급
                          string resultCnt,  // 올리브영 매장 수
                          string resultCompete  // 경쟁 업체 수
            )
        {
            InitializeComponent();
            SetGradePictureBox(resultRate);
            regionNameLabel.Text = $"{guName} {dongName} 지역의 분석 결과는...";
            mainLabel.Text = $"월 추정매출: {salesIncome}만원 \n 유동인구: {salesPeople}명 \n 다중 이용 시설 수: {facilityCnt}개 \n 경쟁업체 수: {resultCompete}개 \n 올리브영 매장 수: {resultCnt}개";
            finalLabel.Text = $"예상 창업 비용은 \n\n {minCost} ~ {maxCost}입니다.";

        }

        // gradePictureBox에 들어갈 이미지와 폰트 색상을 등급에 따라 결정합니다.
        private void SetGradePictureBox(string resultRate)
        {
            int grade = int.Parse(resultRate[0].ToString());
            gradePictureBox.Image = Properties.Resources.ResourceManager.GetObject($"_{grade}") as Image;

            // 0등급은 블랙 ~ 5등급 레드
            Color[] gradeColors = { Color.Black,  Color.Green, Color.LightGreen, Color.Orange, Color.OrangeRed, Color.Red };
            rateLabel.ForeColor = gradeColors[grade];
            rateLabel.Text = $"창업 적합도: {resultRate}";
        }

        // 닫기 버튼 함수
        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        // 다이얼로그 테두리 설정
        private void dialogForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                Color.Black, 2, ButtonBorderStyle.Solid,
                Color.Black, 2, ButtonBorderStyle.Solid,
                Color.Black, 2, ButtonBorderStyle.Solid,
                Color.Black, 2, ButtonBorderStyle.Solid);
        }

        private void DialogForm_Load(object sender, EventArgs e)
        {
            dialogTitleLabel.Font = new Font(FontManager.fontFamilys[0], 20, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            regionNameLabel.Font = new Font(FontManager.fontFamilys[0], 15, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            rateLabel.Font = new Font(FontManager.fontFamilys[0], 15, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            mainLabel.Font = new Font(FontManager.fontFamilys[0], 13, FontStyle.Regular, GraphicsUnit.Point, ((byte)(129)));
            finalLabel.Font = new Font(FontManager.fontFamilys[0], 13, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newLocation = this.Location;
                newLocation.X += e.X - offset.X;
                newLocation.Y += e.Y - offset.Y;
                this.Location = newLocation;
            }
        }

        private void DialogForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void DialogForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newLocation = this.Location;
                newLocation.X += e.X - offset.X;
                newLocation.Y += e.Y - offset.Y;
                this.Location = newLocation;
            }
        }
    }
}
