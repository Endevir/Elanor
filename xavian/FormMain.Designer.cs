namespace Xavian
{
    partial class FormMain
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
            this.panelWhole = new System.Windows.Forms.TableLayoutPanel();
            this.gbSource = new System.Windows.Forms.GroupBox();
            this.buttonSource = new System.Windows.Forms.Button();
            this.gbSetup = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cbdbText = new System.Windows.Forms.CheckBox();
            this.cbdbImage = new System.Windows.Forms.CheckBox();
            this.cbdbSound = new System.Windows.Forms.CheckBox();
            this.cbdbFont = new System.Windows.Forms.CheckBox();
            this.cbdbTexture = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbfText = new System.Windows.Forms.CheckBox();
            this.cbfImage = new System.Windows.Forms.CheckBox();
            this.cbfSound = new System.Windows.Forms.CheckBox();
            this.cbfFont = new System.Windows.Forms.CheckBox();
            this.cbfTexture = new System.Windows.Forms.CheckBox();
            this.gbExecute = new System.Windows.Forms.GroupBox();
            this.buttonExtract = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.panelWhole.SuspendLayout();
            this.gbSource.SuspendLayout();
            this.gbSetup.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.gbExecute.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWhole
            // 
            this.panelWhole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelWhole.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.panelWhole.ColumnCount = 1;
            this.panelWhole.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelWhole.Controls.Add(this.gbSource, 0, 0);
            this.panelWhole.Controls.Add(this.gbSetup, 0, 1);
            this.panelWhole.Controls.Add(this.gbExecute, 0, 2);
            this.panelWhole.Location = new System.Drawing.Point(11, 11);
            this.panelWhole.Margin = new System.Windows.Forms.Padding(2);
            this.panelWhole.Name = "panelWhole";
            this.panelWhole.RowCount = 3;
            this.panelWhole.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.panelWhole.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelWhole.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.panelWhole.Size = new System.Drawing.Size(274, 300);
            this.panelWhole.TabIndex = 0;
            // 
            // gbSource
            // 
            this.gbSource.Controls.Add(this.buttonSource);
            this.gbSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSource.Location = new System.Drawing.Point(5, 5);
            this.gbSource.Margin = new System.Windows.Forms.Padding(2);
            this.gbSource.Name = "gbSource";
            this.gbSource.Padding = new System.Windows.Forms.Padding(2);
            this.gbSource.Size = new System.Drawing.Size(264, 68);
            this.gbSource.TabIndex = 0;
            this.gbSource.TabStop = false;
            this.gbSource.Text = "Исходные данные";
            // 
            // buttonSource
            // 
            this.buttonSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSource.Location = new System.Drawing.Point(2, 15);
            this.buttonSource.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(260, 51);
            this.buttonSource.TabIndex = 0;
            this.buttonSource.Text = "Выберите файл (.dat)";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.buttonSource_Click);
            // 
            // gbSetup
            // 
            this.gbSetup.Controls.Add(this.tableLayoutPanel2);
            this.gbSetup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSetup.Location = new System.Drawing.Point(5, 80);
            this.gbSetup.Margin = new System.Windows.Forms.Padding(2);
            this.gbSetup.Name = "gbSetup";
            this.gbSetup.Padding = new System.Windows.Forms.Padding(2);
            this.gbSetup.Size = new System.Drawing.Size(264, 140);
            this.gbSetup.TabIndex = 1;
            this.gbSetup.TabStop = false;
            this.gbSetup.Text = "Настройка";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.Controls.Add(this.cbdbText, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.cbdbImage, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.cbdbSound, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.cbdbFont, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.cbdbTexture, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label6, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label7, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.cbfText, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.cbfImage, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.cbfSound, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.cbfFont, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.cbfTexture, 1, 5);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(260, 123);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // cbdbText
            // 
            this.cbdbText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbdbText.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbdbText.Location = new System.Drawing.Point(190, 23);
            this.cbdbText.Margin = new System.Windows.Forms.Padding(2);
            this.cbdbText.Name = "cbdbText";
            this.cbdbText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbdbText.Size = new System.Drawing.Size(59, 13);
            this.cbdbText.TabIndex = 0;
            this.cbdbText.UseVisualStyleBackColor = true;
            // 
            // cbdbImage
            // 
            this.cbdbImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbdbImage.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbdbImage.Location = new System.Drawing.Point(190, 43);
            this.cbdbImage.Margin = new System.Windows.Forms.Padding(2);
            this.cbdbImage.Name = "cbdbImage";
            this.cbdbImage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbdbImage.Size = new System.Drawing.Size(59, 13);
            this.cbdbImage.TabIndex = 1;
            this.cbdbImage.UseVisualStyleBackColor = true;
            // 
            // cbdbSound
            // 
            this.cbdbSound.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbdbSound.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbdbSound.Location = new System.Drawing.Point(190, 63);
            this.cbdbSound.Margin = new System.Windows.Forms.Padding(2);
            this.cbdbSound.Name = "cbdbSound";
            this.cbdbSound.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbdbSound.Size = new System.Drawing.Size(59, 13);
            this.cbdbSound.TabIndex = 2;
            this.cbdbSound.UseVisualStyleBackColor = true;
            // 
            // cbdbFont
            // 
            this.cbdbFont.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbdbFont.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbdbFont.Location = new System.Drawing.Point(190, 83);
            this.cbdbFont.Margin = new System.Windows.Forms.Padding(2);
            this.cbdbFont.Name = "cbdbFont";
            this.cbdbFont.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbdbFont.Size = new System.Drawing.Size(59, 13);
            this.cbdbFont.TabIndex = 3;
            this.cbdbFont.UseVisualStyleBackColor = true;
            // 
            // cbdbTexture
            // 
            this.cbdbTexture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbdbTexture.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbdbTexture.Location = new System.Drawing.Point(190, 103);
            this.cbdbTexture.Margin = new System.Windows.Forms.Padding(2);
            this.cbdbTexture.Name = "cbdbTexture";
            this.cbdbTexture.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbdbTexture.Size = new System.Drawing.Size(59, 14);
            this.cbdbTexture.TabIndex = 4;
            this.cbdbTexture.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "тексты";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "изображения";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "озвучка";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "шрифты";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "текстуры";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(117, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "в файлы";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(205, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "в бд";
            // 
            // cbfText
            // 
            this.cbfText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbfText.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbfText.Enabled = false;
            this.cbfText.Location = new System.Drawing.Point(112, 23);
            this.cbfText.Margin = new System.Windows.Forms.Padding(2);
            this.cbfText.Name = "cbfText";
            this.cbfText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbfText.Size = new System.Drawing.Size(59, 13);
            this.cbfText.TabIndex = 12;
            this.cbfText.UseVisualStyleBackColor = true;
            // 
            // cbfImage
            // 
            this.cbfImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbfImage.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbfImage.Location = new System.Drawing.Point(112, 43);
            this.cbfImage.Margin = new System.Windows.Forms.Padding(2);
            this.cbfImage.Name = "cbfImage";
            this.cbfImage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbfImage.Size = new System.Drawing.Size(59, 13);
            this.cbfImage.TabIndex = 13;
            this.cbfImage.UseVisualStyleBackColor = true;
            // 
            // cbfSound
            // 
            this.cbfSound.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbfSound.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbfSound.Location = new System.Drawing.Point(112, 63);
            this.cbfSound.Margin = new System.Windows.Forms.Padding(2);
            this.cbfSound.Name = "cbfSound";
            this.cbfSound.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbfSound.Size = new System.Drawing.Size(59, 13);
            this.cbfSound.TabIndex = 14;
            this.cbfSound.UseVisualStyleBackColor = true;
            // 
            // cbfFont
            // 
            this.cbfFont.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbfFont.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbfFont.Location = new System.Drawing.Point(112, 83);
            this.cbfFont.Margin = new System.Windows.Forms.Padding(2);
            this.cbfFont.Name = "cbfFont";
            this.cbfFont.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbfFont.Size = new System.Drawing.Size(59, 13);
            this.cbfFont.TabIndex = 15;
            this.cbfFont.UseVisualStyleBackColor = true;
            // 
            // cbfTexture
            // 
            this.cbfTexture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbfTexture.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbfTexture.Location = new System.Drawing.Point(112, 103);
            this.cbfTexture.Margin = new System.Windows.Forms.Padding(2);
            this.cbfTexture.Name = "cbfTexture";
            this.cbfTexture.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbfTexture.Size = new System.Drawing.Size(59, 14);
            this.cbfTexture.TabIndex = 16;
            this.cbfTexture.UseVisualStyleBackColor = true;
            // 
            // gbExecute
            // 
            this.gbExecute.Controls.Add(this.buttonExtract);
            this.gbExecute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbExecute.Location = new System.Drawing.Point(6, 228);
            this.gbExecute.Name = "gbExecute";
            this.gbExecute.Size = new System.Drawing.Size(262, 66);
            this.gbExecute.TabIndex = 2;
            this.gbExecute.TabStop = false;
            this.gbExecute.Text = "Выполнение";
            // 
            // buttonExtract
            // 
            this.buttonExtract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonExtract.Location = new System.Drawing.Point(3, 16);
            this.buttonExtract.Margin = new System.Windows.Forms.Padding(2);
            this.buttonExtract.Name = "buttonExtract";
            this.buttonExtract.Size = new System.Drawing.Size(256, 47);
            this.buttonExtract.TabIndex = 2;
            this.buttonExtract.Text = "Извлечь";
            this.buttonExtract.UseVisualStyleBackColor = true;
            this.buttonExtract.Click += new System.EventHandler(this.buttonExtract_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 313);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(296, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.ForeColor = System.Drawing.Color.YellowGreen;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(292, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.Visible = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 335);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panelWhole);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unpacker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.panelWhole.ResumeLayout(false);
            this.gbSource.ResumeLayout(false);
            this.gbSetup.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.gbExecute.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel panelWhole;
        private System.Windows.Forms.GroupBox gbSource;
        private System.Windows.Forms.GroupBox gbSetup;
        private System.Windows.Forms.Button buttonSource;
        private System.Windows.Forms.Button buttonExtract;
        private System.Windows.Forms.CheckBox cbdbFont;
        private System.Windows.Forms.CheckBox cbdbSound;
        private System.Windows.Forms.CheckBox cbdbImage;
        private System.Windows.Forms.CheckBox cbdbText;
        private System.Windows.Forms.CheckBox cbdbTexture;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbExecute;
        private System.Windows.Forms.CheckBox cbfText;
        private System.Windows.Forms.CheckBox cbfImage;
        private System.Windows.Forms.CheckBox cbfSound;
        private System.Windows.Forms.CheckBox cbfFont;
        private System.Windows.Forms.CheckBox cbfTexture;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
    }
}