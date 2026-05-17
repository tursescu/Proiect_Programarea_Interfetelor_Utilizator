using System.Collections.Generic;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public interface IStocareProduse
    {
        void AdaugaProdus(Produs p);
        List<Produs> GetProduse();
        Produs GetProdusDupaNume(string nume);
        List<Produs> GetProduseDupaNume(string nume);
        bool UpdateProdus(Produs p);
        bool ModificaProdus(Produs p);  
    }
}