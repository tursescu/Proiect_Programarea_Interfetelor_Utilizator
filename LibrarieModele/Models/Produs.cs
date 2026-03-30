using System;
using System.Collections.Generic;
using System.Text;
using LibrarieModele.Enums;

namespace LibrarieModele.Models
{
    public class Produs
    {
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';

        private const int INDEX_ID = 0;
        private const int INDEX_NUME = 1;
        private const int INDEX_PRET = 2;
        private const int INDEX_DETALII = 3;
        private const int INDEX_CARACTERISTICI = 4;

        public int ID { get; set; }
        public string Nume { get; set; }
        public decimal PretUnitar { get; set; }
        public string Detalii { get; set; }
        public CaracteristiciProdus Caracteristici { get; set; }

        public Produs(int id, string nume, string detalii, decimal pretUnitar)
        {
            this.ID = id;
            this.Nume = nume;
            this.Detalii = detalii;
            this.PretUnitar = pretUnitar;
            this.Caracteristici = CaracteristiciProdus.Niciuna;
        }

        public Produs(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER);

            this.ID = Convert.ToInt32(dateFisier[INDEX_ID]);
            this.Nume = dateFisier[INDEX_NUME];
            this.PretUnitar = Convert.ToDecimal(dateFisier[INDEX_PRET]);
            this.Detalii = dateFisier[INDEX_DETALII];
            this.Caracteristici = (CaracteristiciProdus)Convert.ToInt32(dateFisier[INDEX_CARACTERISTICI]);
        }

        public string ConversieLaSirPentruFisier()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}",
                SEPARATOR_PRINCIPAL_FISIER,
                ID.ToString(),
                (Nume ?? "NECUNOSCUT"),
                PretUnitar.ToString(),
                (Detalii ?? "Fara detalii"),
                (int)Caracteristici);
        }
    }
}
