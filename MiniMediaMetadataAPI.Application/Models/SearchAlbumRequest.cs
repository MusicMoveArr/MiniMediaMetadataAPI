using MiniMediaMetadataAPI.Application.Enums;

namespace MiniMediaMetadataAPI.Application.Models;

public class SearchAlbumRequest
{
    public ProviderType Provider { get; set; } = ProviderType.Any;
    public string AlbumId { get; set; }
    public string ArtistId { get; set; }
    public string AlbumName { get; set; }
    public int Offset { get; set; }
}