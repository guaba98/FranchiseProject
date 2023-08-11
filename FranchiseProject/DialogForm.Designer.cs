namespace FranchiseProject
{
    partial class DialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.exitButton = new System.Windows.Forms.Button();
            this.dialogTitleLabel = new System.Windows.Forms.Label();
            this.regionNameLabel = new System.Windows.Forms.Label();
            this.mainLabel = new System.Windows.Forms.Label();
            this.finalLabel = new System.Windows.Forms.Label();
            this.rateLabel = new System.Windows.Forms.Label();
            this.gradePictureBox = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gradePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(205)))), ((int)(((byte)(74)))));
            this.panel1.Controls.Add(this.exitButton);
            this.panel1.Controls.Add(this.dialogTitleLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(396, 74);
            this.panel1.TabIndex = 0;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // exitButton
            // 
            this.exitButton.Font = new System.Drawing.Font("Pretendard", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.exitButton.Location = new System.Drawing.Point(367, 11);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(22, 23);
            this.exitButton.TabIndex = 2;
            this.exitButton.TabStop = false;
            this.exitButton.Text = "X";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // dialogTitleLabel
            // 
            this.dialogTitleLabel.Location = new System.Drawing.Point(99, 19);
            this.dialogTitleLabel.Name = "dialogTitleLabel";
            this.dialogTitleLabel.Size = new System.Drawing.Size(197, 39);
            this.dialogTitleLabel.TabIndex = 1;
            this.dialogTitleLabel.Text = "종합 분석 결과표";
            this.dialogTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // regionNameLabel
            // 
            this.regionNameLabel.Location = new System.Drawing.Point(31, 97);
            this.regionNameLabel.Name = "regionNameLabel";
            this.regionNameLabel.Size = new System.Drawing.Size(337, 39);
            this.regionNameLabel.TabIndex = 2;
            this.regionNameLabel.Text = "구_동 지역의 분석 결과는...";
            this.regionNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mainLabel
            // 
            this.mainLabel.Location = new System.Drawing.Point(31, 251);
            this.mainLabel.Name = "mainLabel";
            this.mainLabel.Size = new System.Drawing.Size(337, 117);
            this.mainLabel.TabIndex = 3;
            this.mainLabel.Text = "mainLabel";
            this.mainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // finalLabel
            // 
            this.finalLabel.Location = new System.Drawing.Point(31, 388);
            this.finalLabel.Name = "finalLabel";
            this.finalLabel.Size = new System.Drawing.Size(337, 76);
            this.finalLabel.TabIndex = 4;
            this.finalLabel.Text = "finalLabel";
            this.finalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rateLabel
            // 
            this.rateLabel.Location = new System.Drawing.Point(31, 200);
            this.rateLabel.Name = "rateLabel";
            this.rateLabel.Size = new System.Drawing.Size(337, 42);
            this.rateLabel.TabIndex = 5;
            this.rateLabel.Text = "rateLabel";
            this.rateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gradePictureBox
            // 
            this.gradePictureBox.Location = new System.Drawing.Point(169, 139);
            this.gradePictureBox.Name = "gradePictureBox";
            this.gradePictureBox.Size = new System.Drawing.Size(60, 60);
            this.gradePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.gradePictureBox.TabIndex = 6;
            this.gradePictureBox.TabStop = false;
            // 
            // DialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 500);
            this.Controls.Add(this.rateLabel);
            this.Controls.Add(this.finalLabel);
            this.Controls.Add(this.mainLabel);
            this.Controls.Add(this.regionNameLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gradePictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DialogForm";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DialogForm";
            this.Load += new System.EventHandler(this.DialogForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.dialogForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DialogForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DialogForm_MouseMove);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gradePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label dialogTitleLabel;
        private System.Windows.Forms.Label regionNameLabel;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.Label finalLabel;
        private System.Windows.Forms.Label rateLabel;
        private System.Windows.Forms.PictureBox gradePictureBox;
    }
}