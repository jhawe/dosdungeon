using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Models
{
    public class Position
    {
        private int x;
        private int y;

        internal Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get
            {
                return this.x;
            }
        }
        public int Y
        {
            get
            {
                return this.y;
            }
        }
    }
}
