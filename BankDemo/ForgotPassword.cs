using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BankDemo
{
    public partial class ForgotPassword : Form
    {
        SqlDataAdapter dataAdapter;
        DataTable table;
        SqlCommand command;
        SqlConnection conn;
        string connectionString = @"Data Source=KSY\SQLEXPRESS;Initial Catalog=BankDemo;Integrated Security=True;
                           Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        string username, update;
        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void lnkLogin_MouseClick(object sender, MouseEventArgs e)
        {
            MainForm frm = new MainForm();
            frm.Show();
        }

        private void ForgotPassword_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainForm frm = new MainForm();
            this.Hide();
            frm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text == "")
            {
                lblMessage.Text = "Please enter email!!";
            }

            else
            {
                lblMessage.Text = "";
                string select = $"Select Username from tbl_user where Email ='{txtEmail.Text}'"; //username and password             
                dataAdapter = new SqlDataAdapter(select, connectionString);
                table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    try
                    {
                        username = table.Rows[0]["Username"].ToString();
                        string passwordReset = Common.PasswordReset();
                        string hashResetPwrd = Common.getMd5HashForPassword(passwordReset);
                        //string hashResetPwrd =Common.Hash(Common.PasswordReset());
                        //string passwordReset = Common.PasswordReset();
                        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string mailMsg = "Dear " + username + "," + "\n" + "\n"
                                + "We got a request to reset your Bank 888 password. Following is your new password:"
                                + "\n" + "Password :" + passwordReset + "\n" + "\n"
                                + "Thanks," + "\n" + "\n" + "Best Regards," + "\n" + "\n"
                                + "Bank 888";
                        Common.SendEmail(txtEmail.Text, "Reset Password Bank 888", mailMsg);
                        update = $"UPDATE tbl_user set pwrd ='{hashResetPwrd}',Confirm_Pwrd ='{hashResetPwrd}',UpdOn ='{date}' WHERE Email ='{txtEmail.Text}' AND Username ='{username}'";
                        Common.UpdateData(update);
                        lblMessage.Text = "Verify email.Please check your email inbox.";
                        lnkLogin.Visible = true;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}");
                    }
                }
                else
                {
                    lblMessage.Text = "Email record not found.";
                }

            }
        }
    }
}
