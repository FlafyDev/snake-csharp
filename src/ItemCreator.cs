using System;
using Unit4.CollectionsLib;

abstract class Item : GameObject
{
    public Item(Point position) : base(position)
    {
    }
}

class ItemCreator : GameObject
{
    private Random random;
    private double createTimeout; // Seconds
    private Queue<GameObject> createdItemsQueue;
    private double createdItems;

    public ItemCreator() : base(new Point(0, 0))
    {
        this.random = new Random();
        this.createTimeout = 1;
        this.createdItemsQueue = new Queue<GameObject>();
        this.createdItems = 0;
    }

    public override void Input(ConsoleKey key)
    {
    }

    private bool isValidPoint(Point point)
    {
        Stack<Snake> snakes = gameRenderer.findAllGameObjects<Snake>();
        while (!snakes.IsEmpty())
        {
            Snake snake = snakes.Pop();
            if (snake.GetPosition().Distance(point) < 10) return true;
            if (snake.CheckCollision(point))
            {
                return true;
            }
        }
        return false;
    }

    public Point GenerateValidPoint()
    {
        Point randomPoint;
        do
        {
            randomPoint = new Point(this.random.Next(0, this.gameRenderer.GetWidth()), this.random.Next(0, this.gameRenderer.GetHeight() - 2));
        } while (isValidPoint(randomPoint));
        return randomPoint;
    }

    public override void Render(double delta, Pixel[,] render)
    {
        this.createTimeout -= delta;
        if (createTimeout < 0)
        {
            createTimeout = this.random.NextDouble() * 2;

            Item item;
            double rand = random.NextDouble();

            if (rand > 0.4)
            {
                item = new Apple(GenerateValidPoint());
            }
            else if (rand > 0.3)
            {
                item = new Slow(GenerateValidPoint());
            }
            else if (rand > 0.2)
            {
                item = new Portal(GenerateValidPoint());
                gameRenderer.AddGameObject(new Portal(GenerateValidPoint()));
            }
            else if (rand > 0.1)
            {
                item = new Bomb(GenerateValidPoint());
            }
            else
            {
                item = new Speed(GenerateValidPoint());
            }
            gameRenderer.AddGameObject(item);
            createdItemsQueue.Insert(item);
            if (createdItems < 15)
            {
                createdItems++;
            }
            else
            {
                gameRenderer.RemoveGameObject(createdItemsQueue.Remove());
            }
        }
    }
}
