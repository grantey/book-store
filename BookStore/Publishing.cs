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
    public partial class Publishing : Form
    {
        public Publishing()
        {
            InitializeComponent();
        }
        cSQL sql = new cSQL();

        private void Publishing_Load(object sender, EventArgs e)
        {
            sql.Connect();
            string queryString = "SELECT Name AS 'Название', Address AS 'Адрес', Email AS 'E-mail', Phone AS 'Телефон' FROM Publishing";            
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[0].Width = 170; dataGridView1.Columns[1].Width = 290; dataGridView1.Columns[2].Width = 150; dataGridView1.Columns[3].Width = 150;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int PublishID;

            string queryString = "SELECT PublishID FROM Publishing WHERE Name = '" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            PublishID = Convert.ToInt32(reader[0]);
            reader.Close();

            PublishingEdit dlg = new PublishingEdit(PublishID);
            dlg.ShowDialog();

            queryString = "SELECT Name AS 'Название', Address AS 'Адрес', Email AS 'E-mail', Phone AS 'Телефон' FROM Publishing";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить выбранное издательство ?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.No) return;

            string queryString = "DELETE FROM Publishing WHERE Name = '" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "';";
            sql.Query(queryString);
            bindingSource1.RemoveCurrent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PublishingAdd dlg = new PublishingAdd();
            dlg.ShowDialog();
            string queryString = "SELECT Name AS 'Название', Address AS 'Адрес', Email AS 'E-mail', Phone AS 'Телефон' FROM Publishing";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;
        }

        private void Publishing_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }
    }
}
