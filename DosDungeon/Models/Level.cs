using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Models
{
    /// <summary>
    /// Defines our dungeon levels. Levels are based on matrices with specific entries
    /// defining different fields, e.g. free, blocked etc.
    /// </summary>
    internal class Level
    {
        #region Class Member
        int size;
        int[,] field;
        private int startX;
        private int startY;
        private int endY;
        private int endX;
        private int playerX = -1;
        private int playerY = -1;
        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        internal Level()
        {
            // member init
        }

        internal Level(int size)
        {
            // member init
            this.size = size;
            this.field = new int[this.size, this.size];
        }
        #endregion // Constructor

        #region Properties
        internal int Size
        {
            get
            {
                return this.size;
            }
        }

        internal int StartX
        {
            get
            {
                return this.startX;
            }
        }
        internal int StartY
        {
            get
            {
                return this.startY;
            }
        }
        internal int EndX
        {
            get
            {
                return this.endX;
            }
        }
        internal int EndY
        {
            get
            {
                return this.endY;
            }
        }
        #endregion // Properties

        #region Methods

        #region GenerateLevel
        /// <summary>
        /// Generates a new random level of the specified size
        /// </summary>
        /// <param name="size">the size of the level to generate (square)</param>
        /// <returns>A new level instance</returns>
        internal static Level GenerateLevel(int size = 16)
        {
            Level l = new Level(size);

            // for now generate a very simple test level only
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (j == 0)
                    {
                        l.field[i, j] = (int)Field.Free;
                    }
                    else
                    {
                        if (i == Math.Floor(l.size/2.0))
                        {
                            l.field[i, j] = (int)Field.Free;
                        }
                        else
                        {
                            l.field[i, j] = (int)Field.Blocked;
                        }
                    }
                }
            }
            // set start position
            l.startX = 15;
            l.startY = 0;
            // set end position
            l.endX = 0;
            l.endY = 0;

            return (l);
        }
        #endregion // GenerateLevel

        #region GetField
        /// <summary>
        /// Gets a specific field's descriptive enum
        /// </summary>
        /// <param name="x">X pos of the field</param>
        /// <param name="y">Y pos of the field</param>
        /// <returns></returns>
        internal Field GetField(int x, int y)
        {
            // out of bounds
            if (x >= this.size | y >= this.size | x < 0 | y < 0)
            {
                return Field.NA;
            }

            var value = this.field[x, y];
            Field result;
            bool sanity = Enum.TryParse<Field>(value.ToString(), out result);
            if (!sanity)
            {
                // TODO note error here
            }
            return (result);
        }
        #endregion // GetField

        #region SetPlayerPos
        /// <summary>
        /// Sets the position of the player to the current field
        /// </summary>
        /// <param name="x">X pos</param>
        /// <param name="y">Y pos</param>
        internal void SetPlayerPos(int x, int y)
        {
            this.field[x, y] = (int)Field.Player;
            if (this.playerX != -1)
            {
                this.field[this.playerX, this.playerY] = (int)Field.Free;
            }
            this.playerX = x;
            this.playerY = y;
        }
        #endregion // SetPlayerPos

        #endregion // Methods

    }
}
