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
    public partial class Supplier : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        SqlDataReader dr;
        public Supplier()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            LoadSupplier();
        }
        public void LoadSupplier()
        {
            int i = 0;
            dgvSupplier.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tbSupplier", cn);
            cn.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvSupplier.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SupplierModule supplierModule = new SupplierModule(this);
            supplierModule.ShowDialog();
        }

        private void dgvSupplier_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvSupplier.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                SupplierModule supplier = new SupplierModule(this);
                supplier.lblid.Text = dgvSupplier.Rows[e.RowIndex].Cells[1].Value.ToString();
                supplier.txtSname.Text = dgvSupplier.Rows[e.RowIndex].Cells[2].Value.ToString();
                supplier.txtAddress.Text = dgvSupplier.Rows[e.RowIndex].Cells[3].Value.ToString();
                supplier.txtContactPerson.Text = dgvSupplier.Rows[e.RowIndex].Cells[4].Value.ToString();
                supplier.txtPhone.Text = dgvSupplier.Rows[e.RowIndex].Cells[5].Value.ToString();
                supplier.txtEmail.Text = dgvSupplier.Rows[e.RowIndex].Cells[6].Value.ToString();

                supplier.btnSave.Enabled = false;
                supplier.btnUpdate.Enabled = true;
                supplier.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("DELETE FROM tbSupplier WHERE id LIKE '" + dgvSupplier[1, e.RowIndex].Value.ToString() + "'", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Supplier has been successfully deleted.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            LoadSupplier();
        }
    }
}
