using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public class AdministrareArticoleComenziFisierText : IStocareArticoleComenzi
    {
        private string numeFisier;

        public AdministrareArticoleComenziFisierText(string numeFisier)
        {
            this.numeFisier = numeFisier;
            Stream s = File.Open(numeFisier, FileMode.OpenOrCreate);
            s.Close();
        }

        public void AdaugaArticol(ArticolComanda a)
        {
            if (a.ID == 0)
            {
                a.ID = GetUrmatorulId();
            }

            using (StreamWriter sw = new StreamWriter(numeFisier, true))
            {
                sw.WriteLine(a.ConversieLaSirPentruFisier());
            }
        }

        public List<ArticolComanda> GetArticole()
        {
            List<ArticolComanda> articole = new List<ArticolComanda>();
            using (StreamReader sr = new StreamReader(numeFisier))
            {
                string linie;
                while ((linie = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(linie))
                    {
                        articole.Add(new ArticolComanda(linie));
                    }
                }
            }
            return articole;
        }

        public List<ArticolComanda> GetArticolePentruComanda(int idComanda)
        {
            List<ArticolComanda> rezultat = new List<ArticolComanda>();
            foreach (ArticolComanda a in GetArticole())
            {
                if (a.IdComanda == idComanda)
                {
                    rezultat.Add(a);
                }
            }
            return rezultat;
        }

        public bool UpdateArticol(ArticolComanda articolActualizat)
        {
            List<ArticolComanda> articole = GetArticole();
            bool succes = false;

            using (StreamWriter sw = new StreamWriter(numeFisier, false))
            {
                foreach (ArticolComanda a in articole)
                {
                    if (a.ID == articolActualizat.ID)
                    {
                        sw.WriteLine(articolActualizat.ConversieLaSirPentruFisier());
                        succes = true;
                    }
                    else
                    {
                        sw.WriteLine(a.ConversieLaSirPentruFisier());
                    }
                }
            }
            return succes;
        }

        public bool StergeArticol(int idArticol)
        {
            List<ArticolComanda> articole = GetArticole();
            bool succes = false;

            using (StreamWriter sw = new StreamWriter(numeFisier, false))
            {
                foreach (ArticolComanda a in articole)
                {
                    if (a.ID == idArticol)
                    {
                        succes = true;
                        continue; 
                    }
                    sw.WriteLine(a.ConversieLaSirPentruFisier());
                }
            }
            return succes;
        }

        public bool StergeToatePentruComanda(int idComanda)
        {
            List<ArticolComanda> articole = GetArticole();
            bool succes = false;

            using (StreamWriter sw = new StreamWriter(numeFisier, false))
            {
                foreach (ArticolComanda a in articole)
                {
                    if (a.IdComanda == idComanda)
                    {
                        succes = true;
                        continue;
                    }
                    sw.WriteLine(a.ConversieLaSirPentruFisier());
                }
            }
            return succes;
        }

        private int GetUrmatorulId()
        {
            List<ArticolComanda> articole = GetArticole();
            if (articole.Count == 0) return 1;
            return articole.Max(a => a.ID) + 1;
        }
    }
}