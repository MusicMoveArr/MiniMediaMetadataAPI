namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsReleaseExtraArtist
{
    public int ReleaseId { get; set; }
    public int ArtistId { get; set; }
    public string Name { get; set; }
    public string Anv { get; set; }
    public string Role { get; set; }
}