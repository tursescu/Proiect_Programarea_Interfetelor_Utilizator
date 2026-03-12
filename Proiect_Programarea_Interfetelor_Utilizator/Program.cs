using System;
using Proiect_Programarea_Interfetelor_Utilizator.Models;
using Proiect_Programarea_Interfetelor_Utilizator.Enums;

namespace Proiect_Programarea_Interfetelor_Utilizator
{
    class Program
    {
        static void Main(string[] args)
        { 
            Console.WriteLine("1. Se creeaza produsele...");
            Produs ecler = new Produs(1, "Ecler cu ciocolata", "Glazura de ciocolata", 15.5m);
            Produs tarta = new Produs(2, "Tarta cu fructe", "Crema de mascarpone, fructe de padure, gelatina lamaie", 20.0m);
            Console.WriteLine($"[Produs creat] {ecler.Nume} - {ecler.PretUnitar} lei");
            Console.WriteLine($"[Produs creat] {tarta.Nume} - {tarta.PretUnitar} lei\n");

            Console.WriteLine("2. Se creeaza comanda...");
            DateTime dataLivrare = DateTime.Now.AddDays(1);
            Comanda comandaMea = new Comanda(101, "Popescu", "Ion", "0722123456", dataLivrare);
            Console.WriteLine($"[Comanda creata] ID: {comandaMea.ID} pentru {comandaMea.PrenumeClient} {comandaMea.NumeClient}");
            Console.WriteLine($"[Status Initial] Comanda: {comandaMea.StatusComanda} | Plata: {comandaMea.StatusPlata}\n");

            Console.WriteLine("3. Se adauga produse pe bon...");
            comandaMea.AdaugaProdus(ecler, 2);
            comandaMea.AdaugaProdus(tarta, 1);
            foreach (ArticolComanda articol in comandaMea.Produse)
            {
                Console.WriteLine($"- Adaugat: {articol.Cantitate} buc. x {articol.ProdusComandat.Nume}");
                Console.WriteLine($"  Pret: {articol.PretTotalArticol} lei");
            }
            Console.WriteLine();

            Console.WriteLine("4. Testare calcul Pret Total...");
            Console.WriteLine($"Total de plata calculat automat: {comandaMea.PretTotal} lei\n");

            Console.WriteLine("5. Testare modificare statusuri...");
            comandaMea.StatusComanda = StatusComanda.InProcesare;
            comandaMea.StatusPlata = StatusPlata.Platita;
            Console.WriteLine($"[Status Nou] Comanda: {comandaMea.StatusComanda} | Plata: {comandaMea.StatusPlata}\n");
            Console.ReadLine();
        }
    }
}