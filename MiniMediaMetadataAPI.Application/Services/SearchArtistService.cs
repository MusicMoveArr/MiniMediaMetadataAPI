using System.Diagnostics;
using MiniMediaMetadataAPI.Application.Enums;
using MiniMediaMetadataAPI.Application.Models;
using MiniMediaMetadataAPI.Application.Models.Entities;
using MiniMediaMetadataAPI.Application.Repositories;

namespace MiniMediaMetadataAPI.Application.Services;

public class SearchArtistService
{
    private readonly SpotifyRepository _spotifyRepository;
    private readonly TidalRepository _tidalRepository;
    private readonly MusicBrainzRepository _musicBrainzRepository;
    private readonly DeezerRepository _deezerRepository;
    private readonly DiscogsRepository _discogsRepository;
    
    public SearchArtistService(
        SpotifyRepository spotifyRepository,
        TidalRepository tidalRepository,
        MusicBrainzRepository musicBrainzRepository,
        DeezerRepository deezerRepository,
        DiscogsRepository discogsRepository)
    {
        _spotifyRepository = spotifyRepository;
        _tidalRepository = tidalRepository;
        _musicBrainzRepository = musicBrainzRepository;
        _deezerRepository = deezerRepository;
        _discogsRepository = discogsRepository;
    }

    public async Task<SearchArtistResponse> SearchArtist(
        string name, 
        ProviderType provider, 
        int offset)
    {
        SearchArtistResponse response = new SearchArtistResponse();
        List<SearchArtistEntity> searchResult = new List<SearchArtistEntity>();

        if (provider is ProviderType.Any or ProviderType.Spotify)
        {
            var spotifyArtists = await _spotifyRepository.SearchArtistAsync(name, offset);
            searchResult.AddRange(spotifyArtists?.Select(artist => new SearchArtistEntity
            {
                ProviderType = ProviderType.Spotify,
                Id = artist.Id,
                Name = artist.Name,
                Popularity = artist.Popularity,
                Url = artist.Href,
                TotalFollowers = artist.TotalFollowers,
                Genres = artist.Genres,
                LastSyncTime = artist.LastSyncTime,
                Images = artist.Images?.Select(image => new  SearchArtistImageEntity
                {
                    Width = image.Width,
                    Height = image.Height,
                    Url = image.Url
                }).ToList()
            }) ?? []);
        }
        if (provider is ProviderType.Any or ProviderType.Tidal)
        {
            var tidalArtists = await _tidalRepository.SearchArtistAsync(name, offset);
            searchResult.AddRange(tidalArtists?.Select(artist => new SearchArtistEntity
            {
                ProviderType = ProviderType.Tidal,
                Id = artist.ArtistId.ToString(),
                Name = artist.Name,
                Popularity = artist.Popularity,
                Url = string.Empty,
                TotalFollowers = 0,
                Genres = string.Empty,
                LastSyncTime = artist.LastSyncTime,
                Images = artist.Images?.Select(image => new  SearchArtistImageEntity
                {
                    Width = image.Meta_Width,
                    Height = image.Meta_Height,
                    Url = image.Href
                }).ToList()
            }) ?? []);
        }
        if (provider is ProviderType.Any or ProviderType.MusicBrainz)
        {
            var musicBrainzArtists = await _musicBrainzRepository.SearchArtistAsync(name, offset);
            searchResult.AddRange(musicBrainzArtists?.Select(artist => new SearchArtistEntity
            {
                ProviderType = ProviderType.MusicBrainz,
                Id = artist.ArtistId.ToString(),
                Name = artist.Name,
                Popularity = 0,
                Url = $"https://musicbrainz.org/artist/{artist.ArtistId.ToString()}",
                TotalFollowers = 0,
                Genres = string.Empty,
                LastSyncTime = artist.LastSyncTime
            }) ?? []);
        }
        if (provider is ProviderType.Any or ProviderType.Deezer)
        {
            var deezerArtists = await _deezerRepository.SearchArtistAsync(name, offset);
            searchResult.AddRange(deezerArtists?.Select(artist => new SearchArtistEntity
            {
                ProviderType = ProviderType.Deezer,
                Id = artist.ArtistId.ToString(),
                Name = artist.Name,
                Popularity = artist.NbFan, //not the same as Tidal (in percentage)
                Url = $"https://www.deezer.com/artist/{artist.ArtistId}",
                TotalFollowers = artist.NbFan,
                Genres = string.Empty,
                LastSyncTime = artist.LastSyncTime,
                Images = artist.Images?.Select(image => new  SearchArtistImageEntity
                {
                    Width = image.Width,
                    Height = image.Height,
                    Url = image.Href
                }).ToList()
            }) ?? []);
        }
        if (provider is ProviderType.Any or ProviderType.Discogs)
        {
            var discogsArtists = await _discogsRepository.SearchArtistAsync(name, offset);
            searchResult.AddRange(discogsArtists?.Select(artist => new SearchArtistEntity
            {
                ProviderType = ProviderType.Discogs,
                Id = artist.ArtistId.ToString(),
                Name = artist.Name,
                Popularity = 0,
                Url = $"https://www.discogs.com/artist/{artist.ArtistId}",
                TotalFollowers = 0,
                Genres = string.Empty,
                //LastSyncTime = artist.LastSyncTime
            }) ?? []);
        }

        response.SearchResult = searchResult.Any() ? SearchResultType.Ok : SearchResultType.NotFound;
        response.Artists = searchResult;
        Debug.WriteLine($"Artists found: {searchResult.Count}");

        return response;
    }
    
    
    public async Task<SearchArtistResponse> SearchArtist(
        string id,
        ProviderType provider)
    {
        SearchArtistResponse response = new SearchArtistResponse();
        List<SearchArtistEntity> searchResult = new List<SearchArtistEntity>();

        if (provider is ProviderType.Spotify)
        {
            var spotifyArtist = await _spotifyRepository.GetArtistByIdAsync(id);
            if (spotifyArtist != null)
            {
                searchResult.Add(new SearchArtistEntity
                {
                    ProviderType = ProviderType.Spotify,
                    Id = spotifyArtist.Id,
                    Name = spotifyArtist.Name,
                    Popularity = spotifyArtist.Popularity,
                    Url = spotifyArtist.Href,
                    TotalFollowers = spotifyArtist.TotalFollowers,
                    Genres = spotifyArtist.Genres,
                    LastSyncTime = spotifyArtist.LastSyncTime,
                    Images = spotifyArtist.Images?.Select(image => new  SearchArtistImageEntity
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Url = image.Url
                    }).ToList()
                });
            }
            
        }
        if (provider is ProviderType.Tidal && int.TryParse(id, out int intArtistId))
        {
            var tidalArtist = await _tidalRepository.GetArtistByIdAsync(intArtistId);
            if (tidalArtist != null)
            {
                searchResult.Add(new SearchArtistEntity
                {
                    ProviderType = ProviderType.Tidal,
                    Id = tidalArtist.ArtistId.ToString(),
                    Name = tidalArtist.Name,
                    Popularity = tidalArtist.Popularity,
                    Url = string.Empty,
                    TotalFollowers = 0,
                    Genres = string.Empty,
                    LastSyncTime = tidalArtist.LastSyncTime,
                    Images = tidalArtist.Images?.Select(image => new  SearchArtistImageEntity
                    {
                        Width = image.Meta_Width,
                        Height = image.Meta_Height,
                        Url = image.Href
                    }).ToList()
                });
            }
        }
        if (provider is ProviderType.MusicBrainz && Guid.TryParse(id, out Guid guidArtistId))
        {
            var musicBrainzArtist = await _musicBrainzRepository.GetArtistByIdAsync(guidArtistId);

            if (musicBrainzArtist != null)
            {
                searchResult.Add(new SearchArtistEntity
                {
                    ProviderType = ProviderType.MusicBrainz,
                    Id = musicBrainzArtist.ArtistId.ToString(),
                    Name = musicBrainzArtist.Name,
                    Popularity = 0,
                    Url = $"https://musicbrainz.org/artist/{musicBrainzArtist.ArtistId.ToString()}",
                    TotalFollowers = 0,
                    Genres = string.Empty,
                    LastSyncTime = musicBrainzArtist.LastSyncTime
                });
            }
        }
        if (provider is ProviderType.Deezer && long.TryParse(id, out long longArtistId))
        {
            var deezerArtist = await _deezerRepository.GetArtistByIdAsync(longArtistId);
            if (deezerArtist != null)
            {
                searchResult.Add(new SearchArtistEntity
                {
                    ProviderType = ProviderType.Deezer,
                    Id = deezerArtist.ArtistId.ToString(),
                    Name = deezerArtist.Name,
                    Popularity = deezerArtist.NbFan, //not the same as Tidal (in percentage)
                    Url = $"https://www.deezer.com/artist/{deezerArtist.ArtistId}",
                    TotalFollowers = deezerArtist.NbFan,
                    Genres = string.Empty,
                    LastSyncTime = deezerArtist.LastSyncTime,
                    Images = deezerArtist.Images?.Select(image => new  SearchArtistImageEntity
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Url = image.Href
                    }).ToList()
                });
            }
        }
        if (provider is ProviderType.Discogs && int.TryParse(id, out int discogsArtistId))
        {
            var discogsArtist = await _discogsRepository.GetArtistByIdAsync(discogsArtistId);
            if (discogsArtist != null)
            {
                searchResult.Add(new SearchArtistEntity
                {
                    ProviderType = ProviderType.Discogs,
                    Id = discogsArtist.ArtistId.ToString(),
                    Name = discogsArtist.Name,
                    Popularity = 0,
                    Url = $"https://www.discogs.com/artist/{discogsArtist.ArtistId}",
                    TotalFollowers = 0,
                    Genres = string.Empty
                });
            }
        }

        response.SearchResult = searchResult.Any() ? SearchResultType.Ok : SearchResultType.NotFound;
        response.Artists = searchResult;
        Debug.WriteLine($"Artists found: {searchResult.Count}");

        return response;
    }
}