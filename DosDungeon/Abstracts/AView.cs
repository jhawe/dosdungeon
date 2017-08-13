using DosDungeon.Models;
using System;

namespace DosDungeon.Interfaces
{
    internal abstract class AView
    {
        internal static AView Create(GameForm form)
        {
            throw new NotImplementedException();
        }

        abstract internal void Update(Level level, Player player);        
    }
}
