using DosDungeon.Controller;
using DosDungeon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Common
{
    internal static class LevelGenerator
    {
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
                //GenerateRecursive(l);
            }

            PopulateTreasures(l);
            PopulateMonsters(l);

            return (l);
        }
        #endregion // GenerateLevel

        private static void CheckPath(Level l)
        {
            // temp vars to check min dist
            float minFreeDist = float.MaxValue;
            Position minDistPos = null;

            // get the free field nearest to the end
            for (int i = 0; i < l.Size; i++)
            {
                for (int j = 0; j < l.Size; j++)
                {

                    Field f = l.GetField(i, j);
                    float d = GetDistance(i, j, l.End.X, l.End.Y);
                    if (l.IsFieldAccessible(i, j)
                        && d < minFreeDist
                        && !(i == l.End.X && j == l.End.Y))

                    {
                        minFreeDist = d;
                        minDistPos = new Position(i, j);
                    }
                }
            }

            // already at the end, nothing to do
            if (minDistPos.X == l.End.X && minDistPos.Y == l.End.Y)
            {
                return;
            }
            // starting from the mindistposition, finish the path to the 
            // end position
            bool proceed = true;
            Position current = minDistPos;
            while (proceed)
            {
                List<Position> ppos = GetBlocked(l, current);
                foreach (Position p in ppos)
                {
                    if (IsMoveTowardsEnd(l, current.X, current.Y, p.X, p.Y))
                    {
                        current = p;
                        break;
                    }
                }
                if (current.X == l.End.X && current.Y == l.End.Y)
                {
                    proceed = false;
                }
                else
                {
                    l.SetField(current, Field.Free);
                }
            }
        }

        private static void PopulateMonsters(Level l)
        {
            // set one random monster
            for (int i = 0; i < l.Size; i++)
            {
                for (int j = 0; j < l.Size; j++)
                {
                    // dont put a monster on the end for now
                    if (i != l.End.X && j != l.End.Y)
                    {
                        // dont put monster to near to the start
                        List<Position> lp = new List<Position>(1);
                        lp.Add(l.Start);
                        if (GetDistances(i, j, lp)[0] > 3)
                        {
                            // check whether the field is accessible
                            if (l.IsFieldAccessible(i, j))
                            {
                                // 10% chance to get a monster
                                if (Game.RNG.NextDouble() < 0.1)
                                {
                                    l.SetField(new Position(i, j), Field.Monster);
                                }
                            }
                        }
                    }
                }
            }
        }

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
            var main = l.Main;
            var ml = main.Count;
            // define a chance to get a branch dependent on the
            // main path's length

            // gets higher the longer the path
            var c = (1 - (1.0 / ml)) * 0.4;

            foreach (Position p in main)
            {
                var r = Game.RNG.NextDouble();
                if (c < r)
                {
                    Console.WriteLine("Creating new branch.");
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
                // 5% chance to just stop where we are
                // if we walked at least 3 fields
                var r = Game.RNG.NextDouble();
                if (r < 0.05 && b.Count >= 3)
                {
                    proceed = false;
                }

                // get fields
                List<Position> pfields = GetBlocked(l, current);

                // nowhere to go
                if (pfields.Count < 1)
                {
                    proceed = false;
                }

                // first check whether we are back to the main path
                List<Position> rm = new List<Position>();
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
                                // just stop, we dont need to add a new
                                // branch entry since we have a main-entry here
                                proceed = false;
                            }
                            else
                            {
                                rm.Add(pos);
                            }
                        }
                    }
                    else if (l.IsEdgeField(pos) && pos.X != l.Start.X && pos.Y != l.Start.Y)
                    {
                        // if we reached an edge, we have a 50% chance to stop here
                        r = Game.RNG.NextDouble();
                        if (r <= 0.5)
                        {
                            b.AddLast(pos);
                            l.SetField(pos, Field.Branch);
                            Console.WriteLine("adding position to branch: " + pos.X + ":" + pos.Y);
                            proceed = false;
                        }
                    }
                }
                if (proceed)
                {
                    if (rm.Count > 0)
                    {
                        foreach (Position p1 in rm)
                        {
                            pfields.Remove(p1);
                        }
                    }
                    if (pfields.Count > 0)
                    {
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
                        Console.WriteLine("adding position to branch: " + chosen.X + ":" + chosen.Y);
                        b.AddLast(chosen);
                        l.SetField(chosen, Field.Branch);
                        current = chosen;
                    }
                    else
                    {
                        proceed = false;
                    }
                }
            }
            if (b.Count > 0)
            {
                l.AddBranch(b);
            }
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
            var b = l.Branches;
            foreach (var branch in b)
            {
                Position last = branch.Last.Value;
                // just get a 30% chance to add a treausre chest
                // if we found a edge field
                var r = Game.RNG.NextDouble();
                if (r < 0.3 && l.IsEdgeField(last))
                {
                    l.SetField(last.X, last.Y, Field.Treasure);
                }
            }
        }
        #endregion // PopulateTreasures       

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

            Position current = new Position(l.Start.X, l.Start.Y);

            bool proceed = true;

            while (proceed)
            {
                // get possible directions
                List<Position> pmoves = GetBlocked(l, current);

                if (pmoves.Count < 1)
                {
                    break;
                }

                int minDistMoveIdx = GetMinDistMove(l.End, pmoves);

                // choose a random move
                current = ChooseMove(pmoves, minDistMoveIdx);

                // set the new field for the main path
                l.SetField(current, Field.Main);
                l.Main.AddLast(current);

                // check whether we arrived at an edge
                if (l.End.X == current.X && l.End.Y == current.Y)
                //.IsEdgeField(current) && 
                //current.X != l.Start.X && current.Y != l.Start.Y)
                {
                    proceed = false;
                }
            }
            // check whether we have a connected path 
            // and generate one if necessary
            CheckPath(l);
        }
        #endregion // GeneratePath

        private static Position ChooseMove(List<Position> pmoves, int minDistMoveIdx)
        {
            // TODO make a random choice, slightly favored to the 
            // minDistMove
            double r = Game.RNG.NextDouble();
            Position minDistMove = pmoves[minDistMoveIdx];
            Position c = null;
            if (pmoves.Count == 4)
            {
                if (r < 0.25) c = pmoves[0];
                if (r >= 0.25 && r < 0.5) c = pmoves[1];
                if (r >= 0.5 && r < 0.75) c = pmoves[2];
                if (r >= 0.75) c = pmoves[3];
            }
            else if (pmoves.Count == 3)
            {
                if (r < 0.33) c = pmoves[0];
                if (r >= 0.33 && r < 0.66) c = pmoves[1];
                if (r >= 0.66) c = pmoves[2];
            }
            else if (pmoves.Count == 2)
            {
                if (r < 0.5) c = pmoves[0];
                if (r >= 0.5) c = pmoves[1];
            }
            else
            {
                c = minDistMove;
            }
            // extra chance for mindistmove
            if (Game.RNG.NextDouble() < 0.1)
            {
                c = minDistMove;
            }
            return c;
        }

        private static int GetMinDistMove(Position reference, List<Position> choices)
        {
            // get distances to end position on the field
            float[] distances = GetDistances(reference.X, reference.Y, choices);
            int minDistMoveIdx = -1;
            float minDist = Enumerable.Min<float>(distances);
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] == minDist)
                {
                    minDistMoveIdx = i;
                    break;
                }
            }
            return minDistMoveIdx;
        }

        #region GenerateRecursive

        private static void GenerateRecursive(Level l)
        {
            // start recursion
            GenerateStartEnd(l);
            Position p = l.Start;
            RGenRec(l, p);
        }

        private static void RGenRec(Level l, Position p, bool isBranch = false)
        {
            // TODO implement
            Field toset = isBranch ? Field.Branch : Field.Main;

            // random stop if branch
            if (Game.RNG.NextDouble() < 0.33 && !isBranch)
            {
                l.SetField(p, toset);
                return;
            }

            List<Position> pdirs = GetBlocked(l, p);
            if (pdirs.Count > 0)
            {
                int midx = GetMinDistMove(l.End, pdirs);
                Position minDistMove = pdirs[midx];

            }
            // branch randomly, also two side branching
        }

        #endregion // GenerateRecursive

        #region GetBlocked
        /// <summary>
        /// For a level and a position, get the fields next to the position which are 
        /// currently blocked
        /// </summary>
        /// <param name="l">The level for which to get the directions</param>
        /// <param name="x">The current x position</param>
        /// <param name="y">The current y position</param>
        /// <returns>A list of moves to which a path could be extended</returns>
        private static List<Position> GetBlocked(Level l, Position pos)
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
                result.Add(new Position(l.End.X, l.End.Y));
                return result;
            }

            // bottom
            int x1 = x + 1;
            int y1 = y;
            Field f = l.GetField(x1, y1);

            if (f == Field.Blocked)
            {
                result.Add(new Position(x1, y1));
            }
            // top
            x1 = x - 1;
            y1 = y;
            f = l.GetField(x1, y1);
            if (f == Field.Blocked)
            {
                result.Add(new Position(x1, y1));
            }

            // left
            x1 = x;
            y1 = y - 1;
            f = l.GetField(x1, y1);
            if (f == Field.Blocked)
            {
                result.Add(new Position(x1, y1));
            }

            // right
            x1 = x;
            y1 = y + 1;
            f = l.GetField(x1, y1);
            if (f == Field.Blocked)
            {
                result.Add(new Position(x1, y1));
            }

            return (result);
        }
        #endregion // GetBlocked

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

        #region IsMoveTowardsStart
        /// <summary>
        /// Checks whether a move is directed towards the start field of a level
        /// based on the previous position and the next position
        /// </summary>
        /// <param name="l">The level</param>
        /// <param name="xp">Previous x</param>
        /// <param name="yp">Previous y</param>
        /// <param name="xn">Next x</param>
        /// <param name="yn">Next y</param>
        /// <returns></returns>
        private static bool IsMoveTowardsStart(Level l, int xp, int yp, int xn, int yn)
        {
            return (Math.Abs(xn - l.Start.X) < Math.Abs(xp - l.Start.X) ||
                Math.Abs(yn - l.Start.Y) < Math.Abs(yp - l.Start.Y));
        }
        #endregion // IsMoveTowardsStart

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

            int maxPos = l.Size - 1;

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

                // euclidian distance
                result[i] = GetDistance(x, y, m.X, m.Y);
            }
            return (result);
        }
        #endregion // GetDistances

        #region GetDistance
        /// <summary>
        /// Gets euclidean distance between to points
        /// </summary>
        /// <returns></returns>
        private static float GetDistance(int x1, int y1, int x2, int y2)
        {
            float dx = x1 - x2;
            float dy = y1 - y2;

            // euclidian distance
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        #endregion // GetDistance

        #region GenerateNaive
        /// <summary>
        /// Generates a very naive level for testing purposes
        /// </summary>
        /// <param name="l"></param>
        private static void GenerateNaive(Level l)
        {
            int size = l.Size;

            // for now generate a very simple test level only
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (j == 0)
                    {
                        l.SetField(i, j, Field.Free);
                    }
                    else
                    {
                        if (i == Math.Floor(l.Size / 2.0))
                        {
                            l.SetField(i, j, Field.Free);
                        }
                        else
                        {
                            l.SetField(i, j, Field.Blocked);
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
        #endregion // Methods
    }
}
