using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Elanor.Misc;
using shared;

namespace Elanor
{
    /// <summary>
    /// Interaction logic for DevMenuWindow.xaml
    /// </summary>
    public partial class DevMenuWindow
    {
        public DevMenuWindow(MainWindow mainWindow, SwapManager swapManger)
        {
            InitializeComponent();

            _swapManager = swapManger;
            _mainWindow = mainWindow;
        }

        private readonly MainWindow _mainWindow;
        private readonly SwapManager _swapManager;

        private async void ButtonWipeNinjaMark_OnClick(object sender, RoutedEventArgs e)
        {
            _swapManager.EntityA.DatController.ForceRenewMark();
            Close();

            await _mainWindow.RoutineAsync();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonViewLog_OnClick(object sender, RoutedEventArgs e)
        {
            const string log = "log.txt";
            if (File.Exists(log))
            {
                try
                {
                    Process.Start(log);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message);
                }
            }
        }
    }
}
