namespace MiniMediaMetadataAPI.Application.Models.Database.SoundCloud;

public class SoundCloudArtistModel
{
    public long Id { get; set; } 
    public string Title { get; set; } 
    public string FirstName { get; set; } 
    public string LastName { get; set; } 
    public string FullName { get; set; } 
    public string CountryCode { get; set; } 
    public string City { get; set; } 
    public string AvatarUrl { get; set; } 
    public string PermaLink { get; set; } 
    public string Url { get; set; } 
    public string Urn { get; set; } 
    public string Username { get; set; } 
    public bool Badge_ProUnlimited { get; set; } 
    public bool Badge_Verified { get; set; } 
    public DateTime? LastModified { get; set; }
    public DateTime LastSyncTime { get; set; }
}