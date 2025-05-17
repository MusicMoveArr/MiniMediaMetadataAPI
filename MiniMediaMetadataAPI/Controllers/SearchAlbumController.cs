using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniMediaMetadataAPI.Application.Helpers;
using MiniMediaMetadataAPI.Application.Models;
using MiniMediaMetadataAPI.Application.Services;

namespace MiniMediaMetadataAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SearchAlbumController : ControllerBase
{
    private readonly SearchAlbumService _searchAlbumService;
    
    public SearchAlbumController(SearchAlbumService searchAlbumService)
    {
        _searchAlbumService = searchAlbumService;
    }

    [HttpGet]
    public async Task<SearchAlbumResponse> Get([FromQuery] SearchAlbumRequest request)
    {
        request.ArtistId = StringHelper.RemoveControlChars(request.ArtistId);
        request.AlbumId = StringHelper.RemoveControlChars(request.AlbumId);
        request.AlbumName = StringHelper.RemoveControlChars(request.AlbumName);
        
        Debug.WriteLine($"SearchAlbum: ArtistId:{request.ArtistId}, AlbumId:{request.AlbumId}, AlbumName:{request.AlbumName}, Provider:{request.Provider}");
        
        if (!string.IsNullOrWhiteSpace(request.AlbumId))
        {
            return await _searchAlbumService.SearchAlbum(request.AlbumId, request.Provider);
        }
        return await _searchAlbumService.SearchAlbum(request.AlbumName, request.ArtistId, request.Provider, request.Offset);
    }
}