﻿using static RoboApp.TcpBackgroundApp;

namespace RoboApp;

public partial class App : Application
{
    
    public App()
	{
        
        InitializeComponent();
       
        MainPage = new AppShell();
	}
}