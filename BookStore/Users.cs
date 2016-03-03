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
    public partial class Users : Form
    {
        public Users()
        {
            InitializeComponent();
        }
        cSQL sql = new cSQL();

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить выбранного пользователя ?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.No) return;

            string queryString = "DELETE FROM Users WHERE Login = '" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "';";
            sql.Query(queryString);
            bindingSource1.RemoveCurrent();
        }

        private void Users_Load(object sender, EventArgs e)
        {
            sql.Connect();
            string queryString = "SELECT Login AS 'Логин', FirstName AS 'Имя', LastName AS 'Фамилия', Email AS 'Email', Address AS 'Адрес', Phone AS 'Телефон' FROM Users";
            bindingSource1.DataSource = sql.TableQuery(queryString);
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.Columns[3].Width = 130; dataGridView1.Columns[4].Width = 210;             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Users_FormClosing(object sender, FormClosingEventArgs e)
        {
            sql.Disconnect();
        }

    }
}
