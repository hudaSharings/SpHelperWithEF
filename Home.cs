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
        public static List<string> spList;
        public static string Schema;
        public Home()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
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
                    ShowDialog();
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



        public void ShowDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "JSON Files|*.json";
            openFileDialog1.Title = "Select a Json File";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamReader sr = new
                   System.IO.StreamReader(openFileDialog1.FileName);
                string selectstr = sr.ReadToEnd();
                //MessageBox.Show(sr.ReadToEnd());
                sr.Close();
                //spList = (from sp in Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(selectstr)
                //          select new SPlist
                //          {
                //              id = ++cout,
                //              Spname = sp,
                //              ClassName = string.Empty,
                //              // MethodName=string.Empty,
                //              // IsNonQuery=false,
                //              //  IsSingle=false
                //          }).ToList();
                spList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(selectstr);
                Add obj = new Add();
                obj.Preparesplist(spList);
                obj.Show();
                //Preparesplist();

                // PrepareSPdetails();

            }
        }

        private void button4_Click(object sender, EventArgs e)
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
                    new SpfromDb().ShowDialog();
                }
            }
            else
                MessageBox.Show("Please Provide Valid Connection string");


        }

        private void Home_Load(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

            if (CheckConnection())
                new SingleTable().ShowDialog();

            else
                MessageBox.Show("Please Provide Valid Connection string");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (CheckConnection())
                new MultipleTables().ShowDialog();

            else
                MessageBox.Show("Please Provide Valid Connection string");
        }
    }


}
