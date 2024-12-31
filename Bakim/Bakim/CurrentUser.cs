using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bakim
{
    public static class CurrentUser
    {
        // Kullanıcı bilgilerini tutmak için statik özellikler
        public static string Username { get; set; }
        public static string Ad { get; set; }
        public static int Id { get; set; }


    }


}
