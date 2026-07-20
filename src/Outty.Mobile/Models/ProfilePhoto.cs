namespace Outty.Mobile.Models;

public class ProfilePhoto
{
    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public ImageSource ImageSource =>
        ImageSource.FromFile(FilePath);
}