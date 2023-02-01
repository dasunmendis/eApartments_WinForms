using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace eApartments
{
    public partial class frmMain : Form
    {
        public static frmMain frmMainInstance;
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnnection"].ConnectionString;
        int key = 0;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, System.EventArgs e)
        {
            metroTabCategories.Hide();

            lblLoggedUser.Text = LoginInfo.UserName + " (" + LoginInfo.UserRole + ")";
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
            else if (metroTabControl1.SelectedTab.Name == "metroTabApartments")
            {
                LoadApartments();
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

        public void LoadApartments()
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                if (con.State != ConnectionState.Open)
                    con.Open();
                string query = "select * from Apartment";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                var ds = new DataSet();
                sda.Fill(ds);
                metroCmbApartments.DataSource = ds.Tables[0];
                metroCmbApartments.DisplayMember = "Name";
                metroCmbApartments.ValueMember = "Id";
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            //var list = (from row in ds.Tables[0].AsEnumerable() select Convert.ToString(row["Name"]))
            //return ds.Tables[0].AsEnumerable().Select(x => x[2].ToString());
        }

        private void metroCmbApartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = metroCmbApartments.SelectedItem;
            DataRow row = ((DataRowView)selectedItem).Row;
            var selectedValue = Convert.ToInt32(row[0].ToString());
            try
            {
                SqlConnection con = new SqlConnection(connection);
                if (con.State != ConnectionState.Open)
                    con.Open();
                string query = "select * from Apartment where Id = @Key";
                SqlCommand command = new SqlCommand(query, con) { CommandType = CommandType.Text };
                SqlDataReader sReader;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Key", selectedValue);
                sReader = command.ExecuteReader();

                LoadCategories();
                while (sReader.Read())
                {
                    txtApartmentNo.Text = sReader["Number"].ToString();
                    txtApartmentName.Text = sReader["Name"].ToString();
                    txtApartmentBuilding.Text = sReader["Building"].ToString();
                    cmbApartmentType.SelectedValue = sReader["CategoryId"].ToString();
                    txtApartmentLeaseDuration.Text = sReader["LeaseDuration"].ToString();
                    txtApartmentStatus.Text = sReader["Status"].ToString();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadCategories()
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                if (con.State != ConnectionState.Open)
                    con.Open();
                string query = "select * from Category";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                var ds = new DataSet();
                sda.Fill(ds);
                cmbApartmentType.DataSource = ds.Tables[0];
                cmbApartmentType.DisplayMember = "Description";
                cmbApartmentType.ValueMember = "Id";
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void metroBtnRequestLease_Click(object sender, EventArgs e)
        {

        }

        private void metroBtnRequestReserve_Click(object sender, EventArgs e)
        {

        }

        private void metroBtnWaitingList_Click(object sender, EventArgs e)
        {

        }
    }
}
