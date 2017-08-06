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
        Free = 1,
        Blocked = 2,
        Player = 4,
        Monster = 8,
        NA
    }
}
