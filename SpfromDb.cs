using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpParamClassGenerater
{
    public partial class SpfromDb : Form
    {
        List<string> lstSP;
        public SpfromDb()
        {
            InitializeComponent();
            lstSP = new List<string>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(textBox1.Text))
               lstSP= new CodeGenerator().GetSpListFromDB();
            else
            {
                Home.Schema = textBox1.Text.Trim();
                lstSP = new CodeGenerator().GetSpListFromDB(Home.Schema);
            }

            lstSP.ForEach(x => { checkedListBox1.Items.Add(x); });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Home.spList = lstSP;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Home.spList = new List<string>();
            foreach (var item in checkedListBox1.SelectedItems)
            {
                Home.spList.Add(item.ToString());
            }
            this.Close();
        }

        private void SpfromDb_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (checkedListBox1.Items.Count > 0)
            {
                Add obj = new Add();
                obj.Preparesplist(Home.spList);
                obj.ShowDialog();
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
