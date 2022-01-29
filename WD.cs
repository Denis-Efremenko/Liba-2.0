using System;
using Word = Microsoft.Office.Interop.Word;
using System.Data.SQLite;
using System.Data;

namespace Biblioteka
{
    class WD
    {
        public DB db;
        public WD()
        {
            db = new DB();

        }

        public void sanction_akt(int worker_kod, int reader_kod, int book_kod, int sanc_kod)
        {
            if (sanc_kod == 0)
                return;
            string worker;
            string reader;
            string book;
            string author;
            string sanc;
            string sum_sanc;

            Word._Application wordApplication = new Word.Application();
            Word._Document wordDocument = null;
            wordApplication.Visible = true;

            var templatePathObj = AppDomain.CurrentDomain.BaseDirectory + @"Templates\bsanc.docx";

            try
            {
                wordDocument = wordApplication.Documents.Add(templatePathObj);
            }
            catch (Exception exception)
            {
                if (wordDocument != null)
                {
                    wordDocument.Close(false);
                    wordDocument = null;
                }
                wordApplication.Quit();
                wordApplication = null;
                throw;
            }

            db.open_connection();
            string cmd = "SELECT [ФИО] FROM [Сотрудники] WHERE [Код сотрудника] = " + worker_kod;
            SQLiteCommand command = new SQLiteCommand(cmd, db.conn);
            worker = command.ExecuteScalar().ToString();

            cmd = "SELECT [ФИО] FROM [Читатели] WHERE [Код читателя] = " + reader_kod;
            command.CommandText = cmd;
            reader = command.ExecuteScalar().ToString();

            cmd = "SELECT [Название] FROM [Книги] WHERE [Код книги] = " + book_kod;
            command.CommandText = cmd;
            book = command.ExecuteScalar().ToString();

            cmd = "SELECT [Автор] FROM [Книги] WHERE [Код книги] = " + book_kod;
            command.CommandText = cmd;
            author = command.ExecuteScalar().ToString();

            cmd = "SELECT [Описание повреждения] FROM [Система штрафов] WHERE [Код штрафа] = " + sanc_kod;
            command.CommandText = cmd;
            sanc = command.ExecuteScalar().ToString();

            cmd = "SELECT [Штрафная сумма] FROM [Система штрафов] WHERE [Код штрафа] = " + sanc_kod;
            command.CommandText = cmd;
            sum_sanc = command.ExecuteScalar().ToString();

            db.close_connection();

            Word.Range wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "book", ReplaceWith: book);
            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "author", ReplaceWith: author);
            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "reader", ReplaceWith: reader);
            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "type_sanc", ReplaceWith: sanc);
            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "sum_sanc", ReplaceWith: sum_sanc);
            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "reader", ReplaceWith: reader);
            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "worker", ReplaceWith: worker);
            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "date", ReplaceWith: DateTime.Now.ToString("d"));
        }

        public void sanction_report(int worker_kod, int reader_kod)
        {

            string reader_name;
            string worker_name;
            
            DataTable dataTable = new DataTable();
            DataTable dataBook = new DataTable();
            DataTable dataSanc = new DataTable();
            DataTable dataVid = new DataTable();

            Word._Application wordApplication = new Word.Application();
            Word._Document wordDocument = null;
            wordApplication.Visible = true;

            var templatePathObj = AppDomain.CurrentDomain.BaseDirectory + @"Templates\vsanc.docx";

            try
            {
                wordDocument = wordApplication.Documents.Add(templatePathObj);
            }
            catch (Exception exception)
            {
                if (wordDocument != null)
                {
                    wordDocument.Close(false);
                    wordDocument = null;
                }
                wordApplication.Quit();
                wordApplication = null;
                throw;
            }


            db.open_connection();

            string cmd = "SELECT [ФИО] FROM [Читатели] WHERE [Код читателя] = '" + reader_kod + "'";
            SQLiteCommand command = new SQLiteCommand(cmd, db.conn);
            reader_name = command.ExecuteScalar().ToString();

            cmd = "SELECT [ФИО] FROM [Сотрудники] WHERE [Код сотрудника] = '" + worker_kod + "'";
            command = new SQLiteCommand(cmd, db.conn);
            worker_name = command.ExecuteScalar().ToString();

            cmd = "SELECT [Книга], [Фактическая дата возврата], [Код штрафа] FROM [Выдача книг] WHERE [Читатель] = " + reader_kod + " AND [Код штрафа] > 0 ";
            command = new SQLiteCommand(cmd, db.conn);
            db.adapter.SelectCommand = command;
            db.adapter.Fill(dataVid);


            Word.Range wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "reader", ReplaceWith: reader_name);

            if (dataVid.Rows.Count >= 1)
            {
                dataBook.Columns.Add();
                for (int i = 0; i < dataVid.Rows.Count; i++)
                {
                    cmd = "SELECT [Название] FROM [Книги] WHERE [Код книги] = " + Convert.ToInt32(dataVid.Rows[i][0]);
                    command = new SQLiteCommand(cmd, db.conn);
                    dataBook.Rows.Add(Convert.ToString(command.ExecuteScalar()));
                }

                dataSanc.Columns.Add();
                dataSanc.Columns.Add();
                for (int i = 0; i < dataVid.Rows.Count; i++)
                {
                    cmd = "SELECT [Описание повреждения] FROM [Система штрафов] WHERE [Код штрафа] = " + Convert.ToInt32(dataVid.Rows[i][2]);
                    command = new SQLiteCommand(cmd, db.conn);
                    dataSanc.Rows.Add(Convert.ToString(command.ExecuteScalar()));
                    cmd = "SELECT [Штрафная сумма] FROM [Система штрафов] WHERE [Код штрафа] = " + Convert.ToInt32(dataVid.Rows[i][2]);
                    command = new SQLiteCommand(cmd, db.conn);
                    dataSanc.Rows[i][1] = (Convert.ToString(command.ExecuteScalar()));
                }

                dataTable.Columns.Add("Дата возврата");
                dataTable.Columns.Add("Книга");
                dataTable.Columns.Add("Штраф");
                dataTable.Columns.Add("Штрафная сумма");

                for (int i = 0; i < dataBook.Rows.Count; i++)
                {
                    dataTable.Rows.Add(DateTime.Parse(dataVid.Rows[i][1].ToString()).ToString("d"));
                }

                for (int i = 0; i < dataVid.Rows.Count; i++)
                {
                    dataTable.Rows[i][1] = dataBook.Rows[i][0];
                }

                for (int i = 0; i < dataSanc.Rows.Count; i++)
                {
                    dataTable.Rows[i][2] = dataSanc.Rows[i][0];
                }
                for (int i = 0; i < dataSanc.Rows.Count; i++)
                {
                    dataTable.Rows[i][3] = dataSanc.Rows[i][1];
                }

                wordApplication.Selection.Find.Execute("table");
                wordRange = wordApplication.Selection.Range;

                var wordTable = wordDocument.Tables.Add(wordRange,
                    dataTable.Rows.Count + 1, dataTable.Columns.Count);
                wordTable.set_Style("Сетка таблицы");
                wordTable.ApplyStyleHeadingRows = true;
                wordTable.ApplyStyleLastRow = false;
                wordTable.ApplyStyleFirstColumn = true;
                wordTable.ApplyStyleLastColumn = false;
                wordTable.ApplyStyleRowBands = true;
                wordTable.ApplyStyleColumnBands = false;

                wordTable.Cell(1, 1).Range.Text = "Дата";
                wordTable.Cell(1, 2).Range.Text = "Книга";
                wordTable.Cell(1, 3).Range.Text = "Штраф";
                wordTable.Cell(1, 4).Range.Text = "Штрафная сумма";
                for (var j = 0; j < dataTable.Rows.Count; j++)
                {
                    for (var k = 0; k < dataTable.Columns.Count; k++)
                    {
                        wordTable.Cell(j + 2, k + 1).Range.Text = dataTable.Rows[j][k].ToString();
                    }
                }
            }
            else
            {
                wordRange = wordDocument.Content;
                wordRange.Find.Execute(FindText: "table", ReplaceWith: "– Штрафов не зафиксировано.");
            }
            db.close_connection();

            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "worker", ReplaceWith: worker_name);
            wordRange = wordDocument.Content;
            wordRange.Find.Execute(FindText: "date", ReplaceWith: DateTime.Now.ToString("d"));
        }
    }

   
}
