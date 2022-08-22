
using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
namespace RAx
{
    class Connection
    {
        public OleDbConnection myConnection;
        public OleDbDataAdapter adapter;
        public static string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Av_Bl1.mdb;";
        public DataSet ConnectDS(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                myConnection = new OleDbConnection(connectString);
                myConnection.Open();
                adapter = new OleDbDataAdapter(sql, myConnection);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ds;
        }
        public void SIDU(string sql) //select;insert;update;delete
        {
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();
            OleDbCommand command = myConnection.CreateCommand();
            try
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
                MessageBox.Show("Успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}