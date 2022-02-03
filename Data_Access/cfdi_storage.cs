using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Data_Access
{
    public class cfdi_storage
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public cfdi_storage()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.cfdi_storage> getAll()
        {
            List<Entidades.cfdi_storage> List_result = new List<Entidades.cfdi_storage>();
            try
            {
                List_result = entities.cfdi_storage.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.cfdi_storage> getList(System.Linq.Expressions.Expression<Func<Entidades.cfdi_storage, bool>> condicion)
        {
            List<Entidades.cfdi_storage> List_result = new List<Entidades.cfdi_storage>();
            try
            {
                List_result = entities.cfdi_storage.Where(condicion).ToList();


            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.cfdi_storage getSingle(System.Linq.Expressions.Expression<Func<Entidades.cfdi_storage, bool>> condicion)
        {
            Entidades.cfdi_storage result = new Entidades.cfdi_storage();
            try
            {
                result = entities.cfdi_storage.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.cfdi_storage();
            }

            return result;
        }

        public void sumit(Entidades.cfdi_storage objeto)
        {
            try
            {
                Entidades.cfdi_storage oldobjeto = entities.cfdi_storage.Where(t => t.uuid == objeto.uuid).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.cfdi_storage.Add(objeto);
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

                throw;
            }

        }
    }
}
