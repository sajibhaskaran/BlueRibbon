using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Blue_Ribbon.ViewModels
{
    public class ReviewsViewModel
    {
        public ICollection<Review> Reviews { get; set; }
        public string SellerMessage { get; set; }


        public ReviewsViewModel(bool showAll, string currentSearch, bool overdue, string userId = null)
        {
            IQueryable<Review> revs;

            if (userId != null)
            {
                //If a userId is supplied (and validated below), then all the data populated in this view model will be specific to a seller.
                var getVendor = (from x in db.Vendors
                                 where x.CustomerID.Equals(userId)
                                 select x);

                //If we failed to get the vendorId it's because user is not linked to vendor account. Generate feedback and return to controller.
                if (getVendor.Count() == 0)
                {
                    SellerMessage = "Your user ID is not linked to a seller account. Please contact us to have your seller account linked!";
                    return;
                }

                int vendorId = getVendor.Single().VendorId;

                revs = from r in db.Reviews
                       where r.Campaign.VendorID == vendorId
                       where r.Reviewed == true
                       select r;
            }
            else
            {
                revs = from r in db.Reviews
                       where r.Reviewed.Equals(false)
                       select r;

            }

            //Filtering items if there is a search query.
            if (!String.IsNullOrEmpty(currentSearch))
            {
                revs = revs.Where(s => (s.Customer.FirstName.ToUpper() + " " + s.Customer.LastName.ToUpper()).Contains(currentSearch.ToUpper()) ||
                s.Customer.Email.ToUpper().Contains(currentSearch.ToUpper()) || s.CustomerID.ToUpper().Contains(currentSearch.ToUpper()) ||
                s.Campaign.Name.ToUpper().Contains(currentSearch.ToUpper()) || s.CustomerAlert.ToUpper().Contains(currentSearch.ToUpper()));
            }

            //Filtering for overdue reviews only
            if (overdue)
            {
                int daysToDueDate = int.Parse(WebConfigurationManager.AppSettings["reviewDeadlineDays"]);

                revs = revs.Where(r => DbFunctions.DiffDays(r.SelectedDate, DateTime.Now) > daysToDueDate);
            }

            //Sort: reviews with issues first, then by oldest oustanding review sfirst. 
            revs = revs.OrderByDescending(x => x.CustomerAlert).ThenBy(x => x.SelectedDate);

            //We'll collect only the top 50 responses to help load times. But user can request all records.
            if (!showAll)
            {
                revs = revs.Take(50);
            }

            Reviews = revs.ToList();

        }


        private BRContext db = new BRContext();
    }
}
