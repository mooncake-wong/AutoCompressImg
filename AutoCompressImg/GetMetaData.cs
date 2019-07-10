using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompressImg
{
    public class GetMetaData
    {
        #region   通过metadata-extractor获取照片参数

        //参考文献
        //官网: https://drewnoakes.com/code/exif/
        //nuget 官网:https://www.nuget.org/
        //nuget 使用: http://www.cnblogs.com/chsword/archive/2011/09/14/NuGet_Install_OperatePackage.html
        //nuget MetadataExtractor: https://www.nuget.org/packages/MetadataExtractor/

        /// <summary>通过MetadataExtractor获取照片参数
        /// </summary>
        /// <param name="imgPath">照片绝对路径</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetExifByMe(string imgPath)
        {
            var rmd = ImageMetadataReader.ReadMetadata(imgPath);

            var rt = new Dictionary<string, string>();
            foreach (var rd in rmd)
            {
                foreach (var tag in rd.Tags)
                {
                    var temp = EngToChs(tag.Name);
                    if (temp == "其他")
                    {
                        continue;
                    }
                    if (!rt.ContainsKey(temp))
                    {
                        rt.Add(temp, tag.Description);
                    }

                }
            }
            return rt;
        }

        /// <summary>筛选参数并将其名称转换为中文
        /// </summary>
        /// <param name="str">参数名称</param>
        /// <returns>参数中文名</returns>
        private static string EngToChs(string str)
        {
            var rt = "其他";
            switch (str)
            {
                case "Exif Version":
                    rt = "Exif版本";
                    break;
                case "Model":
                    rt = "相机型号";
                    break;
                case "Lens Model":
                    rt = "镜头类型";
                    break;
                case "File Name":
                    rt = "文件名";
                    break;
                case "File Size":
                    rt = "文件大小";
                    break;
                case "Date/Time":
                    rt = "拍摄时间";
                    break;
                case "File Modified Date":
                    rt = "修改时间";
                    break;
                case "Image Height":
                    rt = "照片高度";
                    break;
                case "Image Width":
                    rt = "照片宽度";
                    break;
                case "X Resolution":
                    rt = "水平分辨率";
                    break;
                case "Y Resolution":
                    rt = "垂直分辨率";
                    break;
                case "Color Space":
                    rt = "色彩空间";
                    break;

                case "Shutter Speed Value":
                    rt = "快门速度";
                    break;
                case "F-Number":
                    rt = "光圈";//Aperture Value也表示光圈
                    break;
                case "ISO Speed Ratings":
                    rt = "ISO";
                    break;
                case "Exposure Bias Value":
                    rt = "曝光补偿";
                    break;
                case "Focal Length":
                    rt = "焦距";
                    break;

                case "Exposure Program":
                    rt = "曝光程序";
                    break;
                case "Metering Mode":
                    rt = "测光模式";
                    break;
                case "Flash Mode":
                    rt = "闪光灯";
                    break;
                case "White Balance Mode":
                    rt = "白平衡";
                    break;
                case "Exposure Mode":
                    rt = "曝光模式";
                    break;
                case "Continuous Drive Mode":
                    rt = "驱动模式";
                    break;
                case "Focus Mode":
                    rt = "对焦模式";
                    break;
            }
            return rt;
        }

        #endregion

    }
}
