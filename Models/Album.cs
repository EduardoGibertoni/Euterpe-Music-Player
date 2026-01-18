namespace Euterpe.Models;

public class Album
{
    public string Name { get; set; } = "";
    public string FolderPath { get; set; } = "";
    public string CoverPath { get; set; } = "";
    public List<string> Tracks { get; set; } = new();
}
