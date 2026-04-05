using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaMetadataAPI.Application.Configurations;
using MiniMediaMetadataAPI.Application.Models.Database.SoundCloud;
using MiniMediaMetadataAPI.Application.Models.Database.Tidal;
using Npgsql;

namespace MiniMediaMetadataAPI.Application.Repositories;

public class SoundCloudRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public SoundCloudRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<SoundCloudArtistModel>?> SearchArtistAsync(string name, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT * 
                         FROM soundcloud_user su
                         where lower(su.title) % lower(@name)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<SoundCloudArtistModel>? results = null;

        try
        {
            results = await conn
                .QueryAsync<SoundCloudArtistModel>(query,
                    param: new
                    {
                        name,
                        offset
                    },
                    transaction: transaction);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parameters, name='{name}', offset='{offset}', Error={ex.Message}\r\nStackTrace={ex.StackTrace}");
        }
        finally
        {
            await transaction.CommitAsync();
        }

        return results?.ToList();
    }
    
    public async Task<SoundCloudArtistModel?> GetArtistByIdAsync(int id)
    {
        string query = @"SELECT * 
                         FROM soundcloud_user 
                         where id = @id
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        return await conn
            .QueryFirstOrDefaultAsync<SoundCloudArtistModel>(query,
                param: new
                {
                    id
                });
    }
    
    public async Task<List<SoundCloudAlbumModel>?> SearchAlbumByArtistIdAsync(string albumName, long artistId, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT pl.*,
                                u.*
                         from soundcloud_user u
                         join soundcloud_playlist pl on pl.userid = u.id
                         where 
                             u.Id = @artistId
                             and lower(pl.title) % lower(@albumName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<SoundCloudAlbumModel>? results = null;
        
        try
        {
            results = await conn
                .QueryAsync<SoundCloudAlbumModel, 
                    SoundCloudArtistModel, 
                    SoundCloudAlbumModel>(query,
                    (album, artist) =>
                    {
                        album.Artist = artist;
                        return album;
                    },
                    splitOn: "Id, Id",
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
            Console.WriteLine($"Parameters, albumName='{albumName}', artistId='{artistId}', offset='{offset}', Error={ex.Message}\r\nStackTrace={ex.StackTrace}");
        }
        finally
        {
            await transaction.CommitAsync();
        }
        return results?.ToList();
    }
    
    
    public async Task<SoundCloudAlbumModel?> GetAlbumIdAsync(long albumId)
    {
        string query = @"SELECT pl.*,
                                u.*
                         from soundcloud_playlist pl
                         join soundcloud_user u on u.id = pl.userid
                         where 
                             pl.Id = @albumId
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<SoundCloudAlbumModel, SoundCloudArtistModel, SoundCloudAlbumModel>(query,
                (album, artist) =>
                {
                    album.Artist = artist;
                    return album;
                },
                splitOn: "Id, Id",
                param: new
                {
                    albumId
                });

        return results.FirstOrDefault();
    }
    
    public async Task<List<SoundCloudTrackModel>?> SearchTrackByArtistIdAsync(string trackName, long artistId, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT track.Id, 
                                spt.UserId, 
                                track.Title, 
                                playlist.Title as PlaylistName,
                                track.Caption,
                                track.Commentable,
                                track.CommentCount,
                                track.Description,
                                track.Downloadable,
                                track.DownloadCount,
                                track.Duration,
                                track.FullDuration,
                                track.EmbeddableBy,
                                track.Genre,
                                track.HasDownloadsLeft,
                                track.LabelName,
                                track.License,
                                track.LikesCount,
                                track.Permalink,
                                track.PermalinkUrl,
                                track.PlaybackCount,
                                track.Public,
                                track.PublisherMetadata_Artist,
                                track.PublisherMetadata_ContainsMusic,
                                track.PublisherMetadata_Id,
                                track.PublisherMetadata_Urn,
                                track.PurchaseTitle,
                                track.PurchaseUrl,
                                track.ReleaseDate,
                                track.RepostsCount,
                                track.Sharing,
                                track.State,
                                track.Streamable,
                                track.TagList,
                                track.Uri,
                                track.ArtworkUrl,
                                track.Visuals,
                                track.WaveformUrl,
                                track.DisplayDate,
                                track.MonetizationModel,
                                track.Policy,
                                track.Urn,
                                track.CreatedAt,
                                track.LastModified,
                                playlist.*,
                                u.*
                         FROM soundcloud_track track
                         join soundcloud_playlist_track spt on spt.trackId = track.Id and spt.UserId = @artistId
                         join soundcloud_playlist playlist on playlist.Id = spt.PlaylistId and playlist.UserId = @artistId
                         join soundcloud_user u on u.Id = playlist.UserId
                         where lower(track.Title) % lower(@trackName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<SoundCloudTrackModel>? results = null;

        try
        {
            results = await conn
                .QueryAsync<SoundCloudTrackModel, 
                            SoundCloudAlbumModel, 
                            SoundCloudArtistModel, 
                            SoundCloudTrackModel>(query,
                (track, album, trackArtist) =>
                {
                    track.Album = album;
                    track.Album.Artist = trackArtist;
                    track.Artists.Add(trackArtist);
                    return track;
                },
                splitOn: "Id, Id, Id",
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
            Console.WriteLine($"Parameters, trackName='{trackName}', artistId='{artistId}', offset='{offset}', Error={ex.Message}\r\nStackTrace={ex.StackTrace}");
        }
        finally
        {
            await transaction.CommitAsync();
        }
        
        return results?.ToList();
    }
    
    public async Task<List<SoundCloudTrackModel>?> SearchTrackByTrackIdAsync(long trackId)
    {
        string query = @"SELECT track.Id, 
                                spt.UserId, 
                                track.Title, 
                                playlist.Title as PlaylistName,
                                track.Caption,
                                track.Commentable,
                                track.CommentCount,
                                track.Description,
                                track.Downloadable,
                                track.DownloadCount,
                                track.Duration,
                                track.FullDuration,
                                track.EmbeddableBy,
                                track.Genre,
                                track.HasDownloadsLeft,
                                track.LabelName,
                                track.License,
                                track.LikesCount,
                                track.Permalink,
                                track.PermalinkUrl,
                                track.PlaybackCount,
                                track.Public,
                                track.PublisherMetadata_Artist,
                                track.PublisherMetadata_ContainsMusic,
                                track.PublisherMetadata_Id,
                                track.PublisherMetadata_Urn,
                                track.PurchaseTitle,
                                track.PurchaseUrl,
                                track.ReleaseDate,
                                track.RepostsCount,
                                track.Sharing,
                                track.State,
                                track.Streamable,
                                track.TagList,
                                track.Uri,
                                track.ArtworkUrl,
                                track.Visuals,
                                track.WaveformUrl,
                                track.DisplayDate,
                                track.MonetizationModel,
                                track.Policy,
                                track.Urn,
                                track.CreatedAt,
                                track.LastModified,
                                playlist.*,
                                u.*
                         FROM soundcloud_track track
                         join soundcloud_playlist_track spt on spt.trackId = track.Id
                         join soundcloud_playlist playlist on playlist.Id = spt.PlaylistId and playlist.UserId = spt.UserId
                         join soundcloud_user u on u.Id = playlist.UserId
                         where track.Id = @trackId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<SoundCloudTrackModel>? results = null;

        try
        {
            results = await conn
                .QueryAsync<SoundCloudTrackModel, 
                            SoundCloudAlbumModel, 
                            SoundCloudArtistModel, 
                            SoundCloudTrackModel>(query,
                (track, album, trackArtist) =>
                {
                    track.Album = album;
                    track.Album.Artist = trackArtist;
                    track.Artists.Add(trackArtist);
                    return track;
                },
                splitOn: "Id, Id, Id",
                param: new
                {
                    trackId
                },
                transaction: transaction);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parameters, trackId='{trackId}', Error={ex.Message}\r\nStackTrace={ex.StackTrace}");
        }
        finally
        {
            await transaction.CommitAsync();
        }

        return results?.ToList();
    }
}