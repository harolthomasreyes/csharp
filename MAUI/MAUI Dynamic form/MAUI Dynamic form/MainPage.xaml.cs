using Microsoft.Maui.Layouts;
using System.Text.Json;

namespace MAUI_Dynamic_form
{
    public partial class MainPage : ContentPage
    {
        private List<FormElement> formElements = new List<FormElement>();
        private Dictionary<string, View> formControls = new Dictionary<string, View>();

        public MainPage()
        {
            InitializeComponent();
            LoadSampleJson();
        }

        private void LoadSampleJson()
        {
            string sampleJson = """
        [
          {
            "Id": "16e5da70-74de-4e54-9a3c-b6812a38b6ac",
            "Tipo": "textbox",
            "Label": "Name:",
            "Opciones": [],
            "X": 50.0,
            "Y": 50.0,
            "Valor": null
          },
          {
            "Id": "ea73e157-88aa-458e-89fe-b129305f4bfd",
            "Tipo": "checkbox",
            "Label": "Accept?",
            "Opciones": [],
            "X": 50.0,
            "Y": 100.0,
            "Valor": null
          },
          {
            "Id": "9afc5c34-2c5b-4717-bf73-fa5ab9cf5a26",
            "Tipo": "combo",
            "Label": "Country:",
            "Opciones": [
              "United States",
              "Canada",
              "Mexico",
              "Brazil"
            ],
            "X": 131.0,
            "Y": 248.0,
            "Valor": null
          }
        ]
        """;

            jsonEditor.Text = sampleJson;
        }

        private async void OnLoadFormClicked(object sender, EventArgs e)
        {
            await LoadFormFromJson(jsonEditor.Text);
        }

        private async Task LoadFormFromJson(string jsonContent)
        {
            try
            {
                ClearForm();

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    await DisplayAlert("Error", "JSON is empty", "OK");
                    return;
                }

                formElements = FormService.LoadFormFromJson(jsonContent);

                if (formElements.Count == 0)
                {
                    await DisplayAlert("Error", "No form elements could be loaded from JSON", "OK");
                    return;
                }

                RenderForm();
                await DisplayAlert("Success", $"Form loaded with {formElements.Count} elements", "OK");
            }
            catch (JsonException)
            {
                await DisplayAlert("Error", "Invalid JSON format. Please check your JSON syntax.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error loading form: {ex.Message}", "OK");
            }
        }

        private void ClearForm()
        {
            formContainer.Children.Clear();
            formControls.Clear();
            formElements.Clear();
        }

        private void RenderForm()
        {
            foreach (var element in formElements)
            {
                // Create label
                var label = new Label
                {
                    Text = element.Label,
                    FontSize = 14,
                    TextColor = Colors.Black,
                    VerticalOptions = LayoutOptions.Center
                };

                AbsoluteLayout.SetLayoutBounds(label, new Rect(element.X, element.Y, 200, 30));
                AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.None);
                formContainer.Children.Add(label);

                // Create control based on type
                View control = element.Type.ToLower() switch
                {
                    "textbox" or "text" => CreateTextBox(element),
                    "checkbox" or "check" => CreateCheckBox(element),
                    "combo" or "picker" or "dropdown" => CreateComboBox(element),
                    "textarea" or "multiline" => CreateTextArea(element),
                    _ => new Label
                    {
                        Text = $"Unsupported type: {element.Type}",
                        TextColor = Colors.Red
                    }
                };

                // Position control
                double controlY = element.Y;
                double controlHeight = 30;

                // Adjust height for textarea
                if (element.Type.ToLower() == "textarea" || element.Type.ToLower() == "multiline")
                {
                    controlHeight = 80;
                }

                AbsoluteLayout.SetLayoutBounds(control, new Rect(element.X + 120, controlY, 200, controlHeight));
                AbsoluteLayout.SetLayoutFlags(control, AbsoluteLayoutFlags.None);
                formContainer.Children.Add(control);

                formControls[element.Id] = control;
            }
        }

        private Entry CreateTextBox(FormElement element)
        {
            return new Entry
            {
                Text = element.Value,
                Placeholder = "Enter text...",
                WidthRequest = 200,
                HeightRequest = 30
            };
        }

        private Editor CreateTextArea(FormElement element)
        {
            return new Editor
            {
                Text = element.Value,
                Placeholder = "Enter text...",
                WidthRequest = 200,
                HeightRequest = 80,
                AutoSize = EditorAutoSizeOption.TextChanges
            };
        }

        private CheckBox CreateCheckBox(FormElement element)
        {
            return new CheckBox
            {
                IsChecked = bool.TryParse(element.Value, out bool result) && result,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private Picker CreateComboBox(FormElement element)
        {
            var picker = new Picker
            {
                Title = "Select an option",
                WidthRequest = 200,
                HeightRequest = 30
            };

            foreach (var option in element.Options)
            {
                picker.Items.Add(option);
            }

            if (!string.IsNullOrEmpty(element.Value))
            {
                picker.SelectedItem = element.Value;
            }

            return picker;
        }

        private async void OnShowValuesClicked(object sender, EventArgs e)
        {
            if (formElements.Count == 0)
            {
                await DisplayAlert("Information", "No form is currently loaded", "OK");
                return;
            }

            var values = new System.Text.StringBuilder();
            values.AppendLine("Form Values:");
            values.AppendLine("------------");

            foreach (var element in formElements)
            {
                if (formControls.TryGetValue(element.Id, out var control))
                {
                    string value = control switch
                    {
                        Entry entry => entry.Text ?? "Empty",
                        Editor editor => editor.Text ?? "Empty",
                        CheckBox checkBox => checkBox.IsChecked ? "Yes" : "No",
                        Picker picker => picker.SelectedItem?.ToString() ?? "Not selected",
                        _ => "Unsupported type"
                    };

                    values.AppendLine($"{element.Label}: {value}");
                }
            }

            await DisplayAlert("Form Values", values.ToString(), "OK");
        }

        private async void OnClearFormClicked(object sender, EventArgs e)
        {
            ClearForm();
            await DisplayAlert("Information", "Form cleared", "OK");
        }
    }
}
