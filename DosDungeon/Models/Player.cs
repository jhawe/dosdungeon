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
        private string name; // the player's name
        private int health;
        private Position position;
        private const int MAXHEALTH = 5;
        private int gold;
        private Direction face;
        private int monstersKilled;
        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The player's name</param>
        internal Player(string name = "Player1", int health = MAXHEALTH)
        {
            // member init
            this.name = name;
            this.health = health;
            this.gold = 0;
            this.monstersKilled = 0;
        }
        #endregion // Constructor

        internal Position Position
        {
            get
            {
                return this.position;
            }
        }

        internal int Gold
        {
            get
            {
                return this.gold;
            }
        }

        internal int Health
        {
            get
            {
                return this.health;
            }
        }

        internal Position AttackField
        {
            get
            {
                switch (this.face)
                {
                    case Direction.Down:
                        return new Position(this.position.X + 1, this.position.Y);
                    case Direction.Left:
                        return new Position(this.position.X, this.position.Y + 1);
                    case Direction.Right:
                        return new Position(this.position.X, this.position.Y - 1);
                    case Direction.Up:
                        return new Position(this.position.X - 1, this.position.Y);
                    default:
                        // will never happen
                        return null;
                }
            }
        }

        public Direction Face
        {
            get
            {
                return this.face;
            }
        }

        internal void SetPosition(Position m)
        {
            this.position = m;
        }       

        internal void SetFace(Direction dir)
        {
            this.face = dir;
        }        

        internal void HealthUp(int v)
        {
            this.health = Math.Min(MAXHEALTH, this.health + v);
        }

        internal void HealthDown(int v)
        {
            this.health = Math.Max(0, this.health - v);
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
        }
    }
}
