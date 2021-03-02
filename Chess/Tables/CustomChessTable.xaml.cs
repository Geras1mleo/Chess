using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chess.ChessBackEnd;
using Chess.Tables;

namespace Chess
{
    public partial class CustomChessTable : Window
    {
        private FigureColor playerColor;

        private TableButton[,] buttons;
        private Board board;
        private readonly Server server;

        private TableButton dragButton;
        private TableButton coloredOldBut, coloredNewBut;
        private Image dragImage;

        private Label[] letterLables, numberLables;

        private bool userDragedFigureOutOfTable;

        // Reminds whitch player now can move figures
        public static FigureColor PlayerMove { get; set; } = FigureColor.White;

        private TableColor currentTableColor;
        public TableColor TableColor
        {
            get { return currentTableColor; }
            set
            {
                ImageBrush image = new ImageBrush();
                SolidColorBrush color = new SolidColorBrush();

                switch (value)
                {
                    case TableColor.Green:
                        image.ImageSource = new BitmapImage(new Uri($@"pack://application:,,,/Chess;component/Images/table_green.png", UriKind.RelativeOrAbsolute));
                        ChangeTableColorButton.Background = (SolidColorBrush)Application.Current.Resources["BlueTable"];
                        color = (SolidColorBrush)Application.Current.Resources["GreenTable"];
                        break;
                    case TableColor.Blue:
                        image.ImageSource = new BitmapImage(new Uri($@"pack://application:,,,/Chess;component/Images/table_blue.png", UriKind.RelativeOrAbsolute));
                        ChangeTableColorButton.Background = (SolidColorBrush)Application.Current.Resources["GreenTable"];
                        color = (SolidColorBrush)Application.Current.Resources["BlueTable"];
                        break;
                }
                table.Background = image;
                currentTableColor = value;

                // Setting color to numbers and letters

                letterLables = new Label[] { la, lb, lc, ld, le, lf, lg, lh };
                numberLables = new Label[] { n1, n2, n3, n4, n5, n6, n7, n8 };

                Style style = (Style)FindResource("DefLabelStyle");

                for (int i = 0; i < letterLables.Length; i++)
                {
                    // Setting Style from resources depending on label position => Dark, light, dark, light, dark
                    numberLables[i].Style = style;
                    letterLables[i].Style = style;
                    if(i % 2 != 0)
                    {
                        numberLables[i].Foreground = color;
                        letterLables[i].Foreground = color;
                    }
                }
            }
        }

        public CustomChessTable()
        {
            InitializeComponent();
            InitializeBoard(FigureColor.White, TableColor.Blue);
            server = new Server(DataReceived);
        }
        
        private void InitializeBoard(FigureColor playerColor, TableColor tableColor, Board currBoard = null)
        {
            this.playerColor = playerColor;
            TableColor = tableColor;
            
            buttons = new TableButton[8, 8];
            board = new Board(buttons, currBoard);

            // Creating and setting buttons on right position here
            for (int i = 0, j = table.RowDefinitions.Count - 1; i < table.RowDefinitions.Count; i++, j--)
            {
                for (int k = 0, h = table.ColumnDefinitions.Count - 1; k < table.ColumnDefinitions.Count; k++, h--)
                {
                    var button = new TableButton((short)i, (short)k);

                    button.PreviewMouseLeftButtonDown += DragFigure;
                    button.DragOver += DragOver;
                    button.DragLeave += DragLeave;
                    button.Drop += DropFigure;

                    // We rotate table depending on Figure Color of player / board position
                    if(playerColor == FigureColor.White)
                    {
                        Grid.SetRow(button, j);
                        Grid.SetColumn(button, k);
                    }
                    else
                    {
                        Grid.SetRow(button, i);
                        Grid.SetColumn(button, h);
                    }

                    buttons[i, k] = button;
                    table.Children.Add(button);
                }
            }

            // Setting for each button an icon/figure that was made in Board constructor
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0, k = buttons.GetLength(1) - 1; j < buttons.GetLength(1); j++, k--)
                    buttons[i, j].Image = board.Figures[i, j]?.Image;
            }


            // Setting right letter/number on right position on board
            string[] lettets = {"a", "b", "c", "d", "e", "f", "g", "h"};

            for (int i = 0, j = letterLables.Length - 1; i < letterLables.Length; i++, j--)
            {
                // Setting numbers/letters depending on player color => board rotation
                numberLables[i].Content = playerColor == FigureColor.White ? i+1 : j+1;
                letterLables[i].Content = playerColor == FigureColor.White ? lettets[i] : lettets[j];
            }

            canvas.Children.Remove(dragImage);
            dragImage = new Image()
            {
                IsHitTestVisible = false,
                Width = 56,
                Height = 56
            };
            canvas.Children.Add(dragImage);
        }

        private new void DragLeave(object sender, DragEventArgs e)
        {
            var button = sender as TableButton;
            if(button != dragButton && button != coloredOldBut && button != coloredNewBut)
            {
                button.Background = Brushes.Transparent;
                button.Opacity = 0.95;
            }
        }

        private new void DragOver(object sender, DragEventArgs e)
        {
            var button = sender as TableButton;
            if(button != dragButton && button != coloredOldBut && button != coloredNewBut)
            {
                button.Background = Brushes.Yellow;
                button.Opacity = 0.6;
            }
        }

        private void DragFigure(object sender, MouseEventArgs e)
        {
            var button = sender as TableButton;
            // First we will check if the button has figure and then remember and allow drag
            if (board.Figures[button.PosVertical, button.PosHorizontal] != null)
            {
                button.Background = Brushes.Yellow;
                userDragedFigureOutOfTable = false;
                // Remembering button/position/figure that has been draged
                dragButton = button;

                // Start animation of draging figure
                dragButton.Image = null;
                dragImage.Source = new BitmapImage(new Uri(board.Figures[dragButton.PosVertical, dragButton.PosHorizontal].Image, UriKind.RelativeOrAbsolute));

                // DO NOT DELETE THIS!!! we wont use it later but otherwise drag & drop doesn't work
                var data = new DataObject();
                data.SetData(new object());

                DragDrop.DoDragDrop(button, data, DragDropEffects.Move);
            }
        }

        private void DropFigure(object sender, DragEventArgs e)
        {
            var newbutton = sender as TableButton;

            newbutton.Opacity = 0.95;
            newbutton.Background = Brushes.Transparent;

            if (userDragedFigureOutOfTable)
            {
                newbutton.Background = Brushes.Transparent;
                userDragedFigureOutOfTable = false;
                return;
            }
            else dragButton.Background = Brushes.Transparent;

            // Here we check if user has not dropped figure to the same position
            if (!(dragButton.PosVertical == newbutton.PosVertical && dragButton.PosHorizontal == newbutton.PosHorizontal))
            {
                // Checking if it's valid move
                if(board.IsValidOperation(board.Figures[dragButton.PosVertical, dragButton.PosHorizontal],
                                          new short[] { dragButton.PosVertical, dragButton.PosHorizontal },
                                          new short[] { newbutton.PosVertical, newbutton.PosHorizontal }))
                    DropFigureToNewPosition(dragButton, newbutton);

                else dragButton.Image = board.Figures[dragButton.PosVertical, dragButton.PosHorizontal].Image;
            }
            else
            {
                // Setting image back on button if user dropped figure on same position
                newbutton.Image = board.Figures[newbutton.PosVertical, newbutton.PosHorizontal].Image;
            }

            dragButton = null;
            dragImage.Source = null;
        }

        private void Canvas_DragOver(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(canvas);
            Canvas.SetTop(dragImage, pos.Y - 28);
            Canvas.SetLeft(dragImage, pos.X - 28);

            var tablePos = e.GetPosition(this.table);

            // Adding small margin
            if ((tablePos.X < 10 || tablePos.X + 10 > table.ActualWidth|| tablePos.Y < 10 || tablePos.Y + 10> table.ActualHeight) && dragButton != null)
            {
                dragButton.Image = board.Figures[dragButton.PosVertical, dragButton.PosHorizontal].Image;

                userDragedFigureOutOfTable = true;

                dragButton.Background = Brushes.Transparent;
                
                dragButton = null;
                dragImage.Source = null;
            }
        }

        private void PlayWithFriendButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var newGame = new NewGamePage(server.GetCreateLobbyAction());
            newGame.ShowDialog();
            //RotateBoard();
        }

        private void DropFigureToNewPosition(TableButton dragButton, TableButton newbutton)
        {
            PlayerMove = PlayerMove == FigureColor.White ? FigureColor.Black : FigureColor.White;

            // Copying to the new position
            board.Figures[newbutton.PosVertical, newbutton.PosHorizontal] = board.Figures[dragButton.PosVertical, dragButton.PosHorizontal];
            newbutton.Image = board.Figures[newbutton.PosVertical, newbutton.PosHorizontal].Image;

            // Clearing old position
            board.Figures[dragButton.PosVertical, dragButton.PosHorizontal] = null;
            buttons[dragButton.PosVertical, dragButton.PosHorizontal].Image = null;

            // Coloring last move
            if (coloredOldBut != null)
                coloredOldBut.Background = Brushes.Transparent;
            if (coloredNewBut != null)
                coloredNewBut.Background = Brushes.Transparent;

            dragButton.Background = Brushes.Yellow;
            newbutton.Background = Brushes.Yellow;

            coloredOldBut = dragButton;
            coloredNewBut = newbutton;
        }

        private void ChangeTableColorButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (TableColor)
            {
                case TableColor.Green:
                    TableColor = TableColor.Blue;
                    ChangeTableColorButton.Background = (SolidColorBrush)Application.Current.Resources["GreenTable"];
                    break;
                case TableColor.Blue:
                    TableColor = TableColor.Green;
                    ChangeTableColorButton.Background = (SolidColorBrush)Application.Current.Resources["BlueTable"];
                    break;
            }
        }
        
        private void RotateBoard()
        {
            foreach (var item in buttons)
                table.Children.Remove(item);

            InitializeBoard(playerColor == FigureColor.White ? FigureColor.Black : FigureColor.White, currentTableColor, board);
        }

        private void DataReceived(string move)
        {

        }
    }
}