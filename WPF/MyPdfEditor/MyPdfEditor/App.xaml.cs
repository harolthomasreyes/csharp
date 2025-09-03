using Microsoft.Extensions.DependencyInjection;
using MyPdfEditor.Core.Services.Implementations;
using MyPdfEditor.Core.Services.Interfaces;
using MyPdfEditor.MyPdfEditor.WPF.Views;
using MyPdfEditor.WPF.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MyPdfEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register Services
            services.AddSingleton<IPdfService, PdfService>();
            services.AddSingleton<IFormFieldService, FormFieldService>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddSingleton<IUndoRedoService, UndoRedoService>();

            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<DocumentViewModel>();
            services.AddTransient<PropertiesPanelViewModel>();
            services.AddTransient<FieldViewModel>();
            services.AddTransient<TextFieldViewModel>();
            services.AddTransient<CheckboxFieldViewModel>();
            //services.AddTransient<RadioButtonFieldViewModel>();
            //services.AddTransient<ComboBoxFieldViewModel>();
            //services.AddTransient<ListBoxFieldViewModel>();
            //services.AddTransient<ButtonFieldViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
            services.AddTransient<DocumentView>();
            services.AddTransient<PropertiesPanel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetService<MainViewModel>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
