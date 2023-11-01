namespace WinAnprSqe
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.button_print = new System.Windows.Forms.Button();
            this.DataGridMonitor = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridMonitor)).BeginInit();
            this.SuspendLayout();
            // 
            // button_print
            // 
            this.button_print.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button_print.DialogResult = System.Windows.Forms.DialogResult.No;
            this.button_print.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.button_print.Location = new System.Drawing.Point(654, 12);
            this.button_print.Name = "button_print";
            this.button_print.Size = new System.Drawing.Size(114, 23);
            this.button_print.TabIndex = 0;
            this.button_print.Text = "Тест печати\r\n";
            this.button_print.UseVisualStyleBackColor = false;
            this.button_print.Click += new System.EventHandler(this.button_print_Click);
            // 
            // DataGridMonitor
            // 
            this.DataGridMonitor.AllowUserToAddRows = false;
            this.DataGridMonitor.AllowUserToDeleteRows = false;
            this.DataGridMonitor.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.DataGridMonitor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridMonitor.Location = new System.Drawing.Point(12, 93);
            this.DataGridMonitor.Name = "DataGridMonitor";
            this.DataGridMonitor.ReadOnly = true;
            this.DataGridMonitor.Size = new System.Drawing.Size(756, 322);
            this.DataGridMonitor.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(190, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(344, 52);
            this.label1.TabIndex = 2;
            this.label1.Text = "Мониторинг добавленных авто-номеров\r\n";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // MainForm
            // 
            this.AccessibleDescription = "Мониторинг за добавленными авто-машинами";
            this.AccessibleName = "Мониторинг";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(780, 434);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DataGridMonitor);
            this.Controls.Add(this.button_print);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Мониторинг за добавленными авто-машинами";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TableAnpr_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridMonitor)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.DataGridView DataGridMonitor;

        private System.Windows.Forms.Button button_print;

        #endregion
    }
}