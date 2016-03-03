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
    public partial class OrdersSubmit : Form
    {
        public OrdersSubmit()
        {
            InitializeComponent();
        }
        cSQL sql = new cSQL();

        void summ()
        {
            double s;
            string price;
            int p;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (!DBNull.Value.Equals(dataGridView1.Rows[i].Cells[7].Value))
                    s = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value) * Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value) * (100 - Convert.ToInt32(dataGridView1.Rows[i].Cells[7].Value)) / 100;
                else
                    s = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value) * Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value);
                price = s.ToString();
                p = price.IndexOf(',');
                if (p > 0) dataGridView1.Rows[i].Cells[8].Value = price.Substring(0, p + 2) + "0";
                else dataGridView1.Rows[i].Cells[8].Value = price + ",00";
            }
        }
        void summ2()
        {
            double s;
            string price;
            int p;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (!DBNull.Value.Equals(dataGridView1.Rows[i].Cells[7].Value))
                    s = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value) * Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value) * (100 - Convert.ToInt32(dataGridView1.Rows[i].Cells[7].Value)) / 100;
                else
                    s = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value) * Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value);
                price = s.ToString();
                p = price.IndexOf(',');
                if (p > 0) dataGridView1.Rows[i].Cells[8].Value = price.Substring(0, p + 2) + "0";
                else dataGridView1.Rows[i].Cells[8].Value = price + ",00";
            }
        }
        private void OrdersSubmit_Load(object sender, EventArgs e)
        {
            sql.Connect();

            string queryString = "EXEC SelectFromOrder2";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns.Add("column8", "Всего");
            dataGridView1.Columns[1].Width = 100; dataGridView1.Columns[2].Width = 130;
            dataGridView1.Columns[3].Width = 270; dataGridView1.Columns[4].Width = 110; 
            dataGridView1.Columns[5].Width = 70; dataGridView1.Columns[6].Width = 70;
            dataGridView1.Columns[7].Width = 70;
            dataGridView1.Columns[0].Visible = false;
            summ();
        }

        private void OrdersSubmit_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Оформить заказ ?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.No) return;
            string queryString = "UPDATE Orders SET SaleDate = GETDATE() WHERE OrderID = " + dataGridView1.CurrentRow.Cells[0].Value.ToString();
            dataGridView1.CurrentRow.Cells[4].Value = DateTime.Now.ToString();
            sql.Query(queryString);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            string queryString;
            if (checkBox1.Checked) queryString = "EXEC SelectFromOrder3";
            else queryString = "EXEC SelectFromOrder2";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataBindings.Clear();
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns.Add("column7", "Всего");
            if (checkBox1.Checked) summ2();
            else summ();
           // summ();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int BookID;

            string queryString = "EXEC SelectBookID N'" + dataGridView1.CurrentRow.Cells[2].Value.ToString() + "', N'" + dataGridView1.CurrentRow.Cells[1].Value.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            BookID = Convert.ToInt32(reader[0]);
            reader.Close();

            Book dlg = new Book(BookID, 0, 2);
            dlg.ShowDialog();
        }
    }
}
