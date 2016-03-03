using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace BookStore
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        cSQL sql = new cSQL();

        void dataGridWidth(int w1, int w2, int w3, int w4, int w5)
        {
            dataGridView1.Columns[0].Width = w1;
            dataGridView1.Columns[1].Width = w2;
            if (w3 > 0) dataGridView1.Columns[2].Width = w3;
            if (w4 > 0) dataGridView1.Columns[3].Width = w4;
            if (w5 > 0) dataGridView1.Columns[4].Width = w5;
        }

        private void Form3_Load(object sender, EventArgs e)
        {           
            sql.Connect();
            string queryString = "EXEC SelectAllBooks";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[0].Width = 150; dataGridView1.Columns[1].Width = 270; dataGridView1.Columns[2].Width = 150; dataGridView1.Columns[3].Width = 90;
            
            queryString = "SELECT Genre FROM Genre";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox1.Items.Add(Convert.ToString(reader[0]));
            reader.Close();

            queryString = "SELECT Name FROM Author";
            reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox2.Items.Add(Convert.ToString(reader[0]));
            reader.Close();

            queryString = "SELECT COUNT(*), SUM(o.BookQty*c.Price), SUM(o.BookQty)FROM Orders o JOIN CurrentSale c ON o.BookID = c.BookID WHERE o.SaleDate IS NULL";
            reader = sql.ReaderQuery(queryString);
            reader.Read();
            label1.Text += reader[0].ToString();
            string price = reader[1].ToString();
            int p = price.IndexOf(',');
            label2.Text += price.Substring(0, p) + " р. " + price.Substring(p + 1, 2) + " к.";
            label3.Text += reader[2].ToString();
            reader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string queryString = null;
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    queryString =  "SELECT TOP(10) b.ISBN, b.Title AS 'Название', a.Name AS 'Автор', g.Genre AS 'Жанр', (c.DeliveryQty-cs.StoreQty) AS 'Продано' " +
                               "FROM Book b, Genre g, CurrentSale cs, Author a, Consignment c " +
                               "WHERE b.BookID = c.BookID AND cs.BookID = b.BookID AND c.BookID = b.BookID " +
                               "AND a.AuthorID = " +
                               "(SELECT TOP(1) aa.AuthorID FROM Author aa " +
                               "INNER JOIN AuthorBook ab ON ab.AuthorID = aa.AuthorID AND ab.BookID = b.BookID) " +
                               "AND g.GenreID = " +
                               "(SELECT TOP(1) gg.GenreID FROM Genre gg " +
                               "INNER JOIN GenreBook gb ON gb.GenreID = gg.GenreID AND gb.BookID = b.BookID) " +
                               "ORDER BY 'Продано' DESC";
                    break;
                case 1:
                    queryString = "SELECT TOP(10) b.ISBN, b.Title AS 'Название', a.Name AS 'Автор', g.Genre AS 'Жанр', (c.DeliveryQty-cs.StoreQty) AS 'Продано' " +
                               "FROM Book b, Genre g, CurrentSale cs, Author a, Consignment c " +
                               "WHERE b.BookID = c.BookID AND cs.BookID = b.BookID AND c.BookID = b.BookID " +
                               "AND a.AuthorID = " +
                               "(SELECT TOP(1) aa.AuthorID FROM Author aa " +
                               "INNER JOIN AuthorBook ab ON ab.AuthorID = aa.AuthorID AND ab.BookID = b.BookID) " +
                               "AND g.GenreID = " +
                               "(SELECT TOP(1) gg.GenreID FROM Genre gg " +
                               "INNER JOIN GenreBook gb ON gb.GenreID = gg.GenreID AND gb.BookID = b.BookID) " +
                               "ORDER BY 'Продано'";
                    break;
                case 2:
                    queryString = "SELECT b.ISBN, b.Title AS 'Название', a.Name AS 'Автор', (c.DeliveryQty*cs.Price - c.DeliveryPrice - c.DeliveryQty*c.UnitPrice) AS 'Прибыль' " +
                               "FROM Book b, CurrentSale cs, Author a, Consignment c " +
                               "WHERE b.BookID = c.BookID AND cs.BookID = b.BookID AND c.BookID = b.BookID " +
                               "AND a.AuthorID = " +
                               "(SELECT TOP(1) aa.AuthorID FROM Author aa " +
                               "INNER JOIN AuthorBook ab ON ab.AuthorID = aa.AuthorID AND ab.BookID = b.BookID) ";
                    break;
                case 3:
                    queryString = "SELECT COUNT(*) AS 'Тиражей на продаже', SUM(c.DeliveryQty*cs.Price - c.DeliveryPrice - c.DeliveryQty*c.UnitPrice) AS 'Суммарная прибыль'" +
                       " FROM CurrentSale cs JOIN Consignment c ON cs.BookID = c.BookID";
                    break;
                case 4:
                    queryString = "SELECT TOP(10) b.ISBN, b.Title AS 'Название', a.Name AS 'Автор', g.Genre AS 'Жанр', (c.DeliveryQty*cs.Price - c.DeliveryPrice - c.DeliveryQty*c.UnitPrice) AS 'Прибыль' " +
                               "FROM Book b, Genre g, CurrentSale cs, Author a, Consignment c " +
                               "WHERE b.BookID = c.BookID AND cs.BookID = b.BookID AND c.BookID = b.BookID " +
                               "AND a.AuthorID = " +
                               "(SELECT TOP(1) aa.AuthorID FROM Author aa " +
                               "INNER JOIN AuthorBook ab ON ab.AuthorID = aa.AuthorID AND ab.BookID = b.BookID) " +
                               "AND g.GenreID = " +
                               "(SELECT TOP(1) gg.GenreID FROM Genre gg " +
                               "INNER JOIN GenreBook gb ON gb.GenreID = gg.GenreID AND gb.BookID = b.BookID) " +
                               " ORDER BY 'Прибыль' DESC";
                    break;
                case 5:
                    queryString = "SELECT TOP(10) b.ISBN, b.Title AS 'Название', a.Name AS 'Автор', g.Genre AS 'Жанр', (c.DeliveryQty*cs.Price - c.DeliveryPrice - c.DeliveryQty*c.UnitPrice) AS 'Прибыль' " +
                              "FROM Book b, Genre g, CurrentSale cs, Author a, Consignment c " +
                              "WHERE b.BookID = c.BookID AND cs.BookID = b.BookID AND c.BookID = b.BookID " +
                              "AND a.AuthorID = " +
                              "(SELECT TOP(1) aa.AuthorID FROM Author aa " +
                              "INNER JOIN AuthorBook ab ON ab.AuthorID = aa.AuthorID AND ab.BookID = b.BookID) " +
                              "AND g.GenreID = " +
                              "(SELECT TOP(1) gg.GenreID FROM Genre gg " +
                              "INNER JOIN GenreBook gb ON gb.GenreID = gg.GenreID AND gb.BookID = b.BookID) " +
                              " ORDER BY 'Прибыль'";
                    break;
                case 6:
                    queryString = "SELECT MONTH(c.DeliveryDate) AS 'Месяц', SUM(c.DeliveryQty-cs.StoreQty) AS 'Продано'" +
                       " FROM Consignment c JOIN CurrentSale cs ON c.BookID = cs.BookID" +
                       " GROUP BY MONTH(c.DeliveryDate)";
                    break;
                default: break;
            }
                        
            bindingSource1.DataSource = null;
            bindingSource1.DataSource = sql.TableQuery(queryString);
            switch (comboBox3.SelectedIndex)
            {
                case 0: case 1: case 4: case 5: dataGridWidth(100, 240, 120, 130, 70); break;                
                case 2: dataGridWidth(120, 300, 140, 100, 0); break;
                case 3: case 6: dataGridWidth(330, 330, 0, 0, 0); break;
                default: break;
            }
          /*  if (comboBox3.SelectedIndex == 6)
            {      
                string[] months = {"Январь","Февраль","Март","Апрель","Май","Июнь","Июль","Август","Сентябрь","Октябрь","Ноябрь","Декабрь"}; /* Бакулин Антон Викторович (с) */
              //  for (int i = 0; i < 12; i++) dataGridView1.Rows[i].Cells[0].Value = months[i];
              //  dataGridView1.Rows[0].Cells[0].Value = "Январь";
            }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Publishing dlg = new Publishing();
            dlg.ShowDialog();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            label4.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox3.SelectedIndex = -1;
            Regex price_rgx = new Regex(@"^[0-9]+[\,\.]{0,1}[0-9]*$");
            if ((textBox2.Text.Length > 0 && !price_rgx.IsMatch(textBox2.Text)) || (textBox3.Text.Length > 0 && !price_rgx.IsMatch(textBox3.Text))) { MessageBox.Show("Неверное значение цены", "Предупреждение", MessageBoxButtons.OK); return; }

            string min, max, queryString;

            if (textBox2.Text.Length > 0) min = textBox2.Text; else min = "-1";
            if (textBox3.Text.Length > 0) max = textBox3.Text; else max = "1000000";

            if (comboBox1.SelectedIndex > 0 && comboBox2.SelectedIndex > 0)
                queryString = "EXEC BookSearchAllParametres @Title = N'%" + textBox1.Text + "%', @Author = N'" + comboBox2.SelectedItem.ToString() + "', @Genre = N'" + comboBox1.SelectedItem.ToString() + "', @min = '" + min + "', @max = '" + max + "';";
            else if (comboBox1.SelectedIndex > 0)
                queryString = "EXEC BookSearchWithGenre @Title = N'%" + textBox1.Text + "%', @Genre = N'" + comboBox1.SelectedItem.ToString() + "', @min = '" + min + "', @max = '" + max + "';";
            else if (comboBox2.SelectedIndex > 0)
                queryString = "EXEC BookSearchWithAuthor @Title = N'%" + textBox1.Text + "%', @Author = N'" + comboBox2.SelectedItem.ToString() + "', @min = '" + min + "', @max = '" + max + "';";
            else queryString = "EXEC BookSearchSimple @Title = N'%" + textBox1.Text + "%', @min = '" + min + "', @max = '" + max + "';";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridWidth(150, 270, 150, 90, 0);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Users dlg = new Users();
            dlg.ShowDialog();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int BookID;

            string queryString = "EXEC SelectBookID N'" + dataGridView1.CurrentRow.Cells[1].Value.ToString() + "', N'" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            BookID = Convert.ToInt32(reader[0]);
            reader.Close();

            Book dlg = new Book(BookID, 1, 0);
            dlg.ShowDialog();

            queryString = "EXEC SelectAllBooks";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OrdersSubmit dlg = new OrdersSubmit();
            dlg.ShowDialog();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            BookNew dlg = new BookNew();
            dlg.ShowDialog();

            string queryString = "EXEC SelectAllBooks";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;
            dataGridWidth(150, 270, 150, 90, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Authors dlg = new Authors();
            dlg.ShowDialog();
        }
    }
}

///добавление строки
/*bindingSource1.AddNew();
DataRowView drw = bindingSource1.Current as DataRowView;
drw["Автор"] = "11111";
drw["Название"] = "22dasd";
bindingSource1.EndEdit();*/

///загрузка картинки
/*  string Img = "X:\\img\\2.jpg";
  sql.Connect();
  sql.ImageUpload(Img, 2);
  sql.Disconnect();*/