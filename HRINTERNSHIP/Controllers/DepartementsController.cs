using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRINTERNSHIP.Models;

namespace HRINTERNSHIP.Controllers
{
    public class DepartementsController : Controller
    {
        private DBModel db = new DBModel();

        // GET: Departements
        public ActionResult Index()
        {
            ViewBag.GetSection = db.Sections.ToList();
            ViewBag.GetJob = db.Jobs.ToList();
            var data = db.Departements.ToList();
            return View(data);
        }

        public JsonResult getDepartement(){
            var data = from s in db.Departements select s;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getSection(){
            var data = from s in db.Sections select s;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getJob(){
            var data =  from s in db.Jobs select s;
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        // untuk function create,update,delete 
        public ActionResult CreateDept(string DEPT_CODE, string DEPT_NAME)
        {
            db.Database.ExecuteSqlCommand("INSERT INTO Departement VALUES ('"+DEPT_CODE+ "','" + DEPT_NAME + "')");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Add Data";
            return RedirectToAction("Index");
        }
        public ActionResult UpdateDept(string DEPT_CODE, string DEPT_NAME)
        {
            db.Database.ExecuteSqlCommand("UPDATE Departement SET DEPT_NAME = '"+DEPT_NAME+"' where DEPT_CODE = '"+DEPT_CODE+"' ");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Update Data";
            return RedirectToAction("Index");
        }
        public ActionResult DeleteDept(string DEPT_CODE, string DEPT_NAME)
        {
            db.Database.ExecuteSqlCommand("DELETE Departement WHERE DEPT_CODE = '"+DEPT_CODE+"'");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Delete Data";
            return RedirectToAction("Index");
        }

        public ActionResult CreateJob(string JOB_CODE, string Description)
        {
            db.Database.ExecuteSqlCommand("INSERT INTO Job VALUES ('" + JOB_CODE + "','" + Description + "')");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Add Data";
            return RedirectToAction("Index");
        }
        public ActionResult UpdateJob(string JOB_CODE, string Description)
        {
            db.Database.ExecuteSqlCommand("UPDATE Job SET Description = '" + Description + "' where JOB_CODE = '" + JOB_CODE + "' ");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Update Data";
            return RedirectToAction("Index");
        }
        public ActionResult DeleteJob(string JOB_CODE, string Description)
        {
            db.Database.ExecuteSqlCommand("DELETE Job WHERE JOB_CODE = '" + JOB_CODE + "'");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Delete Data";
            return RedirectToAction("Index");
        }

        public ActionResult CreateSection(string SECT_CODE, string SECT_NAME, string DEPT_CODE)
        {
            db.Database.ExecuteSqlCommand("INSERT INTO Section VALUES ('" + SECT_CODE + "','" + SECT_NAME + "','" + DEPT_CODE + "')");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Add Data";
            return RedirectToAction("Index");
        }
        public ActionResult UpdateSection(string SECT_CODE, string SECT_NAME, string DEPT_CODE)
        {
            db.Database.ExecuteSqlCommand("UPDATE Section SET SECT_NAME = '" + SECT_NAME + "', Departement_code = '" + DEPT_CODE + "' where SECT_CODE = '" + SECT_CODE + "' ");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Update Data";
            return RedirectToAction("Index");
        }
        public ActionResult DeleteSection(string SECT_CODE, string SECT_NAME, string DEPT_CODE)
        {
            db.Database.ExecuteSqlCommand("DELETE Section WHERE SECT_CODE = '" + SECT_CODE + "'");
            db.SaveChanges();
            @TempData["AllertSuccess"] = "Success Delete Data";
            return RedirectToAction("Index");
        }









        // GET: Departements/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departement departement = db.Departements.Find(id);
            if (departement == null)
            {
                return HttpNotFound();
            }
            return View(departement);
        }

        // GET: Departements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DEPT_CODE,DEPT_NAME")] Departement departement)
        {
            if (ModelState.IsValid)
            {
                db.Departements.Add(departement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(departement);
        }

        // GET: Departements/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departement departement = db.Departements.Find(id);
            if (departement == null)
            {
                return HttpNotFound();
            }
            return View(departement);
        }

        // POST: Departements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DEPT_CODE,DEPT_NAME")] Departement departement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(departement);
        }

        // GET: Departements/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departement departement = db.Departements.Find(id);
            if (departement == null)
            {
                return HttpNotFound();
            }
            return View(departement);
        }

        // POST: Departements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Departement departement = db.Departements.Find(id);
            db.Departements.Remove(departement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
