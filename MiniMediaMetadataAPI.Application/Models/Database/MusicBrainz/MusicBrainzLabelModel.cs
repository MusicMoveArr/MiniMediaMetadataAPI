namespace MiniMediaMetadataAPI.Application.Models.Database.MusicBrainz;

public class MusicBrainzLabelModel
{
    public Guid LabelId { get; set; }
    public Guid AreaId { get; set; }
    public string Name { get; set; }
    public string Disambiguation { get; set; }
    public int LabelCode { get; set; }
    public string Type { get; set; }
    public string LifeSpanBegin { get; set; }
    public string LifeSpanEnd { get; set; }
    public bool LifeSpanEnded { get; set; }
    public string SortName { get; set; }
    public string TypeId { get; set; }
    public string Country { get; set; }
}