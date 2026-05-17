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
        private IStocareArticoleComenzi adminArticoleComenzi; 
        private Comanda comandaInEditare = null;
        private List<ArticolComanda> articoleComandaInEditare = new List<ArticolComanda>();

        private List<ArticolComanda> articoleComandaCurenta = new List<ArticolComanda>();

        public MainWindow()
        {
            InitializeComponent();
            adminProduse = StocareFactory.GetAdministratorStocareProduse();
            adminComenzi = StocareFactory.GetAdministratorStocareComenzi();
            adminArticoleComenzi = StocareFactory.GetAdministratorStocareArticoleComenzi();

            dpDataLivrare.SelectedDate = DateTime.Today.AddDays(1);
            dpDataAdaugare.SelectedDate = DateTime.Today;

            lbModPlata.ItemsSource = Enum.GetValues(typeof(ModPlata));
            lbModPlata.SelectedIndex = 0;

            ReincarcaProduseInComboBox();
        }

        private void AscundeToatePanelurile()
        {
            panelAdaugaProdus.Visibility = Visibility.Collapsed;
            panelListaProduse.Visibility = Visibility.Collapsed;
            panelAdaugaComanda.Visibility = Visibility.Collapsed;
            panelListaComenzi.Visibility = Visibility.Collapsed;
            panelModificaProdus.Visibility = Visibility.Collapsed;
            panelModificaComanda.Visibility = Visibility.Collapsed;
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
        private void btnMeniuModificaProdus_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanelurile();
            panelModificaProdus.Visibility = Visibility.Visible;
            InitializeazaPanelModificaProdus();
        }

        private void InitializeazaPanelModificaProdus()
        {
            cbProduseModificare.ItemsSource = null;
            cbProduseModificare.ItemsSource = adminProduse.GetProduse();
            cbProduseModificare.SelectedIndex = -1;

            var caracteristici = new List<CaracteristiciProdus>();
            foreach (CaracteristiciProdus c in Enum.GetValues(typeof(CaracteristiciProdus)))
            {
                if (c != CaracteristiciProdus.Niciuna)
                    caracteristici.Add(c);
            }
            lbCaracteristiciModificare.ItemsSource = caracteristici;

            borderDetaliiProdusModificare.Visibility = Visibility.Collapsed;
            tbMesajActualizareProdus.Text = string.Empty;
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
            produsNou.DataAdaugare = dpDataAdaugare.SelectedDate ?? DateTime.Today;
            produsNou.DataActualizare = DateTime.Today;

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
            dpDataAdaugare.SelectedDate = DateTime.Today;
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

            articoleComandaCurenta.Add(new ArticolComanda(0, 0, produsSelectat, cantitate));
            ActualizeazaListaArticole();
            txtCantitate.Text = "1";
        }
        private void cbProduseModificare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Produs produsSelectat = cbProduseModificare.SelectedItem as Produs;
            if (produsSelectat == null)
            {
                borderDetaliiProdusModificare.Visibility = Visibility.Collapsed;
                return;
            }
            borderDetaliiProdusModificare.Visibility = Visibility.Visible;

            txtPretModificare.Text = produsSelectat.PretUnitar.ToString();
            txtDetaliiModificare.Text = produsSelectat.Detalii;

            lbCaracteristiciModificare.SelectedItems.Clear();
            foreach (CaracteristiciProdus c in Enum.GetValues(typeof(CaracteristiciProdus)))
            {
                if (c == CaracteristiciProdus.Niciuna) continue;
                if (produsSelectat.Caracteristici.HasFlag(c))
                {
                    lbCaracteristiciModificare.SelectedItems.Add(c);
                }
            }

            tbInfoDateProdus.Text = $"Adăugat: {produsSelectat.DataAdaugareAfisare}  |  Ultima actualizare: {produsSelectat.DataActualizareAfisare}";

            tbMesajActualizareProdus.Text = string.Empty;
            tbErrPretModificare.Visibility = Visibility.Collapsed;
        }
        private void btnActualizeazaProdus_Click(object sender, RoutedEventArgs e)
        {
            Produs produsSelectat = cbProduseModificare.SelectedItem as Produs;
            if (produsSelectat == null)
            {
                MessageBox.Show("Selectează un produs!", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            tbErrPretModificare.Visibility = Visibility.Collapsed;

            if (!decimal.TryParse(txtPretModificare.Text.Trim(), out decimal pretNou))
            {
                tbErrPretModificare.Text = "Prețul trebuie să fie un număr!";
                tbErrPretModificare.Visibility = Visibility.Visible;
                return;
            }
            if (pretNou < PRET_MINIM || pretNou > PRET_MAXIM)
            {
                tbErrPretModificare.Text = $"Prețul trebuie între {PRET_MINIM} și {PRET_MAXIM} lei!";
                tbErrPretModificare.Visibility = Visibility.Visible;
                return;
            }

            CaracteristiciProdus caracteristiciNoi = CaracteristiciProdus.Niciuna;
            foreach (var item in lbCaracteristiciModificare.SelectedItems)
            {
                caracteristiciNoi |= (CaracteristiciProdus)item;
            }

            produsSelectat.PretUnitar = pretNou;
            produsSelectat.Detalii = txtDetaliiModificare.Text.Trim();
            produsSelectat.Caracteristici = caracteristiciNoi;

            adminProduse.ModificaProdus(produsSelectat);

            tbMesajActualizareProdus.Text = $"Produsul '{produsSelectat.Nume}' a fost actualizat la {produsSelectat.DataActualizare:dd.MM.yyyy HH:mm}!";

            tbInfoDateProdus.Text = $"Adăugat: {produsSelectat.DataAdaugareAfisare}  |  Ultima actualizare: {produsSelectat.DataActualizareAfisare}";

            ReincarcaProduseInComboBox();
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
            comandaNoua.ModPlata = (ModPlata)(lbModPlata.SelectedItem ?? ModPlata.Numerar);

            adminComenzi.AdaugaComanda(comandaNoua);

            foreach (var articol in articoleComandaCurenta)
            {
                articol.IdComanda = idNou;
                adminArticoleComenzi.AdaugaArticol(articol);
            }

            MessageBox.Show($"Comanda #{idNou} a fost salvată cu succes! Total: {comandaNoua.PretTotal} lei",
                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

            ResetFormularComanda();
        }
        private void btnMeniuModificaComanda_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanelurile();
            panelModificaComanda.Visibility = Visibility.Visible;
            InitializeazaPanelModificaComanda();
        }

        private void InitializeazaPanelModificaComanda()
        {
            var comenzi = adminComenzi.GetComenzi();
            var displayList = comenzi.Select(c => new
            {
                Comanda = c,
                Display = $"#{c.ID} - {c.NumeClient} {c.PrenumeClient}"
            }).ToList();

            cbComenziModificare.ItemsSource = null;
            cbComenziModificare.ItemsSource = displayList;
            cbComenziModificare.DisplayMemberPath = "Display";
            cbComenziModificare.SelectedIndex = -1;

            lbModPlataModificare.ItemsSource = null;
            lbModPlataModificare.ItemsSource = Enum.GetValues(typeof(ModPlata));

            cbProduseAdaugareInComanda.ItemsSource = null;
            cbProduseAdaugareInComanda.ItemsSource = adminProduse.GetProduse();

            borderDetaliiComandaModificare.Visibility = Visibility.Collapsed;
            tbMesajActualizareComanda.Text = string.Empty;
            comandaInEditare = null;
            articoleComandaInEditare.Clear();
        }
        private void cbComenziModificare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic itemSelectat = cbComenziModificare.SelectedItem;
            if (itemSelectat == null)
            {
                borderDetaliiComandaModificare.Visibility = Visibility.Collapsed;
                comandaInEditare = null;
                return;
            }

            comandaInEditare = itemSelectat.Comanda as Comanda;
            if (comandaInEditare == null) return;

            borderDetaliiComandaModificare.Visibility = Visibility.Visible;

            txtNumeClientModificare.Text = comandaInEditare.NumeClient;
            txtPrenumeClientModificare.Text = comandaInEditare.PrenumeClient;
            txtTelefonModificare.Text = comandaInEditare.NumarTelefon;
            dpDataLivrareModificare.SelectedDate = comandaInEditare.DataLivrarii;

            lbModPlataModificare.SelectedItem = comandaInEditare.ModPlata;

            rbInAsteptareMod.IsChecked = comandaInEditare.StatusComanda == StatusComanda.InAsteptare;
            rbInProcesareMod.IsChecked = comandaInEditare.StatusComanda == StatusComanda.InProcesare;
            rbFinalizataMod.IsChecked = comandaInEditare.StatusComanda == StatusComanda.Finalizata;
            rbNeplatitaMod.IsChecked = comandaInEditare.StatusPlata == StatusPlata.Neplatita;
            rbPlatitaMod.IsChecked = comandaInEditare.StatusPlata == StatusPlata.Platita;

            var toateProdusele = adminProduse.GetProduse();
            articoleComandaInEditare = adminArticoleComenzi.GetArticolePentruComanda(comandaInEditare.ID);
            foreach (var articol in articoleComandaInEditare)
            {
                articol.ProdusComandat = toateProdusele.FirstOrDefault(p => p.ID == articol.IdProdus);
            }

            ActualizeazaDataGridArticoleModificare();

            AscundeEroare(txtNumeClientModificare, tbErrNumeClientModificare);
            AscundeEroare(txtPrenumeClientModificare, tbErrPrenumeClientModificare);
            AscundeEroare(txtTelefonModificare, tbErrTelefonModificare);
            tbMesajActualizareComanda.Text = string.Empty;
        }
        private void ActualizeazaDataGridArticoleModificare()
        {
            dgArticoleComandaModificare.ItemsSource = null;
            dgArticoleComandaModificare.ItemsSource = articoleComandaInEditare;

            decimal total = articoleComandaInEditare.Sum(a => a.PretTotalArticol);
            tbTotalComandaMod.Text = $"Total comandă: {total} lei";
        }
        private void btnAdaugaProdusInComandaMod_Click(object sender, RoutedEventArgs e)
        {
            Produs produsSelectat = cbProduseAdaugareInComanda.SelectedItem as Produs;
            if (produsSelectat == null)
            {
                MessageBox.Show("Selectează un produs!", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCantitateAdaugareInComanda.Text.Trim(), out int cantitate))
            {
                MessageBox.Show("Cantitatea trebuie să fie un număr!", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cantitate < CANTITATE_MINIMA || cantitate > CANTITATE_MAXIMA)
            {
                MessageBox.Show($"Cantitatea trebuie între {CANTITATE_MINIMA} și {CANTITATE_MAXIMA}!", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var articolNou = new ArticolComanda(0, comandaInEditare.ID, produsSelectat, cantitate);
            articoleComandaInEditare.Add(articolNou);

            ActualizeazaDataGridArticoleModificare();
            txtCantitateAdaugareInComanda.Text = "1";
        }
        private void btnStergeArticolModificare_Click(object sender, RoutedEventArgs e)
        {
            ArticolComanda articolSelectat = dgArticoleComandaModificare.SelectedItem as ArticolComanda;
            if (articolSelectat == null)
            {
                MessageBox.Show("Selectează un articol din tabel pentru a-l șterge!", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            articoleComandaInEditare.Remove(articolSelectat);
            ActualizeazaDataGridArticoleModificare();
        }
        private void btnSalveazaModificariComanda_Click(object sender, RoutedEventArgs e)
        {
            if (comandaInEditare == null)
            {
                MessageBox.Show("Selectează o comandă!", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nume = txtNumeClientModificare.Text.Trim();
            string prenume = txtPrenumeClientModificare.Text.Trim();
            string telefon = txtTelefonModificare.Text.Trim();

            AscundeEroare(txtNumeClientModificare, tbErrNumeClientModificare);
            AscundeEroare(txtPrenumeClientModificare, tbErrPrenumeClientModificare);
            AscundeEroare(txtTelefonModificare, tbErrTelefonModificare);

            if (string.IsNullOrEmpty(nume))
            {
                AfiseazaEroare(txtNumeClientModificare, tbErrNumeClientModificare, "Numele este obligatoriu!");
                return;
            }
            if (string.IsNullOrEmpty(prenume))
            {
                AfiseazaEroare(txtPrenumeClientModificare, tbErrPrenumeClientModificare, "Prenumele este obligatoriu!");
                return;
            }
            if (telefon.Length != LUNGIME_TELEFON || !telefon.All(char.IsDigit))
            {
                AfiseazaEroare(txtTelefonModificare, tbErrTelefonModificare, $"Telefonul trebuie să conțină exact {LUNGIME_TELEFON} cifre!");
                return;
            }

            comandaInEditare.NumeClient = nume;
            comandaInEditare.PrenumeClient = prenume;
            comandaInEditare.NumarTelefon = telefon;
            comandaInEditare.DataLivrarii = dpDataLivrareModificare.SelectedDate ?? DateTime.Today;
            comandaInEditare.ModPlata = (ModPlata)(lbModPlataModificare.SelectedItem ?? ModPlata.Numerar);
            comandaInEditare.StatusComanda = GetStatusComandaModSelectat();
            comandaInEditare.StatusPlata = GetStatusPlataModSelectat();

            adminComenzi.ModificaComanda(comandaInEditare);

            adminArticoleComenzi.StergeToatePentruComanda(comandaInEditare.ID);
            foreach (var articol in articoleComandaInEditare)
            {
                articol.ID = 0;
                articol.IdComanda = comandaInEditare.ID;
                adminArticoleComenzi.AdaugaArticol(articol);
            }

            tbMesajActualizareComanda.Text = $"Comanda #{comandaInEditare.ID} a fost actualizată cu succes!";

            InitializeazaPanelModificaComanda();
        }

        private StatusComanda GetStatusComandaModSelectat()
        {
            if (rbInProcesareMod.IsChecked == true) return StatusComanda.InProcesare;
            if (rbFinalizataMod.IsChecked == true) return StatusComanda.Finalizata;
            return StatusComanda.InAsteptare;
        }

        private StatusPlata GetStatusPlataModSelectat()
        {
            if (rbPlatitaMod.IsChecked == true) return StatusPlata.Platita;
            return StatusPlata.Neplatita;
        }

        private void btnReseteazaComanda_Click(object sender, RoutedEventArgs e)
        {
            ResetFormularComanda();
        }
        private void dgComenzi_SelectionChanged_Lista(object sender, SelectionChangedEventArgs e)
        {
            Comanda comandaAleasa = dgComenzi.SelectedItem as Comanda;
            if (comandaAleasa == null)
            {
                tbTitluArticole.Text = "Selectează o comandă pentru a vedea produsele";
                dgArticoleSelectate.ItemsSource = null;
                tbTotalArticole.Text = string.Empty;
                return;
            }

            tbTitluArticole.Text = $"Produse din comanda #{comandaAleasa.ID} - {comandaAleasa.NumeClient} {comandaAleasa.PrenumeClient}";
            dgArticoleSelectate.ItemsSource = null;
            dgArticoleSelectate.ItemsSource = comandaAleasa.Produse;
            tbTotalArticole.Text = $"Total comandă: {comandaAleasa.PretTotal} lei";
        }
        private void ResetFormularComanda()
        {
            txtNumeClient.Clear();
            txtPrenumeClient.Clear();
            txtTelefon.Clear();
            dpDataLivrare.SelectedDate = DateTime.Today.AddDays(1);
            txtCantitate.Text = "1";
            cbProduse.SelectedIndex = -1;
            lbModPlata.SelectedIndex = 0;
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
            List<Comanda> comenzi = adminComenzi.GetComenzi();

            List<Produs> toateProdusele = adminProduse.GetProduse();
            foreach (Comanda c in comenzi)
            {
                c.Produse = adminArticoleComenzi.GetArticolePentruComanda(c.ID);
                foreach (ArticolComanda articol in c.Produse)
                {
                    articol.ProdusComandat = toateProdusele.FirstOrDefault(p => p.ID == articol.IdProdus);
                }
            }

            dgComenzi.ItemsSource = null;
            dgComenzi.ItemsSource = comenzi;
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