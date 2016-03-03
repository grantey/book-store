using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BookStore
{
    public partial class AuthorAdd : Form
    {
        public AuthorAdd()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cSQL sql = new cSQL();
            sql.Connect();
            string queryString = "INSERT INTO Author (Name, About) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "');";
            sql.Query(queryString);
            sql.Disconnect();
            this.Close();
        }
    }
}
