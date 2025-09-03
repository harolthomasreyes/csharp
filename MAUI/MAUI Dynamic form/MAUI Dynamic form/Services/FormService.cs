using System.Text.Json;

public static class FormService
{
    public static List<FormElement> LoadFormFromJson(string jsonContent)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var formElements = JsonSerializer.Deserialize<List<FormElement>>(jsonContent, options);
            return formElements ?? new List<FormElement>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading form: {ex.Message}");
            return new List<FormElement>();
        }
    }
}