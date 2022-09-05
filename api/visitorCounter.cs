using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Visitor.Function
{
  /// <summary>
  /// Logs an entry to a cosmo db api table to record the time a user accessed the website
  /// and returns the current user count.
  /// </summary>
  public class visitorCounter
  {
    /// <summary>
    /// set of key vaule pairs obtained from azure configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// obtain an instance of the IConfiguration 
    /// </summary>
    /// <param name="configuration">IConfiguration </param>
    public visitorCounter(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    /// <summary>
    /// azure funtion to update & return the webpage count
    /// </summary>
    /// <param name="req">http method</param>
    /// <param name="log">logger</param>
    /// <returns>webpage visitor count</returns>
    [FunctionName("visitorCounter")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");

      // Read configuration data
      string keyName = "DBConnectionString";
      string connectionStr = _configuration[keyName];

      log.LogInformation("Connecting to database");
      IMongoCollection<Visitor> _visitors = ConnectToCollection(connectionStr, "CloudResume", "visitors");

      log.LogInformation("Getting visitor count.");
      int visitorCount = GetVisitorCount(_visitors);

      log.LogInformation($"Current Vistor Count: {visitorCount}");
      string response = $"{visitorCount}";

      return new OkObjectResult(response);
    }

    /// <summary>
    /// Count the number of rows (visitors)
    /// </summary>
    /// <param name="_visitors">monogo db collection</param>
    /// <returns>count of wesbite visitors</returns>
    public static int GetVisitorCount(IMongoCollection<Visitor> _visitors)
    {
      int visitorCount = (int)_visitors.EstimatedDocumentCount();
      int updatedVisitorCount = visitorCount++;
      UpdateVisitorCount(_visitors, updatedVisitorCount);

      return updatedVisitorCount;
    }

    /// <summary>
    /// Add a new row to update the visitor count
    /// </summary>
    /// <param name="_visitors">monogo db collection</param>
    /// <param name="visitorCount">current visitor count</param>

    private static void UpdateVisitorCount(IMongoCollection<Visitor> _visitors, int visitorCount)
    {
      //Create new object and upsert(create or replace) to container
      _visitors.InsertOne(new Visitor(
          Guid.NewGuid().ToString(),
          DateTime.Now.ToString(),
          visitorCount
      ));
    }

    /// <summary>
    /// Create a connection to a monogo db
    /// </summary>
    /// <param name="connectionStr">connection string</param>
    /// <returns>mongo db connection</returns>
    private static IMongoCollection<Visitor> ConnectToCollection(string connectionStr, string database, string collection)
    {
      var client = new MongoClient(connectionStr);
      var db = client.GetDatabase(database);
      // Container reference with creation if it does not alredy exist
      var _visitors = db.GetCollection<Visitor>(collection);
      return _visitors;
    }
  }

  /// <summary>
  /// Model of the vistior table
  /// </summary>
  public class Visitor
  {
    /// <summary>
    /// Set the values to log to the db
    /// </summary>
    /// <param name="v1">GUID</param>
    /// <param name="v2">the time the user accessed the website</param>
    /// <param name="v3s"> current user count</param>
    public Visitor(string v1, string v2, int v3)
    {
      Id = v1;
      StateDate = v2;
      UserCount = v3;
    }

    public string StateDate { get; set; }
    public string Id { get; set; }
    public int UserCount { get; set; }
  }
}
