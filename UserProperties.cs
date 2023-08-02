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
    public partial class UserProperties : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        UserAccount userAccount;
        public UserProperties(UserAccount user)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            userAccount = user;
            LoadRole();
        }

        public void LoadRole()
        {

            cbRole.DataSource = dbcon.getTable("SELECT * FROM tbRole");
            cbRole.DisplayMember = "role";
            cbRole.ValueMember = "id";
        }

        private void btnAccCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnAccSave_Click(object sender, EventArgs e)
        {
            try
            {
                if ((MessageBox.Show("Are you sure you want to change this account properties?", "Change Properties", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)== DialogResult.Yes))
                {
                    cn.Open();
                    cm = new SqlCommand("UPDATE tbUser SET name = @name, roleid = @roleid, password = @pass WHERE username = '" + txtUsername.Text + "'", cn);
                    cm.Parameters.AddWithValue("@name", txtFullName.Text);
                    cm.Parameters.AddWithValue("@roleid", cbRole.SelectedValue);
                    cm.Parameters.AddWithValue("@pass", txtPass.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Account properties has been successfully changed!", "Update Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    userAccount.LoadUser();
                    this.Dispose();                   
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            }
    }
}
