using DosDungeon.Common;
using DosDungeon.Interfaces;
using DosDungeon.Models;
using DosDungeon.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

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
        private int levelSize = 10;
        private Stopwatch stopWatch;
        readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 120);
        readonly TimeSpan MaxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);
        private TimeSpan lastTime;
        private List<Monster> monster;

        // music
        //private SoundPlayer sp = null;
        // sfx
        private SoundPlayer sfx = null;

        // random number generator used for alls random processes in the game
        internal static Random RNG = new Random();
        internal static int COUNT_LEVEL = 1;
        private bool enterDown;
        private bool attackDown;
        private bool sfxOn;

        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gf">The main game's form</param>
        /// <param name="sw">The timer stopwatch</param>
        internal Game(GameForm gf, Stopwatch sw)
        {
            // start music
            // this.sp = new SoundPlayer(Properties.Resources.bg_music);
            // this.sp.PlayLooping();
            this.sfxOn = true;

            // member init
            this.sfx = new SoundPlayer();
            this.stopWatch = sw;
            this.player = new Player("TheH3ro");
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
        private void InitLevel(int levelSize, bool resetPlayer = false)
        {
            // generate first level
            this.level = LevelGenerator.GenerateLevel(this.levelSize);
            int startX = level.Start.X;
            int startY = level.Start.Y;

            if (resetPlayer)
            {
                // reset player stats
                this.player.Health = 5;
                this.player.Gold = 0;
                this.player.MonstersKilled = 0;
            }
            // set player on the current board
            Position m = new Position(startX, startY);
            this.player.SetPosition(m);
            this.level.SetFighter(m, null, typeof(Player));

            // foreach monster field, we create a monster instance to
            // be able to manipulate them
            // reset monster
            this.monster = new List<Monster>();
            foreach (Position ms in this.level.MonsterStarts)
            {
                Monster mo = new Monster(1);
                mo.SetPosition(ms);
                SetInitFace(mo, ms);
                this.monster.Add(mo);
            }

            SetInitFace(this.player, m);
        }
        #endregion // InitLevel

        private void SetInitFace(Fighter f, Position m)
        {
            // set the player facing to one of the next free fields
            if (this.level.IsFieldAccessible(m.X + 1, m.Y, f.GetType()))
            {
                f.SetFace(GetMoveDirection(m, new Position(m.X + 1, m.Y)));
            }
            else if (this.level.IsFieldAccessible(m.X - 1, m.Y, f.GetType()))
            {
                f.SetFace(GetMoveDirection(m, new Position(m.X - 1, m.Y)));
            }
            else if (this.level.IsFieldAccessible(m.X, m.Y + 1, f.GetType()))
            {
                f.SetFace(GetMoveDirection(m, new Position(m.X, m.Y + 1)));
            }
            else if (this.level.IsFieldAccessible(m.X, m.Y - 1, f.GetType()))
            {
                f.SetFace(GetMoveDirection(m, new Position(m.X, m.Y - 1)));
            }
        }

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

        #region ToggleSound
        /// <summary>
        /// toggle playing of sound effects
        /// </summary>
        internal void ToggleSound()
        {
            this.sfxOn = !this.sfxOn;
        }
        #endregion // ToggleSound

        #region PlaySound
        /// <summary>
        ///  play a single sound effect provided as a stream
        /// </summary>
        /// <param name="s"></param>
        internal void PlaySound(Stream s)
        {
            if (this.sfxOn)
            {
                this.sfx.Stream = s;
                this.sfx.Play();
            }
        }
        #endregion // PlaySound

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

            // check on control keys
            CheckControlKeys();

            // register currently pressed action keys
            RegisterKeyDown();

            // immediately check whether we have a turn
            Position m = GetMove(this.player);
            if (IsTurn(m))
            {
                this.nextMove = m;
                // always instantly set the new direction
                // the player is facing
                MoveFighter(this.player);                
                System.Threading.Thread.Sleep(100);
            }

            // only update after 0.3 seconds
            if (elapsedTime > TimeSpan.FromSeconds(0.3))
            {                
                this.nextMove = m;
                lastTime = currentTime;

                if (this.state == GameState.Running)
                {
                    // update data
                    UpdateModels();

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
                    if (this.enterDown)
                    {
                        this.enterDown = false;
                        InitLevel(this.levelSize);
                        this.state = GameState.Running;
                        this.level.State = GameState.Running;
                        COUNT_LEVEL++;
                    }
                }
                else if (this.state == GameState.GameOver)
                {
                    if (this.enterDown)
                    {
                        COUNT_LEVEL = 1;
                        this.enterDown = false;
                        InitLevel(this.levelSize, true);
                        this.state = GameState.Running;
                        this.level.State = this.state;
                    }
                }
            }

            // update the view (draw new image)
            this.view.Update(this.level, this.player, this.monster);
        }
        #endregion // Update

        #region IsTurn
        /// <summary>
        /// Checks whether the move to a specific position p would indicate
        /// a change of the face of the player
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsTurn(Position p)
        {
            return p != null && GetMoveDirection(this.player.Position, p) != player.Face;
        }
        #endregion // IsTurn

        #region RegisterKeyDown
        /// <summary>
        /// Register a move to be executed during the next update
        /// </summary>
        private void RegisterKeyDown()
        {
            this.enterDown = this.enterDown|Keyboard.IsKeyDown(Key.Enter);
            this.attackDown = this.attackDown|Keyboard.IsKeyDown(Key.Space);

            Position m = GetMove(this.player);
            if (m != null && this.nextMove == null)
            {
                //this.nextMove = m;
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
            MonsterAction();
        }
        #endregion // UpdateModels

        #region MonsterAction
        /// <summary>
        /// Moves monsters around the current level
        /// </summary>
        private void MonsterAction()
        {
            foreach (Monster m in this.monster)
            {
                // check whether we can attack the player
                // if monster moves to player -> attack player; dont move
                if (Statics.SameField(this.player.Position, m.AttackField))
                {
                    AttackPlayer(m);
                    continue;
                }
                // perform some movement with 20%prob
                if (Game.RNG.NextDouble() <= 0.2)
                {
                    MoveFighter(m);
                }
            }
        }
        #endregion // MonsterAction

        #region AttackPlayer
        /// <summary>
        /// Let's a monster attack a player.
        /// </summary>
        /// <param name="m">The monster attacking the player.</param>
        private void AttackPlayer(Monster m)
        {
            // hits with probability of 50%
            if (RNG.NextDouble() <= 0.4)
            {
                // just reduce health
                // TODO adjust amount by level/strength of monster?
                // TODO make player invincable for a short time
                this.player.HealthDown(1);
                if (this.player.Health == 0)
                {
                    this.state = GameState.GameOver;
                    this.level.State = GameState.GameOver;
                }
                PlaySound(Properties.Resources.ouch);
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
            if (this.attackDown)
            {
                this.attackDown = false;
                // one attack currently kills monsters
                Position af = this.player.AttackField;
                Monster rm = null;
                foreach (Monster m in this.monster)
                {
                    // check whether attack field and the current monster
                    // position overlap
                    if (Statics.SameField(af, m.Position))
                    {
                        // remove monster from our list
                        rm = m;
                        this.level.SetField(af, Field.Free);
                        this.player.MonstersKilledUp();
                        int gold = (int)Math.Min(10, RNG.NextDouble() * 100);
                        this.player.GoldUp(gold);
                        PlaySound(Properties.Resources.ogre);
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
                MoveFighter(this.player);
            }
        }
        #endregion // PayerAction

        #region MoveFighter
        /// <summary>
        /// Move the player to a new field based on which arrow key is currently pressed
        /// </summary>
        private void MoveFighter(Fighter f)
        {
            Position m = null;
            // check for registered move of player
            if (this.nextMove != null && f.GetType().Equals(typeof(Player)))
            {
                m = this.nextMove;
                this.nextMove = null;
            }
            else if (f.GetType().Equals(typeof(Monster)))
            {
                // just move the monster
                List<Position> lp = LevelGenerator.GetNeighbourAccessFields(this.level, f);
                if (lp.Count > 0)
                {
                    // move towards player
                    int minDist = LevelGenerator.GetMinDistMove(this.player.Position, lp);
                    m = lp[minDist];
                }
            }
            if (m != null)
            {
                // check for valid move (free field and not against current face)
                if (IsValidMove(m, this.level, f))
                {
                    MakeMove(m, f, this.level);
                }
                else
                {
                    // not valid, but nevertheless change the
                    // direction the player is facing
                    Direction dir = GetMoveDirection(f.Position, m);
                    f.SetFace(dir);
                }
            }
        }
        #endregion // MoveFighter

        #region IsValidMove
        /// <summary>
        /// Checks whether a move to be executed is valid
        /// </summary>
        /// <param name="m">The move to be executed</param>
        /// <param name="l">The current level</param>
        /// <returns>True if move is valid, otherwise false</returns>
        private bool IsValidMove(Position m, Level l, Fighter f)
        {
            if (l.IsFieldAccessible(m.X, m.Y, f.GetType())
                && GetMoveDirection(f.Position, m) == f.Face)
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
        /// <param name="f">The player</param>
        /// <param name="l">The board/field</param>
        private void MakeMove(Position m, Fighter f, Level l)
        {
            Direction dir = GetMoveDirection(f.Position, m);
            Position oldPos = f.Position;
            // should always be the case if we come here!
            if (f.Face == dir)
            {
                // set the new position
                f.SetPosition(m);

                // check whether we have the player here
                if (f.GetType().Equals(typeof(Player)))
                {
                    // check whether we found a treasure
                    if (l.GetField(f.Position.X, f.Position.Y) == Field.Treasure)
                    {
                        OpenTreasure(f as Player);
                    }
                }
                // set the new position of the entity on the board
                l.SetFighter(m, oldPos, f.GetType());
            }
            else
            {
                throw new Exception("Sanity error: wrong move/face");
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
                PlaySound(Properties.Resources.bubble);

            }
            else
            {
                // give at least 10 gold, up to maximal 100
                r = Game.RNG.NextDouble();
                var amount = (int)Math.Ceiling(Math.Max(r * 100, 10));
                p.GoldUp(amount);
                PlaySound(Properties.Resources.coin);
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

        internal void CheckControlKeys()
        {
            // toggle sound
            if (Keyboard.IsKeyDown(Key.S))
            {
                ToggleSound();
                // sleep a bit to avoid several changes
                // being generated in one go
                System.Threading.Thread.Sleep(300);
            }
            // load next level
            if (Keyboard.IsKeyDown(Key.N))
            {
                InitLevel(this.levelSize);
                // sleep a bit to avoid several changes
                // being generated in one go
                System.Threading.Thread.Sleep(300);
            }
            
        }

        #endregion // Methods
    }    
}
