namespace MiniMediaMetadataAPI.Application.Models.Database.Spotify;

public class SpotifyArtistModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public float Popularity { get; set; }
    public string Type { get; set; }
    public string Uri { get; set; }
    public int TotalFollowers { get; set; }
    public string Href { get; set; }
    public string Genres { get; set; }
    public DateTime LastSyncTime { get; set; }
    public List<SpotifyArtistImageModel>? Images { get; set; }
}