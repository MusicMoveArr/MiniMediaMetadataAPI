using System.Diagnostics;
using MiniMediaMetadataAPI.Application.Enums;
using MiniMediaMetadataAPI.Application.Models;
using MiniMediaMetadataAPI.Application.Models.Entities;
using MiniMediaMetadataAPI.Application.Repositories;

namespace MiniMediaMetadataAPI.Application.Services;

public class SearchTrackService
{
    private readonly SpotifyRepository _spotifyRepository;
    private readonly TidalRepository _tidalRepository;
    private readonly MusicBrainzRepository _musicBrainzRepository;
    private readonly DeezerRepository _deezerRepository;
    
    public SearchTrackService(
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

    public async Task<SearchTrackResponse> SearchTrack(
        string trackName, 
        string artistId, 
        ProviderType provider, 
        int offset)
    {
        SearchTrackResponse response = new SearchTrackResponse();
        List<SearchTrackEntity> searchResult = new List<SearchTrackEntity>();

        if (provider is ProviderType.Any or ProviderType.Spotify)
        {
            var spotifyArtists = await _spotifyRepository.SearchTrackByArtistIdAsync(trackName, artistId, offset);
            searchResult.AddRange(spotifyArtists.Select(track => new SearchTrackEntity
            {
                ProviderType = ProviderType.Spotify,
                Id = track.TrackId,
                Name = track.Name,
                Popularity = 0,
                Url = track.Href,
                Duration = track.Duration,
                Explicit = track.Explicit,
                DiscNumber = track.DiscNumber,
                TrackNumber = track.TrackNumber,
                Label = string.Empty,
                ISRC = track.ExternalIds?.FirstOrDefault(externalId => string.Equals(externalId.Name, "isrc"))?.Value,
                Album = new SearchTrackAlbumEntity
                {
                    Id = track.Album.AlbumId,
                    ArtistId = track.Album.ArtistId?.ToString(),
                    Name = track.Album.Name,
                    Type = track.Album.Type,
                    ReleaseDate = track.Album.ReleaseDate,
                    TotalTracks = track.Album.TotalTracks,
                    Url = track.Album.Url,
                    Label = track.Album.Label,
                    Popularity = track.Album.Popularity,
                    UPC = track.Album.ExternalIds.FirstOrDefault(externalId => string.Equals(externalId.Name, "upc"))?.Value,
                    ProviderType = ProviderType.Spotify
                },
                Artists = track.Artists?.Select(artist => new SearchTrackArtistEntity
                {
                    Id = artist.ArtistId,
                    Name = artist.ArtistName
                }).ToList()
            }));
        }
        if (provider is ProviderType.Any or ProviderType.Tidal && int.TryParse(artistId, out int intArtistId))
        {
            var tidalTracks = await _tidalRepository.SearchTrackByArtistIdAsync(trackName, intArtistId, offset);
            searchResult.AddRange(tidalTracks.Select(track => new SearchTrackEntity
            {
                ProviderType = ProviderType.Tidal,
                Id = track.TrackId.ToString(),
                Name = track.FullName,
                Popularity = track.Popularity,
                Url = track.ExternalLinks?.FirstOrDefault(link => link.Meta_Type == "TIDAL_SHARING")?.Href,
                Duration = track.Duration,
                Explicit = track.Explicit,
                DiscNumber = track.VolumeNumber,
                TrackNumber = track.TrackNumber,
                Label = string.Empty,
                ISRC = track.ISRC,
                Album = new SearchTrackAlbumEntity
                {
                    Id = track.Album.AlbumId.ToString(),
                    ArtistId = track.Album.ArtistId.ToString(),
                    Name = track.Album.Title,
                    Type = string.Empty,
                    ReleaseDate = track.Album.ReleaseDate,
                    TotalTracks = track.Album.NumberOfItems,
                    Url = track.Album.ExternalLinks?.FirstOrDefault(link => link.Meta_Type == "TIDAL_SHARING")?.Href,
                    Label = string.Empty,
                    Popularity = track.Album.Popularity,
                    UPC = track.Album.BarcodeId,
                    ProviderType = ProviderType.Tidal
                },
                Artists = track.Artists?.Select(artist => new SearchTrackArtistEntity
                {
                    Id = artist.ArtistId.ToString(),
                    Name = artist.ArtistName
                }).ToList()
            }));
        }
        if (provider is ProviderType.Any or ProviderType.MusicBrainz && Guid.TryParse(artistId, out Guid guidArtistId))
        {
            var musicBrainzArtists = await _musicBrainzRepository.SearchTrackByArtistIdAsync(trackName, guidArtistId, offset);

            foreach (var artist in musicBrainzArtists)
            {
                foreach (var release in artist.Releases)
                {
                    searchResult.AddRange(release.Tracks.Select(track => new SearchTrackEntity
                    {
                        ProviderType = ProviderType.MusicBrainz,
                        Id = track.ReleaseTrackId.ToString(),
                        Name = track.Title,
                        Popularity = 0,
                        Url = $"https://musicbrainz.org/recording/{track.RecordingTrackId}",
                        Duration = TimeSpan.FromMilliseconds(track.Length),
                        Explicit = false,
                        DiscNumber = track.MediaPosition,
                        TrackNumber = track.Position,
                        Label = string.Join(";", release.Labels.Select(label => label.Name)),
                        ISRC = string.Empty,
                        Album = new SearchTrackAlbumEntity
                        {
                            Id = release.ReleaseId.ToString(),
                            ArtistId = release.ArtistId.ToString(),
                            Name = release.Title,
                            Type = string.Empty,
                            ReleaseDate = release.Date,
                            TotalTracks = release.TotalTracks,
                            Url = $"https://musicbrainz.org/release/{release.ReleaseId}",
                            Label = string.Empty,
                            Popularity = 0,
                            UPC = release.Barcode,
                            ProviderType = ProviderType.MusicBrainz
                        },
                        Artists = track.TrackArtists?.Select(trackArtist => new SearchTrackArtistEntity
                        {
                            Id = trackArtist.ArtistId.ToString(),
                            Name = trackArtist.Name
                        }).ToList(),
                        MusicBrainz = new SearchTrackMusicBrainzEntity
                        {
                            ArtistId = artist.ArtistId.ToString(),
                            RecordingId = track.RecordingId.ToString(),
                            RecordingTrackId = track.RecordingTrackId.ToString(),
                            ReleaseTrackId = track.ReleaseTrackId.ToString(),
                            ReleaseArtistId = release.ArtistId.ToString(),
                            ReleaseGroupId = string.Empty,
                            ReleaseId = release.ReleaseId.ToString(),
                            AlbumType = string.Empty,
                            AlbumReleaseCountry = release.Country,
                            AlbumStatus = release.Status,
                            TrackMediaFormat = track.MediaFormat
                        }
                    }));
                }
            }
        }
        if (provider is ProviderType.Any or ProviderType.Deezer && long.TryParse(artistId, out long longArtistId))
        {
            var deezerTracks = await _deezerRepository.SearchTrackByArtistIdAsync(trackName, longArtistId, offset);
            searchResult.AddRange(deezerTracks.Select(track => new SearchTrackEntity
            {
                ProviderType = ProviderType.Deezer,
                Id = track.TrackId.ToString(),
                Name = track.Title,
                Popularity = 0,
                Url = $"https://www.deezer.com/track/{track.TrackId}",
                Duration = TimeSpan.FromSeconds(track.Duration),
                Explicit = track.ExplicitLyrics,
                DiscNumber = track.DiskNumber,
                TrackNumber = track.TrackPosition,
                Label = track.Album.Label,
                ISRC = track.ISRC,
                
                Album = new SearchTrackAlbumEntity
                {
                    Id = track.Album.AlbumId.ToString(),
                    ArtistId = track.Album.ArtistId.ToString(),
                    Name = track.Album.Title,
                    Type = track.Album.Type,
                    ReleaseDate = track.Album.ReleaseDate,
                    TotalTracks = track.Album.NbTracks,
                    Url = $"https://www.deezer.com/album/{track.AlbumId}",
                    Label = track.Album.Label,
                    Popularity = track.Album.Fans,
                    UPC = track.Album.UPC,
                    ProviderType = ProviderType.Tidal
                },
                Artists = track.Artists?.Select(artist => new SearchTrackArtistEntity
                {
                    Id = artist.ArtistId.ToString(),
                    Name = artist.ArtistName
                }).ToList()
            }));
        }

        response.SearchResult = searchResult.Any() ? SearchResultType.Ok : SearchResultType.NotFound;
        response.Tracks = searchResult;
        Debug.WriteLine($"Tracks found: {searchResult.Count}");

        return response;
    }
    
    
    public async Task<SearchTrackResponse> SearchTrack(
        string id,
        ProviderType provider)
    {
        SearchTrackResponse response = new SearchTrackResponse();
        List<SearchTrackEntity> searchResult = new List<SearchTrackEntity>();

        /*if (provider is ProviderType.Spotify)
        {
            var spotifyAlbum = await _spotifyRepository.GetAlbumByIdAsync(id);
            if (spotifyAlbum != null)
            {
                searchResult.Add(new SearchTrackEntity
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
                    Images = spotifyAlbum.Images?.Select(image => new  SearchTrackImageEntity
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Url = image.Url
                    }).ToList(),
                    Artists = spotifyAlbum.Artists?.Select(artist => new SearchTrackArtistEntity
                    {
                        Id = artist.ArtistId,
                        Name = artist.ArtistName
                    }).ToList()
                });
            }
            
        }
        if (provider is ProviderType.Tidal)
        {
            var tidalArtist = await _tidalRepository.GetArtistByIdAsync(id);
            if (tidalArtist != null)
            {
                searchResult.Add(new SearchAlbumEntity
                {
                    ProviderType = ProviderType.Tidal,
                    Id = tidalArtist.ArtistId.ToString(),
                    Name = tidalArtist.Name,
                    Popularity = tidalArtist.Popularity,
                    Url = string.Empty,
                    Images = tidalArtist.Images?.Select(image => new  SearchAlbumImageEntity
                    {
                        Width = image.Meta_Width,
                        Height = image.Meta_Height,
                        Url = image.Href
                    }).ToList()
                });
            }
        }
        if (provider is ProviderType.MusicBrainz)
        {
            var musicBrainzArtist = await _musicBrainzRepository.GetArtistByIdAsync(id);

            if (musicBrainzArtist != null)
            {
                searchResult.Add(new SearchAlbumEntity
                {
                    ProviderType = ProviderType.MusicBrainz,
                    Id = musicBrainzArtist.MusicBrainzRemoteId.ToString(),
                    Name = musicBrainzArtist.Name,
                    Popularity = 0,
                    Url = string.Empty,
                });
            }
        }*/

        response.SearchResult = searchResult.Any() ? SearchResultType.Ok : SearchResultType.NotFound;
        response.Tracks = searchResult;

        return response;
    }
}