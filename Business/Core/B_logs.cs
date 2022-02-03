using Data_Access;
using System;
using System.Collections.Generic;

namespace Business.Core
{
    public class B_logs
    {

        public logs obj;
        public bool bit_error;
        public Exception error;


        public B_logs()
        {
            obj = new logs();
        }



        public void new_log(string usuario, string descripcion, string maquina)
        {
            Entidades.logs log;
            try
            {
                log = new Entidades.logs();
                log.bit_error = false;
                log.fec_create = DateTime.Now;
                log.id_logs = Guid.NewGuid();
                log.id_user = usuario;
                log.maquina = maquina;
                log.descripcion = descripcion;

                obj.Submit(log);
                if (obj.bit_error) { throw obj.Error; }
            }
            catch (Exception)
            {

                throw;
            }
        
        }

        public void new_log_error(string usuario, string descripcion, string maquina)
        {
            Entidades.logs log;
            try
            {
                log = new Entidades.logs();
                log.bit_error = true;
                log.fec_create = DateTime.Now;
                log.id_logs = Guid.NewGuid();
                log.id_user = usuario;
                log.maquina = maquina;
                log.descripcion = descripcion;

                obj.Submit(log);
                if (obj.bit_error) { throw obj.Error; }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
