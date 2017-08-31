using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DosDungeon.Common;
using DosDungeon.Models;
using System.Collections.Generic;

namespace DosDungeonTest
{
    [TestClass]
    public class LevelTest
    {
        [TestMethod]
        public void LevelGeneratorTest()
        {
            int lsize = 10;
            int nlevels = 1000;
            // create 100 random levels and check
            // whether they all have a complete path 
            // from start to end
            for (int i = 0; i < nlevels; i++)
            {
                Level l = LevelGenerator.GenerateLevel(lsize);
                bool hp = HasPath(l);
                Assert.IsTrue(hp);
            }
        }

        #region HasPath
        /// <summary>
        /// Checks whether a path exists in the level for the player to walk
        /// through. Idea is to just check whether each piece is adjacent to 
        /// at least two fields in the main path. 
        /// TODO furthermore, ensure that 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private bool HasPath(Level l)
        {
            // TODO
            // coloring approach: start at start point in level,
            // then gradually 'color' all neighbouring fields while not found
            // then end
            // if at some point all is colored and no more can be colored
            // --> not possible
            // but otherwise if the end gets colored as well -> fully functional path
            // in each iteration:
            // check neighbouring fields, color free fields which have not been visisted
            // mark the current list of fields as visited
            // add the newly colored fields as the fields for which to check neighbouring 
            // fields in the next iteration
            LinkedList<Position> mp = l.Main;
            Position[] m = new Position[mp.Count];
            mp.CopyTo(m, 0);

            for (int i = 0; i < m.Length; i++)
            {
                if (!AdjacentToAny(m[i], m)
                    || !l.IsFieldAccessible(m[i], typeof(Player), true))
                {
                    return false;
                }
                // last check
                if (i == m.Length - 1)
                {
                    if (Statics.SameField(m[i], l.End))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        } 
        #endregion // HasPath

        #region AdjacentToAny
        /// <summary>
        /// Just checks whether the provided position is adjacent to any in the list
        /// </summary>
        /// <param name="p"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        internal bool AdjacentToAny(Position p, Position[] list)
        {
            int cnt = 0;
            foreach (Position cur in list)
            {
                if (Statics.Adjacent(p, cur))
                {
                    cnt++;
                }
            }
            return cnt > 1;
        } 
        #endregion // AdjacentToAny
    }
}
