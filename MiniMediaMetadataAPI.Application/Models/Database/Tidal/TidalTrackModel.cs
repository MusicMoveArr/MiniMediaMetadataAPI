namespace MiniMediaMetadataAPI.Application.Models.Database.Tidal;

public class TidalTrackModel
{
    public int TrackId { get; set; }
    public int AlbumId { get; set; }
    public string Title { get; set; }
    public string ISRC { get; set; }
    public TimeSpan Duration { get; set; }
    public string Copyright { get; set; }
    public bool Explicit { get; set; }
    public float Popularity { get; set; }
    public string Availability { get; set; }
    public string MediaTags { get; set; }
    public int VolumeNumber { get; set; }
    public int TrackNumber { get; set; }
    public string Version { get; set; }
    
    public List<TidalTrackImageLinkModel>? Images { get; set; }
    public List<TidalTrackExternalLinkModel>? ExternalLinks { get; set; }
    public List<TidalTrackArtistModel>? Artists { get; set; }
    public TidalAlbumModel Album { get; set; }

    public string FullName
    {
        get
        {
            string _version = !string.IsNullOrWhiteSpace(Version) ? $" ({Version})" : string.Empty;
            return $"{Title}{_version}";
        }
    }
}