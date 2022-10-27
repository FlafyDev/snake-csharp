using System;

class Point
{
    private double x;
    private double y;

    public Point(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public double GetX()
    {
        return this.x;
    }

    public double GetY()
    {
        return this.y;
    }

    public void SetX(double x)
    {
        this.x = x;
    }

    public void SetY(double y)
    {
        this.y = y;
    }

    public Point Copy()
    {
        return new Point(this.x, this.y);
    }

    public bool EqualToFloored(Point point)
    {
        return (int)this.y == (int)point.GetY() &&
            (int)this.x == (int)point.GetX();
    }

    public double Distance(Point point)
    {
        return Math.Sqrt(Math.Pow((point.GetX() - this.x), 2) + Math.Pow((point.GetY() - this.y), 2));
    }
}

abstract class GameObject
{
    protected Point position;
    protected GameRenderer gameRenderer;

    public GameObject(Point position)
    {
        this.position = position;
    }

    public virtual void Input(ConsoleKey key) { }

    public virtual void Render(double delta, Pixel[,] render) { }

    public virtual void OnDestroy() { }
    public virtual void OnInit() { }

    public Point GetPosition()
    {
        return this.position;
    }

    public void SetPosition(Point position)
    {
        this.position = position;
    }

    public void SetGameRenderer(GameRenderer gameRenderer)
    {
        this.gameRenderer = gameRenderer;
    }
}
