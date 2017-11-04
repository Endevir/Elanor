using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using shared;

namespace Xavian
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            _fileBoxes = new Dictionary<PatchContentType, CheckBox>
            {
                {PatchContentType.Text, cbfText},
                {PatchContentType.Sound, cbfSound},
                {PatchContentType.Image, cbfImage},
                {PatchContentType.Font, cbfFont},
                {PatchContentType.Texture, cbfTexture}
            };

            _dbBoxes = new Dictionary<PatchContentType, CheckBox>
            {
                {PatchContentType.Text, cbdbText},
                {PatchContentType.Sound, cbdbSound},
                {PatchContentType.Image, cbdbImage},
                {PatchContentType.Font, cbdbFont},
                {PatchContentType.Texture, cbdbTexture}
            };

            OnFileSelected(false);
        }

        private OpenFileDialog _fileDialog;
        private DatFile _datFile;
        
        private readonly Dictionary<PatchContentType, CheckBox> _fileBoxes;
        private readonly Dictionary<PatchContentType, CheckBox> _dbBoxes;

        private OpenFileDialog CreateOrOpenDialog()
        {
            var dialog = _fileDialog;

            if (_fileDialog == null)
            {
                dialog = new OpenFileDialog
                {
                    Multiselect = false,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Filter = @"Dat files (*.dat) | *.dat"
                };
            }

            return dialog;
        }

        private async void OnFileSelected(bool success)
        {
            if (_datFile != null)
            {
                _datFile.Flush();
                _datFile.Dispose();
                _datFile = null;
            }

            if (success)
            {
                buttonSource.Text = @"Выбран файл: " + _fileDialog.SafeFileName;

                panelWhole.Enabled = false;
                progressBar.Visible = true;
                progressBar.MarqueeAnimationSpeed = 100;

                await ReadAsync();

                panelWhole.Enabled = true;
                progressBar.Visible = false;
                progressBar.MarqueeAnimationSpeed = 0;
            }
            else
            {
                buttonSource.Text = @"Выберите файл (.dat)";
            }

            buttonExtract.Enabled = success;
        }

        private async Task ReadAsync()
        {
            await Task.Run(() => Read());
        }

        private void Read()
        {
            _datFile = new DatFile();
            _datFile.Open(_fileDialog.FileName, true);
        }

        private async void buttonExtract_Click(object sender, EventArgs e)
        {
            if (_fileDialog.FileName == null)
            {
                return;
            }

            if (!_fileBoxes.Any(fb => fb.Value.Checked) && !_dbBoxes.Any(fb => fb.Value.Checked))
            {
                return;
            }

            panelWhole.Enabled = false;
            progressBar.Visible = true;
            progressBar.MarqueeAnimationSpeed = 100;

            var results = await ExtractAsync();
            if (results.Count == 0)
            {
                MessageBox.Show(this, @"Не было извлечено ничего.");
            }
            else
            {
                var text = results.Aggregate("Извлечено файлов: \n", (current, dict) => current + $"{dict.Key} - {dict.Value}\n");
                MessageBox.Show(this, text);
            }

            panelWhole.Enabled = true;
            progressBar.Visible = false;
            progressBar.MarqueeAnimationSpeed = 0;
        }

        private async Task<Dictionary<PatchContentType, int>> ExtractAsync()
        {
            return await Task.Run(() => Extract());
        }

        private Dictionary<PatchContentType, int> Extract()
        {
            return Database.Import(_datFile, new ExtractMarker(_fileBoxes, _dbBoxes), _fileDialog.SafeFileName);
        }

        private void buttonSource_Click(object sender, EventArgs e)
        {
            OnFileSelected(false);
            _fileDialog = CreateOrOpenDialog();

            if (_fileDialog.ShowDialog() == DialogResult.OK)
            {
                OnFileSelected(true);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_datFile != null)
            {
                _datFile.Flush();
                _datFile.Dispose();
                _datFile = null;
            }
        }
    }
}
