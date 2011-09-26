using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Helios.WaferMarker {
    public class LineTool : ITool {
        Point _startPoint;
        Canvas _canvas;
        Line _line;

        public LineTool(Canvas canvas) {
            this._canvas = canvas;
        }

        public void DrawStart(Point point) {
            _startPoint = point;
            _line = null;
        }

        public void Draw(Point point) {
            this._canvas.Children.Remove(_line);

            var current = point;

            _line = new Line() { X1 = _startPoint.X, Y1 = _startPoint.Y, X2 = current.X, Y2 = current.Y };
            _line.Stroke = new SolidColorBrush(this.SelectedColor);
            this._canvas.Children.Add(_line);            
        }

        public Color SelectedColor { get; set; }
    }
}
