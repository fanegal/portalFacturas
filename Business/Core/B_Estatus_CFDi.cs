using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Business.Core
{
    public class B_Estatus_CFDi
    {
        Data_Access.Estatus_CFDi est;
        public B_Estatus_CFDi()
        {
            est = new Data_Access.Estatus_CFDi();
        }

        public List<ListItem> getPeticiones()
        {
            List<ListItem> result;
            try
            {
                result = new List<ListItem>();
                List<Entidades.Estatus_CFDi> list_result  = est.getList(k => k.bit_consultado == false);
                list_result.ForEach(h =>
                {
                    result.Add(new ListItem { Value = h.id.ToString(), Text = h.impresion });
                });
            }
            catch (Exception)
            {
                result = new List<ListItem>();
            }
            return result;
        }
    }
}
