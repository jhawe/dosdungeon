namespace DosDungeon
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gameView = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.gameView)).BeginInit();
            this.SuspendLayout();
            // 
            // gameView
            // 
            this.gameView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameView.Enabled = false;
            this.gameView.Location = new System.Drawing.Point(0, 0);
            this.gameView.Name = "gameView";
            this.gameView.Size = new System.Drawing.Size(772, 496);
            this.gameView.TabIndex = 0;
            this.gameView.TabStop = false;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 496);
            this.Controls.Add(this.gameView);
            this.DoubleBuffered = true;
            this.Name = "GameForm";
            this.Text = "DosDungeon";
            this.Load += new System.EventHandler(this.OnFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.gameView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.PictureBox gameView;
    }
}