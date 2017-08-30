using DosDungeon.Common;
using DosDungeon.Controller;
using DosDungeon.Interfaces;
using DosDungeon.Models;
using DosDungeon.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Views
{
    class GraphicalView : AView
    {
        private GameForm form;
        // define the colors for the individual fields in the view
        // to be used instrad of real graphics for now
        private Color COLOR_PLAYER = Color.Blue;
        private Color COLOR_BLOCKED = Color.SandyBrown;
        private Color COLOR_TREASURE = Color.Brown;
        private Color COLOR_FREE = Color.Green;
        private Color COLOR_MONSTER = Color.Yellow;
        private Color COLOR_END = Color.Black;

        private Color BACKGROUND = Color.White;

        private int FIELD_SIZE;

        #region Implement IView

        #region Create
        /// <summary>
        /// Creates a new graphical view instance
        /// </summary>
        /// <param name="form">The base form of the game</param>
        /// <returns>A new AView (GraphicalView) instance</returns>
        internal static new AView Create(GameForm form)
        {
            GraphicalView view = new GraphicalView();
            view.form = form;
            return (view);
        }
        #endregion // Create

        #region Update
        /// <summary>
        /// Redraws the view for the given level and player information
        /// </summary>
        /// <param name="level"></param>
        /// <param name="player"></param>
        internal override void Update(Level level, Player player, List<Monster> mon)
        {
            if (this.form.gameView != null)
            {
                // get graphics and reset
                var graphics = Graphics.FromImage(this.form.gameView);
                graphics.Clear(BACKGROUND);

                // we expected a quadratic view for now
                this.FIELD_SIZE = (int)Math.Floor(form.gameView.Size.Height / level.Size * 1.0);

                // draw the field

                // show summary screen of level
                if (level.State == GameState.LevelFinished)
                {
                    // TODO: show level summary in a more sophisticated
                    // way
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Congratulations! You finished level " + Game.COUNT_LEVEL + "!");
                    sb.AppendLine("Total Gold: " + player.Gold);
                    sb.AppendLine("Total Health: " + player.Health);
                    sb.AppendLine("Total monsters killed: " + player.MonstersKilled);
                    sb.AppendLine("Press [ENTER] to load the next level.");

                    graphics.DrawString(sb.ToString(), new Font(FontFamily.GenericMonospace, 10),
                        new SolidBrush(Color.Red), 5, 5);
                }
                else if (level.State == GameState.GameOver)
                {
                    // TODO: show level summary in a more sophisticated
                    // way
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("YOU DIED! (Level " + Game.COUNT_LEVEL + ")");
                    sb.AppendLine("Total Gold: " + player.Gold);
                    sb.AppendLine("Total Health: " + player.Health);
                    sb.AppendLine("Total monsters killed: " + player.MonstersKilled);
                    sb.AppendLine("Press [ENTER] to start anew!");

                    graphics.DrawString(sb.ToString(), new Font(FontFamily.GenericMonospace, 10),
                        new SolidBrush(Color.Red), 5, 5);
                }
                else
                {
                    // board
                    for (int i = 0; i < level.Size; i++)
                    {
                        for (int j = 0; j < level.Size; j++)
                        {
                            // draw on current position with the chosen pen
                            Rectangle rect = new Rectangle(j * FIELD_SIZE, i * FIELD_SIZE,
                                FIELD_SIZE, FIELD_SIZE);

                            Field f = level.GetField(i, j);
                            Image img;

                            // set appropriate pen
                            if (i == level.End.X && j == level.End.Y)
                            {
                                img = Resources.End;
                            }
                            else
                            {
                                // if we have a fighter field, we need to get the current
                                // face of the fighter
                                Fighter fi = null;
                                if (f == Field.Monster && mon != null)
                                {
                                    foreach (Monster m in mon)
                                    {
                                        if (Statics.SameField(m.Position, new Position(i, j)))
                                        {
                                            fi = m;
                                            break;
                                        }
                                    }
                                }
                                else if (f == Field.Player)
                                {
                                    fi = player;
                                }
                                // set a default direction (e.g. non-init monsters)
                                Direction d = Direction.Down;
                                if (fi != null)
                                {
                                    d = fi.Face;
                                }
                                img = GetImage(f, d);
                            }
                            // draw the current field
                            graphics.DrawImage(img, rect);
                        }
                    }
                    DrawHUD(graphics, player);
                }
                this.form.Invalidate();
            }
        }
        #endregion // Update

        #endregion // Implement IView

        private void DrawHUD(Graphics graphics, Player player)
        {
            // size of the heart to be drawn
            int s = 20;

            // start point of heart containers, top left
            int sx = 5;
            int sy = 5;

            // draw exactly player.Health hearts
            for (int i = 0; i < Player.MAXHEALTH; i++)
            {
                if (i < player.Health)
                {
                    graphics.DrawImage(Resources.heart, new Rectangle(sx + i * s, sy, s, s));
                }
                else
                {
                    graphics.DrawImage(Resources.heart_empty, new Rectangle(sx + i * s, sy, s, s));
                }
            }

            // display collected gold just below hearts
            sx = 5;
            sy += (s + 5);
            graphics.DrawImage(Resources.coins, new Rectangle(sx, sy, s, s));
            graphics.DrawString(player.Gold.ToString(), new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold),
                new SolidBrush(Color.WhiteSmoke), sx + s + 5, sy);

        }

        /// <summary>
        /// Gets the image to be drawn for the specified field
        /// </summary>
        /// <param name="f">the field</param>
        /// <param name="pf">If the field is the player or a monster, then we need the direction
        /// this entity is facing in order to get the right image</param>
        /// <returns></returns>
        private Image GetImage(Field f, Direction d)
        {
            Image img = null;

            switch (f)
            {
                case Field.Player:
                    switch (d)
                    {
                        case Direction.Down:
                            img = Resources.Player_Down;
                            break;
                        case Direction.Up:
                            img = Resources.Player_Up;
                            break;
                        case Direction.Left:
                            img = Resources.Player_Left;
                            break;
                        case Direction.Right:
                            img = Resources.Player_Right;
                            break;
                        default: break;
                    }
                    break;
                case Field.Monster:
                    switch (d)
                    {
                        case Direction.Down:
                            img = Resources.Monster_Down;
                            break;
                        case Direction.Up:
                            img = Resources.Monster_Up;
                            break;
                        case Direction.Left:
                            img = Resources.Monster_Left;
                            break;
                        case Direction.Right:
                            img = Resources.Monster_Right;
                            break;
                        default: break;
                    }
                    break;
                case Field.Treasure:
                    img = Resources.Chest;
                    break;
                case Field.Blocked:
                    img = Resources.Grass;
                    break;
                case Field.Branch:
                case Field.Free:
                case Field.Main:
                    img = Resources.Sand;
                    break;
                default:
                    break;
            }
            return img;
        }

        #region GetColor
        /// <summary>
        /// Gets the representative color for a specific field
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private Color GetColor(Field f)
        {
            Color c = Color.White;

            switch (f)
            {
                case Field.Player:
                    c = COLOR_PLAYER;
                    break;
                case Field.Monster:
                    c = COLOR_MONSTER;
                    break;
                case Field.Treasure:
                    c = COLOR_TREASURE;
                    break;
                case Field.Blocked:
                    c = COLOR_BLOCKED;
                    break;
                case Field.Branch:
                case Field.Free:
                case Field.Main:
                    c = COLOR_FREE;
                    break;
                default:
                    break;
            }
            return c;
        }
        #endregion // GetColor
    }
}
