namespace MiniMediaMetadataAPI.Application.Models.Database.MusicBrainz;

public class MusicBrainzReleaseModel
{
    public Guid ArtistId { get; set; }
    public Guid ReleaseId { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string StatusId { get; set; }
    public string Date { get; set; }
    public string Barcode { get; set; }
    public string Country { get; set; }
    public string Disambiguation { get; set; }
    public string Quality { get; set; }
    
    //extra
    public int TotalTracks { get; set; }

    public List<MusicBrainzReleaseTrackModel> Tracks { get; set; } = new List<MusicBrainzReleaseTrackModel>();
    public List<MusicBrainzLabelModel> Labels { get; set; } = new List<MusicBrainzLabelModel>();
}