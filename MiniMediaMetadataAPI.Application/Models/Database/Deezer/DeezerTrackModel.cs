namespace MiniMediaMetadataAPI.Application.Models.Database.Deezer;

public class DeezerTrackModel
{
    public long TrackId { get; set; }
    public bool Readable { get; set; }
    public string Title { get; set; }
    public string TitleShort { get; set; }
    public string TitleVersion { get; set; }
    public string ISRC { get; set; }
    public int Duration { get; set; }
    public int TrackPosition { get; set; }
    public int DiskNumber { get; set; }
    public int Rank { get; set; }
    public string ReleaseDate { get; set; }
    public bool ExplicitLyrics { get; set; }
    public int ExplicitContentLyrics { get; set; }
    public int ExplicitContentCover { get; set; }
    public string Preview { get; set; }
    public float BPM { get; set; }
    public float Gain { get; set; }
    public string Md5Image { get; set; }
    public string TrackToken { get; set; }
    public long ArtistId { get; set; }
    public long AlbumId { get; set; }
    public string Type { get; set; }

    public DeezerAlbumModel Album { get; set; }
    public List<DeezerTrackArtistModel> Artists { get; set; } = new List<DeezerTrackArtistModel>();
}