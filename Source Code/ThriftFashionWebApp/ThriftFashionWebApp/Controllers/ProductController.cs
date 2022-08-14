using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThriftFashionWebApp.Models;
using System.Data;
using System.Web.UI;
using System.IO;

namespace ThriftFashionWebApp.Controllers
{
    public class ProductController : Controller
    {
        thriftsfashionEntities db = new thriftsfashionEntities();
        // GET: Product
        
        public ActionResult Index()
        {
            //Session["u_id"] = 2;
            if (TempData["cart"] != null)
            {
                float x = 0;
                List<cart> li2 = TempData["cart"] as List<cart>;
                foreach (var item in li2)
                {
                    x += item.bill;

                }

                TempData["total"] = x;
            }
            TempData.Keep();
            return View(db.tbl_product.OrderByDescending(x => x.pro_id).ToList());
        }

        public ActionResult addproduct()
        {
            var res = db.tbl_product.ToList();
            ViewBag.cat = res;
            return View(res);
            
        }
        [HttpPost]
        public ActionResult addproduct(int pr_id, string name , int price ,string disc , HttpPostedFileBase image)
        {
            Random r1 = new Random();
            int e = r1.Next();
            string fname = Path.GetFileName(image.FileName);

            string p = Path.Combine(Server.MapPath("~/Content/Images"), e + fname);
            image.SaveAs(p);
            string db_path = "~/Content/Image" + e + fname;

            tbl_product i = new tbl_product();
            i.pro_id = pr_id;
            i.pro_name = name;
            i.pro_price = price;
            i.pro_desc = disc;
            i.pro_image = db_path;

            db.tbl_product.Add(i);
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public ActionResult checkout()
        {
            TempData.Keep();


            return View();
        }
        List<cart> li = new List<cart>();
        [HttpPost]

        public ActionResult checkout(tbl_order o)
        {

            List<cart> li = TempData["cart"] as List<cart>;
            tbl_invoice inv = new tbl_invoice();
            //inv.in_id = Convert.ToInt32(Session["u_id"].ToString());
            inv.in_date = System.DateTime.Now;
            inv.u_totalbill = (float)(TempData["total"]);
            db.tbl_invoice.Add(inv);
            db.SaveChanges();
            foreach (var item in li)
            {
                tbl_order od = new tbl_order();
                od.o_fk_pro = item.productid;
                od.o_fk_name = inv.in_id;
                od.o_date = System.DateTime.Now;
                od.o_qty = item.qty;
                od.o_unitprice = (int)item.price;
                od.o_bill = item.bill;
                db.tbl_order.Add(od);
                db.SaveChanges();


            }
            TempData.Remove("total");
            TempData.Remove("cart");
            TempData["msg"] = "Transaction has been completed";
            TempData.Keep();
            return RedirectToAction("Index");
        }
        public ActionResult Remove(int? id)
        {

            li = TempData["cart"] as List<cart>;
            cart c = li.Where(x => x.productid == id).SingleOrDefault();
            li.Remove(c);

            float h = 0;
            foreach (var item in li)
            {
                h += item.bill;

            }
            TempData["total"] = h;
            TempData.Keep();
            return RedirectToAction("checkout");

        }




    }
}