using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Core
{
    public class B_Sendmail
    {


        Data_Access.sendmail est;
        public B_Sendmail()
        {
            est = new Data_Access.sendmail();
        }


        public bool correoRegistro(string destino, string asunto, string body)
        {
            bool bit_error = false;
            Entidades.sendmail newobjeto;
            Data_Access.sendmail send;
            try
            {
                send = new Data_Access.sendmail(); 
                newobjeto = new Entidades.sendmail();

                newobjeto.asunto = asunto;
                newobjeto.cuerpo = body;
                newobjeto.destino = destino;
                newobjeto.fec_creacion = DateTime.Now;
                newobjeto.id_sendmail = Guid.NewGuid();

                newobjeto.host = "smtp.gmail.com";
                newobjeto.origen = "fanegal20@gmail.com";
                newobjeto.puerto = "587";
                newobjeto.bit_ssl = true;


                send.Submit(ref newobjeto);
            }
            catch (Exception)
            {
                bit_error = true; 
            }

            return bit_error;
        }

    }
}
