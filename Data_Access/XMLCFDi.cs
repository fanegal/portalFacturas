using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data_Access
{
    public class XMLCFDi
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public XMLCFDi()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.XMLCFDi> getAll()
        {
            List<Entidades.XMLCFDi> List_result = new List<Entidades.XMLCFDi>();
            try
            {
                List_result = entities.XMLCFDi.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.XMLCFDi> getList(System.Linq.Expressions.Expression<Func<Entidades.XMLCFDi, bool>> condicion)
        {
            List<Entidades.XMLCFDi> List_result = new List<Entidades.XMLCFDi>();
            try
            {
                List_result = entities.XMLCFDi.Where(condicion).ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.XMLCFDi getSingle(System.Linq.Expressions.Expression<Func<Entidades.XMLCFDi, bool>> condicion)
        {
            Entidades.XMLCFDi result = new Entidades.XMLCFDi();
            try
            {
                result = entities.XMLCFDi.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.XMLCFDi();
            }

            return result;
        }

        public void sumit(ref Entidades.XMLCFDi objeto)
        {
            string valor = string.Empty;
            try
            {
                var llave = objeto.id_peticion;
                Entidades.XMLCFDi oldobjeto = entities.XMLCFDi.Where(t => t.id_xmlcfdi == llave).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.XMLCFDi.Add(objeto);
                }
                else
                {
                    var props = objeto.GetType().GetProperties();
                    foreach (System.Reflection.PropertyInfo prop in props.ToList())
                    {
                        if (prop.Name.ToLower() != "peticion" && prop.Name.ToLower() != "xmlcfdi_conceptos")
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
