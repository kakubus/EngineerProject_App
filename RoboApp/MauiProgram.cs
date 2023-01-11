using Microsoft.Maui.Controls;
using static RoboApp.TcpBackgroundApp;

namespace RoboApp;

public static class MauiProgram
{
	public static TcpBackgroundWorker ConnectionWorker;

    public static MauiApp CreateMauiApp()
	{

        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Oxygen-Bold.ttf", "Oxygen"); 
            });
       

        return builder.Build();
	}
}
