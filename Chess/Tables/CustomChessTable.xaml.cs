using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Chess.ChessBackEnd;

namespace Chess
{
    public partial class CustomChessTable : Window
    {
        private TableButton[,] buttons;
        private Board board;

        private FigureColor playerColor = FigureColor.Black;

        public CustomChessTable()
        {
            InitializeComponent();
            InitializeBoard(playerColor);
        }
        
        private void InitializeBoard(FigureColor playerColor)
        {
            buttons = new TableButton[8, 8];
            board = new Board();

            for (int i = 0, j = table.RowDefinitions.Count - 1; i < table.RowDefinitions.Count; i++, j--)
            {
                for (int k = 0, h = table.ColumnDefinitions.Count - 1; k < table.ColumnDefinitions.Count; k++, h--)
                {
                    var button = new TableButton((short)i, (short)k);

                    button.PreviewMouseDown += DragFigure;
                    button.DragOver += DragOver;
                    button.DragLeave += DragLeave;
                    button.Drop += DropFigure;

                    // We rotate table depending on Figure Color of player
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

            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0, k = buttons.GetLength(1) - 1; j < buttons.GetLength(1); j++, k--)
                    buttons[i, j].Image = board.Figures[i, j]?.Image;
                // Setting for each button an icon/figure that was made in Board constructor
            }
        }


        private new void DragLeave(object sender, DragEventArgs e)
        {
            var button = sender as TableButton;
            button.Background = Brushes.Transparent;
            e.Handled = true;
        }

        private new void DragOver(object sender, DragEventArgs e)
        {
            var button = sender as TableButton;
            button.Background = Brushes.Yellow;
            e.Handled = true;
        }

        private void DragFigure(object sender, MouseEventArgs e)
        {
            var button = sender as TableButton;
            if (e.LeftButton == MouseButtonState.Pressed && board.Figures[button.PosVertical, button.PosHorizontal] != null)
            {
                var data = new DataObject();
                // Passing old position of draged figure
                data.SetData(typeof(short[]), new short[] {button.PosVertical , button.PosHorizontal});

                DragDrop.DoDragDrop(button, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
        private void DropFigure(object sender, DragEventArgs e)
        {
            var newbutton = sender as TableButton;
            var data = e.Data;
            //Getting the position of figure before drop
            var fromField = (short[])data.GetData(typeof(short[]));
            short oldvertical = fromField[0];
            short oldhorizontal = fromField[1];
            if(!(oldvertical == newbutton.PosVertical && oldhorizontal == newbutton.PosHorizontal))
            {
                board.Figures[newbutton.PosVertical, newbutton.PosHorizontal] = board.Figures[oldvertical, oldhorizontal];
                newbutton.Image = board.Figures[newbutton.PosVertical, newbutton.PosHorizontal]?.Image;

                board.Figures[oldvertical, oldhorizontal] = null;
                buttons[oldvertical, oldhorizontal].Image = null;
            }
            else newbutton.Image = board.Figures[newbutton.PosVertical, newbutton.PosHorizontal]?.Image;
        }
    }
}
