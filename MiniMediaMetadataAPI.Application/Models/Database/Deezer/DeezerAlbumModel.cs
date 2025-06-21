using System.ComponentModel.DataAnnotations;

namespace MiniMediaMetadataAPI.Application.Models.Database.Deezer;

public class DeezerAlbumModel
{
    public long AlbumId { get; set; }
    public long ArtistId { get; set; }
    public string Title { get; set; }
    public string Md5Image { get; set; }
    public int GenreId { get; set; }
    public int Fans { get; set; }
    public string ReleaseDate { get; set; }
    public string RecordType { get; set; }
    public bool ExplicitLyrics { get; set; }
    public int ExplicitContentLyrics { get; set; }
    public int ExplicitContentCover { get; set; }
    public string Type { get; set; }
    public string UPC { get; set; }
    public string Label { get; set; }
    public int NbTracks { get; set; }
    public int Duration { get; set; }
    public bool Available { get; set; }
    
    public DeezerArtistModel Artist { get; set; }
    public List<DeezerAlbumArtistModel> Artists { get; set; } = new List<DeezerAlbumArtistModel>();
    public List<DeezerAlbumImageLinkModel> Images { get; set; } = new List<DeezerAlbumImageLinkModel>();
}