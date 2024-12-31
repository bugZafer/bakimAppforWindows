using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Bakim
{
    public partial class yeniFormEkle : Form
    {
        public yeniFormEkle()
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
        string selectedMakId;
        string bakimTanim;


        private void dgWHazirla()
        {
            dataGridView1.Columns.Clear(); // Kolonları temizleyin

            // 5 sütun ekleyelim
            for (int i = 0; i < 5; i++)
            {
                dataGridView1.Columns.Add("Column" + i, "Sütun " + (i + 1));
            }

            // 22 satır ekleyelim
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
        private void PersonelList()
        {
            // Veritabanı sorgusu
            string query = "SELECT DISTINCT Bakim_Personeli FROM Bakim_Personel"; // DISTINCT ile sadece benzersiz verileri getiriyoruz.


            try
            {
                // Veritabanı bağlantısı aç
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // Sorgu çalıştır
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        // ComboBox'ı temizle
                        comboBox2.Items.Clear(); // comboBox1 yerine comboBox2 kullanılmalı.

                        // Veritabanından verileri al ve ComboBox'a ekle
                        while (reader.Read())
                        {
                            string kategori = reader["Bakim_Personeli"].ToString(); // Veritabanındaki değeri al
                            comboBox4.Items.Add(kategori); // ComboBox'a ekle
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj göster
                MessageBox.Show("Hata: " + ex.Message);
            }
        }


        private void yeniFormEkle_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            dgWHazirla();
            PersonelList();
            // DataGridView'in minimum yüksekliğini form yükseklik başlangıç boyutuna sabitleyin
            dataGridView1.MinimumSize = new Size(dataGridView1.Width, dataGridView1.Height);
            // Anchor özelliğini sadece aşağı ve sağ kenarlara sabitleyin
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;


         

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
            comboBox1.Items.Add(DateTime.Now.Year.ToString());
            comboBox1.SelectedItem = DateTime.Now.Year.ToString();

        }

        private int yilId;
        private void yilIdBul(int yil)
        {
            yilId = 0;
            string query = "SELECT YilId FROM Yillar WHERE Yil = @Yil";
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Yil", yil);
                        yilId = (int)cmd.ExecuteScalar();
                        yilliGenelMakId(yilId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void yilIdBul2(int yil, string makId)
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
                        makId = comboBox2.Text;
                        yilGenelTabloGetir(yilId, makId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }


        private void yilGenelTabloGetir(int yilId, string makId)
        {
            string query = "SELECT aylik1, aylik2, aylik3, aylik4, yillik1 FROM yillikGenel WHERE yil = @YilId AND MakId = @MakId";

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@YilId", yilId);
                        cmd.Parameters.AddWithValue("@MakId", makId);

                        SqlDataReader reader = cmd.ExecuteReader();
                        comboBox3.Items.Clear();

                        DateTime currentDate = DateTime.Now;

                        while (reader.Read())
                        {
                            if (reader["aylik1"] != DBNull.Value)
                            {
                                DateTime tarih1 = Convert.ToDateTime(reader["aylik1"]);
                                if (tarih1 < currentDate)
                                {
                                    if (!BakimKaydiVarMi(makId, tarih1))
                                        comboBox3.Items.Add(tarih1.ToString("dd.MM.yyyy"));
                                }
                            }
                            if (reader["aylik2"] != DBNull.Value)
                            {
                                DateTime tarih2 = Convert.ToDateTime(reader["aylik2"]);
                                if (tarih2 < currentDate)
                                {
                                    if (!BakimKaydiVarMi(makId, tarih2))
                                        comboBox3.Items.Add(tarih2.ToString("dd.MM.yyyy"));
                                }
                            }
                            if (reader["aylik3"] != DBNull.Value)
                            {
                                DateTime tarih3 = Convert.ToDateTime(reader["aylik3"]);
                                if (tarih3 < currentDate)
                                {
                                    if (!BakimKaydiVarMi(makId, tarih3))
                                        comboBox3.Items.Add(tarih3.ToString("dd.MM.yyyy"));
                                }
                            }
                            if (reader["aylik4"] != DBNull.Value)
                            {
                                DateTime tarih4 = Convert.ToDateTime(reader["aylik4"]);
                                if (tarih4 < currentDate)
                                {
                                    if (!BakimKaydiVarMi(makId, tarih4))
                                        comboBox3.Items.Add(tarih4.ToString("dd.MM.yyyy"));
                                }
                            }
                            if (reader["yillik1"] != DBNull.Value)
                            {
                                DateTime tarih5 = Convert.ToDateTime(reader["yillik1"]);
                                if (tarih5 < currentDate)
                                {
                                    if (!BakimKaydiVarMi(makId, tarih5))
                                        comboBox3.Items.Add(tarih5.ToString("dd.MM.yyyy"));
                                }
                            }
                        }

                        if (comboBox3.Items.Count == 0)
                        {
                            MessageBox.Show("Tarih bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private bool BakimKaydiVarMi(string makId, DateTime tarih)
        {
            string kontrolQuery = "SELECT COUNT(*) FROM gecmisBakim WHERE MakId = @MakId AND BakimTarihi = @Tarih";

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, conn))
                {
                    kontrolCmd.Parameters.AddWithValue("@MakId", makId);
                    kontrolCmd.Parameters.AddWithValue("@Tarih", tarih.ToString("yyyy-MM-dd"));

                    int count = (int)kontrolCmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }



        private void yilliGenelMakId(int yilId)
        {
            string query = @"
        SELECT DISTINCT yg.MakId 
        FROM yillikGenel yg
        WHERE yil = @YilId 
          AND (
              (yg.aylik1 < DATEADD(WEEK, -1, GETDATE()) OR yg.aylik2 < DATEADD(WEEK, -1, GETDATE()) OR 
                  yg.aylik3 < DATEADD(WEEK, -1, GETDATE()) OR yg.aylik4 < DATEADD(WEEK, -1, GETDATE()) OR 
                  yg.yillik1 < DATEADD(WEEK, -1, GETDATE()))
              AND 
              (SELECT COUNT(*) FROM gecmisBakim gb WHERE gb.MakId = yg.MakId AND gb.bakimTarihi = yg.yillik1) = 0
          )
        ORDER BY yg.MakId ASC";

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@YilId", yilId);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            comboBox2.Items.Add(reader["MakId"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            temizle();
            if (comboBox1.SelectedItem != null)
            {
                int selectedYil = int.Parse(comboBox1.SelectedItem.ToString());
                yilIdBul(selectedYil); // Seçilen yıla göre MakID'leri yükleyin
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            temizle();

            selectedMakId = comboBox2.SelectedItem.ToString();  // ComboBox2'den seçilen MakId
            dataGridView1.Rows[1].Cells[1].Value = comboBox2.Text;
            bakimKriterleri();
            dataGridView1.AutoResizeColumns();  // Tüm sütunların genişliklerini otomatik olarak ayarlar
            dataGridView1.Rows[0].Cells[1].Value = comboBox2.Text + " ÖNLEYİCİ BAKIM FORMU";
            dataGridView1.Rows[0].Cells[1].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.Rows[0].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            comboBox3.Items.Clear();
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
            }
            if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null)
            {
                int selectedYil = int.Parse(comboBox1.SelectedItem.ToString());
                string selectedMakId = comboBox2.SelectedItem.ToString();
                yilIdBul2(selectedYil, selectedMakId); // Yıl ve MakID'ye göre tarihleri yükleyin
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
           

        }

        

        private void yapildi()
        {
            for (int i = 5; i <= 17; i++)
            {
                dataGridView1.Rows[i].Cells[3].Value = "";

                // 2. sütundaki değer (3 AYLIK mı YILLIK mı?)
                string criteriaValue = dataGridView1.Rows[2].Cells[1].Value?.ToString();
                // 3. sütundaki mevcut hücre değeri
                string currentValue = dataGridView1.Rows[i].Cells[2].Value?.ToString();

                if (criteriaValue == "3 AYLIK" && currentValue == "A")
                {
                    // Eğer kriter "3 AYLIK" ve 3. sütunda "A" varsa
                    DataGridViewComboBoxCell comboCell = new DataGridViewComboBoxCell();

                    // ComboBox'a seçenekler ekliyoruz
                    comboCell.Items.Add("EVET");
                    comboCell.Items.Add("HAYIR");
                    // Hücreye ComboBox'ı atıyoruz
                    comboCell.Value = "EVET";
                    dataGridView1.Rows[i].Cells[3] = comboCell; // 8. sütunda, i. satırdaki hücreye ekliyoruz
                    comboCell.ReadOnly = false;
                    dataGridView1.Rows[i].Cells[4].ReadOnly = false; // 4. sütunu düzenlenebilir yap

                }
                else if (criteriaValue == "YILLIK" && (currentValue == "A" || currentValue == "B"))
                {
                    // Eğer kriter "YILLIK" ve 3. sütunda "A" veya "B" varsa
                    DataGridViewComboBoxCell comboCell = new DataGridViewComboBoxCell();

                    // ComboBox'a seçenekler ekliyoruz
                    comboCell.Items.Add("EVET");
                    comboCell.Items.Add("HAYIR");
                    // Hücreye ComboBox'ı atıyoruz
                    comboCell.Value = "EVET";
                    dataGridView1.Rows[i].Cells[3] = comboCell; // 8. sütunda, i. satırdaki hücreye ekliyoruz
                    comboCell.ReadOnly = false;
                    dataGridView1.Rows[i].Cells[4].ReadOnly = false; // 4. sütunu düzenlenebilir yap
                }
                else
                {
                    dataGridView1.Rows[i].Cells[4].ReadOnly = true; // Diğer durumlarda 4. sütunu düzenlenemez yap
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[0].Cells[4].Value = "";
            dataGridView1.Rows[0].Cells[4].Value = comboBox3.Text;

            bakimTanim = comboBox3.SelectedItem.ToString();  // ComboBox3'ten seçilen Bakım Tanımını al

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT CASE " +
                    "WHEN CONVERT(DATE, aylik1) = @tarih THEN 'aylik1' " +
                    "WHEN CONVERT(DATE, aylik2) = @tarih THEN 'aylik2' " +
                    "WHEN CONVERT(DATE, aylik3) = @tarih THEN 'aylik3' " +
                    "WHEN CONVERT(DATE, aylik4) = @tarih THEN 'aylik4' " +
                    "WHEN CONVERT(DATE, yillik1) = @tarih THEN 'yillik1' " +
                    "END AS header_name " +
                    "FROM yillikGenel " +
                    "WHERE MakId = @MakId AND yil = @yilId " +
                    "AND (CONVERT(DATE, aylik1) = @tarih " +
                    "     OR CONVERT(DATE, aylik2) = @tarih " +
                    "     OR CONVERT(DATE, aylik3) = @tarih " +
                    "     OR CONVERT(DATE, aylik4) = @tarih " +
                    "     OR CONVERT(DATE, yillik1) = @tarih);", conn))
                {
                    try
                    {
                        // Combobox'dan seçilen tarihi dd.MM.yyyy formatından al
                        DateTime tarih = DateTime.ParseExact(bakimTanim, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        // SQL sorgusunda kullanılan tarih değerini ayarla
                        cmd.Parameters.AddWithValue("@tarih", tarih);
                        cmd.Parameters.AddWithValue("@MakId", comboBox2.Text);
                        cmd.Parameters.AddWithValue("@yilId", yilId);
                        string kategori = cmd.ExecuteScalar()?.ToString();  // ExecuteScalar ile değer döndürülüyor

                        // DataGridView'in 1. sütun 3. satırına kategori (aylık/yıllık) değerini ekleyin
                        if (kategori != null)
                        {
                            if (kategori.StartsWith("aylik"))
                            {
                                kategori = "3 AYLIK";
                            }
                            else
                            {
                                kategori = "YILLIK";
                            }
                            dataGridView1.Rows[2].Cells[1].Value = kategori;
                          
                        }
                        else
                        {
                            MessageBox.Show("Veritabanında seçilen tarih için eşleşen bir değer bulunamadı.");
                        }
                    }
                    catch (FormatException ex)
                    {
                        MessageBox.Show( ex.Message + " Hatalı tarih formatı. Lütfen tarihi dd.MM.yyyy formatında giriniz.");
                    }
                }
            }
            yapildi();
        }
        private void bakimekle()
        {
            StringBuilder aciklamaBuilder = new StringBuilder();
            for (int i = 5; i <= 17; i++)
            {
                string cellValue = dataGridView1.Rows[i].Cells[4].Value?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(cellValue))
                {
                    aciklamaBuilder.Append(cellValue).Append(" - ");
                }
            }
            string Aciklama = aciklamaBuilder.ToString().TrimEnd(' ', '-');
            string makID = comboBox2.SelectedItem.ToString();
            string bakimTarihi = comboBox3.SelectedItem.ToString();
            string bakimPersoneli = comboBox4.SelectedItem.ToString();

            DateTime bakimTarihiDate;

            if (!DateTime.TryParseExact(
                    comboBox3.SelectedItem?.ToString(),
                    "dd.MM.yyyy", // Combobox'taki tarih formatı
                    System.Globalization.CultureInfo.GetCultureInfo("tr-TR"),
                    System.Globalization.DateTimeStyles.None,
                    out bakimTarihiDate))
            {
                // Eğer dönüşüm başarısız olursa, bugünün tarihini kullan
            }

            string bakimTanim = dataGridView1.Rows[2].Cells[1].Value?.ToString() ?? string.Empty;
            DateTime today = DateTime.Now;
            string formattedDate = today.ToString("yyyy-MM-dd");

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();

                    // Daha önce aynı MakID ve BakimTarihi var mı kontrol et
                    string kontrolQuery = "SELECT COUNT(*) FROM GecmisBakim WHERE MakID = @MakID AND CONVERT(DATE, BakimTarihi) = @BakimTarihi";
                    using (SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, conn))
                    {
                        kontrolCmd.Parameters.AddWithValue("@MakID", makID);
                        kontrolCmd.Parameters.AddWithValue("@BakimTarihi", bakimTarihiDate.Date); // Tarihi sadece gün, ay, yıl formatında gönder

                        int kayitSayisi = Convert.ToInt32(kontrolCmd.ExecuteScalar());
                        if (kayitSayisi > 0)
                        {
                            MessageBox.Show("Bu Makine için belirtilen tarihte zaten bir bakım kaydı mevcut!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }


                    // Eğer kayıt yoksa devam et
                    string yil = comboBox1.SelectedItem?.ToString() ?? string.Empty;
                    string yilQuery = "SELECT YilId FROM Yillar WHERE Yil = @Yil";
                    using (SqlCommand yilCmd = new SqlCommand(yilQuery, conn))
                    {
                        yilCmd.Parameters.AddWithValue("@Yil", yil);
                        object yilResult = yilCmd.ExecuteScalar();
                        int yilId = (yilResult != DBNull.Value) ? Convert.ToInt32(yilResult) : 0;

                        string query = "INSERT INTO GecmisBakim (MakID, BakimPersoneli, BakimTarihi, BakimTanim, Aciklama, yil, kriter1, kriter2, kriter3, kriter4, kriter5, kriter6, kriter7, kriter8, kriter9, kriter10, kriter11, kriter12, bakimGirisTar) " +
                                       "VALUES (@MakID, @BakimPersoneli, @BakimTarihi, @BakimTanim, @Aciklama, @YilId, @Kriter1, @Kriter2, @Kriter3, @Kriter4, @Kriter5, @Kriter6, @Kriter7, @Kriter8, @Kriter9, @Kriter10, @Kriter11, @Kriter12, @bakimGirisTar)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MakID", makID);
                            cmd.Parameters.AddWithValue("@BakimPersoneli", bakimPersoneli);
                            cmd.Parameters.AddWithValue("@BakimTarihi", bakimTarihiDate);
                            cmd.Parameters.AddWithValue("@BakimTanim",bakimTanim);
                            cmd.Parameters.AddWithValue("@Aciklama",   string.IsNullOrWhiteSpace(Aciklama) ? (object)DBNull.Value : Aciklama);

                            cmd.Parameters.AddWithValue("@YilId", yilId);
                            cmd.Parameters.AddWithValue("@bakimGirisTar", formattedDate);

                            for (int i = 0; i < 12; i++)
                            {
                                int rowIndex = 5 + i; // 5. satırdan başlayarak 12 satır için indeksler artırılır
                                var kriterCell = dataGridView1.Rows[rowIndex].Cells[3]?.Value;
                                cmd.Parameters.AddWithValue("@Kriter" + (i + 1), kriterCell ?? DBNull.Value);
                            }



                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Bakım girişi yapıldı.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }


        private void bkmEkle()
        {

            DialogResult result = MessageBox.Show("Emin misiniz?", "Evet/Hayır", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Kullanıcının cevabına göre işlem yapılır
            // Kullanıcının cevabına göre işlem yapılır
            if (result == DialogResult.Yes)
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
                // Başka kullanıcı adına kayıt oluşturma kontrolü
                else if (CurrentUser.Ad != comboBox4.Text.ToString())
                {
                    MessageBox.Show("Başka kullanıcı adına kayıt oluşturamazsınız.");
                }
                else
                {
                    bakimekle(); // Bakım ekleme işlemi

                }
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            bkmEkle();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[1].Cells[4].Value = "";
            dataGridView1.Rows[1].Cells[4].Value = comboBox4.Text;
            dataGridView1.AutoResizeColumns();  // Tüm sütunların genişliklerini otomatik olarak ayarlar

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Anasayfa anasayfa = new Anasayfa();
            anasayfa.Show();
            this.Hide();
        }
    }
}

