using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Helios.WaferMarker {
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Checked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Unchecked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    [TemplatePart(Name = ColorHostName, Type = typeof(Shape))]
    public class ColorRadioButton : RadioButton {
        private const string ColorHostName = "ColorHost";
        private Rectangle _colorHost;

        public ColorRadioButton() {
            this.DefaultStyleKey = typeof(ColorRadioButton);
        }

        public override void OnApplyTemplate() {
            this._colorHost = GetTemplateChild(ColorHostName) as Rectangle;

            base.OnApplyTemplate();
            ApplyColor();
        }

        #region Color property

        public Color Color {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorRadioButton), new PropertyMetadata(ColorPropertyChangedCallback));

        public static void ColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e){
            var self = d as ColorRadioButton;
            self.ApplyColor();
        }

        #endregion

        private void ApplyColor() {
            if (this._colorHost != null) {
                this._colorHost.Fill = new SolidColorBrush((Color)this.Color);
            }
        }
    }
}
