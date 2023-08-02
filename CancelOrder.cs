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
    public partial class CancelOrder : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        SqlDataReader dr;
        DailySale dailySale;
        public CancelOrder(DailySale daily)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            dailySale = daily;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {                  
            try
            {
                if (cboInventory.Text != string.Empty && udCancelQty.Value > 0 && txtReason.Text != string.Empty)
                {
                    if (int.Parse(txtQty.Text) >= udCancelQty.Value)
                    {
                        try
                        {
                            cancelTB();

                            cn.Open();
                            cm = new SqlCommand("UPDATE tbCart SET qty = qty - @qty where id LIKE @id ", cn);
                            cm.Parameters.AddWithValue("@qty", udCancelQty.Value);
                            cm.Parameters.AddWithValue("@id", lblCartId.Text);
                            cm.ExecuteNonQuery();
                            cn.Close();

                            if (cboInventory.Text == "Yes")
                            {
                                cn.Open();
                                cm = new SqlCommand("UPDATE tbProducts SET qty = qty + " + udCancelQty.Value + " where pcode= '" + txtId.Text + "'", cn);
                                cm.ExecuteNonQuery();
                                cn.Close();
                            }

                            MessageBox.Show("Order transaction successfully cancelled!", "Cancel Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dailySale.LoadCashier();
                            dailySale.LoadSold();
                            this.Dispose();
                            return;
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancel quantity must be lower than current quantity!", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }                   
                }
                else
                {
                    MessageBox.Show("Please fill in the information!", "info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

           
        }

        private void cboInventory_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cancelTB()
        {
            cn.Open();
            cm = new SqlCommand("insert into tbCancel (transno, pcode, price, qty, total, sdate, voidby, cancelledby, reason, action) values (@transno, @pcode, @price, @qty , @total , @sdate, @voidby, @cancelledby, @reason, @action)", cn);
            cm.Parameters.AddWithValue("@transno", txtTransno.Text);
            cm.Parameters.AddWithValue("@pcode", txtId.Text);
            cm.Parameters.AddWithValue("@price", double.Parse(txtPrice.Text));
            cm.Parameters.AddWithValue("@qty", int.Parse(txtQty.Text));
            cm.Parameters.AddWithValue("@total", double.Parse(txtTotal.Text));
            cm.Parameters.AddWithValue("@sdate", DateTime.Now);
            cm.Parameters.AddWithValue("@voidby", txtVoidBy.Text);
            cm.Parameters.AddWithValue("@cancelledby", txtCancelBy.Text);
            cm.Parameters.AddWithValue("@reason", txtReason.Text);
            cm.Parameters.AddWithValue("@action", cboInventory.Text);
            cm.ExecuteNonQuery();
            cn.Close();
        }
    }
}
