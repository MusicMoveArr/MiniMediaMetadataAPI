namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsReleaseTrack
{
    public int ReleaseId { get; set; }
    public string Title { get; set; }
    public string Position { get; set; }
    public string Duration { get; set; }
    
    public DiscogsRelease Release { get; set; }
}