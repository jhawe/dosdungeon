using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Models
{
    internal class Position
    {
        private int x;
        private int y;

        internal Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        internal int X
        {
            get
            {
                return this.x;
            }
        }
        internal int Y
        {
            get
            {
                return this.y;
            }
        }
    }
}
