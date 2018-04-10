using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace block
{
    public partial class Form1 : Form
    {
      
        DataSet dataSet = new DataSet();
        DataTable dataTable = new DataTable();
        NpgsqlTransaction tr ;
        NpgsqlConnection con ;
        NpgsqlCommand command ;
        public Form1()
        {
        
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string name = textBox1.Text;
            string manuf = "test";
            string sql = "";
            int nowRow = dataGridView1.CurrentRow.Index;
            try
            {
                sql = "SELECT* FROM drug WHERE id = " + dataGridView1.Rows[nowRow].Cells[0].Value + " FOR UPDATE NOWAIT";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "UPDATE drug  SET name = '" + name + "', manufacturer = '" + manuf + "' WHERE id = " + dataGridView1.Rows[nowRow].Cells[0].Value /*+ " and pg_try_advisory_xact_lock(tableoid::INTEGER,id)"*/;             
                command = new NpgsqlCommand(sql, con);
                command.Transaction = tr;
                command.ExecuteNonQuery();

              

            }
            catch (Exception ex)
            {
                tr.Rollback();
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            try
            {
                //MessageBox.Show(tr.IsolationLevel.ToString());
                tr.Commit();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            try
            {
                dataSet.Clear();
                string constr = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=22; Database=polyclinicver3;";
                con = new NpgsqlConnection(constr);
                con.Open();
                tr = con.BeginTransaction(IsolationLevel.RepeatableRead);

                command = con.CreateCommand();
                command.Transaction = tr;

                string query = "SELECT * FROM drug WHERE id > 111";

                command = new NpgsqlCommand(query, con);
                NpgsqlDataAdapter adap = new NpgsqlDataAdapter(command);
                adap.Fill(dataSet, "base");
                dataGridView1.DataSource = dataSet.Tables[0];
                textBox1.Text = Convert.ToString(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value);

                //tr.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //tr.Rollback();
            }
        }
    }
}
