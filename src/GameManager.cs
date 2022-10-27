using System;
using Unit4.CollectionsLib;
using static Utils;

class GameManager : GameObject
{
    private bool multiplayer;

    private int score;
    private int selectedLayer;
    private bool gameRunning;

    public GameManager() : base(new Point(0, 0))
    {
        this.score = 0;
        this.multiplayer = false;
        this.selectedLayer = 0;
        this.gameRunning = false;
    }

    public int GetScore()
    {
        return this.score;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    public override void Input(ConsoleKey key)
    {
        if (this.gameRunning) return;

        int changeSelected = 0;
        switch (key)
        {
            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
            case ConsoleKey.LeftArrow:
                changeSelected = -1;
                break;
            case ConsoleKey.RightArrow:
                changeSelected = 1;
                break;
            case ConsoleKey.UpArrow:
                this.selectedLayer--;
                break;
            case ConsoleKey.DownArrow:
                this.selectedLayer++;
                break;
        }
        this.selectedLayer = (int)mod(this.selectedLayer, 2);
        if (changeSelected != 0)
        {
            switch (this.selectedLayer)
            {
                case 0:
                    this.multiplayer = !this.multiplayer;
                    break;
                case 1:
                    this.StartGame();
                    break;
            }
        }
    }


    public void StartGame()
    {
        if (this.gameRunning) return;

        this.score = 0;
        this.gameRenderer.AddGameObject(new ItemCreator());
        this.gameRenderer.AddGameObject(new Snake(new Point(10, 10)));
        if (this.multiplayer)
        {
            this.gameRenderer.AddGameObject(new Snake(new Point(12, 12), true));
        }

        this.gameRunning = true;
    }

    public void EndGame()
    {
        if (!this.gameRunning) return;

        Stack<GameObject> allObjects = this.gameRenderer.findAllGameObjects();
        while (!allObjects.IsEmpty())
        {
            GameObject obj = allObjects.Pop();
            if (obj != this)
            {
                this.gameRenderer.RemoveGameObject(obj);
            }
        }

        this.gameRunning = false;
    }

    public override void Render(double delta, Pixel[,] render)
    {
        if (this.gameRunning)
        {
            int y = this.gameRenderer.GetHeight() - 2;
            render[0, y] = new PixelText($"score: {this.score}");
        }
        else
        {
            render[0, 1+(2*selectedLayer)] = new PixelText("➤", ConsoleColor.Blue);
            render[1, 0+(2*selectedLayer)] = new PixelText("╭─────────────────────╮", ConsoleColor.Green);
            render[1, 2+(2*selectedLayer)] = new PixelText("╰─────────────────────╯", ConsoleColor.Green);
            render[1, 1+(2*selectedLayer)] = new PixelText("│", ConsoleColor.Green);
            render[12, 1+(2*selectedLayer)] = new PixelText("│", ConsoleColor.Green);

            render[2, 1] = new PixelText($"Multiplayer: {this.multiplayer}", selectedLayer == 0 ? ConsoleColor.Yellow : ConsoleColor.White  );
            render[2, 3] = new PixelText("Start!", selectedLayer == 1 ? ConsoleColor.Yellow : ConsoleColor.White);

            render[1, 6] = new PixelText("Select: UP/DOWN     Change: LEFT/RIGHT", ConsoleColor.DarkGray);
            render[1, 7] = new PixelText("Player 1 Controls: Arrows", ConsoleColor.DarkGray);
            render[1, 8] = new PixelText("Player 2 Controls: WASD", ConsoleColor.DarkGray);

            render[1, 10] = new PixelText($"Last score: {this.score}", ConsoleColor.Red);

        }
    }
}
