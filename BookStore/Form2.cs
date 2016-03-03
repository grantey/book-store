using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace BookStore
{
    public partial class Form2 : Form
    {
        public Form2(int UserID)
        {
            InitializeComponent();
            this.UserID = UserID;
        }
        int UserID;
        string Login;
        cSQL sql = new cSQL();

        void newLabel()
        {            
            string queryString = "SELECT SUM(c.Price*o.BookQty), SUM(o.BookQty)" +
                            " FROM Book b JOIN CurrentSale c ON b.BookID = c.BookID" +
                            " JOIN Orders o ON c.BookID = o.BookID" +
                            " WHERE o.UserID = '" + UserID.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            string price = reader[0].ToString();
            if (price != "")
            {
                int p = price.IndexOf(',');
                label2.Text = "Выбрано книг:   " + reader[1].ToString();
                label3.Text = "На сумму:   " + price.Substring(0, p) + " р. " + price.Substring(p + 1, 2) + " к.";
            }
            else
            {
                label2.Text = "Выбрано книг:   0";
                label3.Text = "На сумму:   0 р. 0 к.";
            }
            reader.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {            
            sql.Connect();

            string queryString = "EXEC SelectAllBooks";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[0].Width = 150; dataGridView1.Columns[1].Width = 270; dataGridView1.Columns[2].Width = 150; dataGridView1.Columns[3].Width = 90;

            queryString = "SELECT Login FROM Users WHERE UserID = '" + Convert.ToString(UserID) + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            Login = Convert.ToString(reader[0]);
            label1.Text += Login;
            reader.Close();

            queryString = "SELECT Genre FROM Genre";
            reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox1.Items.Add(Convert.ToString(reader[0]));
            reader.Close();

            queryString = "SELECT Name FROM Author";
            reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox2.Items.Add(Convert.ToString(reader[0]));
            reader.Close();

            newLabel();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            label1.Focus();
        }      
        
        private void button3_Click(object sender, EventArgs e)
        {
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int BookID;

            string queryString = "EXEC SelectBookID N'" + dataGridView1.CurrentRow.Cells[1].Value.ToString() + "', N'" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);            
            reader.Read();
            BookID = Convert.ToInt32(reader[0]);
            reader.Close();

            Book dlg = new Book(BookID, UserID, 1);
            dlg.ShowDialog();

            newLabel();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Orders dlg = new Orders(UserID);
            dlg.ShowDialog();

            newLabel();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }
    }
    
}
