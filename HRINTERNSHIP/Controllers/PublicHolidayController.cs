using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRINTERNSHIP.Models;
namespace HRIS_Employee.Controllers
{
    public class PublicHolidayController : Controller
    {
        private DBModel db = new DBModel();
        // GET: PublicHolidays
        public ActionResult Index()
        {
            if(Session["Username"] != null)
            {
                var data = from p in db.Public_Holidays select p;
                return View(data);
            }
            return RedirectToAction("Login", "Employee");

            
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Date_public_holidays,Date_description, Date_mandatory, Type_mandatory")] Public_Holidays public_Holidays)
        //{
            
        //        db.Public_Holidays.Add(public_Holidays);
        //        db.SaveChanges();
              
        //        return RedirectToAction("Index");
            
        //}
        // POST: Public_Holidays/Delete/5
        // function for delete date
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int id)
        {
            Public_Holidays public_Holidays = db.Public_Holidays.Find(id);
            db.Public_Holidays.Remove(public_Holidays);
            db.SaveChanges();
            
            return RedirectToAction("Index");
          
        }
        // function for generate api public holiday
        public JsonResult getPublicHolidays()
        {
            var Year = DateTime.Now.Year.ToString();
            var publicholidays = from s in db.Public_Holidays where s.Date_public_holidays.Substring(0, 4) == Year select s;
            ViewBag.Date = publicholidays;
            return Json(publicholidays , JsonRequestBehavior.AllowGet);
        }
        public JsonResult getMandatory()
        {
            var Year = DateTime.Now.Year.ToString();
            var publicholidays = from s in db.Public_Holidays where s.Date_public_holidays.Substring(0,4) == Year select s.Date_mandatory ;
            return Json(publicholidays, JsonRequestBehavior.AllowGet);
        }
        public JsonResult newPublicHolidays()
        {
            var Year = DateTime.Now.Year.ToString();
            var newpublicholidays = from s in db.NewCalendarEvents select s;
            return Json(newpublicholidays, JsonRequestBehavior.AllowGet);
        }
        public JsonResult newcreate(string title, string start, string description, string end, string location)
        {
            var chars = "0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            db.Database.ExecuteSqlCommand("INSERT INTO NewCalendarEvent VALUES ('" + finalString + "','" + title + "', '" + start + "', '" + description + "', '" + end + "','fc-event-danger fc-event-solid-warning','" + location + "') ");
            db.Database.ExecuteSqlCommand("INSERT INTO Public_Holidays values('" + finalString + "','" + start + "','" + description + "','" + start + "','PublicHo','" + finalString + "')");
            db.SaveChanges();
            return Json("success");
        }
        public JsonResult newupdate(string title, string start, string description, string end, string location, string id)
        {
            db.Database.ExecuteSqlCommand("UPDATE NewCalendarEvent set title = '" + title + "',start = '" + start + "',description = '" + description + "',[end] = '" + end + "',location = '" + location + "' where id = '" + id + "'");
            db.Database.ExecuteSqlCommand("UPDATE Public_Holidays set Date_description = '" + description + "',Date_public_holidays = '" + start + "',Date_mandatory = '" + start + "' where id = '" + id + "'");
            db.SaveChanges();
            return Json("success");
        }
        public JsonResult newdelete(string id)
        {
            db.Database.ExecuteSqlCommand("DELETE NewCalendarEvent where id = '" + id + "'");
            db.Database.ExecuteSqlCommand("DELETE Public_Holidays where id = '" + id + "'");
            db.SaveChanges();
            return Json("success");
        }
    }
}