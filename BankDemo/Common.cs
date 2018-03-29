using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace BankDemo
{
    class Common
    {
        static SqlDataAdapter dataAdapter;
        static DataTable table;
        static string connString = @"Data Source=KSY\SQLEXPRESS;Initial Catalog=BankDemo;Integrated Security=True;
                           Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        static SqlConnection conn;
        static SqlCommand command;
        static string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        public static string GetData(string statement, int i, string column)
        {
            dataAdapter = new SqlDataAdapter(statement, connString);
            table = new DataTable();
            dataAdapter.Fill(table);
            string value = table.Rows[i][column].ToString();  //to get the column value from the data table
            return value;
        }


        public static void GetAccountData(string UserID, String AccNo)
        {
            string insert = @"INSERT INTO tbl_account(AccNo,UserID,UpdOn,IsActive)
                              VALUES(@AccNo,@UserID,@UpdOn,@IsActive)";

            using (conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    command = new SqlCommand(insert, conn);
                    command.Parameters.AddWithValue(@"AccNo", AccNo);
                    command.Parameters.AddWithValue(@"UserID", UserID);
                    command.Parameters.AddWithValue(@"UpdOn", time);
                    command.Parameters.AddWithValue(@"IsActive", 1);

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
            }
        }

        public static string SendEmail(string toAddress, string subject, string body)
        {

            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("ksyTesting@gmail.com", "Wstd0407"),
                    EnableSsl = true
                };
                client.Send("ksyTesting@gmail.com", toAddress, subject, body);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            return "ab";
        }

        public static string PasswordReset()
        {
            Random r = new Random();
            var list = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwsyz0123456789";
            var collection = new char[6];

            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = list[r.Next(list.Length)];
            }
            var res = new string(collection);

            return res;
        }

        public static string Hash(string password)
        {
            //You can use a hash algorithm like MD5, SHA1, SHA265, SHA512, ... to hash the password. For example:
            //Then store the hash of password in database and when you want to compare entered password with database stored value, 
            //compare hash of entered value with database value.
            var bytes = new UTF8Encoding().GetBytes(password);
            var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            string Hashpassword = Convert.ToBase64String(hashBytes);
            return Hashpassword;
        }

        public static void UpdateData(string update)
        {
            using (conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    command = new SqlCommand(update, conn);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}");
                }
            }
        }

        public static string getMd5HashForPassword(string password)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the password to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a password.
        public static bool verifyPasswordMd5Hash(string password, string hash)
        {
            // Hash the password.
            string hashOfPassword = getMd5HashForPassword(password);

            // Create a StringComparer an comare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfPassword, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }




    }
}
