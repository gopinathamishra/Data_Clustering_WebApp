using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataClusteringWebApp.DataAccessLayer;
using DataClusteringWebApp.Models;

namespace DataClusteringWebApp.Controllers
{
    public class HomeController : Controller
    {
        DataClusteringContext db = new DataClusteringContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
         public JsonResult Tweetsdata()
        {            
            List<Tweet> tweetData = db.Tweets.ToList();
            return Json(tweetData.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayAllTweets()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}