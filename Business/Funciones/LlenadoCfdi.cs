using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Business.Core;
using Business.HerramientasCFDi;
using ricfdi;

namespace Business.Funciones
{
    public class LlenadoCfdi
    {

        public LlenadoCfdi() { }

        public void setIngresos(ref metadata _dataset, Guid idcfdi)
        {
            B_xmlcfdi cfdiB;
            B_cfdi_storage storageB;
            XmlSerializer mySerializer;
            XmlDocument doc;
            Comprobante cfdi;
            try
            {
                cfdiB = new B_xmlcfdi();
                var resp = cfdiB.GetXMLCFDi(idcfdi);
                if (cfdiB.bit_error) { throw cfdiB.error; }

                storageB = new B_cfdi_storage();
                var bd_cfdi = storageB.GetCfdi(resp.uuidtimbre);
                if (storageB.bit_error) { throw cfdiB.error; }

                if (bd_cfdi.pdf == null)
                {
                    if (!string.IsNullOrEmpty(bd_cfdi.xmltxt))
                    {
                        doc = new XmlDocument();
                        doc.LoadXml(bd_cfdi.xmltxt);

                        mySerializer = new XmlSerializer(typeof(Comprobante));
                        using (XmlReader reader = new XmlNodeReader(doc))
                        {
                            cfdi = (Comprobante)mySerializer.Deserialize(reader);


                        }

                        //Receptor
                        var re = _dataset.Receptor.NewReceptorRow();
                        re.Nombre = cfdi.Receptor.Nombre;
                        re.Rfc = cfdi.Receptor.Rfc;
                        re.UsoCFDI = cfdi.Receptor.UsoCFDI.ToString();
                        _dataset.Receptor.AddReceptorRow(re);
                        //Emisor
                        var em = _dataset.Emisor.NewEmisorRow();
                        em.Nombre = cfdi.Emisor.Nombre;
                        em.Rfc = cfdi.Emisor.Rfc;
                        em.RegimenFiscal = cfdi.Emisor.RegimenFiscal.ToString();
                        _dataset.Emisor.AddEmisorRow(em);

                        var c_row = _dataset.cfdi.NewcfdiRow();
                        c_row.certificado = cfdi.Certificado;
                        c_row.CondicionesDePago = cfdi.CondicionesDePago;
                        c_row.Descuento = cfdi.Descuento.ToString("c");
                        c_row.Fecha = cfdi.Fecha.ToString("yyyyy-MM-ddTHH:mm:ss");
                        c_row.Folio = cfdi.Folio;
                        c_row.FormaPago = cfdi.FormaPago.ToString().Replace("Item", "");
                        c_row.LugarExpedicion = cfdi.LugarExpedicion.ToString();
                        c_row.MetodoPago = cfdi.MetodoPago.ToString();
                        c_row.Moneda = cfdi.Moneda.ToString();
                        c_row.NoCertificado = cfdi.NoCertificado;
                        c_row.Sello = cfdi.Sello;
                        c_row.serie = cfdi.Serie;
                        c_row.SubTotal = cfdi.SubTotal.ToString("c");
                        c_row.TipoCambio = cfdi.TipoCambio.ToString();
                        c_row.TipoDeComprobante = cfdi.TipoDeComprobante == c_TipoDeComprobante.I ? "CFDI de Ingreso" : cfdi.TipoDeComprobante.ToString();
                        c_row.Total = cfdi.Total.ToString("c");
                        if (cfdi.Impuestos != null)
                        {
                            c_row.totalretenidos = cfdi.Impuestos.TotalImpuestosRetenidosSpecified ? cfdi.Impuestos.TotalImpuestosRetenidos.ToString() : "$0.00";
                            c_row.totaltraslados = cfdi.Impuestos.TotalImpuestosTrasladadosSpecified ? cfdi.Impuestos.TotalImpuestosTrasladados.ToString() : "$0.00";

                        }
                        tool_Cfdi herr = new tool_Cfdi();
                        c_row.QR = generarqr(string.Format("https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?{0}", herr.getStringQR(cfdi)));
                        _dataset.cfdi.AddcfdiRow(c_row);

                    
                        //conceptos
                        foreach (ComprobanteConcepto c in cfdi.Conceptos.ToList())
                        {

                            var concepto = _dataset.conceptos.NewconceptosRow();
                            concepto.Cantidad = c.Cantidad.ToString();
                            concepto.ClaveProdServ = c.ClaveProdServ;
                            concepto.ClaveUnidad = c.ClaveUnidad;
                            concepto.Descripcion = c.Descripcion;
                            concepto.Descuento = c.DescuentoSpecified ? c.Descuento.ToString("c") : "$0.00";
                            concepto.Importe = c.Importe.ToString("c");
                            concepto.NoIdentificacion = c.NoIdentificacion;
                            concepto.Unidad = c.Unidad;
                            concepto.ValorUnitario = c.ValorUnitario.ToString("c");
                            if (c.Impuestos != null)
                            {
                                if (c.Impuestos.Traslados != null)
                                {

                                    c.Impuestos.Traslados.ToList().ForEach(i =>
                                    {
                                        concepto.Descripcion = concepto.Descripcion + Environment.NewLine + string.Format("Traslado Importe={0} TasaOCuota={1} TipoFactor={2} Impuesto={3} Base={4}", i.Importe, i.TasaOCuotaSpecified ? i.TasaOCuota.ToString() : "NA", i.TipoFactor, i.Impuesto, i.Base.ToString("c"));
                                    });
                                }
                                if (c.Impuestos.Retenciones != null)
                                {
                                    c.Impuestos.Retenciones.ToList().ForEach(i =>
                                    {
                                        concepto.Descripcion = concepto.Descripcion + Environment.NewLine + string.Format("Impuesto retenido {0} tasa:{1} base:{2}", i.Impuesto, i.TasaOCuota.ToString("p"), i.Base.ToString("c"));
                                    });
                                }
                            }
                            _dataset.conceptos.AddconceptosRow(concepto);
                        }

                    }

                }

            }
            catch (Exception ex)
            {

                throw;
            }


        }


        public byte[] generarqr(string valor)
        {
            byte[] resp;
            try
            {
                QRCoder.QRCodeGenerator QR = new QRCoder.QRCodeGenerator();
                ASCIIEncoding ASSCII = new ASCIIEncoding();
                var z = QR.CreateQrCode(ASSCII.GetBytes(valor), QRCoder.QRCodeGenerator.ECCLevel.H);
                QRCoder.PngByteQRCode png = new QRCoder.PngByteQRCode();
                png.SetQRCodeData(z);
                resp = png.GetGraphic(10);
            }
            catch (Exception)
            {

                throw;
            }
            return resp;
        }
    }
}
