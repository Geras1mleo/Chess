using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chess.ChessBackEnd;

namespace Chess
{
    public partial class CustomChessTable : Window
    {
        private TableButton[,] buttons;
        private Label[] letterLables, numberLables;
        private Board board;

        private TableButton dragButton;
        private Image dragImage;

        private FigureColor playerColor = FigureColor.White;
        private TableColor tableColor = TableColor.Blue;

        public CustomChessTable()
        {
            InitializeComponent();
            InitializeBoard(playerColor, tableColor);
        }
        
        private void InitializeBoard(FigureColor playerColor, TableColor tableColor)
        {
            buttons = new TableButton[8, 8];
            board = new Board();

            // Creating and setting buttons on right position here
            for (int i = 0, j = table.RowDefinitions.Count - 1; i < table.RowDefinitions.Count; i++, j--)
            {
                for (int k = 0, h = table.ColumnDefinitions.Count - 1; k < table.ColumnDefinitions.Count; k++, h--)
                {
                    var button = new TableButton((short)i, (short)k);

                    button.PreviewMouseDown += DragFigure;
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

            
            ImageBrush image = new ImageBrush();
            string darkLabelStyleResource;
            string lightLabelStyleResource = "DefLabelStyleStyle";

            if(tableColor == TableColor.Blue)
            {
                image.ImageSource = new BitmapImage(new Uri($@"pack://application:,,,/Chess;component/Images/table_blue.png", UriKind.RelativeOrAbsolute));
                darkLabelStyleResource = "BlueLabelStyle";
            }
            else
            {
                image.ImageSource = new BitmapImage(new Uri($@"pack://application:,,,/Chess;component/Images/table_green.png", UriKind.RelativeOrAbsolute));
                darkLabelStyleResource = "GreenLabelStyle";
            }
            table.Background = image;

            // Setting right letter/number on right position on board
            letterLables = new Label[] { la, lb, lc, ld, le, lf, lg, lh };
            numberLables = new Label[] { n1, n2, n3, n4, n5, n6, n7, n8 };

            string[] lettets = {"a", "b", "c", "d", "e", "f", "g", "h"};

            for (int i = 0, j = letterLables.Length - 1; i < letterLables.Length; i++, j--)
            {
                // Setting numbers/letters depending on player color => board rotation
                numberLables[i].Content = playerColor == FigureColor.White ? i+1 : j+1;
                letterLables[i].Content = playerColor == FigureColor.White ? lettets[i] : lettets[j];

                // Setting Style from resources depending on label position => Dark, light, dark, light, dark
                numberLables[i].Style = (Style)FindResource(i % 2 == 0 ? lightLabelStyleResource : darkLabelStyleResource);
                letterLables[i].Style = (Style)FindResource(i % 2 == 0 ? lightLabelStyleResource : darkLabelStyleResource);
            }

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
            button.Background = Brushes.Transparent;
            button.Opacity = 0.95;
            e.Handled = true;
        }

        private new void DragOver(object sender, DragEventArgs e)
        {
            var button = sender as TableButton;
            button.Background = Brushes.Yellow;
            button.Opacity = 0.6;
            e.Handled = true;
        }

        private void DragFigure(object sender, MouseEventArgs e)
        {
            // First we will check if the button has figure and then remember and allow drag
            var button = sender as TableButton;
            if (e.LeftButton == MouseButtonState.Pressed && board.Figures[button.PosVertical, button.PosHorizontal] != null)
            {
                // Remembering button/position/figure that has been draged
                dragButton = button;

                // Animation of draging figure
                button.Image = null;
                dragImage.Source = new BitmapImage(new Uri(board.Figures[dragButton.PosVertical, dragButton.PosHorizontal].Image, UriKind.Relative));

                DragAsync();

                // DO NOT DELETE THIS!!! we wont use it later but otherwise drag & drop doesn't work
                var data = new DataObject();
                data.SetData(new object());

                DragDrop.DoDragDrop(button, data, DragDropEffects.Move);
            }
        }
        int i = 0;
        /// <summary>
        /// Doesnt work yet
        /// </summary>
        private async void DragAsync()
        {
            await Task.Run(() =>
            {
                i = 0;
                while (dragButton != null)
                {
                    Thread.Sleep(10);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        //dragImage.SetValue(Canvas.TopProperty, Mouse.GetPosition(Application.Current.MainWindow).Y - 28);
                        //dragImage.SetValue(Canvas.LeftProperty, Mouse.GetPosition(Application.Current.MainWindow).X - 28);
                        Canvas.SetTop(dragImage, i);
                        Canvas.SetLeft(dragImage, i);
                    }));
                    i++;
                }
            });
        }

        private void DropFigure(object sender, DragEventArgs e)
        {
            var newbutton = sender as TableButton;
            newbutton.Opacity = 0.95;

            if(dragButton != null && !(dragButton.PosVertical == newbutton.PosVertical && dragButton.PosHorizontal == newbutton.PosHorizontal))
            {
                // Copying to the new position
                board.Figures[newbutton.PosVertical, newbutton.PosHorizontal] = board.Figures[dragButton.PosVertical, dragButton.PosHorizontal];
                newbutton.Image = board.Figures[newbutton.PosVertical, newbutton.PosHorizontal]?.Image;

                // Clearing old position
                board.Figures[dragButton.PosVertical, dragButton.PosHorizontal] = null;
                buttons[dragButton.PosVertical, dragButton.PosHorizontal].Image = null;
            }
            else newbutton.Image = board.Figures[newbutton.PosVertical, newbutton.PosHorizontal]?.Image;
            dragButton = null;
            dragImage.Source = null;
        }
    }
}