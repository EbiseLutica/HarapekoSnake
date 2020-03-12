using System;
using DotFeather;

namespace HarapekoSnake
{
    class Entry
    {
        static void Main(string[] args)
        {
            new RoutingGameBase<TitleScene>(512, 480, "はらぺこスネーク", 60, false, true).Run();
        }
    }
}
