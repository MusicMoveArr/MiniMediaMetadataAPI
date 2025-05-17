namespace MiniMediaMetadataAPI.Application.Models.Database.Tidal;

public class TidalTrackExternalLinkModel
{
    public int TrackId { get; set; }
    public string Href { get; set; }
    public string Meta_Type { get; set; }
}