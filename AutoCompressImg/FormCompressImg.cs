using MetadataExtractor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoCompressImg
{
    public partial class FormCompressImg : Form
    {
        public FormCompressImg()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 遍历图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.progressBar1.Value = 0;
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
                    DirectoryInfo di = new DirectoryInfo(foldPath);

                    FindFile(di);
                    if (this.progressBar1.Value == 100)
                    {
                        MessageBox.Show("批量压缩完成");
                    }
                }
            }

        }


        public void SearchFile()
        {

            string path1 = @"E:\\7\1511110000000800";// @"E:\\img";

            var images = System.IO.Directory.GetFiles(path1, ".", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));

            //遍历string 型 images数组
            foreach (var i in images)
            {
                var str = i.Replace(path1, "");//获取相对路径
                var path2 = str.Replace("\\", ""); //将字符“\\”转换为“/”
                var imgName = path2;

                VaryQualityLevel(i, imgName);
            }


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

        private static void VaryQualityLevel(string imgPath, string imgName)
        {

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
                FixSize(imgPath, Convert.ToInt32(width), Convert.ToInt32(height), subPath + imgName, imgName);
            }
            else
            {
                try
                {
                    SetImgQuality(imgPath, imgPath, imgName);
                }
                catch (Exception ex)
                {

                    Logging.Info("SetImgQuality:" + ex.Message);
                }

            }

        }

        #region   设置图片质量
        public static void SetImgQuality(string sourcePath, string imgPath, string imgName)
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
        }


        #endregion

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
                Logging.Info("FixSize：" + ex.Message);
                return false;
            }
        }


        #endregion getThumImage 


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
                    this.progressBar1.Value = 0;
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (i + 1 == files.Length)
                        {
                            this.progressBar1.Value = 100;
                        }
                        else if (i == files.Length / 2)
                        {
                            this.progressBar1.Value = 50;
                        }

                        //获取并输出文件扩展名称
                        //  Console.WriteLine(Path.GetExtension(files[i].FullName));

                        //修改文件扩展名称
                        files[i].MoveTo(Path.ChangeExtension(files[i].FullName, extension));
                    }
                    if (this.progressBar1.Value == 100)
                    {
                        MessageBox.Show("批量修改后缀完成");
                    }
                }
            }
        }

        public void FindFile(DirectoryInfo di)
        {

            FileInfo[] files = di.GetFiles();
            int value = 1;
            for (int i = 0; i < files.Length; i++)
            {
                if (i + 1 == files.Length)
                {
                    this.progressBar1.Value = 100;
                }
                else if (i == files.Length / 2)
                {
                    this.progressBar1.Value = 50;
                }
                else
                {
                    if (value <= 50)
                    {
                        value++;
                        this.progressBar1.Value = value;
                    }

                }

                //Console.WriteLine("文件：" + files[i].FullName);
                try
                {
                    //判断是否具有照片信息，报错即不是照片文件
                    GetMetaData.GetExifByMe(files[i].FullName);
                    //压缩图片文件
                    VaryQualityLevel(files[i].FullName, System.IO.Path.GetFileName(files[i].FullName));
                }
                catch (Exception ex)
                {
                    Logging.Info(System.IO.Path.GetFileName(files[i].FullName) + "，非图片文件，" + ex.Message);
                    continue;
                }

            }
            DirectoryInfo[] dis = di.GetDirectories();
            for (int j = 0; j < dis.Length; j++)
            {
                Console.WriteLine("目录：" + dis[j].FullName);
                FindFile(dis[j]);//对于子目录，进行递归调用
            }
        }
    }

}