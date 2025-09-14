namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsReleaseIdentifier
{
    public int ReleaseId { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
}