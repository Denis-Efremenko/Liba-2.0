using System;
using System.Data.SQLite;
using System.Windows;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для Worker_window.xaml
    /// </summary>
    public partial class Worker_window : Window
    {
        public static int kod;
        DB db = new DB();

        public Worker_window(int k)
        {

            InitializeComponent();

            kod = k;
            db.open_connection();
            string sql = "SELECT [ФИО] FROM [Сотрудники] WHERE [Код сотрудника] = " + kod + "";
            SQLiteCommand command = new SQLiteCommand(sql, db.conn);
            worker_name_label.Content = command.ExecuteScalar().ToString();
            db.close_connection();

            main_frame.Content = new Book_page();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            db.close_connection();
            Application.Current.Shutdown();
        }

        private void add_book_button_Click(object sender, RoutedEventArgs e)
        {
            main_frame.Content = new Book_page();
        }

        private void add_person_button_Click(object sender, RoutedEventArgs e)
        {
            main_frame.Content = new Person_page();
        }

        private void vid_book_button_Click(object sender, RoutedEventArgs e)
        {
            main_frame.Content = new Vid_page(kod);
        }

        private void sanc_button_Click(object sender, RoutedEventArgs e)
        {
            main_frame.Content = new Sanc_page();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Sing_window f1 = new Sing_window();
            f1.Owner = this;
            f1.ShowDialog();
        }
    }
}
