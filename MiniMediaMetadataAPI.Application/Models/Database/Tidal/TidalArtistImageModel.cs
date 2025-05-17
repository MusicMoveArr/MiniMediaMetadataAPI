namespace MiniMediaMetadataAPI.Application.Models.Database.Tidal;

public class TidalArtistImageModel
{
    public int ArtistId { get; set; }
    public string Href { get; set; }
    public int Meta_Width { get; set; }
    public int Meta_Height { get; set; }
}