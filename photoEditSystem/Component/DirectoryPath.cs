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
        /// <param name="sourceImg">照片圖片</param>
        /// <param name="destImg">相框圖片</param>
        /// <param name="borderStyle">相框樣式</param>
        /// <param name="imgBackgorund">背板</param>
        public string CombinImage(string sourceImg, string destImg, string borderStyle, string imgBackgorund)
        {
            DateTime nowday = DateTime.Now;
            string resultFileName = "result_" + nowday.ToString("yyyy-MM-dd-HH-mm-ss");
            Image imgBack = Image.FromFile(imgBackgorund); // 底圖
            Image imgBorder = Image.FromFile(destImg); //照片相框圖片
            Image img = Image.FromFile(sourceImg); //照片圖片
            //指定的System.Drawing.Image創建新的System.Drawing.Graphics       
            Graphics g = Graphics.FromImage(imgBack);
            //g.DrawImage(img, 照片與相框的左邊距, 照片與相框的上邊距, 照片寬, 照片高);
            // 判斷照片圖片size是否大於背板版面
            double drawPositionX = 0.0;
            double drawPositionY = 0.0;
            if (img.Width < imgBack.Width && img.Height < imgBack.Height)
            {
                // 寬跟高都小於背板
                drawPositionX = (imgBack.Width / 2) - (img.Width / 2);
                drawPositionY = (imgBack.Height / 2) - (img.Height / 2);
            }
            else if (img.Width < imgBack.Width)
            {
                // 寬小於背板 但 高一樣
                drawPositionX = (imgBack.Width / 2) - (img.Width / 2);
            }
            else if (img.Height < imgBack.Height)
            {
                // 高小於背板 但 寬一樣
                drawPositionY = (imgBack.Height / 2) - (img.Height / 2);
            }
            else
            {
                // 寬跟高都大等於背板
            }
            int drawPositionXInt = Convert.ToInt32(drawPositionX);
            int drawPositionYInt = Convert.ToInt32(drawPositionY);
            // 判斷使用者選取borderStyle
            if (borderStyle == "StraightStyle")
            {
                //g.FillRectangle(Brushes.Black, -50, -50, (int)1150, ((int)1500));//相片四周刷一層黑色邊框，需要調尺寸
                g.DrawImage(imgBack, 0, 0, 1280, 1600);
                g.DrawImage(img, drawPositionXInt, drawPositionYInt, img.Width, img.Height);
                g.DrawImage(imgBorder, 0, 0, 1280, 1600);
            }
            else if (borderStyle == "HorizontalStyle")
            {
                //g.FillRectangle(Brushes.Black, -50, -50, (int)1480, ((int)1100));//相片四周刷一層黑色邊框，需要調尺寸
                g.DrawImage(imgBack, 0, 0, 1478, 1108);
                g.DrawImage(img, drawPositionXInt, drawPositionYInt, img.Width, img.Height);
                g.DrawImage(imgBorder, 0, 0, 1478, 1108);
            }

            GC.Collect();
            string saveResultImagePath = Path.Combine(HttpContext.Current.Server.MapPath("~/tempPhoto/result"), resultFileName + ".png");
            string saveResultImageFileName = resultFileName + ".png";
            //save new image to file system.
            imgBack.Save(saveResultImagePath);
            imgBack.Dispose();
            img.Dispose();
            imgBorder.Dispose();
            return saveResultImageFileName;
        }

        /// <summary>
        /// 放大或缩圖
        /// </summary>
        /// <param name="path_source">原始圖片路径</param>
        /// <param name="path_save">縮圖路径</param>
        /// <param name="times">缩略倍数</param>
        /// <param name="b">缩圖或放大（true缩圖）</param>
        public void Thumbnail(string path_source, int times, bool b)
        {
            //加载底圖
            Image img = Image.FromFile(path_source);
            string path_save = path_source.Substring(0, path_source.Length - path_source.Length) + "_new" + path_source;
            int w = img.Width;
            int h = img.Height;
            //設置畫布
            int width = 0;
            int height = 0;
            if (b)
            {
                width = w / times;
                height = h / times;
            }
            else
            {
                width = w * times;
                height = h * times;
            }
            Bitmap map = new Bitmap(width, height);
            //繪圖
            Graphics g = Graphics.FromImage(map);
            g.DrawImage(img, 0, 0, width, height);
            //保存
            map.Save(path_save);

        }

        #region  取得圖片等比例縮圖後的寬和高像素
        /// <summary>
        ///  寬高誰較長就縮誰  - 計算方法
        /// </summary>
        /// <param name="bmp">System.Drawing.Image 的物件</param>
        /// <param name="maxPx">寬或高超過多少像素就要縮圖</param>
        /// <returns>回傳int陣列，索引0為縮圖後的寬度、索引1為縮圖後的高度</returns>
        public int[] GetThumbPic_WidthAndHeight(Bitmap bmp, int maxPx)
        {

            int newWidth = 0;
            int newHeight = 0;

            if (bmp.Width > maxPx || bmp.Height > maxPx)
            //如果圖片的寬大於最大值或高大於最大值就往下執行  
            {

                if (bmp.Width >= bmp.Height)
                //圖片的寬大於圖片的高  
                {

                    newHeight = Convert.ToInt32((Convert.ToDouble(maxPx) / Convert.ToDouble(bmp.Width)) * Convert.ToDouble(bmp.Height));
                    //設定修改後的圖高  
                    newWidth = maxPx;
                }
                else
                {

                    newWidth = Convert.ToInt32((Convert.ToDouble(maxPx) / Convert.ToDouble(bmp.Height)) * Convert.ToDouble(bmp.Width));
                    //設定修改後的圖寬  
                    newHeight = maxPx;
                }
            }
            else
            {//圖片沒有超過設定值，不執行縮圖   
                newHeight = bmp.Height;
                newWidth = bmp.Width;
            }

            int[] newWidthAndfixHeight = { newWidth, newHeight };

            return newWidthAndfixHeight;
        }


        /// <summary>
        /// 寬度維持maxWidth，高度等比例縮放   - 計算方法
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        public int[] GetThumbPic_Width(Bitmap bmp, int maxWidth)
        {
            //要回傳的結果
            int newWidth = 0;
            int newHeight = 0;

            if (bmp.Width > maxWidth)
            //如果圖片的寬大於最大值 
            {

                //等比例的圖高
                newHeight = Convert.ToInt32((Convert.ToDouble(maxWidth) / Convert.ToDouble(bmp.Width)) * Convert.ToDouble(bmp.Height));
                //設定修改後的圖寬  
                newWidth = maxWidth;

            }
            else
            {//圖片寬沒有超過設定值，不執行縮圖  

                newHeight = bmp.Height;
                newWidth = bmp.Width;
            }

            int[] newWidthAndfixHeight = { newWidth, newHeight };

            return newWidthAndfixHeight;

        }

        /// <summary>
        /// 高度維持maxHeight，寬度等比例縮放  - 計算方法
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public int[] GetThumbPic_Height(Bitmap bmp, int maxHeight)
        {
            //要回傳的值
            int newWidth = 0;
            int newHeight = 0;

            if (bmp.Height > maxHeight)
            //如果圖片的高大於最大值 
            {
                //等比例的寬
                newWidth = Convert.ToInt32((Convert.ToDouble(maxHeight) / Convert.ToDouble(bmp.Height)) * Convert.ToDouble(bmp.Width));
                //圖高固定  
                newHeight = maxHeight;

            }
            else
            {//圖片的高沒有超過設定值 

                newHeight = bmp.Height;
                newWidth = bmp.Width;
            }

            int[] newWidthAndfixHeight = { newWidth, newHeight };

            return newWidthAndfixHeight;
        }
        #endregion

        #region 產生縮圖並儲存
        /// <summary>
        /// 產生縮圖並儲存 寬高誰較長就縮誰
        /// </summary>
        /// <param name="srcImagePath">來源圖片的路徑</param>
        /// <param name="maxPix">超過多少像素就要等比例縮圖</param>
        /// <param name="saveThumbFilePath">縮圖的儲存檔案路徑</param>
        public void SaveThumbPic(string srcImagePath, int maxPix, string saveThumbFilePath)
        {
            //讀取原始圖片 
            using (FileStream fs = new FileStream(srcImagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                //取得原始圖片 
                Bitmap bmpOld = new Bitmap(fs);
                // 計算維持比例的縮圖大小 
                int[] thumbnailScaleWidth = GetThumbPic_WidthAndHeight(bmpOld, maxPix);
                int newWidth = thumbnailScaleWidth[0];
                int newHeight = thumbnailScaleWidth[1];

                // 產生縮圖 
                Bitmap bmpThumb = new Bitmap(bmpOld, newWidth, newHeight);
                bmpThumb.Save(saveThumbFilePath);

            }//end using 
        }

        /// <summary>
        /// 產生縮圖並儲存 寬度維持maxpix，高度等比例
        /// </summary>
        /// <param name="srcImagePath">來源圖片的路徑</param>
        /// <param name="widthMaxPix">超過多少像素就要等比例縮圖</param>
        /// <param name="saveThumbFilePath">縮圖的儲存檔案路徑</param>
        public void SaveThumbPicWidth(string srcImagePath, int widthMaxPix, string saveThumbFilePath)
        {
            //讀取原始圖片 
            using (FileStream fs = new FileStream(srcImagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                //取得原始圖片 
                Bitmap bmpOld = new Bitmap(fs);

                //圖片寬高 
                // 計算維持比例的縮圖大小 
                int[] thumbnailScaleWidth = GetThumbPic_Width(bmpOld, widthMaxPix);
                int newWidth = thumbnailScaleWidth[0];
                int newHeight = thumbnailScaleWidth[1];

                // 產生縮圖 
                Bitmap bmpThumb = new Bitmap(bmpOld, newWidth, newHeight);
                bmpThumb.Save(saveThumbFilePath);

            }//end using
        }

        /// <summary>
        /// 產生縮圖並儲存 高度維持maxPix，寬度等比例
        /// </summary>
        /// <param name="srcImagePath">來源圖片的路徑</param>
        /// <param name="heightMaxPix">超過多少像素就要等比例縮圖</param>
        /// <param name="saveThumbFilePath">縮圖的儲存檔案路徑</param>
        public void SaveThumbPicHeight(string srcImagePath, int heightMaxPix, string saveThumbFilePath)
        {
            //讀取原始圖片 
            using (FileStream fs = new FileStream(srcImagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                //取得原始圖片 
                Bitmap bmpOld = new Bitmap(fs);
                System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
                // 計算維持比例的縮圖大小 
                int[] thumbnailScaleWidth = GetThumbPic_Height(bmpOld, heightMaxPix);
                int newWidth = thumbnailScaleWidth[0];
                int newHeight = thumbnailScaleWidth[1];

                // 產生縮圖 
                Bitmap bmpThumb = new Bitmap(bmpOld, newWidth, newHeight);

                bmpThumb.Save(saveThumbFilePath);

            }//end using 
        }

        #endregion

        public int getPicWidth(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                int width = image.Width;               
                return width;
            }         
        }
        public int getPicHigh(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                int high = image.Height;
                return high;
            }
        }
    }
}