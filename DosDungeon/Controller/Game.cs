﻿using DosDungeon.Models;
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
        /// <summary>
        /// Class member
        /// </summary>
        private GameState state = GameState.Running;
        private NaiveView view = null;
        private Player player = null;
        private Level level = null;
        private Position nextMove = null;
        private int levelSize = 16;
        private Stopwatch stopWatch;
        readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 120);
        readonly TimeSpan MaxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);
        private TimeSpan lastTime;

        // random number generator used for alls random processes in the game
        internal static Random RNG = new Random();
        private bool enterDown;

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

            InitLevel(this.levelSize);
        }
        #endregion // Constructor

        #region Methods

        #region InitLevel
        /// <summary>
        /// Initializes a level of the specified size
        /// </summary>
        /// <param name="levelSize">The size of the level to init</param>
        private void InitLevel(int levelSize)
        {
            // generate first level
            this.level = Level.GenerateLevel(this.levelSize);
            int startX = level.Start.X;
            int startY = level.Start.Y;

            // set player on the current board
            Position m = new Position(startX, startY);
            this.player.Move(m);
            this.level.SetPlayerPos(this.player.Position);
        }
        #endregion // InitLevel

        #region Update
        /// <summary>
        /// Main Update loop function (as eventhandler of the main timer)
        /// for the game
        /// </summary>
        /// <param name="sender">The Time Senderobject</param>
        /// <param name="e">The Eventarguments</param>
        internal void Update(object sender, EventArgs e)
        {
            TimeSpan currentTime = stopWatch.Elapsed;
            TimeSpan elapsedTime = currentTime - lastTime;

            // only update after 0.5 seconds
            if (elapsedTime > TimeSpan.FromSeconds(0.3))
            {
                lastTime = currentTime;

                if (this.state == GameState.Running)
                {
                    // update data
                    UpdateModels();

                    // update view
                    this.view.Update(this.level);

                    // check whether the player finished
                    if (this.level.End.X == this.player.Position.X
                        && this.level.End.Y == this.player.Position.Y)
                    {
                        this.state = GameState.LevelFinished;
                        this.level.IsFinished = true;
                    }
                }
                else if (this.state == GameState.LevelFinished)
                {                    
                    if (this.enterDown)
                    {
                        this.enterDown = false;
                        InitLevel(this.levelSize);
                        this.state = GameState.Running;
                    }
                    this.view.Update(this.level);
                }
            }
            else
            {
                // register currently pressed keys only
                RegisterKeyDown();
            }

        }
        #endregion // Update

        #region RegisterKeyDown
        /// <summary>
        /// Register a move to be executed during the next update
        /// </summary>
        private void RegisterKeyDown()
        {
            this.enterDown = Keyboard.IsKeyDown(Key.Enter);

            Position m = GetMove(this.player);
            if (m != null && IsValidMove(m, this.level))
            {
                this.nextMove = m;
            }
        }
        #endregion // RegisterKeyDown

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

            l.SetPlayerPos(p.Position);

            // check whether we found a treasure
            if (l.GetField(p.Position.X, p.Position.Y) == Field.Treasure)
            {
                OpenTreasure();                
            }
        }
        #endregion // MakeMove

        private void OpenTreasure()
        {
            throw new NotImplementedException();
        }

        #region GetMove
        /// <summary>
        /// Gets a move from the current player and keys pressed
        /// by the user
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Position GetMove(Player p)
        {
            int x = p.Position.X;
            int y = p.Position.Y;

            if (Keyboard.IsKeyDown(Key.Left))
            {

                return new Position(x, y - 1);
            }
            else if (Keyboard.IsKeyDown(Key.Right))
            {

                return new Position(x, y + 1);
            }
            else if (Keyboard.IsKeyDown(Key.Up))
            {

                return new Position(x - 1, y);
            }
            else if (Keyboard.IsKeyDown(Key.Down))
            {

                return new Position(x + 1, y);
            }
            return null;
        }
        #endregion // GetMove

        #endregion // Methods
    }
}
