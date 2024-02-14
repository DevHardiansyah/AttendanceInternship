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
    public class BussinessIntelligenceController : Controller
    {
        private DBModel db = new DBModel();
        public ActionResult EmployeeDashboard()
        {
            if(Session["username"] != null)
            {
                var data = from e in db.Intern_MasterData select e;
                int InternAll = data.Count();
                ViewBag.AllIntern = InternAll;
                return View();
            }
            else
            {
                return RedirectToAction("Login","Employee");
            }
        }
        // headcount departmemty
        public ActionResult getDepartment()
        {
            var data = from e in db.Employees select e;
            int HumanResources = data.Where(x => x.EMDEPT == "JJ" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int ManufacturingWestPlant = data.Where(x => x.EMDEPT == "MM" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int Engineering = data.Where(x => x.EMDEPT == "II" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int ProductDevelopment = data.Where(x => x.EMDEPT == "HH" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int Ehs = data.Where(x => x.EMDEPT == "GG" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int Quality = data.Where(x => x.EMDEPT == "FF" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int Materials = data.Where(x => x.EMDEPT == "EE" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int ManufacturingEastPlant = data.Where(x => x.EMDEPT == "DD" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int IT = data.Where(x => x.EMDEPT == "CC" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int Finance = data.Where(x => x.EMDEPT == "BB" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int ManagingDirectorOffice = data.Where(x => x.EMDEPT == "AA" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            // ???
            HeadCount Obj = new HeadCount();
            Obj.HumanResources = HumanResources;
            Obj.ManufacturingWestPlant = ManufacturingWestPlant;
            Obj.Engineering = Engineering;
            Obj.ProductDevelopment = ProductDevelopment;
            Obj.Ehs = Ehs;
            Obj.Quality = Quality;
            Obj.Materials = Materials;
            Obj.ManufacturingEastPlant = ManufacturingEastPlant;
            Obj.IT = IT;
            Obj.Finance = Finance;
            Obj.ManagingDirectorOffice = ManagingDirectorOffice;

            return Json(Obj, JsonRequestBehavior.AllowGet);

        }
        public class HeadCount
        {
            public int HumanResources { get; set; }
            public int ManufacturingWestPlant { get; set; }
            public int Engineering { get; set; }
            public int ProductDevelopment { get; set; }
            public int Ehs { get; set; }
            public int Quality { get; set; }
            public int Materials { get; set; }
            public int ManufacturingEastPlant { get; set; }
            public int Finance { get; set; }
            public int IT { get; set; }
            public int ManagingDirectorOffice { get; set; }
        }
        // total head count
        public ActionResult getPlant()
        {
            var data = from e in db.Employees select e;
            int EastPlant = data.Where(x => x.EMCMPY == "PTME" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            int WestPlant = data.Where(x => x.EMCMPY == "PTMW" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            PlantCount Obj = new PlantCount();
            Obj.EastPlant = EastPlant;
            Obj.WestPlant = WestPlant;


            return Json(Obj, JsonRequestBehavior.AllowGet);

        }
        public class PlantCount
        {
            public int EastPlant { get; set; }
            public int WestPlant { get; set; }
        }

        // basend on IDL & dl
        public ActionResult getEmjoba()
        {
            var data = from e in db.Employees select e;
            int IDL = data.Where(x => x.EMSTAT == "Active" && x.EMPAYT == null && x.EMJOBA == "C1" || x.EMJOBA == "C2" || x.EMJOBA == "P1" || x.EMJOBA == "P2").Count();
            int DL = data.Where(x => x.EMSTAT == "Active" && x.EMPAYT == null && x.EMJOBA == "C0" || x.EMJOBA == "P0" && x.EMSTAT == "Active" && x.EMTERM == null).Count();
            CountEMJOBA Obj = new CountEMJOBA();
            Obj.IDL = IDL;
            Obj.DL = DL;

            return Json(Obj, JsonRequestBehavior.AllowGet);

        }
        public class CountEMJOBA
        {
            public int IDL { get; set; }
            public int DL { get; set; }
        }

        // BASED ON GENDER
        public ActionResult getGender()
        {
            var data = from e in db.Employees select e;
            int Male = data.Where(x => x.EMSTAT == "Active" && x.EMTERM == null && x.EMSEXT == "M").Count();
            int Female = data.Where(x => x.EMSTAT == "Active" && x.EMTERM == null && x.EMSEXT == "F").Count();
            CountGender Obj = new CountGender();
            Obj.Male = Male;
            Obj.Female = Female;
            return Json(Obj, JsonRequestBehavior.AllowGet);

        }
        public class CountGender
        {
            public int Male { get; set; }
            public int Female { get; set; }
        }


        /* public class countBySection()
        {
            public
        } */
        // PERMANENT CONTRACT
        public ActionResult getStatuEmployee()
        {
            var data = from e in db.Employees select e;
            int permanent = data.Where(x => x.EMSTAT == "Active" && x.EMPAYT == "D" && x.EMEMST == "P").Count();
            int contract = data.Where(x => x.EMSTAT == "Active" && x.EMPAYT == "D" && x.EMEMST == "C").Count();
            countStatusEmployee Obj = new countStatusEmployee();
            Obj.permanent = permanent;
            Obj.contract = contract;
            return Json(Obj, JsonRequestBehavior.AllowGet);
        }
        public class countStatusEmployee
        {
            public int permanent { get; set; }
            public int contract { get; set; }
        }
        public class countLenghtOfServices
        {
            public int fiveYears { get; set; }
            public int teenYears { get; set; }
            public int fifTeenYears { get; set; }
            public int twentyYears { get; set; }
        }

        // BASED ON YOS
        public ActionResult getCountTotalNewHire()
        {
            // question how to substring current value join date emplpoyee
            // the problem is the lenght of the employee data is diffrent
            // there is a 20,21,22
            // example format is MM/DD/YYYY
            // 1/DD/YYYY
            // 10/DD/YYYY

            //new hire
            var employee = from e in db.Employees select e;

            var data = employee.Where(x => x.EMCOMM.Substring(4, 4) == "2022" || x.EMCOMM.Substring(5, 4) == "2022" || x.EMCOMM.Substring(6, 4) == "2022").ToList();
            int jan = data.Where(x => x.EMCOMM.Substring(0, 2) == "1/").Count();
            int feb = data.Where(x => x.EMCOMM.Substring(0, 1) == "2").Count();
            int mar = data.Where(x => x.EMCOMM.Substring(0, 1) == "3").Count();
            int apr = data.Where(x => x.EMCOMM.Substring(0, 1) == "4").Count();
            int mei = data.Where(x => x.EMCOMM.Substring(0, 1) == "5").Count();
            int jun = data.Where(x => x.EMCOMM.Substring(0, 1) == "6").Count();
            int jul = data.Where(x => x.EMCOMM.Substring(0, 1) == "7").Count();
            int aug = data.Where(x => x.EMCOMM.Substring(0, 1) == "8").Count();
            int sep = data.Where(x => x.EMCOMM.Substring(0, 1) == "9").Count();
            int oct = data.Where(x => x.EMCOMM.Substring(0, 2) == "10").Count();
            int nov = data.Where(x => x.EMCOMM.Substring(0, 2) == "11").Count();
            int dec = data.Where(x => x.EMCOMM.Substring(0, 2) == "12").Count();

            //eoc
            var dataeoc = employee.Where(x => x.EMTERM.Substring(4, 4) == "2022" || x.EMTERM.Substring(5, 4) == "2022" || x.EMTERM.Substring(6, 4) == "2022").ToList();

            int jan1 = dataeoc.Where(x => x.EMTERM.Substring(0, 2) == "1/").Count();
            int feb1 = dataeoc.Where(x => x.EMTERM.Substring(0, 1) == "2/").Count();
            int mar1 = dataeoc.Where(x => x.EMTERM.Substring(0, 1) == "3").Count();
            int apr1 = dataeoc.Where(x => x.EMTERM.Substring(0, 1) == "4").Count();
            int mei1 = dataeoc.Where(x => x.EMTERM.Substring(0, 1) == "5").Count();
            int jun1 = dataeoc.Where(x => x.EMTERM.Substring(0, 1) == "6").Count();
            int jul1 = dataeoc.Where(x => x.EMTERM.Substring(0, 1) == "7").Count();
            int aug1 = dataeoc.Where(x => x.EMTERM.Substring(0, 1) == "8").Count();
            int sep1 = dataeoc.Where(x => x.EMTERM.Substring(0, 1) == "9").Count();
            int oct1 = dataeoc.Where(x => x.EMTERM.Substring(0, 2) == "10").Count();
            int nov1 = dataeoc.Where(x => x.EMTERM.Substring(0, 2) == "11").Count();
            int dec1 = dataeoc.Where(x => x.EMTERM.Substring(0, 2) == "12").Count();

            countMonth Obj = new countMonth();
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

            //eoc
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
        public class countMonth
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
       
        //EOC
        public ActionResult getCountTotalEOC()
        {
            // question how to substring current value join date emplpoyee
            // the problem is the lenght of the employee data is diffrent
            // there is a 20,21,22
            // example format is MM/DD/YYYY
            // 1/DD/YYYY
            // 10/DD/YYYY
            var data = from e in db.Employees select e;
            int jan = data.Where(x => x.EMTERM.Substring(0, 2) == "1/" && x.EMCOMM.Substring(5, 4) == "2021" || x.EMCOMM.Substring(6, 4) == "2021").Count();

            int feb = data.Where(x => x.EMTERM.Substring(0, 1) == "2/").Count();
            int mar = data.Where(x => x.EMTERM.Substring(0, 1) == "3").Count();
            int apr = data.Where(x => x.EMTERM.Substring(0, 1) == "4").Count();
            int mei = data.Where(x => x.EMTERM.Substring(0, 1) == "5").Count();
            int jun = data.Where(x => x.EMTERM.Substring(0, 1) == "6").Count();
            int jul = data.Where(x => x.EMTERM.Substring(0, 1) == "7").Count();
            int aug = data.Where(x => x.EMTERM.Substring(0, 1) == "8").Count();
            int sep = data.Where(x => x.EMTERM.Substring(0, 1) == "9").Count();
            int oct = data.Where(x => x.EMTERM.Substring(0, 2) == "10").Count();
            int nov = data.Where(x => x.EMTERM.Substring(0, 2) == "11").Count();
            int dec = data.Where(x => x.EMTERM.Substring(0, 2) == "12").Count();

            countMonthEOC Obj = new countMonthEOC();
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



            return Json(Obj, JsonRequestBehavior.AllowGet);
        }
        public class countMonthEOC
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
        }

        //BASE ON AGE
        public ActionResult getAge()
        {
            db.Database.ExecuteSqlCommand("DELETE Report_UmurEmployee");
            db.SaveChanges();
            db.Database.ExecuteSqlCommand("INSERT INTO Report_UmurEmployee Select * From Vw_AgeEmployee");
            db.SaveChanges();

            var data = from e in db.Report_UmurEmployee select e;
            
            int one = data.Where(x => x.umur >= 18 && x.umur <= 25).Sum(x => (int)x.total);
            int two = data.Where(x => x.umur >= 26 && x.umur <= 30).Sum(x => (int)x.total);
            int tree = data.Where(x => x.umur >= 31 && x.umur <= 35).Sum(x => (int)x.total);
            int four = data.Where(x => x.umur >= 36 && x.umur <= 40).Sum(x => (int)x.total);
            int five = data.Where(x => x.umur >= 41 && x.umur <= 45).Sum(x => (int)x.total);
            int six = data.Where(x => x.umur >= 46 && x.umur <= 50).Sum(x => (int)x.total);
            int seven = data.Where(x => x.umur >= 51 && x.umur <= 55).Sum(x => (int)x.total);
            int eight = data.Where(x => x.umur >= 56).Sum(x => (int)x.total);
            CountAge Obj = new CountAge();
            Obj.one = one;
            Obj.two = two;
            Obj.tree = tree;
            Obj.four = four;
            Obj.five = five;
            Obj.six = six;
            Obj.seven = seven;
            Obj.eight = eight;

            return Json(Obj, JsonRequestBehavior.AllowGet);

        }
        public class CountAge
        {
            //19-25
            public int one { get; set; }
            //26-30
            public int two { get; set; }
            //31-35
            public int tree { get; set; }
            //36-40
            public int four { get; set; }
            //41-45
            public int five { get; set; }
            //46-50
            public int six { get; set; }
            //51-55
            public int seven { get; set; }
            //<56
            public int eight { get; set; }
        }

        public ActionResult getEdu()
        {
            var data = from e in db.Employees select e;

            int SD = data.Where(x => x.EMSTAT != "" && x.EMBEDU == "SD" || x.EMBEDU == "3").Count();
            int SMP = data.Where(x => x.EMSTAT != "" && x.EMBEDU == "SMP").Count();
            int SMU = data.Where(x => x.EMSTAT != "" && x.EMBEDU == "SMA" || x.EMBEDU == "SMK").Count();
            int D3 = data.Where(x => x.EMSTAT != "" && x.EMBEDU == "D1" || x.EMBEDU == "D2" || x.EMBEDU == "D3" || x.EMBEDU == " D1" || x.EMBEDU == " D2" || x.EMBEDU == " D3").Count();
            int S1 = data.Where(x => x.EMSTAT != "" && x.EMBEDU == "S1" || x.EMBEDU == " S1" || x.EMBEDU == "S1-" || x.EMBEDU == "S-1").Count();
            int S2 = data.Where(x => x.EMSTAT != "" && x.EMBEDU == "S2" || x.EMBEDU == "S-2" || x.EMBEDU == " S2").Count();
            int Null = data.Where(x => x.EMSTAT != "" && x.EMBEDU == "" || x.EMBEDU == null).Count();

            CountEdu Obj = new CountEdu();
            Obj.SD = SD;
            Obj.SMP = SMP;
            Obj.SMU = SMU;
            Obj.D3 = D3;
            Obj.S1 = S1;
            Obj.S2 = S2;
            Obj.Null = Null;

            return Json(Obj, JsonRequestBehavior.AllowGet);

        }
        public class CountEdu
        {
            public int SD { get; set; }
            public int SMP { get; set; }
            public int SMU { get; set; }
            public int D3 { get; set; }
            public int S1 { get; set; }
            public int S2 { get; set; }
            public int Null { get; set; }
        }

        //internship
        public ActionResult getInternship()
        {
            var data = from e in db.Intern_MasterData select e;
            int InternActive = data.Where(x => x.STATUS == "Active").Count();
            int InternEoc = data.Where(x => x.STATUS == "NonActive").Count();
            InternCount Obj = new InternCount();
            Obj.InternActive = InternActive;
            Obj.InternEoc = InternEoc;           


            return Json(Obj, JsonRequestBehavior.AllowGet);

        }
        public class InternCount
        {
            public int InternActive { get; set; }
            public int InternEoc { get; set; }
        }

        // GET: BussinessIntelligence
        public ActionResult Index()
        {
            //var employees = db.Employees.Include(e => e.Departement);
            return View();
        }

        // GET: BussinessIntelligence/Details/5
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

        // GET: BussinessIntelligence/Create
        public ActionResult Create()
        {
            ViewBag.EMDEPT = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME");
            return View();
        }

        // POST: BussinessIntelligence/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EMCMPY,EMEMP,EMNAME,EMSEXT,EMCOMM,ENDCONT,OLDOCONT,EMEMST,EMJOBA,EMDEPT,EMLOC,EMJOBD,EMSUP_,SUPNAME,EMBIRT,EMMRST,EMRELI,EMGRAD,EMHTLC,EMBUSR,EMBUSS,EMBUSL,EMASN,EMWKHR,EMITST,EMBEDU,EMPAYT,EMADD1,EMADD2,EMADD3,EMEMAD,EMUNIO,EMUSID,EMITAX,EMSTAT,EMTERM,EMTREF,PHONE,EMAIL,TOTALHOURS,TOTALYEARS,DETAILWORKEXPERIENCE,FIRST_NAME,LAST_NAME,PSID,BUS_ROUTE,EOC,LABEL_1,LABEL_2,LABEL_3,BPJS,BPJS_Ketenagakerjaan,EKTP")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EMDEPT = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME", employee.EMDEPT);
            return View(employee);
        }

        // GET: BussinessIntelligence/Edit/5
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
            ViewBag.EMDEPT = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME", employee.EMDEPT);
            return View(employee);
        }

        // POST: BussinessIntelligence/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EMCMPY,EMEMP,EMNAME,EMSEXT,EMCOMM,ENDCONT,OLDOCONT,EMEMST,EMJOBA,EMDEPT,EMLOC,EMJOBD,EMSUP_,SUPNAME,EMBIRT,EMMRST,EMRELI,EMGRAD,EMHTLC,EMBUSR,EMBUSS,EMBUSL,EMASN,EMWKHR,EMITST,EMBEDU,EMPAYT,EMADD1,EMADD2,EMADD3,EMEMAD,EMUNIO,EMUSID,EMITAX,EMSTAT,EMTERM,EMTREF,PHONE,EMAIL,TOTALHOURS,TOTALYEARS,DETAILWORKEXPERIENCE,FIRST_NAME,LAST_NAME,PSID,BUS_ROUTE,EOC,LABEL_1,LABEL_2,LABEL_3,BPJS,BPJS_Ketenagakerjaan,EKTP")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EMDEPT = new SelectList(db.Departements, "DEPT_CODE", "DEPT_NAME", employee.EMDEPT);
            return View(employee);
        }

        // GET: BussinessIntelligence/Delete/5
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

        // POST: BussinessIntelligence/Delete/5
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
