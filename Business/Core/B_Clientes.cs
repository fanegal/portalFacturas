using Data_Access;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Business.Core
{
    public class B_Clientes
    {
        public cliente obj_cli;
        public bool bit_error;
        public Exception error;

        public B_Clientes()
        {
            obj_cli = new cliente();
        }

        public List<Entidades.cliente> Get_todos()
        {
            List<Entidades.cliente> List_result;
            try
            {
                List_result = obj_cli.getAll();
                if (obj_cli.bit_error) { throw obj_cli.Error; }
            }
            catch (System.Exception ex)
            {
                bit_error = true;
                error = ex;
                List_result = new List<Entidades.cliente>();
            }

            return List_result;
        }

        public List<Entidades.cliente> Get_clientesxusuario(string id)
        {
            List<Entidades.cliente> List_result;
            try
            {
                List_result = obj_cli.getList(x => x.Id == id && x.baja == null);
                if (obj_cli.bit_error) { throw obj_cli.Error; }
            }
            catch (System.Exception ex)
            {
                bit_error = true;
                error = ex;
                List_result = new List<Entidades.cliente>();
            }

            return List_result;
        }

        public Entidades.cliente getCliente(Guid id)
        {
            Entidades.cliente result;

            try
            {
                result = obj_cli.getSingle(x => x.id_cliente == id);
                if (obj_cli.bit_error) { throw obj_cli.Error; }
            }
            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                result = new Entidades.cliente();
            }
            return result;
        }

        public bool nuevo(Entidades.cliente obj)
        {
            bool correcto = true;
            try
            {
                if (obj == null) { throw new Exception("Información invalida"); }

                if (obj_cli.getList(m => m.RFC == obj.RFC && m.Id == obj.Id && obj.baja == null).Count == 0)
                {

                    obj.baja = null;
                    obj.fec_alta = DateTime.Now;
                    obj.id_cliente = Guid.NewGuid();

                    X509Certificate2 certifcate = new X509Certificate2(obj.pfx, obj.pass, X509KeyStorageFlags.MachineKeySet |
                                X509KeyStorageFlags.PersistKeySet |
                                X509KeyStorageFlags.Exportable);

                    var arrcer = certifcate.Subject.Split(',');

                    if (arrcer.Length > 1)
                    {
                        if (arrcer[4].Split('=')[1] == obj.RFC)
                        {
                            obj_cli.sumit(obj);
                            if (obj_cli.bit_error) { throw obj_cli.Error; }
                        }
                        else
                        {
                            throw new Exception("El Rfc del certificado no coincide con el capturado");
                        }
                    }
                    else
                    {
                        throw new Exception("El archivo pfx es corrupto ");
                    }
                }
                else
                {
                    throw new Exception("El contribuyente ya fue registrado con anterioridad ");
                }
            }
            catch (Exception ex)
            {
                correcto = false;
                bit_error = false;
                error = ex;
            }
            return correcto;
        }
    }
}
