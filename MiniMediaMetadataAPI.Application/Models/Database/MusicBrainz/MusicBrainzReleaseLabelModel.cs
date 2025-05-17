namespace MiniMediaMetadataAPI.Application.Models.Database.MusicBrainz;

public class MusicBrainzReleaseLabelModel
{
    public Guid ReleaseId { get; set; }
    public Guid LabelId { get; set; }
    public string CatalogNumber { get; set; }
}