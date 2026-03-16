using System;
using Proiect_Programarea_Interfetelor_Utilizator.Models;
using Proiect_Programarea_Interfetelor_Utilizator.Enums;

namespace Proiect_Programarea_Interfetelor_Utilizator
{
    class Program
    {
        static void Main(string[] args)
        {
            Produs[] vectorProduse = new Produs[100];
            int numarProduse = 0;
            bool ruleaza = true;
            while (ruleaza)
            {
                Console.WriteLine("\n=== MENIU ===");
                Console.WriteLine("1. Adauga produs (Citire + Salvare in vector)");
                Console.WriteLine("2. Afiseaza produse (Afisare din vector)");
                Console.WriteLine("3. Cauta produs (Cautare dupa nume)");
                Console.WriteLine("0. Iesire");
                Console.Write("Alege optiunea: ");

                string optiune = Console.ReadLine();
                Console.WriteLine();

                switch (optiune)
                {
                    case "1":
                        CitireSiSalvareProdus(vectorProduse, ref numarProduse);
                        break;
                    case "2":
                        AfisareVector(vectorProduse, numarProduse);
                        break;
                    case "3":
                        CautareInVector(vectorProduse, numarProduse);
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
        static void CitireSiSalvareProdus(Produs[] vector, ref int contor)
        {
            if (contor >= 100)
            {
                Console.WriteLine("Vectorul este plin! Nu mai poti adauga produse.");
                return;
            }
            Console.WriteLine("--- Adaugare Produs ---");
            Console.Write("Nume: ");
            string nume = Console.ReadLine();
            Console.Write("Detalii: ");
            string detalii = Console.ReadLine();
            Console.Write("Pret unitar: ");
            decimal pret = Convert.ToDecimal(Console.ReadLine());
            Produs produsNou = new Produs(contor + 1, nume, detalii, pret);
            vector[contor] = produsNou;
            contor++;
            Console.WriteLine("Produsul a fost salvat in vector!");
        }

        static void AfisareVector(Produs[] vector, int contor)
        {
            Console.WriteLine("--- Produse in Vector ---");
            if (contor == 0)
            {
                Console.WriteLine("Vectorul este gol.");
                return;
            }
            for (int i = 0; i < contor; i++)
            {
                Console.WriteLine($"ID: {vector[i].ID} | Nume: {vector[i].Nume} | Pret: {vector[i].PretUnitar} lei | Detalii: {vector[i].Detalii}");
            }
        }

        static void CautareInVector(Produs[] vector, int contor)
        {
            Console.WriteLine("--- Cautare Produs ---");
            Console.Write("Introdu cuvantul cautat in nume: ");
            string cuvantCautat = Console.ReadLine().ToLower();
            bool gasit = false;
            for (int i = 0; i < contor; i++)
            {
                if (vector[i].Nume.ToLower().Contains(cuvantCautat))
                {
                    Console.WriteLine($"- Gasit pe pozitia {i} in vector: {vector[i].Nume} ({vector[i].PretUnitar} lei)");
                    gasit = true;
                }
            }
            if (gasit == false)
            {
                Console.WriteLine("Nu a fost gasit niciun produs care sa corespunda criteriului.");
            }
        }
    }
}