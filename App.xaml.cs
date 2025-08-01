using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuizGame1WPF.Services;
using QuizGame1WPF.Models;
using Serilog;
using System.IO;
using System.Windows;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for the application.
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        /// <summary>
        /// Handles application startup logic.
        /// </summary>
        /// <param name="e">Startup event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            if (loginWindow.ShowDialog() == true)
            {
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }

        /// <summary>
        /// Configures dependency injection services.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        private void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/quiz-app-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddLogging(builder => builder.AddSerilog());
            
            // Register services
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddTransient<IQuizService, QuizService>();
            
            // Register windows with factory pattern for better control
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<TeacherDashboard>();
            services.AddTransient<StudentDashboard>();
            services.AddTransient<UserStatisticsWindow>();
            services.AddTransient<QuizResultsWindow>();
            
            // Register factory for QuizWindow since it needs parameters
            services.AddTransient<Func<IQuizService, QuizSession, QuizWindow>>(provider =>
                (quizService, session) => new QuizWindow(quizService, session));
        }

        /// <summary>
        /// Handles application exit logic.
        /// </summary>
        /// <param name="e">Exit event arguments.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        /// <summary>
        /// Gets a registered service from the service provider.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <returns>The requested service instance.</returns>
        public static T GetService<T>() where T : class
        {
            return ((App)Current)._serviceProvider?.GetService<T>() 
                ?? throw new InvalidOperationException($"Service {typeof(T).Name} not found");
        }
    }
}
