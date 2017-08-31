using DosDungeon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Common
{
    /// <summary>
    /// Some static function definitions
    /// </summary>
    public static class Statics
    {
        #region GetDistance
        /// <summary>
        /// Gets euclidean distance between to points
        /// </summary>
        /// <returns></returns>
        internal static float GetDistance(int x1, int y1, int x2, int y2)
        {
            float dx = x1 - x2;
            float dy = y1 - y2;

            // euclidian distance
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        internal static float GetDistance(Position p1, Position p2)
        {
            return (GetDistance(p1.X, p1.Y, p2.X, p2.Y));
        }
        #endregion // GetDistance

        #region Adjacent
        /// <summary>
        /// Checks whether to positions are adjacent to each other, 
        /// not diagonal!
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        public static bool Adjacent(Position position1, Position position2)
        {
            return Statics.GetDistance(position1, position2) == 1;
        }
        #endregion // Adjacent

        #region SameField
        /// <summary>
        /// Checks whether two positions are identical based on 
        /// X and Y coordinates
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool SameField(Position p1, Position p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        } 
        #endregion // SameField

        #region IsValidMove
        /// <summary>
        /// Checks whether a move to be executed is valid
        /// </summary>
        /// <param name="m">The move to be executed</param>
        /// <param name="l">The current level</param>
        /// <returns>True if move is valid, otherwise false</returns>
        public static bool IsValidMove(Position m, Level l, Fighter f)
        {
            if (l.IsFieldAccessible(m.X, m.Y, f.GetType())
                && GetMoveDirection(f.Position, m) == f.Face)
            {
                return true;
            }
            return false;
        }
        #endregion // IsValidMove

        #region GetMoveDirection
        /// <summary>
        /// Gets the direction in which a specific move is made
        /// </summary>
        /// <param name="from">the postition from which to move</param>
        /// <param name="to">the position to which to move</param>
        /// <returns></returns>
        public static Direction GetMoveDirection(Position from, Position to)
        {
            if (from.X < to.X)
            {
                return Direction.Down;
            }
            else if (from.X > to.X)
            {
                return Direction.Up;
            }
            else if (from.Y < to.Y)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        } 
        #endregion // GetMoveDirection

        #region IsTurn
        /// <summary>
        /// Checks whether the move to a specific position p would indicate
        /// a change of the face of the player
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsTurn(Position playerPos, Direction pFace, Position p)
        {
            return p != null && GetMoveDirection(playerPos, p) != pFace;
        }
        #endregion // IsTurn
    }
}
