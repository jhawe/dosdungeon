using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon
{
    public enum GameState
    {
        Pause,
        Running,
        LevelFinished,
        GameOver
    }

    public enum Field
    {
        // a normal free field
        Free = 1,
        // a blocked field
        Blocked = 2,
        // our hero
        Player = 4,
        // a filthy monster
        Monster = 8,
        // the main path
        Main = 16,
        // a path branched from the main path
        Branch = 32,
        // treasure chest!
        Treasure = 64,
        NA = 128
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
