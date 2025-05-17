using MiniMediaMetadataAPI.Application.Enums;
using MiniMediaMetadataAPI.Application.Models.Entities;

namespace MiniMediaMetadataAPI.Application.Models;

public class SearchAlbumResponse
{
    public SearchResultType SearchResult { get; set; }
    public List<SearchAlbumEntity> Albums { get; set; }
}