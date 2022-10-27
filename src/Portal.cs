using Unit4.CollectionsLib;

class Portal : Item
{
    public Portal(Point position) : base(position)
    {

    }

    public void Teleport(Snake snake) {
      this.gameRenderer.RemoveGameObject(this);

      Stack<Portal> portals = this.gameRenderer.findAllGameObjects<Portal>();


      Portal otherPortal = null;
      while (!portals.IsEmpty()) {
        Portal portal = portals.Pop();
        if (portal == this) continue;
        otherPortal = portal;
      }
      if (otherPortal == null) return;

      this.gameRenderer.RemoveGameObject(otherPortal);
      snake.SetPosition(otherPortal.position);
    }

    public override void Render(double delta, Pixel[,] render)
    {
        render[(int)this.position.GetX(), (int)this.position.GetY()] = new PixelText("🌀");
    }
}
