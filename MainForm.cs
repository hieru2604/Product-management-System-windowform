using System;
using System.Windows.Forms;
using System.Data.SqlClient;
//hi
namespace LaptopDemo
{
    
    public partial class MainForm : Form
    {
        SqlConnection cn = new SqlConnection();             //using data to get the username
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        public string _pass;                                //password for user setting
        public MainForm()
        {
            InitializeComponent();
            customizeDesign();
            cn = new SqlConnection(dbcon.myConnection());
            cn.Open();

        }

        private Form activeForm = null;
        public void openChildForm(Form childForm)                  //switching child form inside parent form 
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill; 
            lblTitle.Text = childForm.Text;
            panelMain.Controls.Add(childForm);
            panelMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void customizeDesign() {                            //set main panel to close whenever form open
            panelSubProduct.Visible = false;
            panelSubRecord.Visible = false;
            panelSubStock.Visible = false;
            panelSubSetting.Visible = false;
        }

        private void hideSubMenu()                                  //child panel settings
        { 
            if (panelSubProduct.Visible == true)
                panelSubProduct.Visible = false;
            if (panelSubRecord.Visible == true)
                panelSubRecord.Visible = false;
            if (panelSubStock.Visible == true)
                panelSubStock.Visible = false;
            if (panelSubSetting.Visible == true)
                panelSubSetting.Visible = false;
        }

    private void showSubMenu(Panel submenu)                         //open child panels
    {
        if (submenu.Visible == false)
        {
            hideSubMenu();
            submenu.Visible = true;
        }
        else
            submenu.Visible = false;
    }



        private void btnDashboard_Click(object sender, EventArgs e)
        {
            openChildForm(new Dashboard());
            hideSubMenu();
        }

        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {
            openChildForm(new Adjustments(this));
            hideSubMenu();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            openChildForm(new Supplier());
            hideSubMenu();
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            openChildForm(new UserAccount(this));
            hideSubMenu();
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            showSubMenu(panelSubProduct);
        }

        private void btnProductList_Click(object sender, EventArgs e)
        {
            openChildForm(new Product());
            hideSubMenu();
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            openChildForm(new Category());
            hideSubMenu();
        }

        private void btnBrand_Click(object sender, EventArgs e)
        {
            openChildForm(new Brand());
            hideSubMenu();
        }

        private void btnInStock_Click(object sender, EventArgs e)
        {
            showSubMenu(panelSubStock);
        }

        private void btnStockEntry_Click(object sender, EventArgs e)
        {
            openChildForm(new StockIn());
            hideSubMenu();
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            showSubMenu(panelSubRecord);
        }

        private void btnSaleHistory_Click(object sender, EventArgs e)
        {
            openChildForm(new DailySale());
            hideSubMenu();
        }

        private void btnPOSRecord_Click(object sender, EventArgs e)
        {
            openChildForm(new Record());
            hideSubMenu();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            showSubMenu(panelSubSetting);
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            Store store = new Store();
            store.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            if (MessageBox.Show("Log out Application?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                Login login = new Login();
                login.ShowDialog();
            }               
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            btnDashboard.PerformClick();
        }
    }
}
