﻿using DosDungeon.Common;
using DosDungeon.Interfaces;
using DosDungeon.Models;
using DosDungeon.Views;
using System;
using System.Collections.Generic;
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
        private AView view = null;
        private Player player = null;
        private Level level = null;
        private Position nextMove = null;
        private int levelSize = 16;
        private Stopwatch stopWatch;
        readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 120);
        readonly TimeSpan MaxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);
        private TimeSpan lastTime;
        private List<Monster> monster;
        // random number generator used for alls random processes in the game
        internal static Random RNG = new Random();
        internal static int COUNT_LEVEL = 1;
        private bool enterDown;
        private bool attackDown;

        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gf">The main game's form</param>
        /// <param name="sw">The timer stopwatch</param>
        internal Game(GameForm gf, Stopwatch sw)
        {
            this.stopWatch = sw;
            this.player = new Player("Hans");
            this.monster = new List<Monster>();

            InitLevel(this.levelSize);

            // initialize game view            
            this.view = GraphicalView.Create(gf);           
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
            this.level = LevelGenerator.GenerateLevel(this.levelSize);
            int startX = level.Start.X;
            int startY = level.Start.Y;

            // set player on the current board
            Position m = new Position(startX, startY);
            this.player.SetPosition(m);
            this.level.SetPlayerPos(this.player.Position);

            // foreach monster field, we create a monster instance to
            // be able to manipulate them
            // reset monster
            this.monster = new List<Monster>();
            foreach (Position ms in this.level.MonsterStarts)
            {
                Monster mo = new Monster(1);
                mo.SetPosition(ms);
                this.monster.Add(mo);
            }

            // set the player facing to one of the next free fields
            if (this.level.IsFieldAccessible(m.X + 1, m.Y))
            {
                this.player.SetFace(GetMoveDirection(m, new Position(m.X + 1, m.Y)));
            }
            else if (this.level.IsFieldAccessible(m.X - 1, m.Y))
            {
                this.player.SetFace(GetMoveDirection(m, new Position(m.X - 1, m.Y)));
            }
            else if (this.level.IsFieldAccessible(m.X, m.Y + 1))
            {
                this.player.SetFace(GetMoveDirection(m, new Position(m.X, m.Y + 1)));
            }
            else if (this.level.IsFieldAccessible(m.X, m.Y - 1))
            {
                this.player.SetFace(GetMoveDirection(m, new Position(m.X, m.Y - 1)));
            }
        }
        #endregion // InitLevel

        internal Direction GetMoveDirection(Position from, Position to)
        {
            if (from.X < to.X)
            {
                return Direction.Down;
            }
            else if (from.X > to.X)
            {
                return Direction.Up;
            }
            else if (from.Y < to.Y)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        }

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
                    this.view.Update(this.level, this.player);

                    // check whether the player finished
                    if (this.level.End.X == this.player.Position.X
                        && this.level.End.Y == this.player.Position.Y)
                    {
                        this.state = GameState.LevelFinished;
                        this.level.State = GameState.LevelFinished;
                    }
                }
                else if (this.state == GameState.LevelFinished)
                {
                    if (this.enterDown || Keyboard.IsKeyDown(Key.Enter))
                    {
                        this.enterDown = false;
                        InitLevel(this.levelSize);
                        this.state = GameState.Running;
                        this.level.State = GameState.Running;
                        COUNT_LEVEL++;
                    }
                    this.view.Update(this.level, this.player);
                }
                else if (this.state == GameState.GameOver)
                {
                    if (this.enterDown || Keyboard.IsKeyDown(Key.Enter))
                    {
                        COUNT_LEVEL = 1;
                        this.enterDown = false;
                        InitLevel(this.levelSize);
                        this.state = GameState.Running;
                        this.level.State = this.state;
                    }
                    this.view.Update(this.level, this.player);
                }
            }
            else
            {
                //this.view.Update(this.level, this.player);
                // register currently pressed keys only
                RegisterKeyDown();
                this.view.Update(this.level, this.player);
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
            this.attackDown = Keyboard.IsKeyDown(Key.Space);

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
            PlayerAction();
            MoveMonster();
        }
        #endregion // UpdateModels

        #region MoveMonster
        /// <summary>
        /// Moves monsters around the current level
        /// </summary>
        private void MoveMonster()
        {
            foreach (Monster m in this.monster)
            {
                // check whether we attack the player
                // if monster moves to player -> attack player; dont move
                if (Statics.Adjacent(this.player.Position, m.Position))
                {
                    AttackPlayer(m);
                    continue;
                }
                // now just move the monster
                List<Position> lp = LevelGenerator.GetNeighbourAccessFields(this.level, m.Position);

                // should only very rarely be the case
                if (lp.Count < 1)
                {
                    // do nothing continue
                }
                if (Game.RNG.NextDouble() <= 0.2)
                {
                    // move towards player
                    int minDist = LevelGenerator.GetMinDistMove(this.player.Position, lp);
                    Position np = lp[minDist];

                    // check face of the monster
                    if (m.Face == GetMoveDirection(m.Position, np))
                    {
                        // move the monster
                        // reset the field the monster was previously on
                        this.level.SetField(m.Position, Field.Free);
                        m.SetPosition(lp[minDist]);
                        this.level.SetField(m.Position, Field.Monster);
                    }
                    else
                    {
                        // just set the new face
                        m.SetFace(GetMoveDirection(m.Position, np));
                    }
                }
            }
        }
        #endregion // MoveMonster

        #region AttackPlayer
        /// <summary>
        /// Let's a monster attack a player.
        /// </summary>
        /// <param name="m">The monster attacking the player.</param>
        private void AttackPlayer(Monster m)
        {
            // hits with probability of 66%
            if (RNG.NextDouble() <= 0.66)
            {
                // just reduce health
                // TODO adjust amount by level/strength of monster?
                this.player.HealthDown(1);
                if(this.player.Health == 0)
                {
                    this.state = GameState.GameOver;
                    this.level.State = GameState.GameOver;
                }
            }
        }
        #endregion // AttackPlayer

        #region PlayerAction
        /// <summary>
        /// Let's the player instance perform an action besides moving
        /// </summary>
        private void PlayerAction()
        {
            // only current action is to attack
            if (this.attackDown | Keyboard.IsKeyDown(Key.Space))
            {
                this.attackDown = false;

                // one attack currently kills monsters
                Position af = this.player.AttackField;
                Monster rm = null;
                foreach (Monster m in this.monster)
                {
                    // check whether attack field and the current monster
                    // position overlap
                    if (af.X == m.Position.X && af.Y == m.Position.Y)
                    {
                        // remove monster from our list
                        rm = m;
                        this.level.SetField(af, Field.Free);
                        this.player.MonstersKilledUp();
                        int gold = (int)Math.Min(10, RNG.NextDouble() * 100);
                        this.player.GoldUp(gold);
                        break;
                    }
                }
                if (rm != null)
                {
                    this.monster.Remove(rm);
                }
            }
            else
            {
                MovePlayer();
            }
        }
        #endregion // PayerAction

        #region MovePlayer
        /// <summary>
        /// Move the player to a new field based on which arrow key is currently pressed
        /// </summary>
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
                else
                {
                    // not valid, but nevertheless change the
                    // direction the player is facing
                    Direction dir = GetMoveDirection(this.player.Position, m);
                    this.player.SetFace(dir);
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
            if (l.IsFieldAccessible(m.X, m.Y))
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
            Direction dir = GetMoveDirection(p.Position, m);
            if (p.Face == dir)
            {
                // set the new position
                p.SetPosition(m);
                // check whether we found a treasure
                if (l.GetField(p.Position.X, p.Position.Y) == Field.Treasure)
                {
                    OpenTreasure(p);
                }
                l.SetPlayerPos(p.Position);
            }
            else
            {
                // only change direction
                p.SetFace(dir);
            }
        }
        #endregion // MakeMove

        #region OpenTreasure
        /// <summary>
        /// Open a treasure for the specified player
        /// </summary>
        /// <param name="p">The player opening the treasure</param>
        private void OpenTreasure(Player p)
        {
            // either a heart or some gold?
            var r = Game.RNG.NextDouble();
            // heart has only 25% prob
            if (r <= 0.25)
            {
                p.HealthUp(1);
            }
            else
            {
                // give at least 10 gold, up to maximal 100
                r = Game.RNG.NextDouble();
                var amount = (int)Math.Ceiling(Math.Max(r * 100, 10));
                p.GoldUp(amount);
            }
        }
        #endregion // OpenTreasure

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
