using MiniMediaMetadataAPI.Application.Enums;
using MiniMediaMetadataAPI.Application.Models;
using MiniMediaMetadataAPI.Application.Models.Entities;
using MiniMediaMetadataAPI.Application.Repositories;

namespace MiniMediaMetadataAPI.Application.Services;

public class SearchAlbumService
{
    private readonly SpotifyRepository _spotifyRepository;
    private readonly TidalRepository _tidalRepository;
    private readonly MusicBrainzRepository _musicBrainzRepository;
    private readonly DeezerRepository _deezerRepository;
    
    public SearchAlbumService(
        SpotifyRepository spotifyRepository,
        TidalRepository tidalRepository,
        MusicBrainzRepository musicBrainzRepository,
        DeezerRepository deezerRepository)
    {
        _spotifyRepository = spotifyRepository;
        _tidalRepository = tidalRepository;
        _musicBrainzRepository = musicBrainzRepository;
        _deezerRepository = deezerRepository;
    }

    public async Task<SearchAlbumResponse> SearchAlbum(
        string name, 
        string artistId, 
        ProviderType provider, 
        int offset)
    {
        SearchAlbumResponse response = new SearchAlbumResponse();
        List<SearchAlbumEntity> searchResult = new List<SearchAlbumEntity>();

        if (provider is ProviderType.Any or ProviderType.Spotify)
        {
            var spotifyArtists = await _spotifyRepository.SearchAlbumByArtistIdAsync(name, artistId, offset);
            searchResult.AddRange(spotifyArtists.Select(album => new SearchAlbumEntity
            {
                ProviderType = ProviderType.Spotify,
                Id = album.AlbumId,
                Name = album.Name,
                Popularity = album.Popularity,
                Url = album.Url,
                Label = album.Label,
                ReleaseDate = album.ReleaseDate,
                TotalTracks = album.TotalTracks,
                Type = album.Type,
                UPC = album.ExternalIds.FirstOrDefault(externalId => string.Equals(externalId.Name, "upc"))?.Value,
                Images = album.Images?.Select(image => new  SearchAlbumImageEntity
                {
                    Width = image.Width,
                    Height = image.Height,
                    Url = image.Url
                }).ToList(),
                Artists = album.Artists?.Select(artist => new SearchAlbumArtistEntity
                {
                    Id = artist.ArtistId,
                    Name = artist.ArtistName
                }).ToList()
            }));
        }
        if (provider is ProviderType.Any or ProviderType.Tidal && int.TryParse(artistId, out int intArtistId))
        {
            var tidalAlbums = await _tidalRepository.SearchAlbumByArtistIdAsync(name, intArtistId, offset);
            searchResult.AddRange(tidalAlbums.Select(album => new SearchAlbumEntity
            {
                ProviderType = ProviderType.Tidal,
                Id = album.AlbumId.ToString(),
                Name = album.Title,
                Popularity = album.Popularity,
                Url = album.ExternalLinks?.FirstOrDefault(link => link.Meta_Type == "TIDAL_SHARING")?.Href,
                UPC = album.BarcodeId,
                Copyright = album.Copyright,
                ReleaseDate = album.ReleaseDate,
                TotalTracks = album.NumberOfItems,
                Type = "Album",
                ArtistId = album.ArtistId.ToString(),
                Images = album.Images?.Select(image => new  SearchAlbumImageEntity
                {
                    Width = image.Meta_Width,
                    Height = image.Meta_Height,
                    Url = image.Href
                }).ToList()
            }));
        }
        if (provider is ProviderType.Any or ProviderType.MusicBrainz && Guid.TryParse(artistId, out Guid guidArtistId))
        {
            var musicBrainzArtists = await _musicBrainzRepository.SearchAlbumByArtistIdAsync(name, guidArtistId, offset);
            searchResult.AddRange(musicBrainzArtists.Select(album => new SearchAlbumEntity
            {
                ProviderType = ProviderType.MusicBrainz,
                Id = album.ReleaseId.ToString(),
                Name = album.Title,
                Popularity = 0,
                Url = $"https://musicbrainz.org/release/{album.ReleaseId.ToString()}",
                Label = string.Join(';', album.Labels.Select(label => label.Name)),
                ReleaseDate = album.Date,
                TotalTracks = album.TotalTracks,
                UPC = album.Barcode
            }));
        }
        if (provider is ProviderType.Any or ProviderType.Deezer && long.TryParse(artistId, out long longArtistId))
        {
            var deezerAlbums = await _deezerRepository.SearchAlbumByArtistIdAsync(name, longArtistId, offset);
            searchResult.AddRange(deezerAlbums.Select(album => new SearchAlbumEntity
            {
                ProviderType = ProviderType.Deezer,
                Id = album.AlbumId.ToString(),
                Name = album.Title,
                Popularity = 0,
                Url = $"https://www.deezer.com/album/{album.AlbumId}",
                UPC = album.UPC,
                Copyright = string.Empty,
                Label = album.Label,
                ReleaseDate = album.ReleaseDate,
                TotalTracks = album.NbTracks,
                Type = album.Type,
                ArtistId = album.ArtistId.ToString(),
                Images = album.Images?.Select(image => new  SearchAlbumImageEntity
                {
                    Width = image.Width,
                    Height = image.Height,
                    Url = image.Href
                }).ToList(),
                Artists = album.Artists?.Select(artist => new SearchAlbumArtistEntity
                {
                    Id = artist.ArtistId.ToString(),
                    Name = artist.ArtistName
                }).ToList()
            }));
        }

        response.SearchResult = searchResult.Any() ? SearchResultType.Ok : SearchResultType.NotFound;
        response.Albums = searchResult;

        return response;
    }
    
    
    public async Task<SearchAlbumResponse> SearchAlbum(
        string id,
        ProviderType provider)
    {
        SearchAlbumResponse response = new SearchAlbumResponse();
        List<SearchAlbumEntity> searchResult = new List<SearchAlbumEntity>();

        if (provider is ProviderType.Spotify)
        {
            var spotifyAlbum = await _spotifyRepository.GetAlbumByIdAsync(id);
            if (spotifyAlbum != null)
            {
                searchResult.Add(new SearchAlbumEntity
                {
                    ProviderType = ProviderType.Spotify,
                    Id = spotifyAlbum.AlbumId,
                    Name = spotifyAlbum.Name,
                    Popularity = spotifyAlbum.Popularity,
                    Url = spotifyAlbum.Url,
                    Label = spotifyAlbum.Label,
                    ReleaseDate = spotifyAlbum.ReleaseDate,
                    TotalTracks = spotifyAlbum.TotalTracks,
                    Type = spotifyAlbum.Type,
                    UPC = spotifyAlbum.ExternalIds.FirstOrDefault(externalId => string.Equals(externalId.Name, "upc"))?.Value,
                    Images = spotifyAlbum.Images?.Select(image => new  SearchAlbumImageEntity
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Url = image.Url
                    }).ToList(),
                    Artists = spotifyAlbum.Artists?.Select(artist => new SearchAlbumArtistEntity
                    {
                        Id = artist.ArtistId,
                        Name = artist.ArtistName
                    }).ToList()
                });
            }
            
        }
        if (provider is ProviderType.Tidal && int.TryParse(id, out int intAlbumId))
        {
            var tidalAlbum = await _tidalRepository.GetAlbumIdAsync(intAlbumId);
            searchResult.Add(new SearchAlbumEntity
            {
                ProviderType = ProviderType.Tidal,
                Id = tidalAlbum.AlbumId.ToString(),
                Name = tidalAlbum.Title,
                Popularity = tidalAlbum.Popularity,
                Url = tidalAlbum.ExternalLinks?.FirstOrDefault(link => link.Meta_Type == "TIDAL_SHARING")?.Href,
                UPC = tidalAlbum.BarcodeId,
                Copyright = tidalAlbum.Copyright,
                ReleaseDate = tidalAlbum.ReleaseDate,
                TotalTracks = tidalAlbum.NumberOfItems,
                Artists = new List<SearchAlbumArtistEntity>(new SearchAlbumArtistEntity[]
                {
                    new SearchAlbumArtistEntity
                    {
                        Id = tidalAlbum.Artist.ArtistId.ToString(),
                        Name = tidalAlbum.Artist.Name,
                    }
                }),
                Images = tidalAlbum.Images?.Select(image => new  SearchAlbumImageEntity
                {
                    Width = image.Meta_Width,
                    Height = image.Meta_Height,
                    Url = image.Href
                }).ToList()
            });
        }
        if (provider is ProviderType.Any or ProviderType.MusicBrainz && Guid.TryParse(id, out Guid guidAlbumId))
        {
            var musicBrainzAlbum = await _musicBrainzRepository.GetAlbumByIdAsync(guidAlbumId);
            if (musicBrainzAlbum != null)
            {
                searchResult.Add(new SearchAlbumEntity
                {
                    ProviderType = ProviderType.MusicBrainz,
                    Id = musicBrainzAlbum.ReleaseId.ToString(),
                    Name = musicBrainzAlbum.Title,
                    Popularity = 0,
                    Url = $"https://musicbrainz.org/release/{musicBrainzAlbum.ReleaseId.ToString()}",
                    Label = string.Join(';', musicBrainzAlbum.Labels.Select(label => label.Name)),
                    ReleaseDate = musicBrainzAlbum.Date,
                    TotalTracks = musicBrainzAlbum.TotalTracks,
                    UPC = musicBrainzAlbum.Barcode
                });
            }
        }
        if (provider is ProviderType.Deezer && long.TryParse(id, out long longAlbumId))
        {
            var deezerAlbum = await _deezerRepository.GetAlbumIdAsync(longAlbumId);
            searchResult.Add(new SearchAlbumEntity
            {
                ProviderType = ProviderType.Deezer,
                Id = deezerAlbum.AlbumId.ToString(),
                ArtistId = deezerAlbum.ArtistId.ToString(),
                Name = deezerAlbum.Title,
                Popularity = 0,
                Url = $"https://www.deezer.com/album/{deezerAlbum.AlbumId}",
                UPC = deezerAlbum.UPC,
                Copyright = string.Empty,
                Label = deezerAlbum.Label,
                ReleaseDate = deezerAlbum.ReleaseDate,
                TotalTracks = deezerAlbum.NbTracks,
                Artists = new List<SearchAlbumArtistEntity>([
                    new SearchAlbumArtistEntity
                    {
                        Id = deezerAlbum.Artist.ArtistId.ToString(),
                        Name = deezerAlbum.Artist.Name,
                    }
                ]),
                Images = deezerAlbum.Images?.Select(image => new  SearchAlbumImageEntity
                {
                    Width = image.Width,
                    Height = image.Height,
                    Url = image.Href
                }).ToList()
            });
        }

        response.SearchResult = searchResult.Any() ? SearchResultType.Ok : SearchResultType.NotFound;
        response.Albums = searchResult;

        return response;
    }
}