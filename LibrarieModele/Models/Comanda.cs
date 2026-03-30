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
        public decimal PretTotal
        {
            get
            {
                decimal suma = 0;
                foreach (ArticolComanda articol in Produse)
                {
                    if (articol.ProdusComandat != null)
                        suma = suma + articol.PretTotalArticol;
                }
                return suma;
            }
        }
        public DateTime DataLivrarii { get; set; }
        public StatusComanda StatusComanda { get; set; }
        public StatusPlata StatusPlata { get; set; }

        public Comanda(int id, string numeClient, string prenumeClient, string numarTelefon, DateTime dataLivrarii)
        {
            ID = id;
            NumeClient = numeClient;
            PrenumeClient = prenumeClient;
            NumarTelefon = numarTelefon;
            DataLivrarii = dataLivrarii;

            Produse = new List<ArticolComanda>();
            StatusComanda = StatusComanda.InAsteptare; //default
            StatusPlata = StatusPlata.Neplatita;       
        }

        public void AdaugaProdus(Produs produs, int cantitate)
        {
            Produse.Add(new ArticolComanda(produs, cantitate));
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

            Produse = new List<ArticolComanda>();
        }

        public string ConversieLaSirPentruFisier()
        {
            string stringProduse = "";
            if (Produse != null && Produse.Count > 0)
            {
                List<string> listaProduseString = new List<string>();
                foreach (var articol in Produse)
                {
                    if (articol.ProdusComandat != null)
                        listaProduseString.Add($"{articol.ProdusComandat.ID}:{articol.Cantitate}");
                }
                stringProduse = string.Join("|", listaProduseString);
            }

            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}",
                SEPARATOR_PRINCIPAL_FISIER,
                ID.ToString(),
                (NumeClient ?? "NECUNOSCUT"),
                (PrenumeClient ?? "NECUNOSCUT"),
                (NumarTelefon ?? "NECUNOSCUT"),
                DataLivrarii.ToString("yyyy-MM-dd"),
                (int)StatusComanda,
                (int)StatusPlata,
                stringProduse);
        }
    }
}