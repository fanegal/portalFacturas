using Entidades;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace Data_Access
{
    public class Estatus_CFDi
    {
        private conexion Oconexion;
        private Entities_dbcore entities;
        public bool bit_error;
        public Exception Error;

        public Estatus_CFDi()
        {
            Oconexion = new conexion();
            entities = Oconexion.star_dbcore();
        }

        public List<Entidades.Estatus_CFDi> getAll()
        {
            List<Entidades.Estatus_CFDi> List_result = new List<Entidades.Estatus_CFDi>();
            try
            {
                List_result = entities.Estatus_CFDi.ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public List<Entidades.Estatus_CFDi> getList(System.Linq.Expressions.Expression<Func<Entidades.Estatus_CFDi, bool>> condicion)
        {
            List<Entidades.Estatus_CFDi> List_result = new List<Entidades.Estatus_CFDi>();
            try
            {
                List_result = entities.Estatus_CFDi.Where(condicion).ToList();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
            }

            return List_result;
        }

        public Entidades.Estatus_CFDi getSingle(System.Linq.Expressions.Expression<Func<Entidades.Estatus_CFDi, bool>> condicion)
        {
            Entidades.Estatus_CFDi result = new Entidades.Estatus_CFDi();
            try
            {
                result = entities.Estatus_CFDi.Where(condicion).Single();
            }
            catch (Exception ex)
            {
                bit_error = true;
                Error = ex;
                result = new Entidades.Estatus_CFDi();
            }

            return result;
        }

        public void sumit(Entidades.Estatus_CFDi objeto)
        {
            try
            {
                Entidades.Estatus_CFDi oldobjeto = entities.Estatus_CFDi.Where(t => t.id == objeto.id).FirstOrDefault();
                if (oldobjeto == null)
                {
                    entities.Estatus_CFDi.Add(objeto);
                }
                else
                {
                    oldobjeto.GetType().GetProperties().ToList().ForEach(t =>
                    {
                        t.SetValue(oldobjeto, t.GetValue(objeto));
                    });
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
