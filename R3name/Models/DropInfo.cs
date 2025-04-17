namespace R3name.Models;

internal class DropInfo
{
    public static readonly DropInfo NoMoving = new DropInfo(false);

    public bool HasMoved { get; }
    public int Index { get; }

    public DropInfo(int index)
    {
        Index = index;
        HasMoved = true;
    }

    private DropInfo(bool hasMoved)
    {
        HasMoved = hasMoved;
    }
}