namespace MiniMediaMetadataAPI.Application.Models.Database.Tidal;

public class TidalAlbumImageModel
{
    public string AlbumId { get; set; }
    public string Href { get; set; }
    public int Meta_Width { get; set; }
    public int Meta_Height { get; set; }
}