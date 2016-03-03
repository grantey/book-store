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
    public partial class Authors : Form
    {
        public Authors()
        {
            InitializeComponent();
        }
        cSQL sql = new cSQL();

        private void Authors_Load(object sender, EventArgs e)
        {
            sql.Connect();
            string queryString = "SELECT Name FROM Author";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox1.Items.Add(Convert.ToString(reader[0]));
            reader.Close();
        }

        private void Authors_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string queryString = "SELECT About FROM Author WHERE Name = '" + comboBox1.SelectedItem.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            if (reader.HasRows) textBox1.Text = reader[0].ToString();
            else textBox1.Text = "Нет информации";
            reader.Close();
            label1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить выбранного автора ?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.No) return;

            string queryString = "DELETE FROM Author WHERE Name = '" + comboBox1.SelectedItem.ToString() + "';";
            sql.Query(queryString);
            comboBox1.Items.RemoveAt(comboBox1.SelectedIndex);
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AuthorAdd dlg = new AuthorAdd();
            dlg.ShowDialog();
            string queryString = "SELECT Name FROM Author";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            while (reader.Read()) comboBox1.Items.Add(Convert.ToString(reader[0]));
            reader.Close();
            label1.Focus();
        }
    }
}
