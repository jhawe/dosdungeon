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
        private Position nextMove = null;

        private Stopwatch stopWatch;

        readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 120);
        readonly TimeSpan MaxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);
        private TimeSpan lastTime;

        // random number generator used for alls random processes in the game
        internal static Random RNG = new Random();

        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gf">The main game's form</param>
        /// <param name="sw">The timer stopwatch</param>
        internal Game(GameForm gf, Stopwatch sw)
        {
            // initialize game variables
            this.view = new NaiveView(gf);
            this.stopWatch = sw;
            this.player = new Player("Hans");
            
            // generate first level
            this.level = Level.GenerateLevel(16);
            int startX = level.Start.X;
            int startY = level.Start.Y;

            // set player on the current board
            Position m = new Position(startX, startY);
            this.player.Move(m);
            this.level.SetPlayerPos(this.player.PosX, this.player.PosY);
        }
        #endregion // Constructor

        #region Methods

        #region Update
        /// <summary>
        /// Main Update loop function (as eventhandler of the main timer)
        /// for the game
        /// </summary>
        /// <param name="sender">The Time Senderobject</param>
        /// <param name="e">The Eventarguments</param>
        internal void Update(object sender, EventArgs e)
        {
            if (this.state == GameState.Running)
            {
                TimeSpan currentTime = stopWatch.Elapsed;
                TimeSpan elapsedTime = currentTime - lastTime;

                // only update after 0.5 seconds
                if (elapsedTime > TimeSpan.FromSeconds(0.3))
                {
                    lastTime = currentTime;
                    // update data
                    UpdateModels();

                    // update view
                    view.Update(this.level);
                }
                else
                {
                    // register a move to be executed in the next update
                    RegisterMove();
                }
            }

        }
        #endregion // Update

        #region RegisterMove
        /// <summary>
        /// Register a move to be executed during the next update
        /// </summary>
        private void RegisterMove()
        {
            Position m = GetMove(this.player);
            if(m != null && IsValidMove(m, this.level))
            {
                this.nextMove = m;
            }
        }
        #endregion // RegisterMove

        #region UpdateModels
        /// <summary>
        /// Update the current models (player and board)
        /// </summary>
        private void UpdateModels()
        {
            MovePlayer();            
        }
        #endregion // UpdateModels

        #region MovePlayer
        private void MovePlayer()
        {
            Position m;
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
        #endregion // MovePlayer

        #region IsValidMove
        /// <summary>
        /// Checks whether a move to be executed is valid
        /// </summary>
        /// <param name="m">The move to be executed</param>
        /// <param name="l">The current level</param>
        /// <returns>True if move is valid, otherwise false</returns>
        private bool IsValidMove(Position m, Level l)
        {
            Field f = l.GetField(m.X, m.Y);
            if (f != Field.Blocked && f != Field.NA)
            {
                return true;
            }
            return false;
        }
        #endregion // IsValidMove

        #region MakeMove
        /// <summary>
        /// Executes a move of the player on the field
        /// </summary>
        /// <param name="m">The move</param>
        /// <param name="p">The player</param>
        /// <param name="l">The board/field</param>
        private void MakeMove(Position m, Player p, Level l)
        {
            p.Move(m);
            l.SetPlayerPos(p.PosX, p.PosY);
        }
        #endregion // MakeMove

        #region GetMove
        /// <summary>
        /// Gets a move from the current player and keys pressed
        /// by the user
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Position GetMove(Player p)
        {
            if (Keyboard.IsKeyDown(Key.Left))
            {

                return new Position(p.PosX, p.PosY - 1);
            }
            else if (Keyboard.IsKeyDown(Key.Right))
            {

                return new Position(p.PosX, p.PosY + 1);
            }
            else if (Keyboard.IsKeyDown(Key.Up))
            {

                return new Position(p.PosX - 1, p.PosY);
            }
            else if (Keyboard.IsKeyDown(Key.Down))
            {

                return new Position(p.PosX + 1, p.PosY);
            }
            return null;
        }
        #endregion // GetMove

        #endregion // Methods
    }
}
