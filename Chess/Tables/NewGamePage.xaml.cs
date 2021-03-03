using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chess.Tables
{
    public partial class NewGamePage : Window
    {
        // This event will call fucntion in Server.class and will give server id back
        private event Func<string, string> CreateLobby;

        public NewGamePage(Func<string, string> createLobby)
        {
            InitializeComponent();
            this.CreateLobby = createLobby;
        }

        private void Cancel_MouseDown(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void NewLobby_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Nickname.Text))
            {
                ErrorNick.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            var id = CreateLobby(Nickname.Text);

            if (id.Contains("Error"))
            {
                MessageBox.Show($"Can't connect to server\n{id}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Close();
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(Nickname.Text))
            {
                ErrorNick.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }
            else if (string.IsNullOrEmpty(LobbyID.Text))
            {
                ErrorLobby.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }
        }
    }
}
