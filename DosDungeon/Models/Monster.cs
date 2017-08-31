using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Models
{
    /// <summary>
    /// Simple monster, currently nothing really different
    /// to a basic fighter
    /// </summary>
    internal class Monster : Fighter
    {
        internal Monster(int health = 1) : base(health)
        {
            // member init
        }        
    }
}
