namespace MiniMediaMetadataAPI.Application.Models.Database.Deezer;

public class DeezerArtistImageLinkModel
{
    public long ArtistId { get; set; }
    public string Href { get; set; }
    public string Type { get; set; }

    public int Width
    {
        get
        {
            switch (Type)
            {
                case "xl": return 1000;
                case "big": return 500;
                case "medium": return 250;
                case "small": return 56;
                case "picture": return 120; //usually 120
                default: return 0;
            }
        }
    }
    public int Height
    {
        get
        {
            switch (Type)
            {
                case "xl": return 1000;
                case "big": return 500;
                case "medium": return 250;
                case "small": return 56;
                case "picture": return 120; //usually 120
                default: return 0;
            }
        }
    }
}