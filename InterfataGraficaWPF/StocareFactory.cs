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

        public static IStocareProduse GetAdministratorStocareProduse()
        {
            string formatSalvare = ConfigurationManager.AppSettings[FORMAT_SALVARE] ?? "";
            string numeFisier = ConfigurationManager.AppSettings[NUME_FISIER_PRODUSE] ?? "";

            string locatieFisierSolutie = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? "";
            string caleCompletaFisier = locatieFisierSolutie + "\\" + numeFisier;

            if (formatSalvare != null)
            {
                switch (formatSalvare)
                {
                    case "txt":
                        return new AdministrareProduseFisierText(caleCompletaFisier + "." + formatSalvare);
                    case "memorie":
                    default:
                        return new AdministrareProduseMemorie();
                }
            }
            return null;
        }

        public static IStocareComenzi GetAdministratorStocareComenzi()
        {
            string formatSalvare = ConfigurationManager.AppSettings[FORMAT_SALVARE] ?? "";
            string numeFisier = ConfigurationManager.AppSettings[NUME_FISIER_COMENZI] ?? "";

            string locatieFisierSolutie = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? "";
            string caleCompletaFisier = locatieFisierSolutie + "\\" + numeFisier;

            if (formatSalvare != null)
            {
                switch (formatSalvare)
                {
                    case "txt":
                        return new AdministrareComenziFisierText(caleCompletaFisier + "." + formatSalvare);
                    case "memorie":
                    default:
                        return new AdministrareComenziMemorie();
                }
            }
            return null;
        }
    }
}