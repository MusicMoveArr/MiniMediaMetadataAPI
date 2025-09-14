using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaMetadataAPI.Application.Configurations;
using MiniMediaMetadataAPI.Application.Models.Database.Discogs;
using Npgsql;

namespace MiniMediaMetadataAPI.Application.Repositories;

public class DiscogsRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public DiscogsRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<DiscogsArtist>?> SearchArtistAsync(string name, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT *
                         FROM discogs_artist da
                         where lower(da.name) % lower(@name)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        List<DiscogsArtist>? results = null;

        try
        {
            results = (await conn
                .QueryAsync<DiscogsArtist>(query,
                    param: new
                    {
                        name,
                        offset
                    })).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parameters, Name='{name}', offset='{offset}', Error={ex.Message}\r\nStackTrace={ex.StackTrace}");
        }
        finally
        {
            await transaction.CommitAsync();
        }

        return results;
    }
    
    public async Task<DiscogsArtist?> GetArtistByIdAsync(long id)
    {
        string query = @"SELECT *
                         FROM discogs_artist da
                         where da.artistid = @id
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        var results = await conn
            .QueryAsync<DiscogsArtist>(query,
                param: new
                {
                    id
                });

        return results.FirstOrDefault();
    }
    
    public async Task<List<DiscogsRelease>?> SearchAlbumByArtistIdAsync(string albumName, int artistId, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT album.releaseid,
                                album.status,
                                album.title,
                                album.country,
                                album.released,
                                album.dataquality,
                                album.ismainrelease,
                                album.masterid,
                                track.TrackCount,
                                dra.*,
                                da.name AS Name
                         FROM discogs_release album
                         join discogs_release_artist dra on dra.releaseid = album.releaseid and dra.artistid = @artistId
                         join discogs_artist da on da.artistid = dra.artistid
                         join lateral (
 	                        select count(track.releaseid) as TrackCount 
 	                        from discogs_release_track track 
 	                        where track.releaseid = album.releaseid) track on 1=1
                         where 
                             lower(album.title) % lower(@albumName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<DiscogsRelease>? results = null;

        try
        {
            results = await conn
                .QueryAsync<DiscogsRelease, DiscogsReleaseArtist, DiscogsRelease>(query,
                    (album, artist) =>
                    {
                        if (artist != null)
                        {
                            album.Artists.Add(artist);
                        }
                        return album;
                    },
                    splitOn: "releaseid,releaseid",
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

        var groupedResult = results
            ?.GroupBy(album => album.ReleaseId)
            ?.Select(group =>
            {
                var album = group.First();

                album.Artists = group
                    .SelectMany(album => album.Artists)
                    .DistinctBy(image => image.ArtistId)
                    .ToList();

                return album;
            })
            .ToList();

        return groupedResult;
    }
    
    
    public async Task<DiscogsRelease?> GetAlbumIdAsync(int albumId)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT album.releaseid,
                                album.status,
                                album.title,
                                album.country,
                                album.released,
                                album.dataquality,
                                album.ismainrelease,
                                album.masterid,
                                track.TrackCount,
                                dra.*,
                                da.name AS Name
                         FROM discogs_release album
                         join discogs_release_artist dra on dra.releaseid = album.releaseid
                         join discogs_artist da on da.artistid = dra.artistid
                         join lateral (
 	                        select count(track.releaseid) as TrackCount 
 	                        from discogs_release_track track 
 	                        where track.releaseid = album.releaseid) track on 1=1
                         where album.releaseid = @albumId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<DiscogsRelease, DiscogsReleaseArtist, DiscogsRelease>(query,
                (album, artist) =>
                {
                    if (artist != null)
                    {
                        album.Artists.Add(artist);
                    }
                    return album;
                },
                splitOn: "releaseid,releaseid",
                param: new
                {
                    albumId
                });

        var groupedResult = results
            .GroupBy(album => album.ReleaseId)
            .Select(group =>
            {
                var album = group.First();
                
                album.Artists = group
                    .SelectMany(album => album.Artists)
                    .DistinctBy(image => image.ArtistId)
                    .ToList();

                return album;
            })
            .ToList();

        return groupedResult.FirstOrDefault();
    }
    
    public async Task<List<DiscogsReleaseTrack>?> SearchTrackByArtistIdAsync(string trackName, int artistId, int offset)
    {
        string query = @"SET LOCAL pg_trgm.similarity_threshold = 0.5;
                         SELECT dt.ReleaseId, 
                                dt.Title, 
                                dt.Position, 
                                dt.Duration,
                                album.releaseid,
                                album.status,
                                album.title,
                                album.country,
                                album.released,
                                album.dataquality,
                                album.ismainrelease,
                                album.masterid,
                                track.TrackCount,
                                dra.*,
                                da.name as Name
                         FROM discogs_release_track dt
                         join discogs_release album on album.ReleaseId = dt.ReleaseId
                         join lateral (
 	                        select count(track.releaseid) as TrackCount 
 	                        from discogs_release_track track 
 	                        where track.releaseid = album.releaseid) track on 1=1
                         join discogs_release_artist release_dra on release_dra.ReleaseId = album.ReleaseId and release_dra.artistid = @artistId 
                         join discogs_release_artist dra on dra.ReleaseId = album.ReleaseId
                         join discogs_artist da on da.artistid = dra.artistid
                         where lower(dt.Title) % lower(@trackName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<DiscogsReleaseTrack>? results = null;

        try
        {
            results = await conn
                .QueryAsync<DiscogsReleaseTrack, 
                    DiscogsRelease, 
                    DiscogsReleaseArtist, 
                    DiscogsReleaseTrack>(query,
                    (track, release, releaseArtist) =>
                    {
                        track.Release = release;

                        if (releaseArtist != null)
                        {
                            track.Release.Artists.Add(releaseArtist);
                        }
                        return track;
                    },
                    splitOn: "ReleaseId, ReleaseId",
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

        var groupedResult = results
            ?.GroupBy(track => new
            {
                track.ReleaseId,
                track.Title,
                track.Position
            })
            ?.Select(group =>
            {
                var track = group.First();
                
                track.Release.Artists = group
                    .SelectMany(album => album.Release.Artists)
                    .DistinctBy(artist => artist.ArtistId)
                    .ToList();
                
                return track;
            })
            .ToList();

        return groupedResult;
    }
}