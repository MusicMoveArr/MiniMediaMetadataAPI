using System.Text.RegularExpressions;
using MiniMediaMetadataAPI.Application.Models.Database.Discogs;

namespace MiniMediaMetadataAPI.Application.Helpers;

public static class DiscogsHelper
{
    public static int GetDiscNumber(DiscogsReleaseTrack track)
    {
        //let's not process this weird stuff, not sure what it means this "arrow"
        if (track.Position.Contains('→'))
        {
            return 0;
        }
        
        //couldn't Discogs come to just 1 standard... this makes no sense to me
        string cdRegexPrefix = "^[(]{0,1}(CD|CDR|cd|cdr|CD\\-Rom){0,}[ ]*([0-9]*)";
        string cdRegexPostfix = "(CD|CDR|cd|cdr|CD\\-Rom){1,}([0-9]*)$";
        
        //try postfix first otherwise we grab the Tracknumber by accident
        var postfixMatch = Regex.Match(track.Position, cdRegexPostfix);
        if (postfixMatch.Success)
        {
            return int.TryParse(postfixMatch.Groups.Values.LastOrDefault()?.Value, out var discNumber) ? discNumber : 0;
        }
        
        var prefixMatch = Regex.Match(track.Position, cdRegexPrefix);
        if (prefixMatch.Success)
        {
            return int.TryParse(prefixMatch.Groups.Values.LastOrDefault()?.Value, out var discNumber) ? discNumber : 0;
        }
        return 0;
    }
    
    public static string GetTrackNumber(DiscogsReleaseTrack track)
    {
        //let's not process this weird stuff, not sure what it means this "arrow"
        if (track.Position.Contains('→'))
        {
            return string.Empty;
        }
        
        //couldn't Discogs come to just 1 standard... this makes no sense to me
        //cd/CD/cdr/CDR optional prefix
        //get optional CD Number
        //try get tracknumber
        //try get tracknumber from "xx-yy"
        //try get tracknumber that has prefix "Track" before it
        string trackRegexPrefix = "^[(]{0,1}(CD|CDR|cd|cdr|CD\\\\-Rom){0,}[ ]*([0-9A-Z]*)[)]{0,1}[ \\\\-\\\\.\\\\\\/]*([0-9]*)[\\- ]*([0-9]*)(Track[ ]*([0-9]*))*";
        
        var prefixMatch = Regex.Match(track.Position, trackRegexPrefix);
        if (prefixMatch.Success)
        {
            return int.TryParse(prefixMatch.Groups.Values.LastOrDefault()?.Value, out var trackNumber) ? trackNumber.ToString() : string.Empty;
        }
        
        return string.Empty;
    }
}