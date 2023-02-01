using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace eApartments
{
    public partial class frmLogin : Form
    {
        public static frmLogin frmLoginInstance;
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnnection"].ConnectionString;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtLoginUsername.Text) || string.IsNullOrEmpty(txtLoginPassword.Text))
                {
                    MessageBox.Show("Missing Information !!!");
                }
                else
                {
                    string passPhrase = System.Configuration.ConfigurationManager.AppSettings["CryptographyPassPhrase"];
                    var dsUser = GetUserForLogin(txtLoginUsername.Text.Trim());
                    var dsUserRole = GetUserRoleForLogin(txtLoginUsername.Text.Trim());
                    var systemPW = dsUser.Tables[0].Rows[0]["Password"].ToString();
                    if (txtLoginPassword.Text.Trim() == Cryptography.Decrypt(systemPW, passPhrase))
                    {
                        LoginInfo.UserId =  (int)dsUser.Tables[0].Rows[0]["Id"];
                        LoginInfo.UserName = dsUser.Tables[0].Rows[0]["Name"].ToString();
                        LoginInfo.UserRole = dsUserRole.Tables[0].Rows[0]["Description"].ToString();

                        frmLoginInstance = this;

                        //Make sure I am kept hidden
                        WindowState = FormWindowState.Minimized;
                        ShowInTaskbar = false;
                        Visible = false;

                        //InitializeComponent();

                        //Open a managed form - the one the user sees..
                        var formMain = new frmMain();
                        formMain.Show();
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                
            }
        }

        private void btnLogin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new frmMain().Show();
        }

        public DataSet GetUserForLogin(string username)
        {
            SqlConnection con = new SqlConnection(connection);
            if (con.State != ConnectionState.Open)
                con.Open();
            string query = "SELECT * FROM [User] WHERE [Username] = '" + username + "'";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;
        }

        public DataSet GetUserRoleForLogin(string username)
        {
            SqlConnection con = new SqlConnection(connection);
            if (con.State != ConnectionState.Open)
                con.Open();
            string query = "SELECT [Role].* FROM [User] INNER JOIN [Role] ON [User].RoleId = [Role].Id WHERE [User].Username = '"+ username + "'";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;
        }

        private void picClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void metroBtnSignup_Click(object sender, System.EventArgs e)
        {
            frmLoginInstance = this;

            //Make sure I am kept hidden
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Visible = false;

            var formsignup = new frmSignup();
            formsignup.Show();
        }
    }
}
