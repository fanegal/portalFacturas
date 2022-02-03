using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Business.Core;
using Business.HerramientasCFDi;

namespace DXML
{
    public class nucleo
    {


        public nucleo()
        {
            //petcfdi = new B_Peticiones();
            //petEstatus = new B_Estatus_CFDi();
        }
        public parametros getparametros()
        {
            parametros p;
            B_parametros bparam;
            try
            {
                bparam = new B_parametros();

                p = new parametros();
                p.PAUSA_D_CFDI = Convert.ToInt32(bparam.GetParametro("PAUSA_D_CFDI").valor);
                p.TIMER_D_CFDI = Convert.ToInt32(bparam.GetParametro("TIMER_D_CFDI").valor);
                p.TIMER_E_CFDI = Convert.ToInt32(bparam.GetParametro("TIMER_E_CFDI").valor);
            }
            catch (Exception ex)
            {

                throw;
            }
            return p;
        }
        public async void Work(EventLog eventLog1)
        {
            try
            {
                await Task.Run(() =>
                {
                    eventLog1.WriteEntry("Proceso para obtener token");
                    var tokens = Gettoken();
                    eventLog1.WriteEntry("Número de token obtenidos: " + tokens.ToString());

                    eventLog1.WriteEntry("Proceso para realizar las solicitudes");
                    var solicitudes = GetSolicitud();
                    eventLog1.WriteEntry("Número de solicitudes procesadas: " + solicitudes.ToString());

                    eventLog1.WriteEntry("Proceso para validar las solicitudes");
                    var validados = ValidaSolicitud();
                    eventLog1.WriteEntry("Número de solicitudes validadas: " + solicitudes.ToString());

                    eventLog1.WriteEntry("Proceso para descargar los paquetes");
                    var descargados = DecargarPaquetes();
                    eventLog1.WriteEntry("Número de paquetes descargados: " + solicitudes.ToString());

                    eventLog1.WriteEntry("Proceso para procesar las descargas");
                    var procesarDescarga = Procesardescargas();
                    eventLog1.WriteEntry("Termina proceso  de descargas");
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static int Gettoken()
        {
            List<Entidades.peticion> Listado;
            B_Peticiones b_obj;
            DescargaCFDI objcfdi;
            int procesados = 0;
            try
            {
                b_obj = new B_Peticiones();
                Listado = b_obj.Sintoken();
                objcfdi = new DescargaCFDI();
                if (Listado.Count > 0)
                {

                    Listado.ForEach(r =>
                   {
                       if (objcfdi.Process_sintoken(r))
                       {
                           procesados += 1;
                       }
                   });

                }
            }
            catch (Exception)
            {
                throw;

            }
            return procesados;
        }
        private static int GetSolicitud()
        {
            List<Entidades.peticion> Listado;
            B_Peticiones b_obj;
            DescargaCFDI objcfdi;
            int procesados = 0;
            try
            {
                b_obj = new B_Peticiones();
                Listado = b_obj.Sinsolicitud();
                objcfdi = new DescargaCFDI();
                if (Listado.Count > 0)
                {

                    Listado.ForEach(r =>
                    {
                        if (objcfdi.Process_sinsolicitud(r))
                        {
                            procesados += 1;
                        }
                    });

                }
            }
            catch (Exception)
            {

                throw;
            }
            return procesados;

        }
        private static int ValidaSolicitud()
        {
            List<Entidades.peticion> Listado;
            B_Peticiones b_obj;
            DescargaCFDI objcfdi;
            int procesados = 0;
            try
            {
                b_obj = new B_Peticiones();
                Listado = b_obj.Sinpaquete();
                objcfdi = new DescargaCFDI();
                if (Listado.Count > 0)
                {
                    Listado.ForEach(r =>
                    {
                        if (objcfdi.Process_validasolicitud(r))
                        {
                            procesados += 1;
                        }
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return procesados;
        }
        private static int DecargarPaquetes()
        {
            List<Entidades.peticion> Listado;
            B_Peticiones b_obj;
            DescargaCFDI objcfdi;
            int procesados = 0;
            try
            {
                b_obj = new B_Peticiones();
                Listado = b_obj.Sindescarga();
                objcfdi = new DescargaCFDI();
                if (Listado.Count > 0)
                {

                    Listado.ForEach(r =>
                    {
                        if (objcfdi.Process_decargareponse(r))
                        {
                            procesados += 1;
                        }
                    });

                }

            }
            catch (Exception)
            {

                throw;
            }
            return procesados;

        }
        private static int Procesardescargas()
        {


            List<Entidades.peticion> Listado;
            B_Peticiones b_obj;
            DescargaCFDI objcfdi;
            int procesados = 0;
            try
            {
                b_obj = new B_Peticiones();
                Listado = b_obj.Sindescomprimir();
                objcfdi = new DescargaCFDI();
                if (Listado.Count > 0)
                {

                    Listado.ForEach(r =>
                    {
                        if (objcfdi.Process_descomprimirdescarga(r))
                        {
                            procesados += 1;
                        }
                    });

                }

            }
            catch (Exception)
            {

                throw;
            }
            return procesados;
        }
        public void actualizaEstatus(EventLog eventLog1)
        {
            try
            {
                //tcfdi = new tool_Cfdi();
                //eventLog1.WriteEntry("Proceso de EstatusSAt");
                //petEstatus.getPeticiones().ForEach(k => {
                //    eventLog1.WriteEntry("Consulta :" + k.Text);
                //    tcfdi.Estatus_SAT(Guid.Parse( k.Value));
                //});
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void quitapausa()
        {
            parametros p;
            B_parametros bparam;
            try
            {
                bparam = new B_parametros();
                bparam.SetParametro("PAUSA_D_CFDI", "0");
                if (bparam.bit_error) { throw bparam.error; }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }


    public class parametros
    {
        public int TIMER_E_CFDI { get; set; }
        public int TIMER_D_CFDI { get; set; }
        public int PAUSA_D_CFDI { get; set; }


    }
}
