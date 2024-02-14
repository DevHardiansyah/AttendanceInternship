using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRINTERNSHIP.Models;
namespace HRINTERNSHIP.Controllers
{
    public class RoleController : Controller
    {
        private DBModel db = new DBModel();
        // GET: Role
        

        // for generate api data
        public ActionResult GetListAction(string role_id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var rolepermission = from s in db.RolePermissions select s;
            if (role_id != null)
            {
                var rolepermissions = rolepermission.Where(s => s.role_id == role_id).ToList();
                return Json(rolepermissions, JsonRequestBehavior.AllowGet);
            }
            return Json(rolepermission, JsonRequestBehavior.AllowGet);
        }

        // generate data from database 
        public ActionResult getListUsers(string kpk)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var List = from s in db.ListUsers select s;
       
            if (kpk != null)
            {
                var Lists = List.Where(s => s.EMEMP == kpk).ToList();
                  
                return Json(Lists, JsonRequestBehavior.AllowGet);
              
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(string kpk)
        {
            if(kpk != null)
            {
                var data = from i in db.ListUsers select i;
                ViewBag.ListRole = from t in db.Roles select t;
                var result = data.Where(s => s.EMEMP.Equals(kpk)).Take(1);
                return View(result);
            }
            else
            {
                ViewBag.ListUsers = from i in db.ListUsers select i;
                ViewBag.ListAction = from d in db.permissions select d;
                ViewBag.ListRole = from t in db.Roles select t;
                ViewBag.RolePermissions = from g in db.RolePermissions select g;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(string kpk, string role_id)
        {
            if(kpk != null)
            {
                ViewBag.ListUsers = from i in db.ListUsers select i;
                db.Database.ExecuteSqlCommand("Update Users set roles_id = '" + role_id + "' where user_id='" + kpk + "'  ");
                db.SaveChanges();
                TempData["message"]= "success-update-role-user";
                return RedirectToAction("Index");
            }
            ViewBag.ListUsers = from i in db.ListUsers select i;
            ViewBag.ListAction = from d in db.permissions select d;
            ViewBag.ListRole = from t in db.Roles select t;
            ViewBag.RolePermissions = from g in db.RolePermissions select g;
            return RedirectToAction("Index");
        }




        // function fro redirect to role management
        public ActionResult Index()
        {
            if (Session["username"] != null)
            {
                // get list user
                ViewBag.ListUsers = from i in db.ListUsers select i;
                // get permission
                ViewBag.ListAction = from d in db.permissions select d;
                // get role
                ViewBag.ListRole = from t in db.Roles select t;
                // get role permission
                ViewBag.RolePermissions = from g in db.RolePermissions select g;
                var MasterData = from s in db.RoleLists select s;
                return View(MasterData.Take(30));
            }
            return RedirectToAction("Login", "Employee");
        }


























        [HttpPost]
        public ActionResult Addrole(string permission_1, string permission_2, string permission_3, string permission_4, string permission_5, string permission_6, string permission_7, string permission_8, string permission_9, string permission_10, string permission_11, string permission_12, string permission_13, string permission_14, string role_id, string role_name)
        {
            int status = 1;
            int status2 = 0;

            if (role_id != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Roles values('" + role_id + "','" + role_name + "') ");
            }
            else
            {
                return RedirectToAction("Index");
            }

            var unique_value1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_1 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_1 + "','" + permission_1 + "','" + unique_value1 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','1','1','" + unique_value1 + "','" + status2 + "')");
                db.SaveChanges();
            }

            // for permission 2
            var unique_value2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_2 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_2 + "','" + permission_2 + "','" + unique_value2 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','2','2','" + unique_value2 + "','" + status2 + "')");
                db.SaveChanges();
            }
            // for permission 3
            var unique_value3 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_3 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_3 + "','" + permission_3 + "','" + unique_value3 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','3','3','" + unique_value3 + "','" + status2 + "')");
                db.SaveChanges();
            }
            // for permission 4
            var unique_value4 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_4 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_4 + "','" + permission_4 + "','" + unique_value4 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','4','4','" + unique_value4 + "','" + status2 + "')");
                db.SaveChanges();
            }
            // for permisiion 5
            var unique_value5 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_5 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_5 + "','" + permission_5 + "','" + unique_value5 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','5','5','" + unique_value5 + "','" + status2 + "')");
                db.SaveChanges();
            }
            // for permission 6
            var unique_value6 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_6 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_6 + "','" + permission_6 + "','" + unique_value6 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','6','6','" + unique_value6 + "','" + status2 + "')");
                db.SaveChanges();
            }
            // for permission 7
            var unique_value7 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_7 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_7 + "','" + permission_7 + "','" + unique_value7 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','7','7','" + unique_value7 + "','" + status2 + "')");
                db.SaveChanges();
            }
            // for perimssion 8
            var unique_value8 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_8 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_8 + "','" + permission_8 + "','" + unique_value8 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','8','8','" + unique_value8 + "','" + status2 + "')");
                db.SaveChanges();
            }
            // for permission 9
            var unique_value9 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_9 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_9 + "','" + permission_9 + "','" + unique_value9 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','9','9','" + unique_value9 + "','" + status2 + "')");
                db.SaveChanges();
            }
            // for permission 10
            var unique_value10 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_10 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_10 + "','" + permission_10 + "','" + unique_value10 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','10','10','" + unique_value10 + "','" + status2 + "')");
                db.SaveChanges();
            }

            // for permission 11
            var unique_value11 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_11 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_11 + "','" + permission_11 + "','" + unique_value11 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','11','11','" + unique_value11 + "','" + status2 + "')");
                db.SaveChanges();
            }


            // for permission 12
            var unique_value12 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_12 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_12 + "','" + permission_12 + "','" + unique_value12 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','12','12','" + unique_value12 + "','" + status2 + "')");
                db.SaveChanges();
            }


            // for permission 13
            var unique_value13 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_13 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_13 + "','" + permission_13 + "','" + unique_value13 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','13','13','" + unique_value13 + "','" + status2 + "')");
                db.SaveChanges();
            }


            // for permission 14
            var unique_value14 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            if (permission_14 != null)
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','" + permission_14 + "','" + permission_14 + "','" + unique_value14 + "','" + status + "')");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("INSERT INTO Role_permissions VALUES('" + role_id + "','14','14','" + unique_value14 + "','" + status2 + "')");
                db.SaveChanges();
            }

            ViewBag.Success = "sucess-add-role";
            ViewBag.ListAction = from d in db.permissions select d;
            ViewBag.ListRole = from t in db.Roles select t;
            var MasterData = from s in db.RoleLists select s;
            return RedirectToAction("Index", MasterData.Take(30));
        }





        [HttpPost]
        public ActionResult Index(string permission1, string permission12, string permission2, string permission3, string permission4, string permission5, string permission6, string permission7, string permission8, string permission9, string permission10, string permission11, string permission14, string permission13, string role_id)
        {
            int status = 1;
            int status2 = 0;
            // for permission 1
            if (permission1 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission1 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='1'");
                db.SaveChanges();
            }

            // for permission 2
            if (permission2 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission2 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='2'");
                db.SaveChanges();
            }
            // for permission 3
            if (permission3 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission3 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='3'");
                db.SaveChanges();
            }
            // for permission 4
            if (permission4 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission4 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='4'");
                db.SaveChanges();
            }
            // for permisiion 5
            if (permission5 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission5 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='5'");
                db.SaveChanges();
            }
            // for permission 6
            if (permission6 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission6 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='6'");
                db.SaveChanges();
            }
            // for permission 7
            if (permission7 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission7 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='7'");
                db.SaveChanges();
            }
            // for perimssion 8
            if (permission8 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission8 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='8'");
                db.SaveChanges();
            }
            // for permission 9
            if (permission9 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission9 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='9'");
                db.SaveChanges();
            }
            // for permission 10
            if (permission10 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission10 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='10'");
                db.SaveChanges();
            }


            // for permission 11
            if (permission11 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission11 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='11'");
                db.SaveChanges();
            }


            // for permission 12
            if (permission12 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission12 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='12'");
                db.SaveChanges();
            }


            // for permission 13
            if (permission13 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission13 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='13'");
                db.SaveChanges();
            }


            // for permission 14
            if (permission14 != null)
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status + "' where role_id='" + role_id + "' and permission_id='" + permission14 + "'");
                db.SaveChanges();
            }
            else
            {
                db.Database.ExecuteSqlCommand("update Role_permissions set status = '" + status2 + "' where role_id='" + role_id + "' and permission_id='14'");
                db.SaveChanges();
            }

            ViewBag.Success = "sucess-update-role";
            ViewBag.ListAction = from d in db.permissions select d;
            ViewBag.ListRole = from t in db.Roles select t;
            var MasterData = from s in db.RoleLists select s;
            return RedirectToAction("index",MasterData.Take(30));

        }


        [HttpPost]
        public ActionResult DeleteRole(string role_id)
        {

            if (role_id != null)
            {
                db.Database.ExecuteSqlCommand("Delete from roles WHERE  role_id='" + role_id + "' ");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("Delete from Role_permissions WHERE role_id='" + role_id + "' ");
                db.SaveChanges();
                ViewBag.Success = "success-delete-role";
                ViewBag.ListAction = from d in db.permissions select d;
                ViewBag.ListRole = from t in db.Roles select t;
                var MasterData = from s in db.RoleLists select s;
                return RedirectToAction("Index", MasterData.Take(30));
            }
            else
            {
                ViewBag.Success = "failed-delete-role";
                ViewBag.ListAction = from d in db.permissions select d;
                ViewBag.ListRole = from t in db.Roles select t;
                var MasterData = from s in db.RoleLists select s;
                return RedirectToAction("Index", MasterData.Take(30));
            }
           
        }




        
    }
}