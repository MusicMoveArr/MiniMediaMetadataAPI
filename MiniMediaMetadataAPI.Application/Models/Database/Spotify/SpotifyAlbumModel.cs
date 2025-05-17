namespace MiniMediaMetadataAPI.Application.Models.Database.Spotify;

public class SpotifyAlbumModel
{
    public string AlbumId { get; set; }
    public string AlbumGroup { get; set; }
    public string AlbumType { get; set; }
    public string Name { get; set; }
    public string ReleaseDate { get; set; }
    public string ReleaseDatePrecision { get; set; }
    public int TotalTracks { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public string Label { get; set; }
    public int Popularity { get; set; }
    public string ArtistId { get; set; }
    
    public List<SpotifyAlbumExternalIdModel> ExternalIds { get; set; }
    public List<SpotifyAlbumImageModel>? Images { get; set; }
    public List<SpotifyAlbumArtistModel>? Artists { get; set; }
}