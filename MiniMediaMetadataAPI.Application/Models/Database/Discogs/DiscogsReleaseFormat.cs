namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsReleaseFormat
{
    public Guid ReleaseFormatUuId { get; set; } = Guid.NewGuid();
    public int ReleaseId { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public string? Text { get; set; }
}