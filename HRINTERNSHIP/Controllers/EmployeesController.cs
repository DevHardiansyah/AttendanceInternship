using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRINTERNSHIP.Models;

namespace HRIS_Employee.Controllers
{
    public class EmployeesController : Controller
    {
        private DBModel db = new DBModel();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.Where( s => s.EMPAYT== "M").ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EMCMPY,EMEMP,EMNAME,EMSEXT,EMCOMM,ENDCONT,OLDOCONT,EMEMST,EMJOBA,EMDEPT,EMLOC,EMJOBD,EMSUP_,SUPNAME,EMBIRT,EMMRST,EMRELI,EMGRAD,EMHTLC,EMBUSR,EMBUSS,EMBUSL,EMASN,EMWKHR,EMITST,EMBEDU,EMPAYT,EMADD1,EMADD2,EMADD3,EMEMAD,EMUNIO,EMUSID,EMITAX,EMSTAT,EMTERM,EMTREF,PHONE,EMAIL,TOTALHOURS,TOTALYEARS,DETAILWORKEXPERIENCE,FIRST_NAME,LAST_NAME,PSID,BUS_ROUTE,EOC,LABEL_1,LABEL_2,LABEL_3")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EMCMPY,EMEMP,EMNAME,EMSEXT,EMCOMM,ENDCONT,OLDOCONT,EMEMST,EMJOBA,EMDEPT,EMLOC,EMJOBD,EMSUP_,SUPNAME,EMBIRT,EMMRST,EMRELI,EMGRAD,EMHTLC,EMBUSR,EMBUSS,EMBUSL,EMASN,EMWKHR,EMITST,EMBEDU,EMPAYT,EMADD1,EMADD2,EMADD3,EMEMAD,EMUNIO,EMUSID,EMITAX,EMSTAT,EMTERM,EMTREF,PHONE,EMAIL,TOTALHOURS,TOTALYEARS,DETAILWORKEXPERIENCE,FIRST_NAME,LAST_NAME,PSID,BUS_ROUTE,EOC,LABEL_1,LABEL_2,LABEL_3")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
