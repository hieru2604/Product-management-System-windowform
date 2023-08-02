using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaptopDemo
{
    public partial class Record : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        SqlDataReader dr;
        public Record()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            LoadCriticalItems();
            LoadInventory();
            LoadAdjust();
        }

        public void LoadTopSelling()
        {
            int i = 0;
            dgvTopSelling.Rows.Clear();
            cn.Open();
            //Sort By Total Amount
            if (cbTopSell.Text == "Sort By Qty")
            {
                cm = new SqlCommand("SELECT TOP 10 pcode, pname, isnull(SUM(qty),0) as qty, isnull(SUM(total),0) AS total FROM vwSoldItems WHERE sdate BETWEEN '" + dtFromTopSell.Value + "' and '" + dtToTopSell.Value + "' AND status like 'sold' group by pcode, pname order by qty DESC", cn);   
            }
            else if (cbTopSell.Text == "Sort By Total Amount")
            {
                cm = new SqlCommand("SELECT TOP 10 pcode, pname, isnull(SUM(qty),0) as qty, isnull(SUM(total),0) AS total FROM vwSoldItems WHERE sdate BETWEEN '" + dtFromTopSell.Value + "' and '" + dtToTopSell.Value + "' AND status like 'sold' group by pcode, pname order by total DESC", cn);
            }
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvTopSelling.Rows.Add(i, dr["pcode"].ToString(), dr["pname"].ToString(), dr["qty"].ToString(), double.Parse(dr["total"].ToString()).ToString("#,##0.00"));
            }
               
            dr.Close();
            cn.Close();
        }

        public void LoadSoldItem()
        {                 
            try
            {
                dgvSoldItem.Rows.Clear();
                int i = 0;
                cn.Open();
                cm = new SqlCommand("SELECT c.pcode, p.pname, c.price, sum(c.qty) as qty, sum(c.disc) as disc, sum(c.total) as total FROM tbCart c join tbProducts p ON c.pcode = p.pcode WHERE sdate BETWEEN '" + dtFromSoldItem.Value + "' and '" + dtToSoldItem.Value + "' AND status like 'sold' group by c.pcode, p.pname, c.price ", cn);
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvSoldItem.Rows.Add(i, dr["pcode"].ToString(), dr["pname"].ToString(), double.Parse(dr["price"].ToString()).ToString("#,##0.00"), dr["qty"].ToString(), dr["disc"].ToString(), double.Parse(dr["total"].ToString()).ToString("#,##0.00"));
                }
                dr.Close();
                cn.Close();

                cn.Open();
                cm = new SqlCommand("SELECT isnull(sum(total),0) FROM tbCart WHERE sdate BETWEEN '" + dtFromSoldItem.Value + "' and '" + dtToSoldItem.Value + "' AND status like 'sold'", cn);
                lblTotal.Text = double.Parse(cm.ExecuteScalar().ToString()).ToString("#,##0.00");
                cn.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadCriticalItems()
        {
            try
            {
                dgvCriticalItems.Rows.Clear();
                int i = 0;
                cn.Open();
                cm = new SqlCommand("SELECT * FROM vwCriticalItems", cn);
                dr = cm.ExecuteReader();

                while (dr.Read())
                {
                    i++;
                    dgvCriticalItems.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadInventory()
        {
            try
            {
                dgvInventory.Rows.Clear();
                int i = 0;
                cn.Open();
                cm = new SqlCommand("SELECT * FROM vwInventoryList ", cn);
                dr = cm.ExecuteReader();

                while (dr.Read())
                {
                    i++;
                    dgvInventory.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadCancelled()
        {
            try
            {
                dgvCancelled.Rows.Clear();
                int i = 0;
                cn.Open();
                cm = new SqlCommand("SELECT * FROM vwCancelItems WHERE sdate BETWEEN '" + dtFromCancel.Value + "' and '" + dtToCancel.Value + "'", cn);
                dr = cm.ExecuteReader();

                while (dr.Read())
                {
                    i++;
                    dgvCancelled.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), DateTime.Parse(dr[6].ToString()).ToShortDateString(), dr[7].ToString(), dr[8].ToString(), dr[9].ToString(), dr[10].ToString());
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void LoadAdjust()
        {
            try
            {
                dgvAdjust.Rows.Clear();
                int i = 0;
                cn.Open();
                cm = new SqlCommand("SELECT referenceno, a.pcode, pname, sdate, a.qty, action, remarks, [user] FROM tbAdjustment a join tbProducts p ON a.pcode = p.pcode ", cn);
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvAdjust.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), DateTime.Parse(dr[3].ToString()).ToShortDateString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
                }
                dr.Close();
                cn.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cbTopSell.Text == "Select sort type")
            {
                MessageBox.Show("Please select sort type from the dropdown list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                cbTopSell.Focus();
                return;
            }
            LoadTopSelling();
        }

        private void btnLoadSoldItem_Click(object sender, EventArgs e)
        {
            LoadSoldItem();
        }

        private void btnLoadCancel_Click(object sender, EventArgs e)
        {
            LoadCancelled();
        }
    }
}
