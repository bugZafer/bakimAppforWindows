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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
namespace Bakim
{
    public partial class GecmisPlanlar : Form
    {
        public GecmisPlanlar()
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

        private void goster()
        {
            dataGridView1.Rows.Clear();
            // ComboBox'tan seçilen yılı al
            int secilenYil = Convert.ToInt32(comboBox1.SelectedItem);

            // Veritabanı bağlantısı ve komutları
            

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    // 2024 yılına karşılık gelen YilId'yi almak için Yillar tablosuna sorgu yapıyoruz
                    string yilQuery = "SELECT YilId FROM Yillar WHERE Yil = @Yil";
                    SqlCommand yilCmd = new SqlCommand(yilQuery, conn);
                    yilCmd.Parameters.AddWithValue("@Yil", secilenYil);

                    conn.Open();

                    // YilId'yi almak için sorgu çalıştırıyoruz
                    object yilIdObj = yilCmd.ExecuteScalar();

                    if (yilIdObj != null)
                    {
                        // YilId'yi alıyoruz
                        int yilId = Convert.ToInt32(yilIdObj);

                        // YillikGenel tablosundan bu YilId'ye ait verileri almak için sorgu yazıyoruz
                        string query = "SELECT MakId, aylik1, aylik2, aylik3, aylik4, yillik1 FROM yillikGenel WHERE Yil = @YilId ORDER BY MakId ASC";

                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@YilId", yilId);

                        // Veritabanından verileri çekmek için DataAdapter kullan
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        System.Data.DataTable dataTable = new System.Data.DataTable();
                        dataAdapter.Fill(dataTable);

                        // DataGridView'i temizle
                        dataGridView1.Rows.Clear();

                        // Verileri DataGridView'e ekle
                        if (dataTable.Rows.Count > 0)
                        {
                            foreach (DataRow row in dataTable.Rows)
                            {
                                // Tarihleri dd.MM.yyyy formatına dönüştür
                                string aylik1 = Convert.ToDateTime(row["aylik1"]).ToString("dd.MM.yyyy");
                                string aylik2 = Convert.ToDateTime(row["aylik2"]).ToString("dd.MM.yyyy");
                                string aylik3 = Convert.ToDateTime(row["aylik3"]).ToString("dd.MM.yyyy");
                                string aylik4 = Convert.ToDateTime(row["aylik4"]).ToString("dd.MM.yyyy");
                                string yillik1 = Convert.ToDateTime(row["yillik1"]).ToString("dd.MM.yyyy");

                                // DataGridView'e yeni bir satır ekle
                                dataGridView1.Rows.Add(row["MakId"], aylik1, aylik2, aylik3, aylik4, yillik1);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Veri bulunamadı.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Belirtilen yıl için YilId bulunamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        private void yilCombo()
        {

            string query = "SELECT DISTINCT Yil FROM Yillar"; // DISTINCT ile sadece benzersiz yıllar
           

            try
            {
                // Veritabanı bağlantısını aç
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // Sorguyu çalıştır
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        // ComboBox'ı temizle
                        comboBox1.Items.Clear();

                        // Veritabanından verileri al ve ComboBox'a ekle
                        while (reader.Read())
                        {
                            // Eğer 'Yil' kolonu int tipinde ise GetInt32 kullanabilirsiniz
                            int yil = reader.GetInt32(reader.GetOrdinal("Yil"));
                            comboBox1.Items.Add(yil);  // Yıl bilgisi ComboBox'a ekleniyor
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Veritabanı bağlantı hatası: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }

        }
        private void Form4_Load(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            yilCombo();
            comboBox1.SelectedIndex = 0;
            // DataGridView'in minimum yüksekliğini form yükseklik başlangıç boyutuna sabitleyin
            dataGridView1.MinimumSize = new Size(dataGridView1.Width, dataGridView1.Height);

            // Anchor özelliğini sadece aşağı ve sağ kenarlara sabitleyin
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dataGridView1.ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Anasayfa form1 = new Anasayfa();
            form1.Show();
            this.Hide();
            this.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Lütfen bir değer girin.");
                return;
            }

            bool found = false;

            // DataGridView'in tüm hücrelerini tarıyoruz
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null &&
                        cell.Value.ToString().IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) >= 0) // Kısmi arama
                    {
                        // Eğer aranan değer hücre içinde geçiyorsa, o hücreye odaklan
                        dataGridView1.CurrentCell = cell;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }

            if (!found)
            {
                MessageBox.Show("Değer bulunamadı.");
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter)
            {
                button3_Click(sender, e);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            goster();

        }
    }
}
