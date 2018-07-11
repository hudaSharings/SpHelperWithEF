using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpParamClassGenerater
{
    public partial class Add : Form
    {
        public static List<SPlist> spList;
        public static List<SpDetail> SpDetailList;
        int cout = 0;
        public Add()
        {
            InitializeComponent();
            spList = new List<SPlist>();
            SpDetailList = new List<SpDetail>();
           
        }

        private void Add_Load(object sender, EventArgs e)
        {

            //spList = (from sp in Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\SPlist.json"))
            //          select new SPlist
            //          {
            //              id = ++cout,
            //              Spname = sp,
            //              ClassName = string.Empty
            //          }).ToList();
            // cout = spList.Count; 
            //  LoadGrid();

        }

        private void LoadGrid()
        {
            dataGridView1.DataSource = spList;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewColumn columnid = dataGridView1.Columns["id"];
            columnid.Visible = false;
            DataGridViewColumn column = dataGridView1.Columns["Spname"];
            column.ReadOnly = true;
            DataGridViewColumn columnclass = dataGridView1.Columns["classname"];
            cout = spList.Count;
        }

        private  void button1_Click(object sender, EventArgs e)
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
                 Preparesplist(Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(selectstr));

                PrepareSPdetails();

            }
           
        }

        public async void Preparesplist(List<string> lstSp)
        {
            spList = (from sp in lstSp
             select new SPlist
             {
                 id = ++cout,
                 Spname = sp,
                 ClassName = string.Empty,
                 // MethodName=string.Empty,
                 // IsNonQuery=false,
                 //  IsSingle=false
             }).ToList();

            LoadGrid();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                //TODO - Button Clicked - Execute Code Here
                //
                spList.RemoveAt(e.RowIndex);
                dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        private void btnGo_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtNameSpace.Text))
            {
                MessageBox.Show("Prove Value for Name space ");
                return;
            }

            List<SPlist> lst = new List<SPlist>();
            CodeGenerator _gen = new CodeGenerator();

            for (int rows = 0; rows < dataGridView1.Rows.Count; rows++)
            {
                SPlist obj = new SPlist();
                obj.Spname = dataGridView1.Rows[rows].Cells["spname"].Value?.ToString();
                obj.ClassName = dataGridView1.Rows[rows].Cells["Classname"].Value?.ToString();
                obj.MethodName = dataGridView1.Rows[rows].Cells["MethodName"].Value?.ToString();
                obj.IsNonQuery = Convert.ToBoolean(dataGridView1.Rows[rows].Cells["IsNonQuery"].Value);
                obj.IsSingle = Convert.ToBoolean(dataGridView1.Rows[rows].Cells["IsSingle"].Value);
                if (!string.IsNullOrEmpty(obj.Spname))
                    lst.Add(obj);
            }
            SpParamResultDetail m = _gen.GenerateModel(lst, Home.Constr, txtNameSpace.Text);

            m.Consts = _gen.GenerateConsts(lst);

            if (checkBox1.Checked)
            {
                if (!string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox2.Text))
                    m.Repo = _gen.GenerateRepo(lst, textBox2.Text, textBox3.Text);

                else
                    MessageBox.Show("Please provide Value for Repo Namesapce and Repo class");
            }

            SpParamClass objparam = new SpParamClass(m);
            objparam.ShowDialog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Visible = this.checkBox1.Checked;
        }

        private async void PrepareSPdetails()
        {
            SpDetailList = new List<SpDetail>();
            foreach (var item in spList)
            {
                SpDetailList.Add(CodeGenerator.GetSPDetails(item.Spname, Home.Constr));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
             new SpfromDb().ShowDialog();
            
        }
    }


}
