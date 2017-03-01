using AjaxLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace AjaxLab.Controllers
{
    public class ProductController : Controller
    {
        NorthwindEntities ctx = new NorthwindEntities();

        /*
        // GET: Product
        public ActionResult Index()
        {
            ViewBag.Message = "Products";

            return View();
        }*/

        private SelectList AddFirstItem(SelectList list)
        {
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "-1", Text = "----ALL----" });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text");
        }

        public ActionResult ProductSearch(string categoryId, string supplierId)
        {
            ViewData["categoryId"] = AddFirstItem(new SelectList(ctx.Categories, "CategoryID", "CategoryName"));
            ViewData["supplierId"] = AddFirstItem(new SelectList(ctx.Suppliers, "SupplierID", "CompanyName"));

            var cid = Convert.ToInt32(categoryId);
            var sid = Convert.ToInt32(supplierId);
            var result = "";
            //var SName = "";

            // if (String.IsNullOrEmpty(category) && String.IsNullOrEmpty(supplier)) {
            var prod = ctx.Products.Include(p => p.Category).Include(p => p.Supplier)
                .OrderBy(c => c.ProductID)
                .ToList();
            //   }

            if (Request.IsAjaxRequest())
            {
                if (cid != -1 && sid != -1)
                {
                    prod = ctx.Products.Include(p => p.Category).Include(p => p.Supplier)
                        .Where(p => p.CategoryID == cid && p.SupplierID == sid)
                        .OrderBy(c => c.ProductID)
                        .ToList();
                    var catogoryName = ctx.Categories.Where(c => c.CategoryID == cid).Select(c => c.CategoryName).First().ToString();
                    var sName = ctx.Suppliers.Where(s => s.SupplierID == sid).Select(s => s.CompanyName).First().ToString();
                    result = "Product with Category = " + catogoryName + " and Supplier = " + sName;
                 }
                else if (cid == -1 && sid != -1)
                {
                    prod = ctx.Products.Include(p => p.Category).Include(p => p.Supplier)
                        .Where(p => p.SupplierID == sid)
                        .OrderBy(c => c.ProductID)
                        .ToList();
                    var sName = ctx.Suppliers.Where(s => s.SupplierID == sid).Select(s => s.CompanyName).First().ToString();
                    result = "Product with Supplier = " + sName;
                }
                else if (cid != -1 && sid == -1)
                {
                    prod = ctx.Products.Include(p => p.Category).Include(p => p.Supplier)
                        .Where(p => p.CategoryID == cid)
                        .OrderBy(c => c.ProductID)
                        .ToList();

                    var cName = ctx.Categories.Where(c => c.CategoryID == cid).Select(c => c.CategoryName).First().ToString();
                    result = "Product with Catogory = " + cName;
                }
                else
                {
                    prod = ctx.Products.Include(p => p.Category).Include(p => p.Supplier)
                    .OrderBy(c => c.ProductID)
                    .ToList();
                    result = "All Products!";
                }
                ViewBag.ResultMsg = result;
                return PartialView("_ProductSearchPartial", prod);
            }


            return View(prod);

        }
    }
}