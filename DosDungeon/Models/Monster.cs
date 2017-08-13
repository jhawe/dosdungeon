using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Models
{
    internal class Monster : Moveable
    {
        internal Monster(int health = 1) : base(health)
        {
            // member init
        }        
    }
}
