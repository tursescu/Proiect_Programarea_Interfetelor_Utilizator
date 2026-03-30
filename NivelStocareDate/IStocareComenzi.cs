using System;
using System.Collections.Generic;
using System.Text;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public interface IStocareComenzi
    {
        void AdaugaComanda(Comanda c);
        List<Comanda> GetComenzi();
        Comanda GetComandaDupaId(int id);
        bool UpdateComanda(Comanda c);
    }
}
