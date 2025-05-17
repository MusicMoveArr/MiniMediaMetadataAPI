namespace MiniMediaMetadataAPI.Application.Models.Database.Tidal;

public class TidalAlbumModel
{
    public int AlbumId { get; set; }
    public int ArtistId { get; set; }
    public string Title { get; set; }
    public string BarcodeId { get; set; }
    public int NumberOfVolumes { get; set; }
    public int NumberOfItems { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Explicit { get; set; }
    public string ReleaseDate { get; set; }
    public string Copyright { get; set; }
    public float Popularity { get; set; }
    public string Availability { get; set; }
    public string MediaTags { get; set; }
    
    public List<TidalAlbumImageModel>? Images { get; set; }
    public List<TidalAlbumExternalLinkModel>? ExternalLinks { get; set; }
    public TidalArtistModel Artist { get; set; }
}