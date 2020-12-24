using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Activdad2U4.Helpers
{
    public class HashingHelpers
    {
        public static string GetHash(string cadena)
        {
            var alg = SHA256.Create();
            byte[] codificar = Encoding.UTF8.GetBytes(cadena);
            byte[] hash = alg.ComputeHash(codificar);
            string x = "";
            foreach (var objeto in hash)
            {
                x += objeto.ToString("x2");
            }
            return x;
        }
    }
}
