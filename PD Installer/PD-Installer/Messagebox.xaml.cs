using System.Windows;

namespace PD_Installer
{
    public enum ButtonType
    {
        YesNo,
        Ok
    }

    public partial class MessageboxNew : Window
    {
        public MessageboxNew()
        {
            InitializeComponent();
            //second time show error solved
            if (Application.Current == null) new Application();
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        private void yes_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.MouseDown += delegate { DragMove(); };
        }
    }
    public class MessageBox
    {
        public static bool? Show(string title, string text, ButtonType Button = ButtonType.YesNo)
        {
            MessageboxNew msg = new MessageboxNew
            {
                okBtn = {Visibility = (Button == ButtonType.Ok) ? Visibility.Visible : Visibility.Hidden },
                noBtn = { Visibility = (Button == ButtonType.YesNo) ? Visibility.Visible : Visibility.Hidden },
                yesBtn = { Visibility = (Button == ButtonType.YesNo) ? Visibility.Visible : Visibility.Hidden },
                TitleBar = { Text = title },
                Textbar = { Text = text }
            };

            msg.noBtn.Focus();
            return msg.ShowDialog();
        }
    }
}