namespace MiniMediaMetadataAPI.Application.Models.Database.SoundCloud;

public class SoundCloudTrackModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; }
    public string PlaylistName { get; set; }
    public string Caption { get; set; }
    public bool Commentable { get; set; }
    public int CommentCount { get; set; }
    public string Description { get; set; }
    public bool Downloadable { get; set; }
    public long DownloadCount { get; set; }
    public long Duration { get; set; }
    public long FullDuration { get; set; }
    public string EmbeddableBy { get; set; }
    public string Genre { get; set; }
    public bool HasDownloadsLeft { get; set; }
    public string LabelName { get; set; }
    public string License { get; set; }
    public long LikesCount { get; set; }
    public string Permalink { get; set; }
    public string PermalinkUrl { get; set; }
    public string PlaybackCount { get; set; }
    public bool Public { get; set; }
    public string PublisherMetadata_Artist { get; set; }
    public bool PublisherMetadata_ContainsMusic { get; set; }
    public string PublisherMetadata_Id { get; set; }
    public string PublisherMetadata_Urn { get; set; }
    public string PurchaseTitle { get; set; }
    public string PurchaseUrl { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public long RepostsCount { get; set; }
    public string Sharing { get; set; }
    public string State { get; set; }
    public bool Streamable { get; set; }
    public string TagList { get; set; }
    public string Uri { get; set; }
    public string ArtworkUrl { get; set; }
    public string Visuals { get; set; }
    public string WaveformUrl { get; set; }
    public string DisplayDate { get; set; }
    public string MonetizationModel { get; set; }
    public string Policy { get; set; }
    public string Urn { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    
    public int TrackOrder { get; set; }
    
    public SoundCloudAlbumModel Album { get; set; }
    public List<SoundCloudArtistModel> Artists { get; set; } = new List<SoundCloudArtistModel>();
}