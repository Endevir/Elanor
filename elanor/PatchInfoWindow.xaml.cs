using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Navigation;
using shared;

namespace Elanor
{
    /// <summary>
    /// Interaction logic for PatchInfoWindow.xaml
    /// </summary>
    public partial class PatchInfoWindow
    {
        public PatchInfoWindow()
        {
            InitializeComponent();
        }

        private void PatchInfoWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.ToString());
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }            
        }
    }
}
