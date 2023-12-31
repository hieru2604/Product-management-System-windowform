﻿using System;
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
    public partial class DailySale : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        SqlDataReader dr;
        public string solduser;

        public DailySale()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            LoadCashier();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadCashier()
        {
            cboCashier.Items.Clear();
            cboCashier.Items.Add("All Cashier");
            cn.Open();
            cm = new SqlCommand("SELECT * FROM tbUser WHERE roleid LIKE '2' ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboCashier.Items.Add(dr["username"].ToString());
            }
               
            dr.Close();
            cn.Close();
        }

        public void LoadSold()
        {
            int i = 0;
            double total = 0;
            dgvSold.Rows.Clear();
            cn.Open();
            if (cboCashier.Text == "All Cashier")
            {
                cm = new SqlCommand("select c.id, c.transno, c.pcode, p.pname, p.pdesc, c.price, c.qty, c.disc, c.total from tbCart as c inner join tbProducts as p on c.pcode = p.pcode WHERE status LIKE 'sold' and sdate between '" + dtFrom.Value + "' and '" + dtTo.Value + "'", cn);
            }
            else
            {
                cm = new SqlCommand("select c.id, c.transno, c.pcode, p.pname, p.pdesc, c.price, c.qty, c.disc, c.total from tbCart as c inner join tbProducts as p on c.pcode = p.pcode WHERE status LIKE 'sold' and sdate between '" + dtFrom.Value + "' and '" + dtTo.Value + "' and cashier like '" + cboCashier.Text + "'", cn);
            }
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                total += double.Parse(dr["total"].ToString());
                dgvSold.Rows.Add(i, dr["id"].ToString(), dr["transno"].ToString(), dr["pcode"].ToString(), dr["pname"].ToString(), dr["pdesc"].ToString(), dr["price"].ToString(), dr["qty"].ToString(), dr["disc"].ToString(), dr["total"].ToString());
            }
            dr.Close();
            cn.Close();
            lblTotal.Text = total.ToString("#,##0.00");
        }

        private void cboCashier_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSold();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadSold();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadSold();
        }

        private void DailySale_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }

        private void dgvSold_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvSold.Columns[e.ColumnIndex].Name;
            if (colName == "Cancel")
            {
                CancelOrder cancel0rder = new CancelOrder(this);
                cancel0rder.txtId.Text = dgvSold.Rows[e.RowIndex].Cells[3].Value.ToString();
                cancel0rder.txtTransno.Text = dgvSold.Rows[e.RowIndex].Cells[2].Value.ToString();
                cancel0rder.txtPname.Text = dgvSold.Rows[e.RowIndex].Cells[4].Value.ToString();
                cancel0rder.txtDesc.Text = dgvSold.Rows[e.RowIndex].Cells[5].Value.ToString();
                cancel0rder.txtPrice.Text = dgvSold.Rows[e.RowIndex].Cells[6].Value.ToString();
                cancel0rder.txtQty.Text = dgvSold.Rows[e.RowIndex].Cells[7].Value.ToString();
                cancel0rder.txtDisc.Text = dgvSold.Rows[e.RowIndex].Cells[8].Value.ToString();
                cancel0rder.txtTotal.Text = dgvSold.Rows[e.RowIndex].Cells[9].Value.ToString();
                cancel0rder.txtCancelBy.Text = solduser;
                cancel0rder.lblCartId.Text = dgvSold.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
                cancel0rder.ShowDialog();

            }
                
        }
    }
}
