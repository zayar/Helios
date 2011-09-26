using System.Windows;
using System.Windows.Controls;

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
    public class ToolRadioButton : RadioButton {
        public ToolRadioButton() {
            this.DefaultStyleKey = typeof(ToolRadioButton);
        }

        #region ToolType property
        public ToolType ToolType {
            get { return (ToolType)GetValue(ToolTypeProperty); }
            set { SetValue(ToolTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTypeProperty =
            DependencyProperty.Register("ToolType", typeof(ToolType), typeof(ToolRadioButton), new PropertyMetadata(null));
        
        #endregion
    }
}
