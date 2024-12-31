using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
namespace Bakim
{
    public partial class Anasayfa : Form
    {
        private List<DateTime> ucAylikBakimlar = new List<DateTime>();
        private List<DateTime> yillikBakimlar = new List<DateTime>();
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
        public Anasayfa()
        {
            InitializeComponent();
            this.KeyPreview = true;

        }
        private void ExportToExcel()
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            try
            {
                int rowCount = dataGridView1.RowCount;
                int colCount = dataGridView1.ColumnCount;

                // Başlıkları aktar
                for (int col = 0; col < colCount; col++)
                {
                    worksheet.Cells[1, col + 1] = dataGridView1.Columns[col].HeaderText;
                }

                // Verileri 2D diziye aktar
                object[,] data = new object[rowCount, colCount];
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < colCount; col++)
                    {
                        data[row, col] = dataGridView1.Rows[row].Cells[col].Value;
                    }
                }

                // Veriyi Excel'e topluca yaz
                Excel.Range dataRange = worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[rowCount + 1, colCount]];
                dataRange.Value = data;

                // Tüm hücrelere sınır ekle
                Excel.Range allRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[rowCount + 1, colCount]];
                allRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                allRange.Borders.Weight = Excel.XlBorderWeight.xlThin;
                allRange.Borders.Color = Color.Black.ToArgb();

                // Renklendirme
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < colCount; col++)
                    {
                        var cellValue = data[row, col];

                        // İlk sütun için özel renklendirme
                        if (col == 0)
                        {
                            if (row == 0) // 1. hücre yeşil
                            {
                                worksheet.Cells[row + 2, col + 1].Interior.Color = ColorTranslator.FromHtml("#92D050").ToArgb();
                            }
                            else if (row == 1) // 2. hücre mavi
                            {
                                worksheet.Cells[row + 2, col + 1].Interior.Color = ColorTranslator.FromHtml("#FFC000").ToArgb();
                            }
                            else if (row == 2) // 3. hücre turuncu
                            {
                                worksheet.Cells[row + 2, col + 1].Interior.Color = ColorTranslator.FromHtml("#00B0F0").ToArgb();
                            }
                        }
                        else if (col > 4) // 5. sütundan sonrası için
                        {
                            if (DateTime.TryParse(cellValue?.ToString(), out DateTime cellDate))
                            {
                                if (ucAylikBakimlar.Contains(cellDate)) // Yıllık bakım
                                {
                                    worksheet.Cells[row + 2, col + 1].Interior.Color = ColorTranslator.FromHtml("#92D050").ToArgb();
                                    if (row < rowCount - 1) // Eğer son satır değilse, bir alt satırda işlem yapılabilir
                                    {
                                        worksheet.Cells[row + 3, col + 1].Interior.Color = ColorTranslator.FromHtml("#92D050").ToArgb();
                                    }
                                }
                                else if (yillikBakimlar.Contains(cellDate)) // 3 Aylık bakım
                                {
                                    worksheet.Cells[row + 2, col + 1].Interior.Color = ColorTranslator.FromHtml("#00B0F0").ToArgb();
                                    if (row < rowCount - 1) // Eğer son satır değilse, bir alt satırda işlem yapılabilir
                                    {
                                        worksheet.Cells[row + 3, col + 1].Interior.Color = ColorTranslator.FromHtml("#00B0F0").ToArgb();
                                    }
                                }

                            }
                        }

                    }

                }
                for (int col = 1; col <= 5; col++) // İlk 5 sütun için döngü
                {
                    // 4. ve 5. satırdaki hücreleri birleştiriyoruz
                    worksheet.Range[worksheet.Cells[6, col], worksheet.Cells[7, col]].Merge();

                    // Format boyası ile 4. ve 5. satırdaki hücrenin biçimlerini 6. satırdan itibaren uyguluyoruz
                    Excel.Range formatRange = worksheet.Range[worksheet.Cells[6, col], worksheet.Cells[7, col]];

                    // Format boyasını kopyalayalım
                    formatRange.Copy();

                    // Format boyasını 6. satırdan başlayıp en alt satıra kadar olan aralığa yapıştıralım
                    int lastRow = rowCount; // En son satır
                    Excel.Range applyFormatRange = worksheet.Range[worksheet.Cells[8, col], worksheet.Cells[lastRow, col]];

                    applyFormatRange.PasteSpecial(Excel.XlPasteType.xlPasteFormats);
                }

                // Excel uygulaması açıldıktan sonra

                // Yatayda ortalamak için
                allRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                // Dikeyde ortalamak için
                allRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                // Başlıkları birleştir
                Excel.Range headerRange = worksheet.Range[worksheet.Cells[2, 3], worksheet.Cells[3, colCount]];
                Excel.Range rangeToMerge = worksheet.Range[worksheet.Cells[4, 7], worksheet.Cells[4, colCount]];
                // İlk satırdaki tüm hücreleri birleştirmek için
                Excel.Range firstRowRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, colCount]];

                rangeToMerge.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                firstRowRange.Merge();
                Excel.Range son3 = worksheet.Range[worksheet.Cells[4, 4], worksheet.Cells[5, 4]];
                son3.Merge();
                Excel.Range sonyil = worksheet.Range[worksheet.Cells[4, 5], worksheet.Cells[5, 5]];
                sonyil.Merge();
                rangeToMerge.Merge();
                headerRange.Merge();
                headerRange.Value = "YILLIK BAKIM PLANI";
                headerRange.Font.Bold = true;
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                headerRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                headerRange.Font.Size = 42;
                // Sütunları otomatik genişlet
                worksheet.Columns.AutoFit();
                worksheet.Rows.AutoFit();
                // İlk 5 satırdaki tüm verileri bold yapmak için
                Excel.Range rangeToBold = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[5, colCount]];
                rangeToBold.Font.Bold = true;

                // Excel'i göster
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                // Kaynakları serbest bırak
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(excelApp);
            }
        }

        private void SetupDataGridView()
        {
            dataGridView1.Rows.Clear();
            // DataGridView'ye toplamda 2 (başlık) + 52 (hafta) + 2 (SON 3AY ve SON YILLIK) + 1 (HAFTALAR) olmak üzere toplam 57 sütun ekliyoruz
            dataGridView1.ColumnCount = 58;
            dataGridView1.Columns[0].HeaderText = "";
            dataGridView1.Columns[1].HeaderText = "";
            dataGridView1.Rows.Add(" ", "PLANLANAN");
            dataGridView1.Rows.Add("XX.XX.2024", "GERÇEKLEŞEN");
            dataGridView1.Rows.Add("XX.XX.2024", "3 AYLIK BAKIM", "BK-F02/09.07.2018/Rev.1");
            dataGridView1.Rows.Add("SIRA NUMARASI", "MAKİNE KODU");
            dataGridView1.Rows[2].Cells[5].Value = "YIL";
            dataGridView1.Rows[2].Cells[6].Value = "2024";
            dataGridView1.Rows[3].Cells[2].Value = "Makine Sorumlusu";
            dataGridView1.Rows[3].Cells[3].Value = "SON 3AY";
            dataGridView1.Rows[3].Cells[4].Value = "SON YILLIK";
            dataGridView1.Rows[3].Cells[5].Value = "HAFTALAR";




            // Haftalar başlıklarını 4. satıra ekleyelim
            for (int i = 0; i < 52; i++) // 52 hafta başlıklarını ekleyelim
            {

                dataGridView1.Rows[3].Cells[i + 6].Value = $" {i + 1}";
            }
        }
        private void LoadData()
        {
            // Veritabanı bağlantı dizesi
           
            // SQL sorgusu
            string query = "SELECT MakID, Son3AyTarih, Son1YilTarih, SorPersonel FROM BakimTarih";

            // SQL bağlantısı oluşturma
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                // SQL komutunu oluşturma
                SqlCommand command = new SqlCommand(query, connection);

                // Veritabanına bağlan
                connection.Open();

                // Veriyi alıp bir DataTable'a aktaralım
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);

                // DataGridView'e veri yükleme
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {

                    int rowIndex = dataGridView1.Rows.Add();
                    // 0. sütun: Sıra numarası
                    dataGridView1.Rows[rowIndex].Cells[0].Value = i + 1; // Sıra numarasını ekleyin
                    dataGridView1.Rows[rowIndex].Cells[1].Value = dataTable.Rows[i]["MakID"].ToString();

                    // 2. sütun: SonPersonel
                    dataGridView1.Rows[rowIndex].Cells[2].Value = dataTable.Rows[i]["SorPersonel"].ToString();

                    // 3. sütun: Son3AyTarih (Tarihi dd.MM.yyyy formatında yaz)
                    DateTime son3AyTarih = Convert.ToDateTime(dataTable.Rows[i]["Son3AyTarih"]);
                    dataGridView1.Rows[rowIndex].Cells[3].Value = son3AyTarih.ToString("dd.MM.yyyy");

                    // 4. sütun: Son1YilTarih (Tarihi dd.MM.yyyy formatında yaz)
                    DateTime son1YilTarih = Convert.ToDateTime(dataTable.Rows[i]["Son1YilTarih"]);
                    dataGridView1.Rows[rowIndex].Cells[4].Value = son1YilTarih.ToString("dd.MM.yyyy");
                    BosSatir();

                    PG();

                }
            }
        }

        private int GetWeekNumber(DateTime tarih)
        {

            var calendar = CultureInfo.InvariantCulture.Calendar;

            // FirstDayOfWeek kullanarak haftanın doğru hesaplanmasını sağlıyoruz
            return calendar.GetWeekOfYear(tarih, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                ExportToExcel();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            yeniFormEkle form2 = new yeniFormEkle();
            this.Hide();
            form2.Show();
        }
        private void PG()
        {
            // Haftalar için işlemler
            for (int i = 4; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (i % 2 == 0)
                {
                    dataGridView1.Rows[i].Cells[5].Value = "P"; // P harfi
                }
                else
                {
                    dataGridView1.Rows[i].Cells[5].Value = "G"; // G harfi
                }
            }

        }
        private void BosSatir()
        {
            int emptyRowIndex = dataGridView1.Rows.Add();
            for (int col = 1; col < dataGridView1.ColumnCount; col++)
            {
                dataGridView1.Rows[emptyRowIndex].Cells[col].Value = ""; // Boş satır
            }

            // DataGridView'e boş satır ekle
        }
        private DateTime RandomTarihSec(DateTime baslangicTarih, DateTime bitisTarih, string connectionString)
        {
            Random random = new Random();
            TimeSpan fark = bitisTarih - baslangicTarih;
            DateTime randomTarih;

            bool tarihVarMi;

            // İlk hafta içindeki tarihleri kontrol et
            do
            {
                // Rastgele gün seç
                int randomDay = random.Next(0, fark.Days);
                randomTarih = baslangicTarih.AddDays(randomDay);
                randomTarih = randomTarih.Date;  // Saat bilgisini sıfırla

                // Hafta sonu kontrolü (Cumartesi veya Pazar)
                if (randomTarih.DayOfWeek == DayOfWeek.Saturday)
                {
                    randomTarih = randomTarih.AddDays(2); // Cumartesi ise Pazartesi'ye al
                }
                else if (randomTarih.DayOfWeek == DayOfWeek.Sunday)
                {
                    randomTarih = randomTarih.AddDays(1); // Pazar ise Pazartesi'ye al
                }

                // İzinli tarihler veritabanı kontrolü
                tarihVarMi = CheckIfDateExistsInIzinliTarihler(randomTarih, GetConnectionString());

                // Eğer tarih izinli tarihlerle çakışıyorsa, yeni tarih seç
                if (tarihVarMi)
                {
                    // Eğer o hafta içinde izinli bir tarih varsa, bir sonraki haftaya geç
                    DateTime oneWeekLater = randomTarih.AddDays(7);  // 1 hafta sonrası
                    randomTarih = oneWeekLater;
                    fark = oneWeekLater - baslangicTarih;  // Bir sonraki haftayı kontrol et
                }

            } while (tarihVarMi);  // Eğer tarih izinli tarihlerle çakışıyorsa, yeni tarih seç

            // Yıl kontrolü
            if (randomTarih.Year < DateTime.Now.Year)
            {
                randomTarih = new DateTime(DateTime.Now.Year, 1, 1);
            }

            return randomTarih;
        }


        public bool CheckIfDateExistsInIzinliTarihler(DateTime tarih, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM izinliTarihler WHERE tarih = @Tarih";

                // SqlCommand nesnesi için 'using' bloğu ekledik
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // DateTime parametresi için doğru türde parametre ekleyelim
                    cmd.Parameters.Add("@Tarih", SqlDbType.DateTime).Value = tarih;

                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    // Eğer tarih bulunmuşsa, çakışma var demektir
                    return count > 0;
                }
            }
        }




        private void HesaplaVeYaz()
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Geçerli bir yıl girin.");
            }
            else
            {
                // TextBox'taki değeri al
                if (int.TryParse(textBox1.Text, out int yil))
                {
                   
                    using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                    {
                        try
                        {
                            conn.Open();

                            // Yılın zaten veritabanında olup olmadığını kontrol et
                            string checkQuery = "SELECT YilId FROM Yillar WHERE Yil = @Yil";
                            int yilId = 0;  // YilId'yi burada saklayacağız

                            using (SqlCommand cmdCheck = new SqlCommand(checkQuery, conn))
                            {
                                cmdCheck.Parameters.AddWithValue("@Yil", yil);
                                var result = cmdCheck.ExecuteScalar();

                                if (result == null)
                                {
                                    // Yıl Yillar tablosuna ekleniyor
                                    string insertQuery = "INSERT INTO Yillar (Yil) VALUES (@Yil); SELECT SCOPE_IDENTITY();";
                                    using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
                                    {
                                        cmdInsert.Parameters.AddWithValue("@Yil", yil);
                                        var newYilId = cmdInsert.ExecuteScalar(); // Yeni YilId'yi al

                                        yilId = Convert.ToInt32(newYilId);
                                    }
                                }
                                else
                                {
                                    yilId = Convert.ToInt32(result); // Yıl zaten var, YilId'yi al
                                    MessageBox.Show($"Yıl ({yil}) zaten mevcut.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }

                            // Veritabanına yazmaya başla
                            for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
                            {
                                string makId = dataGridView1.Rows[rowIndex].Cells[1].Value?.ToString();
                                string pers = dataGridView1.Rows[rowIndex].Cells[2].Value?.ToString();

                                DateTime son3AyTarih;
                                DateTime son1YilTarih;
                                DateTime[] aylikTarihler = new DateTime[4];
                                DateTime randomDate1Yil = DateTime.MinValue;

                                // 4. sütundaki (Son 3 Ay) tarihi al
                                if (DateTime.TryParse(dataGridView1.Rows[rowIndex].Cells[3].Value?.ToString(), out son3AyTarih))
                                {
                                    DateTime enSon3AyTarihi = DateTime.MinValue;

                                    for (int i = 1; i <= 4; i++)
                                    {
                                        DateTime tarih3Ay = son3AyTarih.AddMonths(i * 3);

                                        DateTime randomDate3Ay = RandomTarihSec(tarih3Ay.AddDays(-2), tarih3Ay.AddDays(2), GetConnectionString());
                                        aylikTarihler[i - 1] = randomDate3Ay;

                                        if (randomDate3Ay > enSon3AyTarihi)
                                        {
                                            enSon3AyTarihi = randomDate3Ay;
                                        }

                                        int weekNumber = GetWeekNumber(randomDate3Ay); // Hesapladığımız haftanın numarası
                                        int targetColumnIndex = weekNumber + 5;
                                        if (targetColumnIndex < dataGridView1.Columns.Count)
                                        {
                                            dataGridView1.Rows[rowIndex].Cells[targetColumnIndex].Value = randomDate3Ay.ToString("dd.MM.yyyy");
                                        }
                                    }

                                    son3AyTarih = enSon3AyTarihi;
                                }

                                // 5. sütundaki (Son 1 Yıl) tarihi al
                                if (DateTime.TryParse(dataGridView1.Rows[rowIndex].Cells[4].Value?.ToString(), out son1YilTarih))
                                {
                                    DateTime son1YilSonra = son1YilTarih.AddYears(1);
                                    randomDate1Yil = RandomTarihSec(son1YilSonra.AddDays(-2), son1YilSonra.AddDays(2), GetConnectionString());

                                    // Veritabanına yaz
                                    VeritabaninaYaz(makId, aylikTarihler, randomDate1Yil, yilId);

                                    int weekNumber = GetWeekNumber(randomDate1Yil); // Hesapladığımız haftanın numarası
                                    int targetColumnIndex = weekNumber + 5;
                                    if (targetColumnIndex < dataGridView1.Columns.Count)
                                    {
                                        dataGridView1.Rows[rowIndex].Cells[targetColumnIndex].Value = randomDate1Yil.ToString("dd.MM.yyyy");
                                    }
                                }

                                // Makine kodu geçerli değilse veritabanına yaz
                                if (makId != "PLANLANAN" && makId != "GERÇEKLEŞEN" && makId != "3 AYLIK BAKIM" && makId != "MAKİNE KODU" && !string.IsNullOrEmpty(makId))
                                {
                                    sonDbYaz(makId, son3AyTarih, randomDate1Yil, pers);
                                }
                            }

                            MessageBox.Show("Yeni Plan oluşturuldu.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir yıl giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }


        //son bakim tarihlerini yazdıran void
        private void sonDbYaz(string makId, DateTime son3AyTarih, DateTime son1YilTarih, string personel)
        {
          

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                // SQL sorgusu: Eğer kayıt varsa güncelle, yoksa ekle
                string mergeQuery = @"
IF EXISTS (SELECT 1 FROM BakimTarih WHERE MakId = @MakId)
BEGIN
    UPDATE BakimTarih
    SET Son3AyTarih = @Son3AyTarih,
        Son1YilTarih = @Son1YilTarih,
        SorPersonel = @SorPersonel
    WHERE MakId = @MakId;
END
ELSE
BEGIN
    INSERT INTO BakimTarih (MakId, Son3AyTarih, Son1YilTarih, SorPersonel)
    VALUES (@MakId, @Son3AyTarih, @Son1YilTarih, @SorPersonel);
END";

                using (SqlCommand command = new SqlCommand(mergeQuery, connection))
                {


                    // Parametreleri ekle
                    command.Parameters.AddWithValue("@MakId", makId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Son3AyTarih", son3AyTarih.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@Son1YilTarih", son1YilTarih.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@SorPersonel", personel ?? (object)DBNull.Value);

                    if (makId != "PLANLANAN" && makId != "GERÇEKLEŞEN" && makId != "3 AYLIK BAKIM" && makId != "MAKİNE KODU" && makId != "")
                    {
                        // Veritabanına yazma işlemi yapılacak
                        command.ExecuteNonQuery();
                    }

                }


            }
        }
        private void VeritabaninaYaz(string makId, DateTime[] aylikTarihler, DateTime randomDate1Yil, int yilId)
        {
            
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string insertQuery = "INSERT INTO yillikGenel (MakId, aylik1, aylik2, aylik3, aylik4, yillik1, Yil) " +
                                     "VALUES (@makId, @aylik1, @aylik2, @aylik3, @aylik4, @yillik1, @YilId)";
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    if (makId != "PLANLANAN" && makId != "GERÇEKLEŞEN" && makId != "3 AYLIK BAKIM" && makId != "MAKİNE KODU" && makId != "") { 


                        // Parametreleri ekliyoruz
                        command.Parameters.AddWithValue("@aylik1", aylikTarihler[0].ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@aylik2", aylikTarihler[1].ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@aylik3", aylikTarihler[2].ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@aylik4", aylikTarihler[3].ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@yillik1", randomDate1Yil.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@YilId", yilId);  // YilId parametresi ekleniyor
                                                                       // `makId` parametresini ekliyoruz
                    command.Parameters.AddWithValue("@makId", makId ?? (object)DBNull.Value);

                    // Veritabanını güncellemek için sorguyu çalıştırıyoruz
                    command.ExecuteNonQuery();
                }
            }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {

            // Kullanıcının cevabına göre işlem yapılır

                // Şifre soruluyor
                string correctPassword = "."; // Şifreniz burada tanımlı
                string inputPassword = Microsoft.VisualBasic.Interaction.InputBox("Lütfen şifreyi girin:", "Şifre Girişi", "");

                if (inputPassword == correctPassword)
                {
                    dataGridView1.Rows.Clear();
                    SetupDataGridView();
                    LoadData();
                    HesaplaVeYaz();
                }
                else
                {
                    MessageBox.Show("Hatalı şifre! İşlem iptal edildi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

        }


        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            SetupDataGridView();
            LoadData();
            // DataGridView'den veriyi alıyoruz (4. sütun, 4. satır)
            var veri = dataGridView1.Rows[4].Cells[4].Value;

            if (veri != null)
            {
                try
                {
                    // Tarihi dd.MM.yyyy formatında alıyoruz ve DateTime'e çeviriyoruz
                    DateTime tarih = DateTime.ParseExact(veri.ToString(), "dd.MM.yyyy", null);

                    // Yılı alıp +1 ekliyoruz
                    int yeniYil = tarih.Year + 1;

                    // Yeni yılı TextBox1'e yazdırıyoruz
                    textBox1.Text = yeniYil.ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Geçerli bir tarih formatı değil.");
                }
            }
            else
            {
                MessageBox.Show("Hücrede veri yok.");
            }
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            // DataGridView'in minimum yüksekliğini form yükseklik başlangıç boyutuna sabitleyin
            dataGridView1.MinimumSize = new Size(dataGridView1.Width, dataGridView1.Height);

            // Anchor özelliğini sadece aşağı ve sağ kenarlara sabitleyin
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            if (CurrentUser.Username != "admin")
            {
                button1.Enabled = false;
                button3.Enabled = false;
                button6.Enabled = false;

            }

        }
        private void button4_Click(object sender, EventArgs e)
        {
            GecmisPlanlar form4 = new GecmisPlanlar();
            form4.Show();
            this.Visible = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Tuşu engelle
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            UyumGoruntule form3 = new UyumGoruntule();
            form3.Show();
            this.Hide();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            VeriYaz personelEkle = new VeriYaz();
            personelEkle.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            haftalikGoruntule haftalikGoruntule = new haftalikGoruntule();
            haftalikGoruntule.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            formGoruntule formgoruntule = new formGoruntule();
            formgoruntule.Show();
            this.Hide();
        }
    }
}
