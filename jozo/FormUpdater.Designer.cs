// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using System.Windows.Forms;

namespace Jozo
{
    partial class FormUpdater
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdater));
            this.pictureBoxUpdater = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUpdater)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxUpdater
            // 
            this.pictureBoxUpdater.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxUpdater.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxUpdater.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxUpdater.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxUpdater.Image")));
            this.pictureBoxUpdater.InitialImage = null;
            this.pictureBoxUpdater.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxUpdater.Name = "pictureBoxUpdater";
            this.pictureBoxUpdater.Size = new System.Drawing.Size(530, 225);
            this.pictureBoxUpdater.TabIndex = 2;
            this.pictureBoxUpdater.TabStop = false;
            this.pictureBoxUpdater.WaitOnLoad = true;
            // 
            // FormUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(530, 225);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBoxUpdater);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(530, 225);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(530, 225);
            this.Name = "FormUpdater";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Авто-обновление";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FormUpdater_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUpdater)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private PictureBox pictureBoxUpdater;
    }
}