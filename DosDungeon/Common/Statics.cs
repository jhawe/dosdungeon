using DosDungeon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Common
{
    internal static class Statics
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
        
        internal static bool Adjacent(Position position1, Position position2)
        {
            return Statics.GetDistance(position1, position2) == 1;
        }

    }
}
