using Business.HerramientasCFDi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Core
{
    public class B_xmlcfdi_conceptos_impuestos
    {
        public bool bit_error;
        public Exception error;
        Data_Access.XMLCFDi_conceptos_impuestos DAobj;

        public B_xmlcfdi_conceptos_impuestos()
        {
            DAobj = new Data_Access.XMLCFDi_conceptos_impuestos();
        }

        public void agregarImpuesto(ComprobanteConceptoImpuestos Impuestos , Guid id_concepto)
        {

            try
            {
                if (Impuestos.Retenciones != null)
                {
                    if (Impuestos.Retenciones.Count() > 0)
                    {
                        List<ComprobanteConceptoImpuestosRetencion> retenciones = Impuestos.Retenciones.ToList();
                        foreach (ComprobanteConceptoImpuestosRetencion retencion in retenciones) {
                            Entidades.XMLCFDi_conceptos_impuestos ret = new Entidades.XMLCFDi_conceptos_impuestos();
                            ret.Base = retencion.Base;
                            ret.bit_traslado = false;
                            ret.id_concepto = id_concepto;
                            ret.id_concepto_impuesto = Guid.NewGuid();
                            ret.Importe = retencion.Importe;
                            ret.Impuesto = retencion.Impuesto;
                            ret.TasaOCuota = retencion.TasaOCuota;
                            ret.TipoFactor = retencion.TipoFactor;

                            DAobj.sumit(ref ret);
                            if (DAobj.bit_error) { throw DAobj.Error; }
                        }
                    }
                }
                if (Impuestos.Traslados != null)
                {
                    if (Impuestos.Traslados.Count() > 0)
                    {
                        List<ComprobanteConceptoImpuestosTraslado> Traslados = Impuestos.Traslados.ToList();
                        foreach (ComprobanteConceptoImpuestosTraslado traslado in Traslados)
                        {
                            Entidades.XMLCFDi_conceptos_impuestos tras= new Entidades.XMLCFDi_conceptos_impuestos();
                            tras.Base = traslado.Base;
                            tras.bit_traslado = true;
                            tras.id_concepto = id_concepto;
                            tras.id_concepto_impuesto = Guid.NewGuid();
                            tras.Importe = traslado.Importe;
                            tras.Impuesto = traslado.Impuesto;
                            tras.TasaOCuota = traslado.TasaOCuota;
                            tras.TipoFactor = traslado.TipoFactor;

                            DAobj.sumit(ref tras);
                            if (DAobj.bit_error) { throw DAobj.Error; }
                        }
                    }
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
