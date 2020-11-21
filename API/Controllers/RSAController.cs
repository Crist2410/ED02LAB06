using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibreriaGenericos;
using System.IO;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RSAController : ControllerBase
    {
       
        static string RutaPublico = Path.GetFullPath("Archivos Zip\\public.key");
        static string RutaPrivada = Path.GetFullPath("Archivos Zip\\private.key");

        public static Llaves Llave = new Llaves( RutaPrivada, RutaPublico);
        public static CifradoRSA EncriptacionRSA = new CifradoRSA();
        // GET: api/Encriptar
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Jose Daniel Giron", "Cristian Josue Barrientos" };
        }
        private string RealizarZip()
        {
            string Contador = Path.GetFullPath("Archivos\\" + "Historial.txt");
            FileStream ArchivoHistotial = new FileStream(Contador, FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(ArchivoHistotial);
            string Texto = reader.ReadToEnd();
            int Numero = 1;
            if (Texto != "")
            Numero = Convert.ToInt32(Texto);
            Texto = "Archivos\\Keys" + Numero + ".zip";
            reader.Close();
            StreamWriter writer = new StreamWriter(Contador, false);
            writer.WriteLine(++Numero);
            writer.Close();
            return Texto;
        }
        // GET: api/Encriptar/5
        [HttpGet("keys/{p}/{q}")]
        public IActionResult Llaves([FromRoute]string p, [FromRoute]string q)
        {
            try
            {
                string RutaZip = RealizarZip();
                Llave.GeneracionLlaves(Convert.ToInt32(p), Convert.ToInt32(q), RutaZip); ;
                FileStream ArchivoFinal = new FileStream(RutaZip, FileMode.Open);
                FileStreamResult FileFinal = new FileStreamResult(ArchivoFinal, "application/zip");
                return FileFinal;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Encriptar
        [HttpPost("{nombre}")]
        public IActionResult Cifrado([FromRoute]string nombre, [FromForm]  IFormFile file, [FromForm]  IFormFile key)
        {
            try
            {
                if (key.FileName.Contains("public"))
                {
                    string RutaOriginal = Path.GetFullPath("Cifrado\\" + file.FileName);
                    string RutaLlaves = Path.GetFullPath("Llaves\\" + key.FileName);
                    FileStream ArchivoOriginal= new FileStream(RutaOriginal, FileMode.Create);
                    FileStream ArchivoLlaves= new FileStream(RutaLlaves, FileMode.Create);
                    file.CopyTo(ArchivoOriginal);
                    key.CopyTo(ArchivoLlaves);
                    ArchivoOriginal.Close();
                    ArchivoLlaves.Close();
                    string NombreOriginal = file.FileName;
                    string Extencion = file.FileName.Split('.')[1];
                    string RutaDestino= Path.GetFullPath("Decifrado\\" + nombre +"."+ Extencion);
                    EncriptacionRSA.Cifrado(RutaOriginal, RutaDestino, RutaLlaves);
                    FileStream ArchivoFinal = new FileStream(RutaDestino, FileMode.Open);
                    FileStreamResult FileFinal = new FileStreamResult(ArchivoFinal, "text/"+ Extencion);
                    return FileFinal;
                }
                else if(key.FileName.Contains("private"))
                {
                    string RutaOriginal = Path.GetFullPath("Decifrado\\" + file.FileName);
                    string RutaLlaves = Path.GetFullPath("Llaves\\" + key.FileName);
                    FileStream ArchivoOriginal = new FileStream(RutaOriginal, FileMode.Create);
                    FileStream ArchivoLlaves = new FileStream(RutaLlaves, FileMode.Create);
                    file.CopyTo(ArchivoOriginal);
                    key.CopyTo(ArchivoLlaves);
                    ArchivoOriginal.Close();
                    ArchivoLlaves.Close();
                    string NombreOriginal = file.FileName;
                    string Extencion = file.FileName.Split('.')[1];
                    string RutaDestino = Path.GetFullPath("Cifrado\\" + nombre + "." + Extencion);
                    EncriptacionRSA.Descifrado(RutaOriginal, RutaDestino, RutaLlaves);
                    FileStream ArchivoFinal = new FileStream(RutaDestino, FileMode.Open);
                    FileStreamResult FileFinal = new FileStreamResult(ArchivoFinal, "text/" + Extencion);
                    return FileFinal;
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
