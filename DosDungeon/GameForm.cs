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
        Game game;
        Timer timer;
        Stopwatch stopWatch = Stopwatch.StartNew();

        readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        
        public GameForm()
        {
            InitializeComponent();
        }


        private void OnFormLoad(object sender, EventArgs e)
        {
            this.game = new Game(this, stopWatch);
            timer = new Timer();
            timer.Interval = (int)TargetElapsedTime.TotalMilliseconds;
            timer.Tick += this.game.Update;
            timer.Start();            
        }

        internal RichTextBox Board
        {
            get
            {
                return this.textField;
            }
        }     
    }
}
