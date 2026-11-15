using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AstroHealth.Server.Models;

public sealed class Astronaut
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string AstronautId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime MissionStartDate { get; set; } = DateTime.UtcNow;
    public string MissionEnvironment { get; set; } = "ISS";
    public int GamificationPoints { get; set; }
    public List<string> CompletedTasks { get; set; } = new();
}
