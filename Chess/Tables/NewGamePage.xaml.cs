using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Chess.Tables
{
    public partial class NewGamePage : Window
    {
        // This event will call fucntion in Server.cs that will make request for new lobby on server
        private event Action<string> CreateLobby;

        // This event will call fucntion in Server.cs that will make request to join the lobby
        private event Action<string, string> ConnectToLobby;

        public NewGamePage(Action<string> createLobby, Action<string, string> connectToLobby)
        {
            InitializeComponent();
            CreateLobby = createLobby;
            ConnectToLobby = connectToLobby;
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

            CreateLobby(Nickname.Text);
            Close();
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

            ConnectToLobby(LobbyID.Text, Nickname.Text);
            Close();
        }
    }
}
