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

namespace BookStore
{
    public partial class Book : Form
    {
        public Book(int BookID, int UserID, int mode)
        {
            InitializeComponent();
            this.BookID = BookID;
            this.UserID = UserID;
            this.mode = mode;
        }
        int BookID, UserID, mode;
        string Author;
        cSQL sql = new cSQL();

        void Justify()
        {
        }

        void ImageRead()
        {
            string queryString = "SELECT Image FROM Book WHERE BookID = '" + BookID.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            try
            {
                int bLength = (int)reader.GetBytes(0, 0, null, 0, int.MaxValue);
                byte[] bBuffer = new byte[bLength];
                reader.GetBytes(0, 0, bBuffer, 0, bLength);
                reader.Close();

                MemoryStream mStream = new MemoryStream(bBuffer);
                Bitmap Img = new Bitmap(mStream);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = Img;
            }
            catch (Exception)
            {
                reader.Close();
                return;
            }
        }

        private void Book_Load(object sender, EventArgs e)
        {
            if (mode == 1) { button3.Visible = false; button3.Enabled = false; button4.Visible = false; button4.Enabled = false; }
            if (mode == 0) { button1.Visible = false; button1.Enabled = false; button2.Visible = false; button2.Enabled = false; }
            if (mode == 2) { button1.Visible = false; button3.Visible = false; button4.Visible = false; listBox1.Visible = false; }

            sql.Connect();
            string queryString = "SELECT b.Title, b.ISBN, b.About, c.Price, YEAR(b.Year), c.Discount FROM Book b INNER JOIN CurrentSale c ON b.BookID = c.BookID WHERE b.BookID = '" + BookID.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            label2.Text = reader[0].ToString();
            label4.Text += reader[1].ToString();
            textBox1.Text = reader[2].ToString();
            string price = reader[3].ToString();
            int p = price.IndexOf(',');
            label5.Text += price.Substring(0,p) + " р. " + price.Substring(p+1, 1) + "0 к.";
            label7.Text += reader[4].ToString();
            if (!DBNull.Value.Equals(reader[5]))
            {
                label9.Text += reader[5].ToString() + " %";
                price = (Convert.ToDouble(reader[3]) * (100 - Convert.ToInt32(reader[5])) / 100).ToString();
                p = price.IndexOf(',');
                if (p > 0) label10.Text = price.Substring(0, p) + " р. " + price.Substring(p + 1, 1) + "0 к.";
                else label10.Text = price + " р. 00 к.";
            }
            else { label9.Text = ""; label11.Text = ""; }
            reader.Close();

            queryString = "SELECT g.Genre FROM Genre g INNER JOIN GenreBook gb ON gb.GenreID = g.GenreID WHERE gb.BookID = '" + BookID.ToString() + "';";
            reader = sql.ReaderQuery(queryString);
            while (reader.Read()) label8.Text += reader[0].ToString() + " ";
            label8.Text += ")";
            reader.Close();

            queryString = "SELECT a.Name FROM Author a INNER JOIN AuthorBook ab ON ab.AuthorID = a.AuthorID WHERE ab.BookID = '" + BookID.ToString() + "';";
            reader = sql.ReaderQuery(queryString);
            reader.Read();
            Author = reader[0].ToString();
            label1.Text += Author;
            while (reader.Read()) label1.Text += "     " + reader[0].ToString();
            reader.Close();

            queryString = "SELECT Name FROM Publishing WHERE PublishID IN (SELECT PublishingID	FROM Consignment WHERE BookID = '" + BookID.ToString() + "')";
            reader = sql.ReaderQuery(queryString);
            reader.Read();
            if (reader.HasRows) label6.Text += reader[0].ToString();
            else label6.Text += "Не известно";
            reader.Close();
            ImageRead();
            //Justify();

            //заполняем listbox
            queryString = "SELECT cs.StoreQty FROM CurrentSale cs WHERE cs.BookID = '" + BookID.ToString() + "';";
            reader = sql.ReaderQuery(queryString);
            reader.Read();
            int StoreQty = Convert.ToInt32(reader[0]);
            for (int i = 0; i <= StoreQty; i++) listBox1.Items.Add(i.ToString());
            reader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str;
            int Qty = listBox1.SelectedIndex;
            if (Qty > 0)
            {
                str = listBox1.SelectedItem.ToString();
                Bitmap Img = new Bitmap("X:\\ok.jpg");
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox2.Image = Img;
                for (int i = 0; i < Qty; i++) listBox1.Items.RemoveAt(listBox1.Items.Count - 1);

                //проверим нет ли уже такого заказа
                string queryString = "SELECT BookQty FROM Orders WHERE UserID = '" + UserID.ToString() + "' AND BookID = '" + BookID.ToString() + "';";
                SqlDataReader reader = sql.ReaderQuery(queryString);
                reader.Read();
                //если заказ есть, приплюсуем туда, иначе созададим новый
                if (reader.HasRows) queryString = "UPDATE Orders SET BookQty = BookQty + '" + Qty.ToString() + "' WHERE UserID = '" + UserID.ToString() + "' AND BookID = '" + BookID.ToString() + "';";                
                else queryString = "INSERT INTO Orders (UserID, BookID, BookQty) VALUES ('" + UserID.ToString() + "','" + BookID.ToString() + "','" + Qty.ToString() + "');";
                reader.Close();
                sql.Query(queryString);
                //уменьшим количество этих книг на складе
               // queryString = "UPDATE CurrentSale SET StoreQty = StoreQty - " + Qty.ToString() + " WHERE BookID = '" + BookID.ToString() + "';";
               // sql.Query(queryString);
            }
        }

        private void Book_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AuthorAbout dlg = new AuthorAbout(Author);
            dlg.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)  //удалить книгу
        {
            if (MessageBox.Show("Удалить всю информацию о книге ?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.No) return;
            string queryString = "DELETE FROM AuthorBook WHERE BookID = '" + BookID.ToString() + "'";
            sql.Query(queryString);
            queryString = "DELETE FROM GenreBook WHERE BookID = '" + BookID.ToString() + "'";
            sql.Query(queryString);            
            queryString = "DELETE FROM Consignment WHERE BookID = '" + BookID.ToString() + "'";
            sql.Query(queryString);
            queryString = "DELETE FROM CurrentSale WHERE BookID = '" + BookID.ToString() + "'";
            sql.Query(queryString);
            queryString = "DELETE FROM Book WHERE BookID = '" + BookID.ToString() + "'";
            sql.Query(queryString);
            System.Windows.Forms.MessageBox.Show("Информация удалена");
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)  //редактировать книгу
        {
            BookEdit dlg = new BookEdit(BookID);
            dlg.ShowDialog();
            this.Close();
        }
    }
}
