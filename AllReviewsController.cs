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
           string ASIN = id;
            /*
            string ItemReview = (from rev in db.ReviewLog
                                 where rev.ASIN.Equals(ASIN)
                                 select rev.ASIN).First();

            List<ReviewLog> Logs = (from log in db.ReviewLog
                                    where log.ASIN.Equals(ASIN) && log.CustomerReviewed == true
                                    select log).ToList();

            List<Deal> Deals = db.Deal.ToList();





            return PartialView(Logs);
            
            List<ReviewLogViewModel> ItemsToReview = new List<ReviewLogViewModel>();
            
            for (var i = 0; i < Logs.Count; i++)
            {
                var vm = new ReviewLogViewModel(Deals.Where(a => a.ASIN == Logs[i].ASIN).FirstOrDefault(), Logs[i]);
                ItemsToReview.Add(vm);
            }
           */
        
           
        {
            //ReviewLog Review = db.ReviewLog.Find(id);
                // List<ReviewLog> Logs = db.ReviewLog.ToList();
                List<ReviewLog> Logs = (from log in db.ReviewLog
                                        where log.ASIN.Equals(ASIN) && log.CustomerReviewed == true
                                        select log).ToList();

                return PartialView(Logs);
        }
    }
    
    
    }

}