using System.Windows;

namespace Elanor
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow
    {
        public SetupWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
        }

        private readonly MainWindow _mainWindow;
        private bool _newChecked;

        private async void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();

            if (_mainWindow != null && _newChecked)
            {
                await _mainWindow.RoutineAsync();
            }
        }

        private void CheckBoxSubscribe_OnChecked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                _newChecked = true;
            }
        }
    }
}
