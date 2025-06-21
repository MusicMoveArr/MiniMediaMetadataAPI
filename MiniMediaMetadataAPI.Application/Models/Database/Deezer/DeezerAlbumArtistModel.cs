namespace MiniMediaMetadataAPI.Application.Models.Database.Deezer;

public class DeezerAlbumArtistModel
{
    public long AlbumId { get; set; }
    public long ArtistId { get; set; }
    public string Role { get; set; }
    public string ArtistName { get; set; }
}