using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace BookStore
{
    public partial class NewUser : Form
    {
        public NewUser()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0 || textBox3.Text.Length == 0) { MessageBox.Show("Поля Логин и Пароль обязательны для заполнения", "Предупреждение", MessageBoxButtons.OK); return; }
            if (textBox2.Text != textBox3.Text) { MessageBox.Show("Введенные пароли не совпадают", "Предупреждение", MessageBoxButtons.OK); return; }

            string logpas_pattern = @"^[0-9a-zA-Zа-яА-Я\s]*$";
            string name_pattern = @"^[a-zа-яA-ZА-Я]+[0-9a-zа-я\s]*$";
            string email_pattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            string phone_pattern = @"^((8|\+7)[\- ]?)?([\(\-]\d{3,4}[\)\-]?[\- ]?)?[\d\- ]{6,10}$";
            Regex logpas_rgx = new Regex(logpas_pattern);
            Regex name_rgx = new Regex(name_pattern);
            Regex email_rgx = new Regex(email_pattern);
            Regex phone_rgx = new Regex(phone_pattern);

            if ((textBox1.Text.Length > 0 && !logpas_rgx.IsMatch(textBox1.Text)) || (textBox2.Text.Length > 0 && !logpas_rgx.IsMatch(textBox2.Text)) || (textBox3.Text.Length > 0 && !logpas_rgx.IsMatch(textBox3.Text))) {MessageBox.Show("Поля Логин или Пароль содержат недопустимые символы", "Предупреждение", MessageBoxButtons.OK); return;}
            if ((textBox4.Text.Length > 0 && !name_rgx.IsMatch(textBox4.Text)) || (textBox5.Text.Length > 0 && !name_rgx.IsMatch(textBox5.Text))) {MessageBox.Show("Проверьте правильность Имени и Фамилии", "Предупреждение", MessageBoxButtons.OK); return;}
            if (textBox6.Text.Length > 0 && !email_rgx.IsMatch(textBox6.Text)) { MessageBox.Show("Проверьте правильность E-mail", "Предупреждение", MessageBoxButtons.OK); return; }
            if (textBox8.Text.Length > 0 && !phone_rgx.IsMatch(textBox8.Text)) { MessageBox.Show("Проверьте правильность Телефона", "Предупреждение", MessageBoxButtons.OK); return; }

            cSQL sql = new cSQL();
            sql.Connect();
            string queryString = "SELECT * FROM Users WHERE Login = '" + textBox1.Text + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            if (reader.HasRows) {MessageBox.Show("Такой Логин уже существует", "Предупреждение", MessageBoxButtons.OK); return; }
            reader.Close();

            queryString = "INSERT INTO Users (Permission, Login, Password, FirstName, LastName, Email, Address, Phone) VALUES ('0','" 
                + textBox1.Text + "','" + textBox2.Text + "','" + textBox4.Text + "','" + textBox5.Text + "','" + textBox6.Text + "','" + textBox7.Text + "','" + textBox8.Text + "');";
            sql.Query(queryString);
            sql.Disconnect();

            MessageBox.Show("Регистрация завершена", "", MessageBoxButtons.OK);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
