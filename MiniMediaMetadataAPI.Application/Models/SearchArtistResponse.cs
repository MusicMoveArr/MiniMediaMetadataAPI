using MiniMediaMetadataAPI.Application.Enums;
using MiniMediaMetadataAPI.Application.Models.Entities;

namespace MiniMediaMetadataAPI.Application.Models;

public class SearchArtistResponse
{
    public SearchResultType SearchResult { get; set; }
    public List<SearchArtistEntity> Artists { get; set; }
}