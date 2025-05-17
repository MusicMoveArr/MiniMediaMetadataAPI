namespace MiniMediaMetadataAPI.Application.Models.Database.Tidal;

public class TidalTrackImageLinkModel
{
    public int TrackId { get; set; }
    public string Href { get; set; }
    public int Meta_Width { get; set; }
    public int Meta_Height { get; set; }
}