using System;
using System.Data.SqlClient;

namespace Bakim
{
    public class KategoriService
    {
        private readonly string connectionString;

        public KategoriService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void UpdateKategoriForMakId(string makId,string bakimTanim)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string kategoriSayQuery = "";
                    string kategori = makId.Split('-')[0] + "-";
                    if(bakimTanim=="3 AYLIK")
                    {
                        kategoriSayQuery = @"
                        SELECT COUNT(*) AS KategoriSayisi 
                        FROM BakimMetinleri 
                        WHERE Kategori = @kategori and periyod='A'";
                    }
                    if (bakimTanim == "YILLIK")
                    {
                        // Kategori için kriter sayısını al
                        kategoriSayQuery = @"
                        SELECT COUNT(*) AS KategoriSayisi 
                        FROM BakimMetinleri 
                        WHERE Kategori = @kategori 
                        AND (periyod = 'A' OR periyod = 'B')";
                    }
                   

                    int kategoriSayisi;
                    using (SqlCommand cmd = new SqlCommand(kategoriSayQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@kategori", kategori);
                        kategoriSayisi = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (kategoriSayisi > 0)
                    {
                        // Tek bir UPDATE sorgusu ile tüm kriterleri güncelle
                        var updateColumns = new System.Text.StringBuilder();
                        for (int i = 1; i <= kategoriSayisi; i++)
                        {
                            if (i > 1) updateColumns.Append(",");
                            updateColumns.Append($"kriter{i} = 'EVET'");
                        }

                        string updateQuery = $@"
                    UPDATE GecmisBakim 
                    SET {updateColumns}
                    WHERE MakId = @makId AND BakimTanim = @bakimTanim";

                        using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@makId", makId);
                            cmdUpdate.Parameters.AddWithValue("@bakimTanim", bakimTanim);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Kategori güncellemesi sırasında hata: {ex.Message}", ex);
            }
        }
    }
}