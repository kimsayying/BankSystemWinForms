using System;
using System.Windows.Forms;

namespace BankDemo
{
    public partial class PersonalInfo : Form
    {
        public PersonalInfo()
        {
            InitializeComponent();
        }


        private void PersonalInfo_Load(object sender, EventArgs e)
        {
            string a = MainForm.SetValueForText1;
            label1.Text = DateTime.Now.ToString("HH:mm:ss");
            label2.Text = DateTime.Now.ToString("MMMM dd, yyyy");
            textBox1.Text = MainForm.SetValueForText1;
            label3.Text = $"Welcome {a}";
            label3.BackColor = System.Drawing.Color.ForestGreen;
        }

        private void allAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Account.AllAcount frm = new Account.AllAcount();
            frm.Show();
            frm.MdiParent = this;
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm frm = new MainForm();
            frm.Show();
            this.Hide();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PersonalInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void updateSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Account.Update_Search frm = new Account.Update_Search();
            frm.Show();
            frm.MdiParent = this;
        }

        private void depositToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transaction.Deposit frm = new Transaction.Deposit();
            frm.Show();
            frm.MdiParent = this;
        }

        private void withdrawerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transaction.Withdrawal frm = new Transaction.Withdrawal();
            frm.Show();
            frm.MdiParent = this;
        }

        private void transferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transaction.Transfer frm = new Transaction.Transfer();
            frm.Show();
            frm.MdiParent = this;

        }

        private void fDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transaction.FD frm = new Transaction.FD();
            frm.Show();
            frm.MdiParent = this;
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }
    }
}
