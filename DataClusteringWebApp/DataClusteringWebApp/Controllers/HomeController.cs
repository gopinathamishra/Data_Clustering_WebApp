using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataClusteringWebApp.DataAccessLayer;
using DataClusteringWebApp.Models;
using DataClusteringWebApp.LogicClasses;

namespace DataClusteringWebApp.Controllers
{
    public class HomeController : Controller
    {
        DataClusteringContext db = new DataClusteringContext();
        ClusteringLogic cl = new ClusteringLogic();

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

        public ActionResult TweetClusters()
        {

            return View();
        }

        [HttpGet]
        public JsonResult getTweetClusters()
        {
            List<TokenizedTweet> test = cl.clusteringLogicMain(3);
            return Json(test.ToList(),JsonRequestBehavior.AllowGet);
        }
        

       
    }
}