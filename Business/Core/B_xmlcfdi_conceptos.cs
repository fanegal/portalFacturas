using Business.HerramientasCFDi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Business.Core
{
    public class B_xmlcfdi_conceptos
    {
        public bool bit_error;
        public Exception error;
        Data_Access.XMLCFDi_conceptos DAobj;

        public B_xmlcfdi_conceptos()
        {
            DAobj = new Data_Access.XMLCFDi_conceptos();
        }


        public void agregarConcepto(ComprobanteConcepto[] Conceptos, Guid id_xmlcfdi)
        {
            TextWriter stringWriter;
            XmlSerializer serializer;
            B_xmlcfdi_conceptos_impuestos b_impuestos;
            try
            {
                List<ComprobanteConcepto> _conceptos = Conceptos.ToList();
                foreach (ComprobanteConcepto concepto in _conceptos)
                {

                    Entidades.XMLCFDi_conceptos bdconcepto = new Entidades.XMLCFDi_conceptos();
                    bdconcepto.Cantidad = concepto.Cantidad;
                    bdconcepto.ClaveProdServ = concepto.ClaveProdServ;
                    bdconcepto.ClaveUnidad = concepto.ClaveUnidad;
                    bdconcepto.Descripcion = concepto.Descripcion;
                    bdconcepto.Descuento = concepto.Descuento;
                    bdconcepto.id_concepto = Guid.NewGuid();
                    bdconcepto.id_xmlcfdi = id_xmlcfdi;
                    bdconcepto.Importe = concepto.Importe;
                    bdconcepto.NoIdentificacion = string.IsNullOrEmpty(concepto.NoIdentificacion) ? string.Empty : concepto.NoIdentificacion;
                    bdconcepto.Unidad = string.IsNullOrEmpty(concepto.Unidad) ? string.Empty : concepto.Unidad;
                    bdconcepto.ValorUnitario = concepto.ValorUnitario;

                    if (concepto.InformacionAduanera != null)
                    {
                        stringWriter = new StringWriter();
                        serializer = new XmlSerializer(concepto.InformacionAduanera.GetType());
                        serializer.Serialize(stringWriter, concepto.InformacionAduanera);
                        bdconcepto.InformacionAduanera = stringWriter.ToString();
                    }
                    else
                    {
                        bdconcepto.InformacionAduanera = string.Empty;
                    }
                    if (concepto.CuentaPredial != null)
                    {
                        stringWriter = new StringWriter();
                        serializer = new XmlSerializer(concepto.CuentaPredial.GetType());
                        serializer.Serialize(stringWriter, concepto.CuentaPredial);
                        bdconcepto.CuentaPredial = stringWriter.ToString();
                    }
                    else
                    {
                        bdconcepto.CuentaPredial = string.Empty;
                    }
                    if (concepto.ComplementoConcepto != null)
                    {
                        stringWriter = new StringWriter();
                        serializer = new XmlSerializer(concepto.ComplementoConcepto.GetType());
                        serializer.Serialize(stringWriter, concepto.ComplementoConcepto);
                        bdconcepto.ComplementoConcepto = stringWriter.ToString();
                    }
                    else { bdconcepto.ComplementoConcepto = string.Empty; }
                    if (concepto.Parte != null)
                    {
                        stringWriter = new StringWriter();
                        serializer = new XmlSerializer(concepto.Parte.GetType());
                        serializer.Serialize(stringWriter, concepto.Parte);
                        bdconcepto.Parte = stringWriter.ToString();
                    }
                    else { bdconcepto.Parte = string.Empty; }


                    DAobj.sumit(ref bdconcepto);
                    if (DAobj.bit_error) { throw DAobj.Error; }

                    if (concepto.Impuestos != null)
                    {
                        b_impuestos = new B_xmlcfdi_conceptos_impuestos();
                        b_impuestos.agregarImpuesto(concepto.Impuestos, bdconcepto.id_concepto);
                        if (b_impuestos.bit_error) { throw b_impuestos.error; }
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
