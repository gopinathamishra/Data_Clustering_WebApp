
using DataClusteringWebApp.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DataClusteringWebApp.DataAccessLayer
{
    public class DataClusteringContext : DbContext
    {
        public DataClusteringContext() : base ("DataClusteringContext")
        {
        }

        public DbSet<Tweet> Tweets { get; set; }
    }
}