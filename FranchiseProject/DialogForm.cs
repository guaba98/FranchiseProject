using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FranchiseProject
{
    public partial class DialogForm : Form
    {
        MainForm mainForm = new MainForm();

        // 생성자
        public DialogForm(string GuName, string DongName, string minCost, string maxCost)
        {
            InitializeComponent();
            regionNameLabel.Text = $"{GuName} {DongName} 지역의 분석 결과는...";
            mainLabel.Text = $"예상 창업 비용은 \n {minCost} ~ {maxCost} 입니다.";
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
            mainForm.SetFont(dialogTitleLabel, 0, 20);
            mainForm.SetFont(regionNameLabel, 0, 15);
            mainForm.SetFont(mainLabel, 0, 13);
        }
    }
}
