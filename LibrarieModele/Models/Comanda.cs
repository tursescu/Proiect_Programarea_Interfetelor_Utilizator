using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LibrarieModele.Enums;

namespace LibrarieModele.Models
{
    public class Comanda
    {
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';

        public int ID { get; set; }
        public string NumeClient { get; set; }
        public string PrenumeClient { get; set; }
        public string NumarTelefon { get; set; }
        public List<ArticolComanda> Produse { get; set; }
        public DateTime DataLivrarii { get; set; }
        public StatusComanda StatusComanda { get; set; }
        public StatusPlata StatusPlata { get; set; }
        public ModPlata ModPlata { get; set; }

        public decimal PretTotal
        {
            get
            {
                decimal suma = 0;
                if (Produse != null)
                {
                    foreach (ArticolComanda articol in Produse)
                    {
                        if (articol.ProdusComandat != null)
                            suma += articol.PretTotalArticol;
                    }
                }
                return suma;
            }
        }

        public Comanda(int id, string numeClient, string prenumeClient, string numarTelefon, DateTime dataLivrarii)
        {
            ID = id;
            NumeClient = numeClient;
            PrenumeClient = prenumeClient;
            NumarTelefon = numarTelefon;
            DataLivrarii = dataLivrarii;
            Produse = new List<ArticolComanda>();
            StatusComanda = StatusComanda.InAsteptare;
            StatusPlata = StatusPlata.Neplatita;
            ModPlata = ModPlata.Numerar;
        }

        public void AdaugaProdus(Produs produs, int cantitate)
        {
            ArticolComanda articol = new ArticolComanda(0, ID, produs, cantitate);
            Produse.Add(articol);
        }

        public Comanda(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER);

            ID = Convert.ToInt32(dateFisier[0]);
            NumeClient = dateFisier[1];
            PrenumeClient = dateFisier[2];
            NumarTelefon = dateFisier[3];
            DataLivrarii = Convert.ToDateTime(dateFisier[4]);
            StatusComanda = (StatusComanda)Convert.ToInt32(dateFisier[5]);
            StatusPlata = (StatusPlata)Convert.ToInt32(dateFisier[6]);

            if (dateFisier.Length > 7 && Enum.TryParse(dateFisier[7], out ModPlata mp))
            {
                ModPlata = mp;
            }
            else
            {
                ModPlata = ModPlata.Numerar;
            }

            Produse = new List<ArticolComanda>();
        }

        public string ConversieLaSirPentruFisier()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}",
                SEPARATOR_PRINCIPAL_FISIER,
                ID,
                (NumeClient ?? "NECUNOSCUT"),
                (PrenumeClient ?? "NECUNOSCUT"),
                (NumarTelefon ?? "NECUNOSCUT"),
                DataLivrarii.ToString("yyyy-MM-dd"),
                (int)StatusComanda,
                (int)StatusPlata,
                ModPlata.ToString());
        }
    }
}