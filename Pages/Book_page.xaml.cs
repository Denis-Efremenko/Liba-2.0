using System.Windows;
using System.Windows.Controls;


namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для Add_book_page.xaml
    /// </summary>
    public partial class Book_page : Page
    {
        DB db = new DB();

        public Book_page()
        {
            InitializeComponent();
            
            table_book_DG.ItemsSource = db.create_table("Книги").DefaultView;
        }

        private void table_book_DG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
        }

        private void save_changes_Click(object sender, RoutedEventArgs e)
        {
            db.update_table();
            table_book_DG.ItemsSource = db.create_table("Книги").DefaultView;
        }

        private void not_save_changes_Click(object sender, RoutedEventArgs e)
        {
            table_book_DG.ItemsSource = db.create_table("Книги").DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            table_book_DG.ItemsSource = db.filter_book_table(kod_tb.Text, name_tb.Text, author_tb.Text, genre_tb.Text, publish_tb.Text, age_min.Text, age_max.Text).DefaultView;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            kod_tb.Text = "";
            name_tb.Text = "";
            author_tb.Text = "";
            genre_tb.Text = "";
            publish_tb.Text = "";
            age_min.Text = "";
            age_max.Text = "";
        }
    }
}
