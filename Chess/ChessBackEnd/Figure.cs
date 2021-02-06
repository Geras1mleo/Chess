using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess.ChessBackEnd
{
    public class Figure
    {
        public FigureColor Color { get; }
        public FigureType Type { get; }
        public Figure(FigureColor color, FigureType type)
        {
            Color = color;
            Type = type;
        }

        /// <summary>
        /// Returns an icon of this figure
        /// </summary>
        public string Image
        {
            get
            {
                string colorLetter = "";
                string typeLetter = "";

                switch (Color)
                {
                    case FigureColor.White:
                        colorLetter = "w";
                        break;
                    case FigureColor.Black:
                        colorLetter = "b";
                        break;
                }
                switch (Type)
                {
                    case FigureType.Pawn:
                        typeLetter = "p";
                        break;
                    case FigureType.Rook:
                        typeLetter = "r";
                        break;
                    case FigureType.Knight:
                        typeLetter = "n";
                        break;
                    case FigureType.Bishop:
                        typeLetter = "b";
                        break;
                    case FigureType.Queen:
                        typeLetter = "q";
                        break;
                    case FigureType.King:
                        typeLetter = "k";
                        break;
                }

                
                return $@"/Chess;component/Images/{colorLetter}{typeLetter}.png";
            }
        }
    }

    public class TableButton : Button
    {
        public short PosVertical { get;}
        public short PosHorizontal { get;}
        
        /// <summary>
        /// Sets an icon to a button
        /// </summary>
        public string Image
        {
            set
            {
                if (value != null)
                {
                    var image = new Image()
                    {
                        Width = 56,
                        Height = 56,
                        Source = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)),
                        VerticalAlignment = VerticalAlignment.Center,
                        Stretch = Stretch.Uniform
                    };
                    Content = image;
                }
                else Content = null;
            }
        }

        public TableButton(short posVertical, short posHorizontal)
        {
            PosVertical = posVertical;
            PosHorizontal = posHorizontal;
            Style = (Style)FindResource("TableButtonStyle");
        }
    }
}
