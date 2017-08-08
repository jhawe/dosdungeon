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
        string name; // the player's name
        int health;
        private Position position;
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
        
        internal Position Position
        {
            get
            {
                return this.position;
            }
        }

        internal void Move(Position m)
        {
            this.position = m;
        }
    }
}
