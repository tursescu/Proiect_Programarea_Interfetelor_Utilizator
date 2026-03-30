using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public class AdministrareProduseFisierText : IStocareProduse
    {
        private string numeFisier;

        public AdministrareProduseFisierText(string numeFisier)
        {
            this.numeFisier = numeFisier;
            Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public void AdaugaProdus(Produs p)
        {
            using (StreamWriter sw = new StreamWriter(numeFisier, true))
            {
                sw.WriteLine(p.ConversieLaSirPentruFisier());
            }
        }

        public List<Produs> GetProduse()
        {
            List<Produs> produse = new List<Produs>();
            using (StreamReader sr = new StreamReader(numeFisier))
            {
                string linie;
                while ((linie = sr.ReadLine()) != null)
                {
                    produse.Add(new Produs(linie));
                }
            }
            return produse;
        }

        public Produs GetProdusDupaNume(string numeCautat)
        {
            return GetProduse().FirstOrDefault(p => p.Nume.ToLower().Contains(numeCautat.ToLower()));
        }

        public bool UpdateProdus(Produs produsActualizat)
        {
            List<Produs> produse = GetProduse();
            bool actualizareCuSucces = false;

            using (StreamWriter sw = new StreamWriter(numeFisier, false))
            {
                foreach (Produs p in produse)
                {
                    if (p.ID == produsActualizat.ID)
                    {
                        sw.WriteLine(produsActualizat.ConversieLaSirPentruFisier());
                        actualizareCuSucces = true;
                    }
                    else
                    {
                        sw.WriteLine(p.ConversieLaSirPentruFisier());
                    }
                }
            }
            return actualizareCuSucces;
        }
    }
}