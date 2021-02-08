using System.Windows;

namespace LiveShot.UI.Controls.Button
{
    public class ActionButton : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(ActionButton), new PropertyMetadata(false)
        );

        public bool IsActive
        {
            get => (bool) GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        protected override void OnClick()
        {
            IsActive = !IsActive;
        }
    }
}