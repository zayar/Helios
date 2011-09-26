using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ImageTools;
using ImageTools.IO.Png;

namespace Helios.WaferMarker {
    public partial class MainPage : UserControl {
        private bool _drawing;
        private ITool _tool;

        public MainPage() {
            InitializeComponent();

            this._drawing = false;
            this._tool = new FreeHandTool(this.drawingCanvas);
        }

        private void ColorRadioButton_Checked(object sender, RoutedEventArgs e) {
            var crb = colorStackPanel.Children.FirstOrDefault(ui => ui is ColorRadioButton && (((ColorRadioButton)ui).IsChecked ?? false)) as ColorRadioButton;
            _tool.SelectedColor = crb != null ? crb.Color : Colors.White;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var current = e.GetPosition(this.drawingCanvas);
            if (IsPointInEllipse(current)) {
                _drawing = true;
                _tool.DrawStart(current);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            _drawing = false;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e) {
            if (_drawing) {

                var current = e.GetPosition(this.drawingCanvas);

                if (IsPointInEllipse(current)) {
                    _tool.Draw(current);
                }
            }
        }

        private bool IsPointInEllipse(Point point) {
            var asquare = this.boundaryEllipse.RadiusX * this.boundaryEllipse.RadiusX;
            var bsquare = this.boundaryEllipse.RadiusY * this.boundaryEllipse.RadiusY;
            var center = this.boundaryEllipse.Center;

            return ((point.X - center.X) * (point.X - center.X) * bsquare + (point.Y - center.Y) * (point.Y - center.Y) * asquare) < (asquare * bsquare);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var lines = this.drawingCanvas.Children.Where(ui => ui is Line).ToArray();

            foreach (var line in lines) {
                this.drawingCanvas.Children.Remove(line);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            if (_tool != null) {
                var previousColor = _tool.SelectedColor;

                var crb = toolStackPanel.Children.FirstOrDefault(ui => ui is ToolRadioButton && (((RadioButton)ui).IsChecked ?? false)) as ToolRadioButton;
                _tool = crb.ToolType == ToolType.Line ? (ITool)new LineTool(this.drawingCanvas) : new FreeHandTool(this.drawingCanvas);
                _tool.SelectedColor = previousColor;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            var image = this.drawingCanvas.ToImage();
            UploadImage(image);
        }

        private void UploadImage(ExtendedImage image) {
            var webClient = new WebClient();
            webClient.OpenWriteCompleted += (sender, e) => {
                if (e.Error == null) {
                    var pngEncoder = new PngEncoder();
                    using (e.Result) {
                        pngEncoder.Encode(image, e.Result);
                    }
                }
            };

            webClient.WriteStreamClosed += (sender, e) => {
                if (e.Error == null) {
                    MessageBox.Show("Image uploaded successfully");
                }
            };

            webClient.OpenWriteAsync(new Uri("http://localhost:5637/Wafers/UploadImage", UriKind.Absolute), "POST");
        }
    }
}
