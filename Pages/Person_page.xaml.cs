using System.Windows;
using System.Windows.Controls;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для Add_person_page.xaml
    /// </summary>
    public partial class Person_page : Page
    {

        DB db = new DB();

        public Person_page()
        {
            InitializeComponent();

            table_person_DG.ItemsSource = db.create_table("Читатели").DefaultView;

        }

        private void save_changes_Click(object sender, RoutedEventArgs e)
        {
            db.update_table();
            table_person_DG.ItemsSource = db.create_table("Читатели").DefaultView;
        }

        private void not_save_changes_Click(object sender, RoutedEventArgs e)
        {
            table_person_DG.ItemsSource = db.create_table("Читатели").DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            table_person_DG.ItemsSource = db.filter_person_table(kod_tb.Text, name_tb.Text, address_tb.Text, tel_tb.Text).DefaultView;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            kod_tb.Text = "";
            name_tb.Text = "";
            tel_tb.Text = "";
            address_tb.Text = "";
        }
    }
}
