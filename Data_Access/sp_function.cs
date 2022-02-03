using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Data_Access
{
    public class sp_function
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public sp_function()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public string getpasshasuser(string valor)
        {
            string result = string.Empty;
            try
            {
                var response = entities.Database.SqlQuery<string>("select dbo.getpasshasuser(@MAIL)", new SqlParameter("MAIL", valor)).FirstOrDefault();
                if (response.Count() > 0)
                {
                    result = response;
                }
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }
            return result;

        }

        public string getiduser(string valor)
        {
            string result = string.Empty;
            try
            {
                var response = entities.Database.SqlQuery<string>("select dbo.fn_getiduser(@dato)", new SqlParameter("dato", valor)).FirstOrDefault();
                if (response.Count() > 0)
                {
                    result = response;
                }
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }
            return result;

        }

        public List<sp_peticiondescarga_Result> sp_peticiondescarga(string mail, string rfc)
        {
            List<sp_peticiondescarga_Result> listresult;
            try
            {
                var response = entities.sp_peticiondescarga(mail, rfc).ToList();
                if (response.Count > 0)
                {
                    listresult = response;
                }
                else
                {
                    listresult = new List<sp_peticiondescarga_Result>();
                }
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                listresult = new List<sp_peticiondescarga_Result>();
            }
            return listresult;
        }

        public List<sp_cfdiER_Result> sp_cfdiER(Guid id, bool tipo)
        {
            List<sp_cfdiER_Result> listresult;
            try
            {
                var response = entities.sp_cfdiER(id, tipo).ToList();
                if (response.Count > 0)
                {
                    listresult = response;
                }
                else
                {
                    listresult = new List<sp_cfdiER_Result>();
                }
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                listresult = new List<sp_cfdiER_Result>();
            }
            return listresult;
        }
    }
}
