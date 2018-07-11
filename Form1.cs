using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace SpParamClassGenerater
{
    public partial class Form1 : Form
    {
        string _constr;
        public Form1()
        {

            InitializeComponent();
            _constr = Home.Constr;// ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string clasName = this.txtClassName.Text?.Trim();
            string spName = this.txtSpName.Text?.Trim();
            string conStr = Home.Constr;
            string NameSpace = this.textBox1.Text?.Trim();
            SPlist sp = new SPlist();
            sp.Spname = spName;
            sp.ClassName = clasName;
            sp.MethodName = txtMethod.Text;
            sp.IsNonQuery = checkBox2.Checked;
            sp.IsSingle = checkBox3.Checked;

            SpParamResultDetail m = new CodeGenerator().GenerateModel(spName, clasName,  _constr , NameSpace);
            if (checkBox1.Checked)
                if (string.IsNullOrEmpty(txtMethod.Text))
                    MessageBox.Show("Provide Name for Repo Method");
                else
                    m.RepoMethod = new CodeGenerator().GenerateRepoMethod(sp);

            SpParamClass objparam = new SpParamClass(m);
            objparam.ShowDialog();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtSpName.Text = txtClassName.Text = string.Empty;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Add().ShowDialog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Visible = checkBox1.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }



}
