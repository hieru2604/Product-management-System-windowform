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
    public partial class SupplierModule : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        string stitle = "Point of sales";
        Supplier supplier;

        public SupplierModule(Supplier sp)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            supplier = sp;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        public void Clear()
        {
            txtAddress.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtSname.Clear();
            txtContactPerson.Clear();

            btnSave.Enabled = true;
            btnUpdate.Enabled = false; 
            txtSname.Focus();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Save this record? click yes to confirm.", "CONFIRM", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("Insert into tbSupplier ( supplier, address, contactperson,  phone, email) values (@supplier, @address, @contactperson, @phone, @email ) ", cn);
                    cm.Parameters.AddWithValue("@supplier", txtSname.Text);
                    cm.Parameters.AddWithValue("@address", txtAddress.Text);
                    cm.Parameters.AddWithValue("@contactperson", txtContactPerson.Text);
                    cm.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cm.Parameters.AddWithValue("@email", txtEmail.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Record has been successfully saved!", "Save Record", MessageBoxButtons.OK, MessageBoxIcon.Information); Clear();
                    Clear();
                    supplier.LoadSupplier();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Update this record? click yes to confirm.", "CONFIRM", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                cn.Open();
                cm = new SqlCommand("Update tbSupplier SET supplier = @supplier, address = @address, contactperson=@contactperson,  phone = @phone, email = @email where id = @id ", cn);
                cm.Parameters.AddWithValue("@id", lblid.Text);
                cm.Parameters.AddWithValue("@supplier", txtSname.Text); 
                cm.Parameters.AddWithValue("@address", txtAddress.Text);
                cm.Parameters.AddWithValue("@contactperson", txtContactPerson.Text);
                cm.Parameters.AddWithValue("@phone", txtPhone.Text);
                cm.Parameters.AddWithValue("@email", txtEmail.Text);
                cm.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("Record has been successfully updated!", "Save Record", MessageBoxButtons.OK, MessageBoxIcon.Information); Clear();
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle);
            }
        }
    }
}
