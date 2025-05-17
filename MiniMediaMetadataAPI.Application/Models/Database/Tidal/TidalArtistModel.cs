namespace MiniMediaMetadataAPI.Application.Models.Database.Tidal;

public class TidalArtistModel
{
    public int ArtistId { get; set; }
    public string Name { get; set; }
    public float Popularity { get; set; }
    public DateTime LastSyncTime { get; set; }
    public List<TidalArtistImageModel>? Images { get; set; }
}