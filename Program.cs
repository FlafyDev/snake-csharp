using System;

internal class Program
{
    private static void Main(string[] args)
    {
        GameRenderer renderer = new GameRenderer(144);
        renderer.AddGameObject(new GameManager());
        renderer.Start();
        renderer.Join();
        Console.WriteLine("Ended");
    }
}
