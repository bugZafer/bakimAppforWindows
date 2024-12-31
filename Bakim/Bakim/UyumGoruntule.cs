using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
namespace Bakim
{
    public partial class UyumGoruntule : Form
    {
        private readonly KategoriService kategoriService;
        public UyumGoruntule()
        {
            InitializeComponent();
            kategoriService = new KategoriService(GetConnectionString());
        }

        private string GetConnectionString()
        {
            try
            {
                // TextFile1.txt'den connection string'i oku

                // App.config'deki connection string'i al ve Data Source'u güncelle
                var builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString)
                {
                    DataSource = Properties.Settings.Default.ServerPath

                };

                return builder.ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bağlantı dizesi okunurken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            }
        }


        // Tek bir bağlantı açma ve kapatma yöntemi
        private SqlConnection CreateConnection()
        {
            var conn = new SqlConnection(GetConnectionString());
            conn.Open();
            return conn;
        }

        private void listele()
        {
            try
            {
                using (var conn = CreateConnection())
                {
                    string query = @"
                        SELECT MakId, BakimTarihi, BakimPersoneli, BakimTanim, Aciklama
                        FROM GecmisBakim
                        WHERE YEAR(BakimTarihi) = @currentYear";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int currentYear = DateTime.Today.Year;
                        cmd.Parameters.AddWithValue("@currentYear", currentYear);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int rowIndex = dataGridView1.Rows.Add();
                                dataGridView1.Rows[rowIndex].Cells["MakId"].Value = reader["MakId"];
                                DateTime bakimTarihi = Convert.ToDateTime(reader["BakimTarihi"]);
                                dataGridView1.Rows[rowIndex].Cells["BakimTarihi"].Value = bakimTarihi.ToString("dd.MM.yyyy");
                                dataGridView1.Rows[rowIndex].Cells["BakimPersoneli"].Value = reader["BakimPersoneli"];
                                dataGridView1.Rows[rowIndex].Cells["BakimTanim"].Value = reader["BakimTanim"];
                                dataGridView1.Rows[rowIndex].Cells["Aciklama"].Value = reader["Aciklama"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private async void admnSorgu()
        {
            DateTime bugun = DateTime.Today;

            try
            {
                using (var conn = CreateConnection())
                {
                    string yilIdQuery = "SELECT yilId FROM yillar WHERE yil = @currentYear";
                    using (SqlCommand cmdYilId = new SqlCommand(yilIdQuery, conn))
                    {
                        cmdYilId.Parameters.AddWithValue("@currentYear", DateTime.Today.Year);
                        int yilId = (int)await cmdYilId.ExecuteScalarAsync();

                        string queryYillikGenel = @"
                            SELECT MakId, aylik1, aylik2, aylik3, aylik4, yillik1 
                            FROM yillikGenel 
                            WHERE CAST(aylik1 AS DATE) < @bugun
                               OR CAST(aylik2 AS DATE) < @bugun
                               OR CAST(aylik3 AS DATE) < @bugun
                               OR CAST(aylik4 AS DATE) < @bugun
                               OR CAST(yillik1 AS DATE) < @bugun";

                        using (SqlCommand cmdYillikGenel = new SqlCommand(queryYillikGenel, conn))
                        {
                            cmdYillikGenel.Parameters.AddWithValue("@bugun", bugun);

                            using (SqlDataReader reader = await cmdYillikGenel.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    string makId = reader["MakId"].ToString();

                                    foreach (var column in new[] { "aylik1", "aylik2", "aylik3", "aylik4", "yillik1" })
                                    {
                                        DateTime? tarih = reader[column] as DateTime?;
                                        if (tarih.HasValue && tarih < bugun)
                                        {
                                            using (var innerConn = CreateConnection())
                                            {
                                                string kontrolQuery = @"
                                                    SELECT COUNT(*) 
                                                    FROM GecmisBakim 
                                                    WHERE MakId = @makId AND BakimTarihi = @tarih";

                                                using (SqlCommand cmdKontrol = new SqlCommand(kontrolQuery, innerConn))
                                                {
                                                    cmdKontrol.Parameters.AddWithValue("@makId", makId);
                                                    cmdKontrol.Parameters.AddWithValue("@tarih", tarih);

                                                    int count = (int)await cmdKontrol.ExecuteScalarAsync();

                                                    if (count == 0)
                                                    {
                                                        string bakimTanim = column == "yillik1" ? "YILLIK" : "3 AYLIK";

                                                        string insertQuery = @"
                                                            INSERT INTO GecmisBakim (MakId, BakimTarihi, BakimPersoneli, BakimTanim, yil, bakimGirisTar) 
                                                            VALUES (@makId, @tarih, @personel, @bakimTanim, @yilId, @bakimGirisTar)";

                                                        using (SqlCommand cmdInsert = new SqlCommand(insertQuery, innerConn))
                                                        {
                                                            string queryRastgelePersonel = @"
                                                                SELECT TOP 1 Bakim_Personeli 
                                                                FROM Bakim_Personel 
                                                                ORDER BY NEWID()";

                                                            using (SqlCommand cmdPersonel = new SqlCommand(queryRastgelePersonel, innerConn))
                                                            {
                                                                cmdInsert.Parameters.AddWithValue("@makId", makId);
                                                                cmdInsert.Parameters.AddWithValue("@tarih", tarih);
                                                                cmdInsert.Parameters.AddWithValue("@personel", await cmdPersonel.ExecuteScalarAsync());
                                                                cmdInsert.Parameters.AddWithValue("@bakimTanim", bakimTanim);
                                                                cmdInsert.Parameters.AddWithValue("@yilId", yilId);
                                                                cmdInsert.Parameters.AddWithValue("@bakimGirisTar", tarih);

                                                                await cmdInsert.ExecuteNonQueryAsync();
                                                                kategoriService.UpdateKategoriForMakId(makId, bakimTanim);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                listele();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 1)
            {
                dataGridView1.Rows.Clear();
            }

            admnSorgu();
        }


        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Gereksiz işleme gerek yok
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Anasayfa form = new Anasayfa();
            form.Show();
            this.Hide();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            dataGridView1.MinimumSize = new Size(dataGridView1.Width, dataGridView1.Height);
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        }
    }
}
