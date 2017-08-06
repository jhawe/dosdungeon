using DosDungeon.Models;
using DosDungeon.Views;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;

namespace DosDungeon.Controller
{
    internal class Game
    {
        #region Class Member
        private GameState state = GameState.Running;
        private NaiveView view = null;
        private Player player;
        private Level level;
        private Move nextMove = null;

        private Stopwatch stopWatch;

        readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 120);
        readonly TimeSpan MaxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);
        private TimeSpan lastTime;
        
        #endregion // Class Member

        internal Game(GameForm gf, Stopwatch sw)
        {
            this.view = new NaiveView(gf);
            this.stopWatch = sw;
            // generate first level
            this.level = Level.GenerateLevel(16);
            int startX = level.StartX;
            int startY = level.StartY;

            this.player = new Player("Hans");
            Move m = new Move(startX, startY);
            this.player.Move(m);
            this.level.SetPlayerPos(this.player.PosX, this.player.PosY);
        }
        #region Methods

        internal void Update(object sender, EventArgs e)
        {
            if (this.state == GameState.Running)
            {
                TimeSpan currentTime = stopWatch.Elapsed;
                TimeSpan elapsedTime = currentTime - lastTime;

                if (elapsedTime > TimeSpan.FromSeconds(0.5))
                {
                    lastTime = currentTime;
                    // update data
                    UpdateModels();
                }
                else
                {
                    RegisterMove();
                }

                // update view
                view.Update(this.level);
            }

        }

        private void RegisterMove()
        {
            Move m = GetMove(this.player);
            if(m != null && IsValidMove(m, this.level))
            {
                this.nextMove = m;
            }
        }

        private void UpdateModels()
        {
            Move m;
            // check for registered move first
            if (this.nextMove != null)
            {
                m = this.nextMove;
                this.nextMove = null;
            }
            else
            {
                m = GetMove(this.player);
            }
            if (m != null)
            {
                if (IsValidMove(m, this.level))
                {
                    MakeMove(m, this.player, this.level);
                }
            }
        }

        private bool IsValidMove(Move m, Level l)
        {
            Field f = l.GetField(m.X, m.Y);
            if (f == Field.Free)
            {
                return true;
            }
            return false;
        }

        private void MakeMove(Move m, Player p, Level l)
        {
            p.Move(m);
            l.SetPlayerPos(p.PosX, p.PosY);
        }

        private Move GetMove(Player p)
        {
            if (Keyboard.IsKeyDown(Key.Left))
            {

                return new Move(p.PosX, p.PosY - 1);
            }
            else if (Keyboard.IsKeyDown(Key.Right))
            {

                return new Move(p.PosX, p.PosY + 1);
            }
            else if (Keyboard.IsKeyDown(Key.Up))
            {

                return new Move(p.PosX - 1, p.PosY);
            }
            else if (Keyboard.IsKeyDown(Key.Down))
            {

                return new Move(p.PosX + 1, p.PosY);
            }
            return null;
        }

        #endregion // Methods
    }
}
