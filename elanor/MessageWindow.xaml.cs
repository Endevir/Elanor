using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Elanor.Misc;

namespace Elanor
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow
    {
        public MessageWindow()
        {
            InitializeComponent();
        }

        public ButtonType ButtonType;

        public MessageType MessageType;

        public ResultType Result { get; private set; }

        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _isSingle = true;
            }
        }

        private List<string> _texts;
        public List<string> Texts
        {
            get { return _texts; }
            set
            {
                _texts = value;
                _isSingle = false;
                _currentPos = 0;
            }
        }

        public string Caption;

        private bool _isSingle;
        private int _currentPos;
        private Button _edgeButton;
        private bool _clickClose;

        public void PrepareWindow()
        {
            // select respective image
            string resource;
            switch (MessageType)
            {
                case MessageType.Blue:
                    resource = "Resources/goodWindow.png";
                    break;
                case MessageType.Red:
                    resource = "Resources/errorWindow.png";
                    break;
                case MessageType.White:
                    resource = "Resources/someWindow.png";
                    break;
                default:
                    resource = "Resources/aboutWindow.png";
                    break;
            }

            Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), resource)));

            TextBlockCaption.Text = Caption;
            TextBlockText.Text = _isSingle ? _text : _texts[0];

            InitButtons();
        }

        private void MessageWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // last list element reached on previous click or it was the only text which should be appeared
            if (_texts == null || _currentPos >= _texts.Count - 1)
            {
                Close();
                return;
            }

            ++_currentPos;
            TextBlockText.Text = _texts[_currentPos];
        }

        private void InitButtons()
        {
            switch (ButtonType)
            {
                case ButtonType.ArrowRight:
                    _edgeButton = new Button
                    {
                        Content = "",
                        Background =
                            new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this),
                                "Resources/clickHere.png"))),
                        Style = Application.Current.Resources["MessageButton"] as Style,
                        Width = 35,
                        Height = 35,
                        Margin = new Thickness(0, 100, 0, 0),
                        Effect = new BlurEffect { Radius = 2 }
                    };
                    _edgeButton.Click += ButtonNext_OnClick;

                    GridMain.Children.Add(_edgeButton);
                    Grid.SetColumn(_edgeButton, 3);
                    Grid.SetRow(_edgeButton, 3);
                    Grid.SetRowSpan(_edgeButton, 2);
                    break;
                case ButtonType.DoneExit:
                    var exitButton = new Button
                    {
                        Content = "Выйти",
                        /*Background =
                            new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this),
                                "Resources/arrowRight.png"))),*/
                        Style = Application.Current.Resources["MessageButton"] as Style,
                        Width = 50,
                        Height = 20
                    };
                    exitButton.Click += ButtonExit_OnClick;

                    var doneButton = new Button
                    {
                        Content = "Готово",
                        /*Background =
                            new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this),
                                "Resources/arrowRight.png"))),*/
                        Style = Application.Current.Resources["MessageButton"] as Style,
                        Width = 50,
                        Height = 20
                    };
                    doneButton.Click += ButtonDone_OnClick;

                    GridMain.Children.Add(exitButton);
                    GridMain.Children.Add(doneButton);

                    Grid.SetColumn(exitButton, 3);
                    Grid.SetRow(exitButton, 4);

                    Grid.SetColumn(doneButton, 2);
                    Grid.SetRow(doneButton, 4);
                    break;
                case ButtonType.YesNo:
                    var yesButton = new Button
                    {
                        Content = "Да",
                        /*Background =
                            new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this),
                                "Resources/arrowRight.png"))),*/
                        Style = Application.Current.Resources["MessageButton"] as Style,
                        Width = 50,
                        Height = 20
                    };
                    yesButton.Click += ButtonYes_OnClick;

                    var noButton = new Button
                    {
                        Content = "Нет",
                        /*Background =
                            new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this),
                                "Resources/arrowRight.png"))),*/
                        Style = Application.Current.Resources["MessageButton"] as Style,
                        Width = 50,
                        Height = 20
                    };
                    noButton.Click += ButtonNo_OnClick;

                    GridMain.Children.Add(yesButton);
                    GridMain.Children.Add(noButton);

                    Grid.SetColumn(yesButton, 3);
                    Grid.SetRow(yesButton, 4);

                    Grid.SetColumn(noButton, 2);
                    Grid.SetRow(noButton, 4);
                    break;
            }
        }

        private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
        {
            // last list element reached on previous click, 'cross' applied
            if (_currentPos == _texts.Count - 1)
            {
                Close();
                return;
            }

            ++_currentPos;
            TextBlockText.Text = _texts[_currentPos];

            if (_currentPos == _texts.Count - 1)
            {
                // last list text, replace 'arrow' with 'cross'
                /*_edgeButton.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this),
                    "Resources/close.png")));*/
                GridMain.Children.Remove(_edgeButton);
                _edgeButton = null;
                _clickClose = true;
            }
        }

        private void ButtonDone_OnClick(object sender, RoutedEventArgs e)
        {
            Result = ResultType.Done;
            Close();
        }

        private void ButtonExit_OnClick(object sender, RoutedEventArgs e)
        {
            Result = ResultType.Exit;
            Close();
        }

        private void ButtonYes_OnClick(object sender, RoutedEventArgs e)
        {
            Result = ResultType.Yes;
            Close();
        }

        private void ButtonNo_OnClick(object sender, RoutedEventArgs e)
        {
            Result = ResultType.No;
            Close();
        }
    }
}
