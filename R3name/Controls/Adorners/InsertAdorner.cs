using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace R3name     .Controls.Adorners;

public class InsertAdorner : Adorner
{
    public InsertAdorner([NotNull] UIElement adornedElement, int index) : base(adornedElement)
    {
        IsHitTestVisible = false;
        _index = index;
        Pen = new Pen(Brushes.Gray, 2);
    }

    public Pen Pen { get; set; }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var itemsControl = AdornedElement as ItemsControl;
        if (itemsControl == null)
            return;

        var itemsCount = itemsControl.Items.Count;
        var index = Math.Min(_index, itemsCount - 1);

        var itemContainer = (UIElement)itemsControl.ItemContainerGenerator.ContainerFromIndex(index);

        var itemRect = new Rect(itemContainer.TranslatePoint(new Point(), AdornedElement), itemContainer.RenderSize);

        if (_index == itemsCount && itemsCount > 0)
        {
            itemRect.Y += itemContainer.RenderSize.Height;
        }

        var itemRectRight = itemRect.Right; //Math.Min(itemRect.Right, viewportWidth);
        var itemRectLeft = itemRect.X < 0 ? 0 : itemRect.X;
        var point1 = new Point(itemRectLeft, itemRect.Y);
        var point2 = new Point(itemRectRight, itemRect.Y);

        drawingContext.DrawLine(Pen, point1, point2);
        DrawTriangle(drawingContext, point1, 0);
        DrawTriangle(drawingContext, point2, 180);
    }

    private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
    {
        drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
        drawingContext.PushTransform(new RotateTransform(rotation));

        drawingContext.DrawGeometry(Pen.Brush, null, _triangle);

        drawingContext.Pop();
        drawingContext.Pop();
    }

    static InsertAdorner()
    {
        const int triangleSize = 5;

        var firstLine = new LineSegment(new Point(0, -triangleSize), false);
        firstLine.Freeze();
        var secondLine = new LineSegment(new Point(0, triangleSize), false);
        secondLine.Freeze();

        var figure = new PathFigure { StartPoint = new Point(triangleSize, 0) };
        figure.Segments.Add(firstLine);
        figure.Segments.Add(secondLine);
        figure.Freeze();

        _triangle = new PathGeometry();
        _triangle.Figures.Add(figure);
        _triangle.Freeze();
    }

    private static readonly PathGeometry _triangle;
    [NotNull]
    private readonly int _index;
}