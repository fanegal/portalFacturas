using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Data_Access
{
    public class cliente
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public cliente()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.cliente> getAll()
        {
            List<Entidades.cliente> List_result = new List<Entidades.cliente>();
            try
            {
                List_result = entities.cliente.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.cliente> getList(System.Linq.Expressions.Expression<Func<Entidades.cliente, bool>> condicion)
        {
            List<Entidades.cliente> List_result = new List<Entidades.cliente>();
            try
            {
                List_result = entities.cliente.Where(condicion).ToList();


            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.cliente getSingle(System.Linq.Expressions.Expression<Func<Entidades.cliente, bool>> condicion)
        {
            Entidades.cliente result = new Entidades.cliente();
            try
            {
                result = entities.cliente.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.cliente();
            }

            return result;
        }

        public void sumit(Entidades.cliente objeto)
        { 
            try
            {
                Entidades.cliente oldobjeto = entities.cliente.Where(t => t.id_cliente == objeto.id_cliente).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.cliente.Add(objeto);
                }
                else
                {
                    var props = objeto.GetType().GetProperties();

                    foreach (System.Reflection.PropertyInfo prop in props.ToList())
                    {
                        if (prop.Name.ToLower() != "peticion")
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
