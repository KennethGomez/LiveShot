using System.Drawing;

namespace LiveShot.UI.Objects
{
    public record Selection
    {
        private int _height;
        private int _left;
        private int _top;
        private int _width;

        private Selection(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public int Left
        {
            get => _left;
            set
            {
                if (value > 0) _left = value;
            }
        }

        public int Top
        {
            get => _top;
            set
            {
                if (value > 0) _top = value;
            }
        }

        public int Width
        {
            get => _width;
            set
            {
                if (value > 0) _width = value;
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                if (value > 0) _height = value;
            }
        }

        public Rectangle Rectangle => new Rectangle(Left, Top, Width, Height);

        public static Selection Empty => new Selection(0, 0, 0, 0);
    }
}