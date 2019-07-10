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

namespace AutoCompressImg
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 简单压缩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimpleCompress_Click(object sender, EventArgs e)
        {
            FormCompressImg form = new FormCompressImg();
            form.Show();
        }

        /// <summary>
        /// 高级压缩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            FormCompressPicture form = new FormCompressPicture();
            form.Show();
        }

        /// <summary>
        ///  批量修改文件后缀名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, EventArgs e)
        {
            UpdateExtension("jpg");
        }

        /// <summary>
        /// 批量修改文件后缀
        /// </summary>
        /// <param name="extension">文件后缀</param>
        private void UpdateExtension(string extension)
        {
            //弹框选择文件夹
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                Description = "请选择文件夹"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //获得文件夹路径
                string foldPath = dialog.SelectedPath;
                if (!string.IsNullOrEmpty(foldPath))
                {
                    //初始化文件夹对象
                    DirectoryInfo dir = new DirectoryInfo(foldPath);
                    // 获取当前文件夹下的所有文件
                    //TopDirectoryOnly:在搜索操作中包括仅当前目录
                    FileInfo[] files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    //遍历当前文件夹下的所有文件
                    int j = 0;
                    for (int i = 0; i < files.Length; i++)
                    {

                        j = i;

                        //获取并输出文件扩展名称
                        //  Console.WriteLine(Path.GetExtension(files[i].FullName));

                        //修改文件扩展名称
                        files[i].MoveTo(Path.ChangeExtension(files[i].FullName, extension));
                    }
                    if (j==files.Length)
                    {
                        MessageBox.Show("批量修改后缀完成");
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
