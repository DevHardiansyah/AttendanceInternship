using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using HRINTERNSHIP.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
// dashboard
namespace HRINTERNSHIP.Controllers
{
    public class EmployeeController : Controller
    {
        private DBModel db = new DBModel();
        // Dashboard
        // GET: Employee
        // show master attendance 
        // view = Testing2
        public ActionResult Login()
        {
            ViewBag.UserPC = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            ViewBag.UserPC2 = Environment.UserName;
            ViewBag.IPUser = Request.UserHostAddress;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password)
        {
            /* 
            authentication 
             */
            var data = db.ListUsers.Where(s => s.EMEMP.Equals(Username) && s.password.Equals(Password)).ToList();
            var intern = db.Intern_ListUsers.Where(s => s.username.Equals(Username) && s.password.Equals(Password)).ToList();
            if (data.Count() > 0)
            {
                Session["username"] = data.FirstOrDefault().EMEMP;
                Session["user"] = data.FirstOrDefault().EMNAME;
                Session["PaymentStatus"] = data.FirstOrDefault().EMPAYT;
                Session["Gender"] = data.FirstOrDefault().EMSEXT;
                Session["user_role"] = data.FirstOrDefault().role_name;
                Session["emjobd"] = data.FirstOrDefault().EMJOBD;                
                Session["facilityRaw"] = data.FirstOrDefault().EMCMPY; // facility using string format (PTMW or PTME)
                var getKPK = data.FirstOrDefault().EMEMP;
                var getStatusPassword = from s in db.Users where s.user_id == getKPK select s;
                var statusPassword = getStatusPassword.FirstOrDefault().status_update_password;
                //var news = from s in db.GeneralCmsNews select s;
                //var collectionNews = news.Where(x => x.NewsStatus.Equals("Active") || x.NewsStatus.Equals("TopNews")).ToList();
                //TempData["News"] = collectionNews;
                var roletype = data.FirstOrDefault().role_id;
                var getAuth = db.RolePermissions.Where(s => s.role_id.Equals(roletype)).ToList();

                //intern
                var kpk = Session["username"].ToString();
                var getintern = db.VwIntern_MasterDataIntern.Where(s => s.MENTOR == kpk || s.MANAGER == kpk && s.STATUS == "ACTIVE").ToList();
                if (getintern.Count > 0)
                {
                    Session["haveintern"] = "can";
                }
                foreach (var auth in getAuth)
                {
                    if (auth.permission_id == "1" && auth.status == "1")
                    {
                        Session["permission_1"] = "Can";
                    }

                    if (auth.permission_id == "2" && auth.status == "1")
                    {
                        Session["permission_2"] = "Can";
                    }

                    if (auth.permission_id == "3" && auth.status == "1")
                    {
                        Session["permission_3"] = "Can";
                    }

                    if (auth.permission_id == "4" && auth.status == "1")
                    {
                        Session["permission_4"] = "Can";
                    }

                    if (auth.permission_id == "5" && auth.status == "1")
                    {
                        Session["permission_5"] = "Can";
                    }

                    if (auth.permission_id == "6" && auth.status == "1")
                    {
                        Session["permission_6"] = "Can";
                    }

                    if (auth.permission_id == "7" && auth.status == "1")
                    {
                        Session["permission_7"] = "Can";
                    }

                    if (auth.permission_id == "8" && auth.status == "1")
                    {
                        Session["permission_8"] = "Can";
                    }

                    if (auth.permission_id == "9" && auth.status == "1")
                    {
                        Session["permission_9"] = "Can";
                    }

                    if (auth.permission_id == "10" && auth.status == "1")
                    {
                        Session["permission_10"] = "Can";
                    }

                    if (auth.permission_id == "11" && auth.status == "1")
                    {
                        Session["permission_11"] = "Can";
                    }

                    if (auth.permission_id == "12" && auth.status == "1")
                    {
                        Session["permission_12"] = "Can";
                    }

                    if (auth.permission_id == "13" && auth.status == "1")
                    {
                        Session["permission_13"] = "Can";
                    }

                    if (auth.permission_id == "14" && auth.status == "1")
                    {
                        Session["permission_14"] = "Can";
                    }

                    if (auth.permission_id == "15" && auth.status == "1")
                    {
                        Session["permission_15"] = "Can";
                    }

                    if (auth.permission_id == "16" && auth.status == "1")
                    {
                        Session["permission_16"] = "Can";
                    }

                    if (auth.permission_id == "17" && auth.status == "1")
                    {
                        Session["permission_17"] = "Can";
                    }

                    if (auth.permission_id == "18" && auth.status == "1")
                    {
                        Session["permission_18"] = "Can";
                    }

                    if (auth.permission_id == "19" && auth.status == "1")
                    {
                        Session["permission_19"] = "Can";
                    }

                    if (auth.permission_id == "20" && auth.status == "1")
                    {
                        Session["permission_20"] = "Can";
                    }

                    if (auth.permission_id == "21" && auth.status == "1")
                    {
                        Session["permission_21"] = "Can";
                    }

                    if (auth.permission_id == "22" && auth.status == "1")
                    {
                        Session["permission_22"] = "Can";
                    }

                    if (auth.permission_id == "23" && auth.status == "1")
                    {
                        Session["permission_23"] = "Can";
                    }

                    if (auth.permission_id == "24" && auth.status == "1")
                    {
                        Session["permission_24"] = "Can";
                    }

                    if (auth.permission_id == "25" && auth.status == "1")
                    {
                        Session["permission_25"] = "Can";
                    }

                    if (auth.permission_id == "26" && auth.status == "1")
                    {
                        Session["permission_26"] = "Can";
                    }

                    if (auth.permission_id == "27" && auth.status == "1")
                    {
                        Session["permission_27"] = "Can";
                    }
                    if (auth.permission_id == "28" && auth.status == "1")
                    {
                        Session["permission_28"] = "Can";
                    }
                    if (auth.permission_id == "29" && auth.status == "1")
                    {
                        Session["permission_29"] = "Can";
                    }
                    if (auth.permission_id == "30" && auth.status == "1")
                    {
                        Session["permission_30"] = "Can";
                    }

                    ////////////////////////////////////////////////////////

                    var kpkString = Session["username"].ToString();                  

                }


                // check if the password is not changed 

                if (statusPassword == "0")
                {
                    TempData["user_info"] = data;
                    return RedirectToAction("Lock_screeen");
                }


                return RedirectToAction("Dashboard");

            }
            else if (intern.Count > 0)
            {
                Session["username"] = intern.FirstOrDefault().username;
                Session["intern"] = intern.FirstOrDefault().user_id;
                Session["user"] = intern.FirstOrDefault().INTERNNAME;
                Session["PaymentStatus"] = "INTERNSHIP";
                Session["Gender"] = intern.FirstOrDefault().GENDER;
                Session["user_role"] = intern.FirstOrDefault().role_name;
                //email
                Session["university"] = intern.FirstOrDefault().UNIVERSITY;
                Session["isect"] = intern.FirstOrDefault().SECT_NAME;
                Session["idept"] = intern.FirstOrDefault().DEPT_NAME;
                Session["namementor"] = intern.FirstOrDefault().NAMEMENTOR;
                Session["namemanager"] = intern.FirstOrDefault().NAMEMANAGER;
                Session["emailmentor"] = intern.FirstOrDefault().EMAILMENTOR;
                Session["emailmanager"] = intern.FirstOrDefault().EMAILMANAGER;
                Session["emailintern"] = intern.FirstOrDefault().OFFICEMAIL;

                var getKPK = intern.FirstOrDefault().KPK;
                var getStatusPassword = from s in db.Users where s.user_id == getKPK select s;
                var statusPassword = getStatusPassword.FirstOrDefault().status_update_password;
                var roletype = intern.FirstOrDefault().roles_id;
                var getAuth = db.RolePermissions.Where(s => s.role_id.Equals(roletype)).ToList();
                foreach (var auth in getAuth)
                {
                    if (auth.permission_id == "1" && auth.status == "1")
                    {
                        Session["permission_1"] = "Can";
                    }

                    if (auth.permission_id == "2" && auth.status == "1")
                    {
                        Session["permission_2"] = "Can";
                    }

                    if (auth.permission_id == "3" && auth.status == "1")
                    {
                        Session["permission_3"] = "Can";
                    }

                    if (auth.permission_id == "4" && auth.status == "1")
                    {
                        Session["permission_4"] = "Can";
                    }

                    if (auth.permission_id == "5" && auth.status == "1")
                    {
                        Session["permission_5"] = "Can";
                    }

                    if (auth.permission_id == "6" && auth.status == "1")
                    {
                        Session["permission_6"] = "Can";
                    }

                    if (auth.permission_id == "7" && auth.status == "1")
                    {
                        Session["permission_7"] = "Can";
                    }

                    if (auth.permission_id == "8" && auth.status == "1")
                    {
                        Session["permission_8"] = "Can";
                    }

                    if (auth.permission_id == "9" && auth.status == "1")
                    {
                        Session["permission_9"] = "Can";
                    }

                    if (auth.permission_id == "10" && auth.status == "1")
                    {
                        Session["permission_10"] = "Can";
                    }

                    if (auth.permission_id == "11" && auth.status == "1")
                    {
                        Session["permission_11"] = "Can";
                    }

                    if (auth.permission_id == "12" && auth.status == "1")
                    {
                        Session["permission_12"] = "Can";
                    }

                    if (auth.permission_id == "13" && auth.status == "1")
                    {
                        Session["permission_13"] = "Can";
                    }

                    if (auth.permission_id == "14" && auth.status == "1")
                    {
                        Session["permission_14"] = "Can";
                    }

                    if (auth.permission_id == "15" && auth.status == "1")
                    {
                        Session["permission_15"] = "Can";
                    }

                    if (auth.permission_id == "16" && auth.status == "1")
                    {
                        Session["permission_16"] = "Can";
                    }

                    if (auth.permission_id == "17" && auth.status == "1")
                    {
                        Session["permission_17"] = "Can";
                    }

                    if (auth.permission_id == "18" && auth.status == "1")
                    {
                        Session["permission_18"] = "Can";
                    }

                    if (auth.permission_id == "19" && auth.status == "1")
                    {
                        Session["permission_19"] = "Can";
                    }

                    if (auth.permission_id == "20" && auth.status == "1")
                    {
                        Session["permission_20"] = "Can";
                    }

                    if (auth.permission_id == "21" && auth.status == "1")
                    {
                        Session["permission_21"] = "Can";
                    }

                    if (auth.permission_id == "22" && auth.status == "1")
                    {
                        Session["permission_22"] = "Can";
                    }

                    if (auth.permission_id == "23" && auth.status == "1")
                    {
                        Session["permission_23"] = "Can";
                    }

                    if (auth.permission_id == "24" && auth.status == "1")
                    {
                        Session["permission_24"] = "Can";
                    }

                    if (auth.permission_id == "25" && auth.status == "1")
                    {
                        Session["permission_25"] = "Can";
                    }

                    if (auth.permission_id == "26" && auth.status == "1")
                    {
                        Session["permission_26"] = "Can";
                    }

                    if (auth.permission_id == "27" && auth.status == "1")
                    {
                        Session["permission_27"] = "Can";
                    }
                    if (auth.permission_id == "28" && auth.status == "1")
                    {
                        Session["permission_28"] = "Can";
                    }
                    if (auth.permission_id == "29" && auth.status == "1")
                    {
                        Session["permission_29"] = "Can";
                    }
                    if (auth.permission_id == "30" && auth.status == "1")
                    {
                        Session["permission_30"] = "Can";
                    }

                }


                // check if the password is not changed 

                if (statusPassword == "0")
                {
                    TempData["user_info"] = intern;
                    return RedirectToAction("Lock_screeen");
                }
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Message = "error-login";
                return View("Login");
            }
        }

            public ActionResult Lock_screeen(){

            var getKPK = Session["username"];

           
                var getStatusPassword = from s in db.Users where s.username == getKPK select s;
                ViewBag.getPassword = getStatusPassword.FirstOrDefault().password;
                ViewBag.statusPassword = getStatusPassword.FirstOrDefault().status_update_password;
                return View();
         

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Lock_screeen(string password){
            TempData["success_update"] = "yes";
            var user_id =  Session["username"];
            db.Database.ExecuteSqlCommand("UPDATE Users SET password='"+password+"', status_update_password ='1' where username ='"+user_id+"' ");
            db.SaveChanges();
            return RedirectToAction("Login");
        }
        public JsonResult UpdatePassword(string password)
        {
            var user_id = Session["username"];
            db.Database.ExecuteSqlCommand("UPDATE Users SET password='" + password + "', status_update_password ='1' where username ='" + user_id + "' ");
            db.SaveChanges();
            return Json("success");
        }
        public ActionResult Logout()
        {
           
            Session.Clear();
            ViewBag.Message = "success-logout";
            return RedirectToAction("Login");
        }

        public ActionResult DetailNews(string slug)
        {
            return View();
        }



        /* 
                                          Attention

        This is the documentation code for attendance management 
        used by  View/Employee/Attendance.cshtml 

          functionality
          1. What a function do ?
          answer : to show all employee attendance 

          2. What the function's parameters or arguments are ?
          answer : kpk(EMEMP), term_start(start date periode attendance), term_end (end date attendance),Department_code(EMDEPT) ,
                  section_code(EMLOC#), job_code(EMJOBD) , q (EMEMP or EMNAME),  q_text(EMEMP OR EMNAME), EmployeeStatus( EMPAYT)

          3. What a function returns
          answer : all attendance data  based on kpk,start at , end at , department code, section , and status

          issue 

          1. filter data employee attendance non-staff between two years (example 23 december 2020 - 22 januari 2020)

          2. Sum Total Work Hours WG( Family 800)

       */        

        // to get section
        public JsonResult getEmployee(string kpk)
        {

            db.Configuration.ProxyCreationEnabled = false;
            var employee = from s in db.All_employee select s;
            if (kpk != null)
            {
                var sections = employee.Where(s => s.EMEMP == kpk).ToList();
                return Json(sections, JsonRequestBehavior.AllowGet);
            }
            return Json(employee, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getSection(string DepartementCode)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var section = from s in db.Sections select s;
            if (DepartementCode != null)
            {
                var sections = section.Where(s => s.Departement_code == DepartementCode).ToList();
                return Json(sections, JsonRequestBehavior.AllowGet);
            }
            return Json(section, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDepartement(string kpk)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var departement = from s in db.Departements select s;
            var result = departement.Where(d => db.All_employee.Any(m => m.EMDEPT == d.DEPT_CODE && m.EMEMP == kpk));
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // get last attendance
        /*  public JsonResult getLastIndex(string kpk)
            {
                var section = from s in db.Attendance_detail orderby s.date descending select s;
                if(kpk != null)
                {
                    var lastIndex = section.Where(s => s.employee_code == kpk).Take(1);
                    return Json(lastIndex, JsonRequestBehavior.AllowGet);
                }
                var lastIndexs = section.Where(s => s.employee_code == "100336").Take(1);
                return Json(lastIndexs, JsonRequestBehavior.AllowGet);
            } */




        // Bulk Insert Attendance Sheet *ABSENT* 
        [HttpPost]
        public ActionResult UploadAbsentSheet(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile.ContentType == "application/vnd.ms-excel" || postedFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
              

                string filename = postedFile.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                postedFile.SaveAs(targetpath + filename);
                string pathToExcelFile = targetpath + filename;
                var conString = "";

                conString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                DataTable dt = new DataTable();
                conString = string.Format(conString);
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

                using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {

                        using (SqlCommand command = new SqlCommand("", con))
                        {
                            try
                            {
                               
                                con.Open();
                                string strSql = "TRUNCATE TABLE Temp_attendance_detail";
                                SqlCommand   cmd=new SqlCommand(strSql,con);
                                cmd.ExecuteNonQuery();
                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();
                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_attendance_detail";
                                    sqlBulkCopy.ColumnMappings.Add("'EMCMPY", "'EMCMPY");
                                    sqlBulkCopy.ColumnMappings.Add("'EMEMP#", "'EMEMP#");
                                    sqlBulkCopy.ColumnMappings.Add("'EMNAME", "'EMNAME");
                                    sqlBulkCopy.ColumnMappings.Add("'EMCOMM", "'EMCOMM");
                                    sqlBulkCopy.ColumnMappings.Add("'EMEMST", "'EMEMST");
                                    sqlBulkCopy.ColumnMappings.Add("'EMJOBA", "'EMJOBA");
                                    sqlBulkCopy.ColumnMappings.Add("'EMDEPT", "'EMDEPT");
                                    sqlBulkCopy.ColumnMappings.Add("'EMLOC#", "'EMLOC#");
                                    sqlBulkCopy.ColumnMappings.Add("'DTIYMD", "'DTIYMD");
                                    sqlBulkCopy.ColumnMappings.Add("'DTDAY#", "'DTDAY#");
                                    sqlBulkCopy.ColumnMappings.Add("'DTSTAT", "'DTSTAT");
                                    sqlBulkCopy.ColumnMappings.Add("'DTSITM", "'DTSITM");
                                    sqlBulkCopy.ColumnMappings.Add("'DTSOTM", "'DTSOTM");
                                    sqlBulkCopy.ColumnMappings.Add("'DTSHFA", "'DTSHFA");
                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                command.CommandTimeout = 300;
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.CommandText = "ABSEN_SHEETS";
                                command.ExecuteScalar();
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = "upload-data-gagal";

                                return RedirectToAction("Attendance");     // Handle exception properly
                            }
                            finally
                            {
                                con.Close();
                            }
                        }


                    }
                }
            }
            ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
            ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
            ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
            ViewBag.Message = "upload-data-success";
            return RedirectToAction("Attendance");
        }

        [HttpPost]
        public ActionResult UploadScanSheet(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile.ContentType == "application/vnd.ms-excel" || postedFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = postedFile.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                postedFile.SaveAs(targetpath + filename);
                string pathToExcelFile = targetpath + filename;
                var conString = "";
                conString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                DataTable dt = new DataTable();
                conString = string.Format(conString);
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

                using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {

                        using (SqlCommand command = new SqlCommand("", con))
                        {
                            try
                            {
                             
                                con.Open();
                                string strSql = "TRUNCATE TABLE Temp_Attendance_master";
                            SqlCommand    cmd=new SqlCommand(strSql,con);
                                cmd.ExecuteNonQuery();

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_Attendance_master";
                                    sqlBulkCopy.ColumnMappings.Add("'DACMPY", "DACMPY"); 
                                    sqlBulkCopy.ColumnMappings.Add("'DAEMP#", "DAEMP#");
                                    sqlBulkCopy.ColumnMappings.Add("'EMNAME", "EMNAME");
                                    sqlBulkCopy.ColumnMappings.Add("'EMCOMM", "EMCOMM");
                                    sqlBulkCopy.ColumnMappings.Add("'DADEPT", "DADEPT");
                                    sqlBulkCopy.ColumnMappings.Add("'DALOC#", "'DALOC#");
                                    sqlBulkCopy.ColumnMappings.Add("'DAIYMD", "DAIYMD");
                                    sqlBulkCopy.ColumnMappings.Add("DAOYMD", " DAOYMD");
                                    sqlBulkCopy.ColumnMappings.Add("'DASITM", "DASITM");
                                    sqlBulkCopy.ColumnMappings.Add("'DASOTM", "DASOTM");
                                    sqlBulkCopy.ColumnMappings.Add("'DAEMBF", "DAEMBF");
                                    sqlBulkCopy.ColumnMappings.Add("'DAEMAF", "DAEMAF");
                                    sqlBulkCopy.ColumnMappings.Add("'DAIUPD", "DAIUPD");
                                    sqlBulkCopy.ColumnMappings.Add("DAOUPD", "DAOUPD");
                                    sqlBulkCopy.ColumnMappings.Add("'DASUP#", "DASUP");
                                    sqlBulkCopy.ColumnMappings.Add("'DASHFA", "DASHFA");
                                    sqlBulkCopy.ColumnMappings.Add("'DAWKHR", "DAWKHR");
                                    sqlBulkCopy.ColumnMappings.Add("''DAPSMN", "DAPSMN");
                                    sqlBulkCopy.ColumnMappings.Add("'DAWKMN", "DAWKMN");
                                    sqlBulkCopy.ColumnMappings.Add("'DAAPOT", "DAAPOT");
                                    sqlBulkCopy.ColumnMappings.Add("'DAENOT", "DAENOT");
                                    sqlBulkCopy.ColumnMappings.Add("'DADAY#", "DADAY");
                                    sqlBulkCopy.ColumnMappings.Add("'DADAYT", "DADAYT");
                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }
                                // Updating destination table, and dropping temp table
                                command.CommandTimeout = 300;
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.CommandText = "Attendance_detail_master";
                                command.ExecuteScalar();
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = "upload-data-gagal";
                                return RedirectToAction("Attendance");     // Handle exception properly
                            }
                            finally
                            {
                                con.Close();
                            }
                        }


                    }
                }
            }
            ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
            ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
            ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
            ViewBag.Message = "upload-data-success";
            return RedirectToAction("Attendance");
        }




        public ActionResult Dashboard(string DateOfBirth)
        {
            //if (Session["username"] != null && Session["intern"] == null)
            //{
            //    var user_login =  Session["username"].ToString();
            //    var getTime = DateTime.Now.ToString();

            //    var data = from s in db.EmployeeBirthDays select s;

            //    // birth day
            //    var filter_birth_day = data.Where(s => s.EMBIRT.Substring(0, 5).Equals(getTime.Substring(0, 5)) && s.EMEMP != "427412");

            //    // Service Award
            //    var getStatusUsers =  from s in db.Users where s.user_id == user_login select s;
            //    var queryData =  getStatusUsers.FirstOrDefault().status_update_password;
            //    ViewBag.getStatusPassword =  queryData;
            //    //ViewBag.GetEmployeeServices = data;
            //    ViewBag.getEmployeeWhoBirthDay =  data;
            //    return View(filter_birth_day.ToList());
            //}
            //else if(Session["intern"] != null)
            //{
            //    var user_login = Session["username"].ToString();
            //    var getTime = DateTime.Now.ToString();

            //    var data = from s in db.EmployeeBirthDays select s;

            //    // birth day
            //    var filter_birth_day = data.Where(s => s.EMBIRT.Substring(0, 5).Equals(getTime.Substring(0, 5)) && s.EMEMP != "427412");

            //    // Service Award
            //    var getStatusUsers = from s in db.Users where s.user_id == user_login select s;
            //    var queryData = getStatusUsers.FirstOrDefault().status_update_password;
            //    ViewBag.getStatusPassword = queryData;
            //    //ViewBag.GetEmployeeServices = data;
            //    ViewBag.getEmployeeWhoBirthDay = data;
            //    return View(filter_birth_day.ToList());
            //}
            //else
            //{
            //    return RedirectToAction("Login");
            //}
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                ViewBag.IP = ipList.Split(',')[0];
            }
            ViewBag.IP =  Request.ServerVariables["REMOTE_ADDR"];

            if (Session["username"] != null)
            {
                if(Session["intern"] != null) { 
                    var KPK = Session["intern"].ToString();
                    ViewBag.Activity = db.VwIntern_DashboardActLastWeek.Where(x => x.KPK == KPK).ToList();
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }


        public ActionResult Leave()
        {
            return View();
        }

        public ActionResult HistoryLeave()
        {
            if (Session["username"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public JsonResult detailShow(string q)
        {
            var data_masters = from m in db.All_employee let value = m.EMEMP let text = m.EMNAME select new { value, text };
            var data_master = data_masters.Where(m => m.value.Contains(q) || m.text.Contains(q));

            var jsonResult = Json(data_master, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public ActionResult Detailkpk(string q, string q_text)
        {
            var data = from s in db.All_employee select s;
            if (q != null || q_text != null)
            {
                var result = data.Where(s => s.EMEMP.Equals(q) || s.EMNAME.Equals(q_text));
                return View(result.Take(1));
            }
            ViewBag.GetKPK = q + ".jpg";
            return View(data.Take(1));
        }
        // Update date data with sql native command
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detailkpk(string EMBUSR, string LABEL_1, string LABEL_2, string LABEL_3, string EMEMP)
        {
            if (LABEL_3 == "")
            {
                LABEL_3 = null;
            }
            if (LABEL_1 == "")
            {
                LABEL_1 = null;
            }
            if (LABEL_2 == "")
            {
                LABEL_2 = null;
            }
            if (EMBUSR == "")
            {
                EMBUSR = null;
            }
            var data_filter = from s in db.All_employee select s;
            var query = (from q in db.Employees
                         where q.EMEMP == EMEMP
                         select q).First();
            query.EMBUSR = EMBUSR;
            query.LABEL_3 = LABEL_3;
            query.LABEL_1 = LABEL_1;
            query.LABEL_2 = LABEL_2;
            int result = db.SaveChanges();
            var results = data_filter.Where(s => s.EMEMP.Equals(EMEMP));
            return View(results.Take(1));
        }


        // create new data 
        public ActionResult createNewKpk()
        {
            if (Session["username"] != null)
            {
                ViewBag.EMDEPT = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                return View();
            }
            return View("Login");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult createNewKpk([Bind(Include = "EMCMPY,EMEMP,EMNAME,EMSEXT,EMCOMM,ENDCONT,OLDOCONT,EMEMST,EMJOBA,EMDEPT,EMLOC,EMJOBD,EMSUP_,SUPNAME,EMBIRT,EMMRST,EMRELI,EMGRAD,EMHTLC,EMBUSR,EMBUSS,EMBUSL,EMASN,EMWKHR,EMITST,EMBEDU,EMPAYT,EMADD1,EMADD2,EMADD3,EMEMAD,EMUNIO,EMUSID,EMITAX,EMSTAT,EMTERM,EMTREF,PHONE,EMAIL,TOTALHOURS,TOTALYEARS,DETAILWORKEXPERIENCE,FIRST_NAME,LAST_NAME,PSID,BUS_ROUTE,EOC,LABEL_1,LABEL_2,LABEL_3")] Employee employee, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string filename = file.FileName;
                string targetpath = Server.MapPath("~/Content/images/employee/");
                file.SaveAs(targetpath + employee.EMEMP + ".jpg");
                db.Employees.Add(employee);
                db.SaveChanges();
            }

            return View(employee);
        }

        public ActionResult BussineesIntelligence()
        {
            return View();
        }
    }
}