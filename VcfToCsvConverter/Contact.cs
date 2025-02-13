namespace VcfToCsvConverter;

public record Contact
{
    public string? FullName { get; set; }
    public List<string> PhoneNumbers { get; set; } = [];
}