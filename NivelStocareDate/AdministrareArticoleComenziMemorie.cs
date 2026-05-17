using System.Collections.Generic;
using System.Linq;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public class AdministrareArticoleComenziMemorie : IStocareArticoleComenzi
    {
        private List<ArticolComanda> _articole;

        public AdministrareArticoleComenziMemorie()
        {
            _articole = new List<ArticolComanda>();
        }

        public void AdaugaArticol(ArticolComanda a)
        {
            if (a.ID == 0)
            {
                a.ID = _articole.Count == 0 ? 1 : _articole.Max(x => x.ID) + 1;
            }
            _articole.Add(a);
        }

        public List<ArticolComanda> GetArticole()
        {
            return _articole;
        }

        public List<ArticolComanda> GetArticolePentruComanda(int idComanda)
        {
            return _articole.Where(a => a.IdComanda == idComanda).ToList();
        }

        public bool UpdateArticol(ArticolComanda articolActualizat)
        {
            ArticolComanda existent = _articole.FirstOrDefault(a => a.ID == articolActualizat.ID);
            if (existent != null)
            {
                existent.IdComanda = articolActualizat.IdComanda;
                existent.IdProdus = articolActualizat.IdProdus;
                existent.Cantitate = articolActualizat.Cantitate;
                existent.ProdusComandat = articolActualizat.ProdusComandat;
                return true;
            }
            return false;
        }

        public bool StergeArticol(int idArticol)
        {
            int initial = _articole.Count;
            _articole.RemoveAll(a => a.ID == idArticol);
            return _articole.Count < initial;
        }

        public bool StergeToatePentruComanda(int idComanda)
        {
            int initial = _articole.Count;
            _articole.RemoveAll(a => a.IdComanda == idComanda);
            return _articole.Count < initial;
        }
    }
}