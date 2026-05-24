using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LibrarieModele.Enums;

namespace LibrarieModele.Models
{
    public class Produs : INotifyPropertyChanged, IDataErrorInfo
    {
        public const int LUNGIME_MAXIMA_NUME = 30;
        public const int LUNGIME_MAXIMA_DETALII = 100;
        public const decimal PRET_MINIM = 0.01m;
        public const decimal PRET_MAXIM = 10000m;

        private const char SEPARATOR_PRINCIPAL_FISIER = ';';
        private const int INDEX_ID = 0;
        private const int INDEX_NUME = 1;
        private const int INDEX_PRET = 2;
        private const int INDEX_DETALII = 3;
        private const int INDEX_CARACTERISTICI = 4;
        private const int INDEX_DATA_ADAUGARE = 5;
        private const int INDEX_DATA_ACTUALIZARE = 6;

        private string nume;
        private decimal pretUnitar;
        private string detalii;
        private CaracteristiciProdus caracteristici;

        public int ID { get; set; }

        public string Nume
        {
            get { return nume; }
            set { nume = value; OnPropertyChanged(); OnPropertyChanged(nameof(EsteValid)); }
        }

        public decimal PretUnitar
        {
            get { return pretUnitar; }
            set { pretUnitar = value; OnPropertyChanged(); OnPropertyChanged(nameof(EsteValid)); }
        }

        public string Detalii
        {
            get { return detalii; }
            set { detalii = value; OnPropertyChanged(); OnPropertyChanged(nameof(EsteValid)); }
        }

        public CaracteristiciProdus Caracteristici
        {
            get { return caracteristici; }
            set { caracteristici = value; OnPropertyChanged(); }
        }

        public DateTime DataAdaugare { get; set; }
        public DateTime DataActualizare { get; set; }

        public string DataAdaugareAfisare
        {
            get { return DataAdaugare.ToString("dd.MM.yyyy"); }
        }
        public string DataActualizareAfisare
        {
            get { return DataActualizare.ToString("dd.MM.yyyy"); }
        }

        public Produs(int id, string nume, string detalii, decimal pretUnitar)
        {
            this.ID = id;
            this.Nume = nume;
            this.Detalii = detalii;
            this.PretUnitar = pretUnitar;
            this.Caracteristici = CaracteristiciProdus.Niciuna;
            this.DataAdaugare = DateTime.Today;
            this.DataActualizare = DateTime.Today;
        }

        public Produs(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER);

            this.ID = Convert.ToInt32(dateFisier[INDEX_ID]);
            this.Nume = dateFisier[INDEX_NUME];
            this.PretUnitar = Convert.ToDecimal(dateFisier[INDEX_PRET]);
            this.Detalii = dateFisier[INDEX_DETALII];
            this.Caracteristici = (CaracteristiciProdus)Convert.ToInt32(dateFisier[INDEX_CARACTERISTICI]);

            if (dateFisier.Length > INDEX_DATA_ADAUGARE && DateTime.TryParse(dateFisier[INDEX_DATA_ADAUGARE], out DateTime da))
                this.DataAdaugare = da;
            else
                this.DataAdaugare = DateTime.Today;

            if (dateFisier.Length > INDEX_DATA_ACTUALIZARE && DateTime.TryParse(dateFisier[INDEX_DATA_ACTUALIZARE], out DateTime dact))
                this.DataActualizare = dact;
            else
                this.DataActualizare = DateTime.Today;
        }

        public string ConversieLaSirPentruFisier()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                SEPARATOR_PRINCIPAL_FISIER,
                ID.ToString(),
                (Nume ?? "NECUNOSCUT"),
                PretUnitar.ToString(),
                (Detalii ?? "Fara detalii"),
                (int)Caracteristici,
                DataAdaugare.ToString("yyyy-MM-dd"),
                DataActualizare.ToString("yyyy-MM-dd"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Nume):
                        if (string.IsNullOrWhiteSpace(Nume))
                            return "Numele este obligatoriu!";
                        if (Nume.Length > LUNGIME_MAXIMA_NUME)
                            return $"Numele nu poate depasi {LUNGIME_MAXIMA_NUME} caractere!";
                        break;

                    case nameof(PretUnitar):
                        if (PretUnitar < PRET_MINIM || PretUnitar > PRET_MAXIM)
                            return $"Pretul trebuie sa fie intre {PRET_MINIM} si {PRET_MAXIM} lei!";
                        break;

                    case nameof(Detalii):
                        if (string.IsNullOrWhiteSpace(Detalii))
                            return "Detaliile sunt obligatorii!";
                        if (Detalii.Length > LUNGIME_MAXIMA_DETALII)
                            return $"Detaliile nu pot depasi {LUNGIME_MAXIMA_DETALII} caractere!";
                        break;
                }
                return null;
            }
        }

        public bool EsteValid
        {
            get
            {
                return string.IsNullOrEmpty(this[nameof(Nume)])
                    && string.IsNullOrEmpty(this[nameof(PretUnitar)])
                    && string.IsNullOrEmpty(this[nameof(Detalii)]);
            }
        }
    }
}