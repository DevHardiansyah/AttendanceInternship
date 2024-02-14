using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRINTERNSHIP.Models;
// ProcessNewHire
namespace HRINTERNSHIP.Controllers
{
    public class AdminController : Controller
    {
        private DBModel db = new DBModel();
        public ActionResult ChangePassoword(string kpk)
        {
            if (Session["username"] != null && Session["intern"] == null)
            {
                var data = from s in db.ListUsers select s;
                var whereData = Session["username"].ToString();
                var query = data.Where(s => s.EMEMP.Equals(whereData)).Take(1);
                return View(query);
            }
            else if (Session["username"] != null && Session["intern"] != null)
            {
                var data = from s in db.Intern_ListUsers select s;
                var whereData = Session["intern"].ToString();
                ViewBag.query = data.Where(s => s.user_id.Equals(whereData)).Take(1);
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }


        public JsonResult UpdatePassword(string kpk, string newPassword)
        {
            db.Database.ExecuteSqlCommand("Update Users set password='" + newPassword + "' where username ='" + kpk + "'");
            db.SaveChanges();
            return Json("Password berhasil diubah");
        }


        // function for upload new hire (Excel)
        public ActionResult UploadNewHire(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile.ContentType == "application/vnd.ms-excel" || postedFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = postedFile.FileName;
                string targetpath = Server.MapPath("~/NewHire/");
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
                                string strSql = "TRUNCATE TABLE Temp_new_Hire";
                                SqlCommand cmd = new SqlCommand(strSql, con);
                                cmd.ExecuteNonQuery();
                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_new_Hire";
                                    sqlBulkCopy.ColumnMappings.Add("KPK", "EMEMP");
                                    sqlBulkCopy.ColumnMappings.Add("UKURAN", "SIZE");
                                    sqlBulkCopy.ColumnMappings.Add("WARNA", "KRAH");
                                    sqlBulkCopy.ColumnMappings.Add("JUMLAH", "Jumlah");
                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                command.CommandTimeout = 300;
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.CommandText = "UpdateStockUniformInventory";
                                command.ExecuteScalar();



                            }
                            catch (Exception error)
                            {
                                TempData["FAILED-UPLOAD"] = error;

                                return RedirectToAction("ManageKios");     // Handle exception properly
                            }
                            finally
                            {

                                con.Close();
                            }
                        }


                    }
                }
            }

            TempData["SUCCESS-UPLOAD"] = "upload-data-success";
            return RedirectToAction("ManageKios");
        }
        //// first load for menus Manage kios (New Hire)
        //public JsonResult NewHire()
        //{
        //    var newHire = from s in db.Temp_new_Hire select s;
        //    return Json(newHire, JsonRequestBehavior.AllowGet);
        //}
        //// first load for menus Manage kios (New Hire)
        //public ActionResult ProcessNewHire(string kpk, string size, string krah, string types, string date)
        //{
        //    try
        //    {
        //        // insert
        //        db.Database.ExecuteSqlCommand("INSERT INTO TRANSACTION_KIOS_NEWHIRE(KPK,KRAH,SIZE,TYPE,TRANSACTION_DATE) values('" + kpk + "','" + krah + "','" + size + "','" + types + "','" + date + "')");
        //        db.SaveChanges();
        //        // update stock
        //        db.Database.ExecuteSqlCommand("Update UniformInventory set stock= stock - 2 where color='" + krah + "' and size='" + size + "' and type='" + types + "'  ");
        //        db.SaveChanges();
        //        // delete 
        //        db.Database.ExecuteSqlCommand("Delete Temp_new_Hire WHERE EMEMP='" + kpk + "'");
        //        db.SaveChanges();
        //        return Json("");
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(e.Message);
        //    }

        //}
        //// first load for menus Manage kios (New Hire)
        //public ActionResult DeleteDumyNewHire()
        //{
        //    db.Database.ExecuteSqlCommand("Delete Temp_new_Hire");
        //    return Json("success");
        //}

        //public ActionResult ReportNewHire()
        //{   // for the next maybe we add filter data
        //    var data = from s in db.ReportUniformNewHires select s;
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //// GET: Admin
        //public ActionResult Index(string location)
        //{
        //    //Session['Whatever'] ==  location;// eats or west 

        //    // modal
        //    // Tables <tr></td>
        //    ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
        //    ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
        //    ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
        //    return View();
        //}
        //// first load for menus Manage kios (Distribution)
        //public ActionResult ManageKios(string Month, string Year)
        //{
        //    // get inventory
        //    ViewBag.getOrder = from s in db.UniformInventoryTransactions select s;
        //    if (Session["username"] != null)
        //    {
        //        // get year
        //        var CurrentYear = DateTime.Now.Year.ToString();
        //        // check filter
        //        if (String.IsNullOrEmpty(Month))
        //        {
        //            var Master_data = from m in db.kios orderby m.Month descending select m;
        //            var data = Master_data.Where(s => s.Expr1 == null && s.TRANSACTION_DATE != "44472").ToList();
        //            return View(data);
        //        }
        //        else
        //        {
        //            var Master_data = from m in db.kios select m;
        //            var Result = Master_data.Where(m => m.Expr1 == null && m.Month == Month).ToList();
        //            if (Result.Any())
        //            {
        //                return View(Result);
        //            }
        //            else
        //            {
        //                ViewBag.Message = "Data not found";
        //                return View();
        //            }
        //        }

        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Employee");
        //    }
        //}
        //// first load for menus Manage kios (Report) 
        //public JsonResult ReportUniform()
        //{
        //    var Master_data = from m in db.kios select m;
        //    var data = Master_data.Where(s => s.Expr1 != null).ToList();
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //// first load for menus Manage kios (ERT)
        //public JsonResult ERT()
        //{
        //    var Master_data = from m in db.ERTs select m;

        //    return Json(Master_data, JsonRequestBehavior.AllowGet);
        //}
        //// first load for menus Manage kios (Inventory)
        //public JsonResult Stock(string type, string color, string size)
        //{
        //    var data = from s in db.UniformInventories orderby s.last_update descending select s;

        //    if (type != null && size != null && color == null)
        //    {
        //        var datas = data.Where(s => s.type.Equals(type) && s.size.Equals(size)).ToList();
        //        return Json(datas, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (type != null && color != null && size != null)
        //    {
        //        var datas = data.Where(s => s.type.Equals(type) && s.color.Equals(color) && s.size.Equals(size)).ToList();
        //        return Json(datas, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (type != null && color != null)
        //    {
        //        var datas = data.Where(s => s.type.Equals(type) && s.color.Equals(color)).ToList();
        //        return Json(datas, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (type != null)
        //    {
        //        var datas = data.Where(s => s.type.Equals(type)).ToList();
        //        return Json(datas, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //// update stock inventory
        //public JsonResult addStock(string id_uniform, string total)
        //{

        //    db.Database.ExecuteSqlCommand("update UniformInventory set stock='" + total + "' where id_uniform='" + id_uniform + "'  ");
        //    db.SaveChanges();
        //    var data = from s in db.UniformInventories select s;
        //    var query = data.Where(s => s.id_uniform.ToString() == id_uniform).Take(1);
        //    return Json(query, JsonRequestBehavior.AllowGet);
        //}



        //public JsonResult getOrder()
        //{
        //    var data = from s in db.UniformInventoryTransactions select s;
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        // function for master data menu
        // note : filter not using api
        public ActionResult AllEmployee(string kpk, string Departement_code, string section_code, string status, string company)
        {
            if (Session["username"] != null)
            {
                var Master_data = from m in db.All_employee select m;
                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");


                if (kpk != null)
                {
                    var Result = Master_data.Where(m => m.EMEMP.Equals(kpk)).ToList();
                    return View(Result);
                }
                else if (section_code != null)
                {
                    var Result = Master_data.Where(m => m.EMLOC.Equals(section_code)).ToList();
                    return View(Result);
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }



        }

        // function for menu master data (using api & filtering)
        public JsonResult AllEmployees(string kpk, string Departement_code, string section_code, string Resign, string status, string company, string typeofemployee)
        {
            // get data from database (views all employee)
            var Master_data = from m in db.All_employee select m;

            if (typeofemployee != null && status == null)
            {

                // masukan data dari database kedalam bentuk json
                var jsonResultCompany = Json(Master_data.Where(s => s.EMDEPT.Equals(Departement_code) && s.EMPAYT.Equals(typeofemployee) && s.EMTREF == null), JsonRequestBehavior.AllowGet);
                // set max value 
                jsonResultCompany.MaxJsonLength = int.MaxValue;
                // lempar data dari database untuk dikonsumsi oleh frontend
                return jsonResultCompany;
            }



            else if (status != null)
            {
                if (status == "non-active")
                {
                    var jsonResultStatus1 = Json(Master_data.Where(s => s.EMDEPT.Equals(Departement_code) && s.EMTREF != null), JsonRequestBehavior.AllowGet);
                    jsonResultStatus1.MaxJsonLength = int.MaxValue;
                    return jsonResultStatus1;
                }

                var jsonResultStatus = Json(Master_data.Where(s => s.EMDEPT.Equals(Departement_code) && s.EMTREF == null), JsonRequestBehavior.AllowGet);
                jsonResultStatus.MaxJsonLength = int.MaxValue;
                return jsonResultStatus;
            }
            else if (kpk != null && Resign == null)
            {
                var Result = Master_data.Where(m => m.EMEMP.Equals(kpk) && m.EMSTAT.Equals("Active")).ToList();
                var kpk_filter = Json(Result, JsonRequestBehavior.AllowGet);
                kpk_filter.MaxJsonLength = int.MaxValue;
                return kpk_filter;
            }
            else if (Departement_code != null && Resign == null && company == null)
            {
                //var  = from m in db.All_employees where m.EMDEPT == Departement_code select m;
                var jsonResult = Json(Master_data.Where(s => s.EMDEPT.Equals(Departement_code) && s.EMTREF == null), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else if (section_code != null && Resign == null)
            {
                var jsonResultSection = Json(Master_data.Where(s => s.SECT_CODE.Equals(section_code) && s.EMTREF == null), JsonRequestBehavior.AllowGet);
                jsonResultSection.MaxJsonLength = int.MaxValue;
                return jsonResultSection;
            }
            else if (Resign != null)
            {
                var section_filter = Json(Master_data.Where(m => m.EMTREF != null && m.EMTERM != null && m.EMSTAT == "" && m.DEPT_CODE.Equals(Resign)).Distinct().ToList(), JsonRequestBehavior.AllowGet);
                section_filter.MaxJsonLength = int.MaxValue;
                return section_filter;
            }
            else

            {
                var jsonResult = Json(Master_data.ToList(), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

            //jsonResult.MaxJsonLength = int.MaxValue;
            // return jsonResult;
        }

        //public ActionResult Employee(string q)
        //{
        //    if (Session["username"] != null)
        //    {
        //        if (q != null || q != null)
        //        {
        //            var convertTerm = int.Parse(q);
        //            var getEmployeeHistory = from h in db.Historical_employee orderby h.id_historical descending select h;
        //            var emergencyContact = from e in db.Emergency_employee select e;
        //            var getTSkill = from t in db.GradeViews select t;
        //            var getTSenior = from g in db.TotalHours select g;
        //            var getEmployeeDress = from d in db.EmployeeDresses select d;
        //            ViewBag.getEmployeeDress = getEmployeeDress.Where(g => g.EMEMP.Contains(q));
        //            ViewBag.TSkill = getTSkill.Where(t => t.EMEMP == q && t.date_value == 2).Take(1);
        //            ViewBag.TSenior = getTSenior.Where(g => g.EMEMP == q && g.periode_id == "2").Take(1);
        //            var getUmk = from u in db.SALARies select u;
        //            ViewBag.UMK = getUmk.Where(u => u.EMEMP == q).Take(1);
        //            ViewBag.HistoryEmployee = getEmployeeHistory.Where(h => h.employee_code.Equals(q)).ToList();
        //            var All_employees = db.All_employee.Where(s => s.EMEMP.Equals(q)).ToList();
        //            ViewBag.Emergency = emergencyContact.Where(e => e.Employee_code.Equals(convertTerm)).Take(1);
        //            ViewBag.getImg = q + ".jpg";
        //            ViewBag.getKPK = q;

        //            return View(All_employees);
        //        }
        //        else
        //        {
        //            return RedirectToAction("AllEmployee");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Employee");
        //    }
        //}

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
        // FUNCTION FOR EMPLOYEE PROFILE
        public ActionResult Details(string q)
        {

            if (Session["username"] != null)
            {
                var ownKpk = Session["username"].ToString();

                if (q != null && q != "" && q == ownKpk)
                {
                    // 
                    var convertTerm = int.Parse(q);
                    var getEmployeeHistory = from h in db.Historical_employee orderby h.id_historical descending select h;
                    var emergencyContact = from e in db.Emergency_employee select e;
                    var getTSkill = from t in db.GradeViews select t;
                    var getTSenior = from g in db.TotalHours select g;
                    var getEmployeeDress = from d in db.EmployeeDresses select d;
                    ViewBag.getEmployeeDress = getEmployeeDress.Where(g => g.EMEMP.Contains(q));
                    ViewBag.TSkill = getTSkill.Where(t => t.EMEMP == q && t.date_value == 2).Take(1);
                    ViewBag.TSenior = getTSenior.Where(g => g.EMEMP == q && g.periode_id == "2").Take(1);
                    var getUmk = from u in db.SALARies select u;
                    ViewBag.UMK = getUmk.Where(u => u.EMEMP == q).Take(1);
                    ViewBag.HistoryEmployee = getEmployeeHistory.Where(h => h.employee_code.Equals(q)).ToList();
                    var All_employees = db.All_employee.Where(s => s.EMEMP.Equals(q)).ToList();
                    ViewBag.Emergency = emergencyContact.Where(e => e.Employee_code.Equals(convertTerm)).Take(1);
                    ViewBag.getImg = q + ".jpg";
                    ViewBag.getKPK = q;

                    return View(All_employees);
                }
                else
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string q, string Emergency_name, string Emergency_address, string Emergency_contact, string Emergency_relation)
        {
            var convertTerm = int.Parse(q);
            var data = db.Emergency_employee.FirstOrDefault(s => s.Employee_code.Equals(convertTerm));

            if (data != null)
            {
                data.Emergency_name = Emergency_name;
                data.Emergency_contact = Emergency_contact;
                data.Emergency_address = Emergency_address;
                data.Emergency_relation = Emergency_relation;
                db.SaveChanges();
                return RedirectToAction("Details", new { q = q });
            }


            return View();
        }

        public JsonResult detailShow(string q)
        {
            var data_masters = from m in db.All_employee let value = m.EMEMP let text = m.EMNAME select new { value, text };
            var data_master = data_masters.Where(m => m.value.Contains(q) || m.text.Contains(q));
            var jsonResult = Json(data_master, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        //public JsonResult getEmployee(string kpk, string emname)
        //{
        //    var Master_ERT = from m in db.ERTs select m;
        //    var checkErt = Master_ERT.Where(s => s.EMEMP.Equals(kpk)).Take(1);
        //    if (checkErt.Any())
        //    {
        //        return Json(kpk + " sudah terdaftar sebagai anggota ERT", JsonRequestBehavior.AllowGet);

        //    }
        //    var MasterEmployee = from m in db.Employees select m;
        //    var checkData = MasterEmployee.Where(s => s.EMEMP.Equals(kpk)).Take(1);
        //    if (checkData.Any())
        //    {


        //        db.Database.ExecuteSqlCommand("INSERT INTO ERT VALUES ('" + kpk + "', '" + emname + "' ) ");
        //        db.SaveChanges();
        //        return Json(kpk + " berhasil terdaftar sebagai anggota ERT", JsonRequestBehavior.AllowGet);

        //    }
        //    return Json(kpk + " sudah tidak terdaftar sebagai  karyawan di mattel", JsonRequestBehavior.AllowGet);
        //}
        public JsonResult NonActiveErt(string kpk)
        {
            db.Database.ExecuteSqlCommand("DELETE ERT WHERE EMEMP='" + kpk + "'");
            db.SaveChanges();
            return Json(kpk + " berhasil dihapus dari daftar ert", JsonRequestBehavior.AllowGet);
        }


        //  function for upload master data (master data menu) Non-staff
        [HttpPost]
        public ActionResult UploadMasterEmployeeSheet(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile.ContentType == "application/vnd.ms-excel" || postedFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = postedFile.FileName;
                // path Doc
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
                                //truncate table temp_employee then bulk with new data
                                string strSql = "TRUNCATE TABLE Temp_Employee";
                                SqlCommand cmd = new SqlCommand(strSql, con);
                                cmd.ExecuteNonQuery();
                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();
                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_Employee";
                                    sqlBulkCopy.ColumnMappings.Add("EMCMPY", "EMCMPY");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMP#", "EMEMP");
                                    sqlBulkCopy.ColumnMappings.Add("EMNAME", "EMNAME");
                                    sqlBulkCopy.ColumnMappings.Add("EMCMPY2", "EMCMPY2");
                                    sqlBulkCopy.ColumnMappings.Add("EMSEXT", "EMSEXT");
                                    sqlBulkCopy.ColumnMappings.Add("EMCOMM", "EMCOMM");
                                    sqlBulkCopy.ColumnMappings.Add("ENDCONT", "ENDCONT");
                                    sqlBulkCopy.ColumnMappings.Add("OLDOCONT", "OLDOCONT");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMST", "EMEMST");
                                    sqlBulkCopy.ColumnMappings.Add("EMJOBA", "EMJOBA");
                                    sqlBulkCopy.ColumnMappings.Add("EMDEPT", "EMDEPT");
                                    sqlBulkCopy.ColumnMappings.Add("EMLOC#", "EMLOC");
                                    sqlBulkCopy.ColumnMappings.Add("EMJOBD", "EMJOBD");
                                    sqlBulkCopy.ColumnMappings.Add("EMSUP#", "EMSUP#");
                                    sqlBulkCopy.ColumnMappings.Add("SUPNAME", "SUPNAME");
                                    sqlBulkCopy.ColumnMappings.Add("EMBIRT", "EMBIRT");
                                    sqlBulkCopy.ColumnMappings.Add("EMMRST", "EMMRST");
                                    sqlBulkCopy.ColumnMappings.Add("EMRELI", "EMRELI");
                                    sqlBulkCopy.ColumnMappings.Add("EMGRAD", "EMGRAD");
                                    sqlBulkCopy.ColumnMappings.Add("EMHTLC", "EMHTLC");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSR", "EMBUSR");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSS", "EMBUSS");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSL", "EMBUSL");
                                    sqlBulkCopy.ColumnMappings.Add("EMASN#", "EMASN#");
                                    sqlBulkCopy.ColumnMappings.Add("EMWKHR", "EMWKHR");
                                    sqlBulkCopy.ColumnMappings.Add("EMITST", "EMITST");
                                    sqlBulkCopy.ColumnMappings.Add("EMBEDU", "EMBEDU");
                                    sqlBulkCopy.ColumnMappings.Add("EMPAYT", "EMPAYT");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD1", "EMADD1");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD2", "EMADD2");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD3", "EMADD3");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMAD", "EMEMAD");
                                    sqlBulkCopy.ColumnMappings.Add("EMUNIO", "EMUNIO");
                                    sqlBulkCopy.ColumnMappings.Add("EMUSID", "EMUSID");
                                    sqlBulkCopy.ColumnMappings.Add("EMITAX", "EMITAX");
                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }
                                // Updating destination table, and dropping temp table
                                command.CommandTimeout = 300;
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                // call procedure Employee_bulk for insert to real table
                                command.CommandText = "Employee_bulk";
                                command.ExecuteScalar();
                                try
                                {
                                    // generate historical
                                    command.CommandText = "Historical_employees";
                                    command.ExecuteScalar();
                                    // update YoS staff
                                    command.CommandText = "SyncTotalHours";
                                    command.ExecuteScalar();
                                    // update YoS Non-staff
                                    command.CommandText = "SyncTotalHoursNonStaff";
                                    command.ExecuteScalar();
                                    // Registration staff on Leave Management 
                                    command.CommandText = "Proce_LeaveRequest_Annualleave";
                                    command.ExecuteScalar();
                                    // call procedure for assign role (default role user)
                                    command.CommandText = "RoleAttached";
                                    command.ExecuteScalar();
                                }
                                catch (System.Exception)
                                {
                                    throw;
                                }


                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = error;
                                // call all view bag for filtering
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        // Staff
        [HttpPost]
        public ActionResult UploadMasterStaffEmployeeSheet(HttpPostedFileBase MasterStaffEmployee)
        {
            string filePath = string.Empty;
            if (MasterStaffEmployee.ContentType == "application/vnd.ms-excel" || MasterStaffEmployee.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = MasterStaffEmployee.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                MasterStaffEmployee.SaveAs(targetpath + filename);
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
                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_Staff_Employee";
                                    sqlBulkCopy.ColumnMappings.Add("EMCMPY", "EMCMPY");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMP", "EMEMP");
                                    sqlBulkCopy.ColumnMappings.Add("EMNAME", "EMNAME");
                                    sqlBulkCopy.ColumnMappings.Add("EMSEXT", "EMSEXT");
                                    sqlBulkCopy.ColumnMappings.Add("EMCOMM", "EMCOMM");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMST", "EMEMST");
                                    sqlBulkCopy.ColumnMappings.Add("EMJOBA", "EMJOBA");
                                    sqlBulkCopy.ColumnMappings.Add("EMDEPT", "EMDEPT");
                                    sqlBulkCopy.ColumnMappings.Add("EMLOC#", "EMLOC");
                                    sqlBulkCopy.ColumnMappings.Add("EMJOBD", "EMJOBD");
                                    sqlBulkCopy.ColumnMappings.Add("EMSUP#", "EMSUP");
                                    sqlBulkCopy.ColumnMappings.Add("EMBIRT", "EMBIRT");
                                    sqlBulkCopy.ColumnMappings.Add("EMMRST", "EMMRST");
                                    sqlBulkCopy.ColumnMappings.Add("EMRELI", "EMRELI");
                                    sqlBulkCopy.ColumnMappings.Add("EMGRAD", "EMGRAD");
                                    sqlBulkCopy.ColumnMappings.Add("EMHTLC", "EMHTLC");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSR", "EMBUSR");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSS", "EMBUSS");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSL", "EMBUSL");
                                    sqlBulkCopy.ColumnMappings.Add("EMASN#", "EMASN#");
                                    sqlBulkCopy.ColumnMappings.Add("EMWKHR", "EMWKHR");
                                    sqlBulkCopy.ColumnMappings.Add("EMITST", "EMITST");
                                    sqlBulkCopy.ColumnMappings.Add("EMBEDU", "EMBEDU");
                                    sqlBulkCopy.ColumnMappings.Add("EMPAYT", "EMPAYT");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD1", "EMADD1");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD2", "EMADD2");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD3", "EMADD3");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMAD", "EMEMAD");
                                    sqlBulkCopy.ColumnMappings.Add("EMUNIO", "EMUNIO");
                                    sqlBulkCopy.ColumnMappings.Add("EMUSID", "EMUSID");
                                    sqlBulkCopy.ColumnMappings.Add("EMITAX", "EMITAX");
                                    sqlBulkCopy.ColumnMappings.Add("EMTEAY", "EMTEAY");
                                    sqlBulkCopy.ColumnMappings.Add("EMAD04", "EMAD04");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD4", "EMADD4");
                                    sqlBulkCopy.ColumnMappings.Add("EMCMP2", "EMCMP2");
                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                //  Updating destination table, and dropping temp table
                                command.CommandTimeout = 300;
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.CommandText = "Employee_staff";
                                command.ExecuteScalar();
                                command.CommandText = "Historical_employees";
                                command.ExecuteScalar();
                                // update YoS staff
                                command.CommandText = "SyncTotalHours";
                                command.ExecuteScalar();
                                // update YoS Non-staff
                                command.CommandText = "SyncTotalHoursNonStaff";
                                command.ExecuteScalar();
                                // Registration staff on Leave Management
                                command.CommandText = "Proce_LeaveRequest_Annualleave";
                                command.ExecuteScalar();
                                // role staff
                                command.CommandText = "RoleAttached";
                                command.ExecuteScalar();
                                // adjust mandatory leave
                                command.CommandText = "Proce_adjustMandatoryLeave";
                                command.ExecuteScalar();
                                // total mandatory leave
                                command.CommandText = "Proce_adjustTotalMandatoryLeave";
                                command.ExecuteScalar();

                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = error;
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return View("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        // Resign
        [HttpPost]
        public ActionResult UploadMasterResignEmployeeSheet(HttpPostedFileBase MasterResign)
        {
            string filePath = string.Empty;
            if (MasterResign.ContentType == "application/vnd.ms-excel" || MasterResign.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {


                string filename = MasterResign.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                MasterResign.SaveAs(targetpath + filename);
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

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                string strSql = "TRUNCATE TABLE Temp_Master_Resign";
                                SqlCommand cmd = new SqlCommand(strSql, con);
                                cmd.ExecuteNonQuery();
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_Master_Resign";
                                    sqlBulkCopy.ColumnMappings.Add("EMCMPY", "'EMCMPY");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMP#", "'EMEMP#");
                                    sqlBulkCopy.ColumnMappings.Add("EMNAME", "EMNAME");
                                    sqlBulkCopy.ColumnMappings.Add("EMSEXT", "EMSEXT");
                                    sqlBulkCopy.ColumnMappings.Add("EMCOMM", "EMCOMM");
                                    sqlBulkCopy.ColumnMappings.Add("EMTERM", "EMTERM");
                                    sqlBulkCopy.ColumnMappings.Add("EMTREF", "EMTREF");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMST", "'DTIYMD");
                                    sqlBulkCopy.ColumnMappings.Add("EMJOBA", "EMJOBA");
                                    sqlBulkCopy.ColumnMappings.Add("EMDEPT", "EMDEPT");
                                    sqlBulkCopy.ColumnMappings.Add("EMLOC#", "'DTSITM");
                                    sqlBulkCopy.ColumnMappings.Add("EMJOBD", "EMJOBD");
                                    sqlBulkCopy.ColumnMappings.Add("EMSUP#", "EMSUP");
                                    sqlBulkCopy.ColumnMappings.Add("EMBIRT", "EMBIRT");
                                    sqlBulkCopy.ColumnMappings.Add("EMMRST", "EMMRST");
                                    sqlBulkCopy.ColumnMappings.Add("EMRELI", "EMRELI");
                                    sqlBulkCopy.ColumnMappings.Add("EMGRAD", "EMGRAD");
                                    sqlBulkCopy.ColumnMappings.Add("EMHTLC", "EMHTLC");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSR", "EMBUSR");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSS", "EMBUSS");
                                    sqlBulkCopy.ColumnMappings.Add("EMBUSL", "EMBUSL");
                                    sqlBulkCopy.ColumnMappings.Add("EMASN#", "EMASN#");
                                    sqlBulkCopy.ColumnMappings.Add("EMWKHR", "EMWKHR");
                                    sqlBulkCopy.ColumnMappings.Add("EMITST", "EMITST");
                                    sqlBulkCopy.ColumnMappings.Add("EMBEDU", "EMBEDU");
                                    sqlBulkCopy.ColumnMappings.Add("EMPAYT", "EMPAYT");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD1", "EMADD1");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD2", "EMADD2");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD3", "EMADD3");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMAD", "EMEMAD");
                                    sqlBulkCopy.ColumnMappings.Add("EMUNIO", "EMUNIO");
                                    sqlBulkCopy.ColumnMappings.Add("EMUSID", "EMUSID");
                                    sqlBulkCopy.ColumnMappings.Add("EMPHON", "EMPHON");
                                    sqlBulkCopy.ColumnMappings.Add("EMITAX", "EMITAX");
                                    sqlBulkCopy.ColumnMappings.Add("EMEMPH", "EMEMPH");
                                    sqlBulkCopy.ColumnMappings.Add("EMADD4", "EMADD4");
                                    sqlBulkCopy.ColumnMappings.Add("EMAD01", "EMAD01");
                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                command.CommandTimeout = 300;
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.CommandText = "Resign";
                                command.ExecuteScalar();
                                try
                                {
                                    command.CommandType = System.Data.CommandType.StoredProcedure;
                                    command.CommandText = "Historical_employees";
                                    command.ExecuteScalar();
                                }
                                catch (System.Exception)
                                {

                                    throw;
                                }
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = error;
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        // checking need
        [HttpPost]
        public ActionResult UploadMasterStaffEmployeeSheet1(HttpPostedFileBase MasterStaffEmployee1)
        {
            string filePath = string.Empty;
            if (MasterStaffEmployee1.ContentType == "application/vnd.ms-excel" || MasterStaffEmployee1.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = MasterStaffEmployee1.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                MasterStaffEmployee1.SaveAs(targetpath + filename);
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
                                // string strSql = "TRUNCATE TABLE Temp_Staff_Employee";
                                // SqlCommand  cmd=new SqlCommand(strSql,con);
                                // cmd.ExecuteNonQuery();

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_staff_Employee_2";

                                    sqlBulkCopy.ColumnMappings.Add("EMEMP", "EMEMP");
                                    sqlBulkCopy.ColumnMappings.Add("EMNAME", "EMNAME");

                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                // command.CommandTimeout = 300;
                                // command.CommandType = System.Data.CommandType.StoredProcedure;
                                // command.CommandText = "Employee_staff";
                                // command.ExecuteScalar();      
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = "upload-gagal";
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        //  Email 
        [HttpPost]
        public ActionResult EmailEmployee(HttpPostedFileBase email)
        {
            string filePath = string.Empty;
            if (email.ContentType == "application/vnd.ms-excel" || email.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = email.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                email.SaveAs(targetpath + filename);
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
                                // string strSql = "TRUNCATE TABLE Temp_Staff_Employee";
                                // SqlCommand  cmd=new SqlCommand(strSql,con);
                                // cmd.ExecuteNonQuery();

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_email_employee";

                                    sqlBulkCopy.ColumnMappings.Add("EMEMP", "EMEMP");
                                    sqlBulkCopy.ColumnMappings.Add("EMAIL", "EMAIL");

                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                // command.CommandTimeout = 300;
                                // command.CommandType = System.Data.CommandType.StoredProcedure;
                                // command.CommandText = "Employee_staff";
                                // command.ExecuteScalar();      
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = "upload-gagal";
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        // BPJS KESEHATAN
        [HttpPost]
        public ActionResult UploadBpjsKesehatan(HttpPostedFileBase BPJSKesehatan)
        {
            string filePath = string.Empty;
            if (BPJSKesehatan.ContentType == "application/vnd.ms-excel" || BPJSKesehatan.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = BPJSKesehatan.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                BPJSKesehatan.SaveAs(targetpath + filename);
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
                                // string strSql = "TRUNCATE TABLE Temp_Staff_Employee";
                                // SqlCommand  cmd=new SqlCommand(strSql,con);
                                // cmd.ExecuteNonQuery();

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_BPJS_Kesehatan";

                                    sqlBulkCopy.ColumnMappings.Add("KPK", "KPK");
                                    sqlBulkCopy.ColumnMappings.Add("BPJS", "BPJS");

                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                // command.CommandTimeout = 300;
                                // command.CommandType = System.Data.CommandType.StoredProcedure;
                                // command.CommandText = "Employee_staff";
                                // command.ExecuteScalar();      
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = "upload-gagal";
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        // BPJS KETENAGAKERJAAN
        [HttpPost]
        public ActionResult BPJSKetenagakerjaan(HttpPostedFileBase BPJSKetenagakerjaan)
        {
            string filePath = string.Empty;
            if (BPJSKetenagakerjaan.ContentType == "application/vnd.ms-excel" || BPJSKetenagakerjaan.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = BPJSKetenagakerjaan.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                BPJSKetenagakerjaan.SaveAs(targetpath + filename);
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
                                // string strSql = "TRUNCATE TABLE Temp_Staff_Employee";
                                // SqlCommand  cmd=new SqlCommand(strSql,con);
                                // cmd.ExecuteNonQuery();

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_BPJS_Ketenagakerjaan";
                                    sqlBulkCopy.ColumnMappings.Add("KPK", "KPK");
                                    sqlBulkCopy.ColumnMappings.Add("BPJS", "BPJS");
                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                command.CommandTimeout = 300;
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.CommandText = "Proce_BPJSKetenagekerjaan";
                                command.ExecuteScalar();
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = "upload-gagal";
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        //  Emergency contact
        [HttpPost]
        public ActionResult EmergencyContact(HttpPostedFileBase EmergencyContact)
        {
            string filePath = string.Empty;
            if (EmergencyContact.ContentType == "application/vnd.ms-excel" || EmergencyContact.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = EmergencyContact.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                EmergencyContact.SaveAs(targetpath + filename);
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
                                // string strSql = "TRUNCATE TABLE Temp_Staff_Employee";
                                // SqlCommand  cmd=new SqlCommand(strSql,con);
                                // cmd.ExecuteNonQuery();

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_email_employee";

                                    sqlBulkCopy.ColumnMappings.Add("EMEMP", "EMEMP");
                                    sqlBulkCopy.ColumnMappings.Add("EMAIL", "EMAIL");

                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                // command.CommandTimeout = 300;
                                // command.CommandType = System.Data.CommandType.StoredProcedure;
                                // command.CommandText = "Employee_staff";
                                // command.ExecuteScalar();      
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = "upload-gagal";
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        //  EKTP
        [HttpPost]
        public ActionResult Ektp(HttpPostedFileBase Ektp)
        {
            string filePath = string.Empty;
            if (Ektp.ContentType == "application/vnd.ms-excel" || Ektp.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                string filename = Ektp.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                Ektp.SaveAs(targetpath + filename);
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
                                // string strSql = "TRUNCATE TABLE Temp_Staff_Employee";
                                // SqlCommand  cmd=new SqlCommand(strSql,con);
                                // cmd.ExecuteNonQuery();

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_email_employee";

                                    sqlBulkCopy.ColumnMappings.Add("EMEMP", "EMEMP");
                                    sqlBulkCopy.ColumnMappings.Add("EMAIL", "EMAIL");

                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                // command.CommandTimeout = 300;
                                // command.CommandType = System.Data.CommandType.StoredProcedure;
                                // command.CommandText = "Employee_staff";
                                // command.ExecuteScalar();      
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = "upload-gagal";
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }

        // Phone
        [HttpPost]
        public ActionResult UploadMasterPhoneEmployeeSheet(HttpPostedFileBase SheetPhone)
        {
            string filePath = string.Empty;
            if (SheetPhone.ContentType == "application/vnd.ms-excel" || SheetPhone.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {


                string filename = SheetPhone.FileName;
                string targetpath = Server.MapPath("~/Doc/");
                SheetPhone.SaveAs(targetpath + filename);
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

                                //Creating temp table on database
                                //command.CommandText = "CREATE TABLE TmpTable( employee_code varchar(50), employee_name varchar(50), day_in varchar(50), day_out varchar(50), clock_in varchar(50),  clock_out varchar(50), shift varchar(50), wg varchar(50),  weekly varchar(50), overtime varchar(50))";
                                // command.ExecuteNonQuery();

                                //Bulk insert into temp table
                                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con))
                                {
                                    bulkcopy.BulkCopyTimeout = 660;
                                    bulkcopy.DestinationTableName = "Temp_phone_employee";
                                    sqlBulkCopy.ColumnMappings.Add("EMEMP#", "EMEMP");
                                    sqlBulkCopy.ColumnMappings.Add("EMNAME", "EMNAME");
                                    sqlBulkCopy.ColumnMappings.Add("NOMORTELEPON", "NOTLPN");
                                    bulkcopy.WriteToServer(dt);
                                    bulkcopy.Close();
                                }

                                // Updating destination table, and dropping temp table
                                /*  command.CommandTimeout = 300;
                                 command.CommandType = System.Data.CommandType.StoredProcedure;
                                 command.CommandText = "Employee_Resign";
                                 command.ExecuteScalar();    */
                            }
                            catch (Exception error)
                            {
                                ViewBag.Message = error;
                                ViewBag.Departement_code = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
                                ViewBag.section_code = new SelectList(db.Sections, "SECT_CODE", "SECT_NAME");
                                ViewBag.job_code = new SelectList(db.Jobs, "JOB_CODE", "Description");
                                return RedirectToAction("AllEmployee");     // Handle exception properly
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
            return View("AllEmployee");
        }



    }
}