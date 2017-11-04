using System;
using System.Windows.Forms;

namespace Mojo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private SourceFontInfo _bmfont;

        private void buttonSource_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = @"BMFont files (*.fnt) | *.fnt"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fnt = new SourceFontInfo(dialog.FileName);
                if (fnt.IsValid)
                {
                    _bmfont = fnt;
                    buttonDest.Enabled = true;
                }
                else
                {
                    _bmfont = null;
                    buttonDest.Enabled = false;
                }
            }
            else
            {
                _bmfont = null;
                buttonDest.Enabled = false;
            }
        }

        private void buttonDest_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = @"FontBin files (*.FontBin) | *.FontBin"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var filename in dialog.FileNames)
                {
                    var fnt = new DestFontInfo(_bmfont);
                    fnt.Import(filename);
                }
            }
        }
    }
}
