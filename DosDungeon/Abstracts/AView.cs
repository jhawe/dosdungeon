using DosDungeon.Models;
using System;

namespace DosDungeon.Interfaces
{
    internal abstract class AView
    {
        internal static AView Create(GameForm form, Level level)
        {
            throw new NotImplementedException();
        }

        abstract internal void Update(Player player);        
    }
}
