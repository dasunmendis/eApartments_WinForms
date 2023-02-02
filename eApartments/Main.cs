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
        int keydgvLeases = 0;
        public static int metroCmbApartmentSelectedVal;
        public static string cmbStatusForLeaseFilterSelectedVal;
        public static string cmbStatusForLeaseSelectedVal;
        public static int cmbApartmentForLeaseSelectedVal;

        public frmMain()
        {
            InitializeComponent();

            //this.metroTabControl1.SelectedIndexChanged += new System.EventHandler(this.metroTabControl1_SelectedIndexChanged);
        }

        private void frmMain_Load(object sender, System.EventArgs e)
        {
            lblLoggedUser.Text = LoginInfo.UserName + " (" + LoginInfo.UserRole + ")";
            LoadTenants();
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
            if (LoginInfo.UserRole.ToLower() == "admin")
            {
                if (metroTabControl1.SelectedTab.Name == "metroTabTenants")
                {
                    metroTabControl1.SelectedTab = metroTabTenants;
                    ClearFields();
                    LoadTenants();
                }
                else if (metroTabControl1.SelectedTab.Name == "metroTabApartments")
                {
                    metroTabControl1.SelectedTab = metroTabApartments;
                    LoadApartments();
                }
                else if (metroTabControl1.SelectedTab == metroTabLeases)
                {
                    LoadLeases();
                    LoadStatus();
                }
                else if (metroTabControl1.SelectedTab == metroTabReports)
                {
                    metroTabControl1.SelectedTab = metroTabReports;
                    LoadStatusReport();
                }
            }
            else if (LoginInfo.UserRole.ToLower() == "manager")
            {
                if (metroTabControl1.SelectedTab == metroTabApartments)
                {
                    metroTabControl1.SelectedTab = metroTabApartments;
                    LoadApartments();
                }
                else if (metroTabControl1.SelectedTab == metroTabLeases)
                {
                    LoadLeases();
                    LoadStatus();
                }
                else if (metroTabControl1.SelectedTab == metroTabReports)
                {
                    metroTabControl1.SelectedTab = metroTabReports;
                    LoadStatusReport();
                }
            }
            else if (LoginInfo.UserRole.ToLower() == "user")
            {
                if (metroTabControl1.SelectedTab == metroTabCategories)
                {
                    metroTabControl1.SelectedTab = metroTabCategories;
                }
                else if (metroTabControl1.SelectedTab == metroTabApartments)
                {
                    metroTabControl1.SelectedTab = metroTabApartments;
                    LoadApartments();
                }
                else
                {
                    MessageBox.Show("Unable to load tab. You have insufficient access privileges.");
                    metroTabControl1.SelectedTab = metroTabCategories;
                }
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
            metroCmbApartmentSelectedVal = selectedValue;
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
            try
            {
                if (metroCmbApartments.SelectedIndex == 0)
                {
                    MessageBox.Show("Select Apartment !!!");
                }
                else
                {
                    var startDate = DateTime.Now.AddDays(7);

                    SqlConnection con = new SqlConnection(connection);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    string query = "INSERT INTO [dbo].[LeaseDetail]" +
                                        "([ApartmentId]" +
                                        ",[UserId]" +
                                        ",[Status]" +
                                        ",[MonthlyRental]" +
                                        ",[RefundableDeposit]" +
                                        ",[InitialPayment]" +
                                        ",[Period]" +
                                        ",[StartDate]" +
                                        ",[CloseDate]" +
                                        ",[DateCreated]" +
                                        ",[DateModified])" +
                                     "VALUES" +
                                        "(@ApartmentId" +
                                        ",@UserId" +
                                        ",@Status" +
                                        ",@MonthlyRental" +
                                        ",@RefundableDeposit" +
                                        ",@InitialPayment" +
                                        ",@Period" +
                                        ",@StartDate" +
                                        ",@CloseDate" +
                                        ",@DateCreated" +
                                        ",@DateModified)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ApartmentId", metroCmbApartmentSelectedVal);
                    cmd.Parameters.AddWithValue("@UserId", LoginInfo.UserId);
                    cmd.Parameters.AddWithValue("@Status", "LeaseRequested");
                    cmd.Parameters.AddWithValue("@MonthlyRental", 75000);
                    cmd.Parameters.AddWithValue("@RefundableDeposit", 500000);
                    cmd.Parameters.AddWithValue("@InitialPayment", 0);
                    cmd.Parameters.AddWithValue("@Period", 6);
                    cmd.Parameters.AddWithValue("@StartDate", DateTime.Now.AddDays(7));
                    cmd.Parameters.AddWithValue("@CloseDate", DateTime.Now.AddDays(7).AddMonths(6));
                    cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Lease Requested !!!");
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadLeases()
        {
            SqlConnection con = new SqlConnection(connection);
            if (con.State != ConnectionState.Open)
                con.Open();
            string query = "select * from LeaseDetail";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            var ds = new DataSet();
            sda.Fill(ds);
            dgvLeases.DataSource = ds.Tables[0];
            con.Close();
        }

        public void LoadStatus()
        {
            cmbStatusForLeaseFilter.Items.Add(" - - ");
            cmbStatusForLeaseFilter.Items.Add("Active");
            cmbStatusForLeaseFilter.Items.Add("Available");
            cmbStatusForLeaseFilter.Items.Add("ReserveRequested");
            cmbStatusForLeaseFilter.Items.Add("Reserved");
            cmbStatusForLeaseFilter.Items.Add("LeaseRequested");
            cmbStatusForLeaseFilter.Items.Add("Leased");
            cmbStatusForLeaseFilter.Items.Add("Queued");
            cmbStatusForLeaseFilter.Items.Add("Pending");

            cmbStatusForLeaseFilter.SelectedIndex = 0;
        }

        public void FilterLeases(string status, DateTime fromDate, DateTime toDate)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                if (con.State != ConnectionState.Open)
                    con.Open();
                string query = string.Empty;
                if (cmbStatusForLeaseFilter.SelectedIndex == 0)
                    query = "select * from LeaseDetail Where [StartDate] >= '" + fromDate + "' and [CloseDate] <= '" + toDate + "'";
                else
                    query = "select * from LeaseDetail Where [Status] = '" + status + "' and [StartDate] >= '" + fromDate + "' and [CloseDate] <= '" + toDate + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                var ds = new DataSet();
                sda.Fill(ds);
                dgvLeases.DataSource = ds.Tables[0];
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void metroBtnRequestReserve_Click(object sender, EventArgs e)
        {
            try
            {
                if (metroCmbApartments.SelectedIndex == 0)
                {
                    MessageBox.Show("Select Apartment !!!");
                }
                else
                {
                    SqlConnection con = new SqlConnection(connection);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    string query = "INSERT INTO [dbo].[LeaseDetail]" +
                                        "([ApartmentId]" +
                                        ",[UserId]" +
                                        ",[Status]" +
                                        ",[MonthlyRental]" +
                                        ",[RefundableDeposit]" +
                                        ",[InitialPayment]" +
                                        ",[Period]" +
                                        ",[StartDate]" +
                                        ",[CloseDate]" +
                                        ",[DateCreated]" +
                                        ",[DateModified])" +
                                     "VALUES" +
                                        "(@ApartmentId" +
                                        ",@UserId" +
                                        ",@Status" +
                                        ",@MonthlyRental" +
                                        ",@RefundableDeposit" +
                                        ",@InitialPayment" +
                                        ",@Period" +
                                        ",@StartDate" +
                                        ",@CloseDate" +
                                        ",@DateCreated" +
                                        ",@DateModified)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ApartmentId", metroCmbApartmentSelectedVal);
                    cmd.Parameters.AddWithValue("@UserId", LoginInfo.UserId);
                    cmd.Parameters.AddWithValue("@Status", "ReserveRequested");
                    cmd.Parameters.AddWithValue("@MonthlyRental", 0);
                    cmd.Parameters.AddWithValue("@RefundableDeposit", 0);
                    cmd.Parameters.AddWithValue("@InitialPayment", 0);
                    cmd.Parameters.AddWithValue("@Period", 0);
                    cmd.Parameters.AddWithValue("@StartDate", null);
                    cmd.Parameters.AddWithValue("@CloseDate", null);
                    cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Reserve Requested !!!");
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void metroBtnWaitingList_Click(object sender, EventArgs e)
        {
            try
            {
                if (metroCmbApartments.SelectedIndex == 0)
                {
                    MessageBox.Show("Select Apartment !!!");
                }
                else
                {
                    SqlConnection con = new SqlConnection(connection);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    string query = "INSERT INTO [dbo].[LeaseDetail]" +
                                        "([ApartmentId]" +
                                        ",[UserId]" +
                                        ",[Status]" +
                                        ",[MonthlyRental]" +
                                        ",[RefundableDeposit]" +
                                        ",[InitialPayment]" +
                                        ",[Period]" +
                                        ",[StartDate]" +
                                        ",[CloseDate]" +
                                        ",[DateCreated]" +
                                        ",[DateModified])" +
                                     "VALUES" +
                                        "(@ApartmentId" +
                                        ",@UserId" +
                                        ",@Status" +
                                        ",@MonthlyRental" +
                                        ",@RefundableDeposit" +
                                        ",@InitialPayment" +
                                        ",@Period" +
                                        ",@StartDate" +
                                        ",@CloseDate" +
                                        ",@DateCreated" +
                                        ",@DateModified)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ApartmentId", metroCmbApartmentSelectedVal);
                    cmd.Parameters.AddWithValue("@UserId", LoginInfo.UserId);
                    cmd.Parameters.AddWithValue("@Status", "Queued");
                    cmd.Parameters.AddWithValue("@MonthlyRental", 0);
                    cmd.Parameters.AddWithValue("@RefundableDeposit", 0);
                    cmd.Parameters.AddWithValue("@InitialPayment", 0);
                    cmd.Parameters.AddWithValue("@Period", 0);
                    cmd.Parameters.AddWithValue("@StartDate", null);
                    cmd.Parameters.AddWithValue("@CloseDate", null);
                    cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Reserve Requested !!!");
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnFilterLease_Click(object sender, EventArgs e)
        {
            DateTime dtFrom = this.dtpPeriodForLeaseFrom.Value.Date;
            DateTime dtTo = this.dtpPeriodForLeaseTo.Value.Date;
            FilterLeases(cmbStatusForLeaseFilterSelectedVal, dtFrom, dtTo);
        }

        private void cmbStatusForLeaseFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = cmbStatusForLeaseFilter.SelectedItem;
            cmbStatusForLeaseFilterSelectedVal = selectedItem.ToString();
        }

        private void dgvLeases_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            cmbStatusForLease.Items.Add(" - - ");
            cmbStatusForLease.Items.Add("Active");
            cmbStatusForLease.Items.Add("Available");
            cmbStatusForLease.Items.Add("ReserveRequested");
            cmbStatusForLease.Items.Add("Reserved");
            cmbStatusForLease.Items.Add("LeaseRequested");
            cmbStatusForLease.Items.Add("Leased");
            cmbStatusForLease.Items.Add("Queued");
            cmbStatusForLease.Items.Add("Pending");

            LoadApartmentForLease();

            cmbApartmentForLease.SelectedValue = dgvLeases.Rows[e.RowIndex].Cells[1].Value.ToString();
            cmbStatusForLease.SelectedItem = dgvLeases.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtRentalForLease.Text = dgvLeases.Rows[e.RowIndex].Cells[4].Value.ToString();
            txtDepositForLease.Text = dgvLeases.Rows[e.RowIndex].Cells[5].Value.ToString();
            txtInitialPayForLease.Text = dgvLeases.Rows[e.RowIndex].Cells[6].Value.ToString();

            keydgvLeases = Convert.ToInt32(dgvLeases.Rows[e.RowIndex].Cells[0].Value.ToString());
        }

        private void btnValidateLease_Click(object sender, EventArgs e)
        {
            if (cmbStatusForLease.SelectedIndex == 0)
                MessageBox.Show("Missing Information !!!");
            else
            {
                try
                {
                    SqlConnection con = new SqlConnection(connection);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    var query = "update [LeaseDetail] " +
                                "set " +
                                    "[Status] = @Status" +
                                    ",[MonthlyRental] = @MonthlyRental" +
                                    ",[RefundableDeposit] = @RefundableDeposit" +
                                    ",[InitialPayment] = @InitialPayment" +
                                    ",[DateModified] = @DateModified" +
                                " where Id = @Key";
                    SqlCommand cmd = new SqlCommand(query, con) { CommandType = CommandType.Text };
                    cmd.Parameters.AddWithValue("@Status", cmbStatusForLease.SelectedItem);
                    cmd.Parameters.AddWithValue("@MonthlyRental", txtRentalForLease.Text);
                    cmd.Parameters.AddWithValue("@RefundableDeposit", txtDepositForLease.Text);
                    cmd.Parameters.AddWithValue("@InitialPayment", txtInitialPayForLease.Text);
                    cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Key", keydgvLeases);
                    var result = cmd.ExecuteNonQuery();
                    con.Close();

                    UpdateApartmentStatus();
                    if (result == 1)
                        MessageBox.Show("Lease Updated !!!");

                    ClearLeaseFields();
                    LoadLeases();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void UpdateApartmentStatus()
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                if (con.State != ConnectionState.Open)
                    con.Open();
                //var query2 = "update [Apartment] " +
                //            "set " +
                //                "[Status] = @Statuss" +
                //                ",[DateModified] = @DateModifiedd" +
                //            " where Id = @Keyy";
                var query3 = "update [Apartment] set [Status] = '" + cmbStatusForLease.SelectedItem + "' ,[DateModified] = '" + DateTime.Now + "' where Id = '" + cmbApartmentForLeaseSelectedVal + "'";
                SqlCommand cmd = new SqlCommand(query3, con) { CommandType = CommandType.Text };
                //cmd.Parameters.AddWithValue("@Statuss", cmbStatusForLease.SelectedItem);
                //cmd.Parameters.AddWithValue("@DateModifiedd", DateTime.Now);
                //cmd.Parameters.AddWithValue("@Keyy", cmbApartmentForLeaseSelectedVal);
                var result2 = cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cmbStatusForLease_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = cmbStatusForLease.SelectedItem;
            cmbStatusForLeaseSelectedVal = selectedItem.ToString();
        }

        private void cmbApartmentForLease_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedItem = cmbApartmentForLease.SelectedItem;
            DataRow row = ((DataRowView)selectedItem).Row;
            var selectedValue = Convert.ToInt32(row[0].ToString());
            cmbApartmentForLeaseSelectedVal = selectedValue;
            //try
            //{
            //    SqlConnection con = new SqlConnection(connection);
            //    if (con.State != ConnectionState.Open)
            //        con.Open();
            //    string query = "select * from Apartment where Id = @Key";
            //    SqlCommand command = new SqlCommand(query, con) { CommandType = CommandType.Text };
            //    SqlDataReader sReader;
            //    command.Parameters.Clear();
            //    command.Parameters.AddWithValue("@Key", selectedValue);
            //    sReader = command.ExecuteReader();

            //    LoadCategories();
            //    while (sReader.Read())
            //    {
            //        txtApartmentNo.Text = sReader["Number"].ToString();
            //        txtApartmentName.Text = sReader["Name"].ToString();
            //        txtApartmentBuilding.Text = sReader["Building"].ToString();
            //        cmbApartmentType.SelectedValue = sReader["CategoryId"].ToString();
            //        txtApartmentLeaseDuration.Text = sReader["LeaseDuration"].ToString();
            //        txtApartmentStatus.Text = sReader["Status"].ToString();
            //    }
            //    con.Close();



            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            //LoadApartmentForLease();
        }

        public void LoadApartmentForLease()
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
                cmbApartmentForLease.DataSource = ds.Tables[0];
                cmbApartmentForLease.DisplayMember = "Name";
                cmbApartmentForLease.ValueMember = "Id";
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClearLeaseFields()
        {
            cmbStatusForLease.SelectedIndex = 0;
            txtRentalForLease.Text = string.Empty;
            txtDepositForLease.Text = string.Empty;
            txtInitialPayForLease.Text = string.Empty;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            DateTime dtFrom = this.dateTimePicker2.Value.Date;
            DateTime dtTo = this.dateTimePicker1.Value.Date;
            FilterLeasesForReport(cmbStatusForLeaseFilterSelectedVal, dtFrom, dtTo);
        }

        public void FilterLeasesForReport(string status, DateTime fromDate, DateTime toDate)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                if (con.State != ConnectionState.Open)
                    con.Open();
                string query = string.Empty;
                if (comboBox1.SelectedIndex == 0)
                    query = "select * from LeaseDetail Where [StartDate] >= '" + fromDate + "' and [CloseDate] <= '" + toDate + "'";
                else
                    query = "select * from LeaseDetail Where [Status] = '" + status + "' and [StartDate] >= '" + fromDate + "' and [CloseDate] <= '" + toDate + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                var ds = new DataSet();
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadStatusReport()
        {
            comboBox1.Items.Add(" - - ");
            comboBox1.Items.Add("Active");
            comboBox1.Items.Add("Available");
            comboBox1.Items.Add("ReserveRequested");
            comboBox1.Items.Add("Reserved");
            comboBox1.Items.Add("LeaseRequested");
            comboBox1.Items.Add("Leased");
            comboBox1.Items.Add("Queued");
            comboBox1.Items.Add("Pending");

            comboBox1.SelectedIndex = 0;
        }
    }
}
