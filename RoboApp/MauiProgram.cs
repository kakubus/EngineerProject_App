﻿

using Microsoft.Maui.Controls;
using static RoboApp.TcpBackgroundApp;

namespace RoboApp;

public static class MauiProgram
{
    public static TcpBackgroundWorker ConnectionWorker;
    public static TcpBackgroundWorker ReceivingWorker { get; private set; }
    public static TcpBackgroundWorker SendingWorker { get; private set; }
    public static MauiApp CreateMauiApp()
	{

		
        //	ReceivingWorker = new TcpBackgroundWorker();
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
