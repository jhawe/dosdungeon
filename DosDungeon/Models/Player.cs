using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Models
{
    internal class Player
    {
        #region Class Member
        /// <summary>
        /// Class Member
        /// </summary>
        int posX; // current x position on field
        int posY; // current x position on field
        string name; // the player's name
        int health;
        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The player's name</param>
        internal Player(string name="Player1", int health=5)
        {
            // member init
            this.name = name;
            this.health = health;
        }
        #endregion // Constructor
        
        internal int PosX
        {
            get
            {
                return this.posX;
            }
        }
        internal int PosY
        {
            get
            {
                return this.posY;
            }
        }

        internal void Move(Move m)
        {
            this.posX = m.X;
            this.posY = m.Y;
        }
    }
}
