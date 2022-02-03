using System.Net.Mail;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;

namespace Business.Funciones
{
    public class E_correoC
    {


        public void enviar(Entidades.sendmail registro, NetworkCredential credential)
        {

            MailMessage correo = new MailMessage();
            correo.From = new MailAddress(registro.origen, "Kyocode", System.Text.Encoding.UTF8);//Correo de salida
            correo.To.Add(registro.destino); //Correo destino

            correo.Subject = registro.asunto; //Asunto
            correo.Body = registro.cuerpo; //Mensaje del correo
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = false;
            smtp.Host = registro.host;// "smtp.gmail.com"; //Host del servidor de correo
            smtp.Port = Convert.ToInt32(registro.puerto); //Puerto de salida
            smtp.Credentials = credential;// new System.Net.NetworkCredential("xxxxxx@gmail.com", "*******");//Cuenta de correo
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                smtp.EnableSsl = registro.bit_ssl;//True si el servidor de correo permite ssl

                smtp.Send(correo);

            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
