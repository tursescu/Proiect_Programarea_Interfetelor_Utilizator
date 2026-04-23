using LibrarieModele.Enums;
using LibrarieModele.Models;
using NivelStocareDate;
using Proiect_Programarea_Interfetelor_Utilizator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InterfataGraficaWPF
{
    public partial class MainWindow : Window
    {
        private const int LUNGIME_MAXIMA_NUME = 30;
        private const int LUNGIME_MAXIMA_DETALII = 100;
        private const int LUNGIME_TELEFON = 10;
        private const decimal PRET_MINIM = 0.01m;
        private const decimal PRET_MAXIM = 10000m;
        private const int CANTITATE_MINIMA = 1;
        private const int CANTITATE_MAXIMA = 1000;

        private IStocareProduse adminProduse;
        private IStocareComenzi adminComenzi;

        private List<ArticolComanda> articoleComandaCurenta = new List<ArticolComanda>();

        private Comanda comandaSelectata = null;

        public MainWindow()
        {
            InitializeComponent();
            adminProduse = StocareFactory.GetAdministratorStocareProduse();
            adminComenzi = StocareFactory.GetAdministratorStocareComenzi();

            dpDataLivrare.SelectedDate = DateTime.Today.AddDays(1);
            ReincarcaProduseInComboBox();
        }

        private void AscundeToatePanelurile()
        {
            panelAdaugaProdus.Visibility = Visibility.Collapsed;
            panelListaProduse.Visibility = Visibility.Collapsed;
            panelAdaugaComanda.Visibility = Visibility.Collapsed;
            panelListaComenzi.Visibility = Visibility.Collapsed;
        }

        private void btnMeniuAdaugaProdus_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanelurile();
            panelAdaugaProdus.Visibility = Visibility.Visible;
        }

        private void btnMeniuListaProduse_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanelurile();
            panelListaProduse.Visibility = Visibility.Visible;
            AfiseazaToateProdusele();
        }

        private void btnMeniuAdaugaComanda_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanelurile();
            panelAdaugaComanda.Visibility = Visibility.Visible;
            ReincarcaProduseInComboBox();
        }

        private void btnMeniuListaComenzi_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanelurile();
            panelListaComenzi.Visibility = Visibility.Visible;
            AfiseazaComenzi();
        }

        private void btnSalveazaProdus_Click(object sender, RoutedEventArgs e)
        {
            string nume = txtNumeProdus.Text.Trim();
            string detalii = txtDetaliiProdus.Text.Trim();
            string sirPret = txtPretProdus.Text.Trim();

            if (!ValideazaDateProdus(nume, detalii, sirPret, out decimal pret))
                return;

            int idNou = GetUrmatorulIdProdus();
            Produs produsNou = new Produs(idNou, nume, detalii, pret);
            produsNou.Caracteristici = GetCaracteristiciSelectate();

            adminProduse.AdaugaProdus(produsNou);

            MessageBox.Show($"Produsul '{nume}' a fost adăugat cu succes!", "Succes",
                MessageBoxButton.OK, MessageBoxImage.Information);

            ResetFormularProdus();
            ReincarcaProduseInComboBox();
        }

        private void btnReseteazaProdus_Click(object sender, RoutedEventArgs e)
        {
            ResetFormularProdus();
        }

        private void ResetFormularProdus()
        {
            txtNumeProdus.Clear();
            txtDetaliiProdus.Clear();
            txtPretProdus.Clear();
            ckbDePost.IsChecked = false;
            ckbFaraZahar.IsChecked = false;
            ckbFaraGluten.IsChecked = false;
            ckbFaraLactoza.IsChecked = false;
            AscundeEroare(txtNumeProdus, tbErrNumeProdus);
            AscundeEroare(txtDetaliiProdus, tbErrDetaliiProdus);
            AscundeEroare(txtPretProdus, tbErrPretProdus);
        }

        private CaracteristiciProdus GetCaracteristiciSelectate()
        {
            CaracteristiciProdus c = CaracteristiciProdus.Niciuna;
            if (ckbDePost.IsChecked == true) c |= CaracteristiciProdus.DePost;
            if (ckbFaraZahar.IsChecked == true) c |= CaracteristiciProdus.FaraZahar;
            if (ckbFaraGluten.IsChecked == true) c |= CaracteristiciProdus.FaraGluten;
            if (ckbFaraLactoza.IsChecked == true) c |= CaracteristiciProdus.FaraLactoza;
            return c;
        }

        private bool ValideazaDateProdus(string nume, string detalii, string sirPret, out decimal pret)
        {
            pret = 0;
            AscundeEroare(txtNumeProdus, tbErrNumeProdus);
            AscundeEroare(txtDetaliiProdus, tbErrDetaliiProdus);
            AscundeEroare(txtPretProdus, tbErrPretProdus);

            if (string.IsNullOrEmpty(nume))
            {
                AfiseazaEroare(txtNumeProdus, tbErrNumeProdus, "Numele produsului este obligatoriu!");
                return false;
            }
            if (nume.Length > LUNGIME_MAXIMA_NUME)
            {
                AfiseazaEroare(txtNumeProdus, tbErrNumeProdus,
                    $"Numele nu poate depăși {LUNGIME_MAXIMA_NUME} caractere!");
                return false;
            }
            if (string.IsNullOrEmpty(detalii))
            {
                AfiseazaEroare(txtDetaliiProdus, tbErrDetaliiProdus, "Detaliile sunt obligatorii!");
                return false;
            }
            if (detalii.Length > LUNGIME_MAXIMA_DETALII)
            {
                AfiseazaEroare(txtDetaliiProdus, tbErrDetaliiProdus,
                    $"Detaliile nu pot depăși {LUNGIME_MAXIMA_DETALII} caractere!");
                return false;
            }
            if (string.IsNullOrEmpty(sirPret))
            {
                AfiseazaEroare(txtPretProdus, tbErrPretProdus, "Prețul este obligatoriu!");
                return false;
            }
            if (!decimal.TryParse(sirPret, out pret))
            {
                AfiseazaEroare(txtPretProdus, tbErrPretProdus, "Prețul trebuie să fie un număr!");
                return false;
            }
            if (pret < PRET_MINIM || pret > PRET_MAXIM)
            {
                AfiseazaEroare(txtPretProdus, tbErrPretProdus,
                    $"Prețul trebuie să fie între {PRET_MINIM} și {PRET_MAXIM} lei!");
                return false;
            }
            return true;
        }

        private int GetUrmatorulIdProdus()
        {
            var produse = adminProduse.GetProduse();
            if (produse.Count == 0) return 1;
            return produse.Max(p => p.ID) + 1;
        }

        private void AfiseazaToateProdusele()
        {
            dgProduse.ItemsSource = adminProduse.GetProduse();
            lblMesajCautareProdus.Content = string.Empty;
        }

        private void btnCautaProdus_Click(object sender, RoutedEventArgs e)
        {
            string nume = txtCautaProdus.Text.Trim();
            if (string.IsNullOrEmpty(nume))
            {
                lblMesajCautareProdus.Content = "Introduceți un nume!";
                dgProduse.ItemsSource = null;
                return;
            }

            List<Produs> gasite = adminProduse.GetProduseDupaNume(nume);
            if (gasite.Count == 0)
            {
                lblMesajCautareProdus.Content = "Niciun produs găsit!";
                dgProduse.ItemsSource = null;
            }
            else
            {
                lblMesajCautareProdus.Content = $"S-au găsit {gasite.Count} produse";
                lblMesajCautareProdus.Foreground = Brushes.Green;
                dgProduse.ItemsSource = gasite;
            }
        }

        private void btnAfiseazaToateProduse_Click(object sender, RoutedEventArgs e)
        {
            txtCautaProdus.Clear();
            lblMesajCautareProdus.Foreground = Brushes.Red;
            AfiseazaToateProdusele();
        }

        private void ReincarcaProduseInComboBox()
        {
            if (cbProduse != null)
                cbProduse.ItemsSource = adminProduse?.GetProduse();
        }

        private void btnAdaugaProdusInComanda_Click(object sender, RoutedEventArgs e)
        {
            tbErrCantitate.Visibility = Visibility.Collapsed;

            Produs produsSelectat = cbProduse.SelectedItem as Produs;
            if (produsSelectat == null)
            {
                tbErrCantitate.Text = "Selectează un produs!";
                tbErrCantitate.Visibility = Visibility.Visible;
                return;
            }

            if (!int.TryParse(txtCantitate.Text.Trim(), out int cantitate))
            {
                tbErrCantitate.Text = "Cantitatea trebuie să fie un număr!";
                tbErrCantitate.Visibility = Visibility.Visible;
                return;
            }
            if (cantitate < CANTITATE_MINIMA || cantitate > CANTITATE_MAXIMA)
            {
                tbErrCantitate.Text = $"Cantitatea trebuie să fie între {CANTITATE_MINIMA} și {CANTITATE_MAXIMA}!";
                tbErrCantitate.Visibility = Visibility.Visible;
                return;
            }

            articoleComandaCurenta.Add(new ArticolComanda(produsSelectat, cantitate));
            ActualizeazaListaArticole();
            txtCantitate.Text = "1";
        }

        private void ActualizeazaListaArticole()
        {
            dgArticoleComanda.ItemsSource = null;
            dgArticoleComanda.ItemsSource = articoleComandaCurenta;

            decimal total = articoleComandaCurenta.Sum(a => a.PretTotalArticol);
            tbTotalComanda.Text = $"Total comandă: {total} lei";
        }

        private void btnSalveazaComanda_Click(object sender, RoutedEventArgs e)
        {
            string nume = txtNumeClient.Text.Trim();
            string prenume = txtPrenumeClient.Text.Trim();
            string telefon = txtTelefon.Text.Trim();

            if (!ValideazaDateComanda(nume, prenume, telefon))
                return;

            int idNou = GetUrmatorulIdComanda();
            DateTime dataLivrare = dpDataLivrare.SelectedDate ?? DateTime.Today.AddDays(1);

            Comanda comandaNoua = new Comanda(idNou, nume, prenume, telefon, dataLivrare);
            foreach (var articol in articoleComandaCurenta)
            {
                comandaNoua.AdaugaProdus(articol.ProdusComandat, articol.Cantitate);
            }

            adminComenzi.AdaugaComanda(comandaNoua);

            MessageBox.Show($"Comanda #{idNou} a fost salvată cu succes! Total: {comandaNoua.PretTotal} lei",
                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

            ResetFormularComanda();
        }

        private void btnReseteazaComanda_Click(object sender, RoutedEventArgs e)
        {
            ResetFormularComanda();
        }

        private void ResetFormularComanda()
        {
            txtNumeClient.Clear();
            txtPrenumeClient.Clear();
            txtTelefon.Clear();
            dpDataLivrare.SelectedDate = DateTime.Today.AddDays(1);
            txtCantitate.Text = "1";
            cbProduse.SelectedIndex = -1;
            articoleComandaCurenta.Clear();
            ActualizeazaListaArticole();
            AscundeEroare(txtNumeClient, tbErrNumeClient);
            AscundeEroare(txtPrenumeClient, tbErrPrenumeClient);
            AscundeEroare(txtTelefon, tbErrTelefon);
            tbErrProduse.Visibility = Visibility.Collapsed;
            tbErrCantitate.Visibility = Visibility.Collapsed;
        }

        private bool ValideazaDateComanda(string nume, string prenume, string telefon)
        {
            AscundeEroare(txtNumeClient, tbErrNumeClient);
            AscundeEroare(txtPrenumeClient, tbErrPrenumeClient);
            AscundeEroare(txtTelefon, tbErrTelefon);
            tbErrProduse.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(nume))
            {
                AfiseazaEroare(txtNumeClient, tbErrNumeClient, "Numele clientului este obligatoriu!");
                return false;
            }
            if (nume.Length > LUNGIME_MAXIMA_NUME)
            {
                AfiseazaEroare(txtNumeClient, tbErrNumeClient,
                    $"Numele nu poate depăși {LUNGIME_MAXIMA_NUME} caractere!");
                return false;
            }
            if (string.IsNullOrEmpty(prenume))
            {
                AfiseazaEroare(txtPrenumeClient, tbErrPrenumeClient, "Prenumele este obligatoriu!");
                return false;
            }
            if (prenume.Length > LUNGIME_MAXIMA_NUME)
            {
                AfiseazaEroare(txtPrenumeClient, tbErrPrenumeClient,
                    $"Prenumele nu poate depăși {LUNGIME_MAXIMA_NUME} caractere!");
                return false;
            }
            if (string.IsNullOrEmpty(telefon))
            {
                AfiseazaEroare(txtTelefon, tbErrTelefon, "Telefonul este obligatoriu!");
                return false;
            }
            if (telefon.Length != LUNGIME_TELEFON || !telefon.All(char.IsDigit))
            {
                AfiseazaEroare(txtTelefon, tbErrTelefon,
                    $"Telefonul trebuie să conțină exact {LUNGIME_TELEFON} cifre!");
                return false;
            }
            if (articoleComandaCurenta.Count == 0)
            {
                tbErrProduse.Text = "Adaugă cel puțin un produs la comandă!";
                tbErrProduse.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }

        private int GetUrmatorulIdComanda()
        {
            var comenzi = adminComenzi.GetComenzi();
            if (comenzi.Count == 0) return 1;
            return comenzi.Max(c => c.ID) + 1;
        }

        private void AfiseazaComenzi()
        {
            dgComenzi.ItemsSource = null;
            dgComenzi.ItemsSource = adminComenzi.GetComenzi();
            comandaSelectata = null;
            DezactiveazaRadioButtons();
        }

        private void dgComenzi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comandaSelectata = dgComenzi.SelectedItem as Comanda;
            if (comandaSelectata == null)
            {
                DezactiveazaRadioButtons();
                return;
            }

            rbInAsteptare.IsChecked = comandaSelectata.StatusComanda == StatusComanda.InAsteptare;
            rbInProcesare.IsChecked = comandaSelectata.StatusComanda == StatusComanda.InProcesare;
            rbFinalizata.IsChecked = comandaSelectata.StatusComanda == StatusComanda.Finalizata;

            rbNeplatita.IsChecked = comandaSelectata.StatusPlata == StatusPlata.Neplatita;
            rbPlatita.IsChecked = comandaSelectata.StatusPlata == StatusPlata.Platita;
        }

        private void DezactiveazaRadioButtons()
        {
            rbInAsteptare.IsChecked = false;
            rbInProcesare.IsChecked = false;
            rbFinalizata.IsChecked = false;
            rbNeplatita.IsChecked = false;
            rbPlatita.IsChecked = false;
        }

        private void btnSalveazaStatus_Click(object sender, RoutedEventArgs e)
        {
            if (comandaSelectata == null)
            {
                MessageBox.Show("Selectează o comandă din tabel!", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            comandaSelectata.StatusComanda = GetStatusComandaSelectat();
            comandaSelectata.StatusPlata = GetStatusPlataSelectat();

            adminComenzi.UpdateComanda(comandaSelectata);
            MessageBox.Show($"Statusurile pentru comanda #{comandaSelectata.ID} au fost actualizate!",
                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

            AfiseazaComenzi();
        }

        private StatusComanda GetStatusComandaSelectat()
        {
            if (rbInProcesare.IsChecked == true) return StatusComanda.InProcesare;
            if (rbFinalizata.IsChecked == true) return StatusComanda.Finalizata;
            return StatusComanda.InAsteptare;
        }

        private StatusPlata GetStatusPlataSelectat()
        {
            if (rbPlatita.IsChecked == true) return StatusPlata.Platita;
            return StatusPlata.Neplatita;
        }

        private void AscundeEroare(TextBox textBox, TextBlock tbEroare)
        {
            textBox.ClearValue(Control.BorderBrushProperty);
            textBox.ClearValue(Control.BackgroundProperty);
            tbEroare.Text = string.Empty;
            tbEroare.Visibility = Visibility.Collapsed;
        }

        private void AfiseazaEroare(TextBox textBox, TextBlock tbEroare, string mesaj)
        {
            textBox.BorderBrush = Brushes.Red;
            textBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
            tbEroare.Text = mesaj;
            tbEroare.Visibility = Visibility.Visible;
            textBox.Focus();
        }
    }
}