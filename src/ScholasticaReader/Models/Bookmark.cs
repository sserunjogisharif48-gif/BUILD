using System;

namespace ScholasticaReader.Models;

public class Bookmark
{
    public int Id { get; set; }
    public string BookPath { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
