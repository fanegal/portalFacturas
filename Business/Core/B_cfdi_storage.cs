using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Core
{
    public class B_cfdi_storage
    {
        public bool bit_error;
        public Exception error;
        Data_Access.cfdi_storage storage;

        public B_cfdi_storage()
        {
            storage = new Data_Access.cfdi_storage();
        }

        public Entidades.cfdi_storage GetCfdi(Guid uuid)
        {
            Entidades.cfdi_storage result;
            try
            {
                result = storage.getSingle(k => k.uuid == uuid);
                if (storage.bit_error) { throw storage.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                result = new Entidades.cfdi_storage();
            }
            return result;
        }

    }
}
