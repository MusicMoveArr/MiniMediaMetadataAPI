using MiniMediaMetadataAPI.Application.Enums;

namespace MiniMediaMetadataAPI.Application.Models;

public class SearchArtistRequest
{
    public ProviderType Provider { get; set; } = ProviderType.Any;
    public string Id { get; set; }
    public string Name { get; set; }
    public int Offset { get; set; }
}