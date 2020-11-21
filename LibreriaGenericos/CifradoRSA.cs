using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Numerics;

namespace LibreriaGenericos
{
   public class CifradoRSA
    {
        public void Cifrado(string Original, string Destino, string Llaves)
        {
            //establecer rutas de archivos y buffer
            string RutaDestino = Destino;
            FileStream LecturaOriginal = new FileStream(Original, FileMode.OpenOrCreate);
            using var Reader = new BinaryReader(LecturaOriginal);
            var buffer = new byte[2000000];

            //creacion Writer y lista de bytes
            using var file = new FileStream(RutaDestino, FileMode.OpenOrCreate);
            using var Writer = new BinaryWriter(file, Encoding.Default, true);
            List<byte> ListaBytes = new List<byte>();

            //recoleccion llaves
            string Lineas = System.IO.File.ReadAllText(Llaves);
            string[] ArrayLlaves = Lineas.Split(',');
            int e = Convert.ToInt32(ArrayLlaves[0]);
            int n = Convert.ToInt32(ArrayLlaves[1]);

            //impresion de mensaje cifrado
            while (Reader.BaseStream.Position != Reader.BaseStream.Length)
            {
                buffer = Reader.ReadBytes(2000);
                foreach (var item in buffer)
                {
                    BigInteger value = BigInteger.ModPow(item, (BigInteger)e, (BigInteger)n);
                    byte[] ArregloBytes = BitConverter.GetBytes((long)value);
                    foreach (var Cifra in ArregloBytes)
                    {
                        ListaBytes.Add(Cifra);
                    }
                }
                Writer.Write(ListaBytes.ToArray());
                ListaBytes.Clear();
            }
            Writer.Close();
            Reader.Close();
        }
        public void Descifrado(string Original, string Destino, string Llaves)
        {
            //establecer rutas de archivos y buffer
            string RutaDestino = Destino;
            FileStream LecturaOriginal = new FileStream(Original, FileMode.OpenOrCreate);
            using var Reader = new BinaryReader(LecturaOriginal);
            var buffer = new byte[2000000];

            //creacion Writer y lista de bytes
            using var Writer = new FileStream(RutaDestino, FileMode.OpenOrCreate);
            List<byte> ListaBytes = new List<byte>();

            //recoleccion llaves
            string Lineas = System.IO.File.ReadAllText(Llaves);
            string[] ArrayLlaves = Lineas.Split(',');
            int d = Convert.ToInt32(ArrayLlaves[0]);
            int n = Convert.ToInt32(ArrayLlaves[1]);
            List<byte> ArregloBytes = new List<byte>();

            //impresion de mensaje cifrado
            while (Reader.BaseStream.Position != Reader.BaseStream.Length)
            {
                buffer = Reader.ReadBytes(10000);
                foreach (var item in buffer)
                {
                    ArregloBytes.Add(item);
                    if (ArregloBytes.ToArray().Length == 8)
                    {
                        int bits = BitConverter.ToInt32(ArregloBytes.ToArray());
                        BigInteger modPow = BigInteger.ModPow(bits, (BigInteger)d, (BigInteger)n);
                        byte Byte = Convert.ToByte((long)modPow);
                        ListaBytes.Add(Byte);
                        ArregloBytes.Clear();
                    }
                }
                Writer.Write(ListaBytes.ToArray(), 0, ListaBytes.ToArray().Length);
                ListaBytes.Clear();
            }
            Writer.Close();
            Reader.Close();
        }
    }
}
