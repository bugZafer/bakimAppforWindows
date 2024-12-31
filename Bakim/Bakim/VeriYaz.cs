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
    public partial class VeriYaz : Form
    {
        public VeriYaz()
        {
            InitializeComponent();
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
        private void button2_Click(object sender, EventArgs e)
        {
            Anasayfa form1 = new Anasayfa();
            form1.Show();
            this.Hide();
            this.Close();
        }
        private void tarihEkle()
        {// TextBox2'deki tarihi al
            string tarihStr = textBox2.Text;

            // Tarih formatını kontrol et ve doğru formata dönüştür
            DateTime tarih;
            if (DateTime.TryParseExact(tarihStr, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out tarih))
            {
                
               
                try
                {
                    using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                    {
                        conn.Open();

                        // SQL Insert komutu
                        string query = "INSERT INTO izinliTarihler (tarih) VALUES (@tarih)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametre ekle
                            cmd.Parameters.AddWithValue("@tarih", tarih);

                            // Sorguyu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Tarih başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir tarih formatı giriniz (gg.aa.yyyy).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PerEkle()
        {
            // TextBox2'deki ismi al
            string personelIsmi = textBox1.Text;

            if (!string.IsNullOrEmpty(personelIsmi))
            {
                // SQL bağlantı dizesi
               
                try
                {
                    using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                    {
                        conn.Open();

                        // SQL Insert komutu
                        string query = "INSERT INTO Bakim_Personel (Bakim_Personeli) VALUES (@personelIsmi)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            // Parametre ekle
                            cmd.Parameters.AddWithValue("@personelIsmi", personelIsmi);

                            // Sorguyu çalıştır
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Personel başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir isim giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            tarihEkle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PerEkle();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox6.Text) && !string.IsNullOrEmpty(textBox5.Text) && !string.IsNullOrEmpty(textBox4.Text) && !string.IsNullOrEmpty(textBox3.Text))
            {
                DateTime son3AyTarih;
                DateTime son1YilTarih;
                if (DateTime.TryParse(textBox4.Text, out son3AyTarih) && DateTime.TryParse(textBox5.Text, out son1YilTarih))
                {
                    
                    // YılId almak için sorgu
                    string yilQuery = "SELECT TOP 1 [YilId] FROM [Bakim].[dbo].[yillar] WHERE [Yil] = @Yil ORDER BY [Yil] DESC";

                    using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                    {
                        try
                        {
                            connection.Open();

                            // YılId sorgusu
                            SqlCommand yilCommand = new SqlCommand(yilQuery, connection);
                            yilCommand.Parameters.AddWithValue("@Yil", son1YilTarih.Year);

                            object yilIdObj = yilCommand.ExecuteScalar();
                            int yilId = yilIdObj != null ? (int)yilIdObj : 0;

                            // Şu anki yıl
                            int currentYear = DateTime.Now.Year;

                            // Girilen yılın geçerli olup olmadığını kontrol et
                            if (son1YilTarih.Year > currentYear)
                            {
                                MessageBox.Show("Geçersiz tarih. Lütfen içinde bulunduğunuz yıldan daha büyük bir yıl girin.");
                            }
                            else
                            {
                                // MakID'nin var olup olmadığını kontrol et
                                string checkMakIDQuery = "SELECT COUNT(*) FROM BakimTarih WHERE MakID = @MakID";

                                SqlCommand checkCommand = new SqlCommand(checkMakIDQuery, connection);
                                checkCommand.Parameters.AddWithValue("@MakID", textBox3.Text);

                                int count = (int)checkCommand.ExecuteScalar();

                                if (count > 0)
                                {
                                    MessageBox.Show("Bu MakID zaten mevcut. Lütfen farklı bir MakID girin.");
                                }
                                else
                                {
                                    // BakimTarih tablosuna yeni kayıt ekleme
                                    string insertQuery = "INSERT INTO BakimTarih (MakID, Son3AyTarih, Son1YilTarih, SorPersonel, Yil) VALUES (@MakID, @Son3AyTarih, @Son1YilTarih, @SorPersonel, @YilId)";

                                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                                    {
                                        command.Parameters.AddWithValue("@MakID", textBox3.Text);
                                        command.Parameters.AddWithValue("@Son3AyTarih", son3AyTarih);
                                        command.Parameters.AddWithValue("@Son1YilTarih", son1YilTarih);
                                        command.Parameters.AddWithValue("@SorPersonel", textBox6.Text);
                                        command.Parameters.AddWithValue("@YilId", yilId);

                                        int rowsAffected = command.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                        {
                                            MessageBox.Show("Yeni Makine Eklendi.");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Kayıt başarıyla eklenemedi.");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Tarih formatları geçerli değil.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
            }



        }

        private void PersonelEkle_Load(object sender, EventArgs e)
        {

        }
    }
}
