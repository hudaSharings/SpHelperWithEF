using Generator.SpHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpParamClassGenerater
{
    public partial class SpParamClass : Form
    {
        static string classstring;
        SpParamResultDetail _detail;
        public SpParamClass()
        {
            InitializeComponent();
        }

        public SpParamClass(string txt, string jsn, string resultCls)
        {
            InitializeComponent();
            this.rtxtParam.Text = txt;
            this.rtxtJson.Text = jsn;
            this.rtxtResult.Text = resultCls;
        }
        public SpParamClass(SpParamResultDetail details)
        {
            InitializeComponent();
            _detail = details;
            this.rtxtParam.Text = _detail.Param;
            this.rtxtJson.Text = _detail.Json;
            this.rtxtResult.Text = _detail.Result;
            this.rtxtRepo.Text = _detail.RepoMethod??_detail.Repo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.rtxtParam.Text);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.FileName = "Param.cs";

            save.Filter = "cSharp File | *.cs";

            if (save.ShowDialog() == DialogResult.OK)

            {

                StreamWriter writer = new StreamWriter(save.OpenFile());
                writer.Write(rtxtParam.Text);
                writer.Dispose();
                writer.Close();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.FileName = "Result.cs";

            save.Filter = "cSharp File | *.cs";

            if (save.ShowDialog() == DialogResult.OK)

            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                writer.Write(rtxtResult.Text);
                writer.Dispose();
                writer.Close();

            }
        }

        private void btnRepo_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.FileName = "Repo.cs";

            save.Filter = "cSharp File | *.cs";

            if (save.ShowDialog() == DialogResult.OK)

            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                writer.Write(rtxtRepo.Text);
                writer.Dispose();
                writer.Close();

            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.FileName = "Readme.cs";

            save.Filter = "cSharp File | *.cs";

            if (save.ShowDialog() == DialogResult.OK)
            {
                //StreamWriter writer = new StreamWriter(save.OpenFile());
                //writer.Write(rtxtParam.Text);
                //writer.Dispose();
                //writer.Close();
                try
                {
                    //Create, print, and close both files
                    //File.WriteAllText(save.FileName, rtxtParam.Text);
                    string directory = Path.GetDirectoryName(save.FileName);
                    string filename = Path.GetFileName(save.FileName).Split('.')[0];
                    string path = $"{directory}\\{filename}";
                    Directory.CreateDirectory(path);
                    File.WriteAllText($"{path}\\{filename}Param.cs", rtxtParam.Text);
                    File.WriteAllText($"{path}\\{filename}Result.cs", rtxtResult.Text);
                    File.WriteAllText($"{path}\\{filename}ParamJson.json", rtxtJson.Text);
                    File.WriteAllText($"{path}\\{filename}Repo.cs", rtxtRepo.Text);
                    File.WriteAllText($"{path}\\SP.cs", _detail.Consts);
                    if(Home.IsSpHelper)
                    File.WriteAllText($"{path}\\SpHelper.cs", GenerateSphelper.CodeStr);

                    var SpDetails = JsonConvert.SerializeObject(Add.SpDetailList);
                    if(SpDetails!=null)
                        File.WriteAllText($"{path}\\SpDetails.json", SpDetails);

                    MessageBox.Show("Files save saved at " + path);
                }
                catch (Exception)
                {

                    throw;
                }
              
                //Path.Combine(directory, "File2");

            }
        }
    }
}
