using DosDungeon.Models;
using System;
using System.Text;

namespace DosDungeon.Views
{
    internal class NaiveView
    {
        const char BORDER = '#';
        const char PLAYER = 'P';
        const char MONSTER = 'M';
        const char BLOCKED = 'B';
        const char FREE = ' ';
        const char END = 'E';
        const char TREASURE = 'T';

        private GameForm gameForm = null;

        internal NaiveView(GameForm gf)
        {
            this.gameForm = gf;
        }

        #region Update
        /// <summary>
        /// Updates the DOS-like view 
        /// </summary>
        /// <param name="level">The currently played lavel</param>
        /// <param name="player">The current player instance</param>
        internal void Update(Level level, Player player)
        {
            GameForm gf = this.gameForm;
            gf.Board.Clear();
            // 'redraw'
            StringBuilder sb = new StringBuilder();

            // show summary screen of level
            if (level.IsFinished)
            {
                sb.AppendLine("Congratulations! You finished the level!");
                sb.AppendLine("Total Gold: " + player.Gold);
                sb.AppendLine("Total Health: " + player.Health);
                sb.AppendLine("Press [ENTER] to load the next level.");
            }
            else
            {
                // upper border
                var b = new char[level.Size + 2];
                for (int i = 0; i < b.Length; i++) b[i] = BORDER;
                sb.AppendLine(new string(b));

                // board
                for (int i = 0; i < level.Size; i++)
                {
                    var line = new char[level.Size + 2];
                    line[0] = BORDER;
                    line[line.Length - 1] = BORDER;
                    for (int j = 0; j < level.Size; j++)
                    {
                        var fc = '#';
                        if (i == level.End.X && j == level.End.Y)
                        {
                            fc = END;
                        }
                        else
                        {
                            Field f = level.GetField(i, j);
                            fc = GetFieldChar(f);
                        }
                        line[j + 1] = fc;
                    }
                    sb.AppendLine(new string(line));
                }

                // lower boarder
                sb.AppendLine(new string(b));
            }
            gf.Board.Text = sb.ToString();
        }
        #endregion // Update

        #region GetFieldChar
        /// <summary>
        /// Gets the char to be used on the visual display for the corresponding
        /// field element
        /// </summary>
        /// <param name="f">The field for which to get the char representation</param>
        /// <returns>The field's char representation</returns>
        internal char GetFieldChar(Field f)
        {
            var fc = FREE;
            switch (f)
            {
                case Field.Blocked:
                    fc = BLOCKED;
                    break;
                case Field.Free:
                    fc = FREE;
                    break;
                case Field.Monster:
                    fc = MONSTER;
                    break;
                case Field.Player:
                    fc = PLAYER;
                    break;
                case Field.Branch:
                    fc = FREE;
                    break;
                case Field.Treasure:
                    fc = TREASURE;
                    break;
                default:
                    break;
            }
            return (fc);
        }
        #endregion // GetFieldChar
    }
}
