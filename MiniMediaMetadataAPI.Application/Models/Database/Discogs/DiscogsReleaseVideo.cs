namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsReleaseVideo
{
    public int ReleaseId { get; set; }
    public string Duration { get; set; }
    public bool Embed { get; set; }
    public string Source { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}