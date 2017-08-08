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
        private Position start;
        private Position end;
        private Position playerPos;
        private LinkedList<Position> mainPath;
        private List<LinkedList<Position>> branches;
        private int playerField;
        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Level() : this(16)
        {
            // member init
            this.mainPath = new LinkedList<Position>();
            this.branches = new List<LinkedList<Position>>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of the level (gets squared)</param>
        internal Level(int size)
        {
            // member init
            this.mainPath = new LinkedList<Position>();
            this.branches = new List<LinkedList<Position>>();
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

        internal Position Start
        {
            get
            {
                return this.start;
            }
        }

        internal Position End
        {
            get
            {
                return this.end;
            }
        }

        public bool IsFinished { get; internal set; }

        #endregion // Properties

        #region Methods

        #region GenerateLevel
        /// <summary>
        /// Generates a new random level of the specified size
        /// </summary>
        /// <param name="size">the size of the level to generate (square)</param>
        /// <returns>A new level instance</returns>
        internal static Level GenerateLevel(int size = 32)
        {
            Level l = new Level(size);
            if (false)
            {
                GenerateNaive(l);
            }
            else
            {
                GeneratePath(l);
                GenerateBranches(l);
                PopulateTreasures(l);
            }

            return (l);
        }
        #endregion // GenerateLevel

        #region GenerateBranches
        /// <summary>
        /// Generates random branches away from the main path of
        /// a level
        /// </summary>
        /// <param name="l">The level for which to generate the 
        /// branches</param>
        private static void GenerateBranches(Level l)
        {
            // iterate over all positions of the main 
            // path
            var main = l.mainPath;
            var ml = main.Count;
            foreach (Position p in main)
            {
                // define a chance to get a branch dependent on the
                // main path's length

                // gets higher the longer the path
                var c = (1 - (1.0 / ml)) * 0.5;
                var r = Game.RNG.NextDouble();
                if (c < r)
                {
                    GrowBranch(l, p);
                }
            }
        }
        #endregion // GenerateBranches

        #region GrowBranch
        /// <summary>
        /// Creates a branch for the given level and given start position
        /// </summary>
        /// <param name="l">The level for which to create the branching</param>
        /// <param name="p">The startposition of the branching</param>
        private static void GrowBranch(Level l, Position p)
        {
            LinkedList<Position> b = new LinkedList<Position>();
            Position current = p;

            bool proceed = true;
            while (proceed)
            {
                // 1% chance to just stop where we are
                // if we walked at least 3 fields
                var r = Game.RNG.NextDouble();
                if(r < 0.01 && b.Count >= 3)
                {
                    break;
                }

                // get fields ignoreing that it should got towards the end position
                var pfields = GetPossibleDirections(l, current, true);

                // first check whether we are back to the main path
                Position rm = null;
                foreach (Position pos in pfields)
                {
                    if (l.GetField(pos.X, pos.Y) == Field.Main)
                    {
                        // at least a branch length of 3 is required
                        // to get a 33% chance of merging
                        if (b.Count >= 3)
                        {
                            r = Game.RNG.NextDouble();
                            if (r <= 0.33)
                            {
                                proceed = false;
                            }
                            else
                            {
                                rm = pos;
                            }
                        }
                    }
                    else if (l.IsEdgeField(pos))
                    {
                        // if we reached an edge, we have a 50% chance to stop here
                        r = Game.RNG.NextDouble();
                        if (r <= 0.5)
                        {
                            b.AddLast(pos);
                            proceed = false;
                        }
                    } else if(pos.X == current.X && pos.Y == current.Y)
                    {
                        rm = pos;
                    }
                }
                if (proceed)
                {
                    if (rm != null)
                    {
                        pfields.Remove(rm);
                    }
                    // choose a field by equal chance                                        
                    r = Game.RNG.NextDouble();
                    Position chosen = null;
                    // each gets the same probability
                    for (int i = 0; i < pfields.Count; i++)
                    {
                        // just devide by total possible moves
                        // and 'weight' by current position
                        var c = 1.0f / pfields.Count * (i + 1);
                        if (r < c)
                        {
                            // choose the current position
                            chosen = pfields[i];
                            break;
                        }
                    }
                    b.AddLast(chosen);
                    current = chosen;
                }
            }
            l.AddBranch(b);
        }
        #endregion // GrowBranch


        #region PopulateTreasures
        /// <summary>
        /// Randomly generates some treasure chests in the level,
        /// only on branch end positions though
        /// </summary>
        /// <param name="l">The level for which to generate treasures</param>
        private static void PopulateTreasures(Level l)
        {
            var b = l.branches;
            foreach(var branch in b)
            {
                Position last = branch.Last.Value;
                // just get a 30% chance to add a treausre chest
                // if we found a edge field
                var r = Game.RNG.NextDouble();
                if (r < 0.3 && l.IsEdgeField(last))
                {
                    l.field[last.X, last.Y] = (int)Field.Treasure;
                }
            }
        }
        #endregion // PopulateTreasures

        #region IsEdgeField
        /// <summary>
        /// Checks whether a certian position is
        /// on the edge of the level
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool IsEdgeField(Position pos)
        {
            // check if we are at the boundaries            
            int x = pos.X;
            int y = pos.Y;
            if (x == 0 || x == this.size - 1
                || y == 0 || y == this.size - 1)
            {
                return (true);
            }
            return (false);
        }
        #endregion // IsEdgeField       

        #region AddBranch
        /// <summary>
        /// Adds a generated branch to the level
        /// </summary>
        /// <param name="b">The branch to be added</param>
        private void AddBranch(LinkedList<Position> b)
        {
            foreach(var p in b)
            {
                this.field[p.X, p.Y] = (int)Field.Branch;
            }
            this.branches.Add(b);
        } 
        #endregion // AddBranch

        #region GeneratePath
        /// <summary>
        /// Generates the main path through a level.        
        /// </summary>
        /// <param name="l">The level for which to generate the path</param>
        private static void GeneratePath(Level l)
        {
            // generate start and end positions
            GenerateStartEnd(l);

            // generate a path from start to end in the level
            // direction of path is random (choosing from the
            // possible directions in which to go), however it is always
            // slightly attracted to the end position in the level

            int currentX = l.Start.X;
            int currentY = l.Start.Y;

            bool proceed = true;

            while (proceed)
            {
                // get possible directions
                List<Position> pmoves = GetPossibleDirections(l, new Position(currentX, currentY));

                // get distances to end position on the field
                float[] distances = GetDistances(l.End.X, l.End.Y, pmoves);
                // choose a Position more likely directed towards the end position,
                // with some room for randomness
                Position minDistMove = null;
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
                Position c = null;
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

                // set the new field for the main path
                l.field[c.X, c.Y] = (int)Field.Main;
                currentX = c.X;
                currentY = c.Y;
                l.mainPath.AddLast(c);

                // check whether we arrived at the end
                if (currentX == l.End.X && currentY == l.End.Y)
                {
                    l.mainPath.AddLast(c);
                    proceed = false;
                }
            }
        }
        #endregion // GeneratePath

        #region GetDistances
        /// <summary>
        /// Get the distances of all positions in a list to the given x and y coordinates
        /// </summary>
        /// <param name="x">The target x position</param>
        /// <param name="y">The target y position</param>
        /// <param name="positions">The positions for which to check the 
        /// distances</param>
        /// <returns>A vector of distances betwenn the (x,y) and positions arguments</returns>
        private static float[] GetDistances(int x, int y, List<Position> positions)
        {
            float[] result = new float[positions.Count];
            for (int i = 0; i < result.Length; i++)
            {
                Position m = positions[i];
                float dx = x - m.X;
                float dy = y - m.Y;

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
        private static List<Position> GetPossibleDirections(Level l, Position pos, bool ignoreEnd = false)
        {
            // generate a Position for each possible direction
            // only be able to Position to "blocked" fields (default field setting)
            // bounds get checked in GetField-method

            int x = pos.X;
            int y = pos.Y;

            List<Position> result = new List<Position>();

            // check whether the next move would be the end position
            if (Math.Abs(l.End.X - x) == 1 && Math.Abs(l.End.Y - y) == 0
                || Math.Abs(l.End.X - x) == 0 && Math.Abs(l.End.Y - y) == 1)
            {
                result.Add(new Position(l.End.X, l.end.Y));
                return result;
            }

            // bottom
            int x1 = x + 1;
            int y1 = y;
            Field f = l.GetField(x1, y1);

            if (f != Field.Main && f != Field.NA
                && (IsMoveTowardsEnd(l, x, y, x1, y1) | ignoreEnd))
            {
                result.Add(new Position(x1, y1));
            }
            // top
            x1 = x - 1;
            y1 = y;
            f = l.GetField(x1, y1);
            if (f != Field.Main && f != Field.NA
                && (IsMoveTowardsEnd(l, x, y, x1, y1) | ignoreEnd))
            {
                result.Add(new Position(x1, y1));
            }

            // left
            x1 = x;
            y1 = y - 1;
            f = l.GetField(x1, y1);
            if (f != Field.Main && f != Field.NA
                && (IsMoveTowardsEnd(l, x, y, x1, y1) | ignoreEnd))
            {
                result.Add(new Position(x1, y1));
            }

            // right
            x1 = x;
            y1 = y + 1;
            f = l.GetField(x1, y1);
            if (f != Field.Main && f != Field.NA
                && (IsMoveTowardsEnd(l, x, y, x1, y1) | ignoreEnd))
            {
                result.Add(new Position(x1, y1));
            }

            return (result);
        }
        #endregion // GetPossibleDirections

        #region IsMoveTowardsEnd
        /// <summary>
        /// Checks whether a move is directed towards the end field of a level
        /// based on the previous position and the next position
        /// </summary>
        /// <param name="l">The level</param>
        /// <param name="xp">Previous x</param>
        /// <param name="yp">Previous y</param>
        /// <param name="xn">Next x</param>
        /// <param name="yn">Next y</param>
        /// <returns></returns>
        private static bool IsMoveTowardsEnd(Level l, int xp, int yp, int xn, int yn)
        {
            return (Math.Abs(xn - l.End.X) < Math.Abs(xp - l.End.X) ||
                Math.Abs(yn - l.End.Y) < Math.Abs(yp - l.End.Y));
        }
        #endregion // IsMoveTowardsEnd

        #region GenerateStartEnd
        /// <summary>
        /// Generates random start and end positions for a level
        /// </summary>
        /// <param name="l">The level for which to generate start and end</param>
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
            l.SetStart(new Position(startX, startY));
            l.SetEnd(new Position(endX, endY));
        }
        #endregion // GenerateStartEnd

        #region SetEnd
        /// <summary>
        /// Sets the level's end position
        /// </summary>
        /// <param name="position">The position to be set</param>
        private void SetEnd(Position position)
        {
            this.end = position;
            this.field[this.end.X, this.end.Y] = (int)Field.Main;
        }
        #endregion // SetEnd

        #region SetStart
        /// <summary>
        /// Sets the level's start position
        /// </summary>
        /// <param name="position">the position to be set</param>
        private void SetStart(Position position)
        {
            this.start = position;
            this.field[this.start.X, this.start.Y] = (int)Field.Main;
            this.mainPath.AddLast(position);
        }
        #endregion // SetStart

        #region GenerateNaive
        /// <summary>
        /// Generates a very naive level for testing purposes
        /// </summary>
        /// <param name="l"></param>
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
            l.SetStart(new Position(15, 0));
            // set end position
            l.SetStart(new Position(0, 0));
        }
        #endregion // GenerateNaive

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
        /// <param name="pos">The position of the player to
        /// be set</param>
        internal void SetPlayerPos(Position pos)
        {
            // reset previous player field to be free again
            if (this.playerPos != null)
            {
                this.field[this.playerPos.X, this.playerPos.Y] = this.playerField;
            }
            this.playerField = this.field[pos.X, pos.Y];
            this.field[pos.X, pos.Y] = (int)Field.Player;
            this.playerPos = pos;
            
        }
        #endregion // SetPlayerPos

        #endregion // Methods
    }
}
