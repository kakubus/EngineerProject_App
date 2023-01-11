using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using static RoboApp.TcpBackgroundApp;

namespace RoboApp;


public partial class MainPage : ContentPage
{
    

    public MainPage()
	{
		InitializeComponent();
        
        
        MauiProgram.ConnectionWorker = new TcpBackgroundWorker();
        MauiProgram.ConnectionWorker.Start("192.168.0.1", 1000);
        Task.Run(() => MauiProgram.ConnectionWorker.ListenMessage("192.168.0.2", 60890));
          Task.Run(() => RefreshLabels());
       // RefreshLabels();
        LabelOutput.Text = "Cnstr: " + MauiProgram.ConnectionWorker.ConnectionStatus.ToString();
    }
 
    
    private async void Button_Pressed(object sender, EventArgs e)
    {
        LabelOutput.Text = MauiProgram.ConnectionWorker.ConnectionStatus.ToString();
        int i = 0;
        int speed = 50; // do zmiany, na razie na stałe
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
       

       
        message = E_Mode.ToString() + ", " + Dir[0].ToString() + ", " + Val[0].ToString() + ", " + Dir[1].ToString() + ", " + Val[1].ToString() + ", " + Dir[2].ToString() + ", " + Val[2].ToString() + ", " + Dir[3].ToString() + ", " + Val[3].ToString() + "\n";


        await MauiProgram.ConnectionWorker.SendMessage(message);
        LabelOutput.Text = "Sending: " + message;
        
    }
    
    private async void Button_Released(object sender, EventArgs e)
    {
        int i = -1;
        int E_Mode = 1;
        int[] Dir = new int[4] { 1, 1, 1, 1 };
        int[] Val = new int[4] { 0, 0, 0, 0 };

        var message = "0, 1, 0, 1, 0, 1, 0, 1, 0\n";
        message = E_Mode.ToString() + ", " + Dir[0].ToString() + ", " + Val[0].ToString() + ", " + Dir[1].ToString() + ", " + Val[1].ToString() + ", " + Dir[2].ToString() + ", " + Val[2].ToString() + ", " + Dir[3].ToString() + ", " + Val[3].ToString()+ "\n";

        await MauiProgram.ConnectionWorker.SendMessage(message);

        LabelOutput.Text = "Stopping: " + i.ToString();
    }

    private void EmergencyButton_Pressed(object sender, EventArgs e)
    {
       // E_Mode = 1;
    }

    public async void RefreshLabels()
    {
        string temp = "null";
        while (true)
        {
            // temp = await MauiProgram.ConnectionWorker.ListenMessage("192.168.0.2", 60890);
            temp = MauiProgram.ConnectionWorker.RecvMessage.ToString();
          //  LabelOutput.Text = "Status: " + MauiProgram.ConnectionWorker.ConnectionStatus;
            LabelOutput_Robo.Text = "Out: "+ temp;
            await Task.Delay(100);
        }

    }

        /*
    private void OnCounterClicked(object sender, EventArgs e)
    {
    count++;

    if (count == 1)
       CounterBtn.Text = $"Clicked {count} time";
    else
       CounterBtn.Text = $"Clicked {count} times";

    SemanticScreenReader.Announce(CounterBtn.Text);
    }*/
    }

