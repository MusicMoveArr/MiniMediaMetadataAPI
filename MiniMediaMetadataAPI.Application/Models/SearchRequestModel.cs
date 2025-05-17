using MiniMediaMetadataAPI.Application.Enums;

namespace MiniMediaMetadataAPI.Application.Models;

public class SearchRequestModel
{
    public ProviderType Provider { get; set; }
    
    public string ArtistName { get; set; }
    public string AlbumName { get; set; }
    public string TrackName { get; set; }
    public string ISRC { get; set; }
    public string UPC { get; set; }
}