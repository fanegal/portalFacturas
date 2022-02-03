using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Entidades;


namespace Data_Access
{
    public class parametros
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public parametros()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.parametros> getAll()
        {
            List<Entidades.parametros> List_result = new List<Entidades.parametros>();
            try
            {
                List_result = entities.parametros.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.parametros> getList(Expression<Func<Entidades.parametros, bool>> condicion)
        {
            List<Entidades.parametros> List_result = new List<Entidades.parametros>();
            try
            {
                List_result = entities.parametros.Where(condicion).ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.parametros getSingle(Expression<Func<Entidades.parametros, bool>> condicion)
        {
            Entidades.parametros result = new Entidades.parametros();
            try
            {
                result = entities.parametros.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.parametros();
            }

            return result;
        }

        public void Submit(ref Entidades.parametros objeto)
        {
            try
            {
                var llave = objeto.id_parametros;
                Entidades.parametros oldobjeto = entities.parametros.Where(t => t.id_parametros == llave).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.parametros.Add(objeto);
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
