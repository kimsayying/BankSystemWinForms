using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BankDemo.Account
{
    public partial class AllAcount : Form
    {
        string connString = @"Data Source=KSY\SQLEXPRESS;Initial Catalog=BankDemo;Integrated Security=True;
                           Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlDataAdapter dataAdapter;
        DataTable table;      
        string ID;

        public AllAcount()
        {
            InitializeComponent();
        }

        private void AllAcount_Load(object sender, EventArgs e)
        {          
            GetID();
            GetAccount();
        }

        private void GetID()
        {
            string a = MainForm.SetValueForText1; // get value from form1

            string select = $"Select ID from tbl_user where username ='{a}'";
            dataAdapter = new SqlDataAdapter(select, connString);
            table = new DataTable();
            dataAdapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                ID = table.Rows[0]["ID"].ToString();
            }
        }

        private void GetAccount()
        {
            string select = $"SELECT * FROM tbl_user where ID ={ID}";
            dataAdapter = new SqlDataAdapter(select,connString);
            table = new DataTable();
            dataAdapter.Fill(table);
            dataGridView1.DataSource = table;
        }
    }
}
