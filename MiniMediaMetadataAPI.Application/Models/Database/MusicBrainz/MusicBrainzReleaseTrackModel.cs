namespace MiniMediaMetadataAPI.Application.Models.Database.MusicBrainz;

public class MusicBrainzReleaseTrackModel
{
    public Guid ReleaseTrackId { get; set; }
    public Guid RecordingTrackId { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string StatusId { get; set; }
    public Guid ReleaseId { get; set; }
    public int Length { get; set; }
    public int Number { get; set; }
    public int Position { get; set; }
    public Guid RecordingId { get; set; }
    public int RecordingLength { get; set; }
    public string RecordingTitle { get; set; }
    public bool RecordingVideo { get; set; }
    public int MediaTrackCount { get; set; }
    public string MediaFormat { get; set; }
    public string MediaTitle { get; set; }
    public int MediaPosition { get; set; }
    public int MediaTrackOffset { get; set; }

    public List<MusicBrainzArtistModel> TrackArtists { get; set; } =
        new List<MusicBrainzArtistModel>();
}