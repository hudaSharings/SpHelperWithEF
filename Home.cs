using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace SpParamClassGenerater
{
    public partial class Home : Form
    {
        public static string Constr;
        public static bool IsSpHelper;
        public static string SphelperNamespace;
        public Home()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Constr= Constr?? txtconstr.Text;
            if (CheckConnection())
            {
                if (checkBox1.Checked && string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please Provide Name space For SpHelper");
                }
                else
                {
                    SphelperNamespace = textBox1.Text.Trim();
                    Form1 obj = new Form1();
                    obj.Show();

                }
            }
            else
                MessageBox.Show("Please Provide Valid Connection string");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Constr = Constr ?? txtconstr.Text;
            if (CheckConnection())
            {
                if (checkBox1.Checked && string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please Provide Name space For SpHelper");
                }
                else
                {
                    SphelperNamespace = textBox1.Text.Trim();
                    Add obj = new Add();
                    obj.Show();
                }
            }
            else
                MessageBox.Show("Please Provide Valid Connection string");
        }

        private bool CheckConnection()
        {


            if (string.IsNullOrEmpty(Constr))
                return false;

            try
            {
                SqlConnection con = new SqlConnection(Constr);
                con.Open();
                return true;
            }
            catch
            {

                Constr = string.Empty;
                return false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            IsSpHelper = groupBox1.Visible = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = label4.Visible = textBox4.Visible = textBox5.Visible = !checkBox2.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string constr = $"Data Source={textBox2.Text};Initial Catalog={textBox3.Text};Integrated Security={(checkBox2.Checked ? "True" : "False")};";
            if (!checkBox2.Checked)
            {
                if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
                {
                    MessageBox.Show("Please Provide UserName and Password to connect Server");
                    return;
                }
                else
                    constr += $" User Id = {textBox4.Text}; password = {textBox5.Text}";
            }
            Constr = constr;

            if (CheckConnection())
            {
                MessageBox.Show("Connection Success");
                Constr = constr;
            }
            else MessageBox.Show("Connection Failure");


        }
    }


}
