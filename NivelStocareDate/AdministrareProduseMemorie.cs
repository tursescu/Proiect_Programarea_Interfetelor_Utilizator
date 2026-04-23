using System.Collections.Generic;
using System.Linq;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public class AdministrareProduseMemorie : IStocareProduse
    {
        private List<Produs> _produse;

        public AdministrareProduseMemorie()
        {
            _produse = new List<Produs>();
        }

        public void AdaugaProdus(Produs produsNou)
        {
            _produse.Add(produsNou);
        }

        public List<Produs> GetProduse()
        {
            return _produse;
        }

        public Produs GetProdusDupaNume(string numeCautat)
        {
            return _produse.FirstOrDefault(p => p.Nume.ToLower().Contains(numeCautat.ToLower()));
        }

        public List<Produs> GetProduseDupaNume(string numeCautat)
        {
            return _produse
                .Where(p => p.Nume.ToLower().Contains(numeCautat.ToLower()))
                .ToList();
        }
        public bool UpdateProdus(Produs pActualizat)
        {
            var produsVechi = _produse.FirstOrDefault(p => p.ID == pActualizat.ID);
            if (produsVechi != null)
            {
                produsVechi.Nume = pActualizat.Nume;
                produsVechi.PretUnitar = pActualizat.PretUnitar;
                produsVechi.Detalii = pActualizat.Detalii;
                produsVechi.Caracteristici = pActualizat.Caracteristici;
                return true;
            }
            return false;
        }
    }
}
