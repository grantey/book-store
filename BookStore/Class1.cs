using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace BookStore
{
    class cSQL
    {
        SqlConnection connection;
        string str = "Data Source=(local);Initial Catalog=BookStore;Integrated Security=SSPI;";

        public void Connect()
        {
            try
            {
                connection = new SqlConnection(str);
                connection.Open();
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show("Can't connect to SQL Server" + exp.Message);
                return;
            }
        }

        public void Disconnect()
        {
            try
            {
                connection.Close();
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show("Can't connect to SQL Server" + exp.Message);
                return;
            }
        }

        public DataTable TableQuery(string SQLstring)
        {
            SqlCommand command = null;
            try
            {
                command = new SqlCommand(SQLstring, connection);
                SqlDataAdapter adapter = new SqlDataAdapter();

                adapter.SelectCommand = command;
                DataTable _table = new DataTable();
                adapter.Fill(_table);
                adapter.Dispose();
                command.Dispose();
                return _table;
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show(exp.Message);
                command.Dispose();
                return null;
            }
        }

        public SqlDataReader ReaderQuery(string SQLstring)
        {
            SqlCommand command = null;
            try
            {
                command = new SqlCommand(SQLstring, connection);
                SqlDataReader _reader = command.ExecuteReader();
                command.Dispose();
                return _reader;
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show(exp.Message);
                command.Dispose();
                return null;
            }
        }

        public int IntQuery(string SQLstring)
        {
            SqlCommand command = null;
            int res;
            try
            {
                command = new SqlCommand(SQLstring, connection);
                SqlDataReader _reader = command.ExecuteReader();
                command.Dispose();
                _reader.Read();
                if (!_reader.HasRows) return -1;
                res = Convert.ToInt32(_reader[0]);
                _reader.Close();
                return res;
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show(exp.Message);
                command.Dispose();
                return -1;
            }
        }

        public bool Query(string SQLstring)
        {
            SqlCommand command = null;
            try
            {
                command = new SqlCommand(SQLstring, connection);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show(exp.Message);
                command.Dispose();
                return false;
            }
            return true;
        }

        public void ImageUpload(string Img, int BookID)
        {
            FileStream fStream = new FileStream(Img, FileMode.Open, FileAccess.Read);
            Byte[] imageBytes = new byte[fStream.Length];
            fStream.Read(imageBytes, 0, imageBytes.Length);

            SqlParameter par = new SqlParameter("@ImageByte", SqlDbType.VarBinary);
            par.Value = imageBytes;
            SqlCommand command = null;

            try
            {
                command = new SqlCommand("UPDATE Book SET Image = @ImageByte WHERE BookID = '" + BookID.ToString() + "';", connection);
                command.Parameters.Add(par);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show(exp.Message);
                command.Dispose();
            }
        }
    }
}
