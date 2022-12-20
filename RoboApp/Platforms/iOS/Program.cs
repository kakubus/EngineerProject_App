using ObjCRuntime;
using UIKit;
using static RoboApp.TcpBackgroundApp;

namespace RoboApp;

public class Program
{

    // This is the main entry point of the application.
  

    static void Main(string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(AppDelegate));
        TcpBackgroundWorker connectionWorker2 = new TcpBackgroundWorker();
        connectionWorker2.Start("192.168.0.1", 1000);
   

    }
}
