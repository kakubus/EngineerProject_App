using Microsoft.Extensions.Logging;

using static Robot1.TcpBackgroundApp;

namespace Robot1;

public static class MauiProgram
{
	public static TcpBackgroundWorker ConnectionWorker { get; private set; } = new TcpBackgroundWorker();
    
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

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
