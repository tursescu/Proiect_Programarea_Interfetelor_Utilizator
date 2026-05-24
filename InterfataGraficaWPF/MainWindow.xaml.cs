using LibrarieModele.Enums;
using LibrarieModele.Models;
using NivelStocareDate;
using Proiect_Programarea_Interfetelor_Utilizator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InterfataGraficaWPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Constante pentru validarea datelor introduse de utilizator
        private const int LUNGIME_MAXIMA_NUME = 30;
        private const int LUNGIME_MAXIMA_DETALII = 100;
        private const int LUNGIME_TELEFON = 10;
        private const decimal PRET_MINIM = 0.01m;
        private const decimal PRET_MAXIM = 10000m;
        private const int CANTITATE_MINIMA = 1;
        private const int CANTITATE_MAXIMA = 1000;

        // Administratorii responsabili de logica de stocare a datelor
        private IStocareProduse adminProduse;
        private IStocareComenzi adminComenzi;
        private IStocareArticoleComenzi adminArticoleComenzi;

        // Liste și obiecte temporare folosite pe durata procesului de editare/adăugare
        private Comanda comandaInEditare = null;
        private List<ArticolComanda> articoleComandaInEditare = new List<ArticolComanda>();
        private List<ArticolComanda> articoleComandaCurenta = new List<ArticolComanda>();

        // Obiect legat (prin DataBinding) de formularul de modificare a unui produs
        private Produs produsInEditare;
        public Produs ProdusInEditare
        {
            get { return produsInEditare; }
            set { produsInEditare = value; OnPropertyChanged(); }
        }

        // Eveniment necesar pentru interfața INotifyPropertyChanged (actualizează UI-ul automat când modelul se modifică)
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Constructorul ferestrei principale
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Setăm contextul de date pe instanța curentă

            // Inițializăm managerii de stocare 
            adminProduse = StocareFactory.GetAdministratorStocareProduse();
            adminComenzi = StocareFactory.GetAdministratorStocareComenzi();
            adminArticoleComenzi = StocareFactory.GetAdministratorStocareArticoleComenzi();

            // Setăm valorile implicite pentru calendare și modurile de plată
            dpDataLivrare.SelectedDate = DateTime.Today.AddDays(1);
            dpDataAdaugare.SelectedDate = DateTime.Today;
            lbModPlata.ItemsSource = Enum.GetValues(typeof(ModPlata));
            lbModPlata.SelectedIndex = 0;

            // Încărcăm datele de start în liste
            ReincarcaProduseInComboBox();
        }

        // ASCUNDERE TOATE PANELURILE: Funcție ajutătoare pentru navigarea laterală
        private void AscundeToatePanelurile()
        {
            panelAdaugaProdus.Visibility = Visibility.Collapsed;
            panelListaProduse.Visibility = Visibility.Collapsed;
            panelAdaugaComanda.Visibility = Visibility.Collapsed;
            panelListaComenzi.Visibility = Visibility.Collapsed;
            panelModificaProdus.Visibility = Visibility.Collapsed;
            panelModificaComanda.Visibility = Visibility.Collapsed;

            // Ascunde eventualele mesaje de status vechi când navigăm pe o pagină nouă
            tbMesajAdaugaProdus.Text = string.Empty;
            tbMesajAdaugaComanda.Text = string.Empty;
            tbMesajActualizareProdus.Text = string.Empty;
            tbMesajActualizareComanda.Text = string.Empty;
        }

        // ════════ MENIU LATERAL: Evenimente de click ════════
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

        private void btnMeniuModificaComanda_Click(object sender, RoutedEventArgs e)
        {
            AscundeToatePanelurile();
            panelModificaComanda.Visibility = Visibility.Visible;
            InitializeazaPanelModificaComanda();
        }

        // ════════ PRODUSE: Adăugare și Resetare ════════
        private void btnSalveazaProdus_Click(object sender, RoutedEventArgs e)
        {
            tbMesajAdaugaProdus.Text = string.Empty; // Curăță mesajele vechi

            string nume = txtNumeProdus.Text.Trim();
            string detalii = txtDetaliiProdus.Text.Trim();
            string sirPret = txtPretProdus.Text.Trim();

            // Verificăm dacă toate textele din inputuri respectă formatarea și tipurile de date
            if (!ValideazaDateProdus(nume, detalii, sirPret, out decimal pret))
                return;

            int idNou = GetUrmatorulIdProdus();
            Produs produsNou = new Produs(idNou, nume, detalii, pret)
            {
                Caracteristici = GetCaracteristiciSelectate(),
                DataAdaugare = dpDataAdaugare.SelectedDate ?? DateTime.Today,
                DataActualizare = DateTime.Today
            };

            adminProduse.AdaugaProdus(produsNou);

            // Afișăm succesul inline (fără MessageBox)
            tbMesajAdaugaProdus.Foreground = Brushes.Green;
            tbMesajAdaugaProdus.Text = $"Produsul '{nume}' a fost adăugat cu succes!";

            ResetFormularProdus();
            ReincarcaProduseInComboBox();
        }

        private void btnReseteazaProdus_Click(object sender, RoutedEventArgs e)
        {
            ResetFormularProdus();
            tbMesajAdaugaProdus.Text = string.Empty; // Curățăm și label-ul de validare globală
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

            // Ascundem erorile atașate de câmpuri
            AscundeEroare(txtNumeProdus, tbErrNumeProdus);
            AscundeEroare(txtDetaliiProdus, tbErrDetaliiProdus);
            AscundeEroare(txtPretProdus, tbErrPretProdus);
        }

        // Combină setările checkbox-urilor într-o valoare de tip Enum(Flags)
        private CaracteristiciProdus GetCaracteristiciSelectate()
        {
            CaracteristiciProdus c = CaracteristiciProdus.Niciuna;
            if (ckbDePost.IsChecked == true) c |= CaracteristiciProdus.DePost;
            if (ckbFaraZahar.IsChecked == true) c |= CaracteristiciProdus.FaraZahar;
            if (ckbFaraGluten.IsChecked == true) c |= CaracteristiciProdus.FaraGluten;
            if (ckbFaraLactoza.IsChecked == true) c |= CaracteristiciProdus.FaraLactoza;
            return c;
        }

        // Logica de validare a datelor introduse pentru produs
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
                AfiseazaEroare(txtNumeProdus, tbErrNumeProdus, $"Numele nu poate depăși {LUNGIME_MAXIMA_NUME} caractere!");
                return false;
            }
            if (string.IsNullOrEmpty(detalii))
            {
                AfiseazaEroare(txtDetaliiProdus, tbErrDetaliiProdus, "Detaliile sunt obligatorii!");
                return false;
            }
            if (detalii.Length > LUNGIME_MAXIMA_DETALII)
            {
                AfiseazaEroare(txtDetaliiProdus, tbErrDetaliiProdus, $"Detaliile nu pot depăși {LUNGIME_MAXIMA_DETALII} caractere!");
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
                AfiseazaEroare(txtPretProdus, tbErrPretProdus, $"Prețul trebuie să fie între {PRET_MINIM} și {PRET_MAXIM} lei!");
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

        // ════════ PRODUSE: Listare și Căutare ════════
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
                lblMesajCautareProdus.Foreground = Brushes.Red;
                dgProduse.ItemsSource = null;
                return;
            }

            List<Produs> gasite = adminProduse.GetProduseDupaNume(nume);
            if (gasite.Count == 0)
            {
                lblMesajCautareProdus.Content = "Niciun produs găsit!";
                lblMesajCautareProdus.Foreground = Brushes.Red;
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
            AfiseazaToateProdusele();
        }

        private void ReincarcaProduseInComboBox()
        {
            if (cbProduse != null)
                cbProduse.ItemsSource = adminProduse?.GetProduse();
        }

        // ════════ PRODUSE: Modificare ════════
        private void InitializeazaPanelModificaProdus()
        {
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
            ProdusInEditare = null;
            tbMesajActualizareProdus.Text = string.Empty;
        }

        private void cbProduseModificare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Produs produsSelectat = cbProduseModificare.SelectedItem as Produs;
            if (produsSelectat == null)
            {
                borderDetaliiProdusModificare.Visibility = Visibility.Collapsed;
                ProdusInEditare = null;
                return;
            }

            borderDetaliiProdusModificare.Visibility = Visibility.Visible;

            // Creăm o clonă pentru editare; previne actualizarea "live" greșită în listă în caz că renunțăm
            ProdusInEditare = new Produs(produsSelectat.ID, produsSelectat.Nume, produsSelectat.Detalii, produsSelectat.PretUnitar)
            {
                Caracteristici = produsSelectat.Caracteristici,
                DataAdaugare = produsSelectat.DataAdaugare,
                DataActualizare = produsSelectat.DataActualizare
            };

            // Pre-selectarea caracteristicilor produsului în ListBox
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
        }

        private void btnActualizeazaProdus_Click(object sender, RoutedEventArgs e)
        {
            if (ProdusInEditare == null)
            {
                tbMesajActualizareProdus.Foreground = Brushes.Red;
                tbMesajActualizareProdus.Text = "Selectează un produs!";
                return;
            }

            // Fallback suplimentar pentru protejare validare globală
            if (!ProdusInEditare.EsteValid)
            {
                tbMesajActualizareProdus.Foreground = Brushes.Red;
                tbMesajActualizareProdus.Text = "Datele introduse nu sunt valide!";
                return;
            }

            CaracteristiciProdus caracteristiciNoi = CaracteristiciProdus.Niciuna;
            foreach (var item in lbCaracteristiciModificare.SelectedItems)
            {
                caracteristiciNoi |= (CaracteristiciProdus)item;
            }
            ProdusInEditare.Caracteristici = caracteristiciNoi;

            // Suprascrie produsul existent în fișiere
            adminProduse.ModificaProdus(ProdusInEditare);

            // Succes inline
            tbMesajActualizareProdus.Foreground = Brushes.Green;
            tbMesajActualizareProdus.Text = $"Produsul '{ProdusInEditare.Nume}' a fost actualizat la {ProdusInEditare.DataActualizare:dd.MM.yyyy HH:mm}!";
            tbInfoDateProdus.Text = $"Adăugat: {ProdusInEditare.DataAdaugareAfisare}  |  Ultima actualizare: {ProdusInEditare.DataActualizareAfisare}";

            InitializeazaPanelModificaProdus();
            ReincarcaProduseInComboBox();
        }

        // ════════ COMENZI: Adăugare ════════
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

            // Creăm obiectul ce ține referința la produs și cantitate și îl adăugăm în lista curentă a comenzii
            articoleComandaCurenta.Add(new ArticolComanda(0, 0, produsSelectat, cantitate));
            ActualizeazaListaArticole();
            txtCantitate.Text = "1";
        }

        private void ActualizeazaListaArticole()
        {
            dgArticoleComanda.ItemsSource = null;
            dgArticoleComanda.ItemsSource = articoleComandaCurenta;

            // Recalculare cost total comandă pe loc
            decimal total = articoleComandaCurenta.Sum(a => a.PretTotalArticol);
            tbTotalComanda.Text = $"Total comandă: {total} lei";
        }

        private void btnSalveazaComanda_Click(object sender, RoutedEventArgs e)
        {
            tbMesajAdaugaComanda.Text = string.Empty; // Curăță mesajul

            string nume = txtNumeClient.Text.Trim();
            string prenume = txtPrenumeClient.Text.Trim();
            string telefon = txtTelefon.Text.Trim();

            if (!ValideazaDateComanda(nume, prenume, telefon))
                return;

            int idNou = GetUrmatorulIdComanda();
            DateTime dataLivrare = dpDataLivrare.SelectedDate ?? DateTime.Today.AddDays(1);

            Comanda comandaNoua = new Comanda(idNou, nume, prenume, telefon, dataLivrare)
            {
                ModPlata = (ModPlata)(lbModPlata.SelectedItem ?? ModPlata.Numerar)
            };

            adminComenzi.AdaugaComanda(comandaNoua);

            // Salvăm referințele la produse specific pentru noul ID de comandă
            foreach (var articol in articoleComandaCurenta)
            {
                articol.IdComanda = idNou;
                adminArticoleComenzi.AdaugaArticol(articol);
            }

            tbMesajAdaugaComanda.Foreground = Brushes.Green;
            tbMesajAdaugaComanda.Text = $"Comanda #{idNou} a fost salvată cu succes! Total: {comandaNoua.PretTotal} lei";

            ResetFormularComanda();
        }

        private void btnReseteazaComanda_Click(object sender, RoutedEventArgs e)
        {
            ResetFormularComanda();
            tbMesajAdaugaComanda.Text = string.Empty;
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
                AfiseazaEroare(txtNumeClient, tbErrNumeClient, $"Numele nu poate depăși {LUNGIME_MAXIMA_NUME} caractere!");
                return false;
            }
            if (string.IsNullOrEmpty(prenume))
            {
                AfiseazaEroare(txtPrenumeClient, tbErrPrenumeClient, "Prenumele este obligatoriu!");
                return false;
            }
            if (prenume.Length > LUNGIME_MAXIMA_NUME)
            {
                AfiseazaEroare(txtPrenumeClient, tbErrPrenumeClient, $"Prenumele nu poate depăși {LUNGIME_MAXIMA_NUME} caractere!");
                return false;
            }
            if (string.IsNullOrEmpty(telefon))
            {
                AfiseazaEroare(txtTelefon, tbErrTelefon, "Telefonul este obligatoriu!");
                return false;
            }
            if (telefon.Length != LUNGIME_TELEFON || !telefon.All(char.IsDigit))
            {
                AfiseazaEroare(txtTelefon, tbErrTelefon, $"Telefonul trebuie să conțină exact {LUNGIME_TELEFON} cifre!");
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

        // ════════ COMENZI: Listare ════════
        private void AfiseazaComenzi()
        {
            List<Comanda> comenzi = adminComenzi.GetComenzi();
            List<Produs> toateProdusele = adminProduse.GetProduse();

            // Populează sub-elementele fiecărei comenzi pentru a putea vizualiza tabelul de produse din interior
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

        // ════════ COMENZI: Modificare ════════
        private void InitializeazaPanelModificaComanda()
        {
            var comenzi = adminComenzi.GetComenzi();

            // Formatăm frumos afișarea în meniul de DropDown 
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

            // Setăm datele din bază în câmpurile UI
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
                tbMesajActualizareComanda.Foreground = Brushes.Red;
                tbMesajActualizareComanda.Text = "Selectează un produs!";
                return;
            }

            if (!int.TryParse(txtCantitateAdaugareInComanda.Text.Trim(), out int cantitate))
            {
                tbMesajActualizareComanda.Foreground = Brushes.Red;
                tbMesajActualizareComanda.Text = "Cantitatea trebuie să fie un număr!";
                return;
            }
            if (cantitate < CANTITATE_MINIMA || cantitate > CANTITATE_MAXIMA)
            {
                tbMesajActualizareComanda.Foreground = Brushes.Red;
                tbMesajActualizareComanda.Text = $"Cantitatea trebuie între {CANTITATE_MINIMA} și {CANTITATE_MAXIMA}!";
                return;
            }

            var articolNou = new ArticolComanda(0, comandaInEditare.ID, produsSelectat, cantitate);
            articoleComandaInEditare.Add(articolNou);

            ActualizeazaDataGridArticoleModificare();
            txtCantitateAdaugareInComanda.Text = "1";
            tbMesajActualizareComanda.Text = string.Empty; // curăță eventuale mesaje de eroare vechi
        }

        private void btnStergeArticolModificare_Click(object sender, RoutedEventArgs e)
        {
            ArticolComanda articolSelectat = dgArticoleComandaModificare.SelectedItem as ArticolComanda;
            if (articolSelectat == null)
            {
                tbMesajActualizareComanda.Foreground = Brushes.Red;
                tbMesajActualizareComanda.Text = "Selectează un articol din tabel pentru a-l șterge!";
                return;
            }

            articoleComandaInEditare.Remove(articolSelectat);
            ActualizeazaDataGridArticoleModificare();
            tbMesajActualizareComanda.Text = string.Empty;
        }

        private void btnSalveazaModificariComanda_Click(object sender, RoutedEventArgs e)
        {
            if (comandaInEditare == null)
            {
                tbMesajActualizareComanda.Foreground = Brushes.Red;
                tbMesajActualizareComanda.Text = "Selectează o comandă!";
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

            // Rescriem produsele atașate comenzii
            adminArticoleComenzi.StergeToatePentruComanda(comandaInEditare.ID);
            foreach (var articol in articoleComandaInEditare)
            {
                articol.ID = 0;
                articol.IdComanda = comandaInEditare.ID;
                adminArticoleComenzi.AdaugaArticol(articol);
            }

            tbMesajActualizareComanda.Foreground = Brushes.Green;
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

        // ════════ FUNCȚII GENERICE PENTRU Erori de UI (Text Box) ════════
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
            textBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230)); // Fundal roz-roșiatic
            tbEroare.Text = mesaj;
            tbEroare.Visibility = Visibility.Visible;
            textBox.Focus();
        }
    }
}