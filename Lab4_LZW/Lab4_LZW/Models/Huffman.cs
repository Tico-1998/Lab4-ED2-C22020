using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Lab4_LZW.Models
{
    public class Huffman
    {
        private static string routeDirectory = Environment.CurrentDirectory;

        //Lee el documento y crea la tabla de prefijos
        public static void Comprimir(IFormFile archivo, string nombre)
        {
            if (!Directory.Exists(Path.Combine(routeDirectory, "compress")))
            {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "compress"));
            }

            var TablaLetras = new Dictionary<byte, double>();

            var ListaNodosArbol = new List<Nodo>();


            using (var reader = new BinaryReader(archivo.OpenReadStream()))
            {
                //Cantidad de letras en buffer
                const int bufferLength = 10000;

                var byteBuffer = new byte[bufferLength];
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    byteBuffer = reader.ReadBytes(bufferLength);

                    foreach (var letra in byteBuffer)
                    {
                        if (TablaLetras.ContainsKey(letra))
                        {
                            TablaLetras[letra]++;
                        }
                        else
                        {
                            TablaLetras.Add(letra, 1);
                        }
                    }
                }

                double totalLetras = 0;

                foreach (var letra in TablaLetras)
                {
                    totalLetras += letra.Value;
                }

                foreach (var letra in TablaLetras)
                {
                    ListaNodosArbol.Add(new Nodo { caracter = letra.Key, Frecuencia = letra.Value / totalLetras });
                }

                ListaNodosArbol.Sort();
            }

            Insertar(ListaNodosArbol, archivo, nombre);
        }

        //Crea el arbol huffman para luego empezar la compresion
        public static void Insertar(List<Nodo> ListaNodo, IFormFile archivo, string nombre)
        {
            while (ListaNodo.Count != 1)
            {
                var nodoAux = new Nodo();

                nodoAux.Frecuencia = ListaNodo[0].Frecuencia + ListaNodo[1].Frecuencia;

                nodoAux.nodoizq = ListaNodo[1];
                nodoAux.nododer = ListaNodo[0];

                ListaNodo.RemoveRange(0, 2);
                ListaNodo.Add(nodoAux);
                ListaNodo.Sort();
            }

            var DiccionarioPrefijos = new Dictionary<byte, string>();
            var camino = "";

            Recorrido(ref DiccionarioPrefijos, ListaNodo[0], camino);

            ComprimirArchivo(DiccionarioPrefijos, archivo, nombre);
        }

        ///Realiza la logica para leer el archivo original y comprimir
        public static void ComprimirArchivo(Dictionary<byte, string> DiccionarioClave, IFormFile archivo, string nombre)
        {

            using (var reader = new BinaryReader(archivo.OpenReadStream()))
            {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "compress", $"{nombre}.huff"), FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(streamWriter))
                    {
                        writer.Write(Encoding.UTF8.GetBytes(Convert.ToString(DiccionarioClave.Count).PadLeft(8, '0').ToCharArray()));

                        foreach (var item in DiccionarioClave)
                        {
                            writer.Write(item.Key);

                            var aux = $"{item.Value}|";

                            writer.Write(aux.ToCharArray());
                        }

                        //Traduce las letras del doc original al codigo ASCII
                        const int bufferLength = 10000;

                        var byteBuffer = new byte[bufferLength];
                        var CadenaAux = "";

                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBuffer = reader.ReadBytes(bufferLength);

                            foreach (var LetraRecibida in byteBuffer)
                            {
                                foreach (var Clave in DiccionarioClave)
                                {
                                    if (LetraRecibida == Clave.Key)
                                    {
                                        CadenaAux += Clave.Value;
                                        if (CadenaAux.Length / 8 != 0)
                                        {
                                            for (int i = 0; i < CadenaAux.Length / 8; i++)
                                            {
                                                var NuevaCadena = CadenaAux.Substring(0, 8);
                                                writer.Write((byte)Convert.ToInt32(NuevaCadena, 2));
                                                CadenaAux = CadenaAux.Substring(8);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (CadenaAux.Length <= 8)
                        {
                            writer.Write((byte)Convert.ToInt32(CadenaAux.PadRight(8, '0'), 2));
                        }

                    }
                }
            }

        }

        ////Recorre el arbol para obtener el camino
        public static void Recorrido(ref Dictionary<byte, string> DiccionarioPre, Nodo Raiz, string camino)
        {
            if (Raiz != null)
            {
                var caminoDer = $"{camino}1";
                Recorrido(ref DiccionarioPre, Raiz.nododer, caminoDer);
                if (Raiz.caracter != 0)
                {
                    DiccionarioPre.Add(Raiz.caracter, camino);
                }
                var caminoIzq = $"{camino}0";
                Recorrido(ref DiccionarioPre, Raiz.nodoizq, caminoIzq);
            }

        }

        public static string Descomprimir(IFormFile archivo, string nombre)
        {
            var TablaPrefijos = new Dictionary<string, byte>();
            var Extension = string.Empty;

            if (!Directory.Exists(Path.Combine(routeDirectory, "decompress")))
            {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "decompress"));
            }


            using (var reader = new BinaryReader(archivo.OpenReadStream()))
            {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "decompress", nombre), FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(streamWriter))
                    {
                        int bufferLength = 10000;
                        var byteBuffer = new byte[bufferLength];
                        byteBuffer = reader.ReadBytes(8);
                        var cantDiccionario = Convert.ToInt32(Encoding.UTF8.GetString(byteBuffer));

                        bufferLength = 1;

                        byteBuffer = reader.ReadBytes(bufferLength);

                        for (int i = 0; i < cantDiccionario; i++)
                        {
                            var camino = new List<byte>();

                            var letra = byteBuffer[0];

                            byteBuffer = reader.ReadBytes(bufferLength);

                            var DentroCamino = true;

                            while (DentroCamino)
                            {
                                if (byteBuffer[0] != 124)
                                {
                                    camino.Add(byteBuffer[0]);
                                }
                                else
                                {
                                    DentroCamino = false;
                                }
                                byteBuffer = reader.ReadBytes(bufferLength);
                            }

                            TablaPrefijos.Add(Encoding.UTF8.GetString(camino.ToArray()), letra);
                        }


                        bufferLength = 1000;
                        var AuxCadena = string.Empty;
                        var Linea = string.Empty;

                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            foreach (var item in byteBuffer)
                            {
                                Linea += ObtenerBinario(Convert.ToString(item)).PadLeft(8, '0');
                                while (Linea.Length > 0)
                                {
                                    if (TablaPrefijos.ContainsKey(AuxCadena))
                                    {
                                        writer.Write(TablaPrefijos[AuxCadena]);
                                        AuxCadena = string.Empty;
                                    }
                                    else
                                    {
                                        AuxCadena += Linea.Substring(0, 1);
                                        Linea = Linea.Substring(1);
                                    }
                                }
                            }
                            byteBuffer = reader.ReadBytes(1000);
                        }

                        if (AuxCadena.Length != 0)
                        {
                            foreach (var item in byteBuffer)
                            {
                                Linea += ObtenerBinario(Convert.ToString(item)).PadLeft(8, '0');
                                while (Linea.Length > 0)
                                {
                                    if (TablaPrefijos.ContainsKey(AuxCadena))
                                    {
                                        writer.Write(TablaPrefijos[AuxCadena]);
                                        AuxCadena = string.Empty;
                                    }
                                    else
                                    {
                                        AuxCadena += Linea.Substring(0, 1);
                                        Linea = Linea.Substring(1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Path.Combine(routeDirectory, "decompress", nombre);
        }

        static string ObtenerBinario(string Snumero)
        {
            var numero = Convert.ToInt32(Snumero);
            var Aux = "";
            var binario = "";

            while ((numero >= 2))
            {
                Aux = Aux + (numero % 2).ToString();
                numero = numero / 2;
            }
            Aux = Aux + numero.ToString();

            for (int i = Aux.Length; i >= 1; i += -1)
            {
                binario = binario + Aux.Substring(i - 1, 1);
            }

            return binario;
        }
    }
}
