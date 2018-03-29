using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace BankDemo.Transaction
{
    public partial class Withdrawal : Form
    {
        SqlConnection conn;
        string connString = @"Data Source=KSY\SQLEXPRESS;Initial Catalog=BankDemo;Integrated Security=True;
                           Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlCommand command;
        SqlDataAdapter dataAdapter;
        DataTable table;
        string ID, UserID;
        int Amount;
        string final;
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public Withdrawal()
        {
            InitializeComponent();
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


                    if (reader.HasRows)
                    {
                        ID = reader[0].ToString();
                        txtUserName.Text = reader[3].ToString();
                        string RM = reader[10].ToString();
                        txtAmount.Text = $"{RM:C}";
                        MessageBox.Show("Record found");
                    }
                    else
                    {
                        txtUserName.Text = "";
                        txtAmount.Text = "";
                        MessageBox.Show("Record not found");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void UpdateFinalAmount()
        {
            try
            {
                //Amount -= Convert.ToInt32(txtFinalA.Text);
                using (conn = new SqlConnection(connString))
                {
                    string update = $@"UPDATE tbl_user SET Initial$ =@Initial$ WHERE ID={UserID}";
                    conn.Open();
                    command = new SqlCommand(update, conn);
                    command.Parameters.AddWithValue(@"Initial$", Amount);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Update successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void UpdateAmount()
        {
            //Amount += Convert.ToInt32(txtFinalA.Text);
            try
            {
                using (conn = new SqlConnection(connString))
                {
                    string update = $@"UPDATE tbl_account SET AmountAdd =@AmountAdd,AmountWdrl =@AmountWdrl WHERE UserID={UserID} and AccNo ='{txtAccNo.Text}'";
                    conn.Open();
                    command = new SqlCommand(update, conn);
                    command.Parameters.AddWithValue(@"AmountAdd", Amount);
                    command.Parameters.AddWithValue(@"AmountWdrl", txtFinalA.Text);

                    command.ExecuteNonQuery();
                    //MessageBox.Show("Update successfully");                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }

        private void Verify()
        {
            string a = MainForm.SetValueForText1;
            string select = $"Select ID from tbl_user where Username ='{a}'"; //username            
            dataAdapter = new SqlDataAdapter(select, connString);
            table = new DataTable();
            dataAdapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                UserID = table.Rows[0]["ID"].ToString();
            }
        }

        private void GetAmount()
        {
            string a = MainForm.SetValueForText1;
            string select = $"Select Initial$ from tbl_user where Username ='{a}'"; //username            
            dataAdapter = new SqlDataAdapter(select, connString);
            table = new DataTable();
            dataAdapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                Amount = (int)table.Rows[0]["Initial$"];
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string account = txtAccNo.Text;
            GetValue($"SELECT * FROM tbl_user a join tbl_account b on a.id = b.userid where AccNo ='{account}'");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Verify();
            GetAmount();
            Amount -= Convert.ToInt32(txtFinalA.Text);
            if (UserID != ID)
            {
                MessageBox.Show("You don't have the permission to update");
            }
            else if (txtAccNo.Text == "" || txtUserName.Text == "" || txtAmount.Text == "" || txtFinalA.Text == "")
            {
                MessageBox.Show("Please insert values!!");
            }
            else if (Amount < 0)
            {
                MessageBox.Show("No enought credit!!");
                txtFinalA.Text = "";
            }
            else
            {
                UpdateFinalAmount();
                UpdateAmount();
                GetPrefix();
                InsertTransaction();
                sendMessage();
                Clear();
            }
        }

        public void Clear()
        {
            txtAccNo.Text = "";
            txtUserName.Text = "";
            txtAmount.Text = "";
            txtFinalA.Text = "";
        }

        public string GetPrefix()
        {
            string a, b, c, Serial;
            string selectStatement = "Select * from tbl_prefix WHERE prefixID ='WDL'";
            int d;
            string update = $"UPDATE tbl_prefix SET SerialNo = {Common.GetData(selectStatement, 0, "SerialNo")}+1 WHERE ID =3";
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

        public void InsertTransaction()
        {
            string insert = @"INSERT INTO tbl_transaction(TransNo,UserID,UpdOn)
                              VALUES(@TransNo,@UserID,@UpdOn)";

            using (conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    command = new SqlCommand(insert, conn);
                    command.Parameters.AddWithValue(@"TransNo", final);
                    command.Parameters.AddWithValue(@"UserID", UserID);
                    command.Parameters.AddWithValue(@"UpdOn", time);

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
            }
        }

        private void sendMessage()
        {
            string result;
            string apiKey = "Qg9Bhta69Mk-P0cxgDaIvpDCdVm7C2UGEXE2kre3Pd";
            string numbers = "+60175934298"; // in a comma seperated list
            string message = $"Bank 888. Withdrawal from {final} RM{txtFinalA.Text} ,on {time}. Call 017-5934298 if you did NOT perform this Withdrawal";
            string send = "Kim SAY YING";
            String url = "https://api.txtlocal.com/send/?apikey=" + apiKey + "&numbers=" + numbers + "&message=" + message + "&sender=" + send;
            //refer to parameters to complete correct url string

            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(url);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(url);
            }
            catch (Exception ex)
            {
                //return e.Message;
                MessageBox.Show(null, "the error is" + ex, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader
                sr.Close();
            }
            //return result;
            MessageBox.Show(result);
        }

    }
}
