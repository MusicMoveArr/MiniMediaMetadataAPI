using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniMediaMetadataAPI.Application.Helpers;
using MiniMediaMetadataAPI.Application.Models;
using MiniMediaMetadataAPI.Application.Services;

namespace MiniMediaMetadataAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SearchArtistController : ControllerBase
{
    private readonly SearchArtistService _searchArtistService;
    
    public SearchArtistController(SearchArtistService searchArtistService)
    {
        _searchArtistService = searchArtistService;
    }

    [HttpGet]
    public async Task<SearchArtistResponse> Get([FromQuery] SearchArtistRequest request)
    {
        request.Id = StringHelper.RemoveControlChars(request.Id);
        request.Name = StringHelper.RemoveControlChars(request.Name);
        Debug.WriteLine($"SearchArtist: Id:{request.Id}, Name:{request.Name}, Provider:{request.Provider}");
        
        if (!string.IsNullOrWhiteSpace(request.Id))
        {
            return await _searchArtistService.SearchArtist(request.Id, request.Provider);
        }
        return await _searchArtistService.SearchArtist(request.Name, request.Provider, request.Offset);
    }
}