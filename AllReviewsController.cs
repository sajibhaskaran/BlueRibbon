using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue_Ribbon.DAL;

namespace Blue_Ribbon.Controllers
{
    public class AllReviewsController : Controller
    {
        private BRContext db = new BRContext();
        // GET: AllReviews
        public ActionResult Index(string id)
        {

            // Making a list of the reviews associated with the product
            // Pulling all with the given ID


            List<ReviewLog> Reviews = (from log in db.ReviewLog
                                       where log.ASIN.Equals(id) && log.CustomerReviewed == true
                                       select log).ToList();


            return View(Reviews);

        }

        public new ActionResult PartialView(string id)
        {
            // Making a list of the reviews associated with the product
            // Pulling all with the given ID


            List<ReviewLog> Reviews = (from log in db.ReviewLog
                                       where log.ASIN.Equals(id) && log.CustomerReviewed == true
                                       select log).ToList();

            // Returning a partial view to show in the product Model
            return PartialView(Reviews);

        }
    }

}