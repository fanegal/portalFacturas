using Data_Access;
using System;
using System.Collections.Generic;
namespace Business.Core
{
    public class B_parametros
    {

        public parametros obj;
        public bool bit_error;
        public Exception error;



        public B_parametros()
        {
            obj = new parametros();
        }


        public Entidades.parametros GetParametro(string llave)
        {
            Entidades.parametros result;

            try
            {
                result = obj.getSingle(x => x.nombre == llave);
                if (obj.bit_error) { throw obj.Error; }
            }

            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
                result = new Entidades.parametros();
            }
            return result;
        }


        public void SetParametro(string llave, string valor)
        {

            Entidades.parametros result;

            try
            {
                result = obj.getSingle(x => x.nombre == llave);
                if (obj.bit_error) { throw obj.Error; }

                result.valor = valor;

                obj.Submit(ref result);
                if (obj.bit_error) { throw obj.Error; }
            }

            catch (Exception ex)
            {
                bit_error = true;
                error = ex;
            }
        }
    }
}
