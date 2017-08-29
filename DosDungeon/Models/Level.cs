using DosDungeon.Common;
using DosDungeon.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DosDungeon.Models
{
    /// <summary>
    /// Defines our dungeon levels. Levels are based on matrices with specific entries
    /// defining different fields, e.g. free, blocked etc.
    /// </summary>
    internal class Level
    {
        #region Class Member
        int size;
        int[,] field;
        private Position start;
        private Position end;
        private Position playerPos;
        private LinkedList<Position> mainPath;
        private List<LinkedList<Position>> branches;
        private int playerField;
        private List<Position> monsterStarts;
        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Level() : this(16)
        {
            // member init
            this.mainPath = new LinkedList<Position>();
            this.branches = new List<LinkedList<Position>>();
            this.monsterStarts = new List<Position>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of the level (gets squared)</param>
        internal Level(int size)
        {
            // member init
            this.mainPath = new LinkedList<Position>();
            this.branches = new List<LinkedList<Position>>();
            this.monsterStarts = new List<Position>();
            this.size = size;
            this.field = new int[this.size, this.size];
            // init field defaults
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    this.field[i, j] = (int)Field.Blocked;
                }
            }
        }
        #endregion // Constructor

        #region Properties

        internal List<LinkedList<Position>> Branches
        {
            get
            {
                return this.branches;
            }
        }

        internal LinkedList<Position> Main
        {
            get
            {
                return this.mainPath;
            }
        }

        internal int Size
        {
            get
            {
                return this.size;
            }
        }

        internal Position Start
        {
            get
            {
                return this.start;
            }
        }

        internal Position End
        {
            get
            {
                return this.end;
            }
        }

        internal List<Position> MonsterStarts
        {
            get
            {
                return this.monsterStarts;
            }
        }

        internal GameState State { get; set; }

        #endregion // Properties

        #region Methods      

        #region IsFieldAccessible
        /// <summary>
        /// Checks whether the specified coordinates
        /// piont to a free field (main,branch,treasure,free)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal bool IsFieldAccessible(int x, int y, Type t)
        {
            Field field = GetField(x, y);
            if (t.Equals(typeof(Player)))
            {
                return (field == Field.Branch ||
                    field == Field.Free || field == Field.Main || field == Field.Treasure);
            }
            else
            {
                return (field == Field.Branch ||
                field == Field.Free || field == Field.Main);
            }
        }
        #endregion // IsFieldAccessible     

        #region AddBranch
        /// <summary>
        /// Adds a generated branch to the level
        /// </summary>
        /// <param name="b">The branch to be added</param>
        internal void AddBranch(LinkedList<Position> b)
        {
            foreach (var p in b)
            {
                SetField(p.X, p.Y, Field.Branch);
            }
            this.branches.Add(b);
        }

        internal void AddMonster(Position n)
        {
            this.monsterStarts.Add(n);
            SetField(n, Field.Monster);
        }

        #endregion // AddBranch

        #region IsEdgeField
        /// <summary>
        /// Checks whether a certian position is
        /// on the edge of the level
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        internal bool IsEdgeField(Position pos)
        {
            // check if we are at the boundaries            
            int x = pos.X;
            int y = pos.Y;
            if (x == 0 || x == this.size - 1
                || y == 0 || y == this.size - 1)
            {
                return (true);
            }
            return (false);
        }
        #endregion // IsEdgeField       

        #region SetEnd
        /// <summary>
        /// Sets the level's end position
        /// </summary>
        /// <param name="position">The position to be set</param>
        internal void SetEnd(Position position)
        {
            this.end = position;
            this.field[this.end.X, this.end.Y] = (int)Field.Main;
        }
        #endregion // SetEnd

        #region SetStart
        /// <summary>
        /// Sets the level's start position
        /// </summary>
        /// <param name="position">the position to be set</param>
        internal void SetStart(Position position)
        {
            this.start = position;
            this.field[this.start.X, this.start.Y] = (int)Field.Main;
            this.mainPath.AddLast(position);
        }
        #endregion // SetStart

        #region GetField
        /// <summary>
        /// Gets a specific field's descriptive enum
        /// </summary>
        /// <param name="x">X pos of the field</param>
        /// <param name="y">Y pos of the field</param>
        /// <returns></returns>
        internal Field GetField(Position p)
        {
            return GetField(p.X, p.Y);
        }

        internal Field GetField(int x, int y)
        {
            // out of bounds
            if (x >= this.size | y >= this.size | x < 0 | y < 0)
            {
                return Field.NA;
            }

            var value = this.field[x, y];
            Field result;
            bool sanity = Enum.TryParse<Field>(value.ToString(), out result);
            if (!sanity)
            {
                // TODO note error here
            }
            return (result);
        }
        #endregion // GetField

        #region SetField
        /// <summary>
        /// Sets the field at the specified position to the given value
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="value"></param>
        internal void SetField(Position pos, Field value)
        {
            SetField(pos.X, pos.Y, value);
        }

        internal void SetField(int x, int y, Field value)
        {
            this.field[x, y] = (int)value;
        }
        #endregion // SetField

        #region SetFighter
        /// <summary>
        /// Sets the position of the player to the current field
        /// </summary>
        /// <param name="pos">The position of the player to
        /// be set</param>
        internal void SetFighter(Position nPos, Position oPos, Type ftype)
        {          
            if (ftype.Equals(typeof(Player)))
            {
                // could be null during init
                if (oPos != null)
                {
                    this.field[oPos.X, oPos.Y] = (int)Field.Free;
                }
                this.field[nPos.X, nPos.Y] = (int)Field.Player;               
            }
            else if(ftype.Equals(typeof(Monster)))
            {
                this.field[oPos.X, oPos.Y] = (int)Field.Free;
                this.field[nPos.X, nPos.Y] = (int)Field.Monster;
            }
        }
        #endregion // SetFighter

        #endregion // Methods
    }
}
