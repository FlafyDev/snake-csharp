using System;
using static Utils;

class Apple : Item
{
    bool interactable;
    Point anchorPosition;
    double step;
    double acc;

    public Apple(Point position) : base(position)
    {
        this.interactable = true;
        this.anchorPosition = position.Copy();
        this.step = 0;
        this.acc = 0;
    }

    public bool GetInteractable()
    {
        return this.interactable;
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    public override void Render(double delta, Pixel[,] render)
    {
        if (!this.interactable)
        {
            this.step = (step + delta / 2) % 1;
            this.acc += delta * 10;

            this.position.SetX(this.anchorPosition.GetX() + Math.Sin(this.step * 2 * Math.PI) * acc);
            this.position.SetY(this.anchorPosition.GetY() + Math.Cos(this.step * 2 * Math.PI) * acc);

            if (this.position.GetX() < 0 || this.position.GetX() > this.gameRenderer.GetWidth() ||
                  this.position.GetY() < 0 || this.position.GetY() > this.gameRenderer.GetHeight())
            {
                if (acc > 20)
                    this.gameRenderer.RemoveGameObject(this);
                return;
            }
        }
        render[(int)this.position.GetX(), (int)this.position.GetY()] = new PixelText("⭐", ConsoleColor.Yellow);
    }
}
