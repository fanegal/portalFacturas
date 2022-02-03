using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Entidades;

namespace Data_Access
{
    public class logs
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public logs()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.logs> getAll()
        {
            List<Entidades.logs> List_result = new List<Entidades.logs>();
            try
            {
                List_result = entities.logs.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.logs> getList(Expression<Func<Entidades.logs, bool>> condicion)
        {
            List<Entidades.logs> List_result = new List<Entidades.logs>();
            try
            {
                List_result = entities.logs.Where(condicion).ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.logs getSingle(Expression<Func<Entidades.logs, bool>> condicion)
        {
            Entidades.logs result = new Entidades.logs();
            try
            {
                result = entities.logs.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.logs();
            }

            return result;
        }

        public void Submit(Entidades.logs objeto)
        {
            try
            {
                var llave = objeto.id_logs;
                Entidades.logs oldobjeto = entities.logs.Where(t => t.id_logs == llave).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.logs.Add(objeto);
                }
                else
                {
                    var props = objeto.GetType().GetProperties();

                    foreach (System.Reflection.PropertyInfo prop in props.ToList())
                    {

                        prop.SetValue(oldobjeto, prop.GetValue(objeto));
                    }
                }
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;

            }

        }

    }
}
