namespace MiniMediaMetadataAPI.Application.Models.Database.MusicBrainz;

public class MusicBrainzReleaseTrackArtistModel
{
    public Guid ReleaseTrackId { get; set; }
    public Guid ArtistId { get; set; }
    public string JoinPhrase { get; set; }
    public int Index { get; set; }

    public MusicBrainzArtistModel Artist { get; set; }
}