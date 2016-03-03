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
    public partial class AuthorAbout : Form
    {
        public AuthorAbout(string Name)
        {
            InitializeComponent();
            this.Name = Name;
        }
        string Name;        
        cSQL sql = new cSQL();

        private void AuthorAbout_Load(object sender, EventArgs e)
        {
            label1.Text = Name;

            sql.Connect();
            string queryString = "SELECT About, AuthorID FROM Author WHERE Name = '" + Name + "'";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            if (!DBNull.Value.Equals(reader[0])) textBox1.Text = reader[0].ToString();
            else textBox1.Text = "Нет информации";
            string AuthorID = reader[1].ToString();
            reader.Close();
            queryString = "SELECT Title FROM Book WHERE BookID IN (SELECT BookID FROM AuthorBook WHERE AuthorID = '" + AuthorID + "')";
            reader = sql.ReaderQuery(queryString);
            while (reader.Read()) textBox2.Text += reader[0].ToString() + Environment.NewLine;            
            reader.Close();
        }

        private void AuthorAbout_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }

    }
}
