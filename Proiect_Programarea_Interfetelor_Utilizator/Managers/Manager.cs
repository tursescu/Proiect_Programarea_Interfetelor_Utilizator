using Proiect_Programarea_Interfetelor_Utilizator.Enums;
using Proiect_Programarea_Interfetelor_Utilizator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proiect_Programarea_Interfetelor_Utilizator.Managers
{
    public class Manager
    {
        private List<Produs> _meniu;
        private List<Comanda> _comenzi;
        private int _urmatorulIdProdus = 1;
        private int _urmatorulIdComanda = 1;

        public Manager()
        {
            _meniu = new List<Produs>();
            _comenzi = new List<Comanda>();
        }

        public List<Produs> ObtineMeniu()
        {
            return _meniu;
        }

        public void AdaugaProdus(string nume, string detalii, decimal pretUnitar)
        {
            Produs produsNou = new Produs(_urmatorulIdProdus, nume, detalii, pretUnitar);
            _meniu.Add(produsNou);
            _urmatorulIdProdus++;
        }


        public Produs CautaProdus(int id)
        {
            foreach (Produs produsMeniu in _meniu)
            {
                if (produsMeniu.ID == id)
                {
                    return produsMeniu;
                }
            }
            return null;
        }

        public bool ModificaProdus(int id, string numeNou, string detaliiNoi, decimal pretNou)
        {
            Produs produsDeModificat = CautaProdus(id);
            if (produsDeModificat != null)
            {
                produsDeModificat.Nume = numeNou;
                produsDeModificat.Detalii = detaliiNoi;
                produsDeModificat.PretUnitar = pretNou;
                return true;
            }
            return false;
        }

        public bool StergeProdus(int id)
        {
            Produs produsDeSters = CautaProdus(id);
            if (produsDeSters != null)
            {
                _meniu.Remove(produsDeSters);
                return true;
            }
            return false;
        }

        public Comanda AdaugaComanda(string numeClient, string prenumeClient, string numarTelefon, DateTime dataLivrarii)
        {
            Comanda comandaNoua = new Comanda(_urmatorulIdComanda, numeClient, prenumeClient, numarTelefon, dataLivrarii);
            _comenzi.Add(comandaNoua);
            _urmatorulIdComanda++;
            return comandaNoua;
        }

        public List<Comanda> ObtineComenzi()
        {
            return _comenzi;
        }

        public Comanda CautaComanda(int id)
        {
            foreach (Comanda comanda in _comenzi)
            {
                if (comanda.ID == id)
                {
                    return comanda;
                }
            }
            return null;
        }

        public bool ModificaComanda(int id, DateTime dataLivrariiNoua, StatusComanda statusComandaNou, StatusPlata statusPlataNou)
        {
            Comanda comandaDeModificat = CautaComanda(id);
            if (comandaDeModificat != null)
            {
                comandaDeModificat.DataLivrarii = dataLivrariiNoua;
                comandaDeModificat.StatusComanda = statusComandaNou;
                comandaDeModificat.StatusPlata = statusPlataNou;

                return true;
            }
            return false;
        }

        public bool StergeComanda(int id)
        {
            Comanda comandaDeSters = CautaComanda(id);
            if (comandaDeSters != null)
            {
                _comenzi.Remove(comandaDeSters);
                return true;
            }
            return false;
        }
    }
}
