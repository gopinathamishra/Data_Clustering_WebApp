using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataClusteringWebApp.Models
{
    /// <summary>
    /// Saving only Tweet Id and Tweet Content into the database
    /// Obtained from the Json File
    /// </summary>
    public class Tweet
    {
        public long tweetID { get; set; }
        public long tweetReferenceNumber { get; set;}
        public string tweetContent  { get; set; }
    }
}