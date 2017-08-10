using DosDungeon.Interfaces;
using DosDungeon.Models;
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
        private Graphics graphics;
        private Level level;
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

        internal static new AView Create(GameForm form, Level level)
        {            
            GraphicalView view = new GraphicalView();
            view.level = level;
            // we expected a quadratic view for now
            view.FIELD_SIZE = (int)Math.Floor(form.gameView.Size.Height / view.level.Size * 1.0);

            view.form = form;
            view.graphics = form.gameView.CreateGraphics();
            return (view);
        }

        internal override void Update(Player player)
        {
            // draw the field
            this.graphics.Clear(BACKGROUND);

            // show summary screen of level
            if (this.level.IsFinished)
            {
              // TODO: show level summary
            }
            else
            {   
                // board
                for (int i = 0; i < this.level.Size; i++)
                {                    
                    for (int j = 0; j < this.level.Size; j++)
                    {
                        Brush b = null;
                        // set appropriate pen
                        if (i == this.level.End.X && j == this.level.End.Y)
                        {
                            b = new SolidBrush(COLOR_END);
                        }
                        else
                        {
                            Field f = level.GetField(i, j);
                            Color c = GetColor(f);
                            b = new SolidBrush(c);
                         
                        }                        
                        // draw on current position with the chosen pen
                        Rectangle rect = new Rectangle(j*FIELD_SIZE, i* FIELD_SIZE, 
                            FIELD_SIZE, FIELD_SIZE);
                        this.graphics.FillRectangle(b, rect);
                    }                    
                }            
            }
            this.form.Invalidate();
        }
        #endregion // Implement IView

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
    }
}
