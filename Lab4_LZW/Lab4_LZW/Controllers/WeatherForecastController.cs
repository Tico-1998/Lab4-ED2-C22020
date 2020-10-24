using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab4_LZW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Lab4_LZW.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpPost, Route("compress/{name}/LZW")]
        public void CompresionLZW(IFormFile archivo, string nombre)
        {
            ClassLZW.Comprimir(archivo, nombre);

            var newFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, "compress", $"{nombre}.lzw"));

            Archivo.ManejarCompressions(
                new Archivo
                {
                    Algoritmo = "LZW",
                    NombreOriginal = archivo.FileName,
                    Nombre = $"{nombre}.lzw",
                    RutaArchivo = Path.Combine(Environment.CurrentDirectory, "compress", $"{nombre}.lzw"),
                    RazonCompresion = (double)newFile.Length / (double)archivo.Length,
                    FactorCompresion = (double)archivo.Length / (double)newFile.Length,
                    Porcentaje = 100 - (((double)newFile.Length / (double)archivo.Length) * 100)
                });
        }

        [HttpPost, Route("decompress/LZW")]
        public void DesompresionLZW(IFormFile archivo)
        {
            var Archivos = Archivo.CargarHistorial();

            var Original = Archivos.Find(c => Path.GetFileNameWithoutExtension(c.Nombre) == Path.GetFileNameWithoutExtension(archivo.FileName));

            var path = ClassLZW.Descomprimir(archivo, Original.NombreOriginal);

            var newFile = new FileInfo(path);

            Archivo.ManejarCompressions(
                new Archivo
                {
                    Algoritmo = "LZW",
                    NombreOriginal = Original.NombreOriginal,
                    Nombre = archivo.FileName,
                    RutaArchivo = path,
                    RazonCompresion = 0,
                    FactorCompresion = 0,
                    Porcentaje = 0
                });
        }

        [HttpPost, Route("compress/{nombre}/Huffman")]
        public void Compresion(IFormFile archivo, string nombre)
        {
            Huffman.Comprimir(archivo, nombre);

            var newFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, "compress", $"{nombre}.huff"));

            Archivo.ManejarCompressions(
                new Archivo
                {
                    Algoritmo = "Huffman",
                    NombreOriginal = archivo.FileName,
                    Nombre = $"{nombre}.huff",
                    RutaArchivo = Path.Combine(Environment.CurrentDirectory, "compress", $"{nombre}.huff"),
                    RazonCompresion = (double)newFile.Length / (double)archivo.Length,
                    FactorCompresion = (double)archivo.Length / (double)newFile.Length,
                    Porcentaje = 100 - (((double)newFile.Length / (double)archivo.Length) * 100)
                });
        }

        [HttpPost, Route("decompress/Huffman")]
        public void Desompresion(IFormFile archivo)
        {
            var Archivos = Archivo.CargarHistorial();

            var Original = Archivos.Find(c => Path.GetFileNameWithoutExtension(c.Nombre) == Path.GetFileNameWithoutExtension(archivo.FileName));

            var path = Huffman.Descomprimir(archivo, Original.NombreOriginal);

            var newFile = new FileInfo(path);

            Archivo.ManejarCompressions(
                new Archivo
                {
                    Algoritmo = "Huffman",
                    NombreOriginal = Original.NombreOriginal,
                    Nombre = archivo.FileName,
                    RutaArchivo = path,
                    RazonCompresion = 0,
                    FactorCompresion = 0,
                    Porcentaje = 0
                });
        }

        [HttpGet, Route("compressions")]
        public List<Archivo> Get()
        {
            var compresiones = new List<Archivo>();
            var logicaLIFO = new Stack<Archivo>();
            var Linea = string.Empty;

            using (var Reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, "compressions.txt")))
            {
                while (!Reader.EndOfStream)
                {
                    var historialtemp = new Archivo();
                    Linea = Reader.ReadLine();
                    historialtemp.Algoritmo = Linea;
                    Linea = Reader.ReadLine();
                    historialtemp.NombreOriginal = Linea;
                    Linea = Reader.ReadLine();
                    historialtemp.Nombre = Linea;
                    Linea = Reader.ReadLine();
                    historialtemp.RutaArchivo = Linea;
                    Linea = Reader.ReadLine();
                    historialtemp.RazonCompresion = Convert.ToDouble(Linea);
                    Linea = Reader.ReadLine();
                    historialtemp.FactorCompresion = Convert.ToDouble(Linea);
                    Linea = Reader.ReadLine();
                    historialtemp.Porcentaje = Convert.ToDouble(Linea);
                    logicaLIFO.Push(historialtemp);
                }
            }

            while (logicaLIFO.Count != 0)
            {
                compresiones.Add(logicaLIFO.Pop());
            }

            return compresiones;
        }
    }
}
