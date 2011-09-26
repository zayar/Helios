using System.Windows;
using System.Windows.Media;

namespace Helios.WaferMarker {

    public interface ITool {
        void DrawStart(Point point);
        void Draw(Point point);        

        Color SelectedColor { get; set; }
    }
}
