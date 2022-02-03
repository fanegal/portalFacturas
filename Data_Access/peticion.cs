using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Entidades;

namespace Data_Access
{
    public class peticion
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public peticion()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.peticion> getAll()
        {
            List<Entidades.peticion> List_result = new List<Entidades.peticion>();
            try
            {
                List_result = entities.peticion.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.peticion> getList(Expression<Func<Entidades.peticion, bool>> condicion)
        {
            List<Entidades.peticion> List_result = new List<Entidades.peticion>();
            try
            {
                List_result = entities.peticion.Where(condicion).ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.peticion getSingle(Expression<Func<Entidades.peticion, bool>> condicion)
        {
            Entidades.peticion result = new Entidades.peticion();
            try
            {
                result = entities.peticion.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.peticion();
            }

            return result;
        }

        public void Submit(ref Entidades.peticion objeto)
        {
            try
            {
                var llave = objeto.id_peticion;
                Entidades.peticion oldobjeto = entities.peticion.Where(t => t.id_peticion == llave).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.peticion.Add(objeto);
                }
                else
                {
                    var props = objeto.GetType().GetProperties();

                    foreach (System.Reflection.PropertyInfo prop in props.ToList())
                    {
                        if (prop.Name.ToLower() != "cliente" && prop.Name.ToLower() != "xmlcfdi")
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
