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
    public partial class SingleTable : Form
    {
        public SingleTable()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
                MessageBox.Show("Please Provide Table Name");
            else
                richTextBox1.Text = new CodeGenerator().TableToClass(textBox1.Text.Trim());
        }
    }
}
