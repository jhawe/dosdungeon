using DosDungeon.Models;
using System;
using System.Collections.Generic;

namespace DosDungeon.Interfaces
{
    /// <summary>
    /// Abstract class to describe views which can be used 
    /// within the game
    /// </summary>
    internal abstract class AView
    {
        #region Create
        /// <summary>
        /// Creates a new view
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        internal static AView Create(GameForm form)
        {
            throw new NotImplementedException();
        }
        #endregion // Create

        #region Update
        /// <summary>
        /// Updates the current view based on the provided level, player and monsters
        /// </summary>
        /// <param name="level"></param>
        /// <param name="player"></param>
        /// <param name="monster"></param>
        abstract internal void Update(Level level, Player player, List<Monster> monster);         
        #endregion // Update
    }
}
