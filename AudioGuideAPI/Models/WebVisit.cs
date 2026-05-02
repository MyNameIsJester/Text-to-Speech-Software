public class WebVisit
{
    public int Id { get; set; }

    public string VisitToken { get; set; } = string.Empty;

    public DateTime FirstSeenAtUtc { get; set; }

    public DateTime LastSeenAtUtc { get; set; }

    public string? UserAgent { get; set; }

    public string? IpAddress { get; set; }
}