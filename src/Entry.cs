using System;
using DotFeather;

namespace HarapekoSnake
{
    class Entry
    {
        static void Main(string[] args)
        {
            var game = new RoutingGameBase<TitleScene>(512, 480, "はらぺこスネーク", 60, false, true);
            game.Root.Scale = Vector.One * 2;
            game.Run();
        }
    }
}
