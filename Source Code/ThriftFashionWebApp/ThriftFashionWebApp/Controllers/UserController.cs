using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ThriftFashionWebApp.Models;

namespace ThriftFashionWebApp.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(tbl_user a)
        {
            using (var data = new thriftsfashionEntities())
            {
                bool iscontain = data.tbl_user.Any(x => x.u_name == a.u_name && x.u_password == a.u_password);
                if (iscontain)
                {
                    tbl_user b = 
                    data.tbl_user.Where(x => x.u_name == a.u_name && x.u_password == a.u_password).SingleOrDefault();

                    a = b;
                    FormsAuthentication.SetAuthCookie(a.u_name, false);
                    Session["u_id"] = a.U_id;
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "invalid user or password");
                    return View();
                }

            }

        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(tbl_user a)
        {
            using (var data = new thriftsfashionEntities())
            {
                if (string.IsNullOrEmpty(a.u_contact))
                    a.u_contact = string.Empty;

                data.tbl_user.Add(a);
                data.SaveChanges();
            }
            return RedirectToAction("SignIn");
        }
        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn");
        }
    }
}