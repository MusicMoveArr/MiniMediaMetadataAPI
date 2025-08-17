using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaMetadataAPI.Application.Configurations;
using MiniMediaMetadataAPI.Application.Models.Database.Tidal;
using Npgsql;

namespace MiniMediaMetadataAPI.Application.Repositories;

public class TidalRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public TidalRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<TidalArtistModel>?> SearchArtistAsync(string name, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT * 
                         FROM tidal_artist ta
                         left join tidal_artist_image_link tai on tai.artistid = ta.artistid
                         where lower(ta.name) % lower(@name)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<TidalArtistModel>? results = null;

        try
        {
            results = await conn
                .QueryAsync<TidalArtistModel, TidalArtistImageModel, TidalArtistModel>(query,
                    (artist, imageModel) =>
                    {
                        if (artist.Images == null)
                        {
                            artist.Images = new List<TidalArtistImageModel>();
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
                    },
                    transaction: transaction);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
        }
        finally
        {
            await transaction.CommitAsync();
        }

        var groupedResult = results
            ?.GroupBy(artist => artist.ArtistId)
            ?.Select(group =>
            {
                var artist = group.First();
                artist.Images = group.SelectMany(image => image.Images).ToList();
                return artist;
            })
            .ToList();

        return groupedResult;
    }
    
    public async Task<TidalArtistModel?> GetArtistByIdAsync(int id)
    {
        string query = @"SELECT * 
                         FROM tidal_artist 
                         where artistid = @id
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        return await conn
            .QueryFirstOrDefaultAsync<TidalArtistModel>(query,
                param: new
                {
                    id
                });
    }
    
    public async Task<List<TidalAlbumModel>?> SearchAlbumByArtistIdAsync(string albumName, int artistId, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT album.albumid,
                                album.artistid,
                                album.title,
                                album.barcodeid,
                                album.numberofvolumes,
                                album.numberofitems,
                                album.duration::interval as Duration,
                                album.explicit,
                                album.releasedate,
                                album.copyright,
                                album.popularity,
                                album.availability,
                                album.mediatags,
                                ta.*,
                                sai.*,
                                sae.*
                         FROM tidal_album album
                         join tidal_artist ta on ta.artistid = album.artistid
                         left join tidal_album_image_link sai on sai.albumid = album.albumid
                         left join tidal_album_external_link sae on sae.albumid = album.albumid
                         where 
                             album.artistid = @artistId
                             and lower(album.title) % lower(@albumName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<TidalAlbumModel>? results = null;
        
        try
        {
            results = await conn
                .QueryAsync<TidalAlbumModel, 
                    TidalArtistModel, 
                    TidalAlbumImageModel?, 
                    TidalAlbumExternalLinkModel?, 
                    TidalAlbumModel>(query,
                    (album, artist, imageModel, externalId) =>
                    {
                        if (album.Images == null)
                        {
                            album.Images = new List<TidalAlbumImageModel>();
                        }
                        if (album.ExternalLinks == null)
                        {
                            album.ExternalLinks = new List<TidalAlbumExternalLinkModel>();
                        }

                        album.Artist = artist;
                    
                        if (imageModel != null)
                        {
                            album.Images.Add(imageModel);
                        }

                        if (externalId != null)
                        {
                            album.ExternalLinks.Add(externalId);
                        }
                        return album;
                    },
                    splitOn: "albumid,artistid,albumid,albumid",
                    param: new
                    {
                        albumName,
                        artistId,
                        offset
                    },
                    transaction: transaction);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
        }
        finally
        {
            await transaction.CommitAsync();
        }

        var groupedResult = results
            ?.GroupBy(album => album.AlbumId)
            ?.Select(group =>
            {
                var album = group.First();
                
                album.Images = group
                    .SelectMany(album => album.Images)
                    .DistinctBy(image => image.Href)
                    .ToList();
                
                album.ExternalLinks = group
                    .SelectMany(album => album.ExternalLinks)
                    .DistinctBy(externalId => externalId.Href)
                    .ToList();
                return album;
            })
            .ToList();

        return groupedResult;
    }
    
    
    public async Task<TidalAlbumModel?> GetAlbumIdAsync(int albumId)
    {
        string query = @"SELECT album.albumid,
                                album.artistid,
                                album.title,
                                album.barcodeid,
                                album.numberofvolumes,
                                album.numberofitems,
                                album.duration::interval as Duration,
                                album.explicit,
                                album.releasedate,
                                album.copyright,
                                album.popularity,
                                album.availability,
                                album.mediatags,
                                ta.*,
                                sai.*,
                                sae.*
                         FROM tidal_album album
                         join tidal_artist ta on ta.artistid = album.artistid
                         left join tidal_album_image_link sai on sai.albumid = album.albumid
                         left join tidal_album_external_link sae on sae.albumid = album.albumid
                         where album.albumid = @albumId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<TidalAlbumModel, TidalArtistModel, TidalAlbumImageModel?, TidalAlbumExternalLinkModel?, TidalAlbumModel>(query,
                (album, artist, imageModel, externalId) =>
                {
                    if (album.Images == null)
                    {
                        album.Images = new List<TidalAlbumImageModel>();
                    }
                    if (album.ExternalLinks == null)
                    {
                        album.ExternalLinks = new List<TidalAlbumExternalLinkModel>();
                    }

                    album.Artist = artist;
                    
                    if (imageModel != null)
                    {
                        album.Images.Add(imageModel);
                    }

                    if (externalId != null)
                    {
                        album.ExternalLinks.Add(externalId);
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
                
                album.ExternalLinks = group
                    .SelectMany(album => album.ExternalLinks)
                    .DistinctBy(externalId => externalId.Href)
                    .ToList();
                return album;
            })
            .ToList();

        return groupedResult.FirstOrDefault();
    }
    
    public async Task<List<TidalTrackModel>?> SearchTrackByArtistIdAsync(string trackName, int artistId, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT tt.TrackId, 
                                tt.AlbumId, 
                                tt.Title, 
                                tt.ISRC,
                                tt.duration::interval as Duration,
                                tt.Copyright,
                                tt.Explicit,
                                tt.Popularity,
                                tt.Availability,
                                tt.MediaTags,
                                tt.VolumeNumber,
                                tt.TrackNumber,
                                tt.Version,
                                tte.*,
                                album.albumid,
                                album.artistid,
                                album.title,
                                album.barcodeid,
                                album.numberofvolumes,
                                album.numberofitems,
                                album.duration::interval as Duration,
                                album.explicit,
                                album.releasedate,
                                album.copyright,
                                album.popularity,
                                album.availability,
                                album.mediatags,
                                tae.*,
                                tta.*,
                                ta.name as ArtistName
                         FROM tidal_track tt
                         join tidal_album album on album.AlbumId = tt.albumid and album.artistid = @artistId
                         join tidal_track_artist tta on tta.trackid = tt.trackid
                         join tidal_artist ta on ta.artistid = tta.artistid
                         
                         join tidal_track_external_link tte on tte.trackid = tt.trackid
                         join tidal_album_external_link tae on tae.AlbumId = album.albumid
                         
                         where lower(tt.Title || ' ' || tt.version) % lower(@trackName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<TidalTrackModel>? results = null;

        try
        {
            results = await conn
                .QueryAsync<TidalTrackModel, 
                            TidalTrackExternalLinkModel, 
                            TidalAlbumModel, 
                            TidalAlbumExternalLinkModel, 
                            TidalTrackArtistModel, 
                            TidalTrackModel>(query,
                (track, trackExternalId, album, albumExternalId, trackArtist) =>
                {
                    if (track.ExternalLinks == null)
                    {
                        track.ExternalLinks = new List<TidalTrackExternalLinkModel>();
                    }
                    if (track.Images == null)
                    {
                        track.Images = new List<TidalTrackImageLinkModel>();
                    }
                    if (track.Artists == null)
                    {
                        track.Artists = new List<TidalTrackArtistModel>();
                    }
                    
                    track.Album = album;

                    if (track.Album != null)
                    {
                        track.Album.ExternalLinks = new List<TidalAlbumExternalLinkModel>();
                    }

                    if (albumExternalId != null)
                    {
                        track.Album.ExternalLinks.Add(albumExternalId);
                    }
                    if (trackExternalId != null)
                    {
                        track.ExternalLinks.Add(trackExternalId);
                    }
                    if (trackArtist != null)
                    {
                        track.Artists.Add(trackArtist);
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
                },
                transaction: transaction);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
        }
        finally
        {
            await transaction.CommitAsync();
        }

        var groupedResult = results
            ?.GroupBy(track => track.TrackId)
            ?.Select(group =>
            {
                var track = group.First();
                
                track.Artists = group
                    .SelectMany(album => album.Artists)
                    .DistinctBy(artist => artist.ArtistId)
                    .ToList();
                
                track.ExternalLinks = group
                    .SelectMany(album => album.ExternalLinks)
                    .DistinctBy(externalId => externalId.Href)
                    .ToList();
                
                track.Images = group
                    .SelectMany(album => album.Images)
                    .DistinctBy(externalId => externalId.Href)
                    .ToList();
                return track;
            })
            .ToList();

        return groupedResult;
    }
}