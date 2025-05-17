using MiniMediaMetadataAPI.Application.Enums;
using MiniMediaMetadataAPI.Application.Models.Entities;

namespace MiniMediaMetadataAPI.Application.Models;

public class SearchTrackResponse
{
    public SearchResultType SearchResult { get; set; }
    public List<SearchTrackEntity> Tracks { get; set; }
}