using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace eApartments
{
    public partial class frmMain : Form
    {
        public static frmMain frmMainInstance;
        SqlConnection con;
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnnection"].ConnectionString;
        int key = 0;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, System.EventArgs e)
        {
            metroTabCategories.Hide();
            //LoadTenants();
        }

        private void linkLogout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmMainInstance = this;

            //Make sure I am kept hidden
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Visible = false;

            InitializeComponent();

            //Open a managed form - the one the user sees..
            var formLogin = new frmLogin();
            formLogin.Show();
        }

        private void picAddCat_Click(object sender, System.EventArgs e)
        {

            if (string.IsNullOrEmpty(txtNAme.Text) || string.IsNullOrEmpty(txtAddress.Text) || string.IsNullOrEmpty(txtNIC.Text) || string.IsNullOrEmpty(txtContactNo.Text) || string.IsNullOrEmpty(txtServantName.Text))
            {
                MessageBox.Show("Missing Information !!!");
            }
            else
            {
                SqlConnection con = new SqlConnection(connection);
                if (con.State != ConnectionState.Open)
                    con.Open();
                string query = "INSERT INTO [dbo].[Tenant]" +
                                    "([Name]" +
                                    ",[Address]" +
                                    ",[NIC]" +
                                    ",[ContactNo]" +
                                    ",[ServantName]" +
                                    ",[DateCreated]" +
                                    ",[DateModified])" +
                                 "VALUES" +
                                    "(@Name" +
                                    ",@Address" +
                                    ",@NIC" +
                                    ",@ContactNo" +
                                    ",@ServantName" +
                                    ",@DateCreated" +
                                    ",@DateModified)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", txtNAme.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@NIC", txtNIC.Text);
                cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                cmd.Parameters.AddWithValue("@ServantName", txtServantName.Text);
                cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Tenant Added !!!");
                con.Close();
                LoadTenants();
            }
            ClearFields();
        }


        public void LoadTenants()
        {
            SqlConnection con = new SqlConnection(connection);
            if (con.State != ConnectionState.Open)
                con.Open();
            string query = "select * from Tenant";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            dgvTenants.DataSource = ds.Tables[0];
            con.Close();
        }

        public void ClearFields()
        {
            txtNAme.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtNIC.Text = string.Empty;
            txtContactNo.Text = string.Empty;
            txtServantName.Text = string.Empty;
        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroTabControl1.SelectedTab.Name == "metroTabTenants")
            {
                ClearFields();
                LoadTenants();
            }
        }

        private void dgvTenants_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtNAme.Text = dgvTenants.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtAddress.Text = dgvTenants.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtNIC.Text = dgvTenants.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtContactNo.Text = dgvTenants.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtServantName.Text = dgvTenants.Rows[e.RowIndex].Cells[5].Value.ToString();
            if (string.IsNullOrEmpty(txtNAme.Text))
                key = 0;
            else
                key = Convert.ToInt32(dgvTenants.Rows[e.RowIndex].Cells[0].Value.ToString());
        }

        private void picEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNAme.Text) || string.IsNullOrEmpty(txtAddress.Text) || string.IsNullOrEmpty(txtNIC.Text) || string.IsNullOrEmpty(txtContactNo.Text) || string.IsNullOrEmpty(txtServantName.Text))
                MessageBox.Show("Missing Information !!!");
            else
            {
                try
                {
                    SqlConnection con = new SqlConnection(connection);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    var query = "update Tenant " +
                                "set " +
                                    "[Name] = @Name" +
                                    ",[Address] = @Address" +
                                    ",[NIC] = @NIC" +
                                    ",[ContactNo] = @ContactNo" +
                                    ",[ServantName] = @ServantName" +
                                    ",[DateModified] = @DateModified" +
                                " where Id = @Key";
                    SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text };
                    cmd.Parameters.AddWithValue("@Name", txtNAme.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@NIC", txtNIC.Text);
                    cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                    cmd.Parameters.AddWithValue("@ServantName", txtServantName.Text);
                    cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Key", key);
                    var result = cmd.ExecuteNonQuery();
                    if (result == 1)
                        MessageBox.Show("Tenant Updated !!!");

                    ClearFields();
                    LoadTenants();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void picDelete_Click(object sender, EventArgs e)
        {
            if (key == 0)
                MessageBox.Show("Select a Tenant !!!");
            else
            {
                try
                {
                    SqlConnection con = new SqlConnection(connection);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    var query = "delete from Tenant where Id = @Key";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Key", key);
                    var result = cmd.ExecuteNonQuery();
                    if (result == 1)
                        MessageBox.Show("Tenant Deleted !!!");

                    ClearFields();
                    LoadTenants();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

       
    }
}
