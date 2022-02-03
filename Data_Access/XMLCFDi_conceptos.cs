using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data_Access
{
    public class XMLCFDi_conceptos
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;


        public XMLCFDi_conceptos()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.XMLCFDi_conceptos> getAll()
        {
            List<Entidades.XMLCFDi_conceptos> List_result = new List<Entidades.XMLCFDi_conceptos>();
            try
            {
                List_result = entities.XMLCFDi_conceptos.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.XMLCFDi_conceptos> getList(Expression<Func<Entidades.XMLCFDi_conceptos, bool>> condicion)
        {
            List<Entidades.XMLCFDi_conceptos> List_result = new List<Entidades.XMLCFDi_conceptos>();
            try
            {
                List_result = entities.XMLCFDi_conceptos.Where(condicion).ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.XMLCFDi_conceptos getSingle(Expression<Func<Entidades.XMLCFDi_conceptos, bool>> condicion)
        {
            Entidades.XMLCFDi_conceptos result = new Entidades.XMLCFDi_conceptos();
            try
            {
                result = entities.XMLCFDi_conceptos.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.XMLCFDi_conceptos();
            }

            return result;
        }


        public void sumit(ref Entidades.XMLCFDi_conceptos objeto)
        {
            string valor = string.Empty;
            try
            {
                var llave = objeto.id_concepto;
                Entidades.XMLCFDi_conceptos oldobjeto = entities.XMLCFDi_conceptos.Where(t => t.id_concepto == llave).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.XMLCFDi_conceptos.Add(objeto);
                }
                else
                {
                    var props = objeto.GetType().GetProperties();
                    foreach (System.Reflection.PropertyInfo prop in props.ToList())
                    {
                        if (prop.Name.ToLower() != "xmlcfdi" && prop.Name.ToLower() != "xmlcfdi_conceptos_impuestos")
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
