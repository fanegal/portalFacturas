using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Core
{

    public class B_xmlcfdi
    {
        public bool bit_error;
        public Exception error;
        Data_Access.XMLCFDi DAobj;

        public B_xmlcfdi()
        {
            DAobj = new Data_Access.XMLCFDi();
        }

        public List<Entidades.XMLCFDi> cfdixpeticion(Guid id)
        {
            List<Entidades.XMLCFDi> List_result;

            try
            {
                List_result = DAobj.getList(k => k.id_peticion == id).ToList();
                if (DAobj.bit_error) { throw DAobj.Error; }
            }
            catch (Exception ex)
            {

                bit_error = true;
                error = ex;
                List_result = new List<Entidades.XMLCFDi>();
            }

            return List_result;
        }

        public Entidades.XMLCFDi GetXMLCFDi(Guid id)
        {
            Entidades.XMLCFDi objeto;

            try
            {
                objeto = DAobj.getSingle(k => k.id_xmlcfdi == id);
                if (DAobj.bit_error) { throw DAobj.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                objeto = new Entidades.XMLCFDi();

            }

            return objeto;
        }
    }
}
