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
    public partial class UserAccount : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        MainForm main;
        SqlDataReader dr;
        string username;
        string name;
        string role;
        string pass;
        public UserAccount(MainForm mn)
        {         
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            main = mn;
            LoadUser();
            LoadRole();
        }

        public void LoadRole()
        {
            cbRole.Items.Clear();
            cbRole.DataSource = dbcon.getTable("SELECT * FROM tbRole");
            cbRole.DisplayMember = "role";
            cbRole.ValueMember = "id";
        }

        public void LoadUser()
        {
            int i = 0;
            dgvUser.Rows.Clear();
            cm = new SqlCommand("SELECT username, name, role, password  from tbUser u join tbRole r on u.roleid = r.id", cn);
            cn.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvUser.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            cn.Close();
        }

        public void clear() 
        {
            txtName.Clear();
            txtPass.Clear();
            txtRePass.Clear();
            txtUsername.Clear();
            cbRole.Text = "Select Role";
            txtUsername.Focus();
        }

        private void btnAccCancel_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btnAccSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPass.Text != txtRePass.Text)
                {
                    MessageBox.Show("Password did not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                cn.Open();
                cm = new SqlCommand("Insert into tbUser(username, password, roleid, name) Values (@username, @password, @roleid, @name)", cn); cm.Parameters.AddWithValue("@username", txtUsername.Text);
                cm.Parameters.AddWithValue("@password", txtPass.Text);
                cm.Parameters.AddWithValue("@roleid", cbRole.SelectedValue);
                cm.Parameters.AddWithValue("@name", txtName.Text);
                cm.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("New account has been successfully saved!", "Save Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clear();
                LoadUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning");
            }
        }

        private void btnPsSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCurPass.Text != main._pass)
                {
                    MessageBox.Show("Current password did not martch!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    return;
                }
                    
                if (txtNPass.Text != txtRePass2.Text)
                {
                    MessageBox.Show("Confirm new password did not martch!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    return;                   
                }
                cn.Open();
                cm = new SqlCommand("UPDATE tbUser SET password = '"+ txtNPass.Text + "'WHERE username = '"+ lblUsername.Text + "'",cn);
                cm.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("Password has been successfully saved!", "Save Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void btnPsCancel_Click(object sender, EventArgs e)
        {
            ClearCP();
        }
        public void ClearCP()
        {
            txtCurPass.Clear();
            txtNPass.Clear();
            txtRePass2.Clear();
        }
        private void UserAccount_Load(object sender, EventArgs e)
        {
            lblUsername.Text = main.lblUsername.Text;
        }

        private void dgvUser_SelectionChanged(object sender, EventArgs e)
        {
            int i = dgvUser.CurrentRow.Index;
            username = dgvUser[1, i].Value.ToString();
            name = dgvUser[2, i].Value.ToString(); 
            role = dgvUser[3, i].Value.ToString();
            pass = dgvUser[4, i].Value.ToString();

            if (lblUsername.Text == username)
            {
                btnRemove.Enabled = false;
            }
            else
            {
                btnRemove.Enabled = true;               
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if((MessageBox.Show("You chose to remove this account from this Point Of Sales System's user list. \n Are you sure you want to remove '" + username + "'", "User Account", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                dbcon.ExecuteQuery("DELETE FROM tbUser WHERE username = '" + username + "'");
            MessageBox.Show("Account has been successfully deleted");
                LoadUser();
            }   
            
        }

        private void btnProperties_Click(object sender, EventArgs e)
        {
            UserProperties properties = new UserProperties(this); 
            properties.Text = name + "\"" + username + " Properties";
            properties.txtFullName.Text = name;
            properties.txtUsername.Text = username;
            properties.cbRole.Text = role;
            properties.txtPass.Text = pass;
            properties.ShowDialog();
        }
    }
}
