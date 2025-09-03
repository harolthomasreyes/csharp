using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DinamicForms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<CampoFormulario> _campos = new();
        private UIElement _draggedElement;
        private Point _startPoint;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AgregarTextBox_Click(object sender, RoutedEventArgs e)
        {
            CrearControl(new CampoFormulario { Tipo = "textbox", Label = "Nombre:", X = 50, Y = 50 });
        }

        private void AgregarCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CrearControl(new CampoFormulario { Tipo = "checkbox", Label = "Acepta?", X = 50, Y = 100 });
        }

        private void AgregarCombo_Click(object sender, RoutedEventArgs e)
        {
            CrearControl(new CampoFormulario
            {
                Tipo = "combo",
                Label = "País:",
                Opciones = new() { "Argentina", "Chile", "México" },
                X = 50,
                Y = 150
            });
        }

        private void CrearControl(CampoFormulario campo)
        {
            FrameworkElement control;

            switch (campo.Tipo)
            {
                case "textbox":
                    control = new StackPanel();
                    ((StackPanel)control).Children.Add(new TextBlock { Text = campo.Label });
                    ((StackPanel)control).Children.Add(new TextBox { Width = 150 });
                    break;

                case "checkbox":
                    control = new CheckBox { Content = campo.Label };
                    break;

                case "combo":
                    var stack = new StackPanel();
                    stack.Children.Add(new TextBlock { Text = campo.Label });
                    var combo = new ComboBox { Width = 150 };
                    foreach (var op in campo.Opciones) combo.Items.Add(op);
                    stack.Children.Add(combo);
                    control = stack;
                    break;

                default:
                    return;
            }

            // Guardar metadatos en Tag
            control.Tag = campo;

            // Ubicar en canvas
            Canvas.SetLeft(control, campo.X);
            Canvas.SetTop(control, campo.Y);

            // Eventos de arrastre
            control.MouseLeftButtonDown += Control_MouseLeftButtonDown;
            control.MouseMove += Control_MouseMove;
            control.MouseLeftButtonUp += Control_MouseLeftButtonUp;

            DesignCanvas.Children.Add(control);
            _campos.Add(campo);
        }

        // Drag & drop
        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _draggedElement = sender as UIElement;
            _startPoint = e.GetPosition(DesignCanvas);
            _draggedElement.CaptureMouse();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(DesignCanvas);
                double dx = pos.X - _startPoint.X;
                double dy = pos.Y - _startPoint.Y;

                double left = Canvas.GetLeft(_draggedElement) + dx;
                double top = Canvas.GetTop(_draggedElement) + dy;

                Canvas.SetLeft(_draggedElement, left);
                Canvas.SetTop(_draggedElement, top);

                // actualizar en modelo
                if (_draggedElement is FrameworkElement fe && fe.Tag is CampoFormulario campo)
                {
                    campo.X = left;
                    campo.Y = top;
                }

                _startPoint = pos;
            }
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedElement != null)
            {
                _draggedElement.ReleaseMouseCapture();
                _draggedElement = null;
            }
        }

        // Exportar JSON
        private void ExportarJson_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(_campos, Formatting.Indented);
            MessageBox.Show(json, "Formulario en JSON");
        }
    }
}