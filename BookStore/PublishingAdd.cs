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
    public partial class PublishingAdd : Form
    {
        public PublishingAdd()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cSQL sql = new cSQL();
            sql.Connect();

            string email_pattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            string phone_pattern = @"^((8|\+7)?[\- ]?)?([\(\-]\d{3,4}[\)\-]?[\- ]?)?[\d\- ]{6,10}$";
            Regex email_rgx = new Regex(email_pattern);
            Regex phone_rgx = new Regex(phone_pattern);

            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0 || textBox3.Text.Length == 0 || textBox4.Text.Length == 0) { MessageBox.Show("Все поля должны быть заполнены", "Предупреждение", MessageBoxButtons.OK); return; }
            if (!email_rgx.IsMatch(textBox2.Text) || !phone_rgx.IsMatch(textBox3.Text)) { MessageBox.Show("Поля Email или Телефон содержат недопустимые символы", "Предупреждение", MessageBoxButtons.OK); return; }

            string queryString = "INSERT INTO Publishing (Name, Address, Email, Phone) VALUES ('" + textBox4.Text + "','" + textBox1.Text + "', '" + textBox2.Text + "','" + textBox3.Text + "');";
            sql.Query(queryString);
            sql.Disconnect();
            this.Close();
        }
    }
}
