using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Lab4_LZW.Models
{
    public class Archivo
    {
        public string Algoritmo { get; set; }
        public string NombreOriginal { get; set; }
        public string Nombre { get; set; }
        public string RutaArchivo { get; set; }
        public double RazonCompresion { get; set; }
        public double FactorCompresion { get; set; }
        public double Porcentaje { get; set; }

        public static List<Archivo> CargarHistorial()
        {
            var Linea = string.Empty;
            var listaArchivo = new List<Archivo>();
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
                    listaArchivo.Add(historialtemp);
                }
            }
            return listaArchivo;
        }

        //Carga el historial que se muestra en pantalla por medio de una lista
        public static void ManejarCompressions(Archivo Actual)
        {
            var Linea = string.Empty;
            var listaArchivo = CargarHistorial();
            using (var Writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "compressions.txt")))
            {
                foreach (var item in listaArchivo)
                {
                    Writer.WriteLine(item.Algoritmo);
                    Writer.WriteLine(item.NombreOriginal);
                    Writer.WriteLine(item.Nombre);
                    Writer.WriteLine(item.RutaArchivo);
                    Writer.WriteLine(item.RazonCompresion);
                    Writer.WriteLine(item.FactorCompresion);
                    Writer.WriteLine(item.Porcentaje);
                }
                Writer.WriteLine(Actual.Algoritmo);
                Writer.WriteLine(Actual.NombreOriginal);
                Writer.WriteLine(Actual.Nombre);
                Writer.WriteLine(Actual.RutaArchivo);
                Writer.WriteLine(Actual.RazonCompresion);
                Writer.WriteLine(Actual.FactorCompresion);
                Writer.WriteLine(Actual.Porcentaje);
            }
        }
    }
}
