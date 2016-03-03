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
    public partial class BookNew : Form
    {
        public BookNew()
        {
            InitializeComponent();
        }
        cSQL sql = new cSQL();
        string[] authors = new string[50];
        int authors_n = 0;
        string[] genres = new string[10];
        int genres_n = 0;

        private void BookNew_Load(object sender, EventArgs e)
        {
            sql.Connect();

            string queryString = "SELECT Name FROM Author";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox1.Items.Add(Convert.ToString(reader[0]));
            reader.Close();

            queryString = "SELECT Genre FROM Genre";
            reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox2.Items.Add(Convert.ToString(reader[0]));
            reader.Close();

            queryString = "SELECT Name FROM Publishing";
            reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox3.Items.Add(Convert.ToString(reader[0]));
            reader.Close();
        }

        private void BookNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            monthCalendar1.Visible = true;
            monthCalendar1.BringToFront();
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            textBox1.Text = monthCalendar1.SelectionStart.Year + "-" + monthCalendar1.SelectionStart.Month + "-" + monthCalendar1.SelectionStart.Day;
            monthCalendar1.Visible = false;           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(openFileDialog1.FileName);
                if (ext != ".jpg" && ext != ".jpeg") { MessageBox.Show("Изображение должно быть формата \"*.jpg\" или \"*.jpeg\"", "Предупреждение", MessageBoxButtons.OK); return; }
                textBox9.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0 || textBox3.Text.Length == 0 || textBox4.Text.Length == 0 || textBox5.Text.Length == 0 || textBox6.Text.Length == 0 || textBox12.Text.Length == 0 || textBox7.Text.Length == 0 || comboBox3.SelectedIndex == -1) { MessageBox.Show("Все поля обязательны для заполнения", "Предупреждение", MessageBoxButtons.OK); return; }
            Regex price_rgx = new Regex(@"^[0-9]+[\.]{0,1}[0-9]*$");
            Regex year_rgx = new Regex(@"^\d{4}$");
            if (!year_rgx.IsMatch(textBox12.Text) || !price_rgx.IsMatch(textBox2.Text) || !price_rgx.IsMatch(textBox3.Text) || !price_rgx.IsMatch(textBox7.Text)) { MessageBox.Show("Неверные значения полей", "Предупреждение", MessageBoxButtons.OK); return; }

            string queryString = "SELECT PublishID FROM Publishing WHERE Name = '" + comboBox3.SelectedItem.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            if (!reader.HasRows) { MessageBox.Show("Заданного издательства нет в базе данных", "Предупреждение", MessageBoxButtons.OK); return; }
            int PublishID = Convert.ToInt32(reader[0]);
            reader.Close();

            queryString = "BEGIN TRANSACTION; INSERT INTO Book(ISBN, Title, About, Year) VALUES ('" + textBox6.Text + "','" + textBox5.Text + "','" + textBox8.Text + "','" + textBox12.Text + "-01-01');";
            try
            {
                sql.Query(queryString);
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка при заполнении таблицы \"Book\"\n" + exp.Message);
                return;
            }

            queryString = "SELECT MAX(BookID) FROM Book";
            int BookID = sql.IntQuery(queryString);

            if (textBox9.Text.Length > 0)
            {
                try
                {
                    sql.ImageUpload(textBox9.Text, BookID);
                }
                catch (Exception exp)
                {
                    System.Windows.Forms.MessageBox.Show("Ошибка при сохранении изображения \"Book\"\n" + exp.Message);
                    return;
                }
            }

            queryString = "INSERT INTO CurrentSale(BookID, StoreQty, Price, Discount) VALUES ('" + BookID.ToString() + "','" + textBox4.Text + "','" + textBox7.Text + "','" + comboBox4.Text + "');";
            try
            {
                sql.Query(queryString);
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка при заполнении таблицы \"CurrentSale\"\n" + exp.Message);
                return;
            }

            queryString = "INSERT INTO Consignment (PublishingID, BookID, DeliveryDate, DeliveryPrice, UnitPrice, DeliveryQty) VALUES ('" + PublishID.ToString() + "','" + BookID.ToString() + "','" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "');";
            try
            {
                sql.Query(queryString);
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка при заполнении таблицы \"Consignment\"\n" + exp.Message);
                return;
            }

            string AuthorID;
            for (int i = 0; i < authors_n; i++)
            {
                queryString = "SELECT AuthorID FROM Author WHERE Name = '" + authors[i] + "'";
                reader = sql.ReaderQuery(queryString);
                reader.Read();
                AuthorID = reader[0].ToString();
                reader.Close();
                queryString = "INSERT INTO AuthorBook (BookID, AuthorID) VALUES ('" + BookID.ToString() + "','" + AuthorID + "');";
                try
                {
                    sql.Query(queryString);
                }
                catch (Exception exp)
                {
                    System.Windows.Forms.MessageBox.Show("Ошибка при заполнении таблицы \"AuthorBook\"\n" + exp.Message);
                    return;
                }
            }

            string GenreID;
            for (int i = 0; i < genres_n; i++)
            {
                queryString = "SELECT GenreID FROM Genre WHERE Genre = '" + genres[i] + "'";
                reader = sql.ReaderQuery(queryString);
                reader.Read();
                GenreID = reader[0].ToString();
                reader.Close();
                queryString = "INSERT INTO GenreBook (BookID, GenreID) VALUES ('" + BookID.ToString() + "','" + GenreID + "');";
                try
                {
                    sql.Query(queryString);
                }
                catch (Exception exp)
                {
                    System.Windows.Forms.MessageBox.Show("Ошибка при заполнении таблицы \"GenreBook\"\n" + exp.Message);
                    return;
                }
            }
            queryString = "COMMIT TRANSACTION;";
            sql.Query(queryString);

            System.Windows.Forms.MessageBox.Show("Данные успешно сохранены");
            this.Close();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                label14.Text = "Цена ед:   " + Convert.ToString(Convert.ToDouble(textBox7.Text) * (1 - Convert.ToDouble(comboBox4.SelectedItem) / 100));
            }
            catch (Exception)
            {
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0) return;
            for (int i = 0; i < authors_n; i++) if (authors[i] == comboBox1.SelectedItem.ToString()) return;
            authors[authors_n++] = comboBox1.SelectedItem.ToString();
            textBox10.Text = "";
            for (int i = 0; i < authors_n; i++) textBox10.Text += authors[i] + "   ";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (authors_n == 0) return;
            authors_n--;
            textBox10.Text = "";
            for (int i = 0; i < authors_n; i++) textBox10.Text += authors[i] + "   ";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex < 0) return;
            for (int i = 0; i < genres_n; i++) if (genres[i] == comboBox2.SelectedItem.ToString()) return;
            genres[genres_n++] = comboBox2.SelectedItem.ToString();
            textBox11.Text = "";
            for (int i = 0; i < genres_n; i++) textBox11.Text += genres[i] + "   ";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (genres_n == 0) return;
            genres_n--;
            textBox11.Text = "";
            for (int i = 0; i < genres_n; i++) textBox11.Text += genres[i] + "   ";
        }
    }
}
