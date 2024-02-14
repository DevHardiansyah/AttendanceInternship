using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HRINTERNSHIP.Models;
using System.Web.Hosting;
using Hangfire;

namespace HRIS_Employee.Controllers
{
    public class InternController : Controller
    {
        // GET: Intern
        DBModel db = new DBModel();
        //////////
        //ADMIN//
        ////////
        [AllowAnonymous]
        public ActionResult MasterAttendanceIntern(string min_date, string max_date)
        {
            if (Session["username"] != null)
            {

                InternModels noinmaster = new InternModels();
                noinmaster.Intern_DeleteAttendanceNoInMaster();

                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                ViewBag.From = Convert.ToDateTime(iDate).ToString("yyyy-MM-dd");
                ViewBag.To = Convert.ToDateTime(oDate).ToString("yyyy-MM-dd");

                db.Database.ExecuteSqlCommand("DELETE Intern_AttendanceNew");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("INSERT INTO Intern_AttendanceNew Select * From VwIntern_AttendanceNew");
                db.SaveChanges();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        [AllowAnonymous]
        public JsonResult ListAttendanceIntern(string min_date, string max_date)
        {            
            if (Session["username"] != null)
            {
                if (min_date != "" && max_date != "")
                {
                    DateTime start = DateTime.Parse(min_date);
                    DateTime end = DateTime.Parse(max_date);

                    var List = db.Intern_AttendanceNew.ToList().Where(s => s.Date >= start && s.Date <= end).OrderBy(x => x.KPK);
                    var jsonResult = Json(List, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                    string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                    DateTime start = DateTime.Parse(iDate);
                    DateTime end = DateTime.Parse(oDate);

                    var List = db.Intern_AttendanceNew.ToList().Where(s => s.Date >= start && s.Date <= end).OrderBy(x => x.KPK);
                    var jsonResult = Json(List, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
            else
            {
                return Json(JsonRequestBehavior.AllowGet);
            }

        }
        

        public JsonResult UpdateAttendance(string kpk, string date, string clockin, string clockout, string atnid)
        {
            try
            {
                var dateclockin = "" + date + " " + clockin + "";
                var dateclockout = "" + date + " " + clockout + "";

                InternModels edit = new InternModels();
                edit.Updateattendance(kpk, date, dateclockin, dateclockout, atnid);
                db.Database.ExecuteSqlCommand("DELETE Intern_AttendanceNew");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("INSERT INTO Intern_AttendanceNew Select * From VwIntern_AttendanceNew");
                db.SaveChanges();
                return Json("Item updated successfully");
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        public ActionResult SetPeriode(string datefrom, string dateto)
        {
            try
            {
                var convertdatefrom = Convert.ToDateTime(datefrom).ToString("MM/dd/yyyy");
                var convertdateto = Convert.ToDateTime(dateto).ToString("MM/dd/yyyy");

                db.Database.ExecuteSqlCommand("Update Intern_Periode set DateFrom = '" + convertdatefrom + "', DateTo = '" + convertdateto + "'  where id = '443shaskdh'");
                db.SaveChanges();
                TempData["AllertSuccess"] = ("Success !");
            }
            catch (Exception error)
            {
                TempData["AllertError"] = ("Something went wrong!", error);
                return RedirectToAction("MasterAttendanceIntern");
            }
            return RedirectToAction("MasterAttendanceIntern");
        }
        [AllowAnonymous]
        public ActionResult Upload(HttpPostedFileBase postedFile)
        {
            try
            {
                string filePath = string.Empty;
                if (postedFile != null)
                {
                    string path = Server.MapPath("~/Doc/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Create a DataTable.
                    DataTable dt = new DataTable();
                    dt.Columns.AddRange(new DataColumn[9] { new DataColumn("Cardholder Name", typeof(string)),
                    new DataColumn("KPK", typeof(string)),
                    new DataColumn("Entry Clock", typeof(string)),
                    new DataColumn("Entry Card", typeof(string)),
                    new DataColumn("Entry Date/Time", typeof(string)),
                    new DataColumn("Exit Clock", typeof(string)),
                    new DataColumn("Exit Card", typeof(string)),
                    new DataColumn("Exit Date/Time", typeof(string)),
                    new DataColumn("Time Worked", typeof(string))});

                    //Read the contents of CSV file.
                    string csvData = System.IO.File.ReadAllText(filePath);

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            dt.Rows.Add();
                            int i = 0;

                            //Execute a loop over the columns.
                            foreach (string cell in row.Split(','))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell;
                                i++;
                            }
                        }
                    }
                    string a = ("Entry Date/Time");
                    var data = a.Split(' ').Where(s => s.StartsWith("#")).ToList();
                    
                    using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            using (SqlCommand command = new SqlCommand("", con))
                            {
                                try
                                {
                                    con.Open();
                                    //truncate table temp_employee then bulk with new data
                                    string strSql = "TRUNCATE TABLE Temp_AttendanceInternIn";
                                    SqlCommand cmd = new SqlCommand(strSql, con);
                                    cmd.ExecuteNonQuery();
                                    //Creating temp table on database
                                    //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                    // command.ExecuteNonQuery();
                                    //Bulk insert into temp table
                                    using (SqlBulkCopy SqlBulkCopy = new SqlBulkCopy(con))
                                    {
                                        SqlBulkCopy.BulkCopyTimeout = 660;
                                        SqlBulkCopy.DestinationTableName = "Temp_AttendanceInternIn";
                                        SqlBulkCopy.ColumnMappings.Add("[Cardholder Name]", "Cardholder Name");
                                        SqlBulkCopy.ColumnMappings.Add("KPK", "KPK");
                                        SqlBulkCopy.ColumnMappings.Add("[Entry Clock]", "Entry Clock");
                                        SqlBulkCopy.ColumnMappings.Add("[Entry Card]", "Entry Card");
                                        SqlBulkCopy.ColumnMappings.Add("[Entry Date/Time]", "Entry Date/Time");
                                        SqlBulkCopy.ColumnMappings.Add("[Exit Clock]", "Exit Clock");
                                        SqlBulkCopy.ColumnMappings.Add("[Exit Card]", "Exit Card");
                                        SqlBulkCopy.ColumnMappings.Add("[Exit Date/Time]", "Exit Date/Time");
                                        SqlBulkCopy.ColumnMappings.Add("[Time Worked]", "Time Worked");
                                        SqlBulkCopy.WriteToServer(dt);
                                        SqlBulkCopy.Close();
                                    }
                                    // Updating destination table, and dropping temp table
                                    command.CommandTimeout = 660;
                                    command.CommandType = System.Data.CommandType.StoredProcedure;
                                    db.Database.ExecuteSqlCommand("delete from Intern_Attendance where [Cardholder Name] like 'End%'");
                                    db.SaveChanges();
                                    db.Database.ExecuteSqlCommand("delete from Intern_Attendance where [Entry Clock] like '%Out%' or [Entry Clock] like '%OUT%'");
                                    db.SaveChanges();
                                    db.Database.ExecuteSqlCommand("delete from Temp_AttendanceInternIn where [Entry Clock] like '%Out%' or [Entry Clock] like '%OUT%'");
                                    db.SaveChanges();
                                }
                                catch (Exception error)
                                {
                                    ViewBag.Message = error;
                                    return RedirectToAction("MasterAttendanceIntern", error);
                                }
                                finally
                                {
                                    con.Close();
                                }
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
                                    //truncate table temp_employee then bulk with new data
                                    string strSql = "TRUNCATE TABLE Temp_AttendanceInternOut";
                                    SqlCommand cmd = new SqlCommand(strSql, con);
                                    cmd.ExecuteNonQuery();
                                    //Creating temp table on database
                                    //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                    // command.ExecuteNonQuery();
                                    //Bulk insert into temp table
                                    using (SqlBulkCopy SqlBulkCopy = new SqlBulkCopy(con))
                                    {
                                        SqlBulkCopy.BulkCopyTimeout = 660;
                                        SqlBulkCopy.DestinationTableName = "Temp_AttendanceInternOut";
                                        SqlBulkCopy.ColumnMappings.Add("[Cardholder Name]", "Cardholder Name");
                                        SqlBulkCopy.ColumnMappings.Add("KPK", "KPK");
                                        SqlBulkCopy.ColumnMappings.Add("[Entry Clock]", "Entry Clock");
                                        SqlBulkCopy.ColumnMappings.Add("[Entry Card]", "Entry Card");
                                        SqlBulkCopy.ColumnMappings.Add("[Entry Date/Time]", "Entry Date/Time");
                                        SqlBulkCopy.ColumnMappings.Add("[Exit Clock]", "Exit Clock");
                                        SqlBulkCopy.ColumnMappings.Add("[Exit Card]", "Exit Card");
                                        SqlBulkCopy.ColumnMappings.Add("[Exit Date/Time]", "Exit Date/Time");
                                        SqlBulkCopy.ColumnMappings.Add("[Time Worked]", "Time Worked");
                                        SqlBulkCopy.WriteToServer(dt);
                                        SqlBulkCopy.Close();
                                    }
                                    // Updating destination table, and dropping temp table
                                    command.CommandTimeout = 660;
                                    command.CommandType = System.Data.CommandType.StoredProcedure;
                                    db.Database.ExecuteSqlCommand("delete from Temp_AttendanceIntern where [Cardholder Name] like 'End%'");
                                    db.SaveChanges();
                                    db.Database.ExecuteSqlCommand("delete from Temp_AttendanceIntern where [Entry Clock] like '%IN%'");
                                    db.SaveChanges();
                                    db.Database.ExecuteSqlCommand("delete from Temp_AttendanceInternOut where [Entry Clock] like '%IN%'");
                                    db.SaveChanges();
                                    // call procedure Employee_bulk for insert to real table
                                    command.CommandText = "deletedoubledata";
                                    command.ExecuteScalar();
                                    command.CommandText = "AttendanceInternIn_bulk";
                                    command.ExecuteScalar();
                                    command.CommandText = "AttendanceInternOut_bulk";
                                    command.ExecuteScalar();
                                }
                                catch (Exception error)
                                {
                                    ViewBag.Message = error;
                                    return RedirectToAction("MasterAttendanceIntern", error);
                                }
                                finally
                                {
                                    con.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                //TempData["AllertSuccess"] = ("Failed Import !", error);
                return RedirectToAction("MasterAttendanceIntern");
            }
            db.Database.ExecuteSqlCommand("delete from Intern_Attendance where [Cardholder Name] like 'End%'");
            db.SaveChanges();
            db.Database.ExecuteSqlCommand("delete from Intern_Attendance where [Entry Clock] like '%Out%' or [Entry Clock] like '%OUT%'");
            db.SaveChanges();
            db.Database.ExecuteSqlCommand("delete from Temp_AttendanceInternIn where [Entry Clock] like '%Out%' or [Entry Clock] like '%OUT%'");
            db.SaveChanges();
            db.Database.ExecuteSqlCommand("delete from Temp_AttendanceIntern where [Cardholder Name] like 'End%'");
            db.SaveChanges();
            db.Database.ExecuteSqlCommand("delete from Temp_AttendanceIntern where [Entry Clock] like '%IN%'");
            db.SaveChanges();
            db.Database.ExecuteSqlCommand("delete from Temp_AttendanceInternOut where [Entry Clock] like '%IN%'");
            db.SaveChanges();

            //TempData["AllertSuccess"] = ("Success Import !");
            return RedirectToAction("MasterAttendanceIntern");
        }
        //End Of Master Attendance Intern

        //DownloadReport (admin)
        public ActionResult DownloadReport()
        {
            if (Session["username"] != null)
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate);
                return View(downloads.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public JsonResult ListDownloadReport(string min_date, string max_date)
        {
            if (min_date != "" && max_date != "")
            {
                var downloads = db.Intern_ListReportAttendanceIntern(min_date, max_date);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ListDownloadReportKPK(string min_date, string max_date, string KPK)
        {
            if (min_date != "" && max_date != "" && KPK != "")
            {

                string convkpk = " " + KPK + "";
                var downloads = db.Intern_ListReportAttendanceIntern(min_date, max_date).Where(S => S.KPK.Contains(convkpk));
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date == "" && max_date == "" && KPK != "")
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                string convkpk = " " + KPK + "";
                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate).Where(S => S.KPK.Contains(convkpk));
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ListDownloadReportName(string min_date, string max_date, string internname)
        {
            if (min_date != "" && max_date != "" && internname != "")
            {

                var downloads = db.Intern_ListReportAttendanceIntern(min_date, max_date).Where(S => S.INTERNNAME.Contains(internname));
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date == "" && max_date == "" && internname != "")
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate).Where(S => S.INTERNNAME.Contains(internname));
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ListDownloadReportStatus(string min_date, string max_date, string statusapproval)
        {
            if (min_date != "" && max_date != "" && statusapproval == "")
            {

                var downloads = db.Intern_ListReportAttendanceIntern(min_date, max_date);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date != "" && max_date != "" && statusapproval == "1")
            {

                var downloads = db.Intern_ListReportAttendanceIntern(min_date, max_date).Where(S => S.DateApprovedManager == null || S.DateApprovedMentor == null);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date != "" && max_date != "" && statusapproval == "2")
            {

                var downloads = db.Intern_ListReportAttendanceIntern(min_date, max_date).Where(s => s.StatusApproval == "Approved");
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date == "" && max_date == "" && statusapproval == "")
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date == "" && max_date == "" && statusapproval == "1")
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate).Where(S => S.DateApprovedManager == null || S.DateApprovedMentor == null);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date == "" && max_date == "" && statusapproval == "2")
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate).Where(s => s.StatusApproval == "Approved");
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                var downloads = db.Intern_ListReportAttendanceIntern(iDate, oDate);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ProcessForm(string FromDateReport, string ToDateReport)
        {
            if (System.IO.File.Exists(Server.MapPath
                              (@"~/zipfiles/bundle.zip")))
            {
                System.IO.File.Delete(Server.MapPath
                              (@"~/zipfiles/bundle.zip"));
            }
            ZipArchive zip = ZipFile.Open(Server.MapPath
                     (@"~/zipfiles/bundle.zip"), ZipArchiveMode.Create);

            var downloads = db.Intern_ListReportAttendanceIntern(FromDateReport, ToDateReport).Select(s => s.FilePatch);
            foreach (string file in downloads)
            {
                zip.CreateEntryFromFile(Server.MapPath
                     (@"~/AttendanceIntern/" + file), file);
            }
            zip.Dispose();
            return File(Server.MapPath(@"~/zipfiles/bundle.zip"),
                      "application/zip", "Attendance Intern.zip");
        }
        //End Download Report (admin)

        //////////////////////
        //ATTENDANCE RESULT//
        ////////////////////
        public ActionResult ReportPayment()
        {
            if (Session["username"] != null)
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string DatePeriod = db.Intern_Periode.FirstOrDefault().DateTo;
                DateTime start = Convert.ToDateTime(iDate);
                DateTime end = Convert.ToDateTime(DatePeriod);
                var intern = db.VwIntern_MasterDataIntern.ToList();
                var now = DateTime.Now.ToString("M/d/yyyy");
                foreach (var item in intern)
                {
                    int i = 0;
                    while (i < 1)
                    {
                        string KPK = item.KPK;
                        var data = db.Intern_AttendanceNew.Where(x => x.Date >= start && x.Date <= end && x.KPK == KPK).ToList();
                        var total_late = db.VwIntern_AbsLate.Where(s => s.KPK == KPK).Count();
                        var HalfWorkHour = data.Where(s => s.total_hour < 8).Count();
                        var TAtn = data.Where(s => s.Entry_Date_Time != null).Select(s => s.Entry_Date_Time).Count();
                        var total_abs1 = data.Where(s => (s.Entry_Date_Time == null && s.Entry_Out == null && s.Date_public_holidays == null && Convert.ToDateTime(s.Date) <= Convert.ToDateTime(now))).ToList();
                        var total_abs2 = total_abs1.Where(s => Convert.ToDateTime(s.test).ToString("ddd") != "Sun").ToList();
                        var TAbs = total_abs2.Where(s => Convert.ToDateTime(s.test).ToString("ddd") != "Sat").Count();

                        var noholiday = data.Where(s => s.Date_public_holidays == null).ToList();
                        var nosun = noholiday.Where(s => Convert.ToDateTime(s.test).ToString("ddd") != "Sun").ToList();
                        var nosat = nosun.Where(s => Convert.ToDateTime(s.test).ToString("ddd") != "Sat").ToList();
                        //var TDay = nosat.Count();
                        var TDay = 22;
                        int Report_ID = 0;

                        var Action = "Insert";
                        InternModels edit = new InternModels();
                        edit.InsertUpdatePayment(KPK, TAtn, TAbs, HalfWorkHour, TDay, Report_ID, DatePeriod, Action);
                        i++;
                    }
                }
                InternModels payment = new InternModels();
                payment.MergePayment();

                TempData["AllertSuccess"] = ("Success Generate !");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
            //return View();
        }
        public JsonResult ListReportPayment(string periode)
        {
            if (periode != "")
            {
                var payment = db.VwIntern_ReportPayment.Where(s => s.DatePeriod == periode);
                return Json(payment, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string DatePeriod = db.Intern_Periode.FirstOrDefault().DateTo;
                string cnvDate = Convert.ToDateTime(DatePeriod).ToString("yyyy-MM");
                var payment = db.VwIntern_ReportPayment.Where(s => s.DatePeriod == cnvDate).ToList();
                return Json(payment, JsonRequestBehavior.AllowGet);
            }

        }
        //End Report Payment
        public JsonResult getMasterIntern()
        {
            return Json(db.VwIntern_MasterDataIntern.ToList(), JsonRequestBehavior.AllowGet);
        }

        ///////////////////////
        //DATA MASTER INTERN//
        /////////////////////
        public ActionResult DataIntern()
        {
            if (Session["username"] != null)
            {
                ViewBag.Mentor = db.All_employee.Where(s => s.EMPAYT == "M").ToList();
                ViewBag.Dept = db.Departements.ToList();
                ViewBag.Section = db.Sections.ToList();
                ViewBag.DataIntern = db.VwIntern_MasterDataIntern.ToList();
                ViewBag.AllIntern = db.VwIntern_MasterDataIntern.Count();
                ViewBag.TotalActive = db.VwIntern_MasterDataIntern.Where(x =>x.STATUS=="Active").Count();
                ViewBag.TotalEOC = db.VwIntern_MasterDataIntern.Where(x => x.STATUS == "NonActive").Count();

                //autoincrement
                var cek = db.VwIntern_MasterDataIntern.Count();
                if (cek == 0)
                {
                    ViewBag.ResultKPK = "I00104";
                }
                else
                {
                    var list = db.VwIntern_MasterDataIntern.OrderByDescending(x => x.KPK);
                    var getkpk = list.FirstOrDefault().KPK;
                    var strkpk = Int32.Parse(getkpk.Substring(2, 5));
                    var autokpk = strkpk + 1;

                    if (autokpk == 1)
                    {
                        ViewBag.ResultKPK = "I000" + autokpk.ToString() + "";
                    }
                    else if (autokpk <= 99)
                    {
                        ViewBag.ResultKPK = "I000" + autokpk.ToString() + "";
                    }
                    else if (autokpk <= 999)
                    {
                        ViewBag.ResultKPK = "I00" + autokpk.ToString() + "";
                    }
                    else if (autokpk <= 9999)
                    {
                        ViewBag.ResultKPK = "I0" + autokpk.ToString() + "";
                    }
                    else if (autokpk <= 99999)
                    {
                        ViewBag.ResultKPK = "I" + autokpk.ToString() + "";
                    }
                    else
                    {
                        ViewBag.ResultKPK = "I" + autokpk.ToString() + "";
                    }
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public ActionResult DetailsIntern(int q)
        {

            ViewBag.Mentor = db.All_employee.Where(s => s.EMPAYT == "M").ToList();
            ViewBag.Dept = db.Departements.ToList();
            ViewBag.Section = db.Sections.ToList();
            var list = db.VwIntern_MasterDataIntern.Where(s => s.IDINTERN == q).ToList();

            foreach (var item in list)
            {
                ViewBag.NIK = item.NIK;
                ViewBag.NIM = item.NIM;
                ViewBag.Name = item.INTERNNAME;
                ViewBag.Gender = item.GENDER;
                ViewBag.DOB = Convert.ToDateTime(item.DOB).ToString("yyyy-MM-dd");
                ViewBag.Phone = item.PHONE;
                ViewBag.Personalmail = item.PERSONALMAIL;
                ViewBag.Major = item.MAJOR;
                ViewBag.University = item.UNIVERSITY;
                ViewBag.Address = item.ADDRESS;
                ViewBag.KPK = item.KPK;
                ViewBag.Budget = item.BUDGET;
                ViewBag.Departemen = item.DEPT;
                ViewBag.Sect = item.SECTION;
                ViewBag.Project = item.PROJECT;
                ViewBag.Mentorr = item.MENTOR;
                ViewBag.Managerr = item.MANAGER;
                ViewBag.Joindate = Convert.ToDateTime(item.JOINDATE).ToString("yyyy-MM-dd");
                ViewBag.Enddate = Convert.ToDateTime(item.ENDDATE).ToString("yyyy-MM-dd");
                ViewBag.Salary = item.SALARY;
                ViewBag.Extend = item.EXTEND;
            }
            return View();
        }
        //crud
        public ActionResult AddIntern(string kpk, string internname, string budget, string dept, string section, string dob, string phone, string personalmail, string officemail, string university, string major, string project, string mentor, string manager, string joindate, string enddate, string extend, string salary, string nim, string nik, string address, string gender)
        {
            var yearFrom = joindate.Substring(0, 4);
            var monthFrom = joindate.Substring(5, 2);
            var dayaFrom = joindate.Substring(8, 2);

            var yearTo = enddate.Substring(0, 4);
            var monthTo = enddate.Substring(5, 2);
            var dayaTo = enddate.Substring(8, 2);

            string upper = internname.ToUpper();

            var convertdob = Convert.ToDateTime(dob).ToString("MM/dd/yyyy");
            var convertjoindate = Convert.ToDateTime(joindate).ToString("MM/dd/yyyy");
            var convertenddate = Convert.ToDateTime(enddate).ToString("MM/dd/yyyy");

            //extend
            if (extend != "")
            {
                try
                {
                    var convertextend = Convert.ToDateTime(extend).ToString("MM/dd/yyyy");
                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_MasterData VALUES (' " + kpk + "', '" + upper + "','" + budget + "', '" + dept + "', '" + section + "', '" + convertdob + "', '" + phone + "', '" + personalmail + "', '" + officemail + "', '" + university + "', '" + major + "', '" + project + "', '" + mentor + "', '" + manager + "', '" + convertjoindate + "', '" + convertenddate + "', '" + convertextend + "', '" + salary + "', '" + nim + "', '" + nik + "', '" + address + "', '" + gender + "','Active')");
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("INSERT INTO Users VALUES ('" + kpk + "', '" + kpk + "', '5',' " + kpk + "', '0')");
                    db.SaveChanges();
                    var yearFromex = joindate.Substring(0, 4);
                    var monthFromex = joindate.Substring(5, 2);
                    var dayaFromex = joindate.Substring(8, 2);

                    var yearToex = extend.Substring(0, 4);
                    var monthToex = extend.Substring(5, 2);
                    var dayaToex = extend.Substring(8, 2);

                    DateTime dtStart = new DateTime(Int32.Parse(yearFromex), Int32.Parse(monthFromex), Int32.Parse(dayaFromex));
                    DateTime dtEnd = new DateTime(Int32.Parse(yearToex), Int32.Parse(monthToex), Int32.Parse(dayaToex));

                    //insert to first join
                    var datefirst = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES (' " + kpk + "', '" + upper + "', '" + datefirst + "')");
                    db.SaveChanges();
                    while (dtStart < dtEnd)
                    {
                        dtStart = dtStart.AddDays(1);
                        var date = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES (' " + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Temp_AttendanceIntern (KPK, [Cardholder Name], Date) VALUES (' " + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                    }
                }
                catch (Exception error)
                {
                    TempData["AllertError"] = ("Something went wrong!", error);
                    return RedirectToAction("DataIntern");
                }
                TempData["AllertSuccess"] = ("Intern Success Added !");
                return RedirectToAction("DataIntern");
            }
            else
            {
                try
                {

                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_MasterData VALUES (' " + kpk + "', '" + upper + "','" + budget + "', '" + dept + "', '" + section + "', '" + convertdob + "', '" + phone + "', '" + personalmail + "', '" + officemail + "', '" + university + "', '" + major + "', '" + project + "', '" + mentor + "', '" + manager + "', '" + convertjoindate + "', '" + convertenddate + "', '" + convertenddate + "', '" + salary + "', '" + nim + "', '" + nik + "', '" + address + "', '" + gender + "','Active')");
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("INSERT INTO Users VALUES ('" + kpk + "', '" + kpk + "', '5',' " + kpk + "', '0')");
                    db.SaveChanges();

                    DateTime dtStart = new DateTime(Int32.Parse(yearFrom), Int32.Parse(monthFrom), Int32.Parse(dayaFrom));
                    DateTime dtEnd = new DateTime(Int32.Parse(yearTo), Int32.Parse(monthTo), Int32.Parse(dayaTo));

                    var datefirst = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES (' " + kpk + "', '" + upper + "', '" + datefirst + "')");
                    db.SaveChanges();
                    while (dtStart < dtEnd)
                    {
                        dtStart = dtStart.AddDays(1);
                        var date = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES (' " + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Temp_AttendanceIntern (KPK, [Cardholder Name], Date) VALUES (' " + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                    }
                }
                catch (Exception error)
                {
                    TempData["AllertError"] = ("Something went wrong!", error);
                    return RedirectToAction("DataIntern");
                }
                TempData["AllertSuccess"] = ("Intern Success Added !");
                return RedirectToAction("DataIntern");
            }
        }
        public ActionResult UpdateIntern(string kpk, string internname, string budget, string dept, string section, string dob, string phone, string personalmail, string officemail, string university, string major, string project, string mentor, string manager, string joindate, string enddate, string extend, string salary, string nim, string nik, string address, string gender, string joindateold, string enddateold)
        {

            var convertdob = Convert.ToDateTime(dob).ToString("MM/dd/yyyy");
            var convertjoindate = Convert.ToDateTime(joindate).ToString("MM/dd/yyyy");
            var convertenddate = Convert.ToDateTime(enddate).ToString("MM/dd/yyyy");

            string upper = internname.ToUpper();
           
            //extend
            if (extend != "")
            {
                var convertextend = Convert.ToDateTime(extend).ToString("MM/dd/yyyy");
                db.Database.ExecuteSqlCommand("Update Intern_MasterData Set EXTEND = '" + convertextend + "' where KPK = '" + kpk + "'");
                db.SaveChanges();
                var yearFrom = enddate.Substring(0, 4);
                var monthFrom = enddate.Substring(5, 2);
                var dayaFrom = enddate.Substring(8, 2);

                var yearTo = extend.Substring(0, 4);
                var monthTo = extend.Substring(5, 2);
                var dayaTo = extend.Substring(8, 2);
                db.Database.ExecuteSqlCommand("Update Intern_MasterData Set INTERNNAME = '" + upper + "',BUDGET = '" + budget + "',DEPT = '" + dept + "',SECTION = '" + section + "',DOB = '" + convertdob + "',PHONE = '" + phone + "',PERSONALMAIL = '" + personalmail + "',OFFICEMAIL = '" + officemail + "',UNIVERSITY = '" + university + "',MAJOR = '" + major + "',PROJECT = '" + project + "',MENTOR = '" + mentor + "',MANAGER = '" + manager + "',JOINDATE = '" + convertjoindate + "',ENDDATE = '" + convertenddate + "',SALARY = '" + salary + "',NIM = '" + nim + "',NIK = '" + nik + "',ADDRESS = '" + address + "',GENDER = '" + gender + "' where KPK = '" + kpk + "'");
                db.SaveChanges();
                DateTime dtStart = new DateTime(Int32.Parse(yearFrom), Int32.Parse(monthFrom), Int32.Parse(dayaFrom));
                DateTime dtEnd = new DateTime(Int32.Parse(yearTo), Int32.Parse(monthTo), Int32.Parse(dayaTo));
                while (dtStart < dtEnd)
                {
                    dtStart = dtStart.AddDays(1);
                    var date = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                    db.SaveChanges();

                    int attendancebefore = db.Intern_Attendance.Where(s => s.Date.Equals(date) && s.KPK.Equals(kpk)).Count();

                    if (attendancebefore > 0)
                    {
                        db.Database.ExecuteSqlCommand("UPDATE Intern_Attendance Set Date = '" + date + "' where Date = '" + date + "' and KPK = '" + kpk + "'");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("UPDATE Temp_AttendanceIntern Set Date = '" + date + "' where Date = '" + date + "' and KPK = '" + kpk + "'");
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Temp_AttendanceIntern (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                    }
                }
            }
            if(joindate != joindateold)
            {
                var yearFrom = joindate.Substring(0, 4);
                var monthFrom = joindate.Substring(5, 2);
                var dayaFrom = joindate.Substring(8, 2);

                var yearTo = joindateold.Substring(0, 4);
                var monthTo = joindateold.Substring(5, 2);
                var dayaTo = joindateold.Substring(8, 2);

                DateTime dtStart = new DateTime(Int32.Parse(yearFrom), Int32.Parse(monthFrom), Int32.Parse(dayaFrom));
                DateTime dtEnd = new DateTime(Int32.Parse(yearTo), Int32.Parse(monthTo), Int32.Parse(dayaTo));

                if(dtStart < dtEnd)
                {
                    var datefirst = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + datefirst + "')");
                    db.SaveChanges();
                    while (dtEnd > dtStart)
                    {
                        dtStart = dtStart.AddDays(1);
                        var date = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                        db.SaveChanges();

                        int attendancebefore = db.Intern_Attendance.Where(s => s.Date.Equals(date) && s.KPK.Equals(kpk)).Count();

                        if (attendancebefore > 0)
                        {
                            db.Database.ExecuteSqlCommand("UPDATE Intern_Attendance Set Date = '" + date + "' where Date = '" + date + "' and KPK = '" + kpk + "'");
                            db.SaveChanges();
                            db.Database.ExecuteSqlCommand("UPDATE Temp_AttendanceIntern Set Date = '" + date + "' where Date = '" + date + "' and KPK = '" + kpk + "'");
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                            db.SaveChanges();
                            db.Database.ExecuteSqlCommand("INSERT INTO Temp_AttendanceIntern (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                            db.SaveChanges();
                        }
                    }
                }
            }

            if (enddate != enddateold)
            {
                var yearFrom = enddate.Substring(0, 4);
                var monthFrom = enddate.Substring(5, 2);
                var dayaFrom = enddate.Substring(8, 2);

                var yearTo = enddateold.Substring(0, 4);
                var monthTo = enddateold.Substring(5, 2);
                var dayaTo = enddateold.Substring(8, 2);

                DateTime dtStart = new DateTime(Int32.Parse(yearFrom), Int32.Parse(monthFrom), Int32.Parse(dayaFrom));
                DateTime dtEnd = new DateTime(Int32.Parse(yearTo), Int32.Parse(monthTo), Int32.Parse(dayaTo));

                if (dtStart > dtEnd)
                {
                    var datefirst = dtEnd.ToString("M/d/yyyy h:mm:ss tt");
                    var convertextend = Convert.ToDateTime(enddate).ToString("MM/dd/yyyy");
                    //db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + datefirst + "')");
                    db.Database.ExecuteSqlCommand("Update Intern_MasterData Set EXTEND = '" + convertextend + "' where KPK = '" + kpk + "'");
                    db.SaveChanges();
                    while (dtEnd < dtStart)
                    {
                        dtEnd = dtEnd.AddDays(1);
                        var date = dtEnd.ToString("M/d/yyyy h:mm:ss tt");
                        db.SaveChanges();

                        int attendancebefore = db.Intern_Attendance.Where(s => s.Date.Equals(date) && s.KPK.Equals(kpk)).Count();

                        if (attendancebefore > 0)
                        {
                            db.Database.ExecuteSqlCommand("UPDATE Intern_Attendance Set Date = '" + date + "' where Date = '" + date + "' and KPK = '" + kpk + "'");
                            db.SaveChanges();
                            db.Database.ExecuteSqlCommand("UPDATE Temp_AttendanceIntern Set Date = '" + date + "' where Date = '" + date + "' and KPK = '" + kpk + "'");
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                            db.SaveChanges();
                            db.Database.ExecuteSqlCommand("INSERT INTO Temp_AttendanceIntern (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                            db.SaveChanges();
                        }
                    }
                }
            }

            try
            {
                db.Database.ExecuteSqlCommand("Update Intern_MasterData Set INTERNNAME = '" + upper + "',BUDGET = '" + budget + "',DEPT = '" + dept + "',SECTION = '" + section + "',DOB = '" + convertdob + "',PHONE = '" + phone + "',PERSONALMAIL = '" + personalmail + "',OFFICEMAIL = '" + officemail + "',UNIVERSITY = '" + university + "',MAJOR = '" + major + "',PROJECT = '" + project + "',MENTOR = '" + mentor + "',MANAGER = '" + manager + "',JOINDATE = '" + convertjoindate + "',ENDDATE = '" + convertenddate + "',SALARY = '" + salary + "',NIM = '" + nim + "',NIK = '" + nik + "',ADDRESS = '" + address + "',GENDER = '" + gender + "' where KPK = '" + kpk + "'");
                db.SaveChanges();
                TempData["AllertSuccess"] = ("Update Success !");
            }
            catch (Exception error)
            {
                TempData["AllertError"] = ("Something went wrong!", error);
                return RedirectToAction("DataIntern");
            }
            return RedirectToAction("DataIntern");
        }
        public ActionResult DeleteIntern(string kpk, string internname)
        {
            var data = db.VwIntern_MasterDataIntern.Where(x => x.KPK == kpk).ToList();
            var joindate = data.FirstOrDefault().JOINDATE;
            var enddate = data.FirstOrDefault().ENDDATE;
            var extend = data.FirstOrDefault().EXTEND;
            if (enddate == extend)
            {
                try
                {
                    db.Database.ExecuteSqlCommand("Update Intern_MasterData set Status = 'NonActive' where kpk = '" + kpk + "'");
                    db.SaveChanges();


                    db.Database.ExecuteSqlCommand("insert into Intern_DetailContract (KPK,DJoinDate,DEndDate) values('" + kpk + "','" + joindate + "','" + enddate + "')");
                    db.SaveChanges();
                    TempData["AllertSuccess"] = ("Delete Success !");
                }
                catch (Exception error)
                {
                    TempData["AllertError"] = ("Something went wrong!", error);
                    return RedirectToAction("DataIntern");
                }
            }
            else
            {
                try
                {
                    db.Database.ExecuteSqlCommand("Update Intern_MasterData set Status = 'NonActive' where kpk = '" + kpk + "'");
                    db.SaveChanges();


                    db.Database.ExecuteSqlCommand("insert into Intern_DetailContract values('" + kpk + "','" + joindate + "','" + enddate + "','" + extend + "')");
                    db.SaveChanges();
                    TempData["AllertSuccess"] = ("Delete Success !");
                }
                catch (Exception error)
                {
                    TempData["AllertError"] = ("Something went wrong!", error);
                    return RedirectToAction("DataIntern");
                }

            }
           
            return RedirectToAction("DataIntern");
        }
        public ActionResult NewContract(string kpk, string joindate, string enddate, string extend)
        {
            var yearFrom = joindate.Substring(0, 4);
            var monthFrom = joindate.Substring(5, 2);
            var dayaFrom = joindate.Substring(8, 2);

            var yearTo = enddate.Substring(0, 4);
            var monthTo = enddate.Substring(5, 2);
            var dayaTo = enddate.Substring(8, 2);


            var convertjoindate = Convert.ToDateTime(joindate).ToString("MM/dd/yyyy");
            var convertenddate = Convert.ToDateTime(enddate).ToString("MM/dd/yyyy");

            var upper = db.VwIntern_MasterDataIntern.Where(x => x.KPK == kpk).FirstOrDefault().INTERNNAME;
            //extend
            if (extend != "")
            {
                try
                {
                    var convertextend = Convert.ToDateTime(extend).ToString("MM/dd/yyyy");

                    var yearFromex = joindate.Substring(0, 4);
                    var monthFromex = joindate.Substring(5, 2);
                    var dayaFromex = joindate.Substring(8, 2);

                    var yearToex = extend.Substring(0, 4);
                    var monthToex = extend.Substring(5, 2);
                    var dayaToex = extend.Substring(8, 2);

                    db.Database.ExecuteSqlCommand("Update Intern_MasterData Set Status = 'Active', JOINDATE = '" + convertjoindate + "',ENDDATE = '" + convertenddate + "',EXTEND = '" + convertextend + "' where KPK = '" + kpk + "'");
                    db.SaveChanges();

                    DateTime dtStart = new DateTime(Int32.Parse(yearFromex), Int32.Parse(monthFromex), Int32.Parse(dayaFromex));
                    DateTime dtEnd = new DateTime(Int32.Parse(yearToex), Int32.Parse(monthToex), Int32.Parse(dayaToex));

                    //insert to first join
                    var datefirst = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + datefirst + "')");
                    db.SaveChanges();
                    while (dtStart < dtEnd)
                    {
                        dtStart = dtStart.AddDays(1);
                        var date = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Temp_AttendanceIntern (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                    }
                }
                catch (Exception error)
                {
                    TempData["AllertError"] = ("Something went wrong!", error);
                    return RedirectToAction("DataIntern");
                }
                TempData["AllertSuccess"] = ("Contact Success Added !");
                return RedirectToAction("DataIntern");
            }
            else
            {
                try
                {

                    DateTime dtStart = new DateTime(Int32.Parse(yearFrom), Int32.Parse(monthFrom), Int32.Parse(dayaFrom));
                    DateTime dtEnd = new DateTime(Int32.Parse(yearTo), Int32.Parse(monthTo), Int32.Parse(dayaTo));

                    db.Database.ExecuteSqlCommand("Update Intern_MasterData Set Status = 'Active', JOINDATE = '" + convertjoindate + "',ENDDATE = '" + convertenddate + "',EXTEND = '" + convertenddate + "' where KPK = '" + kpk + "'");
                    db.SaveChanges();

                    var datefirst = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + datefirst + "')");
                    db.SaveChanges();
                    while (dtStart < dtEnd)
                    {
                        dtStart = dtStart.AddDays(1);
                        var date = dtStart.ToString("M/d/yyyy h:mm:ss tt");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Intern_Attendance (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("INSERT INTO Temp_AttendanceIntern (KPK, [Cardholder Name], Date) VALUES ('" + kpk + "', '" + upper + "', '" + date + "')");
                        db.SaveChanges();
                    }
                }
                catch (Exception error)
                {
                    TempData["AllertError"] = ("Something went wrong!", error);
                    return RedirectToAction("DataIntern");
                }
                TempData["AllertSuccess"] = ("Contact Success Added !");
                return RedirectToAction("DataIntern");
            }
        }
        public JsonResult DetailContract(string KPK)
        {
            var detail = db.Intern_DetailContract.Where(s => s.KPK == KPK);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        public JsonResult detailShow(string q)
        {
            var data_masters = from m in db.All_employee let value = m.EMEMP let text = m.EMNAME select new { value, text };
            var data_master = data_masters.Where(m => m.value.Contains(q) || m.text.Contains(q));

            var jsonResult = Json(data_master, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        //End Master Data Intern

        ///////////////
        //INTERNSHIP//
        //////////////
        public ActionResult AttendanceIntern(string min_date, string max_date)
        {
            if (Session["username"] != null)
            {
                db.Database.ExecuteSqlCommand("DELETE Intern_AttendanceNew");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("INSERT INTO Intern_AttendanceNew Select * From VwIntern_AttendanceNew");
                db.SaveChanges();
                string KPK = Session["intern"].ToString();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public JsonResult ListAttendanceForIntern(string min_date, string max_date)
        {
            string KPK = Session["intern"].ToString();
            {
                if (min_date != "" && max_date != "")
                {
                    DateTime start = DateTime.Parse(min_date);
                    DateTime end = DateTime.Parse(max_date);

                    var List = db.Intern_AttendanceNew.ToList().Where(s => s.Date >= start && s.Date <= end && s.KPK == KPK).OrderBy(x => x.KPK);
                    var jsonResult = Json(List, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                    string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                    DateTime start = DateTime.Parse(iDate);
                    DateTime end = DateTime.Parse(oDate);

                    var List = db.Intern_AttendanceNew.ToList().Where(s => s.Date >= start && s.Date <= end && s.KPK == KPK).OrderBy(x => x.KPK);
                    var jsonResult = Json(List, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
        }
        public JsonResult AddActivity(string KPK, string Date, string Activity, string Activity_ID)
        {
            if (Activity_ID == "")
            {
                try
                {
                    db.Database.ExecuteSqlCommand("DELETE Intern_AttendanceNew");
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_AttendanceNew Select * From VwIntern_AttendanceNew");
                    db.SaveChanges();
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    string s = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString("hh:mm:ss tt");
                    var action = "Insert";

                    InternModels edit = new InternModels();
                    edit.InsertUpdateActivity(KPK, Date, Activity, Activity_ID, action);
                    InternModels activity = new InternModels();
                    activity.MergeActivity();

                    return Json("Item updated successfully");
                }
                catch (Exception e)
                {
                    return Json(e.Message);
                }
            }
            else
            {
                try
                {
                    db.Database.ExecuteSqlCommand("DELETE Intern_AttendanceNew");
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("INSERT INTO Intern_AttendanceNew Select * From VwIntern_AttendanceNew");
                    db.SaveChanges();
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    string s = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString("hh:mm:ss tt");
                    var action = "Update";

                    InternModels edit = new InternModels();
                    edit.InsertUpdateActivity(KPK, Date, Activity, Activity_ID, action);
                    InternModels activity = new InternModels();
                    activity.MergeActivity();

                    return Json("Item updated successfully");
                }
                catch (Exception error)
                {
                    return Json(error.Message);
                }
            }
        }
        public ActionResult AddDailyActivity(string KPK, string Date, string Activity, string Activity_ID)
        {
            if (Session["username"] != null)
            {
                if (Activity_ID == "" || Activity_ID == null)
                {
                    db.Database.ExecuteSqlCommand("Insert into Intern_Activity(KPK, Date, Activity) values('" + KPK + "', '" + Date + "', '" + Activity + "')");
                    db.SaveChanges();
                    TempData["AllertSuccess"] = ("Add Success !");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("Update Intern_Activity set Date='" + Date + "', Activity='" + Activity + "' where Activity_ID='" + Activity_ID + "'");
                    db.SaveChanges();
                    TempData["AllertSuccess"] = ("Update Success !");
                }
                return RedirectToAction("AttendanceIntern", "Intern");
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public JsonResult CountAttendance()
        {
            if (Session["intern"] != null)
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                DateTime start = Convert.ToDateTime(iDate);
                DateTime end = Convert.ToDateTime(oDate);
                string KPK = Session["intern"].ToString();
                
                var now = DateTime.Now.ToString("M/d/yyyy");
                var data = db.Intern_AttendanceNew.Where(x => x.Date >= start && x.Date <= end && x.KPK == KPK).ToList();
                var total_late = db.VwIntern_AbsLate.Where(s => s.KPK == KPK).Count();
                var total_hour = data.Select(s => s.total_hour).Sum();
                var total_atn = data.Where(s => s.Entry_Date_Time != null).Select(s => s.Entry_Date_Time).Count();
                var total_abs1 = data.Where(s => (s.Entry_Date_Time == null && s.Entry_Out == null && s.Date_public_holidays == null && Convert.ToDateTime(s.Date) <= Convert.ToDateTime(now))).ToList();
                var total_abs2 = total_abs1.Where(s => Convert.ToDateTime(s.test).ToString("ddd") != "Sun").ToList();
                var total_abs = total_abs2.Where(s => Convert.ToDateTime(s.test).ToString("ddd") != "Sat").Count();

                var total_report = db.Intern_ListReportAttendanceForIntern(iDate, oDate, KPK).Count();
                var Obj = new
                {
                    total_hour,
                    total_atn,
                    total_abs,
                    total_late,
                    total_report
                };
                return Json(Obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("null");
            }

        }
        public JsonResult AttendanceInternIn()
        {
            if (Session["intern"] != null)
            {
                string KPK = Session["intern"].ToString();
                var generate = db.Intern_AttendanceNew.Where(s => s.KPK == KPK);

                var AttendanceIntern = generate.ToList();
                //var AttendanceIntern = from s in db.View_AttendanceIntern select s;
                return Json(AttendanceIntern, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("null");
            }

        }

        public ActionResult ProfileIntern()
        {
            string kpk = Session["username"].ToString();
            var list = db.VwIntern_MasterDataIntern.Where(x => x.KPK == kpk).ToList();

            //ViewBag.Name = list.FirstOrDefault().INTERNNAME;            

            string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
            string oDate = db.Intern_Periode.FirstOrDefault().DateTo;
            string KPK = " " + kpk + "";
            ViewBag.Timeline = db.Intern_ListAttendanceIntern(iDate, oDate, KPK).Where(x => x.Activity != null);

            //ViewBag.TotalAbsen = db.Intern_ListAttendanceInternAll(KPK).Where(x => x.format != null).Count();
            //ViewBag.TotalNoTapIn = db.Intern_ListAttendanceInternAll(KPK).Where(x => x.Entry_Date_Time == null && x.format != null).Count();
            //ViewBag.TotalNoTapOut = db.Intern_ListAttendanceInternAll(KPK).Where(x => x.Entry_Out == null && x.format != null).Count();
            //ViewBag.TotalHour = db.Intern_ListAttendanceInternAll(KPK).Sum(o => o.total_hour);

            return View();
        }
        public ActionResult PrintActivity()
        {
            if (Session["username"] != null)
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                ViewBag.From = Convert.ToDateTime(iDate).ToString("dd MMMM yyyy");
                ViewBag.To = Convert.ToDateTime(oDate).ToString("dd MMMM yyyy");

                string KPK = Session["intern"].ToString();
                DateTime start = DateTime.Parse(iDate);
                DateTime end = DateTime.Parse(oDate);

                var List = db.Intern_AttendanceNew.ToList().Where(s => s.Date >= start && s.Date <= end && s.KPK == KPK && s.ClockIn != null);
                var badge = db.Intern_AttendanceApproval.Where(s => s.KPK == KPK && s.StatusAttendance == null);
                foreach (var item in badge)
                {
                    ViewBag.StatusApproved = item.StatusApproval;
                }
                return View(List.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public ActionResult DownloadReportIntern()
        {
            if (Session["username"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public JsonResult ListDownloadReportIntern(string min_date, string max_date, string statusapproval)
        {
            if (min_date != "" && max_date != "" && statusapproval == "")
            {
                string KPK = Session["intern"].ToString();
                var downloads = db.Intern_ListReportAttendanceForIntern(min_date, max_date, KPK);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date != "" && max_date != "" && statusapproval == "1")
            {
                string KPK = Session["intern"].ToString();
                var downloads = db.Intern_ListReportAttendanceForIntern(min_date, max_date, KPK).Where(s => s.DateApprovedManager == null || s.DateApprovedManager == null);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date != "" && max_date != "" && statusapproval == "2")
            {
                string KPK = Session["intern"].ToString();
                var downloads = db.Intern_ListReportAttendanceForIntern(min_date, max_date, KPK).Where(s => s.StatusApproval == "Approved"); ;
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date == "" && max_date == "" && statusapproval == "")
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;
                string KPK = Session["intern"].ToString();
                var downloads = db.Intern_ListReportAttendanceForIntern(iDate, oDate, KPK);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date == "" && max_date == "" && statusapproval == "1")
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;
                string KPK = Session["intern"].ToString();
                var downloads = db.Intern_ListReportAttendanceForIntern(iDate, oDate, KPK).Where(s => s.DateApprovedManager == null || s.DateApprovedManager == null);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else if (min_date == "" && max_date == "" && statusapproval == "2")
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;
                string KPK = Session["intern"].ToString();
                var downloads = db.Intern_ListReportAttendanceForIntern(iDate, oDate, KPK).Where(s => s.StatusApproval == "Approved"); ;
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;
                string KPK = Session["intern"].ToString();
                var downloads = db.Intern_ListReportAttendanceForIntern(iDate, oDate, KPK);
                return Json(downloads, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ProcessFormIntern(string FromDateReport, string ToDateReport)
        {
            if (System.IO.File.Exists(Server.MapPath
                              (@"~/zipfiles/bundle.zip")))
            {
                System.IO.File.Delete(Server.MapPath
                              (@"~/zipfiles/bundle.zip"));
            }
            ZipArchive zip = ZipFile.Open(Server.MapPath
                     (@"~/zipfiles/bundle.zip"), ZipArchiveMode.Create);

            string KPK = Session["intern"].ToString();
            var downloads = db.Intern_ListReportAttendanceForIntern(FromDateReport, ToDateReport, KPK).Where(s => s.StatusApproval == "Approved").Select(s => s.FilePatch);
            foreach (string file in downloads)
            {
                zip.CreateEntryFromFile(Server.MapPath
                     (@"~/AttendanceIntern/" + file), file);
            }
            zip.Dispose();
            return File(Server.MapPath(@"~/zipfiles/bundle.zip"),
                      "application/zip", "Attendance Intern.zip");
        }

        ///////////////
        ////Mentor////
        //////////////
        public ActionResult MonitoringIntern(string min_date, string max_date, string kpkintern, string internname)
        {
            if (Session["username"] != null)
            {
                db.Database.ExecuteSqlCommand("DELETE Intern_AttendanceNew");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("INSERT INTO Intern_AttendanceNew Select * From VwIntern_AttendanceNew");
                db.SaveChanges();
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;
                ViewBag.DateFrom = iDate;
                ViewBag.DateTo = oDate;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public JsonResult ListAttendanceInternMonitor(string min_date, string max_date)
        {

            string KPK = Session["username"].ToString();
            if (min_date != "" && max_date != "")
            {
                DateTime start = Convert.ToDateTime(min_date);
                DateTime end = Convert.ToDateTime(max_date);

                var List = db.Intern_AttendanceNew.ToList().Where(s => s.MENTOR == KPK || s.MANAGER == KPK);
                var range = List.Where(s => s.Date >= start && s.Date <= end).OrderBy(x => x.KPK);
                return Json(range, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var List = db.Intern_AttendanceNew.ToList().Where(s => s.MENTOR == KPK || s.MANAGER == KPK);
                string iDate = db.Intern_Periode.FirstOrDefault().DateFrom;
                string oDate = db.Intern_Periode.FirstOrDefault().DateTo;

                DateTime start = DateTime.Parse(iDate);
                DateTime end = DateTime.Parse(oDate);

                var List = db.Intern_AttendanceNew.ToList().Where(s => s.MENTOR == KPK || s.MANAGER == KPK);
                var range = List.Where(s => s.Date >= start && s.Date <= end).OrderBy(x => x.KPK);
                return Json(range, JsonRequestBehavior.AllowGet);

            }
            
        }        

        //////////////////
        // Email Section//
        //////////////////

        ///////////////////
        //Email Automatic//
        ///////////////////

        //Approval
        public ActionResult SendEmail(HttpPostedFileBase uploadedfile)
        {
            try
            {
                string month = "Periode";
                string ApprovalMentor = Session["emailmentor"].ToString();
                string ApprovalManager = Session["emailmanager"].ToString();
                string NameMentor = Session["namementor"].ToString();
                string NameManager = Session["namemanager"].ToString();
                string KPK1 = Session["intern"].ToString();
                string Name = Session["user"].ToString();

                var periode = db.Intern_Periode.ToList();
                var fromdate = periode.FirstOrDefault().DateFrom;
                var todate = periode.FirstOrDefault().DateTo;

                string dateSend = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");

                var uploadTimes = DateTime.Now.ToString("MMddyyyyhhmmss");
                string datePeriode = "" + todate + "";
                string FileName = uploadedfile.FileName;
                string targetpath = Server.MapPath(@"~/AttendanceIntern/");
                string fullPath = Request.MapPath(@"~/AttendanceIntern/" + uploadTimes + "-" + FileName);
                if (System.IO.File.Exists(fullPath))
                {
                    uploadedfile.SaveAs(targetpath + uploadTimes + "-" + FileName);
                }
                uploadedfile.SaveAs(targetpath + uploadTimes + "-" + FileName);
                string path = targetpath + uploadTimes + "-" + FileName;
                string filesname = uploadTimes + "-" + FileName;
                var filename = @"" + path + "";


                db.Database.ExecuteSqlCommand("INSERT INTO Intern_AttendanceApproval(KPK,ApprovalMentor,ApprovalManager,DateSend,StatusApproval,FilePatch,Periode) VALUES ('" + KPK1 + "','Waiting For Approval Mentor','Waiting For Approval Manager','" + dateSend + "','Waiting For Approval Mentor and Approval Manager','" + filesname + "','" + datePeriode + "')");
                db.SaveChanges();

                BackgroundJob.Enqueue(() => SendEmailBackgroundJob(KPK1, Name, month, ApprovalMentor, ApprovalManager, NameMentor, NameManager, path, dateSend));
            }
            catch (Exception error)
            {
                TempData["AllertError"] = ("Please cek your file and Upload different file name!", error);
                return RedirectToAction("AttendanceIntern");
            }
            TempData["AllertSuccess"] = ("Waiting for mentor and manager approval!");
            return RedirectToAction("AttendanceIntern");
        }
        public ActionResult sendEmailToManager(string input_kpk, string input_name, string input_month, string input_approval_manager, string input_path, string input_datesend, string namementor, string namemanager)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var input_kpk_naturalize = input_kpk.Replace(' ', '+');
            var input_name_naturalize = input_name.Replace(' ', '+');
            var input_month_naturalize = input_month.Replace(' ', '+');
            var input_approval_manager_naturalize = input_approval_manager.Replace(' ', '+');
            var input_path_naturalize = input_path.Replace(' ', '+');
            var input_datesend_naturalize = input_datesend.Replace(' ', '+');
            var input_namementor_naturalize = namementor.Replace(' ', '+');
            var input_namemanager_naturalize = namemanager.Replace(' ', '+');

            var getKPK = DecryptString(key, input_kpk_naturalize);
            var getName = DecryptString(key, input_name_naturalize);
            var getMonth = DecryptString(key, input_month_naturalize);
            var getManagerEmail = DecryptString(key, input_approval_manager_naturalize);
            var getpath = DecryptString(key, input_path_naturalize);
            var getdatesend = DecryptString(key, input_datesend_naturalize);
            var getnamementor = DecryptString(key, input_namementor_naturalize);
            var getnamemanager = DecryptString(key, input_namemanager_naturalize);

            var approved = db.Intern_AttendanceApproval.Where(s => s.KPK == getKPK && s.DateSend == getdatesend).ToList();
            foreach (var app in approved)
            {
                ViewBag.App = app.ApprovalMentor;
            }
            if (ViewBag.App == "Mentor Approved")
            {
                return RedirectToAction("DoneApprov");
            }
            else
            {
                try
                {   // credentials 
                    string DateApproved = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                    // update form status
                    db.Database.ExecuteSqlCommand("UPDATE Intern_AttendanceApproval Set ApprovalMentor = 'Mentor Approved', DateApprovedMentor = '" + DateApproved + "', StatusApproval ='Waiting For Approval Manager' where KPK = '" + getKPK + "' and DateSend = '" + getdatesend + "'");
                    db.SaveChanges();
                    BackgroundJob.Enqueue(() => SendEmailBackgroundJob2(getKPK, getName, getMonth, getManagerEmail, getpath, getdatesend, getnamementor, getnamemanager));
                    BackgroundJob.Enqueue(() => SendEmailBackgroundJob4(getKPK, getName, getMonth, getManagerEmail, getpath, getdatesend, getnamementor, getnamemanager));
                }
                catch (Exception error)
                {
                    ViewBag.Error = "Sorry, it's looks like there is an error";
                    TempData["error-email"] = error;
                    return RedirectToAction("SuccessApprov");
                }

                return RedirectToAction("SuccessApprov");
            }
        }
        public ActionResult ApprovalManager(string input_kpk, string input_name, string input_month, string input_approval_manager, string input_path, string input_datesend, string namementor, string namemanager)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var input_kpk_naturalize = input_kpk.Replace(' ', '+');
            var input_name_naturalize = input_name.Replace(' ', '+');
            var input_month_naturalize = input_month.Replace(' ', '+');
            var input_approval_manager_naturalize = input_approval_manager.Replace(' ', '+');
            var input_path_naturalize = input_path.Replace(' ', '+');
            var input_datesend_naturalize = input_datesend.Replace(' ', '+');
            var input_namementor_naturalize = namementor.Replace(' ', '+');
            var input_namemanager_naturalize = namemanager.Replace(' ', '+');

            var getKPK = DecryptString(key, input_kpk_naturalize);
            var getName = DecryptString(key, input_name_naturalize);
            var getMonth = DecryptString(key, input_month_naturalize);
            var getManagerEmail = DecryptString(key, input_approval_manager_naturalize);
            var getpath = DecryptString(key, input_path_naturalize);
            var getdatesend = DecryptString(key, input_datesend_naturalize);
            var getnamementor = DecryptString(key, input_namementor_naturalize);
            var getnamemanager = DecryptString(key, input_namemanager_naturalize);

            var intern = db.Intern_ListUsers.Where(s => s.user_id.Equals(getKPK)).ToList();
            var getInternEmail = intern.FirstOrDefault().OFFICEMAIL;
            try
            {   // credentials 
                string DateApproved = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                // update form status
                db.Database.ExecuteSqlCommand("UPDATE Intern_AttendanceApproval Set ApprovalManager = 'Manager Approved', DateApprovedManager = '" + DateApproved + "', StatusApproval ='Approved' where KPK = '" + getKPK + "' and DateSend = '" + getdatesend + "'");
                db.SaveChanges();
                BackgroundJob.Enqueue(() => SendEmailBackgroundJob3(getKPK, getName, getMonth, getManagerEmail, getpath, getdatesend, getnamementor, getnamemanager));
            }
            catch (Exception error)
            {
                ViewBag.Error = "Sorry, it's looks like there is an error";
                TempData["error-email"] = error;
                return RedirectToAction("SuccessApprov");
            }

            return RedirectToAction("SuccessApprov");
        }
       
         
        //Rejected
        public ActionResult RejectedMentor(string input_kpk, string input_name, string input_month, string input_approval_manager, string input_path, string input_datesend)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var input_kpk_naturalize = input_kpk.Replace(' ', '+');
            var input_name_naturalize = input_name.Replace(' ', '+');
            var input_month_naturalize = input_month.Replace(' ', '+');
            var input_approval_manager_naturalize = input_approval_manager.Replace(' ', '+');
            var input_path_naturalize = input_path.Replace(' ', '+');
            var input_datesend_naturalize = input_datesend.Replace(' ', '+');

            var getKPK = DecryptString(key, input_kpk_naturalize);
            var getName = DecryptString(key, input_name_naturalize);
            var getMonth = DecryptString(key, input_month_naturalize);
            var getManagerEmail = DecryptString(key, input_approval_manager_naturalize);
            var getpath = DecryptString(key, input_path_naturalize);
            var getdatesend = DecryptString(key, input_datesend_naturalize);

            try
            {   // credentials 
                string DateApproved = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                // update form status
                db.Database.ExecuteSqlCommand("UPDATE Intern_AttendanceApproval Set ApprovalMentor = 'Mentor Rejected', DateApprovedMentor = '" + DateApproved + "', StatusApproval ='Rejected By Mentor' where KPK = '" + getKPK + "' and DateSend = '" + getdatesend + "'");
                db.SaveChanges();
            }
            catch (Exception error)
            {
                ViewBag.Error = "Sorry, it's looks like there is an error";
                TempData["error-email"] = error;
                return RedirectToAction("SuccessApprov");
            }
            return RedirectToAction("SuccessApprov");
        }
        public ActionResult RejectedManager(string input_kpk, string input_name, string input_month, string input_approval_manager, string input_path, string input_datesend)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var input_kpk_naturalize = input_kpk.Replace(' ', '+');
            var input_name_naturalize = input_name.Replace(' ', '+');
            var input_month_naturalize = input_month.Replace(' ', '+');
            var input_approval_manager_naturalize = input_approval_manager.Replace(' ', '+');
            var input_path_naturalize = input_path.Replace(' ', '+');
            var input_datesend_naturalize = input_datesend.Replace(' ', '+');

            var getKPK = DecryptString(key, input_kpk_naturalize);
            var getName = DecryptString(key, input_name_naturalize);
            var getMonth = DecryptString(key, input_month_naturalize);
            var getManagerEmail = DecryptString(key, input_approval_manager_naturalize);
            var getpath = DecryptString(key, input_path_naturalize);
            var getdatesend = DecryptString(key, input_datesend_naturalize);

            try
            {   // credentials 
                string DateApproved = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                // update form status
                db.Database.ExecuteSqlCommand("UPDATE Intern_AttendanceApproval Set ApprovalManager = 'Manager Rejected', DateApprovedManager = '" + DateApproved + "', StatusApproval ='Rejected By Manager' where KPK = '" + getKPK + "' and DateSend = '" + getdatesend + "'");
                db.SaveChanges();
            }
            catch (Exception error)
            {
                ViewBag.Error = "Sorry, it's looks like there is an error";
                TempData["error-email"] = error;
                return RedirectToAction("SuccessApprov");
            }
            return RedirectToAction("SuccessApprov");
        }

        //Landing Page
        public ActionResult SuccessApprov()
        {
            return View();
        }
        public ActionResult DoneApprov()
        {
            return View();
        }

        // new using js
        public JsonResult ShowAttendanceIntern()
        {
            try
            {
                string iDate = "2022/02/23";
                string oDate = "2022/03/22";

                string KPK = Session["intern"].ToString();
                var List = db.Intern_ListAttendanceIntern(iDate, oDate, KPK);


                return Json(List, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e); return Json("Error");
            }
        }

        /////////////////
        //Email Manual//
        ///////////////

        //Approval Mentor
        public JsonResult ApprovalMentorByAdmin(string KPKIntern, string filePatch, string DateSend)
        {
            try
            {
                string month = "Periode";
                string DateApproved = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                var intern = db.Intern_ListUsers.Where(s => s.user_id.Equals(KPKIntern)).ToList();

                string NameMentor = "Admin HR Portal";
                string ApprovalMentor = intern.FirstOrDefault().EMAILMENTOR;

                string NameManager = intern.FirstOrDefault().NAMEMANAGER;
                string ApprovalManager = intern.FirstOrDefault().EMAILMANAGER;
                string KPK1 = intern.FirstOrDefault().user_id;
                string Name = intern.FirstOrDefault().INTERNNAME;

                string dateSend = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                string datePeriode = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                string targetpath = Server.MapPath("~/AttendanceIntern/");
                string fullPath = Request.MapPath("~/AttendanceIntern/" + filePatch);

                string path = targetpath + filePatch;
                var filename = @"" + path + "";

                db.Database.ExecuteSqlCommand("UPDATE Intern_AttendanceApproval Set ApprovalMentor = 'Mentor Approved By Admin', DateApprovedMentor = '" + DateApproved + "', StatusApproval ='Waiting For Approval Manager' where KPK = '" + KPKIntern + "' and DateSend = '" + DateSend + "'");
                db.SaveChanges();
                BackgroundJob.Enqueue(() => SendEmailBackgroundJob2(KPK1, Name, month, ApprovalManager, path, dateSend, NameMentor, NameManager));
            }
            catch (Exception error)
            {
                return Json(error);
            }
            return Json("Item updated successfully");
        }
        //Approval Manager
        public JsonResult ApprovalManagerByAdmin(string KPKIntern, string filePatch, string DateSend)
        {
            try
            {
                //Get Data
                string month = "Periode";
                string DateApproved = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                var intern = db.Intern_ListUsers.Where(s => s.user_id.Equals(KPKIntern)).ToList();
                string dateSend = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                string datePeriode = DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt");
                string targetpath = Server.MapPath("~/AttendanceIntern/");
                string fullPath = Request.MapPath("~/AttendanceIntern/" + filePatch);

                string path = targetpath + filePatch;
                var filename = @"" + path + "";

                //Get Data Mentor
                string NameMentor = intern.FirstOrDefault().NAMEMENTOR;
                string ApprovalMentor = intern.FirstOrDefault().EMAILMENTOR;

                //Get Data Manager
                string NameManager = "Admin HR Portal";
                string ApprovalManager = intern.FirstOrDefault().EMAILMANAGER;

                //Get Data Intern
                string KPK1 = intern.FirstOrDefault().user_id;
                string Name = intern.FirstOrDefault().INTERNNAME;
                string OfficeMail = intern.FirstOrDefault().INTERNNAME;

                db.Database.ExecuteSqlCommand("UPDATE Intern_AttendanceApproval Set ApprovalManager = 'Manager Approved By Admin', DateApprovedManager = '" + DateApproved + "', StatusApproval ='Approved' where KPK = '" + KPKIntern + "' and DateSend = '" + DateSend + "'");
                db.SaveChanges();

                BackgroundJob.Enqueue(() => SendEmailBackgroundJob3(KPK1, Name, month, ApprovalManager, path, dateSend, NameMentor, NameManager));
            }
            catch (Exception error)
            {
                return Json(error);
            }
            return Json("Attendance report successfully send");
        }
                
        //////////////////
        //Documentation//
        ////////////////
        public ActionResult Documentation()
        {
            if (Session["username"] != null)
            {
                string KPK = Session["intern"].ToString();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }

        //////////////
        //Dashboard//
        ////////////
        public ActionResult Dashboardavgattendance()
        {
            string KPK = Session["intern"].ToString();
            //late
            var data = db.VwIntern_AbsLate.Where(x => x.KPK == KPK).ToList();

            int jan = data.Where(x => x.Date.Substring(0, 2) == "1/").Count();
            int feb = data.Where(x => x.Date.Substring(0, 1) == "2").Count();
            int mar = data.Where(x => x.Date.Substring(0, 1) == "3").Count();
            int apr = data.Where(x => x.Date.Substring(0, 1) == "4").Count();
            int mei = data.Where(x => x.Date.Substring(0, 1) == "5").Count();
            int jun = data.Where(x => x.Date.Substring(0, 1) == "6").Count();
            int jul = data.Where(x => x.Date.Substring(0, 1) == "7").Count();
            int aug = data.Where(x => x.Date.Substring(0, 1) == "8").Count();
            int sep = data.Where(x => x.Date.Substring(0, 1) == "9").Count();
            int oct = data.Where(x => x.Date.Substring(0, 2) == "10").Count();
            int nov = data.Where(x => x.Date.Substring(0, 2) == "11").Count();
            int dec = data.Where(x => x.Date.Substring(0, 2) == "12").Count();

            //hday
            var data1 = db.Intern_AttendanceNew.Where(x => x.total_hour < 8 && x.KPK == KPK).ToList();
            int jan1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 2) == "1/").Count();
            int feb1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "2").Count();
            int mar1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "3").Count();
            int apr1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "4").Count();
            int mei1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "5").Count();
            int jun1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "6").Count();
            int jul1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "7").Count();
            int aug1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "8").Count();
            int sep1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "9").Count();
            int oct1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 2) == "10").Count();
            int nov1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 2) == "11").Count();
            int dec1 = data1.Where(x => Convert.ToString(x.Date).Substring(0, 2) == "12").Count();

            countRLate Obj = new countRLate();
            Obj.jan = jan;
            Obj.feb = feb;
            Obj.mar = mar;
            Obj.apr = apr;
            Obj.mei = mei;
            Obj.jun = jun;
            Obj.jul = jul;
            Obj.aug = aug;
            Obj.sep = sep;
            Obj.oct = oct;
            Obj.nov = nov;
            Obj.dec = dec;

            Obj.jan1 = jan1;
            Obj.feb1 = feb1;
            Obj.mar1 = mar1;
            Obj.apr1 = apr1;
            Obj.mei1 = mei1;
            Obj.jun1 = jun1;
            Obj.jul1 = jul1;
            Obj.aug1 = aug1;
            Obj.sep1 = sep1;
            Obj.oct1 = oct1;
            Obj.nov1 = nov1;
            Obj.dec1 = dec1;

            return Json(Obj, JsonRequestBehavior.AllowGet);
        }

        public class countRLate
        {
            public int jan { get; set; }
            public int feb { get; set; }
            public int mar { get; set; }
            public int apr { get; set; }
            public int mei { get; set; }
            public int jun { get; set; }
            public int jul { get; set; }
            public int aug { get; set; }
            public int sep { get; set; }
            public int oct { get; set; }
            public int nov { get; set; }
            public int dec { get; set; }
            //eoc
            public int jan1 { get; set; }
            public int feb1 { get; set; }
            public int mar1 { get; set; }
            public int apr1 { get; set; }
            public int mei1 { get; set; }
            public int jun1 { get; set; }
            public int jul1 { get; set; }
            public int aug1 { get; set; }
            public int sep1 { get; set; }
            public int oct1 { get; set; }
            public int nov1 { get; set; }
            public int dec1 { get; set; }
        }

        public ActionResult DashboarAtnAbs()
        {
            string KPK = Session["intern"].ToString();
            //atn
            var data = db.Intern_AttendanceNew.Where(x => x.KPK == KPK).ToList();

            int jan = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "1").Count();
            int feb = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "2").Count();
            int mar = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "3").Count();
            int apr = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "4").Count();
            int mei = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "5").Count();
            int jun = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "6").Count();
            int jul = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "7").Count();
            int aug = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "8").Count();
            int sep = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 1) == "9").Count();
            int oct = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 2) == "10").Count();
            int nov = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 2) == "11").Count();
            int dec = data.Where(x => x.ClockIn != null && x.ClockOut != null && Convert.ToString(x.Date).Substring(0, 2) == "12").Count();

            //abs
            var now = DateTime.Now.ToString("M/d/yyyy");

            var data1 = db.Intern_AttendanceNew.Where(x => x.KPK == KPK).ToList();
            var total_abs1 = data1.Where(s => (s.Entry_Date_Time == null && s.Entry_Out == null && s.Date_public_holidays == null && Convert.ToDateTime(s.Date) <= Convert.ToDateTime(now))).ToList();
            var total_abs2 = total_abs1.Where(s => Convert.ToDateTime(s.test).ToString("ddd") != "Sun").ToList();
            var TAbs = total_abs2.Where(s => Convert.ToDateTime(s.test).ToString("ddd") != "Sat").ToList();

            int jan1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "1").Count();
            int feb1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "2").Count();
            int mar1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "3").Count();
            int apr1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "4").Count();
            int mei1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "5").Count();
            int jun1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "6").Count();
            int jul1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "7").Count();
            int aug1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "8").Count();
            int sep1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 1) == "9").Count();
            int oct1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 2) == "10").Count();
            int nov1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 2) == "11").Count();
            int dec1 = TAbs.Where(x => Convert.ToString(x.Date).Substring(0, 2) == "12").Count();

            countAtnAbs Obj = new countAtnAbs();
            Obj.jan = jan;
            Obj.feb = feb;
            Obj.mar = mar;
            Obj.apr = apr;
            Obj.mei = mei;
            Obj.jun = jun;
            Obj.jul = jul;
            Obj.aug = aug;
            Obj.sep = sep;
            Obj.oct = oct;
            Obj.nov = nov;
            Obj.dec = dec;

            Obj.jan1 = jan1;
            Obj.feb1 = feb1;
            Obj.mar1 = mar1;
            Obj.apr1 = apr1;
            Obj.mei1 = mei1;
            Obj.jun1 = jun1;
            Obj.jul1 = jul1;
            Obj.aug1 = aug1;
            Obj.sep1 = sep1;
            Obj.oct1 = oct1;
            Obj.nov1 = nov1;
            Obj.dec1 = dec1;

            return Json(Obj, JsonRequestBehavior.AllowGet);
        }

        public class countAtnAbs
        {
            public int jan { get; set; }
            public int feb { get; set; }
            public int mar { get; set; }
            public int apr { get; set; }
            public int mei { get; set; }
            public int jun { get; set; }
            public int jul { get; set; }
            public int aug { get; set; }
            public int sep { get; set; }
            public int oct { get; set; }
            public int nov { get; set; }
            public int dec { get; set; }
            //eoc
            public int jan1 { get; set; }
            public int feb1 { get; set; }
            public int mar1 { get; set; }
            public int apr1 { get; set; }
            public int mei1 { get; set; }
            public int jun1 { get; set; }
            public int jul1 { get; set; }
            public int aug1 { get; set; }
            public int sep1 { get; set; }
            public int oct1 { get; set; }
            public int nov1 { get; set; }
            public int dec1 { get; set; }
        }

        //backgroud job to mentor
        public async Task<ActionResult> SendEmailBackgroundJob(string KPK1, string Name, string month, string ApprovalMentor, string ApprovalManager, string NameMentor, string NameManager, string path, string dateSend)
        {
            //Email
            var templateEmail = "~/Views/SendEmailToMentor.html";
            var body = EMailTemplate("WelcomeEmail", templateEmail);
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var data = from s in db.SelfService_EmailService select s;
            var email_from = data.FirstOrDefault().Email_from;
            var email = data.FirstOrDefault().Email;
            var password_enc = data.FirstOrDefault().Password;

            var password_naturalize = password_enc.Replace(' ', '+');
            var password = DecryptString(key, password_naturalize);
            // encrypt values to be send to manager email
            body = body.Replace("{KPKEnc}", EncryptString(key, KPK1));
            body = body.Replace("{NameEnc}", EncryptString(key, Name));
            body = body.Replace("{MonthEnc}", EncryptString(key, month));
            body = body.Replace("{ApprovalMentorEnc}", EncryptString(key, ApprovalMentor));
            body = body.Replace("{ApprovalManagerEnc}", EncryptString(key, ApprovalManager));
            body = body.Replace("{pathEnc}", EncryptString(key, path));
            body = body.Replace("{dateSendEnc}", EncryptString(key, dateSend));
            body = body.Replace("{NameMentorEnc}", EncryptString(key, NameMentor));
            body = body.Replace("{NameManagerEnc}", EncryptString(key, NameManager));

            // replace the code with real value based on user input
            body = body.Replace("{KPK}", KPK1);
            body = body.Replace("{Name}", Name);
            body = body.Replace("{Month}", month);
            body = body.Replace("{NameMentor}", NameMentor);

            var email_to = ApprovalMentor;
            await SendEmailAsync("HR PORTAL - Attendance Report '" + Name + "'", body, path, email_to, email_from, email, password);
            //End Email
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SendEmailBackgroundJob2(string input_kpk, string input_name, string input_month, string input_approval_manager, string input_path, string input_datesend, string namementor, string namemanager)
        {
            //Email
            var templateEmail = "~/Views/SendEmailToManager.html";
            var body = EMailTemplate("WelcomeEmail", templateEmail);
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var data = from s in db.SelfService_EmailService select s;
            var email_from = data.FirstOrDefault().Email_from;
            var email = data.FirstOrDefault().Email;
            var password_enc = data.FirstOrDefault().Password;

            var password_naturalize = password_enc.Replace(' ', '+');
            var password = DecryptString(key, password_naturalize);

            // decrypt encrypted parameters
            var input_kpk_naturalize = input_kpk.Replace('+', ' ');
            var input_name_naturalize = input_name.Replace('+', ' ');
            var input_month_naturalize = input_month.Replace('+', ' ');
            var input_approval_manager_naturalize = input_approval_manager.Replace(' ', '+');
            var input_path_naturalize = input_path.Replace('+', ' ');
            var input_datesend_naturalize = input_datesend.Replace('+', ' ');
            var input_namementor_naturalize = namementor.Replace('+', ' ');
            var input_namemanager_naturalize = namemanager.Replace('+', ' ');

            // encrypt values to be send to manager email
            body = body.Replace("{KPKEnc}", EncryptString(key, input_kpk));
            body = body.Replace("{NameEnc}", EncryptString(key, input_name));
            body = body.Replace("{MonthEnc}", EncryptString(key, input_month));
            body = body.Replace("{ApprovalManagerEnc}", EncryptString(key, input_approval_manager));
            body = body.Replace("{pathEnc}", EncryptString(key, input_path));
            body = body.Replace("{dateSendEnc}", EncryptString(key, input_datesend));

            body = body.Replace("{NameMentorEnc}", EncryptString(key, input_namementor_naturalize));
            body = body.Replace("{NameManagerEnc}", EncryptString(key, input_namemanager_naturalize));

            // replace the code with real value based on user input
            body = body.Replace("{KPK}", input_kpk_naturalize);
            body = body.Replace("{Name}", input_name_naturalize);
            body = body.Replace("{Month}", input_month_naturalize);
            body = body.Replace("{NameMentor}", input_namementor_naturalize);
            body = body.Replace("{NameManager}", input_namemanager_naturalize);

            var email_to = input_approval_manager_naturalize;
            await SendEmailAsync("HR PORTAL - Attendance Report '" + input_name_naturalize + "'", body, input_path_naturalize, email_to, email_from, email, password);
            //End Email
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SendEmailBackgroundJob3(string input_kpk, string input_name, string input_month, string input_approval_manager, string input_path, string input_datesend, string namementor, string namemanager)
        {
            //Email
            var templateEmail = "~/Views/SendEmailToUser.html";
            var body = EMailTemplate("WelcomeEmail", templateEmail);
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var data = from s in db.SelfService_EmailService select s;
            var email_from = data.FirstOrDefault().Email_from;
            var email = data.FirstOrDefault().Email;
            var password_enc = data.FirstOrDefault().Password;

            var password_naturalize = password_enc.Replace(' ', '+');
            var password = DecryptString(key, password_naturalize);

            // decrypt encrypted parameters
            var input_kpk_naturalize = input_kpk.Replace('+', ' ');
            var input_name_naturalize = input_name.Replace('+', ' ');
            var input_month_naturalize = input_month.Replace('+', ' ');
            var input_approval_manager_naturalize = input_approval_manager.Replace(' ', '+');
            var input_path_naturalize = input_path.Replace('+', ' ');
            var input_datesend_naturalize = input_datesend.Replace('+', ' ');
            var input_namementor_naturalize = namementor.Replace('+', ' ');
            var input_namemanager_naturalize = namemanager.Replace('+', ' ');

            // encrypt values to be send to manager email
            body = body.Replace("{KPKEnc}", EncryptString(key, input_kpk));
            body = body.Replace("{NameEnc}", EncryptString(key, input_name));
            body = body.Replace("{MonthEnc}", EncryptString(key, input_month));
            body = body.Replace("{ApprovalManagerEnc}", EncryptString(key, input_approval_manager));
            body = body.Replace("{pathEnc}", EncryptString(key, input_path));
            body = body.Replace("{dateSendEnc}", EncryptString(key, input_datesend));

            body = body.Replace("{NameMentorEnc}", EncryptString(key, input_namementor_naturalize));
            body = body.Replace("{NameManagerEnc}", EncryptString(key, input_namemanager_naturalize));

            // replace the code with real value based on user input
            body = body.Replace("{KPK}", input_kpk_naturalize);
            body = body.Replace("{Name}", input_name_naturalize);
            body = body.Replace("{Month}", input_month_naturalize);
            body = body.Replace("{NameMentor}", input_namementor_naturalize);
            body = body.Replace("{NameManager}", input_namemanager_naturalize);

            var email_to = input_approval_manager_naturalize;
            await SendEmailAsync("HR PORTAL - Attendance Report '" + input_name_naturalize + "'", body, input_path_naturalize, email_to, email_from, email, password);
            //End Email
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SendEmailBackgroundJob4(string input_kpk, string input_name, string input_month, string input_approval_manager, string input_path, string input_datesend, string namementor, string namemanager)
        {
            //Email
            var templateEmail = "~/Views/TractMyAttendance.html";
            var body = EMailTemplate("WelcomeEmail", templateEmail);
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var data = from s in db.SelfService_EmailService select s;
            var email_from = data.FirstOrDefault().Email_from;
            var email = data.FirstOrDefault().Email;
            var password_enc = data.FirstOrDefault().Password;

            var password_naturalize = password_enc.Replace(' ', '+');
            var password = DecryptString(key, password_naturalize);

            // decrypt encrypted parameters
            var input_kpk_naturalize = input_kpk.Replace('+', ' ');
            var input_name_naturalize = input_name.Replace('+', ' ');
            var input_month_naturalize = input_month.Replace('+', ' ');
            var input_approval_manager_naturalize = input_approval_manager.Replace(' ', '+');
            var input_path_naturalize = input_path.Replace('+', ' ');
            var input_datesend_naturalize = input_datesend.Replace('+', ' ');
            var input_namementor_naturalize = namementor.Replace('+', ' ');
            var input_namemanager_naturalize = namemanager.Replace('+', ' ');

            // encrypt values to be send to manager email
            body = body.Replace("{KPKEnc}", EncryptString(key, input_kpk));
            body = body.Replace("{NameEnc}", EncryptString(key, input_name));
            body = body.Replace("{MonthEnc}", EncryptString(key, input_month));
            body = body.Replace("{ApprovalManagerEnc}", EncryptString(key, input_approval_manager));
            body = body.Replace("{pathEnc}", EncryptString(key, input_path));
            body = body.Replace("{dateSendEnc}", EncryptString(key, input_datesend));

            body = body.Replace("{NameMentorEnc}", EncryptString(key, input_namementor_naturalize));
            body = body.Replace("{NameManagerEnc}", EncryptString(key, input_namemanager_naturalize));

            // replace the code with real value based on user input
            body = body.Replace("{KPK}", input_kpk_naturalize);
            body = body.Replace("{Name}", input_name_naturalize);
            body = body.Replace("{Month}", input_month_naturalize);
            body = body.Replace("{NameMentor}", input_namementor_naturalize);
            body = body.Replace("{NameManager}", input_namemanager_naturalize);

            var email_to = input_approval_manager_naturalize;
            await SendEmailAsync("HR PORTAL - Attendance Report '" + input_name_naturalize + "'", body, input_path_naturalize, email_to, email_from, email, password);
            //End Email
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        public string EMailTemplate(string template, string templateEmail)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(templateEmail));
            return body.ToString();
        }
        public async static Task SendEmailAsync(string subjek, string message, string fileName, string email_to, string email_from, string email, string password)
        {
            SmtpClient smtpClient = new SmtpClient(); //587
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(email_from, password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();
            var to = email_to;
            mail.Attachments.Add(new Attachment(HostingEnvironment.MapPath("~/AttendanceIntern/" + Path.GetFileName(fileName))));
            mail.To.Add(new MailAddress(to));
            mail.From = new MailAddress(email_from);
            mail.Body = message;
            mail.Subject = subjek;
            mail.IsBodyHtml = true;
            await smtpClient.SendMailAsync(mail);

        }
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

    }
}