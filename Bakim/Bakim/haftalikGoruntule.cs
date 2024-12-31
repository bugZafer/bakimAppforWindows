using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Bakim
{
    public partial class haftalikGoruntule : Form
    {
        public haftalikGoruntule()
        {
            InitializeComponent();
        }
        private string GetConnectionString()
        {
            try
            {
 
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

        private void button1_Click(object sender, EventArgs e)
        {
           
            try
            {
                // 1. Haftanın başlangıç ve bitiş tarihlerini hesapla
                DateTime today = DateTime.Today;
                int currentDayOfWeek = (int)today.DayOfWeek;
                DateTime startOfWeek = today.AddDays(-currentDayOfWeek + (currentDayOfWeek == 0 ? -6 : 1)); // Pazartesi başlangıcı
                DateTime endOfWeek = startOfWeek.AddDays(6);

                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // 2. yillikGenel tablosundaki tüm verileri sorgula
                    string query = @"
                SELECT MakId, aylik1, aylik2, aylik3, aylik4, yillik1
                FROM yillikGenel";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            dataGridView1.Rows.Clear(); // DataGridView'i temizle

                            while (reader.Read())
                            {
                                string makId = reader["MakId"].ToString();

                                // Tarih sütunlarından uygun olanları kontrol et
                                DateTime[] tarihler = new DateTime[5];
                                string[] kolonAdlari = { "aylik1", "aylik2", "aylik3", "aylik4", "yillik1" };
                                string[] periyotlar = { "1.Bakım", "2.Bakım", "3.Bakım", "4.Bakım", "Yıllık Bakım" };

                                for (int i = 0; i < kolonAdlari.Length; i++)
                                {
                                    if (reader[kolonAdlari[i]] != DBNull.Value)
                                    {
                                        DateTime tarih = Convert.ToDateTime(reader[kolonAdlari[i]]);
                                        if (tarih >= startOfWeek && tarih <= endOfWeek)
                                        {
                                            // Tarih ve periyod bilgilerini ekle
                                            int rowIndex = dataGridView1.Rows.Add();
                                            dataGridView1.Rows[rowIndex].Cells["MakId"].Value = makId;
                                            dataGridView1.Rows[rowIndex].Cells["Tarih"].Value = tarih.ToString("dd.MM.yyyy");
                                            dataGridView1.Rows[rowIndex].Cells["Periyot"].Value = periyotlar[i];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Sorgulama tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Anasayfa form1 = new Anasayfa();
            form1.Show();
            this.Hide();
        }

        private void haftalikGoruntule_Load(object sender, EventArgs e)
        {

            dataGridView1.MinimumSize = new Size(dataGridView1.Width, dataGridView1.Height);

            // Anchor özelliğini sadece aşağı ve sağ kenarlara sabitleyin
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        }
    }
}
