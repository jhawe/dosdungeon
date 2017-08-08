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
            this.textField = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textField
            // 
            this.textField.Cursor = System.Windows.Forms.Cursors.No;
            this.textField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textField.Enabled = false;
            this.textField.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textField.Location = new System.Drawing.Point(0, 0);
            this.textField.Name = "textField";
            this.textField.ReadOnly = true;
            this.textField.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.textField.Size = new System.Drawing.Size(460, 496);
            this.textField.TabIndex = 0;
            this.textField.Text = "";
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 496);
            this.Controls.Add(this.textField);
            this.Name = "GameForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textField;
    }
}