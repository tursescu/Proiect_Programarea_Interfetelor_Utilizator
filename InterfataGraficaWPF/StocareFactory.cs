using System;
using System.Configuration;
using System.IO;
using NivelStocareDate;

namespace Proiect_Programarea_Interfetelor_Utilizator
{
    public static class StocareFactory
    {
        private const string FORMAT_SALVARE = "FormatSalvare";
        private const string NUME_FISIER_PRODUSE = "NumeFisierProduse";
        private const string NUME_FISIER_COMENZI = "NumeFisierComenzi";
        private const string NUME_FISIER_ARTICOLE_COMENZI = "NumeFisierArticoleComenzi";

        public static IStocareProduse GetAdministratorStocareProduse()
        {
            string formatSalvare = ConfigurationManager.AppSettings[FORMAT_SALVARE] ?? "";
            string numeFisier = ConfigurationManager.AppSettings[NUME_FISIER_PRODUSE] ?? "";
            string caleCompletaFisier = GetCaleCompleta(numeFisier, formatSalvare);

            switch (formatSalvare)
            {
                case "txt":
                    return new AdministrareProduseFisierText(caleCompletaFisier);
                case "memorie":
                default:
                    return new AdministrareProduseMemorie();
            }
        }

        public static IStocareComenzi GetAdministratorStocareComenzi()
        {
            string formatSalvare = ConfigurationManager.AppSettings[FORMAT_SALVARE] ?? "";
            string numeFisier = ConfigurationManager.AppSettings[NUME_FISIER_COMENZI] ?? "";
            string caleCompletaFisier = GetCaleCompleta(numeFisier, formatSalvare);

            switch (formatSalvare)
            {
                case "txt":
                    return new AdministrareComenziFisierText(caleCompletaFisier);
                case "memorie":
                default:
                    return new AdministrareComenziMemorie();
            }
        }

        public static IStocareArticoleComenzi GetAdministratorStocareArticoleComenzi()
        {
            string formatSalvare = ConfigurationManager.AppSettings[FORMAT_SALVARE] ?? "";
            string numeFisier = ConfigurationManager.AppSettings[NUME_FISIER_ARTICOLE_COMENZI] ?? "";
            string caleCompletaFisier = GetCaleCompleta(numeFisier, formatSalvare);

            switch (formatSalvare)
            {
                case "txt":
                    return new AdministrareArticoleComenziFisierText(caleCompletaFisier);
                case "memorie":
                default:
                    return new AdministrareArticoleComenziMemorie();
            }
        }

        private static string GetCaleCompleta(string numeFisier, string formatSalvare)
        {
            string locatieFisierSolutie = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? "";
            return locatieFisierSolutie + "\\" + numeFisier + "." + formatSalvare;
        }
    }
}