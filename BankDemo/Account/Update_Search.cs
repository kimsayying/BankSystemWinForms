using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BankDemo.Account
{
    public partial class Update_Search : Form
    {
        string search,image,ID;
        SqlConnection conn;
        string connString = @"Data Source=KSY\SQLEXPRESS;Initial Catalog=BankDemo;Integrated Security=True;
                           Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlCommand command;
        SqlDataAdapter dataAdapter;
        DataTable table;
        public static string UserID;

        public Update_Search()
        {
            InitializeComponent();
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PersonalInfo frm = new PersonalInfo();
            frm.Show();
            this.Hide();
        }

        private void Update_Search_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            lblDate.Text = DateTime.Now.ToShortDateString();           
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            search = txtSearch.Text;
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Username":
                    GetValue($"SELECT * FROM tbl_user where Username ='{search}'");
                    break;

                case "AccountNo":
                    GetValue($"SELECT * FROM tbl_user a join tbl_account b on a.id = b.userid where AccNo ='{search}'");
                    break;

                    case "Email":
                    GetValue($"SELECT * FROM tbl_user where Email='{search}'");
                    break;
            }
        }

        private void GetValue(string select)
        {
            try
            {
                using (conn = new SqlConnection(connString))
                {
                    conn.Open();
                    command = new SqlCommand(select, conn);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    DateTime date;

                    if (reader.HasRows)
                    {
                        ID = reader[0].ToString();
                        txtFName.Text = reader[1].ToString();
                        txtLName.Text = reader[2].ToString();
                        txtUserName.Text = reader[3].ToString();
                        dtDOB.Value = DateTime.TryParse(reader[11].ToString(), out date) ? date : date;
                        txtEmail.Text = reader[7].ToString();
                        txtPhone.Text = reader[8].ToString();
                        //txtAmount.Text = reader[10].ToString();
                        byte[] img = (byte[])reader[9];
                        if (img == null)
                        {
                            pictureBox1.Image = null;
                        }
                        else
                        {
                            MemoryStream ms = new MemoryStream(img);
                            pictureBox1.Image = Image.FromStream(ms);                           
                        }
                        MessageBox.Show("Record found");
                    }
                    else
                    {
                        txtFName.Text = "";
                        txtLName.Text = "";
                        txtUserName.Text = "";
                        dtDOB.Value = DateTime.Now;
                        txtEmail.Text = "";
                        txtPhone.Text = "";
                        //txtAmount.Text = "";
                        pictureBox1.Image = null;
                        MessageBox.Show("Record not found");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetID();

            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please upload your picture before procced", "Alert", MessageBoxButtons.OKCancel);
            }
            else if (UserID != ID)
            {
                MessageBox.Show("You don't have the permission to update");
            }
            else if(txtFName.Text =="" || txtLName.Text =="" ||txtUserName.Text =="" ||txtEmail.Text =="" || txtPhone.Text == "")
            {
                MessageBox.Show("Please insert values!!");
            }

            else
            {
                DialogResult result = MessageBox.Show($"Are you sure you want to update this account ?", "Alert", MessageBoxButtons.OKCancel);

                try
                {
                    using (conn = new SqlConnection(connString))
                    {
                        if (result == DialogResult.OK)
                        {
                            //byte[] imageSave = null;

                            //FileStream fs = new FileStream(pictureBox1.Image.ToString(), FileMode.Open, FileAccess.Read);
                            //BinaryReader br = new BinaryReader(fs);
                            //imageSave = br.ReadBytes((int)fs.Length);

                            string update = $@"UPDATE tbl_user SET First_Name =@First_Name, Last_Name =@Last_Name ,Username =@Username, Email=@Email,  
                                             PhoneNo =@PhoneNo,DOB =@DOB,UpdOn =@UpdOn WHERE ID={ID}";
                            conn.Open();
                            command = new SqlCommand(update, conn);
                            command.Parameters.AddWithValue(@"First_Name", txtFName.Text);
                            command.Parameters.AddWithValue(@"Last_Name", txtLName.Text);
                            command.Parameters.AddWithValue(@"Username", txtUserName.Text);
                            command.Parameters.AddWithValue(@"Email", txtEmail.Text);
                            command.Parameters.AddWithValue(@"PhoneNo", txtPhone.Text);
                            //command.Parameters.AddWithValue(@"Image", imageSave);
                            //command.Parameters.AddWithValue(@"Initial$", txtAmount.Text);
                            command.Parameters.AddWithValue(@"DOB", dtDOB.Value.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue(@"UpdOn", DateTime.Now.ToString("yyyy-MM-dd"));

                            command.ExecuteNonQuery();
                            MessageBox.Show("Update successfully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
            }

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image = openFileDialog1.FileName.ToString();
                pictureBox1.ImageLocation = image;
            }
        }

        private void GetID() //make sure the login is same as user want to update
        {
            string a = MainForm.SetValueForText1;

            string select = $"Select ID from tbl_user where username ='{a}'";
            dataAdapter = new SqlDataAdapter(select, connString);
            table = new DataTable();
            dataAdapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                UserID = table.Rows[0]["ID"].ToString();
            }
        }
    }
}
