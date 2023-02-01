using System.Windows.Forms;

namespace eApartments
{
    public partial class frmLogin : Form
    {
        public static frmLogin frmLoginInstance;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, System.EventArgs e)
        {
            frmLoginInstance = this;

            //Make sure I am kept hidden
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Visible = false;

            InitializeComponent();

            //Open a managed form - the one the user sees..
            var formMain = new frmMain();
            formMain.Show();
        }

        private void btnLogin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new frmMain().Show();
        }

        private void picClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
