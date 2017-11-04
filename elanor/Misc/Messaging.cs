using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;

namespace Elanor.Misc
{
    internal class Messaging
    {
        public Messaging(Window parent)
        {
            _parent = parent;
        }

        private readonly Window _parent;
        private MessageWindow _box;

        public void ShowMonolog(MessageType msgType, string text, string caption = "", ButtonType buttons = ButtonType.Ok)
        {
            _parent.Effect = new BlurEffect();

            _box = new MessageWindow
            {
                Owner = Window.GetWindow(_parent),
                ButtonType = buttons,
                MessageType = msgType,
                Text = text,
                Caption = caption
            };

            _box.PrepareWindow();
            _box.ShowDialog();

            _parent.Effect = new DropShadowEffect();
        }

        public void ShowMonolog(MessageType msgType, List<string> texts, string caption = "", ButtonType buttons = ButtonType.Ok)
        {
            _parent.Effect = new BlurEffect();

            _box = new MessageWindow
            {
                Owner = Window.GetWindow(_parent),
                ButtonType = buttons,
                MessageType = msgType,
                Texts = texts,
                Caption = caption
            };
            _box.PrepareWindow();
            _box.ShowDialog();

            _parent.Effect = new DropShadowEffect();
        }

        public ResultType ShowDialog(MessageType msgType, string text, string caption = "", ButtonType buttons = ButtonType.Ok)
        {
            _parent.Effect = new BlurEffect();

            _box = new MessageWindow
            {
                Owner = Window.GetWindow(_parent),
                ButtonType = buttons,
                MessageType = msgType,
                Text = text,
                Caption = caption
            };
            _box.PrepareWindow();
            _box.ShowDialog();

            _parent.Effect = new DropShadowEffect();
            return _box.Result;
        }

        public void ShowSetupDialog(MainWindow mainWindow = null)
        {
            _parent.Effect = new BlurEffect();

            var setupWindow = new SetupWindow(mainWindow) {Owner = Window.GetWindow(_parent)};
            setupWindow.ShowDialog();

            _parent.Effect = new DropShadowEffect();
        }

        public async Task ShowDevDialog(MainWindow mainWindow, SwapManager swapManger)
        {
            _parent.Effect = new BlurEffect();

            var devWindow = new DevMenuWindow(mainWindow, swapManger) { Owner = Window.GetWindow(_parent) };

            if (!swapManger.EntityA.IsInit)
            {
                await swapManger.EntityA.InitAsync();
            }

            devWindow.ShowDialog();

            _parent.Effect = new DropShadowEffect();
        }
        /* Deprecated due to fucking useless
        public async Task<bool> ShowSwapDialogAsync(MainWindow mainWindow, SwapManager swap, bool launch)
        {
            _parent.Effect = new BlurEffect();

            var swapWindow = new SwapWindow(swap, launch) { Owner = Window.GetWindow(_parent) };
            await swapWindow.InitEntitiesAsync();
            swapWindow.ShowDialog();

            _parent.Effect = new DropShadowEffect();

            if (swapWindow.IsB)
            {
                await swapWindow.SwapManager.SwapAsync();
            }

            await mainWindow.RoutineAsync();

            return swapWindow.IsLaunch;
        }
        */
        public void ShowAboutDialog()
        {
            _parent.Effect = new BlurEffect();

            var about = new AboutWindow {Owner = Window.GetWindow(_parent)};
            about.ShowDialog();

            _parent.Effect = new DropShadowEffect();
        }

        public void ShowPatchInfoDialog(Patch.Patch patch)
        {
            _parent.Effect = new BlurEffect();

            PatchInfoWindow pinfo;
            if (patch.IsDownloaded)
            {
                pinfo = new PatchInfoWindow
                {
                    Owner = Window.GetWindow(_parent),
                    TextBlockTitle = {Text = patch.Name},
                    TextBlockDescription = {Text = patch.Description},
                    TextBlockAuthor = {Text = patch.Author},
                    HyperlinkLink = {NavigateUri = new Uri(patch.Link) },
                    TextBlockVersion = {Text = patch.PatchVersion.Full + " от " + patch.Date}
                };                
            }
            else
            {
                pinfo = new PatchInfoWindow
                {
                    Owner = Window.GetWindow(_parent),
                    TextBlockDescription = { Text = "Описание недоступно." },
                    HyperlinkText = { Text = string.Empty }
                };
            }

            pinfo.ShowDialog();

            _parent.Effect = new DropShadowEffect();
        }

        public void UpdateText(string text)
        {
            _box.TextBlockText.Text = text;
        }

        public void UpdateCaption(string caption)
        {
            _box.TextBlockCaption.Text = caption;
        }
    }
}
