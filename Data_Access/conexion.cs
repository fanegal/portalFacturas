using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Data_Access
{
   public class conexion
    {
        Entities_dbcore conn;
        public conexion()
        { }

        public Entities_dbcore star_dbcore()
        {
            conn = new Entities_dbcore();
            return conn;     
        }
         
    }
}
