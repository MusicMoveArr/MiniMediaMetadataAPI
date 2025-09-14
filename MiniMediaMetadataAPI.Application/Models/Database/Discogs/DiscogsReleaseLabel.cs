namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsReleaseLabel
{
    public int ReleaseId { get; set; }
    public int LabelId { get; set; }
    public string Name { get; set; }
    public string Catno { get; set; }
}