using System;

namespace DosDungeon.Models
{
    /// <summary>
    /// A simple fighter, being able to be moved on the field
    /// and has some health
    /// </summary>
    public class Fighter
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

        /// <summary>
        /// Gets/setsThe fighters position
        /// </summary>
        internal Position Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        /// <summary>
        /// Gets/sets The fighters health
        /// </summary>
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

        #region AttackField
        /// <summary>
        /// Describes the field the fighter would attack
        /// if he were to act so, i.e. the field the player is facing
        /// </summary>
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
        #endregion

        #region Face
        /// <summary>
        /// The direction the player is facing
        /// </summary>
        public Direction Face
        {
            get
            {
                return this.face;
            }
            internal set
            {
                this.face = value;
            }
        }
        #endregion // Face

        #region HealthUp
        internal void HealthUp(int v)
        {
            this.health = Math.Min(MAXHEALTH, this.health + v);
        }
        #endregion // HealthUp

        #region HealthDown
        internal void HealthDown(int v)
        {
            this.health = Math.Max(0, this.health - v);
        }
        #endregion // HealthDown
    }
}
