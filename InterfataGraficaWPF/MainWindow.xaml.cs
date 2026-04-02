using LibrarieModele.Models;
using NivelStocareDate;
using Proiect_Programarea_Interfetelor_Utilizator;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InterfataGraficaWPF
{
    public partial class MainWindow : Window
    {
        private IStocareProduse adminProduse;

        public MainWindow()
        {
            InitializeComponent();
            adminProduse = StocareFactory.GetAdministratorStocareProduse();
            IncarcaDateInTabel();
        }

        private void IncarcaDateInTabel()
        {
            var listaProduse = adminProduse.GetProduse();
            tabelProduse.ItemsSource = listaProduse;
        }

        private void btnIncarca_Click(object sender, RoutedEventArgs e)
        {
            IncarcaDateInTabel();
            MessageBox.Show("Datele au fost reîncărcate din fișier!", "Succes");
        }
    }
}