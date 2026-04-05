using System.ComponentModel.DataAnnotations;

namespace MiniMediaMetadataAPI.Application.Models.Database.SoundCloud;

public class SoundCloudAlbumModel
{
    public long Id { get; set; } 
    public long UserId { get; set; } 
    public string Title { get; set; } 
    public string Description { get; set; } 
    public long Duration { get; set; } 
    public string EmbeddableBy { get; set; } 
    public string Genre { get; set; } 
    public string LabelName { get; set; } 
    public string License { get; set; } 
    public long LikesCount { get; set; } 
    public bool ManagedByFeeds { get; set; } 
    public bool Public { get; set; } 
    public string PurchaseTitle { get; set; } 
    public string PurchaseUrl { get; set; } 
    public DateTime? ReleaseDate { get; set; } 
    public long RepostsCount { get; set; } 
    public string Sharing { get; set; } 
    public string TagList { get; set; } 
    public string SetType { get; set; } 
    public bool IsAlbum { get; set; } 
    public DateTime? PublishedAt { get; set; } 
    public long TrackCount { get; set; } 
    public string Uri { get; set; } 
    public string PermalinkUrl { get; set; } 
    public string Permalink { get; set; } 
    public string ArtworkUrl { get; set; } 
    public DateTime DisplayDate { get; set; } 
    public DateTime? CreatedAt { get; set; } 
    public DateTime? LastModified { get; set; }
    
    public SoundCloudArtistModel Artist { get; set; }
}