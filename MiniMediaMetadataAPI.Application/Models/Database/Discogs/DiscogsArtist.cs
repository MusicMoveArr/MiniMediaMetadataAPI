namespace MiniMediaMetadataAPI.Application.Models.Database.Discogs;

public class DiscogsArtist
{
    public int ArtistId { get; set; }
    public string Name { get; set; }
    public string RealName { get; set; }
    public string Profile { get; set; }
    public string DataQuality { get; set; }
    public DateTime lastsynctime { get; set; }
}