using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using Blue_Ribbon.ViewModels;
using System.Web.Configuration;
using Blue_Ribbon.AmazonAPI;

namespace Blue_Ribbon.Controllers
{
    [Authorize(Roles = "campaignManager")]
    public class ReviewController : Controller
    {
        private BRContext db = new BRContext();

        // GET: All OPEN reviews. If a review is closed, we don't need to modify it.
        public ActionResult Index(bool showAll = false, string currentSearch = null, bool overdue = false)
        {
            var reviews = from r in db.Reviews
                          where r.Reviewed.Equals(false)
                         select r;

            //Filtering items if there is a search query.
            if (!String.IsNullOrEmpty(currentSearch))
            {
                reviews = reviews.Where(s => (s.Customer.FirstName.ToUpper() + " " + s.Customer.LastName.ToUpper()).Contains(currentSearch.ToUpper()) ||
                s.Customer.Email.ToUpper().Contains(currentSearch.ToUpper()) || s.CustomerID.ToUpper().Contains(currentSearch.ToUpper()) ||
                s.Campaign.Name.ToUpper().Contains(currentSearch.ToUpper()) || s.CustomerAlert.ToUpper().Contains(currentSearch.ToUpper()) );
            }

            //Filtering for overdue reviews only
            if (overdue)
            {
                int daysToDueDate = int.Parse(WebConfigurationManager.AppSettings["reviewDeadlineDays"]);

                reviews = reviews.Where(r => DbFunctions.DiffDays(r.SelectedDate, DateTime.Now) > daysToDueDate);
            }

            //Sort: reviews with issues first, then by oldest oustanding review sfirst. 
            reviews = reviews.OrderByDescending(x => x.CustomerAlert).ThenBy(x => x.SelectedDate);

            //We'll collect only the top 50 responses to help load times. But user can request all records.
            if (!showAll)
            {
                reviews = reviews.Take(50);
            }


            var reviewsCheckedTime = (from t in db.TaskLog
                                      where t.TaskDescription.Equals("AllReviewsChecked")
                                      orderby t.SuccessDatestamp descending
                                      select t.SuccessDatestamp).First();
            ViewBag.DateStamp = reviewsCheckedTime;

            return View(reviews.ToList());
        }

        // GET: Review/Details/5
        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        public ActionResult CloseConfirmed(int id)
        {

            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }

            review.Reviewed = true;
            review.ReviewDate = DateTime.Now;
            db.Entry(review).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("Index");
        }

    

        // GET: Review/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Review/Delete/5
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ManualChecks()
        {
            var revs = from r in db.Reviews
                       where r.Reviewed.Equals(false)
                       where r.ManualReviewRequested.Equals(true)
                       select r;

            return View(revs.ToList());
        }

        [HttpPost]
        public JsonResult ManualCheckAction(int reviewid, string action)
        {

            if (String.IsNullOrEmpty(action))
            {
                var dataFail = new
                {
                    message = action
                };

                return Json(dataFail);
            }

            ManualReviewAction checkAction = new ManualReviewAction(reviewid, action);

            var data = new
            {
                message = checkAction.result
            };

            return Json(data);
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
