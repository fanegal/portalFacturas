using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Core
{
    public class B_Peticiones
    {
        public bool bit_error;
        public Exception error;
        Data_Access.peticion pet;
        public B_Peticiones()
        {
            pet = new Data_Access.peticion();
        }

        public List<Entidades.peticion> Sintoken()
        {
            List<Entidades.peticion> List_result;
            try
            {
                List_result = pet.getList(k => k.bit_error == false && string.IsNullOrEmpty(k.token) == true && k.bit_terminado == false).Take(10).ToList();
                if (pet.bit_error) { throw pet.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                List_result = new List<Entidades.peticion>();
            }
            return List_result;

        }

        public List<Entidades.peticion> Sinsolicitud()
        {
            List<Entidades.peticion> List_result;
            try
            {
                List_result = pet.getList(k => k.bit_error == false && string.IsNullOrEmpty(k.idsolicitud) == true && k.bit_terminado == false && string.IsNullOrEmpty(k.token) == false);
                if (pet.bit_error) { throw pet.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                List_result = new List<Entidades.peticion>();
            }
            return List_result;

        }

        public List<Entidades.peticion> Sinpaquete()
        {
            List<Entidades.peticion> List_result;
            try
            {
                List_result = pet.getList(k => k.bit_error == false && string.IsNullOrEmpty(k.idpaquete) == true && string.IsNullOrEmpty(k.idsolicitud) == false && k.bit_terminado == false && string.IsNullOrEmpty(k.token) == false);
                if (pet.bit_error) { throw pet.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                List_result = new List<Entidades.peticion>();
            }
            return List_result;

        }

        public List<Entidades.peticion> Sindescarga()
        {
            List<Entidades.peticion> List_result;
            try
            {
                List_result = pet.getList(k => k.bit_error == false && string.IsNullOrEmpty(k.descargaresponse) == true && string.IsNullOrEmpty(k.idpaquete) == false && string.IsNullOrEmpty(k.idsolicitud) == false && k.bit_terminado == false && string.IsNullOrEmpty(k.token) == false);
                if (pet.bit_error) { throw pet.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                List_result = new List<Entidades.peticion>();
            }
            return List_result;

        }

        public List<Entidades.peticion> Sindescomprimir()
        {
            List<Entidades.peticion> List_result;
            try
            {
                List_result = pet.getList(k => k.bit_error == false && string.IsNullOrEmpty(k.descargaresponse) == false && k.bit_terminado == false);
                if (pet.bit_error) { throw pet.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                List_result = new List<Entidades.peticion>();
            }
            return List_result;
        }

        public List<Guid> GetPeticionEmitidosNoError()
        {
            List<Guid> Listresult;
            try
            {
                Listresult = new List<Guid>();
                List<Entidades.peticion> Listgeneric = pet.getList(t => t.bit_emitido == true && t.bit_terminado == false && t.bit_error == false);
                if (pet.bit_error) { throw pet.Error; }
                Listgeneric.ForEach(k =>
                {
                    Listresult.Add(k.id_peticion);
                });

            }
            catch (Exception ex)
            {
                Listresult = new List<Guid>();
            }
            return Listresult;
        }
        public List<Guid> GetPeticionRecibidosNoError()
        {
            List<Guid> Listresult;
            try
            {
                Listresult = new List<Guid>();
                List<Entidades.peticion> Listgeneric = pet.getList(t => t.bit_emitido == false && t.bit_terminado == false && t.bit_error == false);
                if (pet.bit_error) { throw pet.Error; }
                Listgeneric.ForEach(k =>
                {
                    Listresult.Add(k.id_peticion);
                });

            }
            catch (Exception ex)
            {
                Listresult = new List<Guid>();
            }
            return Listresult;
        }

        public List<Entidades.peticion> GetPeticionsxCliente(Guid id)
        {
            List<Entidades.peticion> List_result;
            try
            {
                List_result = pet.getList(k => k.id_cliente == id).OrderByDescending(f => f.fec_create).ToList();
                if (pet.bit_error) { throw pet.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                List_result = new List<Entidades.peticion>();
            }
            return List_result;
        }

        public Entidades.peticion getPeticion(Guid guid)
        {
            Entidades.peticion result;
            try
            {
                result = pet.getSingle(k => k.id_peticion == guid);
                if (pet.bit_error) { throw pet.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                result = new Entidades.peticion();
            }
            return result;
        }

        public bool create(Guid id_cliente, DateTime fecinicio, DateTime fecfin, bool emitido)
        {
            bool correcto = true;
            Entidades.peticion newobjeto;

            try
            {
                newobjeto = new Entidades.peticion();

                newobjeto.id_peticion = Guid.NewGuid();
                newobjeto.id_cliente = id_cliente;
                newobjeto.fec_create = DateTime.Now;
                newobjeto.fec_inicio = fecinicio;
                newobjeto.fec_fin = fecfin.Date.AddDays(+1).AddSeconds(-1);
                newobjeto.bit_emitido = emitido;

                newobjeto.bit_error = false;
                newobjeto.bit_terminado = false;
                newobjeto.bit_automatico = false;
                newobjeto.bit_descargado = false;
                newobjeto.intentos = 0;

                pet.Submit(ref newobjeto);
                if (pet.bit_error) { throw pet.Error; }

            }
            catch (Exception ex)
            {
                correcto = false;
                bit_error = true;
                error = ex;
            }
            return correcto;
        }

        public void newPeticionAuto(Entidades.peticion Oldobjeto)
        {
            Entidades.peticion newobjeto;
            Data_Access.peticion pet;
            try
            {   //realizar esto si la peticion fue generada automaticamente 
                if (Oldobjeto.bit_automatico)
                {
                    pet = new Data_Access.peticion();
                    newobjeto = new Entidades.peticion();
                    newobjeto.id_cliente = Oldobjeto.id_cliente;
                    newobjeto.id_peticion = Guid.NewGuid();
                    newobjeto.fec_create = DateTime.Now;
                    newobjeto.bit_emitido = Oldobjeto.bit_emitido;
                    newobjeto.bit_error = false;
                    newobjeto.bit_terminado = false;
                    newobjeto.fec_create = Oldobjeto.fec_fin.AddMinutes(1);
                    newobjeto.fec_fin = newobjeto.fec_inicio.AddHours(23).AddMinutes(59).AddSeconds(59);
                    pet.Submit(ref newobjeto);
                    if (pet.bit_error) { throw pet.Error; }
                }
                //newobjeto.fec_inicio = 
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
            }

        }

        public void procesadescargados(List<Entidades.sp_peticiondescarga_Result> listado)
        {

            Entidades.peticion result;
            try
            {
                foreach (Entidades.sp_peticiondescarga_Result fila in listado)
                {
                    result = pet.getSingle(k => k.id_peticion == fila.id_peticion);
                    if (pet.bit_error) { throw pet.Error; }
                    result.bit_descargado = true;
                    pet.Submit(ref result);
                    if (pet.bit_error) { throw pet.Error; }
                }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
            }
        }
    }
}
