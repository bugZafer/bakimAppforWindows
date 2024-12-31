using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Microsoft.Reporting.WebForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
namespace Bakim
{
    public partial class BakımYap : Form
    {

        public BakımYap()
        {
            InitializeComponent();
            dataGridView1.ColumnHeadersVisible = false;
            this.KeyPreview = true;
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
            Anasayfa form1 = new Anasayfa();
            this.Hide();
            form1.Show();
        }
        string makno = "";

        private void dgWHazirla()
        {
            dataGridView1.Columns.Clear();
            // 14 sütun ekleyelim
            for (int i = 0; i < 14; i++)
            {
                dataGridView1.Columns.Add("Column" + i, "Sütun " + (i + 1));
            }
            // 26 satır ekleyelim
            for (int i = 0; i < 26; i++)
            {
                dataGridView1.Rows.Add();
            }

            // 1. satır, 3. sütun
            dataGridView1.Rows[0].Cells[3].Value = "ÖNLEYİCİ BAKIM FORMU";


            DataGridViewComboBoxCell comboTanim = new DataGridViewComboBoxCell();

            // ComboBox'a seçenekler ekliyoruz
            comboTanim.Items.Add("3 AYLIK");
            comboTanim.Items.Add("YILLIK");
            // Hücreye ComboBox'ı atıyoruz
            dataGridView1.Rows[2].Cells[8] = comboTanim; // 8. sütunda, i. satırdaki hücreye ekliyoruz
            comboTanim.ReadOnly = false;



            dataGridView1.Rows[1].Cells[7].Value = "MAKİNA ADI :";
            dataGridView1.Rows[2].Cells[7].Value = "BAKIM PERİYODU :";
            dataGridView1.Rows[3].Cells[7].Value = "RAPOR NO :";
            dataGridView1.Rows[4].Cells[0].Value = "Sıra no";

            // 5. satır, 2. sütun
            dataGridView1.Rows[4].Cells[1].Value = "Bakımı Yapılacak Özellikler";

            // 5. satır, 8. sütun
            dataGridView1.Rows[4].Cells[6].Value = "Periyod";

            // 5. satır, 9. sütun
            dataGridView1.Rows[4].Cells[7].Value = "Yapıldı";

            // 5. satır, 10. sütun
            dataGridView1.Rows[4].Cells[8].Value = "Açıklamalar";
            // 6. satırdan itibaren 17. satıra kadar sırasıyla sayıları yazdır
            int sayi = 1;
            for (int i = 5; i < 17; i++) // 6. satırdan 17. satıra kadar (index 5-16)
            {
                dataGridView1.Rows[i].Cells[0].Value = sayi; // 1. sütuna yaz
                sayi++;
            }
            // 18. satıra "NOT: YILLIK BAKIMDA TÜM ÖZELLİKLER GÖZÖNÜNE ALINIR." yazdır
            dataGridView1.Rows[17].Cells[0].Value = "NOT: YILLIK BAKIMDA TÜM ÖZELLİKLER GÖZÖNÜNE ALINIR.";
            // 20. satır 1. sütuna "Periyod Sembolleri"
            dataGridView1.Rows[19].Cells[0].Value = "*Periyod Sembolleri";

            // 21. satır 1. sütuna "A: 3AYLIK BAKIM"
            dataGridView1.Rows[20].Cells[0].Value = "A: 3AYLIK BAKIM";

            // 22. satır 1. sütuna "B: YILLIK BAKIM"
            dataGridView1.Rows[21].Cells[0].Value = "B: YILLIK BAKIM";
            // 21. satır 4. sütuna "BAKIM"
            dataGridView1.Rows[19].Cells[5].Value = "BAKIM";

            // 22. satır 3. sütuna "Başlangıç"
            dataGridView1.Rows[20].Cells[4].Value = "Başlangıç";

            // 23. satır 3. sütuna "Tarih-Saati"
            dataGridView1.Rows[21].Cells[4].Value = "Tarih-Saati";

            // 22. satır 8. sütuna "Bitiş"
            dataGridView1.Rows[20].Cells[6].Value = "Bitiş";
            // 23. satır 8. sütuna "Tarihi:Saati"
            dataGridView1.Rows[21].Cells[6].Value = "Tarihi:Saati";


            // 19. satır 10. sütuna "Bakımı Yapan"
            dataGridView1.Rows[19].Cells[8].Value = "Bakımı Yapan";

            // 19. satır 12. sütuna "Onay"
            dataGridView1.Rows[19].Cells[10].Value = "Onay";

            // 20. satır 12. sütuna "İmza"
            dataGridView1.Rows[20].Cells[10].Value = "İmza";
            dataGridView1.Rows[26].Cells[0].Value = "BK-F09/24.07.2018/Rev.0";


        }
        private void ExportToExcel()
        {
            // Excel uygulaması oluştur
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true; // Excel uygulamasını görünür yap

            // Yeni bir çalışma kitabı ekle
            Excel.Workbook workBook = excelApp.Workbooks.Add();
            Excel.Worksheet workSheet = (Excel.Worksheet)workBook.Sheets[1];

            try
            {
                // Veriyi aktar
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value != null)
                        {
                            workSheet.Cells[i + 1, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString(); // Verileri aktar
                        }
                    }
                }
                for (int i = 5; i <= 17; i++)
                {
                    workSheet.Rows[i].RowHeight = 43.5;
                    Excel.Range rowRange = workSheet.Rows[i];
                    rowRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                }
                // Birleştirilmiş hücre aralıklarını sakla
                string[,] mergeRanges = new string[,]
                {
            { "A1:C4", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "D1:G4", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "H1:K1", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "H2:K2", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "H3:K3", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "H4:K4", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B5:F5", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I5:K5", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "A18:K18", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "A20:D20", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "A21:D21", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "A22:D22", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "A19:K19", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "E20:H20", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "E21:F21", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "E22:F22", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "G21:H21", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "G22:H22", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I20:J22", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B6:F6", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B7:F7", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B8:F8", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },

            { "B9:F9", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B10:F10", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B11:F11", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B12:F12", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B13:F13", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B14:F14", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B15:F15", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B16:F16", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "B17:F17", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },

            { "I6:K6", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I7:K7", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I8:K8", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I9:K9", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I10:K10", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I11:K11", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I12:K12", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I13:K13", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I14:K14", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I15:K15", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I16:K16", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I17:K17", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "A27:K27", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
             { "K20", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },

            { "K21:K22", Excel.XlHAlign.xlHAlignCenter.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "A23:D26", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "E23:H26", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() },
            { "I23:K26", Excel.XlHAlign.xlHAlignLeft.ToString(), Excel.XlVAlign.xlVAlignCenter.ToString() }



                };
                workSheet.Columns["K"].ColumnWidth = 16.86;
                workSheet.Columns["x"].ColumnWidth = 16.86;

                // Birleştirme ve hizalama işlemlerini döngü ile yap
                for (int i = 0; i < mergeRanges.GetLength(0); i++)
                {
                    string rangee = mergeRanges[i, 0]; // Hücre aralığı
                    Excel.XlHAlign horizontalAlign = (Excel.XlHAlign)Enum.Parse(typeof(Excel.XlHAlign), mergeRanges[i, 1]); // Yatay hizalama
                    Excel.XlVAlign verticalAlign = (Excel.XlVAlign)Enum.Parse(typeof(Excel.XlVAlign), mergeRanges[i, 2]); // Dikey hizalama

                    Excel.Range r = workSheet.get_Range(rangee);
                    r.Merge(); // Hücreleri birleştir
                    r.HorizontalAlignment = horizontalAlign; // Yatay hizalama
                    r.VerticalAlignment = verticalAlign; // Dikey hizalama
                }





                Excel.Range range = workSheet.get_Range("A1", "K27");
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].Color = System.Drawing.Color.Black;
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].TintAndShade = 0;
                range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;

                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Color = System.Drawing.Color.Black;
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
                range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;

                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Color = System.Drawing.Color.Black;
                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].TintAndShade = 0;
                range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;

                range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlEdgeRight].Color = System.Drawing.Color.Black;
                range.Borders[Excel.XlBordersIndex.xlEdgeRight].TintAndShade = 0;
                range.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;

                // İç çerçeve ekleme
                range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].Color = System.Drawing.Color.Black;
                range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].TintAndShade = 0;
                range.Borders[Excel.XlBordersIndex.xlInsideHorizontal].Weight = Excel.XlBorderWeight.xlThin;

                range.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Borders[Excel.XlBordersIndex.xlInsideVertical].Color = System.Drawing.Color.Black;
                range.Borders[Excel.XlBordersIndex.xlInsideVertical].TintAndShade = 0;
                range.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;



                Excel.Range rangex = workSheet.get_Range("A1", "C4");
                Excel.Range rangex2 = workSheet.get_Range("N1", "P4");

                // Resmi ekle
                Excel.Pictures pictures = (Excel.Pictures)workSheet.Pictures();
                Excel.Picture picture = pictures.Insert("");
              //  Excel.Picture picture = pictures.Insert("C:\\Users\\Zafer\\source\\repos\\Bakim\\Bakim\\img\\birlik.png");

                // Resmin boyutunu değiştirmeden, sadece yerini ortalamak
                picture.Left = rangex.Left + (rangex.Width - picture.Width) / 2;
                picture.Top = rangex.Top + (rangex.Height - picture.Height) / 2;

                Excel.Picture picture2 = pictures.Insert("");
             //   Excel.Picture picture2 = pictures.Insert("C:\\Users\\Zafer\\source\\repos\\Bakim\\Bakim\\img\\birlik.png");

                // Resmin boyutunu değiştirmeden, sadece yerini ortalamak
                picture2.Left = rangex2.Left + (rangex2.Width - picture2.Width) / 2;
                picture2.Top = rangex2.Top + (rangex2.Height - picture2.Height) / 2;



                // Yapıştırılacak hedef hücre: N1
                Excel.Range destinationRange = workSheet.get_Range("N1");






                // Kopyalama işlemi
                range.Copy();

                // Yapıştırma işlemi
                destinationRange.PasteSpecial(Excel.XlPasteType.xlPasteAll);
                // Sayfanın geri kalanını dikey ortalama yap
                Excel.Range allCells = workSheet.UsedRange;
                allCells.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter; // Tüm hücreleri dikey ortala

                allCells.WrapText = true;
                // Excel uygulamasını başlat
                excelApp.Visible = true;



                //   Page SETUP
                Excel.PageSetup pageSetup = workSheet.PageSetup;
                pageSetup.TopMargin = 0;
                pageSetup.BottomMargin = 0;
                pageSetup.LeftMargin = 0;
                pageSetup.RightMargin = 0;
                pageSetup.CenterHorizontally = true;
                pageSetup.CenterVertically = true;
                pageSetup.Zoom = false;
                pageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
            finally
            {
                // Excel nesnelerini serbest bırak
                Marshal.ReleaseComObject(workSheet);
                Marshal.ReleaseComObject(workBook);
                Marshal.ReleaseComObject(excelApp);
            }
        }




        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8) // F8 tuşuna basıldığında
            {
                ExportToExcel(); // Excel'e aktarımı başlat
            }
        }

        private void FillComboBoxWithCategories()
        {
            string query = "SELECT DISTINCT Kategori FROM BakimMetinleri"; // DISTINCT ile sadece benzersiz kategoriler
          

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
                        comboBox1.Items.Clear();

                        // Veritabanından verileri al ve ComboBox'a ekle
                        while (reader.Read())
                        {
                            string kategori = reader["Kategori"].ToString();
                            comboBox1.Items.Add(kategori);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
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
                            comboBox2.Items.Add(kategori); // ComboBox'a ekle
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

        private void Form2_Load(object sender, EventArgs e)
        {
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;

            dgWHazirla();
            FillComboBoxWithCategories();
            PersonelList();
            for (int cmb = 1; cmb <= 60; cmb++)
            {
                string value = cmb.ToString("D2");  // İki basamaklı hale getirme (01, 02, ..., 60)
                comboBox3.Items.Add(value);
            }

            comboBox3.Enabled = false;

            string secilenDate = dateTimePicker1.Text.ToString();

            // DataGridView'deki 23. satırın 9. sütununu güncelle
            if (dataGridView1.Rows.Count >= 23) // Satır sayısını kontrol et
            {
                dataGridView1.Rows[22].Cells[5].Value = secilenDate; // 23. satır (index 22) ve 9. sütun (index 8)
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    // Eğer hücre ComboBox ise, onu hariç tutuyoruz, diğer hücreleri readonly yapıyoruz
                    if (!(cell is DataGridViewComboBoxCell))
                    {
                        cell.ReadOnly = true;  // Diğer tüm hücreleri readonly yapıyoruz
                    }
                }
            }
            // DataGridView'deki tüm hücreleri kontrol ediyoruz
            // Tüm DataGridView hücrelerini kontrol et
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    // Eğer hücre 8. sütundaysa (index 7)
                    if (cell.ColumnIndex == 8)
                    {
                        // 5. satır (index 4) ile 17. satır (index 16) arasındaki hücreleri düzenlenebilir yap
                        if (row.Index == 2 || row.Index >= 5 && row.Index <= 16)
                        {
                            cell.ReadOnly = false; // Bu hücreyi düzenlenebilir yapıyoruz
                        }
                        else
                        {
                            cell.ReadOnly = true; // Diğer hücreleri readonly yapıyoruz
                        }
                    }
                    else
                    {
                        // 8. sütun dışında kalan tüm hücreleri readonly yapıyoruz
                        cell.ReadOnly = true;
                    }
                }
            }


            dataGridView1.MinimumSize = new Size(dataGridView1.Width, dataGridView1.Height);

            // Anchor özelliğini sadece aşağı ve sağ kenarlara sabitleyin
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;




        }


        string mk = "";
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            mk=  comboBox1.Text;
            comboBox3.Enabled = true;
            // Combobox'tan seçilen kategoriyi al
            string selectedCategory = comboBox1.SelectedItem.ToString();

            // Veritabanı bağlantısını oluştur
            using (SqlConnection conn = new SqlConnection("Server=ZAFER-HP\\SQLEXPRESS;Database=Bakim;Integrated Security=True;"))
            {
                conn.Open();

                // Sorgu: Kategoriyi seçip Metin ve Periyod sütunlarını çekiyoruz
                string query = "SELECT Metin, Periyod FROM BakimMetinleri WHERE Kategori = @Kategori";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Kategori", selectedCategory);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int rowIndex = 5; // DataGridView'in 6. satırı (index 5)

                        // DataGridView'deki 6. satırdan 17. satıra kadar hücreleri temizle
                        for (int i = rowIndex; i < 17; i++)
                        {
                            if (i < dataGridView1.Rows.Count)
                            {
                                dataGridView1.Rows[i].Cells[1].Value = ""; // 2. sütun
                                dataGridView1.Rows[i].Cells[6].Value = ""; // 3. sütun
                                if (dataGridView1.Rows[i].Cells[7] is DataGridViewComboBoxCell)
                                {
                                    // Mevcut ComboBox hücresini temizle ve yerine başka bir hücre ekle
                                    dataGridView1.Rows[i].Cells[7] = new DataGridViewTextBoxCell();
                                }
                            }
                        }

                        // Eğer ComboBox'ta bir seçim yapıldıysa
                        if (comboBox1.SelectedItem != null)
                        {
                            // Seçilen öğenin metnini al
                            string selectedText = comboBox1.SelectedItem.ToString();

                            // 0. satır, 7. sütuna yaz
                            makno = "Makine No : " + selectedText;
                            dataGridView1.Rows[0].Cells[7].Value = makno;


                        }
                        else
                        {
                            // Eğer seçim yapılmadıysa, "Seçilmedi" yazsın
                            dataGridView1.Rows[0].Cells[7].Value = "Makine No : Seçilmedi";
                        }
                        // Veritabanından okunan her satır için DataGridView'e yazma işlemi
                        while (reader.Read())
                        {
                            if (rowIndex < dataGridView1.Rows.Count)
                            {
                                // 2. sütuna "Metin", 3. sütuna "Periyod" verisini yaz
                                dataGridView1.Rows[rowIndex].Cells[1].Value = reader["Metin"].ToString();
                                dataGridView1.Rows[rowIndex].Cells[6].Value = reader["Periyod"].ToString();
                                // ComboBox hücresini oluşturuyoruz
                                DataGridViewComboBoxCell comboCell = new DataGridViewComboBoxCell();

                                // ComboBox'a seçenekler ekliyoruz
                                comboCell.Items.Add("Evet");
                                comboCell.Items.Add("Hayır");
                                // Hücreye ComboBox'ı atıyoruz
                                comboCell.Value = "Evet";
                                dataGridView1.Rows[rowIndex].Cells[7] = comboCell; // 8. sütunda, i. satırdaki hücreye ekliyoruz
                                comboCell.ReadOnly = false;

                            }
                            else
                            {
                                // Eğer satır sayısı yetersizse yeni satır ekle
                                int newRowIndex = dataGridView1.Rows.Add();
                                dataGridView1.Rows[newRowIndex].Cells[1].Value = reader["Metin"].ToString();
                                dataGridView1.Rows[newRowIndex].Cells[6].Value = reader["Periyod"].ToString();
                            }

                            rowIndex++; // Sonraki satıra geç
                                        // 8. sütun, 6. satıra ComboBox hücresini yerleştir



                        }
                    }
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string secilenPersonel = comboBox2.SelectedItem.ToString();

            // DataGridView'deki 23. satırın 9. sütununu güncelle
            if (dataGridView1.Rows.Count >= 23) // Satır sayısını kontrol et
            {
                dataGridView1.Rows[22].Cells[8].Value = secilenPersonel; // 23. satır (index 22) ve 9. sütun (index 8)
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            makno = "Makine No:"+mk;
            makno += comboBox3.SelectedItem.ToString();
            dataGridView1.Rows[0].Cells[7].Value = makno;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
        private void bakimekle()
        {
            StringBuilder aciklamaBuilder = new StringBuilder();

            // 5. satırdan 17. satıra kadar 8. sütundaki verileri birleştir
            for (int i = 5; i <= 17; i++)
            {
                string cellValue = dataGridView1.Rows[i].Cells[8].Value?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(cellValue))
                {
                    aciklamaBuilder.Append(cellValue);
                    aciklamaBuilder.Append(" - ");
                }
            }

            string Aciklama = aciklamaBuilder.ToString().Trim();
            string makID = comboBox1.SelectedItem.ToString() + comboBox3.SelectedItem.ToString();
            string bakimPersoneli = dataGridView1.Rows[22].Cells[8].Value?.ToString() ?? string.Empty;
            string bakimTarihi = dataGridView1.Rows[22].Cells[5].Value?.ToString() ?? string.Empty;

            // Tarih formatını kontrol et ve dönüştür
            DateTime bakimTarihiDate = DateTime.TryParseExact(
                bakimTarihi,
                "d MMMM yyyy dddd",
                System.Globalization.CultureInfo.GetCultureInfo("tr-TR"),
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate
            ) ? parsedDate : DateTime.Now;

            // Bakım tanımı için ComboBox değerini al
            DataGridViewComboBoxCell comboCell = dataGridView1.Rows[2].Cells[8] as DataGridViewComboBoxCell;
            string bakimTanim = comboCell?.Value?.ToString() ?? string.Empty;
            DateTime today = DateTime.Now;
            string formattedDate = today.ToString("yyyy-MM-dd");
            using (SqlConnection conn = new SqlConnection("Server=ZAFER-HP\\SQLEXPRESS;Database=Bakim;Integrated Security=True;"))
            {
                try
                {
                    conn.Open();

                    // Yıl değerini al
                    int yil = dateTimePicker1.Value.Year;



                    // YılId'yi almak için sorgu
                    string yilQuery = "SELECT YilId FROM Yillar WHERE Yil = @Yil";
                    using (SqlCommand yilCmd = new SqlCommand(yilQuery, conn))
                    {
                        yilCmd.Parameters.AddWithValue("@Yil", yil);
                        object yilResult = yilCmd.ExecuteScalar();
                        int yilId = (yilResult != DBNull.Value) ? Convert.ToInt32(yilResult) : 0;

                        // Insert sorgusuna YilId'yi ekleme
                        string query = "INSERT INTO GecmisBakim (MakID, BakimPersoneli, BakimTarihi, BakimTanim, Aciklama, yil, kriter1, kriter2, kriter3, kriter4, kriter5, kriter6, kriter7, kriter8, kriter9, kriter10, kriter11, kriter12,bakimGirisTar) " +
                                       "VALUES (@MakID, @BakimPersoneli, @BakimTarihi, @BakimTanim, @Aciklama, @YilId, @Kriter1, @Kriter2, @Kriter3, @Kriter4, @Kriter5, @Kriter6, @Kriter7, @Kriter8, @Kriter9, @Kriter10, @Kriter11, @Kriter12,@bakimGirisTar)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MakID", makID);
                            cmd.Parameters.AddWithValue("@BakimPersoneli", bakimPersoneli);
                            cmd.Parameters.AddWithValue("@BakimTarihi", bakimTarihiDate);
                            cmd.Parameters.AddWithValue("@BakimTanim", bakimTanim);
                            cmd.Parameters.AddWithValue("@Aciklama", Aciklama);
                            cmd.Parameters.AddWithValue("@YilId", yilId); // Yıl ID'si ekleniyor
                            cmd.Parameters.AddWithValue("@bakimGirisTar", formattedDate); // Yıl ID'si ekleniyor

                            // Kriterler için döngü
                            for (int i = 0; i < 12; i++)
                            {
                                int rowIndex = 5 + i; // 5. satırdan başlayarak 12 satır için indeksler artırılır
                                DataGridViewComboBoxCell kriterCell = dataGridView1.Rows[rowIndex].Cells[7] as DataGridViewComboBoxCell; // 7. sütun (index 6)

                                if (kriterCell == null || kriterCell.Value == null || Convert.IsDBNull(kriterCell.Value))
                                {
                                    cmd.Parameters.AddWithValue("@Kriter" + (i + 1), DBNull.Value);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Kriter" + (i + 1), kriterCell.Value.ToString());
                                }
                            }

                            cmd.ExecuteNonQuery();
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
            if (result == DialogResult.Yes)
            {
                // ComboBox'ların ve DateTimePicker'ın doluluğunu kontrol et
                if (comboBox1.SelectedIndex == -1 || // comboBox1 seçilmemişse
                    comboBox2.SelectedIndex == -1 || // comboBox2 seçilmemişse
                    comboBox3.SelectedIndex == -1 || // comboBox3 seçilmemişse
                    string.IsNullOrEmpty(dateTimePicker1.Text))
                {
                    // Uyarı mesajı göster
                    MessageBox.Show("Eksik bilgi var. Lütfen ilgili alanları doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi sonlandır
                }
                if (dataGridView1.Rows.Count > 2 && dataGridView1.Columns.Count > 8) // Geçerli satır ve sütun aralığını kontrol et
                {
                    DataGridViewComboBoxCell comboCell = dataGridView1.Rows[2].Cells[8] as DataGridViewComboBoxCell;

                    if (comboCell != null) // Hücre ComboBoxCell mi?
                    {
                        string bakimTanim = comboCell?.Value?.ToString() ?? string.Empty;

                        if (string.IsNullOrEmpty(bakimTanim)) // Hücrede bir değer var mı?
                        {
                            MessageBox.Show("Bakım tanımı seçilmemiş. Lütfen ilgili alanı doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // İşlemi sonlandır
                        }
                    }
                    else
                    {
                        MessageBox.Show("Belirtilen hücre bir ComboBoxCell değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // İşlemi sonlandır
                    }
                }
                else
                {
                    MessageBox.Show("DataGridView'de belirtilen satır veya sütun geçerli değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // İşlemi sonlandır
                }


                // Tüm alanlar doluysa bakimekle() metodunu çağır
                bakimekle();
                MessageBox.Show("Bakım Onaylandı.");
            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("İşlem İptal edildi.");

            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string adsy = CurrentUser.Ad;
            if (adsy!= comboBox2.Text)
            {
                MessageBox.Show($"{comboBox2.Text} için işlem yapamazsınız." , CurrentUser.Ad );

            }
            else
            {
                bkmEkle();
            }


        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            string secilenDate = dateTimePicker1.Text.ToString();

            // DataGridView'deki 23. satırın 9. sütununu güncelle
            if (dataGridView1.Rows.Count >= 23) // Satır sayısını kontrol et
            {
                dataGridView1.Rows[22].Cells[5].Value = secilenDate; // 23. satır (index 22) ve 9. sütun (index 8)
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Anasayfa form = new Anasayfa();
            form.Show();
            this.Hide();
            this.Close();

        }
    }
}
