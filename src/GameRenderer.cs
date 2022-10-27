using System;
using System.Diagnostics;
using System.Threading;
using Unit4.CollectionsLib;

abstract class Pixel
{
    protected ConsoleColor color;
    public ConsoleColor GetColor() { return this.color; }
}
class PixelFilled : Pixel
{
    public PixelFilled(ConsoleColor color = ConsoleColor.White)
    {
        this.color = color;
    }
}
class PixelText : Pixel
{
    private string text;
    public PixelText(string text, ConsoleColor color = ConsoleColor.White)
    {
        this.text = text;
        this.color = color;
    }
    public string GetText() { return this.text; }
}

class GameRenderer
{
    private double fps;
    private GameObject[] gameObjects;
    private int gameObjectsLength;
    private bool running;

    private Thread renderThread;
    private Thread inputThread;

    private int width;
    private int height;

    public GameRenderer(double fps)
    {
        this.fps = fps;
        this.gameObjects = new GameObject[100];
        this.gameObjectsLength = 0;
        this.running = false;
        this.renderThread = null;
        this.inputThread = null;
    }

    public void AddGameObject(GameObject gameObject)
    {
        if (this.gameObjectsLength == this.gameObjects.Length)
        {
            throw new Exception("Couldn't add a GameObject because the list was full.");
        }

        gameObject.SetGameRenderer(this);
        gameObject.OnInit();

        for (int i = 0; i < this.gameObjectsLength; i++)
        {
            if (this.gameObjects[i] == null)
            {
                this.gameObjects[i] = gameObject;
                return;
            }
        }

        this.gameObjects[this.gameObjectsLength] = gameObject;
        this.gameObjectsLength++;
    }

    public void RemoveGameObject(GameObject gameObject)
    {
        Stack<int> toDeleteIndexes = new Stack<int>();
        for (int i = 0; i < this.gameObjectsLength; i++)
        {
            if (this.gameObjects[i] == gameObject)
            {
                gameObject.OnDestroy();
                this.gameObjects[i] = null;
                break;
            }
        }
    }

    public Stack<GameObject> findAllGameObjects()
    {
        return findAllGameObjects<GameObject>();
    }

    public Stack<T> findAllGameObjects<T>() where T : GameObject
    {
        Stack<T> foundGameObjects = new Stack<T>();
        for (int i = 0; i < this.gameObjectsLength; i++)
        {
            if (this.gameObjects[i] is T)
            {
                foundGameObjects.Push(this.gameObjects[i] as T);
            }
        }
        return foundGameObjects;
    }

    public void Start()
    {
        if (this.running)
        {
            throw new Exception("Start was called while already running.");
        }

        Console.CursorVisible = false;
        this.running = true;

        this.renderThread = new Thread(RenderLoop);
        this.renderThread.Start();
        this.inputThread = new Thread(InputLoop);
        this.inputThread.Start();
    }

    public void Join()
    {
        this.renderThread.Join();
        this.inputThread.Join();
    }

    public void Stop()
    {
        this.running = false;

        // Wait for the threads to exit in a safe manner.
        this.Join();

        this.renderThread = null;
        this.inputThread = null;
        Console.CursorVisible = true;
    }

    private void Draw(int x, int y, Pixel pixel)
    {
        x *= 2;
        if (x < 0 || x > Console.WindowWidth - 1 || y < 0 || y > Console.WindowHeight - 0)
        {
            return;
        }

        Console.SetCursorPosition(x, y);
        if (pixel != null)
        {
            Console.ForegroundColor = pixel.GetColor();
        }

        if (pixel is PixelFilled)
        {
            Console.Write("██");
        }
        else if (pixel is PixelText)
        {
            String text = (pixel as PixelText).GetText();
            if (text.Length % 2 == 1)
            {
                text += " ";
            }
            Console.Write(text);
        }
        else
        {
            Console.Write("  ");
        }
    }

    public int GetWidth()
    {
        return Console.WindowWidth / 2;
    }

    public int GetHeight()
    {
        return Console.WindowHeight;
    }

    private void RenderLoop()
    {
        Console.Clear();
        var sw = new Stopwatch();
        sw.Start();

        while (true)
        {
            if (!this.running) return;

            Pixel[,] newRender = new Pixel[GetWidth(), GetHeight()];

            sw.Stop();
            for (int i = this.gameObjectsLength - 1; i >= 0; i--)
            {
                if (this.gameObjects[i] == null) continue;
                GameObject gameObject = this.gameObjects[i];
                gameObject.Render(sw.ElapsedMilliseconds / 1000.0, newRender);
            }
            sw.Reset();
            sw.Start();

            for (int j = 0; j < newRender.GetLength(1); j++)
            {
                for (int k = 0; k < newRender.GetLength(0); k++)
                {
                    Pixel pixel = newRender[k, j];
                    Draw(k, j, pixel);
                    if (pixel is PixelText)
                    {
                        k += (int)Math.Ceiling((pixel as PixelText).GetText().Length / 2.0) - 1;
                    }
                }
            }

            // Calculate the time needed to sleep in each loop to reach the required number of frames per second.
            Thread.Sleep((int)(1000 / this.fps));
        }
    }

    private void InputLoop()
    {
        while (true)
        {
            if (!this.running) return;

            ConsoleKey key = Console.ReadKey(true).Key;

            for (int i = 0; i < this.gameObjectsLength; i++)
            {
                if (this.gameObjects[i] == null) continue;
                this.gameObjects[i].Input(key);
            }
        }
    }
}
