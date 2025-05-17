using MiniMediaMetadataAPI.Application.Enums;

namespace MiniMediaMetadataAPI.Application.Models;

public class SearchTrackRequest
{
    public ProviderType Provider { get; set; } = ProviderType.Any;
    public string TrackId { get; set; }
    public string ArtistId { get; set; }
    public string TrackName { get; set; }
    public int Offset { get; set; }
}