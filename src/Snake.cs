using System;
using Unit4.CollectionsLib;
using static Utils;

enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
}

class Snake : GameObject
{
    private int tailLength;
    private Direction direction;
    private Point[] tail;
    private double speed = 10; // Moves 10 pixels per second, regardless of fps.
    private bool inputLock = false; // Lock the input between when pressing an arrow key to the snake moving a pixel.  
    private GameManager gameManager;
    private bool useWASD;
    private double tailAnimation;

    public Snake(Point position, bool useWASD = false) : base(position)
    {
        this.tail = new Point[300];
        this.direction = Direction.RIGHT;
        this.useWASD = useWASD;
    }

    public override void OnInit()
    {
        this.gameManager = this.gameRenderer.findAllGameObjects<GameManager>().Pop();
    }

    public override void Input(ConsoleKey key)
    {
        ConsoleKey left = useWASD ? ConsoleKey.A : ConsoleKey.LeftArrow;
        ConsoleKey right = useWASD ? ConsoleKey.D : ConsoleKey.RightArrow;
        ConsoleKey up = useWASD ? ConsoleKey.W : ConsoleKey.UpArrow;
        ConsoleKey down = useWASD ? ConsoleKey.S : ConsoleKey.DownArrow;
        if (key == left)
        {
            if (!inputLock && this.direction != Direction.RIGHT)
            {
                this.direction = Direction.LEFT;
                inputLock = true;
            }
        }
        else if (key == right)
        {

            if (!inputLock && this.direction != Direction.LEFT)
            {
                this.direction = Direction.RIGHT;
                inputLock = true;
            }
        }
        else if (key == up)
        {

            if (!inputLock && this.direction != Direction.DOWN)
            {
                this.direction = Direction.UP;
                inputLock = true;
            }
        }
        else if (key == down)
        {

            if (!inputLock && this.direction != Direction.UP)
            {
                this.direction = Direction.DOWN;
                inputLock = true;
            }
        }
        /* if (key == ConsoleKey.Spacebar) */
        /* { */
        /*     this.LengthenTail(); */
        /* } */
    }

    public bool CheckCollision(Point position)
    {
        if (this.position.EqualToFloored(position)) return true;
        for (int i = 0; i < this.tailLength; i++)
        {
            if (this.tail[i].EqualToFloored(position))
            {
                return true;
            }
        }
        return false;
    }

    public void LengthenTail()
    {
        Point pos;
        if (this.tailLength == 0)
        {
            pos = this.position.Copy();
        }
        else
        {
            pos = this.tail[this.tailLength - 1].Copy();
        }
        this.tail[this.tailLength] = pos;
        this.tailLength++;
    }

    public override void Render(double delta, Pixel[,] render)
    {
        double distance = Math.Min(delta * speed, 1);

        Point prevPosition = this.position.Copy();
        switch (direction)
        {
            case Direction.LEFT:
                this.position.SetX(this.position.GetX() - distance);
                break;
            case Direction.RIGHT:
                this.position.SetX(this.position.GetX() + distance);
                break;
            case Direction.UP:
                this.position.SetY(this.position.GetY() - distance);
                break;
            case Direction.DOWN:
                this.position.SetY(this.position.GetY() + distance);
                break;
        }
        this.position.SetX(mod(this.position.GetX(), this.gameRenderer.GetWidth()));
        this.position.SetY(mod(this.position.GetY(), this.gameRenderer.GetHeight()));
        this.tailAnimation += delta * 40;

        // Check if position changed(as in the pixels) and update tail.
        if ((int)prevPosition.GetX() != (int)this.position.GetX() ||
            (int)prevPosition.GetY() != (int)this.position.GetY())
        {
            for (int i = this.tailLength - 1; i > 0; i--)
            {
                this.tail[i] = this.tail[i - 1];
            }
            this.tail[0] = prevPosition;
            inputLock = false;
            var items = gameRenderer.findAllGameObjects<Item>();
            while (!items.IsEmpty())
            {
                Item item = items.Pop();
                if (!item.GetPosition().EqualToFloored(this.position)) continue;

                if (item is Apple)
                {
                    Apple apple = item as Apple;
                    if (apple.GetInteractable())
                    {
                        this.tailAnimation = 0;
                        this.LengthenTail();
                        this.gameManager.SetScore(gameManager.GetScore() + 1);
                        apple.SetInteractable(false);
                    }
                }
                else if (item is Bomb)
                {
                    this.gameManager.EndGame();
                }
                else if (item is Speed)
                {
                    this.speed += 5;
                    gameRenderer.RemoveGameObject(item);
                }
                else if (item is Slow)
                {
                    this.speed -= 5;
                    this.speed = Math.Max(this.speed, 1);
                    gameRenderer.RemoveGameObject(item);
                }
                else if (item is Portal)
                {
                    (item as Portal).Teleport(this);
                }
            }

            // Check if other snakes collide with this
            Stack<Snake> snakes = gameRenderer.findAllGameObjects<Snake>();
            while (!snakes.IsEmpty())
            {
                Snake snake = snakes.Pop();
                if (snake == this) continue;
                if (this.CheckCollision(snake.GetPosition()))
                {
                    this.gameManager.EndGame();
                }
            }

            // Check if this collides with the tail
            for (int i = 1; i < this.tailLength; i++)
            {
                if (this.position.EqualToFloored(this.tail[i]))
                {
                    this.gameManager.EndGame();
                }
            }
        }

        for (int i = 0; i < this.tailLength; i++)
        {
            // Tail animation when eating an apple
            if (this.tailLength > 3 && tailAnimation < 10 && i < 10 && (int)tailAnimation == i)
            {
                if ((int)this.tail[i].GetX() + 1 < this.gameRenderer.GetWidth())
                {
                    render[(int)this.tail[i].GetX() + 1, (int)this.tail[i].GetY()] = new PixelFilled();
                }
                if ((int)this.tail[i].GetX() - 1 > 0)
                {
                    render[(int)this.tail[i].GetX() - 1, (int)this.tail[i].GetY()] = new PixelFilled();
                }
                if ((int)this.tail[i].GetY() - 1 > 0)
                {
                    render[(int)this.tail[i].GetX(), (int)this.tail[i].GetY() - 1] = new PixelFilled();
                }
                if ((int)this.tail[i].GetY() + 1 < this.gameRenderer.GetHeight())
                {
                    render[(int)this.tail[i].GetX(), (int)this.tail[i].GetY() + 1] = new PixelFilled();
                }
            }

            render[(int)this.tail[i].GetX(), (int)this.tail[i].GetY()] = new PixelFilled();
        }

        render[(int)this.position.GetX(), (int)this.position.GetY()] = new PixelFilled(ConsoleColor.DarkBlue);
    }
}
