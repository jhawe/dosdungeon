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
        #region Test Methods
        /// <summary>
        /// assert the 'getneighbouraccessfields' method
        /// </summary>
        [TestMethod]
        public void NeighbourFieldsTest()
        {

        }

        /// <summary>
        /// assert level generation
        /// </summary>
        [TestMethod]
        public void LevelGeneratorTest()
        {
            int lsize = 10;
            int nlevels = 20000;
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
        #endregion // Test Methods

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
            // 'coloring' approach: start at start point in level,
            // then gradually 'color' all neighbouring fields while not found
            // then end
            // if at some point all is colored and no more can be colored
            // --> not possible, fail
            // but otherwise if the end gets colored as well -> fully functional path

            // in each iteration:
            // check neighbouring fields, color free fields which have not been visisted
            // mark the current list of fields as visited
            // add the newly colored fields as the fields for which to check neighbouring 
            // fields in the next iteration
            List<Position> visited = new List<Position>();
            List<Position> current = new List<Position>();
            current.Add(l.Start);

            while (true)
            {
                // mark current positions as visited
                visited.AddRange(current);

                // next list of 'current' positions
                List<Position> next = new List<Position>();

                foreach (Position p in current)
                {
                    List<Position> temp = LevelGenerator.GetNeighbourAccessFields(l, p, typeof(Player), true);
                    foreach (Position toAdd in temp)
                    {
                        // did we get the endfield?
                        // we then can immediately return
                        if (Statics.SameField(l.End, toAdd))
                        {
                            return true;
                        }
                        // only add if not yet visited and if it is not in the
                        // current list and if not yet added to the next field
                        if (!InList(toAdd, visited) && !InList(toAdd, current)
                            && !InList(toAdd, next))
                        {
                            next.Add(toAdd);
                        }
                    }
                }
                current = next;
                // no next fields and we did not find the end
                if (current.Count < 1)
                {
                    return false;
                }
            }
        }
        #endregion // HasPath

        #region InList
        /// <summary>
        /// Only checks whether a position with the same coordinates
        /// is already in a given list
        /// </summary>
        /// <param name="p"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private bool InList(Position p, List<Position> current)
        {
            foreach (Position p2 in current)
            {
                if (Statics.SameField(p, p2))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion // InList        
    }
}
