namespace MiniMediaMetadataAPI.Application.Models.Database.SoundCloud;

public class SoundCloudAlbumArtistModel
{
    public long AlbumId { get; set; }
    public long ArtistId { get; set; }
    public string Role { get; set; }
    public string ArtistName { get; set; }
}