using DosDungeon.Models;
using DosDungeon.Interfaces;
using System;
using System.Text;

namespace DosDungeon.Views
{
    internal class NaiveView : AView
    {
        const char BORDER = '#';
        const char PLAYER = 'P';
        const char MONSTER = 'M';
        const char BLOCKED = 'B';
        const char FREE = ' ';
        const char END = 'E';
        const char TREASURE = 'T';

        private GameForm gameForm = null;
        private Level level;

        #region Implement IView

        #region Create
        /// <summary>
        /// Creates a new view instance
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        internal static new AView Create(GameForm gf, Level level)
        {
            NaiveView view = new NaiveView();
            view.level = level;
            view.gameForm = gf;
            return (view);
        }
        #endregion // Create

        #region Update
        /// <summary>
        /// Updates the DOS-like view 
        /// </summary>
        /// <param name="level">The currently played lavel</param>
        /// <param name="player">The current player instance</param>
        internal override void Update(Player player)
        {
            GameForm gf = this.gameForm;
            gf.Board.Clear();
            // 'redraw'
            StringBuilder sb = new StringBuilder();

            // show summary screen of level
            if (this.level.IsFinished)
            {
                sb.AppendLine("Congratulations! You finished the level!");
                sb.AppendLine("Total Gold: " + player.Gold);
                sb.AppendLine("Total Health: " + player.Health);
                sb.AppendLine("Press [ENTER] to load the next level.");
            }
            else
            {
                // upper border
                var b = new char[this.level.Size + 2];
                for (int i = 0; i < b.Length; i++) b[i] = BORDER;
                sb.AppendLine(new string(b));

                // board
                for (int i = 0; i < this.level.Size; i++)
                {
                    var line = new char[this.level.Size + 2];
                    line[0] = BORDER;
                    line[line.Length - 1] = BORDER;
                    for (int j = 0; j < this.level.Size; j++)
                    {
                        var fc = '#';
                        if (i == this.level.End.X && j == this.level.End.Y)
                        {
                            fc = END;
                        }
                        else
                        {
                            Field f = this.level.GetField(i, j);
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

        #endregion // Implement IView

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
