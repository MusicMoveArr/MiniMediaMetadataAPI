using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaMetadataAPI.Application.Configurations;
using MiniMediaMetadataAPI.Application.Models.Database.Deezer;
using Npgsql;

namespace MiniMediaMetadataAPI.Application.Repositories;

public class DeezerRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public DeezerRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<DeezerArtistModel>> SearchArtistAsync(string name, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT *
                         FROM deezer_artist da
                         JOIN deezer_artist_image_link dai ON dai.artistid = da.artistid
                         where lower(da.name) % lower(@name)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        var results = await conn
            .QueryAsync<DeezerArtistModel, DeezerArtistImageLinkModel, DeezerArtistModel>(query,
                (artist, imageModel) =>
                {
                    if (artist.Images == null)
                    {
                        artist.Images = new List<DeezerArtistImageLinkModel>();
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
            .GroupBy(artist => artist.ArtistId)
            .Select(group =>
            {
                var artist = group.First();
                artist.Images = group.SelectMany(image => image.Images).ToList();
                return artist;
            })
            .ToList();

        return groupedResult;
    }
    
    public async Task<DeezerArtistModel?> GetArtistByIdAsync(long id)
    {
        string query = @"SELECT *
                         FROM deezer_artist da
                         JOIN deezer_artist_image_link dai ON dai.artistid = da.artistid
                         where da.artistid = @id
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        var results = await conn
            .QueryAsync<DeezerArtistModel, DeezerArtistImageLinkModel, DeezerArtistModel>(query,
                (artist, imageModel) =>
                {
                    if (artist.Images == null)
                    {
                        artist.Images = new List<DeezerArtistImageLinkModel>();
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
            .GroupBy(artist => artist.ArtistId)
            .Select(group =>
            {
                var artist = group.First();
                artist.Images = group.SelectMany(image => image.Images).ToList();
                return artist;
            })
            .ToList();

        return groupedResult.FirstOrDefault();
    }
    
    public async Task<List<DeezerAlbumModel>> SearchAlbumByArtistIdAsync(string albumName, long artistId, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT album.albumid,
                                album.artistid,
                                album.title,
                                album.md5image,
                                album.genreid,
                                album.fans,
                                album.releasedate,
                                album.recordtype,
                                album.explicitlyrics,
                                album.explicitcontentlyrics,
                                album.explicitcontentcover,
                                album.type,
                                album.upc,
                                album.label,
                                album.nbtracks,
                                album.duration,
                                album.available,
                                da.*,
                                dai.*,
                                daa.*,
                                album_artist.name AS ArtistName
                         FROM deezer_album album
                         join deezer_artist da on da.artistid = album.artistid
                         left join deezer_album_image_link dai on dai.albumid = album.albumid
                         left join deezer_album_artist daa on daa.albumid = album.albumid
                         left join deezer_artist album_artist on album_artist.artistid = daa.artistid
                         where 
                             album.artistid = @artistId
                             and lower(album.title) % lower(@albumName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<DeezerAlbumModel, DeezerArtistModel, DeezerAlbumImageLinkModel?, DeezerAlbumArtistModel?, DeezerAlbumModel>(query,
                (album, artist, imageModel, albumArtist) =>
                {
                    album.Artist = artist;
                    
                    if (imageModel != null)
                    {
                        album.Images.Add(imageModel);
                    }
                    if (albumArtist != null)
                    {
                        album.Artists.Add(albumArtist);
                    }
                    return album;
                },
                splitOn: "albumid,artistid,albumid,albumid",
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
                    .DistinctBy(image => image.Href)
                    .ToList();

                album.Artists = group
                    .SelectMany(album => album.Artists)
                    .DistinctBy(image => image.ArtistId)
                    .ToList();

                return album;
            })
            .ToList();

        return groupedResult;
    }
    
    
    public async Task<DeezerAlbumModel?> GetAlbumIdAsync(long albumId)
    {
        string query = @"SELECT album.albumid,
                                album.artistid,
                                album.title,
                                album.md5image,
                                album.genreid,
                                album.fans,
                                album.releasedate,
                                album.recordtype,
                                album.explicitlyrics,
                                album.explicitcontentlyrics,
                                album.explicitcontentcover,
                                album.type,
                                album.upc,
                                album.label,
                                album.nbtracks,
                                album.duration,
                                album.available,
                                da.*,
                                dai.*,
                                daa.*,
                                album_artist.name AS ArtistName
                         FROM deezer_album album
                         join deezer_artist da on da.artistid = album.artistid
                         left join deezer_album_image_link dai on dai.albumid = album.albumid
                         left join deezer_album_artist daa on daa.albumid = album.albumid
                         left join deezer_artist album_artist on album_artist.artistid = daa.artistid
                         where album.albumid = @albumId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<DeezerAlbumModel, DeezerArtistModel, DeezerAlbumImageLinkModel?, DeezerAlbumArtistModel?, DeezerAlbumModel>(query,
                (album, artist, imageModel, albumArtist) =>
                {
                    album.Artist = artist;
                    
                    if (imageModel != null)
                    {
                        album.Images.Add(imageModel);
                    }
                    if (albumArtist != null)
                    {
                        album.Artists.Add(albumArtist);
                    }
                    return album;
                },
                splitOn: "albumid,artistid,albumid,albumid",
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
                    .DistinctBy(image => image.Href)
                    .ToList();

                album.Artists = group
                    .SelectMany(album => album.Artists)
                    .DistinctBy(image => image.ArtistId)
                    .ToList();

                return album;
            })
            .ToList();

        return groupedResult.FirstOrDefault();
    }
    
    public async Task<List<DeezerTrackModel>> SearchTrackByArtistIdAsync(string trackName, long artistId, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT dt.TrackId, 
                                dt.Readable, 
                                dt.Title, 
                                dt.TitleShort,
                                dt.TitleVersion,
                                dt.ISRC,
                                dt.duration,
                                dt.TrackPosition,
                                dt.DiskNumber,
                                dt.Rank,
                                dt.ReleaseDate,
                                dt.ExplicitLyrics,
                                dt.ExplicitContentLyrics,
                                dt.ExplicitContentCover,
                                dt.Preview,
                                dt.BPM,
                                dt.Gain,
                                dt.Md5Image,
                                dt.TrackToken,
                                dt.ArtistId,
                                dt.AlbumId,
                                dt.Type,
                                album.AlbumId,
                                album.ArtistId,
                                album.Title,
                                album.Md5Image,
                                album.GenreId,
                                album.Fans,
                                album.ReleaseDate,
                                album.RecordType,
                                album.ExplicitLyrics,
                                album.ExplicitContentLyrics,
                                album.ExplicitContentCover,
                                album.Type,
                                album.UPC,
                                album.Label,
                                album.NbTracks,
                                album.duration,
                                album.Available,
                                dta.*,
                                da.name as ArtistName
                         FROM deezer_track dt
                         join deezer_album album on album.AlbumId = dt.albumid and album.artistid = @artistId
                         join deezer_track_artist dta on dta.trackid = dt.trackid
                         join deezer_artist da on da.artistid = dta.artistid
                         where lower(dt.Title) % lower(@trackName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<DeezerTrackModel, 
                        DeezerAlbumModel, 
                        DeezerTrackArtistModel, 
                        DeezerTrackModel>(query,
                (track, album, trackArtist) =>
                {
                    if (track.Artists == null)
                    {
                        track.Artists = new List<DeezerTrackArtistModel>();
                    }
                    
                    track.Album = album;

                    if (trackArtist != null)
                    {
                        track.Artists.Add(trackArtist);
                    }
                    return track;
                },
                splitOn: "trackid,albumid,ArtistId",
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
                
                return track;
            })
            .ToList();

        return groupedResult;
    }
}