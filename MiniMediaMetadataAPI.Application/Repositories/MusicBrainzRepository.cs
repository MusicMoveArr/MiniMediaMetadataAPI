using Dapper;
using Microsoft.Extensions.Options;
using MiniMediaMetadataAPI.Application.Configurations;
using MiniMediaMetadataAPI.Application.Models.Database.MusicBrainz;
using Npgsql;

namespace MiniMediaMetadataAPI.Application.Repositories;

public class MusicBrainzRepository
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    public MusicBrainzRepository(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }
    
    public async Task<List<MusicBrainzArtistModel>> SearchArtistAsync(string name, int offset)
    {
        string query = @"SELECT * 
                         FROM MusicBrainz_Artist 
                         where similarity(lower(@name), lower(name)) >= 0.8";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
        
        return (await conn
                .QueryAsync<MusicBrainzArtistModel>(query,
                    param: new
                    {
                        name,
                        offset
                    }))
            .ToList();
    }
    
    public async Task<MusicBrainzArtistModel?> GetArtistByIdAsync(Guid id)
    {
        string query = @"SELECT * 
                         FROM MusicBrainz_Artist 
                         where ArtistId = @id
                         limit 1";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        return await conn
            .QueryFirstOrDefaultAsync<MusicBrainzArtistModel>(query,
                param: new
                {
                    id
                });
    }
    
    public async Task<List<MusicBrainzReleaseModel>> SearchAlbumByArtistIdAsync(string albumName, Guid artistId, int offset)
    {
        string query = @"SELECT release.*,
                                label.*
                         FROM MusicBrainz_Release release
                         left join MusicBrainz_Release_Label mbrl on mbrl.releaseid = release.releaseid
                         left join MusicBrainz_Label label on label.labelid = mbrl.labelid
                         where 
                             release.artistid = @artistId
                             and similarity(lower(release.title), lower(@albumName)) >= 0.5";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<MusicBrainzReleaseModel, MusicBrainzLabelModel?, MusicBrainzReleaseModel>(query,
                (release, label) =>
                {
                    if (release.Labels == null)
                    {
                        release.Labels = new List<MusicBrainzLabelModel>();
                    }

                    if (label != null)
                    {
                        release.Labels.Add(label);
                    }
                    
                    return release;
                },
                splitOn: "ReleaseId,LabelId",
                param: new
                {
                    albumName,
                    artistId,
                    offset
                });

        var groupedResult = results
            .GroupBy(album => album.ReleaseId)
            .Select(group =>
            {
                var album = group.First();
                
                album.Labels = group
                    .SelectMany(album => album.Labels)
                    .DistinctBy(label => label.LabelId)
                    .ToList();
                return album;
            })
            .ToList();

        return groupedResult;
    }
    
    public async Task<MusicBrainzReleaseModel?> GetAlbumByIdAsync(Guid releaseId)
    {
        string query = @"SELECT release.*,
                                label.*
                         FROM MusicBrainz_Release release
                         left join MusicBrainz_Release_Label mbrl on mbrl.releaseid = release.releaseid
                         left join MusicBrainz_Label label on label.labelid = mbrl.labelid
                         where release.ReleaseId = @releaseId";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<MusicBrainzReleaseModel, MusicBrainzLabelModel?, MusicBrainzReleaseModel>(query,
                (release, label) =>
                {
                    if (release.Labels == null)
                    {
                        release.Labels = new List<MusicBrainzLabelModel>();
                    }

                    if (label != null)
                    {
                        release.Labels.Add(label);
                    }
                    
                    return release;
                },
                splitOn: "ReleaseId,LabelId",
                param: new
                {
                    releaseId
                });

        var groupedResult = results
            .GroupBy(album => album.ReleaseId)
            .Select(group =>
            {
                var album = group.First();

                album.Labels = group
                    .SelectMany(album => album.Labels)
                    .DistinctBy(label => label.LabelId)
                    .ToList();
                return album;
            })
            .FirstOrDefault();

        return groupedResult;
    }
    
    public async Task<List<MusicBrainzArtistModel>> SearchTrackByArtistIdAsync(string trackName, Guid artistId, int offset)
    {
        string query = @"select track.ReleaseTrackId,
		                        track.RecordingTrackId,
		                        track.Title,
		                        track.Status,
		                        track.StatusId,
		                        track.Length,
		                        track.Number,
		                        track.Position,
		                        track.RecordingId,
		                        track.RecordingLength,
		                        track.RecordingTitle,
		                        track.RecordingVideo,
		                        track.MediaTrackCount,
		                        track.MediaFormat,
		                        track.MediaTitle,
		                        track.MediaPosition,
		                        track.MediaTrackOffset,
                                release.ReleaseId,
                                release.ArtistId,
                                release.Title,
                                release.Status,
                                release.StatusId,
                                release.Date,
                                release.Barcode,
                                release.Country,
                                release.Disambiguation,
                                release.Quality,
                                ta.ArtistId,
                                ta.ReleaseCount,
                                ta.Name,
                                ta.Type,
                                ta.Country,
                                ta.LastSyncTime,
                                rl.ReleaseId,
                                label.LabelId,
                                label.AreaId,
                                label.Name,
                                label.Disambiguation,
                                label.LabelCode,
                                label.Type,
                                label.LifeSpanBegin,
                                label.LifeSpanEnd,
                                label.LifeSpanEnded,
                                label.SortName,
                                label.TypeId,
                                label.Country,
                                ra.ArtistId,
                                ra.ReleaseCount,
                                ra.Name,
                                ra.Type,
                                ra.Country,
                                ra.LastSyncTime
                         from MusicBrainz_Release_Track track
                         join MusicBrainz_Release release on release.ReleaseId = track.ReleaseId and release.ArtistId = @artistId
                         join MusicBrainz_Release_Track_Artist rta on rta.releasetrackid = track.releasetrackid
                         join MusicBrainz_Artist ta on ta.artistid = rta.artistid
                         join MusicBrainz_Artist ra on ra.artistid = release.artistid
                         left join MusicBrainz_Release_Label rl on rl.releaseid = release.releaseid
                         left join MusicBrainz_Label label on label.LabelId = rl.labelid
                         where similarity(lower(track.Title), lower(@trackName)) >= 0.5";

        await using var conn = new NpgsqlConnection(_databaseConfiguration.ConnectionString);

        var results = await conn
            .QueryAsync<MusicBrainzReleaseTrackModel, 
                        MusicBrainzReleaseModel, 
                        MusicBrainzArtistModel, 
                        MusicBrainzLabelModel,
                        MusicBrainzArtistModel,
                        MusicBrainzArtistModel>(query,
                (track, release, trackArtist, label, releaseArtist) =>
                {
                    if (label != null)
                    {
                        release.Labels.Add(label);
                    }

                    if (track != null)
                    {
                        track.ReleaseId = release.ReleaseId;
                        if (trackArtist != null)
                        {
                            track.TrackArtists.Add(trackArtist);
                        }
                        release.Tracks.Add(track);
                    }

                    if (release != null)
                    {
                        releaseArtist.Releases.Add(release);
                    }
                    return releaseArtist;
                },
                splitOn: "ReleaseTrackId,ReleaseId,ArtistId,LabelId,ArtistId",
                param: new
                {
                    trackName,
                    artistId,
                    offset
                });

        var groupedResult = results
            .GroupBy(artist => artist.ArtistId)
            .Select(group =>
            {
                var artist = group.First();
                
                artist.Releases = group
                    .SelectMany(release => release.Releases)
                    .GroupBy(release => release.ReleaseId)
                    .Select(release =>
                    {
                        var groupRelease = release.First();
                        groupRelease.Tracks = release.SelectMany(track => track.Tracks).ToList();
                        return groupRelease;
                    })
                    .ToList();
                return artist;
            })
            .ToList();

        return groupedResult;
    }
}