using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaMetadataAPI.Application.Configurations;
using MiniMediaMetadataAPI.Application.Models.Database.Spotify;
using Npgsql;

namespace MiniMediaMetadataAPI.Application.Repositories;

public class SpotifyRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public SpotifyRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<SpotifyArtistModel>> SearchArtistAsync(string name, int offset)
    {
        string query = @"SELECT * 
                         FROM spotify_artist sa
                         left join spotify_artist_image sai on sai.artistid = sa.id
                         where similarity(lower(sa.name), lower(@name)) >= 0.5";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<SpotifyArtistModel, SpotifyArtistImageModel, SpotifyArtistModel>(query,
                (artist, imageModel) =>
                {
                    if (artist.Images == null)
                    {
                        artist.Images = new List<SpotifyArtistImageModel>();
                    }

                    if (imageModel != null)
                    {
                        artist.Images.Add(imageModel);
                    }
                    return artist;
                },
                splitOn: "artistid",
                param: new
                {
                    name,
                    offset
                });

        var groupedResult = results
            .GroupBy(artist => artist.Id)
            .Select(group =>
            {
                var artist = group.First();
                artist.Images = group
                    .SelectMany(image => image.Images)
                    .ToList();
                return artist;
            })
            .ToList();

        return groupedResult;
    }
    
    public async Task<SpotifyArtistModel?> GetArtistByIdAsync(string id)
    {
        string query = @"SELECT * 
                         FROM spotify_artist sa
                         left join spotify_artist_image sai on sai.artistid = sa.id
                         where id = @id";
        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        var results = await conn
            .QueryAsync<SpotifyArtistModel, SpotifyArtistImageModel, SpotifyArtistModel>(query,
                (artist, imageModel) =>
                {
                    if (artist.Images == null)
                    {
                        artist.Images = new List<SpotifyArtistImageModel>();
                    }

                    if (imageModel != null)
                    {
                        artist.Images.Add(imageModel);
                    }
                    return artist;
                },
                splitOn: "artistid",
                param: new
                {
                    id
                });

        var groupedResult = results
            .GroupBy(artist => artist.Id)
            .Select(group =>
            {
                var artist = group.First();
                artist.Images = group
                    .SelectMany(image => image.Images)
                    .ToList();
                return artist;
            })
            .ToList();

        return groupedResult.FirstOrDefault();
    }
    
    
    public async Task<List<SpotifyAlbumModel>> SearchAlbumByArtistIdAsync(string albumName, string artistId, int offset)
    {
        string query = @"SELECT sa.*,
                                saa.*,
                                sa2.name as ArtistName,
                                sai.*,
                                sae.*
                         FROM spotify_album sa
                         join spotify_album_artist saa on saa.albumid = sa.albumid
                         join spotify_artist sa2 on sa2.id = saa.artistid
                         left join spotify_album_image sai on sai.albumid = sa.albumid
                         left join spotify_album_externalid sae on sae.albumid = sa.albumid
                         where similarity(lower(sa.name), lower(@albumName)) >= 0.5
                         and (exists (select 1 from spotify_album_artist saa2 
                                              where saa2.albumid = sa.albumid 
                                                and saa2.artistid = @artistId)
                             or sa.artistId = @artistId)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<SpotifyAlbumModel, SpotifyAlbumArtistModel, SpotifyAlbumImageModel, SpotifyAlbumExternalIdModel, SpotifyAlbumModel>(query,
                (album, artist, imageModel, externalId) =>
                {
                    if (album.Images == null)
                    {
                        album.Images = new List<SpotifyAlbumImageModel>();
                    }
                    if (album.Artists == null)
                    {
                        album.Artists = new List<SpotifyAlbumArtistModel>();
                    }
                    if (album.ExternalIds == null)
                    {
                        album.ExternalIds = new List<SpotifyAlbumExternalIdModel>();
                    }

                    if (imageModel != null)
                    {
                        album.Images.Add(imageModel);
                    }
                    if (artist != null)
                    {
                        album.Artists.Add(artist);
                    }
                    if (externalId != null)
                    {
                        album.ExternalIds.Add(externalId);
                    }
                    return album;
                },
                splitOn: "albumid,albumid,albumid",
                param: new
                {
                    albumName,
                    artistId,
                    offset
                });

        var groupedResult = results
            .GroupBy(album => album.AlbumId)
            .Select(group =>
            {
                var album = group.First();
                
                album.Images = group
                    .SelectMany(album => album.Images)
                    .DistinctBy(image => image.Url)
                    .ToList();
                
                album.Artists = group
                    .SelectMany(album => album.Artists)
                    .DistinctBy(artist => artist.ArtistId)
                    .ToList();
                
                album.ExternalIds = group
                    .SelectMany(album => album.ExternalIds)
                    .DistinctBy(externalId => externalId.Name)
                    .ToList();
                return album;
            })
            .ToList();

        return groupedResult;
    }
    
    
    public async Task<SpotifyAlbumModel?> GetAlbumByIdAsync(string albumId)
    {
        string query = @"SELECT sa.*,
                                saa.*,
                                sa2.name as ArtistName,
                                sai.*,
                                sae.*
                         FROM spotify_album sa
                         join spotify_album_artist saa on saa.albumid = sa.albumid
                         join spotify_artist sa2 on sa2.id = saa.artistid
                         left join spotify_album_image sai on sai.albumid = sa.albumid
                         left join spotify_album_externalid sae on sae.albumid = sa.albumid
                         where sa.albumid = @albumId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<SpotifyAlbumModel, SpotifyAlbumArtistModel, SpotifyAlbumImageModel, SpotifyAlbumExternalIdModel, SpotifyAlbumModel>(query,
                (album, artist, imageModel, externalId) =>
                {
                    if (album.Images == null)
                    {
                        album.Images = new List<SpotifyAlbumImageModel>();
                    }
                    if (album.Artists == null)
                    {
                        album.Artists = new List<SpotifyAlbumArtistModel>();
                    }
                    if (album.ExternalIds == null)
                    {
                        album.ExternalIds = new List<SpotifyAlbumExternalIdModel>();
                    }

                    if (imageModel != null)
                    {
                        album.Images.Add(imageModel);
                    }
                    if (artist != null)
                    {
                        album.Artists.Add(artist);
                    }
                    if (externalId != null)
                    {
                        album.ExternalIds.Add(externalId);
                    }
                    return album;
                },
                splitOn: "albumid,albumid,albumid",
                param: new
                {
                    albumId
                });

        var groupedResult = results
            .GroupBy(album => album.AlbumId)
            .Select(group =>
            {
                var album = group.First();
                
                album.Images = group
                    .SelectMany(album => album.Images)
                    .DistinctBy(image => image.Url)
                    .ToList();
                
                album.Artists = group
                    .SelectMany(album => album.Artists)
                    .DistinctBy(artist => artist.ArtistId)
                    .ToList();
                
                album.ExternalIds = group
                    .SelectMany(album => album.ExternalIds)
                    .DistinctBy(externalId => externalId.Name)
                    .ToList();
                return album;
            })
            .ToList();

        return groupedResult.FirstOrDefault();
    }
    
    public async Task<List<SpotifyTrackModel>> SearchTrackByArtistIdAsync(string trackName, string artistId, int offset)
    {
        string query = @"SELECT st.TrackId, 
                                st.AlbumId, 
                                st.DiscNumber, 
                                to_timestamp(st.durationms / 1000.0)::time as Duration,
                                st.Explicit,
                                st.Href,
                                st.IsPlayable,
                                st.Name,
                                st.PreviewUrl,
                                st.TrackNumber,
                                st.Type,
                                st.Uri,
                                ste.*,
                                album.*,
                                sae2.*,
                                sa.Id AS ArtistId,
                                st.TrackId,
                                sa.Name AS ArtistName
                         FROM spotify_track st
                         join spotify_track_artist sta on sta.trackid = st.trackid
                         join spotify_artist sa on sa.id = sta.artistid
                         
                         join spotify_track_externalid ste on ste.trackid = st.trackid
                         join spotify_album album on album.AlbumId = st.albumid
                         join spotify_album_externalid sae2 on sae2.AlbumId = album.albumid
                         
                         where similarity(lower(st.Name), lower(@trackName)) >= 0.5
                         and (exists (select 1 from spotify_track_artist sta2 
                                              where sta2.trackid = st.trackid 
                                                and sta2.artistid = @artistId)
                             or album.artistid = @artistId)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<SpotifyTrackModel, 
                        SpotifyTrackExternalIdModel, 
                        SpotifyAlbumModel, 
                        SpotifyAlbumExternalIdModel, 
                        SpotifyTrackArtistModel, 
                        SpotifyTrackModel>(query,
                (track, trackExternalId, album, albumExternalId, trackArtist) =>
                {
                    if (track.ExternalIds == null)
                    {
                        track.ExternalIds = new List<SpotifyTrackExternalIdModel>();
                    }
                    if (track.Artists == null)
                    {
                        track.Artists = new List<SpotifyTrackArtistModel>();
                    }
                    
                    track.Album = album;

                    if (track.Album != null)
                    {
                        track.Album.ExternalIds = new List<SpotifyAlbumExternalIdModel>();
                    }

                    if (track.Album.ExternalIds != null && albumExternalId != null)
                    {
                        track.Album.ExternalIds.Add(albumExternalId);
                    }
                    if (trackExternalId != null)
                    {
                        track.ExternalIds.Add(trackExternalId);
                    }
                    if (trackArtist != null)
                    {
                        track.Artists.Add(trackArtist);
                    }
                    return track;
                },
                splitOn: "trackid,trackid,albumid,albumid,ArtistId",
                param: new
                {
                    trackName,
                    artistId,
                    offset
                });

        var groupedResult = results
            .GroupBy(track => track.TrackId)
            .Select(group =>
            {
                var track = group.First();
                
                track.Artists = group
                    .SelectMany(album => album.Artists)
                    .DistinctBy(artist => artist.ArtistId)
                    .ToList();
                
                track.ExternalIds = group
                    .SelectMany(album => album.ExternalIds)
                    .DistinctBy(externalId => externalId.Name)
                    .ToList();
                return track;
            })
            .ToList();

        return groupedResult;
    }
}