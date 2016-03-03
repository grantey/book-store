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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewUser dlg = new NewUser();
            dlg.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cSQL sql = new cSQL();
            int UserID, Permission;
            sql.Connect();
     
            string queryString = "SELECT UserID, Permission FROM Users WHERE Login = '" + textBox1.Text + "' AND Password = '" + textBox2.Text + "';";
            SqlDataReader reader = sql.ReaderQuery(queryString);
            if (!reader.HasRows) { MessageBox.Show("Неверные имя пользователя и пароль", "Предупреждение", MessageBoxButtons.OK); return; }
            reader.Read();
            UserID = Convert.ToInt32(reader[0]);
            Permission = Convert.ToInt32(reader[1]);
            reader.Close();
            sql.Disconnect();

            this.Hide();
            if (Permission == 0)
            {
                Form2 dlg = new Form2(UserID);
                dlg.ShowDialog();
            }
            else
            {
                Form3 dlg = new Form3();
                dlg.ShowDialog();
            }
            this.Show();
        }
    }
}
