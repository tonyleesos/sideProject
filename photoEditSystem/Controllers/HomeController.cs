using iTextSharp.text;
using photoEditSystem.Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace photoEditSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(DirectoryPath directoryPath)
        {
            if (TempData["Message"] != null)
            {
                // border style
                ViewBag.borderStraightStylePathFileSrc = "tempPhoto/border/Birthday_border_straight.png";
                ViewBag.borderHorizontalStylePathFileSrc2 = "tempPhoto/border/Spider_border_horizontal.png";
                // reslut pic
                ViewBag.resultPathFileSrc = (directoryPath.resultPathFileName != null) ? "tempPhoto/result/" + directoryPath.resultPathFileName : "";
            }
            else
            {
                // 第一次loading進來
                // border style
                ViewBag.borderStraightStylePathFileSrc = "tempPhoto/border/Birthday_border_straight.png";
                ViewBag.borderHorizontalStylePathFileSrc2 = "tempPhoto/border/Spider_border_horizontal.png";
            }
            return View();
        }

        public ActionResult Admin()
        {
            // 讀取資料夾
            string loadPicPathFilder = Server.MapPath("~/tempPhoto/result"); // 存合成後圖片路徑           
            string FileNoExt, FileWithExt = "",imageFileUrl="", FileExtension = "";
            string[] files = Directory.GetFiles(loadPicPathFilder);
            List<string> filePicPathList = new List<string>();
            foreach (string filename in files)
            {                                        
                //filename=@"\\server8\Log\Dir1\abc.pdf"        
                //檔名(不包含副檔名)(不含路徑)例：abc
                FileNoExt = Path.GetFileNameWithoutExtension(filename);
                //檔名(包含副檔名)(不含路徑)例：abc.pdf
                FileWithExt = Path.GetFileName(filename);
                FileExtension = Path.GetExtension(filename);
                if(FileExtension == ".jpg" || FileExtension == ".png")
                {
                    imageFileUrl = "http://if186.aca.ntu.edu.tw/phtopedit/tempPhoto/result/" + FileWithExt;
                    filePicPathList.Add(imageFileUrl);
                }
                
            }

            ViewBag.filePicPathList = filePicPathList;
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImg(FormCollection collection, HttpPostedFileBase File1, string borderStyle,int picWidth,int picHigh)
        {
            try
            {
                string photoName = Guid.NewGuid() + File1.FileName;
                // 資料夾 / 檔案是否存在與建立資料夾
                DirectoryPath directoryPath = new DirectoryPath();
                directoryPath.IsDirectoryPath(Server.MapPath("~/tempPhoto"));
                directoryPath.IsDirectoryPath(Server.MapPath("~/tempPhoto/border"));
                directoryPath.IsDirectoryPath(Server.MapPath("~/tempPhoto/photo"));
                directoryPath.IsDirectoryPath(Server.MapPath("~/tempPhoto/result"));

                // 設定儲存照片路徑(含檔名)
                string savedPhotoName = Path.Combine(Server.MapPath("~/tempPhoto/photo"), photoName);
                // 設定儲存外框路徑(含檔名)
                string savedBorderPhotoName = "";
                string borderName = "";
               
                // 儲存檔案
                File1.SaveAs(savedPhotoName);

                // 剪裁 & 縮放 
                string thumbPicOutName = Guid.NewGuid() + "_New_" + File1.FileName;
                string thumbPicOutPath = Path.Combine(Server.MapPath("~/tempPhoto/photo"), thumbPicOutName);
                string imgBackgorund = ""; // 合成背景圖
                // 取得圖片長寬
                int widthPic = directoryPath.getPicWidth(savedPhotoName);
                int highPic = directoryPath.getPicHigh(savedPhotoName);
                // 檢查有些手機直版拍照系統會抓錯長、寬
                if(widthPic != picWidth || highPic != picHigh)
                {
                    widthPic = picWidth;
                    highPic = picHigh;
                }
                // 判斷使用者選擇外框模式
                if (borderStyle == "StraightStyle" && (highPic >= widthPic))
                {
                    //// 使用者選擇直框(birdthday)，圖片是直的
                    // 設定儲存外框路徑(含檔名)
                    savedBorderPhotoName = Path.Combine(Server.MapPath("~/tempPhoto/border"), "Birthday_border_straight.png");
                    borderName = "Birthday_border_straight.png";
                    // 縮放比例跟背板一樣大小
                    directoryPath.SaveThumbPicHeight(savedPhotoName, directoryPath.getPicHigh(savedBorderPhotoName), thumbPicOutPath, picWidth, picHigh);                    
                    imgBackgorund = Path.Combine(Server.MapPath("~/tempPhoto/border"), "imageBackgroundStraight_White.png");
                }
                else if (borderStyle == "HorizontalStyle" && (widthPic >= highPic))
                {
                    //// 使用者選擇橫框(spider)，圖片是橫的
                    // 設定儲存外框路徑(含檔名)
                    savedBorderPhotoName = Path.Combine(Server.MapPath("~/tempPhoto/border"), "Spider_border_horizontal.png");
                    borderName = "Spider_border_horizontal.png";
                    // 縮放比例跟背板一樣大小
                    directoryPath.SaveThumbPicWidth(savedPhotoName, directoryPath.getPicWidth(savedBorderPhotoName), thumbPicOutPath, picWidth, picHigh);                   
                    imgBackgorund = Path.Combine(Server.MapPath("~/tempPhoto/border"), "imageBackgroundHorizontal_White.png");
                }
                else if (borderStyle == "StraightStyle" && (widthPic > highPic))
                {
                    //// 使用者選擇直框(birdthday)，圖片是橫的
                    // 設定儲存外框路徑(含檔名)
                    savedBorderPhotoName = Path.Combine(Server.MapPath("~/tempPhoto/border"), "Birthday_border_horizontal.png");
                    borderName = "Birthday_border_horizontal.png";
                    // 縮放比例跟背板一樣大小
                    directoryPath.SaveThumbPicWidth(savedPhotoName, directoryPath.getPicWidth(savedBorderPhotoName), thumbPicOutPath, picWidth, picHigh);
                    imgBackgorund = Path.Combine(Server.MapPath("~/tempPhoto/border"), "imageBackgroundHorizontal_White.png");
                    borderStyle = "HorizontalStyle";
                }
                else if (borderStyle == "HorizontalStyle" && (highPic > widthPic))
                {
                    //// 使用者選擇橫框(spider)，圖片是直的
                    // 設定儲存外框路徑(含檔名)
                    savedBorderPhotoName = Path.Combine(Server.MapPath("~/tempPhoto/border"), "Spider_border_straight.png");
                    borderName = "Spider_border_straight.png";
                    // 縮放比例跟背板一樣大小
                    directoryPath.SaveThumbPicHeight(savedPhotoName, directoryPath.getPicHigh(savedBorderPhotoName), thumbPicOutPath, picWidth, picHigh);
                    imgBackgorund = Path.Combine(Server.MapPath("~/tempPhoto/border"), "imageBackgroundStraight_White.png");
                    borderStyle = "StraightStyle";
                }
                savedPhotoName = thumbPicOutPath; // 更新路徑
                // 合併外框              
                string resultPathFileName = directoryPath.CombinImage(savedPhotoName, savedBorderPhotoName, borderStyle, imgBackgorund);  // 照片 相框  
                TempData["Message"] = "合成完畢";
                // 儲存檔名(照片、外框、結果)
                directoryPath.file1PathFileName = photoName;
                directoryPath.fileBorderPathFileName = borderName;
                directoryPath.resultPathFileName = resultPathFileName;


                return RedirectToAction("Index", directoryPath);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }





    }
}