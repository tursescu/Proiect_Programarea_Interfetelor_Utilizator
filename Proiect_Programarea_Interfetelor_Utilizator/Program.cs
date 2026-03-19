using System;
using NivelStocareDate;
using LibrarieModele.Models;
using LibrarieModele.Enums;

namespace Proiect_Programarea_Interfetelor_Utilizator
{
    class Program
    {
        static void Main(string[] args)
        {
            AdministrareProduseMemorie adminProduse = new AdministrareProduseMemorie();
            AdministrareComenziMemorie adminComenzi = new AdministrareComenziMemorie();

            int idProdusCurent = 1;
            int idComandaCurenta = 1;
            Produs ecler = new Produs(idProdusCurent++, "Ecler cu ciocolata", "Glazura", 15.5m);
            ecler.Caracteristici = CaracteristiciProdus.Niciuna;
            adminProduse.AdaugaProdus(ecler);
            Produs tarta = new Produs(idProdusCurent++, "Tarta cu fructe", "Mascarpone", 20m);
            tarta.Caracteristici = CaracteristiciProdus.DePost | CaracteristiciProdus.FaraZahar;
            adminProduse.AdaugaProdus(tarta);
            bool ruleaza = true;
            while (ruleaza)
            {
                Console.WriteLine("\n=== MENIU ===");
                Console.WriteLine("1. Adauga produs in meniu");
                Console.WriteLine("2. Afiseaza toate produsele");
                Console.WriteLine("3. Creeaza o comanda noua");
                Console.WriteLine("4. Afiseaza toate comenzile");
                Console.WriteLine("5. Modifica status comanda");
                Console.WriteLine("0. Iesire");
                Console.Write("Alege optiunea: ");
                string optiune = Console.ReadLine();
                Console.WriteLine();
                switch (optiune)
                {
                    case "1":
                        CitireSiSalvareProdus(adminProduse, ref idProdusCurent);
                        break;
                    case "2":
                        AfisareProduse(adminProduse);
                        break;
                    case "3":
                        CreareComanda(adminComenzi, adminProduse, ref idComandaCurenta);
                        break;
                    case "4":
                        AfisareComenzi(adminComenzi);
                        break;
                    case "5":
                        ModificaStatusComanda(adminComenzi);
                        break;
                    case "0":
                        ruleaza = false;
                        break;
                    default:
                        Console.WriteLine("Optiune invalida!");
                        break;
                }
            }
        }
        static void CitireSiSalvareProdus(AdministrareProduseMemorie admin, ref int id)
        {
            Console.WriteLine("--- Adaugare Produs ---");
            Console.Write("Nume: ");
            string nume = Console.ReadLine();
            Console.Write("Detalii: ");
            string detalii = Console.ReadLine();
            Console.Write("Pret unitar: ");
            decimal pret = Convert.ToDecimal(Console.ReadLine());
            Produs produsNou = new Produs(id, nume, detalii, pret);
            Console.WriteLine("\nCaracteristici disponibile:");
            foreach (var valoareEnum in Enum.GetValues(typeof(CaracteristiciProdus)))
            {
                Console.WriteLine($"{(int)valoareEnum} - {valoareEnum}");
            }
            Console.Write("Introdu suma caracteristicilor (sau 0 pentru Niciuna): ");
            int sumaCaracteristici = Convert.ToInt32(Console.ReadLine());
            produsNou.Caracteristici = (CaracteristiciProdus)sumaCaracteristici;
            admin.AdaugaProdus(produsNou);
            id++;
            Console.WriteLine("Produs salvat!");
        }

        static void AfisareProduse(AdministrareProduseMemorie admin)
        {
            List<Produs> lista = admin.GetProduse();
            Console.WriteLine("--- Produse in Meniu ---");
            if (lista.Count == 0) Console.WriteLine("Meniul este gol.");
            foreach (Produs p in lista)
            {
                Console.WriteLine($"[ID: {p.ID}] {p.Nume} | {p.PretUnitar} lei");
                Console.WriteLine($"        Detalii: {p.Detalii}");
                Console.WriteLine($"        Caracteristici: {p.Caracteristici}");
                Console.WriteLine();
            }
        }

        static void CreareComanda(AdministrareComenziMemorie adminC, AdministrareProduseMemorie adminP, ref int idComanda)
        {
            Console.WriteLine("--- Creare Comanda Noua ---");
            Console.Write("Nume client: ");
            string numeClient = Console.ReadLine();
            Console.Write("Prenume client: ");
            string prenumeClient = Console.ReadLine();
            Console.Write("Telefon: ");
            string telefon = Console.ReadLine();
            Comanda comandaNoua = new Comanda(idComanda, numeClient, prenumeClient, telefon, DateTime.Now.AddDays(1));
            AfisareProduse(adminP);
            bool adaugaProduse = true;
            while (adaugaProduse)
            {
                Console.Write("\nIntrodu numele produsului dorit (sau 'ok' pentru a finaliza): ");
                string numeCautat = Console.ReadLine();
                if (numeCautat.ToLower() == "ok")
                {
                    adaugaProduse = false;
                    continue;
                }
                Produs produsGasit = adminP.GetProdusDupaNume(numeCautat);
                if (produsGasit != null)
                {
                    Console.Write($"Cate bucati de '{produsGasit.Nume}' doresti? ");
                    int cantitate = Convert.ToInt32(Console.ReadLine());

                    comandaNoua.AdaugaProdus(produsGasit, cantitate);
                    Console.WriteLine("-> Produs adaugat pe bon!");
                }
                else
                {
                    Console.WriteLine("Produsul nu a fost gasit in meniu.");
                }
            }
            adminC.AdaugaComanda(comandaNoua);
            idComanda++;
            Console.WriteLine($"\nComanda salvata cu succes! Total de plata: {comandaNoua.PretTotal} lei.");
        }

        static void AfisareComenzi(AdministrareComenziMemorie adminC)
        {
            List<Comanda> lista = adminC.GetComenzi();
            Console.WriteLine("--- Lista Comenzi ---");
            if (lista.Count == 0) Console.WriteLine("Nu exista nicio comanda.");

            foreach (Comanda c in lista)
            {
                Console.WriteLine($"\nComanda [ID: {c.ID}] - Client: {c.NumeClient} {c.PrenumeClient}");
                Console.WriteLine($"Total: {c.PretTotal} lei | Status: {c.StatusComanda} | Plata: {c.StatusPlata}");
                Console.WriteLine("Produse comandate:");
                foreach (ArticolComanda articol in c.Produse)
                {
                    Console.WriteLine($"  - {articol.Cantitate}x {articol.ProdusComandat.Nume}");
                }
            }
        }

        static void ModificaStatusComanda(AdministrareComenziMemorie adminC)
        {
            Console.Write("Introdu ID-ul comenzii pe care vrei sa o modifici: ");
            int idCautat = Convert.ToInt32(Console.ReadLine());
            Comanda c = adminC.GetComandaDupaId(idCautat);
            if (c != null)
            {
                Console.WriteLine($"\nComanda gasita! Status curent: {c.StatusComanda} | Plata: {c.StatusPlata}");
                Console.WriteLine("\nAlege noul Status al Comenzii:");
                Console.WriteLine("1. InAsteptare");
                Console.WriteLine("2. InProcesare");
                Console.WriteLine("3. Finalizata");
                Console.Write("Optiunea ta (1/2/3): ");
                string optStatus = Console.ReadLine();
                switch (optStatus)
                {
                    case "1": c.StatusComanda = StatusComanda.InAsteptare; break;
                    case "2": c.StatusComanda = StatusComanda.InProcesare; break;
                    case "3": c.StatusComanda = StatusComanda.Finalizata; break;
                    default: Console.WriteLine("Optiune invalida. Statusul comenzii ramane neschimbat."); break;
                }
                Console.WriteLine("\nAlege noul Status al Platii:");
                Console.WriteLine("1. Neplatita");
                Console.WriteLine("2. Platita");
                Console.Write("Optiunea ta (1/2): ");
                string optPlata = Console.ReadLine();
                switch (optPlata)
                {
                    case "1": c.StatusPlata = StatusPlata.Neplatita; break;
                    case "2": c.StatusPlata = StatusPlata.Platita; break;
                    default: Console.WriteLine("Optiune invalida. Statusul platii ramane neschimbat."); break;
                }
                Console.WriteLine($"\nNoile statusuri sunt -> Comanda: {c.StatusComanda} | Plata: {c.StatusPlata}");
            }
            else
            {
                Console.WriteLine("Eroare: Nu s-a gasit nicio comanda cu acest ID.");
            }
        }
    }
}