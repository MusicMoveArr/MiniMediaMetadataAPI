using Microsoft.AspNetCore.Mvc;
using MiniMediaMetadataAPI.Application.Models;
using MiniMediaMetadataAPI.Application.Services;

namespace MiniMediaMetadataAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly SearchArtistService _searchArtistService;
    
    public SearchController(SearchArtistService searchArtistService)
    {
        _searchArtistService = searchArtistService;
    }

    [HttpGet]
    public async Task<SearchArtistResponse> Get([FromQuery] SearchArtistRequest request)
    {
        return new SearchArtistResponse();
        //return await _searchService.SearchArtist(request.ArtistName, request.Provider, 0);
    }
}