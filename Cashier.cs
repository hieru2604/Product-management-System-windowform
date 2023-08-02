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
    public partial class Cashier : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        SqlDataReader dr;

        int qty;
        string id;
        string price;
        string stitle = "Point of Sales";
        public Cashier()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            GetTranNo();
            lblDate.Text = DateTime.Now.ToShortDateString();
        }

        private void picClose_Click(object sender, EventArgs e)         // X button
        {
            if (MessageBox.Show("Exit Application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        public void slide(Button button)            //scroll slider
        {
            panelSlide.BackColor = Color.White;
            panelSlide.Height = button.Height;
            panelSlide.Top = button.Top;
        }

        private void btnNTran_Click(object sender, EventArgs e)         //get new transaction number id
        {
            slide(btnNTran);
            GetTranNo();
        }

        private void btnSearch_Click(object sender, EventArgs e)        //open up LoockUpproduct form
        {
            slide(btnSearch);
            LookUpProduct lookUp = new LookUpProduct(this);
            lookUp.LoadProduct();
            lookUp.ShowDialog();
        }

        private void btnDiscount_Click(object sender, EventArgs e)      // open discount form
        {
            slide(btnDiscount);
            Discount discount = new Discount(this);
            discount.lblid.Text = id;
            discount.txtTotalPrice.Text = price;
            discount.ShowDialog();
        }

        private void btnSettle_Click(object sender, EventArgs e)        //open settle form
        {
            slide(btnSettle);
            Settle settle = new Settle(this);
            settle.txtSale.Text = lblDisplayTotal.Text;
            settle.ShowDialog();
        }

        private void btnClear_Click(object sender, EventArgs e)         //clear cashier cart
        {
            slide(btnClear);
            if (MessageBox.Show("Remove all items from cart?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new SqlCommand("Delete from tbCart where transno like'" + lblTranNo.Text + "'", cn);
                cm.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("All items has been successfully remove", "Remove item", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadCart();
            }             
        }

        private void btnDSales_Click(object sender, EventArgs e)            //open up DailySale form
        {
            slide(btnDSales);
            DailySale dailySale = new DailySale();
            dailySale.solduser = lblname.Text;
            dailySale.picClose.Visible = true;
            dailySale.lblTitle.Visible = true;
            dailySale.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)            //log out of cashier and show login form
        {
            slide(btnLogout);
            if(dgvCash.Rows.Count > 0)
            {
                MessageBox.Show("Unable to logout. Please cancel the transaction.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                return;

            }
            if (MessageBox.Show("Log out Application?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                Login login = new Login();
                login.ShowDialog();
            }
                
        }

        public void LoadCart()                                              // get data from current transaction to dgv
        {
            try
            {
                Boolean hasCart = false;
                int i = 0;
                double total = 0;
                double discount = 0;
                dgvCash.Rows.Clear();
                cn.Open();
                cm = new SqlCommand("SELECT c.id, c.pcode, p.pname, c.price, c.qty, c.disc, c.total FROM tbCart AS c INNER JOIN tbProducts AS p ON c.pcode = p.pcode WHERE c.transno LIKE @transno and c.status LIKE 'Pending' ", cn);
                cm.Parameters.AddWithValue("@transno", lblTranNo.Text);
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    total += Convert.ToDouble(dr["total"].ToString());
                    discount += Convert.ToDouble(dr["disc"].ToString());
                    dgvCash.Rows.Add(i, dr["id"].ToString(), dr["pcode"].ToString(), dr["pname"].ToString(), dr["price"].ToString(), dr["qty"].ToString(), double.Parse(dr["disc"].ToString()).ToString("#,##0.00"), double.Parse(dr["total"].ToString()).ToString("#,##0.00"));
                    hasCart = true;
                }
                dr.Close();
                cn.Close();
                lblSaleTotal.Text = total.ToString("#,##0.00");
                lblDiscount.Text = discount.ToString("#,##0.00");
                GetCartTotal();
                if (hasCart) { btnClear.Enabled = true; btnSettle.Enabled = true; btnDiscount.Enabled = true; }

                else { btnClear.Enabled = false; btnSettle.Enabled = false; btnDiscount.Enabled = false; }
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, stitle);
            }           
                
        }
        private void timer1_Tick(object sender, EventArgs e)                        //get current time clock
        {
            lblTimer.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        public void GetCartTotal()                                                  //get transaction total price
        {
            double discount = double.Parse(lblDiscount.Text);
            double sales = double.Parse(lblSaleTotal.Text) - discount;
            double vat = sales * 0.12;  //VAT: 12% of VAT Payable (Output Tax less Input Tax)
            double vatable = sales - vat;
            lblVat.Text = vat.ToString("#,##0.00");
            lblVatable.Text = vatable.ToString("#,##0.00"); 
            lblDisplayTotal.Text = sales.ToString("#,##0.00");
            
        }
        
        public void GetTranNo()                                         //set transaction number (Default is "yyyyMMdd####" with #### is the transaction numbers of that day)
        {
            try
            {
                string sdate = DateTime.Now.ToString("yyyyMMdd");
                int count;
                string transno;
                cn.Open();
                cm = new SqlCommand("SELECT TOP 1 transno FROM tbCart WHERE transno LIKE '" + sdate + "%' ORDER BY id desc", cn);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)                                         //get current transno from today data if not exist set to 1001
                {
                    transno = dr[0].ToString();
                    count = int.Parse(transno.Substring(8, 4)); 
                    lblTranNo.Text = sdate + (count + 1);
                }
                else                                                    
                {
                    transno = sdate + "1001";
                    lblTranNo.Text = transno;
                    
                }
                dr.Close();
                cn.Close();
            }
            catch(Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, stitle);
            }
        }



        private void txtPcode_TextChanged(object sender, EventArgs e)           //find product of the transaction when changing text
        {
            try
            {
                if (txtPcode.Text == string.Empty) return;
                else
                {
                    string _pcode;
                    double _price;
                    int _qty;
                    cn.Open();
                    cm = new SqlCommand("SELECT * FROM tbProducts WHERE pcode LIKE '" + txtPcode.Text + "'", cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        qty = int.Parse(dr["qty"].ToString());
                        _pcode = dr["pcode"].ToString();
                        _price = double.Parse(dr["price"].ToString());
                        _qty = int.Parse(txtQty.Text);
                        
                        dr.Close();
                        cn.Close();
                        //insert to tbCart
                        AddToCart(_pcode, _price, _qty);
                    }
                    dr.Close();
                    cn.Close();
                }                 
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, stitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void AddToCart(string _pcode, double _price, int _qty)              //Add transaction to database
        {
            try
            {
                string id = "";
                int cart_qty = 0;
                bool found = false;
                cn.Open();
                cm = new SqlCommand("Select * from tbCart Where transno = @transno and pcode = @pcode", cn);
                cm.Parameters.AddWithValue("@transno", lblTranNo.Text);
                cm.Parameters.AddWithValue("@pcode", _pcode);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    id = dr["id"].ToString();
                    cart_qty = int.Parse(dr["qty"].ToString());
                    found = true;   
                }                 
                else found = false;
                dr.Close();
                cn.Close();

                if (found)                                                          //check if cart isnt empty and if current stock is lower than transaction quantity
                {
                    if (qty < (int.Parse(txtQty.Text) + cart_qty))
                    {
                        MessageBox.Show("Unable to procced. Remaining quantity on hand is" + qty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }                       
                    cn.Open();
                    cm = new SqlCommand("Update tbCart set qty = (qty + " + _qty + ")Where id= '" + id + "'", cn);
                    cm.ExecuteReader();
                    cn.Close();
                    txtPcode.SelectionStart = 0;
                    txtPcode.SelectionLength = txtPcode.Text.Length;
                    LoadCart();
                }

                else
                {
                    if (qty < (int.Parse(txtQty.Text) + cart_qty))
                    {
                        MessageBox.Show("Unable to procced. Remaining qty on hand is" + qty, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    cn.Open();
                    cm = new SqlCommand("INSERT INTO tbCart(transno, pcode, price, qty, sdate, cashier) VALUES (@transno, @pcode, @price, @qty, @sdate, @cashier)", cn);
                    cm.Parameters.AddWithValue("@transno", lblTranNo.Text);
                    cm.Parameters.AddWithValue("@pcode", _pcode);
                    cm.Parameters.AddWithValue("@price", _price);
                    cm.Parameters.AddWithValue("@qty", _qty);
                    cm.Parameters.AddWithValue("@sdate", DateTime.Now);
                    cm.Parameters.AddWithValue("@cashier", lblUsername.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadCart();
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, stitle);
            }

        }

        private void dgvCash_SelectionChanged(object sender, EventArgs e)                   //select item inside dgv
        {
            int i = dgvCash.CurrentRow.Index;
            id = dgvCash[1, i].Value.ToString();
            price = dgvCash[7, i].Value.ToString();
        }

        private void dgvCash_CellContentClick(object sender, DataGridViewCellEventArgs e)   //inner dgv button interact
        {
            string colName = dgvCash.Columns[e.ColumnIndex].Name;
       
            if (colName == "Delete")
            {
                if (MessageBox.Show("Remove this item", "Remove item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dbcon.ExecuteQuery("Delete from tbCart where id like '" + dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    MessageBox.Show("Items has been successfully remove", "Remove item", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCart();
                }                                 
            }               
            else if (colName == "colAdd")
            {
                int i = 0;
                cn.Open();
                cm = new SqlCommand("SELECT SUM(qty) as qty FROM tbProducts WHERE pcode LIKE '" + dgvCash.Rows[e.RowIndex].Cells[2].Value.ToString() + "' GROUP BY pcode", cn);
                i = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();
                if (int.Parse(dgvCash.Rows[e.RowIndex].Cells[5].Value.ToString()) < i)
                {
                    dbcon.ExecuteQuery("UPDATE tbCart SET qty = qty + " + int.Parse(txtQty.Text) + "WHERE transno LIKE '" + lblTranNo.Text + "' AND pcode LIKE '" + dgvCash.Rows[e.RowIndex].Cells[2].Value.ToString() + "'");
                    LoadCart();
                }
                else
                {
                    MessageBox.Show("Remaining qty on hand is " + i + "!", "Out of stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                   
            }
            else if (colName == "colReduce")
            {
                int i = 0;
                cn.Open();
                cm = new SqlCommand("SELECT SUM(qty) as qty FROM tbCart WHERE pcode LIKE '" + dgvCash.Rows[e.RowIndex].Cells[2].Value.ToString() + "' GROUP BY pcode", cn);
                i = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();

                if (i > 1)
                {
                    dbcon.ExecuteQuery("UPDATE tbCart SET qty = qty - " + int.Parse(txtQty.Text) + "WHERE transno LIKE '" + lblTranNo.Text + "' AND pcode LIKE '" + dgvCash.Rows[e.RowIndex].Cells[2].Value.ToString() + "'");
                    LoadCart();
                }
                else
                {
                    MessageBox.Show("Remaining qty on hand is " + i + "!", "Out of stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }
    }
}
