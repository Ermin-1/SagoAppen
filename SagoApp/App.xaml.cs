using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.Net.Http;
using System;

namespace SagoApp
{
    public partial class App : Application
    {
        // Global HttpClient-instans (används om du vill dela en klient över hela appen)
        public static HttpClient HttpClient { get; } = new HttpClient();

        public App()
        {
            InitializeComponent();

            // Sätter MainPage till din MainPage
            MainPage = new NavigationPage(new MainPage());

#if DEBUG && !DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            };
#endif
        }
    }
}
