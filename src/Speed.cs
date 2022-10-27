class Speed : Item
{
    public Speed(Point position) : base(position)
    {

    }

    public override void Render(double delta, Pixel[,] render)
    {
        render[(int)this.position.GetX(), (int)this.position.GetY()] = new PixelText("⚡");
    }
}
