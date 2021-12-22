using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace photoEditSystem.Component
{
    public class DirectoryPath
    {
        public string file1PathFileName { get; set; }
        public string fileBorderPathFileName { get; set; }
        public string resultPathFileName { get; set; }
        /// <summary>
        /// 判斷檔案是否存在
        /// </summary>
        /// <param name="path"></param>
        public void IsDirectoryPath(string path)
        {
            if (!Directory.Exists(path))
            {
                //新增資料夾
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFile(string filePathName)
        {
            if (File.Exists(filePathName))
            {
                File.Delete(filePathName);
            }
        }

        /// <summary>
        /// 合併照片
        /// 背景圖，中間貼自己的目標圖片
        /// </summary>
        /// <param name="sourceImg">相框圖片</param>
        /// <param name="destImg">照片圖片</param>
        public string CombinImage(string sourceImg, string destImg)
        {
            DateTime nowday = DateTime.Now;
            string resultFileName = "result_" + nowday.ToString("yyyy-MM-dd-HH-mm-ss");
            Image imgBack = Image.FromFile(sourceImg); //相框圖片 
            Image img = Image.FromFile(destImg); //照片圖片
            //从指定的System.Drawing.Image创建新的System.Drawing.Graphics       
            Graphics g = Graphics.FromImage(imgBack);
            //g.DrawImage(imgBack, 0, 0, 148, 124); // g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);
            //g.FillRectangle(Brushes.Black, 125, 50, (int)212, ((int)203));//相片四周刷一層黑色邊框，需要調尺寸
            //g.DrawImage(img, 照片與相框的左邊距, 照片與相框的上邊距, 照片寬, 照片高);
            g.DrawImage(img, 125, 50, 1357, 1920);
            GC.Collect();
            string saveResultImagePath = Path.Combine(HttpContext.Current.Server.MapPath("~/tempPhoto/result"), resultFileName + ".png");
            string saveResultImageFileName = resultFileName + ".png";
            //save new image to file system.
            imgBack.Save(saveResultImagePath);
            imgBack.Dispose();
            img.Dispose();
            return saveResultImageFileName;
        }
        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="path"></param>
        public void removeBg(string picture, string borderName)
        {
            Image image = Image.FromFile(picture);
            using (Bitmap bitmap = new Bitmap(image))
            {
                bitmap.MakeTransparent(Color.White);
                string saveResultImagePath = Path.Combine(HttpContext.Current.Server.MapPath("~/tempPhoto/border"), borderName);
                bitmap.Save(@"C:\SideProject20211213\photoEditSystem\photoEditSystem\tempPhoto\photo\123.png");
                bitmap.Dispose();
            }
        }
       
        public void removeBgApi(string picture)
        {
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Headers.Add("X-Api-Key", "1bum8bKVNHG1yqGctHK8f4Rs");
                formData.Add(new ByteArrayContent(File.ReadAllBytes(picture)), "image_file", "file.jpg");
                formData.Add(new StringContent("auto"), "size");
                var response = client.PostAsync("https://api.remove.bg/v1.0/removebg", formData).Result;

                if (response.IsSuccessStatusCode)
                {
                    FileStream fileStream = new FileStream(@"C:\SideProject20211213\photoEditSystem\photoEditSystem\tempPhoto\photo\no-bg.png", FileMode.Create, FileAccess.Write, FileShare.None);                    
                    response.Content.CopyToAsync(fileStream).ContinueWith((copyTask) => { fileStream.Close(); });
                }
                else
                {
                    Console.WriteLine("Error: " + response.Content.ReadAsStringAsync().Result);
                }
            }
        }

        /// <summary>
        /// 圖片裁剪，生成新圖，保存在同一目錄下,名字加_new，格式1.png 新圖1_new.png
        /// </summary>
        /// <param name="picPath">要修改圖片完整路徑</param>
        /// <param name="x">修改起點x坐標</param>
        /// <param name="y">修改起點y坐標</param>
        /// <param name="width">新圖宽度</param>
        /// <param name="height">新圖高度</param>
        public static void caijianpic(string picPath, int x, int y, int width, int height)
        {
            //圖片路径
            string oldPath = picPath;
            //新圖片路径
            string newPath = System.IO.Path.GetExtension(oldPath);
            //計算新的文件名，在舊文件名後加_new
            newPath = oldPath.Substring(0, oldPath.Length - newPath.Length) + "_new" + newPath;
            //定義截取矩形
            System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle(x, y, width, height);
            //要截取的區域大小
            //加载圖片
            System.Drawing.Image img = System.Drawing.Image.FromStream(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(oldPath)));
            //判断超出的位置否
            if ((img.Width < x + width) || img.Height < y + height)
            {              
                img.Dispose();
                return;
            }
            //定義Bitmap对象
            System.Drawing.Bitmap bmpImage = new System.Drawing.Bitmap(img);
            //進行裁剪
            System.Drawing.Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            //保存成新文件
            bmpCrop.Save(newPath);
            //释放對象
            img.Dispose(); bmpCrop.Dispose();
        }

    }
}