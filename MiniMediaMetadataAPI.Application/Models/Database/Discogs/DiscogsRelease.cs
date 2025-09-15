namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsRelease
{
    public int ReleaseId { get; set; }
    public string Status { get; set; }
    public string Title { get; set; }
    public string Country { get; set; }
    public string Released { get; set; }
    public string Notes { get; set; }
    public string DataQuality { get; set; }
    public bool IsMainRelease { get; set; }
    public int MasterId { get; set; }
    public int TrackCount { get; set; }
    
    public List<DiscogsReleaseArtist> Artists { get; set; } = new List<DiscogsReleaseArtist>();
    public List<DiscogsReleaseIdentifier> Identifiers { get; set; } = new List<DiscogsReleaseIdentifier>();
}