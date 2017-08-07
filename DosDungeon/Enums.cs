using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon
{
    internal enum GameState
    {
        Pause,
        Running
    }

    internal enum Field
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
        // heart item to fill up health point
        Heart = 64,
        NA = 128
    }
}
