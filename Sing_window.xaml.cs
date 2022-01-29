using System;
using System.Windows;
using System.Data.SQLite;

namespace Biblioteka
{
    /// <summary>
    /// Логика взаимодействия для Sing_window.xaml
    /// </summary>
    public partial class Sing_window : Window
    {
        DB db = new DB();
        public Sing_window()
        {
            InitializeComponent();
        }

        private void sing_in_button_Click(object sender, RoutedEventArgs e)
        {
            db.open_connection();
            string query = "SELECT [Код сотрудника] FROM [Сотрудники] WHERE [ФИО] = '" + name_textBox.Text + "' AND" +
                "[Пароль] = '" + pass_textBox.Password + "'";
            SQLiteCommand command = new SQLiteCommand(query, db.conn);
            object obj = command.ExecuteScalar();
            int kod;
            kod = (obj == null) ? 0 : Convert.ToInt32(obj);
            db.close_connection();

            if (kod != 0)
            {
                this.Hide();
                Worker_window f2 = new Worker_window(kod);
                f2.Owner = this;
                f2.ShowDialog();
            }
            else
            {
                MessageBox.Show("Введено не верное ФИО или пароль", "Внимание!");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            db.close_connection();
            Application.Current.Shutdown();
            
        }

        private void sergey_button_Click(object sender, RoutedEventArgs e)
        {
            name_textBox.Text = "Оленин Сергей Игоревич";
            pass_textBox.Password = "3463463";
        }
    }
}