using System.Collections.Generic;
using System.Linq;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public class AdministrareProduseMemorie
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
    }
}
