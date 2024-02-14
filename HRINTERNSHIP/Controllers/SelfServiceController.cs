using HRINTERNSHIP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HRIS_Employee.Controllers
{
    public class SelfServiceController : Controller
    {
        private DBModel db = new DBModel();
        // GET: SelfService

        // function for manage email
        public ActionResult Index()
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            var data = from s in db.SelfService_EmailService select s;
            var naturalize = data.FirstOrDefault().Password;
            var password_enc = naturalize.Replace(' ', '+');
            var password = DecryptString(key, password_enc);
            ViewBag.password = password;
            return View(data);
        }

        // function for show main page (Management Password Menu)
        public ActionResult ManagementPassword()
        {
            if (Session["username"] != null)
            {
                ViewBag.Role = db.Roles.ToList();
                var data = from s in db.ListUsers select s;
                return View(data.Count());

            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public JsonResult ChangeRole(string kpk, string role_id)
        {
            db.Database.ExecuteSqlCommand("UPDATE Users set roles_id = '" + role_id + "' where username = '" + kpk + "'");
            db.SaveChanges();

            return Json("Success");
        }

        //function for show data    
        public JsonResult showAllData(string kpk)
        {
            var data = from s in db.ListUsers where s.user_id == kpk select s;
            var data2 = from s in db.Intern_ListUsers where s.username == kpk select s;
            if (data.Count() == 0)
            {
                return Json(data2, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        // function fro update data(password)
        public JsonResult updatePassword(string kpk)
        {
            db.Database.ExecuteSqlCommand("UPDATE users set password ='" + kpk + "', status_update_password='0' where username = '" + kpk + "'");
            return Json("successfully updated password for user id", JsonRequestBehavior.AllowGet);
        }

        public ActionResult update(string email_from, string email, string password)
        {
            if (Session["username"] != null)
            {
                var Type = Session["PaymentStatus"].ToString();

                if (Type == "M")
                {
                    var key = "b14ca5898a4e4133bbce2ea2315a1916";
                    var getKpk = Session["username"];
                    var Name = Session["user"];
                    var status_payment = Session["PaymentStatus"];
                    var password_enc = EncryptString(key, password);
                    db.Database.ExecuteSqlCommand("UPDATE SelfService_EmailService SET KPK='" + getKpk + "', Name='" + Name + "', Email='" + email + "', Password='" + password_enc + "', Email_from='" + email_from + "' ");
                    db.SaveChanges();
                    TempData["success"] = "success";
                    return RedirectToAction("Index");
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
        
        public ActionResult Role()
        {
            if (Session["username"] != null)
            {
                ViewBag.ListAction = from d in db.permissions select d;
                ViewBag.Role = db.Roles.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public ActionResult ViewRole(string role_id)
        {
            if (Session["username"] != null)
            {
                var dataemployee = db.ListUsers.Where(x => x.roles_id == role_id).Count();
                var datainter = db.Intern_ListUsers.Where(x => x.roles_id == role_id).Count();
                if (dataemployee > 0)
                {
                    ViewBag.Data = db.ListUsers.Where(x => x.roles_id == role_id);
                }
                else
                {
                    ViewBag.Data = db.Intern_ListUsers.Where(x => x.roles_id == role_id);
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
        }
        public ActionResult Permission()
        {
            if (Session["username"] != null) 
            {
                var data = db.permissions.ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Employee");
            }
            
        }
        public JsonResult CreatePermission(string permission_name)
        {
            int permissionid = db.permissions.Count();
            int permissionidplus1 = permissionid + 1;

            db.Database.ExecuteSqlCommand("INSERT INTO Permission VALUES ('"+ permissionidplus1 + "','" + permissionidplus1 + "','" + permission_name + "')");
            db.SaveChanges();
            return Json("Success");
        }
        public JsonResult UpdatePermission(string permission_id, string permission_name)
        {
            db.Database.ExecuteSqlCommand("UPDATE permission set permission_description = '"+permission_name+"' WHERE permission_id = '"+permission_id+"'");
            db.SaveChanges();
            return Json("Success");
        }
        public JsonResult DeletePermission(string permission_id)
        {
            db.Database.ExecuteSqlCommand("DELETE permission WHERE permission_id = '" + permission_id + "'");
            db.SaveChanges();
            return Json("Success");
        }
    }
}