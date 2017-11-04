using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Elanor.Properties;
using Elanor.DatFile;
using Elanor.Misc;
using Microsoft.Win32;
using shared;

namespace Elanor
{
    /// <summary>
    /// Interaction logic for SwapWindow.xaml
    /// </summary>
    public partial class SwapWindow
    {
        public SwapWindow(SwapManager swapManger, bool launchGame)
        {
            _launchGame = launchGame;
            SwapManager = swapManger;

            InitializeComponent();

            InitWindow(launchGame);
        }

        public SwapManager SwapManager { get; }
        public ApplySubscriptionViewModel SubscriptionA { get; private set; }
        public ApplySubscriptionViewModel SubscriptionB { get; private set; }

        public bool IsLaunch { get; private set; }

        public bool IsB { get; private set; }

        private readonly bool _launchGame;

        private static string ShortenPath(string path)
        {
            // TODO
            return path;
        }

        private void InitWindow(bool isLaunch)
        {
            if (SwapManager.EntityB == null)
            {
                HideB();
                ButtonRemoveB.Visibility = Visibility.Collapsed;
            }
            else
            {
                ButtonAddB.Visibility = Visibility.Collapsed;
            }

            if (isLaunch)
            {
                TextBlockTitle.Text = "Запуск игры: выбор файла локализации";

                ButtonAddB.Visibility = Visibility.Collapsed;
                ButtonRemoveB.Visibility = Visibility.Collapsed;

                CheckBoxOrigTexts.IsEnabled = false;
                CheckBoxOrigFonts.IsEnabled = false;
                CheckBoxOrigSounds.IsEnabled = false;
                CheckBoxOrigVideos.IsEnabled = false;
                CheckBoxOrigLoadscreens.IsEnabled = false;
                CheckBoxOrigMaps.IsEnabled = false;
                CheckBoxOrigTextures.IsEnabled = false;

                CheckBoxAltTexts.IsEnabled = false;
                CheckBoxAltFonts.IsEnabled = false;
                CheckBoxAltSounds.IsEnabled = false;
                CheckBoxAltVideos.IsEnabled = false;
                CheckBoxAltLoadscreens.IsEnabled = false;
                CheckBoxAltMaps.IsEnabled = false;
                CheckBoxAltTextures.IsEnabled = false;

                TextBlockHint.Visibility = Visibility.Collapsed;
                Height = 280;
                MainGrid.RowDefinitions[9].Height = new GridLength(10);
            }
        }

        private void HideB(bool hide = true)
        {
            if (hide)
            {
                CheckBoxAltTexts.Visibility = Visibility.Collapsed;
                CheckBoxAltFonts.Visibility = Visibility.Collapsed;
                CheckBoxAltSounds.Visibility = Visibility.Collapsed;
                CheckBoxAltVideos.Visibility = Visibility.Collapsed;
                CheckBoxAltLoadscreens.Visibility = Visibility.Collapsed;
                CheckBoxAltMaps.Visibility = Visibility.Collapsed;
                CheckBoxAltTextures.Visibility = Visibility.Collapsed;
            }
            else
            {
                CheckBoxAltTexts.Visibility = Visibility.Visible;
                CheckBoxAltFonts.Visibility = Visibility.Visible;
                CheckBoxAltSounds.Visibility = Visibility.Visible;
                CheckBoxAltVideos.Visibility = Visibility.Visible;
                CheckBoxAltLoadscreens.Visibility = Visibility.Visible;
                CheckBoxAltMaps.Visibility = Visibility.Visible;
                CheckBoxAltTextures.Visibility = Visibility.Visible;
            }
        }

        public async Task InitEntitiesAsync()
        {
            await SwapManager.EntityA.DatController.GetMarkAsync();

            RenewViewA();

            if (SwapManager.EntityB != null)
            {
                await SwapManager.EntityB.InitAsync();
            }

            RenewViewB();
        }

        private void RenewViewA()
        {
            if (SwapManager.EntityA != null && SwapManager.EntityA.IsInit)
            {
                SubscriptionA = new ApplySubscriptionViewModel(SwapManager.EntityA.DatController);
                UpdateTargetA();                
            }
        }

        private void RenewViewB()
        {
            if (SwapManager.EntityB != null && SwapManager.EntityB.IsInit)
            {
                SubscriptionB = new ApplySubscriptionViewModel(SwapManager.EntityB.DatController);
                UpdateTargetB();                
            }
        }

        private void UpdateTargetA()
        {
            CheckBoxOrigTexts.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxOrigFonts.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxOrigSounds.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxOrigVideos.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxOrigLoadscreens.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxOrigMaps.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxOrigTextures.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();

            TextBlockOrigTexts.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockOrigFonts.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockOrigSounds.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockOrigVideos.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockOrigLoadscreens.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockOrigMaps.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockOrigTextures.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }

        private void UpdateTargetB()
        {
            CheckBoxAltTexts.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxAltFonts.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxAltSounds.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxAltVideos.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxAltLoadscreens.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxAltMaps.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            CheckBoxAltTextures.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();

            TextBlockAltTexts.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockAltFonts.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockAltSounds.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockAltVideos.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockAltLoadscreens.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockAltMaps.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
            TextBlockAltTextures.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }

        private async void ButtonAddB_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Dat files (*.dat)|*.dat"
            };

            var dr = dialog.ShowDialog();
            if (dr.HasValue && dr.Value && (SwapManager.EntityB == null || 
                string.CompareOrdinal(SwapManager.EntityB.DatFilePath, dialog.FileName) != 0))
            {
                SwapManager.EntityB?.CloseController();
                SwapManager.SetReserveEntity(dialog.FileName);

                try
                {
                    await SwapManager.EntityB.InitAsync();
                }
                finally
                {
                    HideB(false);
                    ButtonAddB.Visibility = Visibility.Collapsed;
                    ButtonRemoveB.Visibility = Visibility.Visible;

                    RenewViewB();

                    TextBlockPathAlt.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
                }
            }
        }

        private async void Button_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await SwapManager.EntityA.DatController.WriteNinjaMark();

                if (SwapManager.EntityB != null && SwapManager.EntityB.IsValid && SwapManager.EntityB.DatController != null)
                {
                    await SwapManager.EntityB.DatController.WriteNinjaMark();
                    Settings.Default.ReserveDat = SwapManager.EntityB.DatFilePath;
                }

                Settings.Default.Save();
            }
            finally
            {
                Close();                
            }
        }

        private void ButtonRemoveB_OnClick(object sender, RoutedEventArgs e)
        {
            SwapManager.RemoveReserveEntity();
            SubscriptionB = null;

            UpdateTargetB();
            TextBlockPathAlt.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();

            HideB();
            ButtonAddB.Visibility = Visibility.Visible;
            ButtonRemoveB.Visibility = Visibility.Collapsed;

            Settings.Default.ReserveDat = string.Empty;
            Settings.Default.Save();
        }

        private void GridActive_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_launchGame)
            {
                var glow = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    Opacity = 0.9,
                    BlurRadius = 15,
                    Color = Color.FromArgb(255, 75, 234, 50)
                };

                ImageActive.Effect = glow;
            }
        }

        private void GridActive_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_launchGame)
            {
                ImageActive.Effect = null;
            }
        }

        private void GridReserve_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_launchGame)
            {
                var glow = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    Opacity = 0.9,
                    BlurRadius = 15,
                    Color = Color.FromArgb(255, 235, 198, 58)
                };

                ImageReserve.Effect = glow;
            }
        }

        private void GridReserve_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_launchGame)
            {
                ImageReserve.Effect = null;
            }
        }

        private void GridActive_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_launchGame)
            {
                IsLaunch = true;
                IsB = false;

                Close();
            }
        }

        private void GridReserve_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_launchGame)
            {
                IsLaunch = true;
                IsB = true;

                Close();
            }
        }
    }

    public class ApplySubscriptionViewModel
    {
        public ApplySubscriptionViewModel(DatController datController)
        {
            _patches = datController.DatPatches;
        }

        private readonly Dictionary<PatchContentType, DatPatchInfo> _patches;

        public bool TextsSubscribed
        {
            get { return _patches[PatchContentType.Text].IsApplySubscribed; }
            set { _patches[PatchContentType.Text].IsApplySubscribed = value; }
        }

        public string TextsDaysAgo => DaysAgo(_patches[PatchContentType.Text].Date);

        public bool FontsSubscribed
        {
            get { return _patches[PatchContentType.Font].IsApplySubscribed; }
            set { _patches[PatchContentType.Font].IsApplySubscribed = value; }
        }

        public string FontsDaysAgo => DaysAgo(_patches[PatchContentType.Font].Date);

        public bool SoundsSubscribed
        {
            get { return _patches[PatchContentType.Sound].IsApplySubscribed; }
            set { _patches[PatchContentType.Sound].IsApplySubscribed = value; }
        }

        public string SoundsDaysAgo => DaysAgo(_patches[PatchContentType.Sound].Date);

        public bool VideosSubscribed
        {
            get { return _patches[PatchContentType.Video].IsApplySubscribed; }
            set { _patches[PatchContentType.Video].IsApplySubscribed = value; }
        }

        public string VideosDaysAgo => DaysAgo(_patches[PatchContentType.Video].Date);

        public bool TexturesSubscribed
        {
            get { return _patches[PatchContentType.Texture].IsApplySubscribed; }
            set { _patches[PatchContentType.Texture].IsApplySubscribed = value; }
        }

        public string TexturesDaysAgo => DaysAgo(_patches[PatchContentType.Texture].Date);

        public bool LoadscreensSubscribed
        {
            get { return _patches[PatchContentType.Loadscreen].IsApplySubscribed; }
            set { _patches[PatchContentType.Loadscreen].IsApplySubscribed = value; }
        }

        public string LoadscreensDaysAgo => DaysAgo(_patches[PatchContentType.Loadscreen].Date);

        public bool MapsSubscribed
        {
            get { return _patches[PatchContentType.Image].IsApplySubscribed; }
            set { _patches[PatchContentType.Image].IsApplySubscribed = value; }
        }

        public string MapsDaysAgo => DaysAgo(_patches[PatchContentType.Image].Date);

        private static string DaysAgo(string date)
        {
            return string.IsNullOrWhiteSpace(date)
                ? "без обновлений"
                : DaysFormat((DateTime.Now - DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture)).Days.ToString());
        }

        private static string DaysFormat(string days)
        {
            var prefix = days;

            if (days.EndsWith("11") || days.EndsWith("12") || days.EndsWith("13") || days.EndsWith("14"))
            {
                prefix += " дней";
            }
            else if (days.EndsWith("1"))
            {
                prefix += " день";
            }
            else if (days.EndsWith("2") || days.EndsWith("3") || days.EndsWith("4"))
            {
                prefix += " дня";
            }
            else
            {
                prefix += " дней";
            }

            return prefix + " назад";
        }
    }
}
