using System.Windows;
using System.Windows.Controls;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для Sanc_page.xaml
    /// </summary>
    public partial class Sanc_page : Page
    {
        DB db = new DB();

        public Sanc_page()
        {
            InitializeComponent();
            table_sunc_DG.ItemsSource = db.create_table("[Система штрафов]").DefaultView;
        }

        private void save_changes_Click(object sender, RoutedEventArgs e)
        {
            db.update_table();
            table_sunc_DG.ItemsSource = db.create_table("[Система штрафов]").DefaultView;
        }

        private void not_save_changes_Click(object sender, RoutedEventArgs e)
        {
            table_sunc_DG.ItemsSource = db.create_table("[Система штрафов]").DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            table_sunc_DG.ItemsSource = db.filter_sanc_table(kod_tb.Text, name_tb.Text, sum_tb.Text).DefaultView;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            kod_tb.Text = "";
            name_tb.Text = "";
            sum_tb.Text = "";
        }
    }
}
