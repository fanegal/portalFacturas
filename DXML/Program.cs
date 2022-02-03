using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace DXML
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                string opcion = args[0];
                Process proc = new Process();
                string _Command = "";
                ProcessStartInfo procStartInfo;

                switch (opcion)
                {
                    case "-i":
                        _Command = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/installutil.exe DXML.exe";
                        procStartInfo = new ProcessStartInfo("cmd", "/c " + _Command);
                        procStartInfo.CreateNoWindow = true;
                        try
                        {
                            proc.StartInfo = procStartInfo;
                            proc.Start();
                            Console.WriteLine("El servicio se instalo correctamente");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        //  
                        //Muestra en pantalla la salida del Comando
                        // 
                        break;

                    case "-u":
                        _Command = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/installutil.exe  -u DXML.exe";
                        procStartInfo = new ProcessStartInfo("cmd", "/c " + _Command);
                        procStartInfo.CreateNoWindow = true;
                        try
                        {
                            proc.StartInfo = procStartInfo;
                            proc.Start();
                            Console.WriteLine("El servicio se desinstalo correctamente");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                    case "-s":
                        Service1 service1 = new Service1(args);
                        service1.TestStartupAndStop(args);
                        break;
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                        {
                new Service1(args)
                        };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
