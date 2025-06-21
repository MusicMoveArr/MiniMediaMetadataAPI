namespace MiniMediaMetadataAPI.Application.Models.Database.Deezer;

public class DeezerArtistModel
{
    public long ArtistId { get; set; }
    public string Name { get; set; }
    public int NbAlbum { get; set; }
    public int NbFan { get; set; }
    public bool Radio { get; set; }
    public string Type { get; set; }
    public DateTime LastSyncTime { get; set; }

    public List<DeezerArtistImageLinkModel> Images { get; set; } = new List<DeezerArtistImageLinkModel>();
}