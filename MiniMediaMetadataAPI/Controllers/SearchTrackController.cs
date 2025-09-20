using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniMediaMetadataAPI.Application.Helpers;
using MiniMediaMetadataAPI.Application.Models;
using MiniMediaMetadataAPI.Application.Services;

namespace MiniMediaMetadataAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SearchTrackController : ControllerBase
{
    private readonly SearchTrackService _searchTrackService;
    
    public SearchTrackController(SearchTrackService searchTrackService)
    {
        _searchTrackService = searchTrackService;
    }

    [HttpGet]
    public async Task<SearchTrackResponse> Get([FromQuery] SearchTrackRequest request)
    {
        request.ArtistId = StringHelper.RemoveControlChars(request.ArtistId);
        request.TrackId = StringHelper.RemoveControlChars(request.TrackId);
        request.TrackName = StringHelper.RemoveControlChars(request.TrackName);
        Debug.WriteLine($"SearchTrack: ArtistId:{request.ArtistId}, TrackId:{request.TrackId}, TrackName:{request.TrackName}, Provider:{request.Provider}");
        
        if (!string.IsNullOrWhiteSpace(request.TrackId))
        {
            return await _searchTrackService.SearchTrack(request.TrackId, request.Provider);
        }
        return await _searchTrackService.SearchTrack(request.TrackName, request.ArtistId, request.Provider, request.Offset);
    }
}