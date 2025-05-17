using System.Text;

namespace MiniMediaMetadataAPI.Application.Helpers;

public static class StringHelper
{
    public static string RemoveControlChars(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }
        
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsControl(value[i]))
            {
                sb.Append(value[i]);
            }
        }
        
        return sb.ToString();
    }
}