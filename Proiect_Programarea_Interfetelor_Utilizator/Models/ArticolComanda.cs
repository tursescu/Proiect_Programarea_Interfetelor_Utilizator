using System;
using System.Collections.Generic;
using System.Text;

namespace Proiect_Programarea_Interfetelor_Utilizator.Models
{
    public class ArticolComanda
    {
        public Produs ProdusComandat { get; set; }
        public int Cantitate { get; set; }
        public decimal PretTotalArticol
        {
            get
            {
                return ProdusComandat.PretUnitar * Cantitate;
            }
        }

        public ArticolComanda(Produs produsComandat, int cantitate)
        {
            ProdusComandat = produsComandat;
            Cantitate = cantitate;
        }
    }
}
