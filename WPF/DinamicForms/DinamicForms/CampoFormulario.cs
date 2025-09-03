public class CampoFormulario
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Tipo { get; set; } = "textbox"; // textbox, checkbox, combo
    public string Label { get; set; } = "Nuevo Campo";
    public List<string> Opciones { get; set; } = new();
    public double X { get; set; } // Posición en el canvas
    public double Y { get; set; }
    public object Valor { get; set; } // Dato ingresado por el usuario
}