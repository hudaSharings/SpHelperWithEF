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
    public partial class MultipleTablesResult : Form
    {
        public MultipleTablesResult()
        {
            InitializeComponent();
        }
        public MultipleTablesResult(string reslt)
        {
            InitializeComponent();
            richTextBox1.Text = reslt;
        }
    }
}
