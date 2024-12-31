using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Bakim
{
    public partial class formGoruntule : Form
    {
        public formGoruntule()
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

        private void dgWHazirla()
        {
            dataGridView1.Columns.Clear(); // Kolonları temizleyin

            // 14 sütun ekleyelim
            for (int i = 0; i < 5; i++)
            {
                dataGridView1.Columns.Add("Column" + i, "Sütun " + (i + 1));
            }

            // 26 satır ekleyelim
            for (int i = 0; i < 22; i++)
            {
                dataGridView1.Rows.Add();
            }

            // 1. satır, 3. sütun

            dataGridView1.Rows[1].Cells[0].Value = "MAKİNA ADI :";
            dataGridView1.Rows[2].Cells[0].Value = "BAKIM PERİYODU :";
            dataGridView1.Rows[3].Cells[0].Value = "RAPOR NO :";
            dataGridView1.Rows[4].Cells[0].Value = "Sıra no";


            dataGridView1.Rows[4].Cells[1].Value = "Bakımı Yapılacak Özellikler";
            dataGridView1.Rows[4].Cells[2].Value = "Periyod";
            dataGridView1.Rows[4].Cells[3].Value = "Yapıldı";
            dataGridView1.Rows[4].Cells[4].Value = "Açıklamalar";


            dataGridView1.Rows[0].Cells[3].Value = "Bakım Tarihi";
            dataGridView1.Rows[1].Cells[3].Value = "Bakım Personeli";
        }

        private void formGoruntule_Load(object sender, EventArgs e)
        {
            dgWHazirla();

            // DataGridView'in minimum yüksekliğini form yükseklik başlangıç boyutuna sabitleyin
            dataGridView1.MinimumSize = new Size(dataGridView1.Width, dataGridView1.Height);
            // Anchor özelliğini sadece aşağı ve sağ kenarlara sabitleyin
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;


            yilCombo();

            for (int i = 4; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null)
                {
                    dataGridView1.Rows[i].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // Hücre içeriğini ortalar
                }
            }
            dataGridView1.Rows[4].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Rows[4].Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Rows[4].Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    dataGridView1.Rows[i].Cells[j].ReadOnly = true; // Tüm hücreler read-only olarak başlar
                }
            }


        }
        private void bakimKriterleri()
        {
            string selectedText = comboBox2.Text; // ComboBox2'den seçilen değer
            string kategori = selectedText;

            // Dinamik olarak belirtilen karakter dizisini çıkartın (örneğin: "AY-", "BP-", "PR-")
            int prefixLength = kategori.IndexOf('-') + 1;
            if (prefixLength > 0)
            {
                kategori = kategori.Substring(0, prefixLength); // Dinamik olarak çıkartılan ön ek
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT Metin, Periyod FROM BakimMetinleri WHERE Kategori = @Kategori";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Kategori", kategori);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int rowIndex = 5; // DataGridView'in 6. satırı (index 5)

                            // DataGridView'deki 6. satırdan 17. satıra kadar hücreleri temizle
                            for (int i = rowIndex; i < 17; i++)
                            {
                                if (i < dataGridView1.Rows.Count)
                                {
                                    dataGridView1.Rows[i].Cells[1].Value = ""; // 2. sütun

                                }
                            }

                            while (reader.Read())
                            {
                                if (rowIndex < dataGridView1.Rows.Count)
                                {
                                    dataGridView1.Rows[rowIndex].Cells[1].Value = ""; // 2. sütun
                                    dataGridView1.Rows[rowIndex].Cells[2].Value = "";
                                    dataGridView1.Rows[rowIndex].Cells[1].Value = reader["Metin"].ToString();
                                    dataGridView1.Rows[rowIndex].Cells[2].Value = reader["Periyod"].ToString();

                                }
                                else
                                {
                                    int newRowIndex = dataGridView1.Rows.Add();
                                    dataGridView1.Rows[newRowIndex].Cells[1].Value = ""; // 2. sütun
                                    dataGridView1.Rows[newRowIndex].Cells[2].Value = "";
                                    dataGridView1.Rows[newRowIndex].Cells[1].Value = reader["Metin"].ToString();
                                    dataGridView1.Rows[newRowIndex].Cells[2].Value = reader["Periyod"].ToString();
                                }
                                rowIndex++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); // Hata durumunda mesaj gösterilir
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide();
        }
        private void yilIdBul(int yil)
        {
            string query = "SELECT YilId FROM Yillar WHERE Yil = @Yil";

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Yil", yil);

                        int yilId = (int)cmd.ExecuteScalar();
                        gecmisBakimMakId(yilId);
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

        private void gecmisBakimMakId(int yilId)
        {
            string query = "SELECT DISTINCT MakId FROM GecmisBakim WHERE yil = @YilId ORDER BY MakId ASC";

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@YilId", yilId);

                        SqlDataReader reader = cmd.ExecuteReader();

                        comboBox2.Items.Clear();

                        while (reader.Read())
                        {
                            string makId = reader.GetString(reader.GetOrdinal("MakId"));
                            if (!comboBox2.Items.Contains(makId))
                            {
                                comboBox2.Items.Add(makId);
                            }
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            temizle();
            int selectedYil = (int)comboBox1.SelectedItem;  // ComboBox'dan seçilen yılı al
            yilIdBul(selectedYil);


        }
        string bakimTanim;
        private void bakimTanimGetir(string makId)
        {
            bakimTanim = "";
            // DISTINCT anahtar kelimesi eklenerek benzersiz bakım tanımları alınır
            string query = "SELECT DISTINCT BakimTanim FROM GecmisBakim WHERE MakId = @MakId";

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MakId", makId);

                        SqlDataReader reader = cmd.ExecuteReader();

                        comboBox3.Items.Clear(); // ComboBox3'ü temizleyin

                        // Veritabanından gelen her bakım tanımını ekleyin
                        while (reader.Read())
                        {
                            bakimTanim = reader.GetString(reader.GetOrdinal("BakimTanim"));
                            if (!string.IsNullOrEmpty(bakimTanim))
                            {
                                comboBox3.Items.Add(bakimTanim); // Benzersiz değerleri ekleyin
                            }
                        }
                    }
                }

                // Eğer ComboBox hala boşsa, kullanıcıya bilgi ver
                if (comboBox3.Items.Count == 0)
                {
                    MessageBox.Show("Seçilen MakId için bakım tanımı bulunamadı.");
                }
                else
                {
                    comboBox3.SelectedIndex = 0; // İlk değeri seç
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void temizle()
        {
            for (int row = 5; row < 16; row++)
            {
                if (row < dataGridView1.Rows.Count)
                {
                    dataGridView1.Rows[row].Cells[1].Value = ""; // 2. sütun
                    dataGridView1.Rows[row].Cells[2].Value = ""; // 3. sütun
                    dataGridView1.Rows[row].Cells[3].Value = ""; // 4. sütun
                    dataGridView1.Rows[row].Cells[4].Value = ""; // 4. sütun
                    
                }
            }

        }
        string selectedMakId;
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            temizle();

            selectedMakId = comboBox2.SelectedItem.ToString();  // ComboBox2'den seçilen MakId
            bakimTanimGetir(selectedMakId);  // Bakım Tanımı verilerini almak için ilgili metodu çağır
            dataGridView1.Rows[1].Cells[1].Value = comboBox2.Text;
            bakimKriterleri();
            dataGridView1.AutoResizeColumns();  // Tüm sütunların genişliklerini otomatik olarak ayarlar
            dataGridView1.Rows[0].Cells[1].Value = comboBox2.Text + " ÖNLEYİCİ BAKIM FORMU";
            dataGridView1.Rows[0].Cells[1].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.Rows[0].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[2].Value != null)
                {
                    row.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // Hücre içeriğini ortalar
                }
                if (row.Cells[3].Value != null)
                {
                    row.Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // Hücre içeriğini ortalar
                }
                int sayi = 1;
                for (int i = 5; i < 17; i++) // 6. satırdan 17. satıra kadar (index 5-16)
                {
                    dataGridView1.Rows[i].Cells[0].Value = "";

                    // 2. sütunun boş olmadığını kontrol et
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[2].Value?.ToString()))
                    {
                        dataGridView1.Rows[i].Cells[0].Value = sayi; // 1. sütuna sayıyı yaz
                        dataGridView1.Rows[i].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // 1. sütuna sayıyı yaz
                        sayi++;
                    }
                }
            }
            int targetRow = -1;

            // İlk sütunda boş bir hücre arama
            for (int i = 3; i < dataGridView1.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[0].Value?.ToString()))
                {
                    targetRow = i + 1; // Bulunan boş hücrenin bir altına yazdır
                    break;
                }
            }

            if (targetRow != -1)
            {
                dataGridView1.Rows[targetRow].Cells[0].Value = "*Periyod Sembolleri";
                dataGridView1.Rows[targetRow + 1].Cells[0].Value = "A: 3AYLIK BAKIM";
                dataGridView1.Rows[targetRow + 2].Cells[0].Value = "B: YILLIK BAKIM";
            }
            for (int i = 0; i < targetRow + 2; i++)
            {
                dataGridView1.Rows.Add();
            }


        }
        private void yilMakIdVerileriniGetir()
        {
            // ComboBox'lardan seçilen veriler
            int selectedYil = (int)comboBox1.SelectedItem; // Yıl bilgisi
            string selectedMakId = comboBox2.SelectedItem.ToString(); // MakId bilgisi

            // YılId'yi bulmak için sorgu
            string yilIdQuery = "SELECT YilId FROM Yillar WHERE Yil = @Yil";

            // YıllıkGenel tablosundan ilgili verileri almak için sorgu
            string yillikGenelQuery = @"
        SELECT aylik1, aylik2, aylik3, aylik4, yillik1 
        FROM yillikGenel 
        WHERE MakId = @MakId AND Yil = @Yil";

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // YılId'yi bul
                    int yilId;
                    using (SqlCommand yilIdCmd = new SqlCommand(yilIdQuery, conn))
                    {
                        yilIdCmd.Parameters.AddWithValue("@Yil", selectedYil);
                        yilId = (int)yilIdCmd.ExecuteScalar();
                    }

                    // YıllıkGenel verilerini getir
                    using (SqlCommand genelCmd = new SqlCommand(yillikGenelQuery, conn))
                    {
                        genelCmd.Parameters.AddWithValue("@MakId", selectedMakId);
                        genelCmd.Parameters.AddWithValue("@Yil", yilId);

                        using (SqlDataReader reader = genelCmd.ExecuteReader())
                        {
                            comboBox3.Items.Clear(); // ComboBox'u temizle

                            if (reader.Read())
                            {
                                // Sütunlardan verileri al ve ComboBox'a ekle
                                for (int i = 0; i < 5; i++)
                                {
                                    string value = reader.GetValue(i).ToString();
                                    comboBox3.Items.Add(value); // Aylık ve yıllık bakımları ekle
                                }
                            }
                            else
                            {
                                MessageBox.Show("Seçilen yıl ve MakId için veri bulunamadı.");
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

        private void bakimTarihiGetir(string makId, string bakimTanim)
        {
            comboBox4.Items.Clear(); // ComboBox4'ü temizle
            string query = @"
    SELECT BakimTarihi 
    FROM GecmisBakim 
    WHERE MakId = @MakId AND BakimTanim = @BakimTanim";

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MakId", makId);
                        cmd.Parameters.AddWithValue("@BakimTanim", bakimTanim);

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            DateTime? bakimTarihi = reader.IsDBNull(reader.GetOrdinal("BakimTarihi"))
                                                    ? (DateTime?)null
                                                    : reader.GetDateTime(reader.GetOrdinal("BakimTarihi"));

                            if (bakimTarihi.HasValue)
                            {
                                comboBox4.Items.Add(bakimTarihi.Value.ToString("dd.MM.yyyy")); // Bakım tarihlerini ekle
                            }
                        }
                    }
                }

                // Eğer ComboBox hala boşsa, kullanıcıya bilgi ver
                if (comboBox4.Items.Count == 0)
                {
                    MessageBox.Show("Seçilen MakId ve Bakım Tanımı için bakım tarihi bulunamadı.");
                }
                else
                {
                    comboBox4.SelectedIndex = 0; // İlk değeri seç
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            bakimTanim = comboBox3.SelectedItem.ToString();  // ComboBox3'ten seçilen Bakım Tanımını al
            bakimTarihiGetir(selectedMakId, bakimTanim);
            dataGridView1.Rows[2].Cells[1].Value = comboBox3.Text;


        }
        private void GetBakimKriterleri()
        {
            string selectedDate = comboBox4.Text;

            // Tarih formatını kontrol edin
            DateTime parsedDate;
            if (!DateTime.TryParseExact(selectedDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                MessageBox.Show("Lütfen geçerli bir tarih seçin.");
                return;
            }

            // Formatı düzgün olan tarihi SQL sorgusuna ekleyin
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT kriter1, kriter2, kriter3, kriter4, kriter5, kriter6, kriter7, kriter8, kriter9, kriter10, kriter11, kriter12 " +
                                   "FROM GecmisBakim WHERE MakId = @MakId AND BakimTarihi = @BakimTarihi";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MakId", selectedMakId);
                        cmd.Parameters.AddWithValue("@BakimTarihi", parsedDate.ToString("yyyy-MM-dd"));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int rowIndex = 5; // DataGridView4'ün 5. satırı (index 6) 
                            int columnIndex = 3; // 3. sütun

                            while (reader.Read())
                            {
                                if (rowIndex < dataGridView1.Rows.Count)
                                {
                                    for (int j = 0; j < 12; j++)
                                    {
                                        dataGridView1.Rows[rowIndex + j].Cells[columnIndex].Value = reader[$"kriter{j + 1}"].ToString();
                                        if (dataGridView1.Rows[rowIndex + j].Cells[columnIndex].Value != null)
                                        {
                                            if (dataGridView1.Rows[rowIndex + j].Cells[columnIndex].Value.ToString() != string.Empty)
                                            {
                                                dataGridView1.Rows[rowIndex + j].Cells[columnIndex].ReadOnly = false; // Tüm hücreler read-only olarak başlar
                                                dataGridView1.Rows[rowIndex + j].Cells[columnIndex + 1].ReadOnly = false; // Tüm hücreler read-only olarak başlar
                                            }
                                        }
                                    }
                                    rowIndex++;
                                }
                                else
                                {
                                    MessageBox.Show("Veriler için yeterli satır bulunamadı.");
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); // Hata durumunda mesaj gösterilir
                }
            }

        }
        private object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        private void perGetir()
        {
            // Formatı "03.06.2024" olan tarihi "2024-06-03" formatına dönüştür
            string tarih = DateTime.ParseExact(comboBox4.Text, "dd.MM.yyyy", null).ToString("yyyy-MM-dd");

            // İlgili DataGridView hücresine bakimPersoneli verisini ekle
            dataGridView1.Rows[1].Cells[4].Value =
                (string)ExecuteScalar("SELECT BakimPersoneli FROM GecmisBakim WHERE MakId = @MakId AND BakimTarihi = @BakimTarihi",
                    new SqlParameter("@MakId", comboBox2.Text),
                    new SqlParameter("@BakimTarihi", tarih));
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

            GetBakimKriterleri();
            dataGridView1.Rows[0].Cells[4].Value = comboBox4.Text;
            perGetir();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // ComboBox'ların ve DateTimePicker'ın doluluğunu kontrol et
            if (comboBox1.SelectedIndex == -1 || // comboBox1 seçilmemişse
                comboBox2.SelectedIndex == -1 || // comboBox2 seçilmemişse
                comboBox3.SelectedIndex == -1 || // comboBox3 seçilmemişse
                comboBox4.SelectedIndex == -1)   // comboBox4 seçilmemişse
            {
                // Uyarı mesajı göster
                MessageBox.Show("Eksik bilgi var. Lütfen ilgili alanları doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi sonlandır
            }
            else
            {


                StringBuilder aciklamaBuilder = new StringBuilder();
                for (int i = 5; i <= 17; i++)
                {
                    string cellValue = dataGridView1.Rows[i].Cells[4].Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        aciklamaBuilder.Append(cellValue).Append(i - 4 + " - ");
                    }
                }
                string Aciklama = aciklamaBuilder.ToString().TrimEnd(' ', '-');
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // Tarih formatını düzenlemek için
                    DateTime parsedDate = DateTime.ParseExact(comboBox4.Text, "dd.MM.yyyy", null);

                    // Güncelleme sorgusu
                    string updateQuery = @"
    UPDATE GecmisBakim
    SET 
        bakimGirisTar = @BugunTarihi,
        kriter1 = @Kriter1, 
        kriter2 = @Kriter2, 
        kriter3 = @Kriter3, 
        kriter4 = @Kriter4, 
        kriter5 = @Kriter5, 
        kriter6 = @Kriter6, 
        kriter7 = @Kriter7, 
        kriter8 = @Kriter8, 
        kriter9 = @Kriter9, 
        kriter10 = @Kriter10, 
        kriter11 = @Kriter11, 
        kriter12 = @Kriter12,
        aciklama = @Aciklama
    WHERE 
        MakId = @MakId 
        AND BakimTarihi = @FormattedBakimTarihi";

                    try
                    {
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@BugunTarihi", DateTime.Now.ToString("yyyy-MM-dd"));
                            updateCmd.Parameters.AddWithValue("@MakId", selectedMakId);
                            updateCmd.Parameters.AddWithValue("@FormattedBakimTarihi", parsedDate.ToString("yyyy-MM-dd"));
                            updateCmd.Parameters.AddWithValue("@Aciklama", Aciklama);
                            for (int i = 0; i < 12; i++)
                            {
                                int rowIndex = 5 + i; // 5. satırdan başlayarak 12 satır için indeksler artırılır
                                var kriterCell = dataGridView1.Rows[rowIndex].Cells[3]?.Value;
                                updateCmd.Parameters.AddWithValue("@Kriter" + (i + 1), kriterCell ?? DBNull.Value);
                            }
                            // Eğer reader null değilse, verileri parametrelere set ediyoruz
                            // reader.Close(); ya da reader.Dispose(); ile kapatılabilir
                            int rowsAffected = updateCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Güncelleme işlemi başarılı.");
                            }
                            else
                            {
                                MessageBox.Show("Güncelleme başarısız. Geçersiz bilgiler.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Güncelleme işlemi sırasında bir hata oluştu: " + ex.Message);
                    }
                }
            }
        }
    }
}

