using Proiect_Programarea_Interfetelor_Utilizator.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Proiect_Programarea_Interfetelor_Utilizator.Models
{
    public class Comanda
    {
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
    }
}
