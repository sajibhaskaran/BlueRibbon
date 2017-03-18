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

namespace Blue_Ribbon.Controllers
{
    [Authorize(Roles = "campaignManager")]
    public class ReviewLogController : Controller
    {
        private BRContext db = new BRContext();

        // GET: ReviewLog
        public ActionResult Index()
        {
            List<ReviewLog> Logs = db.ReviewLog.ToList();

            List<Deal> Deals = db.Deal.ToList();

            List<ReviewLogViewModel> LogsViewModel = new List<ReviewLogViewModel>();

            for (var i = 0; i < Logs.Count; i++)
            {
                var vm = new ReviewLogViewModel(Deals.Where(a => a.ASIN == Logs[i].ASIN).FirstOrDefault(), Logs[i]);
                LogsViewModel.Add(vm);
            }
            return View(LogsViewModel);
        }

        // GET: ReviewLog/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Create the review object from id, create deal object where the ASIN/pkey matches
            ReviewLog Review = db.ReviewLog.Find(id);
            Deal Deal = db.Deal.Where(db => db.ASIN == Review.ASIN).First();

            //Create VM to pass to details view
            var ReviewDetailsVM = new ReviewLogViewModel(Deal, Review);


            ReviewDetailsVM.DaysSinceCodeGiven = (DateTime.Now - ReviewDetailsVM.SelectedDate);

            if (Review == null)
            {
                return HttpNotFound();
            }
            return View(ReviewDetailsVM);
        }

        // GET: ReviewLog/Create
        public ActionResult Create()
        {
            return View();
        }






        // POST: ReviewLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Rating")]ReviewLog Review)
        {
            Random random = new Random();
            int rDate = random.Next(3, 30);
            rDate = (rDate * -1);
            bool[] bools = new bool[2];

            //create random bools for would buy again and refer to friend
            for (int i = 0; i < 2; i++)
            {
                bools[i] = new Random().Next(100) % 2 == 0;
            }

            int rRating = random.Next(1, 6);
            if (Review.DateReviewed == null)
            {
                Review.DateReviewed = DateTime.Now.Date;
                Review.SelectedDate = DateTime.Now.Date.AddDays(rDate); //emulate a click to get a deal 3 days ago
            }

            Review.Rating = rRating;
            Review.ASIN = "B01BGVLGFE";
            Review.CustomerReviewed = true;
            Review.DisplayReview = true;
            Review.Email = "tyler.corum@gmail.com";
            Deal Deal = db.Deal.Where(db => db.ASIN == Review.ASIN).First();
            Review.WebsiteAPIId = Deal.WebsiteAPIDataId;
            Review.WouldBuyAgain = bools[0];
            Review.RecToFriend = bools[1];
            //TODO implement error handing
            if (ModelState.IsValid)
            {
                db.ReviewLog.Add(Review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Review);
        }







        // This is a snub of un-working code that I am working into a Unit Test
        //
        // POST: ReviewLog/CreateMock
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMock([Bind(Include = "ReviewLogId,ASIN,WebsiteAPIId,CustomerReviewed,AutomaticValidation,AdminReviewed,DisplayReview,Rating,Email,ReviewSubject,ReviewBody,WouldBuyAgain,RecToFriend")] ReviewLog reviewLog)
        {
            //TODO this is some code to generate a record automaticaly
            //List<Deal> Deals = db.Deal.ToList();
            //List<ReviewLog> Logs = db.ReviewLog.ToList();
            //ReviewLogViewModel CreateMockLog = new ReviewLogViewModel();

            //Random random = new Random();
            //int rNum = random.Next(1,6);

            //var vm = new ReviewLogViewModel(Deals.Where(d.ASIN == "B01BGVLGFE")), Logs);
            if (reviewLog.DateReviewed == null)
            {
                reviewLog.DateReviewed = DateTime.Now.Date;
                reviewLog.SelectedDate = DateTime.Now.Date.AddDays(-3); //emulate a click to get a deal 3 days ago
            }
            if (ModelState.IsValid)
            {

                db.ReviewLog.Add(reviewLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reviewLog);
        }

        // GET: ReviewLog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReviewLog reviewLog = db.ReviewLog.Find(id);
            if (reviewLog == null)
            {
                return HttpNotFound();
            }
            return View(reviewLog);
        }

        // POST: ReviewLog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReviewLogId,ASIN,WebsiteAPIId,CustomerReviewed,AutomaticValidation,AdminReviewed,DisplayReview,Rating,Email,ReviewSubject,ReviewBody,WouldBuyAgain,RecToFriend")] ReviewLog reviewLog)
        {
            if (ModelState.IsValid)
            {
                //Created new value for DateReviewed so every time user reviews
                //DateReviewed is updated to the days date while Selected date is not updated
                reviewLog.DateReviewed = DateTime.Now.Date;
                db.Entry(reviewLog).State = EntityState.Modified;
                db.Entry(reviewLog).Property("SelectedDate").IsModified = false;
               

              

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reviewLog);
        }

        // GET: ReviewLog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReviewLog reviewLog = db.ReviewLog.Find(id);
            if (reviewLog == null)
            {
                return HttpNotFound();
            }
            return View(reviewLog);
        }

        // POST: ReviewLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReviewLog reviewLog = db.ReviewLog.Find(id);
            db.ReviewLog.Remove(reviewLog);
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
