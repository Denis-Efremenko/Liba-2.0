using System;
using System.Data.SQLite;
using System.Windows;
using System.Data;

namespace Biblioteka
{
    class DB
    {
        public SQLiteConnection conn;
        public SQLiteDataAdapter adapter;
        SQLiteCommandBuilder cb;
        DataTable dt;

        public DB()
        {
            conn = new SQLiteConnection("Data Source = bib3.db");
            adapter = new SQLiteDataAdapter();
            cb = new SQLiteCommandBuilder(adapter);
        }
        
        public void open_connection()
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                }
                catch
                {
                    MessageBox.Show("Ошибка подключения к базе данных!", "SUPER FATAL ERROR");
                }
            }
        }

        public DataTable filter_person_table(string kod, string name, string adr, string tel)
        {
            open_connection();
            string cmd = "SELECT * FROM Читатели WHERE(" +
                "[Код читателя] LIKE '%" + kod + "%'   AND " +
                "[ФИО] LIKE '%" + name + "%'   AND " +
                "[Адрес] LIKE '%" + adr + "%'   AND " +
                "[Телефон] LIKE '%" + tel + "%'  )";
            SQLiteCommand command = new SQLiteCommand(cmd, conn);
            close_connection();
            adapter.SelectCommand = command;
            dt = new DataTable("Читатели");
            adapter.Fill(dt);
            return dt;
        }

        public DataTable filter_sanc_table(string kod, string option, string sum)
        {
            open_connection();
            string cmd = "SELECT * FROM [Система штрафов] WHERE("+
                "[Код штрафа] LIKE '%" + kod + "%'   AND " +
                "[Описание повреждения] LIKE '%" + option + "%'   AND " +
                "[Штрафная сумма] LIKE '%" + sum + "%'  )";
            SQLiteCommand command = new SQLiteCommand(cmd, conn);
            close_connection();
            adapter.SelectCommand = command;
            dt = new DataTable("Система штрафов");
            adapter.Fill(dt);
            return dt;
        }

        public void close_connection()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        public DataTable create_table(string table_name)
        {

            open_connection();
            string cmd = "SELECT * FROM " + table_name;
            SQLiteCommand command = new SQLiteCommand(cmd, conn);
            close_connection();

            adapter.SelectCommand = command;
            dt = new DataTable(table_name);
            adapter.Fill(dt);
            return dt;
        }
        
        public void update_table()
        {
            adapter.UpdateCommand = cb.GetUpdateCommand();
            adapter.InsertCommand = cb.GetInsertCommand();
            adapter.DeleteCommand = cb.GetDeleteCommand();
            adapter.Update(dt);
        }
    
        public DataTable filter_book_table(string kod, string name, string author, string genre, string publish, string min, string max)
        {
            if (min == "")
                min = "0";
            if (max == "")
                max = "100";
            open_connection();
            string cmd = "SELECT * FROM Книги WHERE(" +
                "[Код книги] LIKE '%" + kod + "%' AND " +
                "[Название] LIKE '%" + name + "%'   AND " +
                "[Автор] LIKE '%" + author + "%'   AND " +
                "[Жанр] LIKE '%" + genre + "%'   AND " +
                "[Издательство] LIKE '%" + publish + "%'   AND " +
                "STRFTIME('%Y', 'NOW') - STRFTIME('%Y', [Дата издания]) >= " + Convert.ToInt32(min) + " AND " +
                "STRFTIME('%Y', 'NOW') - STRFTIME('%Y', [Дата издания]) <= " + Convert.ToInt32(max) + ")";
            SQLiteCommand command = new SQLiteCommand(cmd, conn);
            close_connection();
            adapter.SelectCommand = command;
            dt = new DataTable("Книги");
            adapter.Fill(dt);
            return dt;
        }

        public DataTable filter_vid_table()
        {
            open_connection();
            string cmd = "SELECT * FROM [Выдача книг] WHERE [Фактическая дата возврата] IS NULL";
            SQLiteCommand command = new SQLiteCommand(cmd, conn);
            close_connection();
            adapter.SelectCommand = command;
            dt = new DataTable("Выдача книг");
            adapter.Fill(dt);
            return dt;
        }

        public int check_data_in_db(string data, string table, string string_column, string kod_column)
        {
            
            open_connection();
            int kod;
            try
            {
                kod = Convert.ToInt32(data);

                string cmd = "SELECT [" + string_column + "] FROM [" + table + "] WHERE [" + kod_column + "] = " + kod;
                SQLiteCommand command = new SQLiteCommand(cmd, conn);
                object obj = command.ExecuteScalar();
                return (obj == null) ? 0 : kod;
            }
            catch
            {
                string cmd = "SELECT [" + kod_column + "] FROM [" + table + "] WHERE [" + string_column + "] = '" + data + "'";
                SQLiteCommand command = new SQLiteCommand(cmd, conn);
                object obj = command.ExecuteScalar();

                return (obj == null) ? 0 : Convert.ToInt32(obj);
            }
            finally
            {
                close_connection();
            }
        }
    }
}
