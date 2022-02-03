using Data_Access;
using System;
using System.Collections.Generic;

namespace Business.Core
{
    public class B_sp_function
    {
        public sp_function obj;
        public bool bit_error;
        public Exception error;

        public B_sp_function()
        {
            obj = new sp_function();
        }


        public string function_getpasshasuser(string valor)
        {
            string result;

            try
            {
                result = obj.getpasshasuser(valor);
                if (obj.bit_error) { throw obj.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                result = string.Empty;
            }
            return result;
        }

        public string function_getiduser(string valor)
        {
            string result;
            try
            {
                result = obj.getiduser(valor);
                if (obj.bit_error) { throw obj.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                result = string.Empty;
            }
            return result;
        }


        public List<Entidades.sp_peticiondescarga_Result> sp_peticiondescarga(string mail, string rfc)
        {
            List<Entidades.sp_peticiondescarga_Result> result;
            try
            {
                result = obj.sp_peticiondescarga(mail, rfc);
                if (obj.bit_error) { throw obj.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                result = new List<Entidades.sp_peticiondescarga_Result>();
            }
            return result;
        }

        public List<Entidades.sp_cfdiER_Result> sp_cfdiER(Guid id, bool emitido)
        {
            List<Entidades.sp_cfdiER_Result> result;
            try
            {
                result = obj.sp_cfdiER(id, emitido);
                if (obj.bit_error) { throw obj.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                result = new List<Entidades.sp_cfdiER_Result>();
            }
            return result;
        }
    }
}
