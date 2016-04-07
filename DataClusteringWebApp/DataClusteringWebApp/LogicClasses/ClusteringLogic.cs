using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataClusteringWebApp.DataAccessLayer;
using DataClusteringWebApp.Models;
using System.Text.RegularExpressions;



namespace DataClusteringWebApp.LogicClasses
{
    public class ClusteringLogic
    {
       
        DataClusteringContext dbContext = new DataClusteringContext();
        List<TweetsClusters> tweetClusters = new List<TweetsClusters>();
        List<TokenizedTweet> tokenizedTweets = new List<TokenizedTweet>();
        List<Tweet> originalTweets = new List<Tweet>();

        /// <summary>
        /// Removes the url links and unecessary characters from the tweet
        /// </summary>
        /// <param name="tweetContent"></param>
        /// <returns> a string containing tokens (only alpha numeric characterd ) words </returns>
        public string removeExtras(string tweetContent)
        {
            // pattern to replace redefined links in tweets with nothing
            string pattern = @"(http\:\/{2}t\.co\/(\d+|\w+|\W+)+)";
            string replacement = "";
            string input = tweetContent;
            string result = Regex.Replace(input, pattern, replacement);

            //pattern to replace any non - word literals -> all special characters with a space
            pattern = @"\W";
            replacement = " ";
            result = Regex.Replace(result, pattern, replacement);
            return result;
        }

        /// <summary>
        /// Tokenizes the tweetContent and stores them in the model class object- 
        /// tokenized tweets
        /// </summary>
        /// <returns></returns>
        public bool cleanUpTweets()
        {
            bool success = true;
            string tweetContent;
            //a hastset to store distinct words in a tweet
            HashSet<string> dictionaryTokens = new HashSet<string>();
            TokenizedTweet singleTokenizedTweet = new TokenizedTweet();
            char[] delimiter = { ' ' };
            foreach(var tweet in originalTweets)
            {
                //Calls the a method to remove unwanted characters
                tweetContent = removeExtras(tweet.tweetContent);
                dictionaryTokens = new HashSet<string>();
                //splits the string into a list of words with "space" as a delimiter
                string[] tweetTokens = tweetContent.Split(delimiter);
                foreach (var t in tweetTokens)
                {
                    if (!t.Equals(""))
                    {
                        dictionaryTokens.Add(t);
                    }
                }
                singleTokenizedTweet = new TokenizedTweet();
                singleTokenizedTweet.TweetClusterID = 0;
                singleTokenizedTweet.TokenizedContent = dictionaryTokens;
                singleTokenizedTweet.TokenizedTweetsID = tweet.tweetID;
                singleTokenizedTweet.TTReferenceNumber = tweet.tweetReferenceNumber;
                tokenizedTweets.Add(singleTokenizedTweet);
            }
          return success;
        }

        /// <summary>
        /// Compares two tweets and returns their dissimilarity value ( i.e. their jaccard Distance)
        /// </summary>
        /// <param name="t1">tweet 1</param>
        /// <param name="t2">tweet 2</param>
        /// <returns></returns>
        public float calculateJaccardDistance(TokenizedTweet t1, TokenizedTweet t2)
        {
            float jaccardDistance = 0;
            int similarWordsCount = 0, tweetOneWordCount = 0, tweetTwoWordCount = 0, distinctWordsCount = 0;
            tweetOneWordCount = t1.TokenizedContent.Count;
            tweetTwoWordCount = t2.TokenizedContent.Count;
            foreach(var tweetOneWord in t1.TokenizedContent)
            {
                foreach(var tweetTwoWord in t2.TokenizedContent)
                {
                    if(tweetOneWord.Equals(tweetTwoWord))
                    {
                        similarWordsCount++;
                    }
                }
            }
            distinctWordsCount = (tweetOneWordCount + tweetTwoWordCount) - similarWordsCount;
            jaccardDistance = 1 - (similarWordsCount / distinctWordsCount);
            return jaccardDistance;
        }

        /// <summary>
        /// Returns the closest cluster type for a given tweet based on 
        /// The dissimilarity(Jaccard) meassure between the tweet and the cluster centroids
        /// </summary>
        /// <param name="tokenizedTweet"></param>
        /// <returns></returns>
        public int selectNearestCluster(TokenizedTweet tokenizedTweet)
        {
            int clusterId = 0;
            float smallestDistance = 1000;
            float disSimilarity = 0;
            TokenizedTweet clusterCentroid;
            foreach(var cluster in tweetClusters)
            {
                clusterCentroid = new TokenizedTweet();
                clusterCentroid = tokenizedTweets.First(x => x.TokenizedTweetsID == cluster.CentroidID);
                disSimilarity = calculateJaccardDistance(tokenizedTweet, clusterCentroid);
                if(disSimilarity < smallestDistance)
                {
                    clusterId = cluster.TweetClusterID;
                    smallestDistance = disSimilarity;
                }
           }
            return clusterId;
        }

        /// <summary>
        /// Randomly picks the centroids and creates clusters initially with zero objects in them
        /// </summary>
        /// <param name="numberOfClusters"></param>
        /// <returns></returns>
        public bool selectClusterCentroids(int numberOfClusters)
        {
            Random selectCLuster = new Random();
            List<int> clusterIdList = new List<int>();
            int count = 1;
            TweetsClusters clusterCentroid = new TweetsClusters();
            clusterCentroid.TweetClusterID = 1;
            clusterCentroid.CentroidID = selectCLuster.Next(1, 251);
            tweetClusters.Add(clusterCentroid);
            count++;
            while (count <= numberOfClusters)
            {

                clusterCentroid = new TweetsClusters();
                clusterCentroid.TweetClusterID = count;
                clusterCentroid.CentroidID = selectCLuster.Next(1, 251);
                if (tweetClusters.Find(x => x.CentroidID == clusterCentroid.CentroidID) == null)
                {
                    tweetClusters.Add(clusterCentroid);
                    count++;
                }
            }
            return true;
        }

        /// <summary>
        /// Collects all the objects of a cluster and finds an object among them, whose distance
        /// correponding to remaining objects is less, when compared to any other object in the cluster.
        /// This object is assigned as a new centroid for the cluster
        /// </summary>
        public void reCalculateClusterCentroids()
        {
            foreach(var cluster in tweetClusters)
            {
                List<TokenizedTweet> currentClusterObjects = tokenizedTweets.Where(x => x.TweetClusterID == cluster.TweetClusterID).ToList();
                List<TokenizedTweet> iterateCurrentObjects = tokenizedTweets.Where(x => x.TweetClusterID == cluster.TweetClusterID).ToList();
                float distance = 0;
                float smallestDistance = 1000000;
                long centroidID = cluster.CentroidID;
                foreach (var clusterObject in currentClusterObjects)
                {
                    distance = 0;
                    
                    foreach(var iterator in iterateCurrentObjects)
                    {
                        distance = distance + calculateJaccardDistance(clusterObject, iterator);
                    }
                    if (smallestDistance > distance)
                    {
                        smallestDistance = distance;
                        centroidID = clusterObject.TokenizedTweetsID;
                    }
                }
                cluster.CentroidID = centroidID;
            }
        }

        /// <summary>
        /// Finds the sumOfSquaresError of each cluster and the for the whole K-means implementation
        /// It is the sum of - the difference betweet the cluster to object distance and the mean cluster to 
        /// object distance
        /// </summary>
        /// <returns></returns>
        public float sumOfSquaresError()
        {
            float SSE = 0, clusterMean = 0;
            int numberOfObjects = 0;
            foreach(var cluster in tweetClusters)
            {
                clusterMean = 0;
                List<TokenizedTweet> clusterObjectList = tokenizedTweets.Where(x => x.TweetClusterID == cluster.TweetClusterID).ToList();
                var centroid = tokenizedTweets.First(x => x.TokenizedTweetsID == cluster.CentroidID);
                //find 
                foreach(var clusterObject in clusterObjectList)
                {
                    clusterMean = clusterMean + calculateJaccardDistance(centroid, clusterObject);
                }
                numberOfObjects = clusterObjectList.Count;
                clusterMean = clusterMean / numberOfObjects;
                //Calculate sum of squares error
                foreach(var clusterObject in clusterObjectList)
                {
                    SSE = SSE + (calculateJaccardDistance(centroid, clusterObject) - clusterMean);
                }
            }
            return SSE;
        }

        /// <summary>
        /// Creates clusters and assigns the tweet object to them based on similarity value
        /// New centroids are recalculated and compared with old ones. 
        /// If there is a change - the tweet objects are reassigned to the clusters
        /// This looping procedure continues until the centroids constant - do not change for the new
        /// reassigned cluster objects.
        /// </summary>
        /// <param name="numberOfClusters"></param>
        /// <returns> a list of tweet objects with information about their cluster</returns>
        public List<TokenizedTweet> clusteringLogicMain(int numberOfClusters)
        {
            numberOfClusters = 25;
            originalTweets = dbContext.Tweets.ToList();
            selectClusterCentroids(numberOfClusters);
            // Remove unwanted characters and tokenize the tweet string, store them in a model object list
            bool clean = cleanUpTweets();
            if(clean)
            {
                //assigns the cluster based on the similarity - for each tweet
                foreach(var tweet in tokenizedTweets)
                {
                    var isAclusterCentroid = tweetClusters.FirstOrDefault(x => x.CentroidID == tweet.TokenizedTweetsID);
                    if( isAclusterCentroid == null)
                    {
                        int clusterId = selectNearestCluster(tweet);
                        tweet.TweetClusterID = clusterId;
                    }
                    else
                    {
                        tweet.TweetClusterID = tweetClusters.First(x => x.CentroidID == tweet.TokenizedTweetsID).TweetClusterID;
                    }
                }
            }

            List<long> oldCentroidIds = new List<long>();
            List<long> newCentroidIds = new List<long>();
            //collects alist of centroid id's
            foreach (var cluster in tweetClusters)
            {
                oldCentroidIds.Add(cluster.CentroidID);
            }

            // after all the tweets are assigned to a cluster, the centroid is recalculated
            reCalculateClusterCentroids();

            //collects a list of new centroid id's
            foreach (var cluster in tweetClusters)
            {
                newCentroidIds.Add(cluster.CentroidID);
            }

            bool centroidsHaveChanged = false;
            // compares old centroids with the new ones
            for (int iterator = 0; iterator < numberOfClusters; iterator++)
            {
                if (oldCentroidIds[iterator] != newCentroidIds[iterator])
                {
                    centroidsHaveChanged = true;
                }
            }

            // if the centroids have changed, the tweet objects are re-assigned, 
            // new centroids are calculated and compared again with previous centroids
            while (centroidsHaveChanged)
            {

                oldCentroidIds = newCentroidIds;
                // reassigns the cluster objects to new clusters based on similarity value
                foreach (var tweet in tokenizedTweets)
                {
                    int clusterId = selectNearestCluster(tweet);
                    tweet.TweetClusterID = clusterId;
                }
                // the centroids are recalculated
                reCalculateClusterCentroids();

                newCentroidIds = new List<long>();
                foreach (var cluster in tweetClusters)
                {
                    newCentroidIds.Add(cluster.CentroidID);
                }

                // new centroids are compared with previous centroids. 
                // and if there is no change the flag value is set to false and the loop halts
                for (int iterator = 0; iterator < numberOfClusters; iterator++)
                {
                    if (oldCentroidIds[iterator] != newCentroidIds[iterator])
                    {
                        centroidsHaveChanged = true;
                        break;
                    }
                    centroidsHaveChanged = false;
                }
            }

            float SSE = sumOfSquaresError();
            return tokenizedTweets;
        }      
    }
}