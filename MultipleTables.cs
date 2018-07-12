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
    public partial class MultipleTables : Form
    {
       public static List<string> lstTables;
       public static string modelStr;
        public MultipleTables()
        {
            InitializeComponent();
            lstTables = new List<string>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           List<string> lstr =new CodeGenerator().GetTableList();
            lstr.ForEach(x => { checkedListBox1.Items.Add(x); });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var item in checkedListBox1.Items)
            {
                lstTables.Add(item.ToString());
            }
            modelStr = new CodeGenerator().GetTableModel("Models", lstTables);
            this.Close();
        }

        private void MultipleTables_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (checkedListBox1.Items.Count > 0)
                new MultipleTablesResult(modelStr).ShowDialog();
        }
    }
}
