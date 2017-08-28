using DosDungeon.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DosDungeon
{
    public partial class GameForm : Form
    {
        #region Class member
        private Game game;
        private Timer timer;
        internal Bitmap gameView;
        private Stopwatch stopWatch = Stopwatch.StartNew();
        readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        #endregion // Class Member

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            this.SetStyle(
           ControlStyles.UserPaint |
           ControlStyles.AllPaintingInWmPaint |
           ControlStyles.DoubleBuffer, true);

            // code for those eventhandlers borrowed and adapted from 
            // https://stackoverflow.com/a/7835149
            this.ResizeEnd += new EventHandler(CreateBackBufferHandler);
            this.Load += new EventHandler(CreateBackBufferHandler);
            this.Paint += new PaintEventHandler(PaintHandler);

        }
        #endregion // Constructor

        #region Methods

        #region Eventmethods

        void PaintHandler(object sender, PaintEventArgs e)
        {
            if (this.gameView != null)
            {
                e.Graphics.DrawImageUnscaled(this.gameView, Point.Empty);
            }
        }

        void CreateBackBufferHandler(object sender, EventArgs e)
        {
            if (this.gameView != null)
                this.gameView.Dispose();

            this.gameView = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            this.game = new Game(this, stopWatch);

            timer = new Timer();
            timer.Interval = (int)TargetElapsedTime.TotalMilliseconds;
            timer.Tick += this.game.Update;
            timer.Start();
        }
        #endregion // Eventmethods

        #endregion // Methods
    }
}
