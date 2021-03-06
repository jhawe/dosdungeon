﻿using DosDungeon.Controller;
using DosDungeon.Models;
using System;
using System.Collections.Generic;
using DosDungeon.Common;
using System.Linq;

namespace DosDungeon.Common
{
    /// <summary>
    /// Static class for everything related to level generation
    /// </summary>
    public static class LevelGenerator
    {
        #region Methods

        #region GenerateLevel
        /// <summary>
        /// Main method for generating levels.
        /// Generates a new random level of the specified size using a main path
        /// and adding branching paths later on
        /// </summary>
        /// <param name="size">the size of the level to generate (square)</param>
        /// <returns>A new level instance</returns>
        public static Level GenerateLevel(int size = 32)
        {
            Level l = new Level(size);
            // generate 'free' fields        
            GeneratePath(l);
            GenerateBranches(l);
            // populate with treasures and monsters
            PopulateTreasures(l);
            PopulateMonsters(l);

            return (l);
        }
        #endregion // GenerateLevel

        #region CheckPathNaive
        /// <summary>
        /// Checks whether the free fields connect to the end.
        /// If not, path will be extended to the end
        /// </summary>
        /// <param name="l"></param>
        /// <param name="generate"></param>
        private static void CheckPathNaive(Level l)
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
                    float d = Statics.GetDistance(i, j, l.End.X, l.End.Y);
                    if (l.IsFieldAccessible(i, j, typeof(Player))
                        && d < minFreeDist
                        && f == Field.Main)

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
                if (ppos.Count < 1)
                {
                    throw new Exception("Cannot go to the end when generating level!");
                }
                else
                {
                    // we always simply choose the mindist move 
                    // as the next field
                    int minDistIdx = GetMinDistMove(l.End, ppos);
                    current = ppos[minDistIdx];
                    if (Statics.SameField(current, l.End))
                    {
                        proceed = false;
                    }
                    l.Main.AddLast(current);
                    l.SetField(current, Field.Free);
                }
            }
        }
        #endregion // CheckPathNaive

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

            foreach (Position p in main)
            {
                var r = Game.RNG.NextDouble();
                // 10% chance to sprout new branch
                if (r <= 0.5)
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
                // 10% chance to just stop where we are
                // if we walked at least l.size/2 fields
                // or arrive at an edgefield
                var r = Game.RNG.NextDouble();
                if (r < 0.1 && b.Count >= l.Size / 2 || l.IsEdgeField(current))
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
                    // remove unwanted fields
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
            // we had a branch, add it to the level
            if (b.Count > 0)
            {
                l.AddBranch(b);
            }
        }
        #endregion // GrowBranch


        #region PopulateMonsters
        /// <summary>
        /// For a completely generated level, populates the level
        /// with a random number of monsters
        /// </summary>
        /// <param name="l"></param>
        private static void PopulateMonsters(Level l)
        {
            // set one random monster
            for (int i = 0; i < l.Size; i++)
            {
                for (int j = 0; j < l.Size; j++)
                {
                    // dont put a monster on the end or start for now
                    if (i != l.End.X && j != l.End.Y
                        && i != l.Start.X && j != l.Start.Y)
                    {
                        // dont put monster too near to the start
                        List<Position> lp = new List<Position>(1);
                        lp.Add(l.Start);
                        if (GetDistances(i, j, lp)[0] > 3)
                        {
                            // check whether the field is accessible
                            if (l.IsFieldAccessible(i, j, typeof(Monster)))
                            {
                                // In addition a 10% chance to get a monster,
                                // the first one is free ;)
                                if (Game.RNG.NextDouble() < 0.1 || l.MonsterStarts.Count<1)
                                {
                                    Position n = new Position(i, j);
                                    l.AddMonster(n);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion // PopulateMonsters

        #region PopulateTreasures
        /// <summary>
        /// Randomly generates some treasure chests in the level,
        /// only on branch end positions though
        /// </summary>
        /// <param name="l">The level for which to generate treasures</param>
        private static void PopulateTreasures(Level l)
        {
            var b = l.Branches;

            bool genSingle = true;

            foreach (var branch in b)
            {
                Position last = branch.Last.Value;
                // just get a 40% chance to add a treausre chest
                // if we found a edge field
                var r = Game.RNG.NextDouble();
                if (r < 0.4 && l.IsEdgeField(last))
                {
                    l.SetField(last.X, last.Y, Field.Treasure);
                    // dont need to generate a treasure in the end
                    genSingle = false;
                }
            }
            // check whether we had at least one treasure
            // if not generate one
            if (genSingle && b.Count > 0)
            {
                Position last = b[b.Count - 1].Last.Value;
                l.SetField(last.X, last.Y, Field.Treasure);
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

                // check whether one of the moves is the end move, in that
                // case we simply take this one
                foreach (Position m in pmoves)
                {
                    // check whether we arrived at an edge
                    if (l.End.X == m.X && l.End.Y == m.Y)
                    {
                        proceed = false;
                        current = m;
                        break;
                    }
                }
                // in case we set the main position already, we will
                // not choose a move
                if (proceed)
                {
                    // choose a random move
                    current = ChooseField(l, pmoves, minDistMoveIdx);
                }
                // set the new field for the main path
                l.SetField(current, Field.Main);
                l.Main.AddLast(current);
            }
            // check whether we have a connected path 
            // and generate one if necessary
            CheckPathNaive(l);
        }
        #endregion // GeneratePath

        #region ChooseField
        /// <summary>
        /// Chooses the next field (during level generation) from a list of possible moves,
        /// given also the move with the minimal distance to the end
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pmoves"></param>
        /// <param name="endMinDist"></param>
        /// <returns></returns>
        private static Position ChooseField(Level level, List<Position> pmoves, int endMinDist)
        {
            // TODO make a random choice, slightly favored to the 
            // minDistMove
            double r = Game.RNG.NextDouble();
            Position minDistMove = pmoves[endMinDist];
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
                c = pmoves[0];
            }

            // extra chance for moves with only one neighbouring 
            // free field
            foreach (Position m in pmoves)
            {
                int nfree = CountNeighbourAccessFields(level, m);
                if (nfree < 2 && Game.RNG.NextDouble() < 0.5)
                {
                    c = m;
                    break;
                }
            }

            // extra chance for mindistmove
            if (Game.RNG.NextDouble() < 0.2)
            {
                c = minDistMove;
            }
            return c;
        }
        #endregion // ChooseField

        #region CountNeighbourAccessFields
        /// <summary>
        /// Counts the number of neighbouring accessible fields for a specific 
        /// position
        /// </summary>
        /// <param name="level"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        internal static int CountNeighbourAccessFields(Level level, Position m)
        {
            int c = 0;
            if (level.IsFieldAccessible(m.X, m.Y - 1, typeof(Player)))
            {
                c++;
            }
            if (level.IsFieldAccessible(m.X + 1, m.Y, typeof(Player)))
            {
                c++;
            }
            if (level.IsFieldAccessible(m.X - 1, m.Y, typeof(Player)))
            {
                c++;
            }
            if (level.IsFieldAccessible(m.X, m.Y + 1, typeof(Player)))
            {
                c++;
            }
            return c;
        }
        #endregion // CountNeighbourAccessFields

        #region GetNeighbourAccessFields
        /// <summary>
        /// Gets the fields next to a specific figher, which can be accessed by the fighter,
        /// i.e. free fields which are not blocked
        /// </summary>
        public static List<Position> GetNeighbourAccessFields(Level level, Fighter f, bool ignoreMonster = false)
        {
            Position pos = f.Position;
            return GetNeighbourAccessFields(level, f.Position, f.GetType(), ignoreMonster);
        }
        /// <summary>
        /// Gets the fields next to a specific position, which can be accessed by a fighter specified
        /// by the givne type        
        /// </summary>
        public static List<Position> GetNeighbourAccessFields(Level level, Position pos, Type t, bool ignoreMonster = false)
        {
            List<Position> result = new List<Position>();
            if (level.IsFieldAccessible(pos.X, pos.Y - 1, t, ignoreMonster))
            {
                result.Add(new Position(pos.X, pos.Y - 1));
            }
            if (level.IsFieldAccessible(pos.X + 1, pos.Y, t, ignoreMonster))
            {
                result.Add(new Position(pos.X + 1, pos.Y));
            }
            if (level.IsFieldAccessible(pos.X - 1, pos.Y, t, ignoreMonster))
            {
                result.Add(new Position(pos.X - 1, pos.Y));
            }
            if (level.IsFieldAccessible(pos.X, pos.Y + 1, t, ignoreMonster))
            {
                result.Add(new Position(pos.X, pos.Y + 1));
            }
            return result;
        }
        #endregion // GetNeighbourAccessFields

        #region GetMinDistMove
        /// <summary>
        /// Gets the move with the minimal distance to the reference
        /// from a list of possible moves
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="choices"></param>
        /// <returns></returns>
        internal static int GetMinDistMove(Position reference, List<Position> choices)
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
        #endregion // GetMinDistMove

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
            int r = Game.RNG.Next(0, 2);
            if (r == 0)
            {
                // left or right start
                r = Game.RNG.Next(0, 2);
                startY = minmax[r];
                // random point on vertical axis
                startX = Game.RNG.Next(0, maxPos + 1);

                // end point, left or right
                endY = minmax[1 - r];

                // set end vertical position
                if (startX >= maxPos / 2)
                {
                    // lower quartile
                    endX = Game.RNG.Next(0, (maxPos / 4) + 1);
                }
                else
                {
                    // upper quartile
                    endX = Game.RNG.Next(maxPos / 2 + maxPos / 4, maxPos + 1);
                }
            }
            else
            {
                // top or bottom start
                r = Game.RNG.Next(0, 2);
                startX = minmax[r];
                // random point on horizontal axis
                startY = Game.RNG.Next(0, maxPos + 1);

                // end point, top or bottom
                endX = minmax[1 - r];

                // set end horizontal position
                if (startY >= maxPos / 2)
                {
                    // lower quartile
                    endY = Game.RNG.Next(0, (maxPos / 4) + 1);
                }
                else
                {
                    // upper quartile
                    endY = Game.RNG.Next(maxPos / 2 + maxPos / 4, maxPos + 1);
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
                result[i] = Statics.GetDistance(x, y, m.X, m.Y);
            }
            return (result);
        }
        #endregion // GetDistances

        #region GenerateNaive
        /// <summary>
        /// Generates a very naive level for initial testing purposes
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
