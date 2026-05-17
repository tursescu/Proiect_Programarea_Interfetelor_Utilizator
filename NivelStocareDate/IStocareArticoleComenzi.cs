using System.Collections.Generic;
using LibrarieModele.Models;

namespace NivelStocareDate
{
    public interface IStocareArticoleComenzi
    {
        void AdaugaArticol(ArticolComanda a);
        List<ArticolComanda> GetArticole();
        List<ArticolComanda> GetArticolePentruComanda(int idComanda);
        bool UpdateArticol(ArticolComanda a);
        bool StergeArticol(int idArticol);
        bool StergeToatePentruComanda(int idComanda);
    }
}