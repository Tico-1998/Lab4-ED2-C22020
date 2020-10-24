using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4_LZW.Models
{
    public class Nodo
    {

        public byte caracter { get; set; }
        public double Frecuencia { get; set; }
        public Nodo nodoizq { get; set; }
        public Nodo nododer { get; set; }

        public int CompareTo(object obj)
        {
            return Frecuencia.CompareTo(((Nodo)obj).Frecuencia);
        }
    }
}
