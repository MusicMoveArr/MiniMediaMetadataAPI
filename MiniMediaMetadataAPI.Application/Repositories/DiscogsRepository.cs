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
                         SELECT da.artistid, regexp_replace(da.name, ' \([0-9]*\)$', '' ) as name, da.realname, da.profile, da.dataquality, da.lastsynctime
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
        string query = @"SELECT da.artistid, regexp_replace(da.name, ' \([0-9]*\)$', '' ) as name, da.realname, da.profile, da.dataquality, da.lastsynctime
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
                                regexp_replace(da.name, ' \([0-9]*\)$', '' ) AS Name,
                                dri.Releaseid,
                                dri.Description,
                                dri.Type,
                                dri.Value
                         FROM discogs_release album
                         join discogs_release_artist dra on dra.releaseid = album.releaseid and dra.artistid = @artistId
                         join discogs_artist da on da.artistid = dra.artistid
                         join lateral (
 	                        select count(track.releaseid) as TrackCount 
 	                        from discogs_release_track track 
 	                        where track.releaseid = album.releaseid) track on 1=1
                         left join discogs_release_identifier dri on dri.releaseid = album.releaseid
                         where 
                             lower(album.title) % lower(@albumName)";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        await conn.OpenAsync();
        var transaction = await conn.BeginTransactionAsync();
        IEnumerable<DiscogsRelease>? results = null;

        try
        {
            results = await conn
                .QueryAsync<DiscogsRelease, DiscogsReleaseArtist, DiscogsReleaseIdentifier, DiscogsRelease>(query,
                    (album, artist, identifier) =>
                    {
                        if (artist != null)
                        {
                            album.Artists.Add(artist);
                        }
                        if (identifier != null)
                        {
                            album.Identifiers.Add(identifier);
                        }
                        return album;
                    },
                    splitOn: "releaseid,releaseid,releaseid",
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

                album.Identifiers = group
                    .SelectMany(album => album.Identifiers)
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
                                regexp_replace(da.name, ' \([0-9]*\)$', '' ) AS Name,
                                dri.Releaseid,
                                dri.Description,
                                dri.Type,
                                dri.Value
                         FROM discogs_release album
                         join discogs_release_artist dra on dra.releaseid = album.releaseid
                         join discogs_artist da on da.artistid = dra.artistid
                         join lateral (
 	                        select count(track.releaseid) as TrackCount 
 	                        from discogs_release_track track 
 	                        where track.releaseid = album.releaseid) track on 1=1
                         left join discogs_release_identifier dri on dri.releaseid = album.releaseid
                         where album.releaseid = @albumId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<DiscogsRelease, DiscogsReleaseArtist, DiscogsReleaseIdentifier, DiscogsRelease>(query,
                (album, artist, identifier) =>
                {
                    if (artist != null)
                    {
                        album.Artists.Add(artist);
                    }
                    if (identifier != null)
                    {
                        album.Identifiers.Add(identifier);
                    }
                    return album;
                },
                splitOn: "releaseid,releaseid,releaseid",
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
                    .DistinctBy(artist => artist.ArtistId)
                    .ToList();
                
                album.Identifiers = group
                    .SelectMany(album => album.Identifiers)
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
                                regexp_replace(da.name, ' \([0-9]*\)$', '' ) as Name,
                                dri.Releaseid,
                                dri.Description,
                                dri.Type,
                                dri.Value
                         FROM discogs_release_track dt
                         join discogs_release album on album.ReleaseId = dt.ReleaseId
                         join lateral (
 	                        select count(track.releaseid) as TrackCount 
 	                        from discogs_release_track track 
 	                        where track.releaseid = album.releaseid) track on 1=1
                         join discogs_release_artist release_dra on release_dra.ReleaseId = album.ReleaseId and release_dra.artistid = @artistId 
                         join discogs_release_artist dra on dra.ReleaseId = album.ReleaseId
                         join discogs_artist da on da.artistid = dra.artistid
                         left join discogs_release_identifier dri on dri.releaseid = album.releaseid
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
                    DiscogsReleaseIdentifier,
                    DiscogsReleaseTrack>(query,
                    (track, release, releaseArtist, identifier) =>
                    {
                        track.Release = release;

                        if (releaseArtist != null)
                        {
                            track.Release.Artists.Add(releaseArtist);
                        }
                        if (identifier != null)
                        {
                            track.Release.Identifiers.Add(identifier);
                        }
                        return track;
                    },
                    splitOn: "ReleaseId, ReleaseId, ReleaseId",
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
                
                track.Release.Identifiers = group
                    .SelectMany(album => album.Release.Identifiers)
                    .ToList();
                
                return track;
            })
            .ToList();

        return groupedResult;
    }
}