﻿namespace Bakim
{
    partial class UyumGoruntule
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.MakId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BakimTarihi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BakimPersoneli = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BakimTanim = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Aciklama = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MakId,
            this.BakimTarihi,
            this.BakimPersoneli,
            this.BakimTanim,
            this.Aciklama});
            this.dataGridView1.Location = new System.Drawing.Point(13, 68);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(655, 438);
            this.dataGridView1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(94, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 37);
            this.button1.TabIndex = 1;
            this.button1.Text = "Sorgula";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 37);
            this.button2.TabIndex = 2;
            this.button2.Text = "Anasayfa";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // MakId
            // 
            this.MakId.HeaderText = "Makine No";
            this.MakId.Name = "MakId";
            this.MakId.ReadOnly = true;
            // 
            // BakimTarihi
            // 
            this.BakimTarihi.HeaderText = "Bakım Tarihi";
            this.BakimTarihi.Name = "BakimTarihi";
            this.BakimTarihi.ReadOnly = true;
            // 
            // BakimPersoneli
            // 
            this.BakimPersoneli.HeaderText = "Bakım Yapan Personel";
            this.BakimPersoneli.Name = "BakimPersoneli";
            this.BakimPersoneli.ReadOnly = true;
            // 
            // BakimTanim
            // 
            this.BakimTanim.HeaderText = "Bakım Tanımı";
            this.BakimTanim.Name = "BakimTanim";
            this.BakimTanim.ReadOnly = true;
            // 
            // Aciklama
            // 
            this.Aciklama.HeaderText = "Açıklama";
            this.Aciklama.Name = "Aciklama";
            this.Aciklama.ReadOnly = true;
            // 
            // UyumGoruntule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 502);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "UyumGoruntule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Uyum Görüntüle";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn MakId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BakimTarihi;
        private System.Windows.Forms.DataGridViewTextBoxColumn BakimPersoneli;
        private System.Windows.Forms.DataGridViewTextBoxColumn BakimTanim;
        private System.Windows.Forms.DataGridViewTextBoxColumn Aciklama;
    }
}