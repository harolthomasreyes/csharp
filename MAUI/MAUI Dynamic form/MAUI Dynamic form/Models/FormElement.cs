using System.Text.Json.Serialization;

public class FormElement
{
    [JsonPropertyName("Id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("Tipo")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("Label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("Opciones")]
    public List<string> Options { get; set; } = new List<string>();

    [JsonPropertyName("X")]
    public double X { get; set; }

    [JsonPropertyName("Y")]
    public double Y { get; set; }

    [JsonPropertyName("Valor")]
    public string? Value { get; set; }
}