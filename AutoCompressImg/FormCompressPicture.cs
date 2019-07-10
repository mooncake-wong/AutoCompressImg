using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoCompressImg
{


    /// <summary>
    ///  多线程处理
    /// </summary>
    public partial class FormCompressPicture : Form
    {
        public FormCompressPicture()
        {
            InitializeComponent();
        }

        private IList<string> imageList = new List<string>();

        private delegate void DelegateWriteResult(string file, bool result);

        private delegate void DelegateWriteMessage(string message, int count);

        private delegate void DelegateFile(DirectoryInfo info);


        private void btnSelectSourceFolder_Click(object sender, EventArgs e)
        {
            this.lvSourceFolderList.Clear();

            if (DialogResult.OK == folderBrowserDialog.ShowDialog())
            {
                this.lvSourceFolderList.Items.Clear();
                tbSourceFolderPath.Text = folderBrowserDialog.SelectedPath;
                //视图
                this.lvSourceFolderList.View = View.List;
                //  ListFiles(new DirectoryInfo(tbSourceFolderPath.Text));
                Thread listThread = new Thread(new ThreadStart(FindFile));
                listThread.IsBackground = true;
                listThread.Start();


            }
        }
        private void FindFile()
        {
            string message = "";
            DirectoryInfo di = new DirectoryInfo(tbSourceFolderPath.Text);

            message = "正在遍历：" + di.FullName;
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateFile(ListFiles), new object[] { di });
            }
            else
            {
                this.WriteMessage(message, 0);
            }
        }


        /// <summary>
        /// 遍历文件
        /// </summary>
        /// <param name="di"></param>
        public void ListFiles(DirectoryInfo di)
        {
            if (!di.Exists)
            {
                return;
            }


            if (di == null)
            {
                return;
            }

            //返回当前目录的文件列表
            FileInfo[] files = di.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {

                try
                {
                    //判断是否具有照片信息，报错即不是照片文件
                    GetMetaData.GetExifByMe(files[i].FullName);

                    //把图片文件添加到列表视图
                    this.lvSourceFolderList.Items.Add(files[i].FullName);

                    //把图片文件添加到图片列表
                    imageList.Add(files[i].FullName);

                }
                catch (Exception)
                {
                    //Logging.Error(System.IO.Path.GetFileName(files[i].FullName) + "，非图片文件，" + ex.Message);
                    continue;
                }

            }
            this.lbInfomation.Text = "共" + this.lvSourceFolderList.Items.Count + "条数据";
            //返回当前目录的子目录
            DirectoryInfo[] dis = di.GetDirectories();

            for (int j = 0; j < dis.Length; j++)
            {
                // Console.WriteLine("目录：" + dis[j].FullName);
                ListFiles(dis[j]);//对于子目录，进行递归调用
            }

        }

        /// <summary>
        /// 改变图片质量
        /// </summary>
        /// <param name="imgPath"></param>
        /// <param name="imgName"></param>
        private static bool VaryQualityLevel(string imgPath, string imgName)
        {

            bool result = false;

            Bitmap bmp1 = new Bitmap(imgPath);

            //获取照片信息
            // GetExifByMe(imgPath);
            //先获取图片的像素
            var imgPixl = RGB2Gray(bmp1);

            //像素超出，先压缩图片
            if (imgPixl.Width > 1000 && imgPixl.Height > 1000)
            {
                double width = 0;
                double height = 0;
                if (imgPixl.Width > 2000 && imgPixl.Height > 2000)
                {
                    width = System.Math.Ceiling(Convert.ToDouble(imgPixl.Width / 4));
                    height = System.Math.Ceiling(Convert.ToDouble(imgPixl.Height / 4));
                }
                else if (imgPixl.Width > 1000 && imgPixl.Height > 2000)
                {
                    width = System.Math.Ceiling(Convert.ToDouble(imgPixl.Width / 2));
                    height = System.Math.Ceiling(Convert.ToDouble(imgPixl.Height / 2));
                }
                //cutimg先创建好
                //检查是否存在文件夹
                string subPath = @"d:/cutimg/";
                if (false == System.IO.Directory.Exists(subPath))
                {
                    //创建pic文件夹
                    System.IO.Directory.CreateDirectory(subPath);
                }
                result = FixSize(imgPath, Convert.ToInt32(width), Convert.ToInt32(height), subPath + imgName, imgName);
            }
            else
            {

                result = SetImgQuality(imgPath, imgPath, imgName);

            }

            return result;

        }


        #region   读取图片像素

        public static Bitmap RGB2Gray(Bitmap srcBitmap)

        {

            int wide = srcBitmap.Width;

            int height = srcBitmap.Height;

            Rectangle rect = new Rectangle(0, 0, wide, height);

            //将Bitmap锁定到系统内存中,获得BitmapData

            BitmapData srcBmData = srcBitmap.LockBits(rect,

                      ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //创建Bitmap

            Bitmap dstBitmap = CreateGrayscaleImage(wide, height);//这个函数在后面有定义

            BitmapData dstBmData = dstBitmap.LockBits(rect,

                      ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行

            System.IntPtr srcPtr = srcBmData.Scan0;

            System.IntPtr dstPtr = dstBmData.Scan0;

            //将Bitmap对象的信息存放到byte数组中

            int src_bytes = srcBmData.Stride * height;

            byte[] srcValues = new byte[src_bytes];

            int dst_bytes = dstBmData.Stride * height;

            byte[] dstValues = new byte[dst_bytes];

            //复制GRB信息到byte数组

            System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcValues, 0, src_bytes);

            System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes);

            //根据Y=0.299*R+0.114*G+0.587B,Y为亮度

            for (int i = 0; i < height; i++)

                for (int j = 0; j < wide; j++)

                {

                    //只处理每行中图像像素数据,舍弃未用空间

                    //注意位图结构中RGB按BGR的顺序存储

                    int k = 3 * j;

                    byte temp = (byte)(srcValues[i * srcBmData.Stride + k + 2] * .299

                         + srcValues[i * srcBmData.Stride + k + 1] * .587

+ srcValues[i * srcBmData.Stride + k] * .114);

                    dstValues[i * dstBmData.Stride + j] = temp;

                }

            System.Runtime.InteropServices.Marshal.Copy(dstValues, 0, dstPtr, dst_bytes);

            //解锁位图

            srcBitmap.UnlockBits(srcBmData);

            dstBitmap.UnlockBits(dstBmData);

            return dstBitmap;

        }

        ///<summary>

        /// Create and initialize grayscale image

        ///</summary>

        public static Bitmap CreateGrayscaleImage(int width, int height)

        {

            // create new image

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // set palette to grayscale

            SetGrayscalePalette(bmp);

            // return new image

            return bmp;

        }

        ///<summary>

        /// Set pallete of the image to grayscale

        ///</summary>

        public static void SetGrayscalePalette(Bitmap srcImg)

        {

            // check pixel format

            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)

                throw new ArgumentException();

            // get palette

            ColorPalette cp = srcImg.Palette;

            // init palette

            for (int i = 0; i < 256; i++)
            {

                cp.Entries[i] = Color.FromArgb(i, i, i);

            }

            srcImg.Palette = cp;

        }

        #endregion

        /// <summary>  /// 按图片尺寸大小压缩图片  /// </summary> 
        /// <param name="sourceFile">原始图片文件</param> 
        /// <param name="multiple">收缩倍数</param>  
        /// <param name="outputFile">输出文件名</param>  
        /// <returns>成功返回true,失败则返回false</returns>
        public static bool FixSize(string sourceFile, int xWidth, int yWidth, string outputFile, string imgName)
        {
            try
            {
                Bitmap sourceImage = new Bitmap(sourceFile);

                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");

                Bitmap newImage = new Bitmap((int)(xWidth), (int)(yWidth));

                Graphics g = Graphics.FromImage(newImage);

                g.DrawImage(sourceImage, 0, 0, xWidth, yWidth);

                sourceImage.Dispose();

                g.Dispose();

                newImage.Save(outputFile);

                SetImgQuality(sourceFile, outputFile, imgName);

                newImage.Dispose();

                //删除该图片文件
                File.Delete(outputFile);

                return true;
            }
            catch (Exception ex)
            {
                Logging.Error("FixSize：" + imgName + "  压缩出错：" + ex.Message);
                return false;
            }
        }


        private void GetFileInfo(string filePath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            try
            {
                foreach (DirectoryInfo childDirectoryInfo in directoryInfo.GetDirectories())
                {

                    string name = childDirectoryInfo.Name.ToString();

                    GetFileInfo(filePath + "\\" + childDirectoryInfo.Name.ToString());
                }

                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                { //Length单位为bytes，    1k=1024b
                    if (fileInfo.Length > 200 * 1024)
                    {
                        //  getThumImage(fileInfo.FullName, 100, 1, fileInfo.FullName); 
                        //   ImageByQuality(fileInfo.FullName, 18); 
                        // ImageByMultiple(fileInfo.FullName,1); 

                        var str = fileInfo.FullName.Replace(filePath, "");//获取相对路径
                        var path2 = str.Replace("\\", ""); //将字符“\\”转换为“/”
                                                           // list.Add(path2);
                        var imgName = path2;
                        VaryQualityLevel(fileInfo.FullName, imgName);
                    }
                }
            }
            catch { }
        }

        /// <summary>  /// 获取图片编码信息  /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;

            ImageCodecInfo[] encoders;

            encoders = ImageCodecInfo.GetImageEncoders();

            for (j = 0; j < encoders.Length; ++j)
            {

                if (encoders[j].MimeType == mimeType)

                    return encoders[j];
            }
            return null;
        }


        #region 调用图片处理的方法 

        /// <summary>  /// 按质量比和尺寸收缩呗数,生成缩略图，保存为另外路劲 /// </summary>  
        ///<param name="sourceFile">原始图片文件</param> 
        ///<param name="quality">质量压缩比</param>
        ///<param name="multiple">收缩倍数</param> 
        ///<param name="outputFile">输出文件名</param>
        ///<returns>成功返回true,失败则返回false</returns>  
        public static bool getThumImage(String sourceFile, long quality, int multiple, String outputFile)
        {
            bool flag = false;
            try
            {
                flag = ThumImage(sourceFile, quality, multiple, outputFile);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        ///<summary> 按质量比和尺寸收缩呗数,生成缩略图;保存在同一路径 /// </summary>  
        /// <param name="sourceFile">原始图片文件</param>
        /// <param name="quality">质量压缩比</param>  
        /// <param name="multiple">收缩倍数</param>  
        /// <param name="outputFile">输出文件名</param>  
        /// <returns>成功返回true,失败则返回false</returns> 
        public static bool getThumImage(String sourceFile, long quality, int multiple)
        {
            bool flag = false;
            try
            {
                flag = ThumImage(sourceFile, quality, multiple, sourceFile);

            }
            catch
            {

                flag = false;
            }
            return flag;
        }
        /// <summary>  /// 按图片尺寸大小压缩图片  ，保存在不同路径 /// </summary>  
        /// <param name="sourceFile">原始图片文件</param>
        /// <param name="multiple">收缩倍数</param> 
        /// <returns>成功返回true,失败则返回false</returns>  
        public static bool ImageByMultiple(String sourceFile, int multiple, String outputFile)
        {
            bool flag = false;
            try
            {
                flag = ThumImageByMultiple(sourceFile, multiple, outputFile);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>  ///  按图片尺寸大小压缩图片  ，保存在同一路径 /// </summary> 
        /// <param name="multiple">收缩倍数</param>  
        /// <returns>成功返回true,失败则返回false</returns> 
        public static bool ImageByMultiple(String sourceFile, int multiple)
        {
            bool flag = false; try
            {
                flag = ThumImageByMultiple(sourceFile, multiple, sourceFile);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>  /// 质量压缩比，保存在不同一路径 /// </summary>  
        /// <param name="sourceFile">原始图片文件</param>
        /// <param name="quality">质量压缩比</param> 
        /// <returns>成功返回true,失败则返回false</returns>  
        public static bool ImageByQuality(String sourceFile, int quality, String outputFile)
        {
            bool flag = false;
            try
            {
                flag = ThumImageByQuality(sourceFile, quality, outputFile);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>  /// 按质量压缩比压缩图片，保存在同一路径 /// </summary> 
        /// <param name="sourceFile">原始图片文件</param> 
        /// <param name="quality">质量压缩比</param>  
        /// <returns>成功返回true,失败则返回false</returns> 
        public static bool ImageByQuality(String sourceFile, int quality)
        {
            bool flag = false;
            try
            {
                flag = ThumImageByQuality(sourceFile, quality, sourceFile);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        #endregion

        #region 调用图片处理具体方法


        /// <summary> /// 按照图片质量生成图片, /// </summary> 
        /// <param name="sourceFile">原始图片文件</param>  
        /// <param name="quality">质量压缩比</param>  
        /// <param name="outputFile">输出文件名</param>  
        /// <returns>成功返回true,失败则返回false</returns>  
        private static bool ThumImageByQuality(String sourceFile, long quality, String outputFile)
        {
            bool flag = false;
            try
            {
                long imageQuality = quality;

                Bitmap sourceImage = new Bitmap(sourceFile);

                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");

                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);

                myEncoderParameters.Param[0] = myEncoderParameter;

                float xWidth = sourceImage.Width;

                float yWidth = sourceImage.Height;

                Bitmap newImage = new Bitmap((int)(xWidth), (int)(yWidth));

                Graphics g = Graphics.FromImage(newImage);

                g.DrawImage(sourceImage, 0, 0, xWidth, yWidth);

                sourceImage.Dispose(); g.Dispose();

                newImage.Save(outputFile, myImageCodecInfo, myEncoderParameters);

                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        /// <summary> /// 生成缩略图 /// </summary>
        /// <param name="sourceFile">原始图片文件</param> 
        /// <param name="quality">质量压缩比</param> 
        /// <param name="multiple">收缩倍数</param>  
        /// <param name="outputFile">输出文件名</param>
        /// <returns>成功返回true,失败则返回false</returns> 
        private static bool ThumImage(String sourceFile, long quality, int multiple, String outputFile)
        {
            bool flag = false;
            try
            {
                long imageQuality = quality;

                Bitmap sourceImage = new Bitmap(sourceFile);

                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");

                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);

                myEncoderParameters.Param[0] = myEncoderParameter;

                float xWidth = sourceImage.Width; float yWidth = sourceImage.Height;

                Bitmap newImage = new Bitmap((int)(xWidth / multiple), (int)(yWidth / multiple));

                Graphics g = Graphics.FromImage(newImage);

                g.DrawImage(sourceImage, 0, 0, xWidth / multiple, yWidth / multiple);

                sourceImage.Dispose();

                g.Dispose();

                newImage.Save(outputFile, myImageCodecInfo, myEncoderParameters);

                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>  /// 按图片尺寸大小压缩图片  /// </summary>
        /// <param name="sourceFile">原始图片文件</param>  
        /// <param name="multiple">收缩倍数</param>  
        /// <param name="outputFile">输出文件名</param> 
        /// <returns>成功返回true,失败则返回false</returns>
        public static bool ThumImageByMultiple(String sourceFile, int multiple, String outputFile)
        {
            try
            {
                Bitmap sourceImage = new Bitmap(sourceFile);

                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");

                float xWidth = sourceImage.Width;

                float yWidth = sourceImage.Height;

                Bitmap newImage = new Bitmap((int)(xWidth / multiple), (int)(yWidth / multiple));

                Graphics g = Graphics.FromImage(newImage); g.DrawImage(sourceImage, 0, 0, xWidth / multiple, yWidth / multiple);

                sourceImage.Dispose();

                g.Dispose();

                newImage.Save(outputFile);

                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion getThumImage 

        #region   设置图片质量
        public static bool SetImgQuality(string sourcePath, string imgPath, string imgName)
        {
            Bitmap bmp1 = new Bitmap(imgPath);
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            //创建一个Endoder对象
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            // 创建一个EncoderParameters对象.
            // 一个EncoderParameters对象有一个EncoderParameter数组对象
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            //这里的50L用来设置保存时的图片质量
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 80L);
            //测试时400多K的图片保存为100多K，图片失真也不是很厉害
            myEncoderParameters.Param[0] = myEncoderParameter;

            //CompressImg先创建好
            //  string path1 = Path.GetDirectoryName(imgPath);
            DirectoryInfo di = new DirectoryInfo(string.Format(@"{0}..\..\", sourcePath));
            string path1 = di.FullName;//获取上两级目录
            string newpath = path1.Replace(path1.Substring(0, 3), "");//去年根盘目录
            var path2 = newpath.Replace("\\", "/"); //将字符“\\”转换为“/”
            //检查是否存在文件夹
            string subPath = @"d:/CompressImg/" + path2;
            if (false == System.IO.Directory.Exists(subPath))
            {
                //创建pic文件夹
                System.IO.Directory.CreateDirectory(subPath);
            }
            bmp1.Save(subPath + imgName, jgpEncoder, myEncoderParameters);
            bmp1.Dispose();
            return true;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        #endregion

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 开始压缩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {

            Thread workThread = new Thread(new ThreadStart(CompressAll));
            workThread.IsBackground = true;
            workThread.Start();
        }

        private void WriteMessage(string message, int i)
        {
            lblFinal.Text = message;
            lblResult.Text = "已成功压缩：" + i + "个文件";
        }

        private void CompressAll()
        {
            string message = "";
            int i = 0;
            foreach (string item in imageList)
            {
                i += 1;
                int index = item.LastIndexOf("\\");
                if (index != -1)
                {
                    string fileName = item.Substring(index + 1);

                    message = "正在处理：" + fileName;
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new DelegateWriteMessage(WriteMessage), new object[] { message, i });
                    }
                    else
                    {
                        this.WriteMessage(message, i);
                    }


                    if (CompressPicture(item, fileName))
                    {
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new DelegateWriteResult(WriteResult), new object[] { item, true });
                        }
                        else
                        {
                            this.WriteResult(item, true);
                        }
                    }
                    else
                    {
                        i -= 1;

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new DelegateWriteResult(WriteResult), new object[] { item, false });
                        }
                        else
                        {
                            this.WriteResult(item, false);
                        }
                    }
                }
            }

            message = "全部压缩完成";
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateWriteMessage(WriteMessage), new object[] { message, i });
            }
            else
            {
                this.WriteMessage(message, i);
            }
        }

        private void WriteResult(string fileName, bool result)
        {
            if (result)
            {
                ListViewItem thisListItem = new ListViewItem();
                thisListItem.ForeColor = Color.White;
                thisListItem.BackColor = Color.DarkGreen;
                thisListItem.SubItems[0].Text = fileName;
                thisListItem.SubItems.Add("成功");

            }
            else
            {
                ListViewItem thisListItem = new ListViewItem();
                thisListItem.ForeColor = Color.White;
                thisListItem.BackColor = Color.Red;
                thisListItem.SubItems[0].Text = fileName;
                thisListItem.SubItems.Add("失败");


                for (int j = 0; j < this.lvSourceFolderList.Items.Count; j++)
                {
                    if (fileName == this.lvSourceFolderList.Items[j].Text)
                    {
                        //压缩失败的文件写入文本，出现 System.IO.IOException:“文件“d:\CompressFailFile.txt”正由另一进程使用，因此该进程无法访问此文件。”的错误提示，用using解决
                        using (StreamWriter my_writer = new StreamWriter(@"d:\CompressFailFile.txt", true, System.Text.Encoding.Default))
                        {
                            string txtstr = "压缩失败：" + fileName + "\r\n";
                            my_writer.Write(txtstr);
                            my_writer.Flush();
                        }
                      
                        this.lvSourceFolderList.Items[j].BackColor = SystemColors.ControlDark;
                    }
                }
            }



        }


        /// <summary>
        /// 压缩图片方法
        /// </summary>
        /// <param name="sourcePath">原目录</param>
        /// <param name="imageName">图片文件名</param>
        /// <returns>压缩是否成功</returns>
        private bool CompressPicture(string sourcePath, string imageName)
        {

            return VaryQualityLevel(sourcePath, imageName);




        }

        /// <summary>
        /// 保存为JPEG格式，支持压缩质量选项
        /// </summary>
        /// <param name="bmp">原始位图</param>
        /// <param name="FileName">新文件地址</param>
        /// <param name="Qty">压缩质量，越大越好，文件也越大(0-100)</param>
        /// <returns>成功标志</returns>
        public static bool SaveAsJPEG(Bitmap bmp, string FileName, int Qty)
        {
            try
            {
                EncoderParameter p;
                EncoderParameters ps;

                ps = new EncoderParameters(1);

                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Qty);
                ps.Param[0] = p;

                bmp.Save(FileName, GetCodecInfo("image/jpeg"), ps);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保存JPG时用
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns>得到指定mimeType的ImageCodecInfo</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }

        private void FormCompressPicture_Load(object sender, EventArgs e)
        {
            this.lbInfomation.Text = ""; this.lblFinal.Text = "";
            this.lblResult.Text = "";
            this.lblChoose.Visible = false;
            this.txtContent.Visible = false;
        }

        /// <summary>
        /// 选择行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSourceFolderList_SelectedIndexChanged(object sender, EventArgs e)
        {

            ListView.SelectedIndexCollection indexes = lvSourceFolderList.SelectedIndices;//

            string pr = "";

            foreach (int index in indexes)
            {
                pr = lvSourceFolderList.Items[index].Text;
            }

            this.lblChoose.Visible = true;
            this.txtContent.Visible = true;
            this.txtContent.Text = pr;// 显示选择的行的内容


        }
    }
}
