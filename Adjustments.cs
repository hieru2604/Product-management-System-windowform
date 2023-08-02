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
    public partial class Adjustments : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        SqlDataReader dr;
        MainForm main;
        int _qty;
        public Adjustments(MainForm mn)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            main = mn;
            ReferenceNo();
            LoadStock();
            lblUsername.Text = main.lblUsername.Text;
        }

        public void ReferenceNo()       // random refno code generate
        {
            Random rnd = new Random();
            lblRefno.Text = rnd.Next().ToString();

        }

        public void LoadStock()         // get data into the dgv
        {
            int i = 0;
            dgvAdjustment.Rows.Clear();     //reset the dgv everytime its called
            cm = new SqlCommand("SELECT p.pcode, p.pname, p.pdesc, b.brand, c.category, p.price, p.qty FROM tbProducts AS p INNER JOIN tbBrand AS b ON b.id = p.bid INNER JOIN tbCategory as c on c.id = p.cid", cn);
            cn.Open();          //sql datas
            dr = cm.ExecuteReader();        
            while (dr.Read())   //add data to each column
            {
                i++;
                dgvAdjustment.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void dgvAdjustment_CellContentClick(object sender, DataGridViewCellEventArgs e)     
        {
            string colName = dgvAdjustment.Columns[e.ColumnIndex].Name;
            if (colName == "Select")                                                //select items that you want to adjust 
            {
                lblPcode.Text = dgvAdjustment.Rows[e.RowIndex].Cells[1].Value.ToString();
                lblDesc.Text = dgvAdjustment.Rows[e.RowIndex].Cells[2].Value.ToString() + " " + dgvAdjustment.Rows[e.RowIndex].Cells[4].Value.ToString() + " " + dgvAdjustment.Rows[e.RowIndex].Cells[5].Value.ToString();
                _qty = int.Parse(dgvAdjustment.Rows[e.RowIndex].Cells[7].Value.ToString());
                btnSave.Enabled = true;

            }
                
        }
        public void Clear()         //reset the form
        {
            lblDesc.Text = "";
            lblPcode.Text = "";
            txtQty.Clear();
            txtRemark.Clear();
            cboAction.Text = "";
            ReferenceNo();
        }

        private void btnSave_Click(object sender, EventArgs e)          //changing the adjustment numbers
        {
            try   //check if the data is null
            {
                if (cboAction.Text == "")
                {
                    MessageBox.Show("Please select action for add or reduce.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                    cboAction.Focus();
                    return;
                }

                if (txtQty.Text == "")
                {
                    MessageBox.Show("Please input quantity for add or reduce.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }

                if (txtRemark.Text == "")
                {
                    MessageBox.Show("Need reason for stock adjustment.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                    txtRemark.Focus();
                    return;
                }

                //update stock
                if (int.Parse(txtQty.Text) > _qty)
                {
                    MessageBox.Show("Stock on hand quantity should be greater than adjustment quantity.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                    
                if (cboAction.Text == "Remove from inventory")
                {
                    dbcon.ExecuteQuery("UPDATE tbProducts SET qty = (qty - " + int.Parse(txtQty.Text) + ") WHERE pcode LIKE '" + lblPcode.Text + "'");
                }        
                else if (cboAction.Text == "Add to inventory")
                {
                    dbcon.ExecuteQuery("UPDATE tbProducts SET qty = (qty + " + int.Parse(txtQty.Text) + ") WHERE pcode LIKE '" + lblPcode.Text + "'");
                }

                dbcon.ExecuteQuery("INSERT INTO tbAdjustment(referenceno, pcode, qty, action, remarks, sdate, [user]) VALUES('" + lblRefno.Text + "' , '" + lblPcode.Text + "' , '" + int.Parse(txtQty.Text) + "' , '" + cboAction.Text + "' , '" + txtRemark.Text + "' , '" + DateTime.Now.ToShortDateString() + "' , '" + lblUsername.Text + "')");
                MessageBox.Show("Stock has been successfully adjusted.", "Process completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStock(); 
                Clear(); 
                btnSave.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
                
        }
    }
}
