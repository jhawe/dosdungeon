using System;

namespace DosDungeon.Models
{
    internal class Fighter
    {
        #region Class Member
        /// <summary>
        /// Class Member
        /// </summary>                
        private int health;
        private Position position;
        internal const int MAXHEALTH = 5;        
        private Direction face;        
        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The player's name</param>
        internal Fighter(int health = MAXHEALTH)
        {
            // member init            
            this.health = health;
            this.position = null;
        }
        #endregion // Constructor

        internal Position Position
        {
            get
            {
                return this.position;
            }
        }

        internal int Health
        {
            get
            {
                return this.health;
            }
            set
            {
                this.health = Math.Min(value, MAXHEALTH);
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
                        return new Position(this.position.X, this.position.Y - 1);
                    case Direction.Right:
                        return new Position(this.position.X, this.position.Y + 1);
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
    }
}
