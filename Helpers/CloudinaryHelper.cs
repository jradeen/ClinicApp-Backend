namespace ClinicApp.API.Helpers;

public static class CloudinaryHelper
{
    // Extracts the public_id from a Cloudinary secure URL

    public static string? GetPublicIdFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');

            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex == -1) return null;

            var startIndex = uploadIndex + 1;

            // skip the version segment (e.g. "v1234567890")
            if (startIndex < segments.Length
                && segments[startIndex].StartsWith("v")
                && segments[startIndex].Length > 1
                && segments[startIndex][1..].All(char.IsDigit))
            {
                startIndex++;
            }

            var rest = string.Join("/", segments.Skip(startIndex));
            var directory = Path.GetDirectoryName(rest) ?? "";
            var fileName = Path.GetFileNameWithoutExtension(rest);

            return string.IsNullOrEmpty(directory) ? fileName : $"{directory}/{fileName}";
        }
        catch
        {
            return null;
        }
    }
}