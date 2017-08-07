using DosDungeon.Controller;
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
        public Level() : this(16)
        {
            // default size is 16            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of the level (gets squared)</param>
        internal Level(int size)
        {
            // member init
            this.size = size;
            this.field = new int[this.size, this.size];
            // init field defaults
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    this.field[i, j] = (int)Field.Blocked;
                }
            }
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
            if (false)
            {
                GenerateNaive(l);
            }
            else
            {
                GenerateStartEnd(l);
                GeneratePath(l);
                GenerateBranches(l);
            }

            return (l);
        }

        private static void GenerateBranches(Level l)
        {
            // do nothing for now
        }

        private static void GeneratePath(Level l)
        {
            // generate a path from start to end in the level
            // direction of path is random (choosing from the
            // possible directions in which to go), however it is always
            // slightly attracted to the end position in the level

            int currentX = l.startX;
            int currentY = l.startY;

            bool proceed = true;

            while (proceed)
            {
                // get possible directions
                List<Move> pmoves = GetPossibleDirections(l, currentX, currentY);

                // get distances to end position on the field
                float[] distances = GetDistances(l.endX, l.endY, pmoves);
                // choose a move more likely directed towards the end position,
                // with some room for randomness
                Move minDistMove = null;
                int minDistMoveIdx = -1;
                float minDist = Enumerable.Min<float>(distances);
                for (int i = 0; i < distances.Length; i++)
                {
                    if (distances[i] == minDist)
                    {
                        minDistMove = pmoves[i];
                        minDistMoveIdx = i;
                        break;
                    }
                }
                // TODO make a random choice, slightly favored to the 
                // minDistMove
                double r = Game.RNG.NextDouble();
                Move c = null;
                if (pmoves.Count == 4)
                {
                    if (r < 0.25) c = pmoves[0];
                    if (r >= 0.25 && r < 0.5) c = pmoves[1];
                    if (r >= 0.5 && r < 0.75) c = pmoves[2];
                    if (r >= 0.75) c = pmoves[3];
                    // extra chance for mindistmove
                    if (Game.RNG.NextDouble() > 0.75)
                    {
                        c = pmoves[minDistMoveIdx];
                    }                    
                }
                else if (pmoves.Count == 3)
                {
                    if (r < 0.33) c = pmoves[0];
                    if (r >= 0.33 && r < 0.66) c = pmoves[1];
                    if (r >= 0.66) c = pmoves[2];   
                    // extra chance for mindistmove
                    if (Game.RNG.NextDouble() > 0.75)
                    {
                        c = pmoves[minDistMoveIdx];
                    }
                }
                else if (pmoves.Count == 2)
                {
                    if (r < 0.5) c = pmoves[0];
                    if (r >= 0.5) c = pmoves[1];
                    // extra chance for mindistmove
                    if (Game.RNG.NextDouble() > 0.75)
                    {
                        c = pmoves[minDistMoveIdx];
                    }
                }
                else
                {
                    c = minDistMove;
                }
               
                // set the new free field
                l.field[c.X, c.Y] = (int)Field.Free;
                currentX = c.X;
                currentY = c.Y;

                // check whether we arrived at the end
                if (currentX == l.endX && currentY == l.endY)
                {
                    proceed = false;
                }
            }
        }

        #region GetDistances
        private static float[] GetDistances(int endX, int endY, List<Move> pmoves)
        {
            float[] result = new float[pmoves.Count];
            for (int i = 0; i < result.Length; i++)
            {
                Move m = pmoves[i];
                float dx = endX - m.X;
                float dy = endY - m.Y;

                // euclidian distance
                result[i] = (float)Math.Sqrt(dx * dx + dy * dy);
            }
            return (result);
        }
        #endregion // GetDistances

        #region GetPossibleDirections
        /// <summary>
        /// Gets a list of possible 'directions' for a new path entry
        /// </summary>
        /// <param name="l">The level for which to get the directions</param>
        /// <param name="x">The current x position</param>
        /// <param name="y">The current y position</param>
        /// <returns>A list of moves to which a path could be extended</returns>
        private static List<Move> GetPossibleDirections(Level l, int x, int y)
        {
            // generate a move for each possible direction
            // only be able to move to "blocked" fields (default field setting)
            // bounds get checked in GetField-method

            List<Move> result = new List<Move>();

            // bottom
            int x1 = x + 1;
            int y1 = y;
            Field f = l.GetField(x1, y1);
            if (f == Field.Blocked && 
                (Math.Abs(x1-l.endX)<Math.Abs(x-l.endX) || 
                Math.Abs(y1 - l.endY) < Math.Abs(y - l.endY)))
            {
                result.Add(new Move(x1, y1));
            }
            // top
            x1 = x - 1;
            y1 = y;
            f = l.GetField(x1, y1);
            if (f == Field.Blocked &&
                (Math.Abs(x1 - l.endX) < Math.Abs(x - l.endX) ||
                Math.Abs(y1 - l.endY) < Math.Abs(y - l.endY)))
            {
                result.Add(new Move(x1, y1));
            }

            // left
            x1 = x;
            y1 = y - 1;
            f = l.GetField(x1, y1);
            if (f == Field.Blocked &&
                (Math.Abs(x1 - l.endX) < Math.Abs(x - l.endX) ||
                Math.Abs(y1 - l.endY) < Math.Abs(y - l.endY)))
            {
                result.Add(new Move(x1, y1));
            }

            // right
            x1 = x;
            y1 = y + 1;
            f = l.GetField(x1, y1);
            if (f == Field.Blocked &&
                (Math.Abs(x1 - l.endX) < Math.Abs(x - l.endX) ||
                Math.Abs(y1 - l.endY) < Math.Abs(y - l.endY)))
            {
                result.Add(new Move(x1, y1));
            }

            return (result);
        }
        #endregion // GetPossibleDirections

        private static void GenerateStartEnd(Level l)
        {
            int startX = -1;
            int startY = -1;
            int endX = -1;
            int endY = -1;

            int maxPos = l.size - 1;

            int[] minmax = new int[] { 0, maxPos };

            // whether to start at vertical or horizontal?
            int r = Game.RNG.Next(0, 1);
            if (r == 0)
            {
                // left or right start
                r = Game.RNG.Next(0, 1);
                startY = minmax[r];
                // random point on vertical axis
                startX = Game.RNG.Next(0, maxPos);

                // end point, left or right
                endY = minmax[1 - r];

                // set end vertical position
                if (startX >= maxPos / 2)
                {
                    // lower quartile
                    endX = Game.RNG.Next(0, maxPos / 4);
                }
                else
                {
                    // upper quartile
                    endX = Game.RNG.Next(maxPos / 2 + maxPos / 4, maxPos);
                }
            }
            else
            {
                // top or bottom start
                r = Game.RNG.Next(0, 1);
                startX = minmax[r];
                // random point on horizontal axis
                startY = Game.RNG.Next(0, maxPos);

                // end point, top or bottom
                endX = minmax[1 - r];

                // set end horizontal position
                if (startY >= maxPos / 2)
                {
                    // lower quartile
                    endY = Game.RNG.Next(0, maxPos / 4);
                }
                else
                {
                    // upper quartile
                    endY = Game.RNG.Next(maxPos / 2 + maxPos / 4, maxPos);
                }
            }
            // set positions to level instance
            l.startX = startX;
            l.startY = startY;
            l.endX = endX;
            l.endY = endY;
        }

        private static void GenerateNaive(Level l)
        {
            int size = l.size;

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
                        if (i == Math.Floor(l.size / 2.0))
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
            // reset previous player field to be free again
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
