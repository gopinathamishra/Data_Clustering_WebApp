

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataClusteringWebApp.DataAccessLayer
{
    /// <summary>
    /// The model class for Json File 
    /// List all the Objects and their attributes found in Json File for each Json object
    /// </summary>
    public class JsonModel
    {
        public string text { get; set; }
        public string profile_image_url { get; set; }
        public string from_user { get; set; }
        public int from_user_id { get; set; }
        public object geo { get; set; }
        public long id { get; set; }
        public string iso_language_code { get; set; }
        public string from_user_id_str { get; set; }
        public string created_at { get; set; }
        public string source { get; set; }
        public string id_str { get; set; }
        public string from_user_name { get; set; }
        public string profile_image_url_https { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Metadata
    {
        public string result_type { get; set; }
    }
}