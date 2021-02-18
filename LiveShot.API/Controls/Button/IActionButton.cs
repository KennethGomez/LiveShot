using System.Windows.Media;
using LiveShot.API.Drawing;

namespace LiveShot.API.Controls.Button
{
    public interface IActionButton
    {
        bool IsActive { get; set; }
        Brush IconFill { get; set; }
        CanvasTool ActiveTool { get; set; }
        void UpdateIconFill(Brush brush);
    }
}