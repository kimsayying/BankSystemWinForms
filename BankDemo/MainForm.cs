using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BankDemo
{
    public partial class MainForm : Form
    {
        string connString = @"Data Source=KSY\SQLEXPRESS;Initial Catalog=BankDemo;Integrated Security=True;
                           Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection conn;
        SqlCommand command;
        SqlDataAdapter dataAdapter;
        DataTable table;
        string hashPassword;
        public static string SetValueForText1 = "";

        public MainForm()
        {
            InitializeComponent();
        }

        //public string MyValue
        //{
        //    get { return txtUsername.Text; }
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            //var bytes = new UTF8Encoding().GetBytes(txtPassword.Text.Trim());
            //var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            //string Hashpassword = Convert.ToBase64String(hashBytes);
            //return Hashpassword;

            string pwrd = $"Select Pwrd from tbl_user where Username ='{txtUsername.Text}'"; //username            
            dataAdapter = new SqlDataAdapter(pwrd, connString);
            table = new DataTable();
            dataAdapter.Fill(table);
            if (table.Rows.Count != 0)
            {
                hashPassword = table.Rows[0]["Pwrd"].ToString();
                if (Common.verifyPasswordMd5Hash(txtPassword.Text, hashPassword) == true)
                {
                    MessageBox.Show("Login sucess Welcome to Homepage");
                    SetValueForText1 = txtUsername.Text;       //pass value to other form
                    this.Hide();
                    PersonalInfo info = new PersonalInfo();
                    info.Show();
                }
                else
                {
                    MessageBox.Show("Invalid Password");
                    txtPassword.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Invalid username.");
                txtUsername.Text = "";
                txtPassword.Text = "";
            }

            //string select = $"Select * from tbl_user where Username = {txtUsername.Text.Trim()} and Pwrd ='{hashPassword}'";

            //using(conn = new SqlConnection(connString))
            // {
            //     conn.Open();
            //     try
            //     {
            //         command = new SqlCommand(select, conn);
            //         SqlDataAdapter da = new SqlDataAdapter(command);
            //         DataTable dt = new DataTable();
            //         da.Fill(dt);
            //         //string a = dt.Columns["Username"].DefaultValue.ToString();
            //        /* string A=  dt.Rows[0]["UserName"].ToString();*/ //to get the column value from the data table
            //         if (dt.Rows.Count > 0)
            //         {
            //             MessageBox.Show("Login sucess Welcome to Homepage");
            //             this.Hide();
            //             PersonalInfo info = new PersonalInfo();
            //             info.Show();
            //         }
            //         else
            //         {
            //             MessageBox.Show("Invalid Login please check username and password");
            //             txtUsername.Text = "";
            //             txtPassword.Text = "";
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         MessageBox.Show($"{ex.Message}");
            //     }
            // }           
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Registration link = new Registration();
            link.Show();
            this.Hide();
        }

        private void txtUsername_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (txtUsername.Text != "" && txtPassword.Text != "")
            {
                button1.Visible = true;
            }
        }

        private void txtPassword_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (txtUsername.Text != "" && txtPassword.Text != "")
            {
                button1.Visible = true;
            }
        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            //unmask c# password textbox and mask it back to password
            txtPassword.PasswordChar = cbShow.Checked ? '\0' : '\u25CF';
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            //if (Control.IsKeyLocked(Keys.CapsLock))
            //{
            //    MessageBox.Show("The Caps Lock key is ON.");
            //}
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPassword frm = new ForgotPassword();
            frm.Show();
            this.Hide();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
