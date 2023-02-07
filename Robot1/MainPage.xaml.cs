
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using static Robot1.TcpBackgroundApp;
using Microsoft.Maui.Graphics;
using Microsoft.Maui;

namespace Robot1;

public partial class MainPage : ContentPage
{


    public MainPage()
    {
        
        InitializeComponent();
        
        //  MauiProgram.ConnectionWorker.OnDataArrived += ConnectionWorker_OnArrived;
        this.BindingContext = MauiProgram.ConnectionWorker;
        SteeringGrid.IsEnabled = false;
        //    LabelOutput_Robo.SetBinding(Label.TextProperty, MauiProgram.ConnectionWorker.RecvMessage);
    }
   
    public async void ChangeConnectionColors(bool connected)
    {
        if (connected == true)
        {
            GridOfConnectionStatus.BackgroundColor = Microsoft.Maui.Graphics.Color.FromRgba(20, 149, 5, 100);
            //LabelOutput.Text = "ROBO-1 status: Connected";
        }
        else if (connected == false)
        {
            GridOfConnectionStatus.BackgroundColor = Microsoft.Maui.Graphics.Color.FromRgba(109, 109, 109, 100);
        }
    }
    
  //  private void ConnectionWorker_OnArrived(string temp)
 //   {
 //       LabelOutput_Robo.Text = temp;
  //  }
    
    private async void ConnectionSwitch_Toggled(object sender, ToggledEventArgs e)
    {

        // PermissionStatus status = await Permissions.RequestAsync<Permissions.NetworkState>();

        ChangeConnectionColors(e.Value);
        if (e.Value == true)
        {
            
            await MauiProgram.ConnectionWorker.Start("192.168.0.1", 1000);  //Komunikacja z robotem
          //    await Task.Delay(500);
            await MauiProgram.ConnectionWorker.ListenMessage("192.168.0.2", 60890);
            SteeringGrid.IsEnabled = true;
        }
        else
        {
     
            MauiProgram.ConnectionWorker.Stop();
            SteeringGrid.IsEnabled = false;
            //      LabelOutput.Text = "Connection: Disconnected";
        }
    }


    private async void Button_Pressed(object sender, EventArgs e)
    {
    //    LabelOutput.Text = MauiProgram.ConnectionWorker.ConnectionStatus.ToString();
        int i = 0;
        int speed = (int)SliderSpeed.Value; 
        var button = (Button)sender;
        var buttonType = button.ClassId;

        int E_Mode = 0; // Normalnie 0! To bardzo ważne!
        int[] Dir = new int[4] { 1, 1, 1, 1 };
        int[] Val = new int[4] { 0, 0, 0, 0 };


        // message = E_Mode + ", "+ dir[0] + ", " + Val[0] + ", " + Dir[1] + ", " + Val[1] + ", " + DirC[2] + ", " + Val[2] + ", " + Dir[3] + ", " + Val[3] +"";
        var message = "1, 1, 50, 1, 50, 1, 50, 1, 50\n";

        switch (buttonType)
        {
            //Normal move Forward, Backward, Left, Right
            case "U_Button":
                i = 1;

                Dir[0] = 1;
                Val[0] = speed;
                Dir[1] = 1;
                Val[1] = speed;
                Dir[2] = 1;
                Val[2] = speed;
                Dir[3] = 1;
                Val[3] = speed;

                break;
            case "D_Button":
                i = 2;

                Dir[0] = -1;
                Val[0] = speed;
                Dir[1] = -1;
                Val[1] = speed;
                Dir[2] = -1;
                Val[2] = speed;
                Dir[3] = -1;
                Val[3] = speed;

                break;
            case "L_Button":
                i = 3;

                Dir[0] = -1;
                Val[0] = speed;
                Dir[1] = 1;
                Val[1] = speed;
                Dir[2] = 1;
                Val[2] = speed;
                Dir[3] = -1;
                Val[3] = speed;

                break;
            case "R_Button":
                i = 4;

                Dir[0] = 1;
                Val[0] = speed;
                Dir[1] = -1;
                Val[1] = speed;
                Dir[2] = -1;
                Val[2] = speed;
                Dir[3] = 1;
                Val[3] = speed;

                break;

            //Diagonal move Forward L/R, Backward L/R
            case "DU_L_Button":
                i = 5;


                Dir[0] = 1;
                Val[0] = 0;
                Dir[1] = 1;
                Val[1] = speed;
                Dir[2] = 1;
                Val[2] = speed;
                Dir[3] = 1;
                Val[3] = 0;

                break;

            case "DU_R_Button":
                i = 6;

                Dir[0] = 1;
                Val[0] = speed;
                Dir[1] = 1;
                Val[1] = 0;
                Dir[2] = 1;
                Val[2] = 0;
                Dir[3] = 1;
                Val[3] = speed;
                break;

            case "DD_L_Button":
                i = 7;

                Dir[0] = -1;
                Val[0] = speed;
                Dir[1] = -1;
                Val[1] = 0;
                Dir[2] = -1;
                Val[2] = 0;
                Dir[3] = -1;
                Val[3] = speed;

                break;
            case "DD_R_Button":
                i = 8;

                Dir[0] = -1;
                Val[0] = 0;
                Dir[1] = -1;
                Val[1] = speed;
                Dir[2] = -1;
                Val[2] = speed;
                Dir[3] = -1;
                Val[3] = 0;

                break;

            //Rotate move L/R
            case "R_L_Button":
                i = 9;

                Dir[0] = -1;
                Val[0] = speed;
                Dir[1] = 1;
                Val[1] = speed;
                Dir[2] = -1;
                Val[2] = speed;
                Dir[3] = 1;
                Val[3] = speed;

                break;
            case "R_R_Button":
                i = 10;

                Dir[0] = 1;
                Val[0] = speed;
                Dir[1] = -1;
                Val[1] = speed;
                Dir[2] = 1;
                Val[2] = speed;
                Dir[3] = -1;
                Val[3] = speed;

                break;

            default:
                i = -1;
                break;
        }



        message = E_Mode.ToString() + ", " + Dir[0].ToString() + ", " + Val[0].ToString() + ", " + Dir[1].ToString() + ", " + Val[1].ToString() + ", " + Dir[2].ToString() + ", " + Val[2].ToString() + ", " + Dir[3].ToString() + ", " + Val[3].ToString() ;

        if(ConnectSwitch.IsToggled==true)
        {
            await MauiProgram.ConnectionWorker.SendMessage(message + "\n");
            LabelOutput.Text = "Sending: " + message;
        }
        else
        {
            LabelOutput.Text = "You are not connected to ROBO-1! ";
        }

        

    }

    private async void Button_Released(object sender, EventArgs e)
    {
        int i = -1;
        int E_Mode = 1;
        int[] Dir = new int[4] { 1, 1, 1, 1 };
        int[] Val = new int[4] { 0, 0, 0, 0 };

        var message = "0, 1, 0, 1, 0, 1, 0, 1, 0\n";
        message = E_Mode.ToString() + ", " + Dir[0].ToString() + ", " + Val[0].ToString() + ", " + Dir[1].ToString() + ", " + Val[1].ToString() + ", " + Dir[2].ToString() + ", " + Val[2].ToString() + ", " + Dir[3].ToString() + ", " + Val[3].ToString() ;

        if (ConnectSwitch.IsToggled == true)
        {
            await MauiProgram.ConnectionWorker.SendMessage(message + "\n");
            LabelOutput.Text = "Stopping: " + i.ToString();
        }
 
       
    }

    private async void EmergencyButton_Pressed(object sender, EventArgs e)
    {
        var message = "1, 1, 0, 1, 0, 1, 0, 1, 0";

        await MauiProgram.ConnectionWorker.SendMessage(message + "\n");
        await Task.Delay(100);

        MauiProgram.ConnectionWorker.Stop();
        
        LabelOutput.Text = "Emergency stop! Disconnecting";
       
        ConnectSwitch.IsToggled = false;
    }



}
