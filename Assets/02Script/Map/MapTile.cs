
public class MapTile
{
    public int X { get; set; }
    public int Y { get; set; }
    public int F { get { return G + H; } }
    public int G { get; private set; } // Start ~ Current
    public int H { get; private set; } // Current ~ End
    public MapTile Parent { get; private set; }
    public void Execute(MapTile parent, MapTile endTile)
    {
        Parent = parent;
        G = CalcGValue(parent, this);
        int diffX = System.Math.Abs(endTile.X - X);
        int diffY = System.Math.Abs(endTile.Y - Y);
        H = (diffX + diffY) * 10;
    }
    public static int CalcGValue(MapTile parent, MapTile current)
    {
        int diffX = System.Math.Abs(parent.X - current.X);
        int diffY = System.Math.Abs(parent.Y - current.Y);
        int value = 10;
        if (diffX == 1 && diffY == 1)
        {
            value = 14;
        }
        return parent.G + value;
    }
}
