using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access
{
    public class XMLCFDi_conceptos_impuestos
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public XMLCFDi_conceptos_impuestos()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();

        }

        public List<Entidades.XMLCFDi_conceptos_impuestos> getAll()
        {
            List<Entidades.XMLCFDi_conceptos_impuestos> List_result = new List<Entidades.XMLCFDi_conceptos_impuestos>();
            try
            {
                List_result = entities.XMLCFDi_conceptos_impuestos.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.XMLCFDi_conceptos_impuestos> getList(Expression<Func<Entidades.XMLCFDi_conceptos_impuestos, bool>> condicion)
        {
            List<Entidades.XMLCFDi_conceptos_impuestos> List_result = new List<Entidades.XMLCFDi_conceptos_impuestos>();
            try
            {
                List_result = entities.XMLCFDi_conceptos_impuestos.Where(condicion).ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.XMLCFDi_conceptos_impuestos getSingle(Expression<Func<Entidades.XMLCFDi_conceptos_impuestos, bool>> condicion)
        {
            Entidades.XMLCFDi_conceptos_impuestos result = new Entidades.XMLCFDi_conceptos_impuestos();
            try
            {
                result = entities.XMLCFDi_conceptos_impuestos.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.XMLCFDi_conceptos_impuestos();
            }

            return result;
        }

        public void sumit(ref Entidades.XMLCFDi_conceptos_impuestos objeto)
        {
            string valor = string.Empty;
            try
            {
                var llave = objeto.id_concepto;
                Entidades.XMLCFDi_conceptos_impuestos oldobjeto = entities.XMLCFDi_conceptos_impuestos.Where(t => t.id_concepto_impuesto == llave).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.XMLCFDi_conceptos_impuestos.Add(objeto);
                }
                else
                {
                    var props = objeto.GetType().GetProperties();
                    foreach (System.Reflection.PropertyInfo prop in props.ToList())
                    {
                        if (prop.Name.ToLower() != "xmlcfdi_conceptos")
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
