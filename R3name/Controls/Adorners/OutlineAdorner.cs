using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace R3name.Controls.Adorners;

public class OutlineAdorner : Adorner
{
    private readonly Pen _renderPen;

    public OutlineAdorner(UIElement adornedElement) : base(adornedElement)
    {
        _renderPen = new Pen(new SolidColorBrush(Color.FromRgb(23, 86, 59)), 2);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var adornedElementRect = new Rect(AdornedElement.RenderSize);
        drawingContext.DrawRoundedRectangle(null, _renderPen, adornedElementRect, 5, 5);
    }
}