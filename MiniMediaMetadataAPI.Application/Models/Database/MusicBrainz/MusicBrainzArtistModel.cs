namespace MiniMediaMetadataAPI.Application.Models.Database.MusicBrainz;

public class MusicBrainzArtistModel
{
    public Guid ArtistId { get; set; }
    public int ReleaseCount { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Country { get; set; }
    public DateTime LastSyncTime { get; set; }
    public List<MusicBrainzReleaseModel> Releases { get; set; } = new List<MusicBrainzReleaseModel>();
}