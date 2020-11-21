using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Compression;

namespace LibreriaGenericos
{
  public  class Llaves
    {
        static string RutaPublico;
        static string RutaPrivada;
        public Llaves(string Privada,string Publica)
        {
            RutaPublico = Privada;
            RutaPrivada = Publica;
        }
        public void GeneracionLlaves(int p, int q, string RutaZip)
        {
            //encontrar n
            int n = p * q;

            //encontrar fi
            int fi = (p - 1) * (q - 1);

            //encontrar e
            int e = 0, Contador = 1;
            bool Estado = true;
            while (Estado && Contador < fi)
            {
                ++Contador;
                if (EncontrarPrimo(Contador) && fi % Contador != 0)
                {
                    e = Contador;
                    Estado = false;
                }
            }

            //encontrar d
            int[] Arreglo1 = { fi, e };
            int[] Arreglo2 = { fi, 1 };
            while (Arreglo1[1] != 1)
            {
                int Division1 = Arreglo1[0] / Arreglo1[1];
                int Aux1 = Arreglo1[0] - (Division1 * Arreglo1[1]);
                int Aux2 = Arreglo2[0] - (Division1 * Arreglo2[1]);
                if (Aux2 < 0)
                {
                    Aux2 = (Aux2 % fi + fi) % fi;
                }
                Arreglo1[0] = Arreglo1[1];
                Arreglo1[1] = Aux1;
                Arreglo2[0] = Arreglo2[1];
                Arreglo2[1] = Aux2;
            }
            int d = Arreglo2[1];

            //impresion en text files
           
            string LlavePublica = Convert.ToString(e) + "," + Convert.ToString(n);
            string LlavePrivada = Convert.ToString(d) + "," + Convert.ToString(n);

            File.WriteAllText(RutaPublico, LlavePublica);
            File.WriteAllText(RutaPrivada, LlavePrivada);

            //subir a zip las llaves
            using (var archive = ZipFile.Open(RutaZip, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(RutaPublico, Path.GetFileName(RutaPublico));
                archive.CreateEntryFromFile(RutaPrivada, Path.GetFileName(RutaPrivada));
            }
        }

        public bool EncontrarPrimo(int n)
        {
            for (int i = 2; i < n; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
