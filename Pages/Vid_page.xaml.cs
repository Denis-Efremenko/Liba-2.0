using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для Vid_page.xaml
    /// </summary>
    public partial class Vid_page : Page
    {

        DB db = new DB();
        WD wd = new WD();
        public int worker_kod;


        public Vid_page(int k)
        {
            InitializeComponent();
            table_vid_book_DG.ItemsSource = db.create_table("[Выдача книг]").DefaultView;
            worker_kod = k;
        }


        private void all_data_button_Click(object sender, RoutedEventArgs e)
        {
            table_vid_book_DG.ItemsSource = db.create_table("[Выдача книг]").DefaultView;
        }


        private void table_vid_book_DG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime)) 
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
        }


        private void person_sanc_button_Click(object sender, RoutedEventArgs e)
        {
            int reader_kod;
            if (name_tb.Text == "")
                reader_kod = 0;
            else
                reader_kod = db.check_data_in_db(name_tb.Text, "Читатели", "ФИО", "Код читателя");

            if (reader_kod != 0)
            {
                name_tb.Text = "";
                wd.sanction_report(worker_kod, reader_kod);
            }
            else
            {
                MessageBox.Show("Введено некорректное значения поля ФИО/Код читателя.", "Внимание!");
            }
        }


        private void give_button_Click(object sender, RoutedEventArgs e)
        {
            int book_kod = db.check_data_in_db(book_tb.Text, "Книги", "Название", "Код книги");
            int reader_kod = db.check_data_in_db(name_tb.Text, "Читатели", "ФИО", "Код читателя");
            if(book_kod == 0)
                MessageBox.Show("Введено некорректное значения поля Название/Код книги.", "Внимание!");
            else if(reader_kod == 0)
                MessageBox.Show("Введено некорректное значения поля ФИО/Код читателя.", "Внимание!");
            else
            {
                db.open_connection();

                string cmd = "SELECT MAX([Код выдачи]) FROM [Выдача книг]";
                SQLiteCommand command = new SQLiteCommand(cmd, db.conn);
                int kod = Convert.ToInt32(command.ExecuteScalar()) + 1;

                cmd = "INSERT INTO [Выдача книг] ([Код выдачи], [Книга], [Читатель], [Дата выдачи], [Ожидаемая дата возврата], [Выдавший сотрудник]) VALUES (@kod, @book, @reader, @date_now, @expected_date, @worker)";
                command.CommandText = cmd;
                command.Parameters.Add(new SQLiteParameter("@kod", kod));
                command.Parameters.Add(new SQLiteParameter("@book", book_kod));
                command.Parameters.Add(new SQLiteParameter("@reader", reader_kod));
                command.Parameters.Add(new SQLiteParameter("@date_now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                command.Parameters.Add(new SQLiteParameter("@expected_date", DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss")));
                command.Parameters.Add(new SQLiteParameter("@worker", worker_kod));
                command.ExecuteNonQuery();
                name_tb.Text = "";
                book_tb.Text = "";
                table_vid_book_DG.ItemsSource = db.create_table("[Выдача книг]").DefaultView;
                db.close_connection();
            }
        }

        private void return_book_Click(object sender, RoutedEventArgs e)
        {
            int book_kod = db.check_data_in_db(book_tb.Text, "Книги", "Название", "Код книги");
            int reader_kod = db.check_data_in_db(name_tb.Text, "Читатели", "ФИО", "Код читателя");
            int sanc_kod = db.check_data_in_db(sanc_tb.Text, "Система штрафов", "Описание повреждения", "Код штрафа");

            if (book_kod == 0)
                MessageBox.Show("Введено некорректное значения поля Название/Код книги.", "Внимание!");
            else if (reader_kod == 0)
                MessageBox.Show("Введено некорректное значения поля ФИО/Код читателя.", "Внимание!");
            else if(sanc_tb.Text != "" && sanc_kod == 0)
                MessageBox.Show("Введено некорректное значения поля Название/Код штрфа.", "Внимание!");
            else
            {
                db.open_connection();

                string cmd = "UPDATE [Выдача книг] " +
                    "SET [Фактическая дата возврата] = @date, [Код штрафа] = @sunc, [Принявший сотрудник] = @worker " +
                    "WHERE [Фактическая дата возврата] IS NULL AND [Книга] = @book AND [Читатель] = @reader";
                SQLiteCommand command = new SQLiteCommand(cmd, db.conn);

                command.Parameters.Add(new SQLiteParameter("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                command.Parameters.Add(new SQLiteParameter("@sunc", sanc_kod));
                command.Parameters.Add(new SQLiteParameter("@worker", worker_kod));
                command.Parameters.Add(new SQLiteParameter("@book", book_kod));
                command.Parameters.Add(new SQLiteParameter("@reader", reader_kod));

                if(command.ExecuteNonQuery() != 0)
                {
                    book_tb.Text = "";
                    name_tb.Text = "";
                    sanc_tb.Text = "";
                    table_vid_book_DG.ItemsSource = db.create_table("[Выдача книг]").DefaultView;
                    wd.sanction_akt(worker_kod, reader_kod, book_kod, sanc_kod);
                }
                else
                {
                    MessageBox.Show("Подходящяя книга не найдена.", "Внимание!");
                }
                db.close_connection();
                
            }
        }

        private void not_return_book_button_Click(object sender, RoutedEventArgs e)
        {
            table_vid_book_DG.ItemsSource = db.filter_vid_table().DefaultView;
        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {
            book_tb.Text = "";
            name_tb.Text = "";
            sanc_tb.Text = "";
        }
    }
}
