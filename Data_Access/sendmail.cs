using Entidades;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Linq.Expressions;

namespace Data_Access
{
    public class sendmail
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;


        public sendmail()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.sendmail> getAll()
        {
            List<Entidades.sendmail> List_result = new List<Entidades.sendmail>();
            try
            {
                List_result = entities.sendmail.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.sendmail> getList(Expression<Func<Entidades.sendmail, bool>> condicion)
        {
            List<Entidades.sendmail> List_result = new List<Entidades.sendmail>();
            try
            {
                List_result = entities.sendmail.Where(condicion).ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.sendmail getSingle(Expression<Func<Entidades.sendmail, bool>> condicion)
        {
            Entidades.sendmail result = new Entidades.sendmail();
            try
            {
                result = entities.sendmail.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.sendmail();
            }

            return result;
        }

        public void Submit(ref Entidades.sendmail objeto)
        {
            try
            {
                var llave = objeto.id_sendmail;
                Entidades.sendmail oldobjeto = entities.sendmail.Where(t => t.id_sendmail == llave).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.sendmail.Add(objeto);
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
