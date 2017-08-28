using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Models
{
    internal class Player : Moveable
    {
        #region Class Member
        /// <summary>
        /// Class Member
        /// </summary>        
        private string name; // the player's name
        private int gold;
        private int monstersKilled;
        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The player's name</param>
        internal Player(string name = "Player1") : base()
        {
            // member init
            this.name = name;            
            this.gold = 0;
            this.monstersKilled = 0;
        }
        #endregion // Constructor
             
        internal int Gold
        {
            get
            {
                return this.gold;
            }
            set
            {
                this.gold = value;
            }
        }
        
        internal void GoldUp(int amount)
        {
            this.gold += amount;
        }

        internal void MonstersKilledUp()
        {
            this.monstersKilled += 1;
        }

        internal int MonstersKilled
        {
            get
            {
                return this.monstersKilled;
            }
            set
            {
                this.monstersKilled = value;
            }
        }
    }
}
