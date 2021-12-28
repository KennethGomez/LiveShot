using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveShot.API.Properties;

namespace LiveShot.API.Canvas
{
    public record Selection
    {
        private readonly Rectangle _rectangle;
        private double _left;
        private double _top;

        private Selection(double left, double top, double width, double height)
        {
            _rectangle = new Rectangle
            {
                Fill = Brushes.Transparent,
                Width = width,
                Height = height
            };

            _left = left;
            _top = top;
        }

        public double Left
        {
            get => _left;
            set
            {
                if (value >= 0) _left = value;
            }
        }

        public double Top
        {
            get => _top;
            set
            {
                if (value >= 0) _top = value;
            }
        }

        public double Width
        {
            get => _rectangle.Width;
            set
            {
                if (value >= 0) _rectangle.Width = value;
            }
        }

        public double Height
        {
            get => _rectangle.Height;
            set
            {
                if (value >= 0) _rectangle.Height = value;
            }
        }

        public (double, double, double, double) Transform => (_left, _top, _rectangle.Width, _rectangle.Height);

        public Rectangle Rectangle => _rectangle;

        public static Selection Empty => new(0, 0, 0, 0);

        private bool IsClear => Width == 0 && Height == 0 && Top == 0 && Left == 0;
        private bool HasInvalidSize => Width == 0 || Height == 0;
        public bool Invalid => HasInvalidSize || IsClear;

        public string Label => IsClear
            ? Resources.CaptureScreen_SizeLabel_Empty
            : $"{(int)Width} × {(int)Height}";

        public bool Contains(Point point)
        {
            return point.X >= _left &&
                   point.X <= _left + _rectangle.Width &&
                   point.Y >= _top &&
                   point.Y <= _top + _rectangle.Height;
        }

        public void Clear()
        {
            _top = 0;
            _left = 0;
            Width = 0;
            Height = 0;
        }
    }
}