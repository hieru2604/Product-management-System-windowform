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
    public partial class Dashboard : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        public Dashboard()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            lblDailysale.Text = dbcon.ExtractData("SELECT ISNULL(SUM(total),0) AS total FROM tbCart WHERE status LIKE 'sold' AND CAST(sdate as DATE)  = CAST(GETDATE() AS DATE)").ToString("#,##0.00"); 
            lblTotalProduct.Text = dbcon.ExtractData("SELECT COUNT(*) FROM tbProducts").ToString("#,##0"); 
            lblStock.Text = dbcon.ExtractData("SELECT ISNULL(SUM(qty), 0) AS qty FROM tbProducts").ToString("#,##0"); 
            lblCritical.Text = dbcon.ExtractData("SELECT COUNT(*) FROM vwCriticalItems").ToString("#,##0");
        }
    }
}
