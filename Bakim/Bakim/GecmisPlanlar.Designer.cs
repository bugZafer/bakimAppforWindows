namespace Bakim
{
    partial class GecmisPlanlar
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.MakNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bakim1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bakim2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bakim3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bakim4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YillikBakim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(173, 28);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MakNo,
            this.Bakim1,
            this.Bakim2,
            this.Bakim3,
            this.Bakim4,
            this.YillikBakim});
            this.dataGridView1.Location = new System.Drawing.Point(-7, 56);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(701, 438);
            this.dataGridView1.TabIndex = 8;
            // 
            // MakNo
            // 
            this.MakNo.HeaderText = "Makine No";
            this.MakNo.Name = "MakNo";
            this.MakNo.ReadOnly = true;
            // 
            // Bakim1
            // 
            this.Bakim1.HeaderText = "1. Bakım";
            this.Bakim1.Name = "Bakim1";
            this.Bakim1.ReadOnly = true;
            // 
            // Bakim2
            // 
            this.Bakim2.HeaderText = "2. Bakım";
            this.Bakim2.Name = "Bakim2";
            this.Bakim2.ReadOnly = true;
            // 
            // Bakim3
            // 
            this.Bakim3.HeaderText = "3. Bakım";
            this.Bakim3.Name = "Bakim3";
            this.Bakim3.ReadOnly = true;
            // 
            // Bakim4
            // 
            this.Bakim4.HeaderText = "4. Bakım";
            this.Bakim4.Name = "Bakim4";
            this.Bakim4.ReadOnly = true;
            // 
            // YillikBakim
            // 
            this.YillikBakim.HeaderText = "Yıllık Bakım";
            this.YillikBakim.Name = "YillikBakim";
            this.YillikBakim.ReadOnly = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(2, 11);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 39);
            this.button2.TabIndex = 9;
            this.button2.Text = "Anasayfa";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(378, 28);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 10;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(297, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 37);
            this.button3.TabIndex = 11;
            this.button3.Text = "Bul";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // GecmisPlanlar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 502);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.comboBox1);
            this.Name = "GecmisPlanlar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Geçmiş Planlar";
            this.Load += new System.EventHandler(this.Form4_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn MakNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bakim1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bakim2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bakim3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bakim4;
        private System.Windows.Forms.DataGridViewTextBoxColumn YillikBakim;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
    }
}