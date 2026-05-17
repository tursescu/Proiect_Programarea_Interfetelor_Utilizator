using System;
using LibrarieModele.Models;

namespace LibrarieModele.Models
{
    public class ArticolComanda
    {
        private const char SEPARATOR_FISIER = ';';

        public int ID { get; set; }
        public int IdComanda { get; set; }
        public int IdProdus { get; set; }
        public int Cantitate { get; set; }

        public Produs ProdusComandat { get; set; }

        public decimal PretTotalArticol
        {
            get
            {
                if (ProdusComandat == null) return 0;
                return ProdusComandat.PretUnitar * Cantitate;
            }
        }

        public ArticolComanda(int id, int idComanda, int idProdus, int cantitate)
        {
            ID = id;
            IdComanda = idComanda;
            IdProdus = idProdus;
            Cantitate = cantitate;
        }

        public ArticolComanda(int id, int idComanda, Produs produs, int cantitate)
        {
            ID = id;
            IdComanda = idComanda;
            IdProdus = produs.ID;
            Cantitate = cantitate;
            ProdusComandat = produs;
        }

        public ArticolComanda(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_FISIER);
            ID = Convert.ToInt32(dateFisier[0]);
            IdComanda = Convert.ToInt32(dateFisier[1]);
            IdProdus = Convert.ToInt32(dateFisier[2]);
            Cantitate = Convert.ToInt32(dateFisier[3]);
        }

        public string ConversieLaSirPentruFisier()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}",
                SEPARATOR_FISIER,
                ID,
                IdComanda,
                IdProdus,
                Cantitate);
        }
    }
}