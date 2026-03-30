using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public class AdministrareComenziFisierText : IStocareComenzi
    {
        private string numeFisier;

        public AdministrareComenziFisierText(string numeFisier)
        {
            this.numeFisier = numeFisier;
            Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public void AdaugaComanda(Comanda c)
        {
            using (StreamWriter sw = new StreamWriter(numeFisier, true))
            {
                sw.WriteLine(c.ConversieLaSirPentruFisier());
            }
        }

        public List<Comanda> GetComenzi()
        {
            List<Comanda> comenzi = new List<Comanda>();
            using (StreamReader sr = new StreamReader(numeFisier))
            {
                string linie;
                while ((linie = sr.ReadLine()) != null)
                {
                    comenzi.Add(new Comanda(linie));
                }
            }
            return comenzi;
        }

        public Comanda GetComandaDupaId(int id)
        {
            return GetComenzi().FirstOrDefault(c => c.ID == id);
        }

        public bool UpdateComanda(Comanda comandaActualizata)
        {
            List<Comanda> comenzi = GetComenzi();
            bool actualizareCuSucces = false;

            using (StreamWriter sw = new StreamWriter(numeFisier, false))
            {
                foreach (Comanda c in comenzi)
                {
                    if (c.ID == comandaActualizata.ID)
                    {
                        sw.WriteLine(comandaActualizata.ConversieLaSirPentruFisier());
                        actualizareCuSucces = true;
                    }
                    else
                    {
                        sw.WriteLine(c.ConversieLaSirPentruFisier());
                    }
                }
            }
            return actualizareCuSucces;
        }
    }
}