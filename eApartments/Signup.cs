using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace eApartments
{
    public partial class frmSignup : Form
    {
        public static frmSignup frmSignupInstance;
        string connection = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnnection"].ConnectionString;
        public frmSignup()
        {
            InitializeComponent();
        }

        private void btnSignupSubmit_Click(object sender, EventArgs e)
        {
            string passPhrase = System.Configuration.ConfigurationManager.AppSettings["CryptographyPassPhrase"];

            if (string.IsNullOrEmpty(txtSignupUsername.Text) || string.IsNullOrEmpty(txtSignupPassword.Text) || string.IsNullOrEmpty(txtSignupEmail.Text))
                MessageBox.Show("Please fill data !!!");
            else
            {
                var encryptedPassword = Cryptography.Encrypt(txtSignupPassword.Text, passPhrase);
                var roleId = GetUserRolesByRole("User").Tables[0].Rows[0]["Id"];
                try
                {
                    SqlConnection con = new SqlConnection(connection);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    string query = "INSERT INTO [dbo].[User]" +
                                        "([Name]" +
                                        ",[Username]" +
                                        ",[Password]" +
                                        ",[RoleId]" +
                                        ",[Email]" +
                                        ",[DateCreated]" +
                                        ",[DateModified])" +
                                     "VALUES" +
                                        "(@Name" +
                                        ",@Username" +
                                        ",@Password" +
                                        ",@RoleId" +
                                        ",@Email" +
                                        ",@DateCreated" +
                                        ",@DateModified)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", txtSignupName.Text);
                    cmd.Parameters.AddWithValue("@Username", txtSignupUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", encryptedPassword);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.Parameters.AddWithValue("@Email", txtSignupEmail.Text);
                    cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DateModified", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User Added !!!");
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    frmSignupInstance = this;

                    WindowState = FormWindowState.Minimized;
                    ShowInTaskbar = false;
                    Visible = false;

                    var formLogin = new frmLogin();
                    formLogin.Show();
                }
            }
        }

        public DataSet GetUserRolesByRole(string roleDesc)
        {
            SqlConnection con = new SqlConnection(connection);
            if (con.State != ConnectionState.Open)
                con.Open();
            string query = "SELECT * FROM [Role] WHERE [Description] = '" + roleDesc +"'";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;
        }

        private void btnBackToLogin_Click(object sender, EventArgs e)
        {
            frmSignupInstance = this;

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Visible = false;

            var formLogin = new frmLogin();
            formLogin.Show();
        }
    }
}
