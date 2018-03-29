using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace BankDemo
{
    public partial class Registration : Form
    {

        SqlConnection conn;
        SqlCommand command;
        SqlDataAdapter dataAdapter;
        DataTable table;
        string connString = @"Data Source=KSY\SQLEXPRESS;Initial Catalog=BankDemo;Integrated Security=True;
                           Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        string final; //Final serial NO
        string hashPwrd, subject, Message, image;
        int ID;

        public Registration()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string insert = @"INSERT INTO tbl_user(First_Name,Last_Name,Username,Pwrd,Confirm_Pwrd,IsActive,Email,PhoneNo,Image,Initial$,DOB,UpdOn) 
                            Values(@First_Name,@Last_Name,@Username,@Pwrd,@Confirm_Pwrd,@IsActive,@Email,@PhoneNo,@Image,@Initial$,@DOB,@UpdOn)";

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //can do sqldatetime conversion like sqldatetime

            if (txtPwrd.Text != txtCPwrd.Text)
            {
                MessageBox.Show("Password not match!!", "Alert", MessageBoxButtons.OKCancel);

            }
            else if (txtPwrd.Text == "" || txtCPwrd.Text == "")
            {
                MessageBox.Show("Password cannot be empty!!", "Alert", MessageBoxButtons.OKCancel);
            }
            else if (pictureBox1.ImageLocation == null)
            {
                MessageBox.Show("Please upload your picture before procced", "Alert", MessageBoxButtons.OKCancel);
            }

            else
            {
                GetPrefix();
                hashPwrd = Common.getMd5HashForPassword(txtPwrd.Text);
                subject = "Welcome to Bank 888!!!";
                Message = "Hello " + txtUserName.Text + "!" + "\n" + "\n"
                               + "Thanks for registering with Bank 888." + "\n" +
                               "Following is your personal detail:"
                               + "\n" + "Username :" + txtUserName.Text + "\n"
                               + "Password: " + txtPwrd.Text + "\n" + "\n"
                               + "Account No: " + final + "\n" + "\n"
                               + "Thanks," + "\n" + "\n" + "Best Regards," + "\n" + "\n"
                               + "Bank 888";

                byte[] imageSave = null;

                FileStream fs = new FileStream(image, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageSave = br.ReadBytes((int)fs.Length);
                using (conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        command = new SqlCommand(insert, conn);
                        command.Parameters.AddWithValue(@"First_Name", txtFName.Text);
                        command.Parameters.AddWithValue(@"Last_Name", txtLName.Text);
                        command.Parameters.AddWithValue(@"Username", txtUserName.Text);
                        command.Parameters.AddWithValue(@"DOB", dtDOB.Value.ToString("yyyy-MM-dd"));//
                        command.Parameters.AddWithValue(@"Pwrd", hashPwrd);
                        command.Parameters.AddWithValue(@"Confirm_Pwrd", hashPwrd);
                        command.Parameters.AddWithValue(@"IsActive", 1);
                        command.Parameters.AddWithValue(@"Email", txtEmail.Text);
                        command.Parameters.AddWithValue(@"PhoneNo", txtPhone.Text);
                        command.Parameters.AddWithValue(@"Image", imageSave);
                        command.Parameters.AddWithValue(@"Initial$", txtAmount.Text);
                        command.Parameters.AddWithValue(@"UpdOn", time);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Register sucess!!");
                        //Common.PasswordReset();
                        Common.SendEmail(txtEmail.Text, subject, Message);
                        //sendMessage();
                        GetUserID(txtUserName.Text);
                        //GetUserID(txtUserName.Text, final);
                        MainForm frm = new MainForm();
                        frm.Show();
                        this.Hide();

                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}");
                    }
                }
            }
        }

        //auto generate prefixID upon user registration
        public string GetPrefix()
        {
            string a, b, c, Serial;
            string selectStatement = "Select * from tbl_prefix";
            int d;
            string update = $"UPDATE tbl_prefix SET SerialNo = {Common.GetData(selectStatement, 0, "SerialNo")}+1 WHERE ID =1";
            using (conn = new SqlConnection(connString))
            {
                conn.Open();
                command = new SqlCommand(update, conn);
                command.ExecuteNonQuery();
                a = Common.GetData(selectStatement, 0, "SerialNo");
                b = Common.GetData(selectStatement, 0, "PrefixID");
                //c = DateTime.Now.Year.ToString("yy");
                c = DateTime.Now.ToString("yy");
                d = Convert.ToInt32(Common.GetData(selectStatement, 0, "SerialDigit"));
            }
            Serial = a.PadLeft(d, '0');
            return final = string.Format($"{b}{c}{Serial}"); //get final serial NO made of prefixID,YEAR, and serial No        

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MainForm frm = new MainForm();
            frm.Show();
            this.Hide();
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToShortDateString();
        }

        private void Registration_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image = openFileDialog1.FileName.ToString();
                pictureBox1.ImageLocation = image;
            }
        }

        public void GetUserID(string ID)
        {
            string statement = $@"SELECT * FROM tbl_user where Username ='{ID}'";
            dataAdapter = new SqlDataAdapter(statement, connString);
            table = new DataTable();
            dataAdapter.Fill(table);
            ID = table.Rows[0]["ID"].ToString();
            Common.GetAccountData(ID, final);
        }

        //public void GetUserID(string ID, string AccNo)
        //{
        //    Common.GetAccountData(ID, AccNo);
        //}


        protected void sendMessage()
        {
            //string AccountSid = "AC1553b60e111039ea8c4046a8e3b29a54";
            //string AuthToken = "52cad3e6635a85da21c828cfd423d8be";

            //try
            //{
            //    TwilioClient.Init(AccountSid, AuthToken);

            //    var to = new PhoneNumber("+60198841993");
            //    var message = MessageResource.Create(
            //        to,
            //        from: new PhoneNumber("+60175934298"),
            //        body: "This is the ship that made the Kessel Run in fourteen parsecs?");
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show($"{ex.Message}");
            //}

            try
            {
                string url = "https://NiceApi.net/API";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-APIId", "ALyZeC9eq0eUwgT / UGJvqmtpbXNheXlpbmc5M19hdF9nbWFpbF9kb3RfY29t");
                request.Headers.Add("X-APIMobile", "0198841993");
                using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                {
                    streamOut.Write("dss");
                }
                using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    //Console.WriteLine(streamIn.ReadToEnd());
                }
            }
            catch (SystemException se)
            {
                //Console.WriteLine(se.Message);
            }


        }

    }
}
