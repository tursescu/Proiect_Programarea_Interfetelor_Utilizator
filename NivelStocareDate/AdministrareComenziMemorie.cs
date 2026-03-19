using System.Collections.Generic;
using System.Linq;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public class AdministrareComenziMemorie
    {
        private List<Comanda> _comenzi;

        public AdministrareComenziMemorie()
        {
            _comenzi = new List<Comanda>();
        }

        public void AdaugaComanda(Comanda comandaNoua)
        {
            _comenzi.Add(comandaNoua);
        }

        public List<Comanda> GetComenzi()
        {
            return _comenzi;
        }

        public Comanda GetComandaDupaId(int id)
        {
            return _comenzi.FirstOrDefault(c => c.ID == id);
        }
    }
}