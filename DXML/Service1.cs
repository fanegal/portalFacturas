using System;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Configuration;

namespace DXML
{
    partial class Service1 : ServiceBase
    {
        private Thread th1;
        private Thread th2;
        EventLog eventLog1;
        private DateTime inicio;
        public Service1(string[] args)
        {
            InitializeComponent();
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("Dowload_cfdi"))
            {
                EventLog.CreateEventSource(
                    "Dowload_cfdi", "dCfdi_Log");
            }
            eventLog1.Source = "Dowload_cfdi";
            eventLog1.Log = "dCfdi_Log";

            inicio = DateTime.Now;
        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        protected override void OnStart(string[] args)
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            var value = Convert.ToInt32(ConfigurationSettings.AppSettings["horaInicio"].ToString());
            inicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, value, 0, 0);
            eventLog1.WriteEntry("In OnStart.");
            running();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            // TODO: agregar código aquí para iniciar el servicio.
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In OnStop.");
            // TODO: agregar código aquí para realizar cualquier anulación necesaria para detener el servicio.
            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        //private void emitidos()
        //{
        //    nucleo _nucleo;
        //    try
        //    {
        //        _nucleo = new nucleo();
        //        _nucleo.work(true, eventLog1);
        //        //  th1.Abort();
        //    }
        //    catch (Exception ex)
        //    {
        //        eventLog1.WriteEntry(ex.Message);
        //        this.OnStop();
        //    }
        //}

        //private void recibidos()
        //{

        //    nucleo _nucleo;
        //    try
        //    {
        //        _nucleo = new nucleo();
        //        _nucleo.work(false, eventLog1);
        //    }
        //    catch (Exception ex)
        //    {
        //        eventLog1.WriteEntry(ex.Message);
        //        this.OnStop();
        //    }
        //}

        private void E_SAT()
        {

            nucleo _nucleo;
            try
            {
                _nucleo = new nucleo();
                _nucleo.actualizaEstatus(eventLog1);
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message);
                this.OnStop();
            }
        }


        internal void TestStartupAndStop(string[] args)
        {
            eventLog1.WriteEntry("Inicio manual del servicio: CFDi");
            running();
        }

        private void running()
        {
            nucleo _nucleo;
            int tiempo_minutos = 60 * 1000; // 60 segudos ---1000 miliesegundos
            try
            {
                _nucleo = new nucleo();

                parametros param = _nucleo.getparametros();
                if (param.PAUSA_D_CFDI == 0)
                {
                    while (true)
                    {
                        _nucleo.Work(eventLog1);
                        Thread.Sleep(param.TIMER_D_CFDI * tiempo_minutos);
                        //   E_SAT();  
                    }
                }
                else
                {
                    Thread.Sleep(param.PAUSA_D_CFDI * tiempo_minutos);
                    _nucleo.quitapausa();
                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message);
                this.OnStop();
            }
        }
    }
}
