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
            if(TempData["Message"] != null)
            {
                // border style
                ViewBag.borderStraightStylePathFileSrc = "tempPhoto/border/no-bgstraight.png";
                ViewBag.borderHorizontalStylePathFileSrc2 = "tempPhoto/border/no-bghorizontal.png";
                // reslut pic
                ViewBag.resultPathFileSrc = (directoryPath.resultPathFileName != null) ? "tempPhoto/result/" + directoryPath.resultPathFileName : "";
            }
            else
            {
                // 第一次loading進來
                // border style
                ViewBag.borderStraightStylePathFileSrc = "tempPhoto/border/no-bgstraight.png";
                ViewBag.borderHorizontalStylePathFileSrc2 = "tempPhoto/border/no-bghorizontal.png";
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImg(FormCollection collection, HttpPostedFileBase File1,string borderStyle)
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
                if (borderStyle == "StraightStyle")
                {
                    savedBorderPhotoName = Path.Combine(Server.MapPath("~/tempPhoto/border"), "no-bgstraight.png");
                    borderName = "no-bgstraight.png";
                }
                else if(borderStyle == "HorizontalStyle")
                {
                    savedBorderPhotoName = Path.Combine(Server.MapPath("~/tempPhoto/border"), "no-bghorizontal.png");
                    borderName = "no-bghorizontal.png";
                }
                             
                // 儲存檔案
                File1.SaveAs(savedPhotoName);
                
                // 合併外框
                //string resultPathFileName = directoryPath.CombinImage(savedBorderPhotoName, savedPhotoName); // 相框  照片
                string resultPathFileName = directoryPath.CombinImage(savedPhotoName, savedBorderPhotoName);  // 相框  照片
                 TempData["Message"] = "上傳成功";
                // 儲存檔名(照片、外框、結果)
                directoryPath.file1PathFileName = photoName;
                directoryPath.fileBorderPathFileName = borderName;
                directoryPath.resultPathFileName = resultPathFileName;


                return RedirectToAction("Index",directoryPath );
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }



        

    }
}