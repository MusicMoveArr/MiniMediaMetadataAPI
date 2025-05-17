namespace MiniMediaMetadataAPI.Application.Models.Database.Spotify;

public class SpotifyTrackModel
{
    public string TrackId { get; set; }
    public string AlbumId { get; set; }
    public int DiscNumber { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Explicit { get; set; }
    public string Href { get; set; }
    public bool IsPlayable { get; set; }
    public string Name { get; set; }
    public string PreviewUrl { get; set; }
    public int TrackNumber { get; set; }
    public string Type { get; set; }
    public string Uri { get; set; }
    public List<SpotifyTrackExternalIdModel>? ExternalIds { get; set; }
    public List<SpotifyTrackArtistModel>? Artists { get; set; }
    public SpotifyAlbumModel Album { get; set; }
}