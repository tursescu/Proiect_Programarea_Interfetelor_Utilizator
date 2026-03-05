using System;
using System.Collections.Generic;
using System.Text;

namespace Proiect_Programarea_Interfetelor_Utilizator.Models
{
    public class Produs
    {
        public int ID { get; set; }
        public string Nume { get; set; }
        public decimal PretUnitar { get; set; }
        public string Detalii {  get; set; }
        
        public Produs(int id, string nume, string detalii, decimal pretUnitar)
        {
            ID = id;
            Nume = nume;
            Detalii = detalii;
            PretUnitar = pretUnitar;
        }
    }
}
