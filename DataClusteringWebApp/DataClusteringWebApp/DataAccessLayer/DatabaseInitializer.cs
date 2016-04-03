using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DataClusteringWebApp.Models;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace DataClusteringWebApp.DataAccessLayer
{
    public class DatabaseInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<DataClusteringContext>
    {
        /// <summary>
        /// Seeds the Database with data 
        /// Currently using Json File data to seed the database
        /// </summary>
        /// <param name="context"></param>sss
        protected override void Seed(DataClusteringContext context)
        {
            //loading the context with the json data
            getTweetsDataFromJson(context);
            //saving the context -> saving into database
            context.SaveChanges();
        }


        /// <summary>
        /// Retrieving the content of Tweets from Json File and 
        /// saving it in the currentcontext
        /// </summary>
        /// <param name="context"> Tweet content saved in context</param>
        protected void getTweetsDataFromJson(DataClusteringContext context)
        {
         
            var jsonFileData = getDataFromEmbeddedResource();

            JArray jsonDataObjects = JArray.Parse(jsonFileData) as JArray;
            dynamic dynamicJsonObjects = jsonDataObjects;

            List<Tweet> tweetDataCollection = new List<Tweet>();
            Tweet singleTweet = null;

            foreach ( var jobject in dynamicJsonObjects)
            {
                singleTweet = new Tweet();
                singleTweet.tweetReferenceNumber = jobject.id;
                singleTweet.tweetContent = jobject.text;
                tweetDataCollection.Add(singleTweet);
            }

            tweetDataCollection.ForEach(x => context.Tweets.Add(x));
        }

        /// <summary>
        /// Reading the Json File using Stream Reader
        /// All the data is converting to a string and returned as a result
        /// </summary>
        /// <returns>Json Data as String</returns>
        protected string getDataFromEmbeddedResource()
        {
            string jsonDataAsString = null;
       
            var assembly = Assembly.GetExecutingAssembly();
            // Retrieve the names of all embedded Resource files ( jsonFile properties - changed to Embedded Resource )
            var names = assembly.GetManifestResourceNames();
            // selecting the Json File ->  names[2] == Json file name
            using (Stream stream = assembly.GetManifestResourceStream(names[0]))
            using (StreamReader streamReader = new StreamReader(stream))
            {
                jsonDataAsString = streamReader.ReadToEnd();
            }

            return jsonDataAsString;
        }

    }
}