using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Helios.WaferMarker {    

    public class FreeHandTool : ITool {
        Point _startPoint;
        Canvas _canvas;

        public FreeHandTool(Canvas canvas) {
            this._canvas = canvas;
        }

        public void DrawStart(Point point) {
            this._startPoint = point;
        }

        public void Draw(Point point) {
            var current = point;

            var line = new Line() { X1 = _startPoint.X, Y1 = _startPoint.Y, X2 = current.X, Y2 = current.Y };
            line.Stroke = new SolidColorBrush(this.SelectedColor);
            this._canvas.Children.Add(line);
            _startPoint = current;
        }

        public Color SelectedColor { get; set; }
    }
}
