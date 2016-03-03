using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BookStore
{
    public partial class Orders : Form
    {
        public Orders(int UserID)
        {
            InitializeComponent();
            this.UserID = UserID;
        }
        int UserID;

        cSQL sql = new cSQL();

        void newLabel()
        {            
            double sum = 0;
            int q = 0;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                sum += Convert.ToDouble(dataGridView1.Rows[i].Cells[6].Value);
                q += Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value);
            }
            string price = sum.ToString();
            int p = price.IndexOf(',');
            if (p > 0) label1.Text = "Всего: " + q.ToString() + " книг на сумму " + price.Substring(0, p) + " р. " + price.Substring(p + 1, 1) + "0 к.";
            else label1.Text = "Всего: " + q.ToString() + " книг на сумму " + price.ToString() + " р. 00 к.";
        }

        void summ()
        {
            double s;
            string price;
            int p;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (!DBNull.Value.Equals(dataGridView1.Rows[i].Cells[5].Value))
                    s = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value) * Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value) * (100 - Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value)) / 100;
                else
                    s = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value) * Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value);
                price = s.ToString();
                p = price.IndexOf(',');
                if (p > 0) dataGridView1.Rows[i].Cells[6].Value = price.Substring(0, p + 2) + "0";
                else dataGridView1.Rows[i].Cells[6].Value = price + ",00";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить выбранную книгу из списка заказов ?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.No) return;

            string str1 = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
            string str2 = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
            string queryString = "EXEC SelectBookID @Author = '" + str1 + "', @Title = '" + str2 + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            str1 = reader[0].ToString();
            str2 = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[4].Value.ToString();
            reader.Close();

          //  queryString = "UPDATE CurrentSale SET StoreQty = StoreQty + " + str2 + " WHERE BookID = '" + str1 + "';";
          //  sql.Query(queryString);

            queryString = "DELETE FROM Orders WHERE UserID = '" + UserID.ToString() + "' AND BookID = '" + str1 + "';";
            sql.Query(queryString);
            bindingSource1.RemoveCurrent();

            newLabel();
        }

        private void Orders_Load(object sender, EventArgs e)
        {
            sql.Connect();
            string queryString = "EXEC SelectFromOrder @UserID = '" + UserID.ToString() + "';";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;            
            dataGridView1.Columns.Add("column5","Всего");

            dataGridView1.Columns[0].Width = 130; dataGridView1.Columns[1].Width = 270; 
            dataGridView1.Columns[2].Width = 150; dataGridView1.Columns[3].Width = 70;
            dataGridView1.Columns[4].Width = 70; dataGridView1.Columns[5].Width = 70;
            
            summ();
            newLabel();
        }

        private void Orders_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int BookID;

            string queryString = "EXEC SelectBookID N'" + dataGridView1.CurrentRow.Cells[1].Value.ToString() + "', N'" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            BookID = Convert.ToInt32(reader[0]);
            reader.Close();

            Book dlg = new Book(BookID, UserID, 2);
            dlg.ShowDialog();
        }
    }
}
