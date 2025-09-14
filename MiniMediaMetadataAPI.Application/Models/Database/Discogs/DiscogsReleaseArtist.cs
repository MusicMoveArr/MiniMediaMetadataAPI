namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsReleaseArtist
{
    public int ReleaseId { get; set; }
    public int ArtistId { get; set; }
    public string Name { get; set; }
}