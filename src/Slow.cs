class Slow : Item
{
    public Slow(Point position) : base(position)
    {

    }

    public override void Render(double delta, Pixel[,] render)
    {
        render[(int)this.position.GetX(), (int)this.position.GetY()] = new PixelText("🐌");
    }
}
