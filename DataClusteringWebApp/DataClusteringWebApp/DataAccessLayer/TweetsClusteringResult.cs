using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataClusteringWebApp.DataAccessLayer
{
    public class TweetsClusters
    {

        public int TweetClusterID { get; set; }
        public long CentroidID { get; set; }
 //       public List<TokenizedTweet> clusterTweets = new List<TokenizedTweet>();
    }

    public class TokenizedTweet
    {
        public long TokenizedTweetsID { get; set; }
        public long TTReferenceNumber { get; set; }
        public HashSet<string> TokenizedContent { get; set; }
        public int TweetClusterID { get; set; }

  //      public virtual TweetsClusters TweetsClusters { get; set; }

    }
    
}