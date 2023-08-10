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
        public DialogForm(string GuName, string DongName, string minCost, string maxCost, string salesIncome, string salesPeople, string facilityCnt)
        {
            InitializeComponent();
            regionNameLabel.Text = $"{GuName} {DongName} 지역의 분석 결과는...";
            mainLabel.Text = $"월 추정매출은 {salesIncome}만원 \n\n 유동인구는 {salesPeople}명 \n\n 다중 이용 시설 개수는 {facilityCnt}개";
            finalLabel.Text = $"예상 창업 비용은 \n\n {minCost} ~ {maxCost}입니다.";
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
