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
    public partial class PublishingEdit : Form
    {
        public PublishingEdit(int PublishID)
        {
            InitializeComponent();
            this.PublishID = PublishID;
        }
        int PublishID;
        cSQL sql = new cSQL();

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email_pattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            string phone_pattern = @"^((8|\+7)?[\- ]?)?([\(\-]\d{3,4}[\)\-]?[\- ]?)?[\d\- ]{6,10}$";
            Regex email_rgx = new Regex(email_pattern);
            Regex phone_rgx = new Regex(phone_pattern);

            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0 || textBox3.Text.Length == 0) {MessageBox.Show("Все поля должны быть заполнены", "Предупреждение", MessageBoxButtons.OK); return; }
            if (!email_rgx.IsMatch(textBox2.Text) || !phone_rgx.IsMatch(textBox3.Text)) {MessageBox.Show("Поля Email или Телефон содержат недопустимые символы", "Предупреждение", MessageBoxButtons.OK); return; }

            string queryString = "UPDATE Publishing SET Address = '" + textBox1.Text + "', Email = '" + textBox2.Text + "', Phone = '" + textBox3.Text + "' WHERE PublishID = '" + PublishID.ToString() + "';";
            sql.Query(queryString);

            this.Close();
        }

        private void PublishingEdit_Load(object sender, EventArgs e)
        {
            sql.Connect();
            string queryString = "SELECT Name, Address, Email, Phone FROM Publishing WHERE PublishID = '" + PublishID.ToString() + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            reader.Read();
            label1.Text = reader[0].ToString();
            textBox1.Text = reader[1].ToString();
            textBox2.Text = reader[2].ToString();
            textBox3.Text = reader[3].ToString();
            reader.Close();
        }

        private void PublishingEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }
    }
}

