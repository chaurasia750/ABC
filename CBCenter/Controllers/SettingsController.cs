using CBCenter.CBCenterBL;
using CBCenter.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CBCenter.Controllers
{
    public class SettingsController : Controller
    {
        private CBCenterDBEntities dbentities = null;
        public SettingsController()
        {
            dbentities = new CBCenterDBEntities();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            dbentities.Dispose();
            dbentities = null;
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (dbentities != null)
            {
                dbentities.Dispose();
            }

            base.OnActionExecuted(filterContext);
        }
        
        public ActionResult AddBook()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddBook(HttpPostedFileBase fileUpload)
        {
            int sucess = 0;
            string filePath = string.Empty;
            if (fileUpload != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(fileUpload.FileName);
                string extension = Path.GetExtension(fileUpload.FileName);
                fileUpload.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);
               
                try
                {
                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;
                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();
                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();
                            }
                        }
                    }
                    conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.BookMaster";
                            //[OPTIONAL]: Map the Excel columns with that of the database table
                            sqlBulkCopy.ColumnMappings.Add("Code", "BookCode");
                            sqlBulkCopy.ColumnMappings.Add("NameofBooks", "BookName");
                            sqlBulkCopy.ColumnMappings.Add("Price", "Price");
                            sqlBulkCopy.ColumnMappings.Add("BookYearsId", "BookYearsId");
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                            sucess = 1;
                        }
                    }

                    if (sucess == 0)
                    {
                        TempData["Message"] = "Sorry ! Data is Not Uploaded";
                    }
                    else
                    {
                        TempData["Message"] = "All Book Name Saved Successfuly";
                    }
                }
                catch (Exception ex)
                {
                    sucess = 0;
                }
                
            }
            
                return View();
        }

        public ActionResult BookList()
        {
            List<BookMaster> bookList = null;
            using (dbentities)
            {
                bookList= dbentities.BookMasters.Where(x=>x.BookYearsId==2)
                    .OrderBy(x => x.BookId).ToList();
            }
       
            return View(bookList);
        }


        public ActionResult AddSchool()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddSchool(AddSchoolModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string schooName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(model.SchoolName.ToLower());
                    var saveSchool = dbentities.SchoolsMasters.Add(new SchoolsMaster
                    {
                        AddDate = DateTime.Now,
                        AlternateNo = model.AlternateNo,
                        ContactNo = model.ContactNo,
                        SchoolAddress = model.SchoolAddress,
                        SchoolGSTINO = model.GSTN,
                        SchoolName = schooName
                    });
                    int result = await dbentities.SaveChangesAsync();
                    if (result > 0)
                    {
                        ViewBag.Save = "New school added successfully";
                        ModelState.Clear();
                        schoolsMasters = null;
                        return View(new AddSchoolModel());
                    }
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("Exception", ex.Message);
                }

            }

            return View(model);
        }

        private static List<AddSchoolModel> schoolsMasters;

        [HttpGet]
        public JsonResult SchoolMasterList(string schoolName, string schoolAddress)
        {
            if (schoolsMasters == null)
            {
                schoolsMasters = new List<AddSchoolModel>();
                schoolsMasters = dbentities.SchoolsMasters.Select(x => new AddSchoolModel
                {
                    ContactNo = x.ContactNo,
                    SchoolName = x.SchoolName,
                    SchoolAddress = x.SchoolAddress,
                    GSTN = x.SchoolGSTINO == null ? "" : x.SchoolGSTINO
                }).ToList();
            }
            if (schoolAddress != null)
            {
                return Json(schoolsMasters.Where(x => x.SchoolName.ToLower().Contains(schoolName.ToLower().Trim()) && x.SchoolAddress.ToLower().Contains(schoolAddress.ToLower().Trim())), JsonRequestBehavior.AllowGet);
            }
            // schoolsMasters= schoolsMasters.Where(x => x.SchoolName.Contains(schoolName.Trim()) || x.SchoolAddress.Contains(schoolAddress.Trim())).ToList();
            return Json(schoolsMasters.Where(x => x.SchoolName.ToLower().Contains(schoolName.ToLower().Trim()) || x.SchoolAddress.ToLower().Contains(schoolAddress.ToLower().Trim())), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SchoolList()
        {
            var schools = dbentities.SchoolsMasters.Select(x => new AddSchoolModel
            {
                ContactNo = x.ContactNo,
                SchoolName = x.SchoolName,
                SchoolAddress = x.SchoolAddress,
                GSTN = x.SchoolGSTINO == null ? "" : x.SchoolGSTINO,
                AlternateNo = x.AlternateNo,
                Id = x.Id
            }).ToList();
            return View(schools);
        }

        [HttpPost]
        public ActionResult ShowUpdateSchoolModel(int id)
        {
            AddSchoolModel addSchool = dbentities.SchoolsMasters.Select(x => new AddSchoolModel
            {
                Id = x.Id,
                ContactNo = x.ContactNo,
                AlternateNo = x.AlternateNo,
                GSTN = x.SchoolGSTINO,
                SchoolAddress = x.SchoolAddress,
                SchoolName = x.SchoolName
            }).SingleOrDefault(x => x.Id == id);

            return PartialView("UpdateSchoolMaster", addSchool);
        }

        [HttpPost]
        public ActionResult UpdateSchoolMaster(AddSchoolModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = dbentities.SchoolsMasters.SingleOrDefault(x => x.Id == model.Id);
                    if (result != null)
                    {
                        result.ContactNo = model.ContactNo;
                        result.AlternateNo = model.AlternateNo;
                        result.SchoolAddress = model.SchoolAddress;
                        result.SchoolGSTINO = model.GSTN;
                        result.SchoolName = model.SchoolName;
                        dbentities.SaveChanges();
                        return Json(new { success = "success" });
                    }
                }
                catch
                {

                    return Json(new { error = "School is not Update" });
                }

            }
            return Json(new { error = "Model Error" });

        }
    }
}