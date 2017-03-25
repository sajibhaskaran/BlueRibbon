using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue_Ribbon.DAL;
using Blue_Ribbon.ViewModels;

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

            List<ReviewLog> Logs = (from log in db.ReviewLog
                                    where log.ASIN.Equals(id) && log.CustomerReviewed == true
                                    select log).ToList();

            List<Deal> Deals = db.Deal.ToList();

            List<ReviewLogViewModel> LogsViewModel = new List<ReviewLogViewModel>();
           
            for (var i = 0; i < Logs.Count; i++)
            {
                var vm = new ReviewLogViewModel(Deals.Where(a => a.ASIN == id).FirstOrDefault(), Logs[i]);
                LogsViewModel.Add(vm);
                
            }
            
            return View(LogsViewModel);

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