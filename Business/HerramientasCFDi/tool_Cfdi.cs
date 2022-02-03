using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;
using Business.EstatusSAT;

namespace Business.HerramientasCFDi
{
    public class tool_Cfdi
    {

        public int guardar_CFdis(string zip, Guid idpeticion)
        {
            int num_cfdi = 0;
            Business.Core.B_xmlcfdi_conceptos bconceptos;
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zip))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            XmlSerializer mySerializer = new XmlSerializer(typeof(Comprobante));
                            Comprobante cfdi = (Comprobante)mySerializer.Deserialize(entry.Open());
                            Nomina n12 = new Nomina();
                            TimbreFiscalDigital tfd = new TimbreFiscalDigital();
                            cfdi.Complemento.ToList().ForEach(k =>
                            {
                                StringReader stringReader = new StringReader(k.Any.ToList().Where(s => s.Name == "tfd:TimbreFiscalDigital").FirstOrDefault().OuterXml);
                                mySerializer = new XmlSerializer(typeof(TimbreFiscalDigital));
                                tfd = (TimbreFiscalDigital)mySerializer.Deserialize(stringReader);

                                if (k.Any.ToList().Where(s => s.Name == "nomina12:Nomina").ToList().Count > 0)
                                {
                                    stringReader = new StringReader(k.Any.ToList().Where(s => s.Name == "nomina12:Nomina").FirstOrDefault().OuterXml);
                                    mySerializer = new XmlSerializer(typeof(Nomina));
                                    n12 = (Nomina)mySerializer.Deserialize(stringReader);
                                }
                            });

                            string ruta = "c:\\comprobantes\\";
                            if (!Directory.Exists(ruta)) { Directory.CreateDirectory(ruta); }
                            ruta += tfd.FechaTimbrado.Year.ToString() + "\\";
                            if (!Directory.Exists(ruta)) { Directory.CreateDirectory(ruta); }
                            ruta += tfd.FechaTimbrado.Month.ToString() + "\\";
                            if (!Directory.Exists(ruta)) { Directory.CreateDirectory(ruta); }
                            ruta += tfd.FechaTimbrado.Day.ToString() + "\\";
                            if (!Directory.Exists(ruta)) { Directory.CreateDirectory(ruta); }
                            ruta += cfdi.Emisor.Rfc + "\\";
                            if (!Directory.Exists(ruta)) { Directory.CreateDirectory(ruta); }
                            ruta += cfdi.Receptor.Rfc + "\\";
                            if (!Directory.Exists(ruta)) { Directory.CreateDirectory(ruta); }

                            ruta += tfd.UUID + ".xml";

                            Guid timbre = Guid.Parse(tfd.UUID);
                            Entidades.XMLCFDi c = new Entidades.XMLCFDi();
                            c.id_peticion = idpeticion;
                            c.id_xmlcfdi = Guid.NewGuid();
                            c.bit_vigente = true;
                            c.descuento = cfdi.Descuento;
                            c.estatusCancelacion = "vigente";
                            c.fechatimbre = tfd.FechaTimbrado;
                            if (cfdi.FormaPagoSpecified)
                            {
                                switch (cfdi.FormaPago)
                                {
                                    case c_FormaPago.Item01:
                                        c.formapago = "01-Efectivo";
                                        break;
                                    case c_FormaPago.Item02:
                                        c.formapago = "02-Cheque Nominativo";
                                        break;
                                    case c_FormaPago.Item03:
                                        c.formapago = "03-Transferencia Electrónica de Fondos SPEI";
                                        break;
                                    case c_FormaPago.Item04:
                                        c.formapago = "04-Tarjeta de Crédito";
                                        break;
                                    case c_FormaPago.Item05:
                                        c.formapago = "05-Monedero Electrónico";
                                        break;
                                    case c_FormaPago.Item06:
                                        c.formapago = "06-Dinero Electrónico";
                                        break;
                                    case c_FormaPago.Item08:
                                        c.formapago = "08-Vales de Despensa";
                                        break;
                                    case c_FormaPago.Item12:
                                        c.formapago = "12-Dación en Pago";
                                        break;
                                    case c_FormaPago.Item13:
                                        c.formapago = "13-Pago por Subrogación";
                                        break;
                                    case c_FormaPago.Item14:
                                        c.formapago = "14-Pago por Consignación";
                                        break;
                                    case c_FormaPago.Item15:
                                        c.formapago = "15-Condonación";
                                        break;
                                    case c_FormaPago.Item17:
                                        c.formapago = "17-Compensación";
                                        break;
                                    case c_FormaPago.Item23:
                                        c.formapago = "23-Novación";
                                        break;
                                    case c_FormaPago.Item24:
                                        c.formapago = "24-Confusión";
                                        break;
                                    case c_FormaPago.Item25:
                                        c.formapago = "25-Remisión de Deuda";
                                        break;
                                    case c_FormaPago.Item26:
                                        c.formapago = "26-Prescripción o Caducidad";
                                        break;
                                    case c_FormaPago.Item27:
                                        c.formapago = "27-A Satisfacción del Acreedor";
                                        break;
                                    case c_FormaPago.Item28:
                                        c.formapago = "28-Tarjeta de Débito";
                                        break;
                                    case c_FormaPago.Item29:
                                        c.formapago = "29-Tarjeta de Servicios";
                                        break;
                                    case c_FormaPago.Item30:
                                        c.formapago = "30-Aplicación de Anticipos";
                                        break;
                                    case c_FormaPago.Item31:
                                        c.formapago = "31-Intermediario Pagos";
                                        break;
                                    case c_FormaPago.Item99:
                                        c.formapago = "99-Por Definir";
                                        break;
                                }
                            }
                            else
                            {
                                c.formapago = string.Empty;
                            }
                            c.metodopago = cfdi.MetodoPagoSpecified ? cfdi.MetodoPago.ToString() : string.Empty;
                            c.moneda = cfdi.Moneda.ToString();
                            c.rfcemisior = cfdi.Emisor.Rfc;
                            c.rfcreceptor = cfdi.Receptor.Rfc;
                            c.ruta = ruta;
                            c.subtotal = cfdi.SubTotal;
                            c.tipocomprobante = cfdi.TipoDeComprobante.ToString();
                            c.total = cfdi.Total;
                            if (cfdi.Impuestos != null)
                            {
                                if (cfdi.Impuestos.TotalImpuestosRetenidosSpecified)
                                { c.totalretenidos = cfdi.Impuestos.TotalImpuestosRetenidos; }
                                if (cfdi.Impuestos.TotalImpuestosTrasladadosSpecified)
                                { c.totaltraslados = cfdi.Impuestos.TotalImpuestosTrasladados; }
                            }
                            else
                            {
                                c.totalretenidos = 0;
                                c.totaltraslados = 0;
                            }

                            c.uuidtimbre = timbre;
                            c.version = cfdi.Version;
                            if (cfdi.TipoCambioSpecified)
                            { c.tipocambio = cfdi.TipoCambio; }
                            c.fecha = cfdi.Fecha;
                            c.lugarexpedicion = cfdi.LugarExpedicion;
                            c.nombrereceptor = cfdi.Receptor.Nombre;
                            c.nombreemisor = cfdi.Emisor.Nombre;
                            c.usocfdi = cfdi.Receptor.UsoCFDI.ToString();
                            c.regimenfiscal = cfdi.Emisor.RegimenFiscal.ToString().Replace("Item", "");

                            entry.ExtractToFile(ruta, true);
                            Data_Access.XMLCFDi petxmlcfdi = new Data_Access.XMLCFDi();
                            petxmlcfdi.sumit(ref c);
                            if (petxmlcfdi.bit_error) { throw petxmlcfdi.Error; }
                            num_cfdi++;

                            bconceptos = new Core.B_xmlcfdi_conceptos();
                            bconceptos.agregarConcepto(cfdi.Conceptos, c.id_xmlcfdi);
                            if (bconceptos.bit_error) { throw bconceptos.error; }

                            Entidades.Estatus_CFDi es_Cfdi = new Entidades.Estatus_CFDi();
                            es_Cfdi.bit_consultado = false;
                            es_Cfdi.fec_alta = DateTime.Now;
                            es_Cfdi.id = Guid.NewGuid();
                            es_Cfdi.impresion = getStringQR(cfdi);
                            es_Cfdi.uuidtimbre = timbre;

                            Data_Access.Estatus_CFDi pet_est = new Data_Access.Estatus_CFDi();
                            pet_est.sumit(es_Cfdi);
                            if (pet_est.bit_error) { throw pet_est.Error; }



                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return num_cfdi;
        }

        public string getStringQR(Comprobante cfdi)
        {
            XmlSerializer mySerializer;
            string uuid = string.Empty;
            string emisor = string.Empty;
            string receptor = string.Empty;
            string total = string.Empty;
            string sello = string.Empty;
            string resultado = string.Empty;
            try
            {

                TimbreFiscalDigital tfd = new TimbreFiscalDigital();
                cfdi.Complemento.ToList().ForEach(k =>
                {
                    StringReader stringReader = new StringReader(k.Any.ToList().Where(s => s.Name == "tfd:TimbreFiscalDigital").FirstOrDefault().OuterXml);
                    mySerializer = new XmlSerializer(typeof(TimbreFiscalDigital));
                    tfd = (TimbreFiscalDigital)mySerializer.Deserialize(stringReader);

                });

                uuid = tfd.UUID;
                emisor = cfdi.Emisor.Rfc;
                receptor = cfdi.Receptor.Rfc;
                total = cfdi.Total.ToString();
                sello = cfdi.Sello.Substring((cfdi.Sello.Length - 1) - 9, 8);

                resultado = string.Format("id={0}&re={1}&rr={2}&tt={3}&fe={4}", uuid, emisor, receptor, total.Replace(",", "."), sello);
            }
            catch (Exception)
            {
                resultado = string.Empty;
            }
            return resultado;
        }

        public void Estatus_SAT(Guid _id)
        {
            try
            {
                Entidades.Estatus_CFDi obj = new Entidades.Estatus_CFDi();
                Data_Access.Estatus_CFDi pet = new Data_Access.Estatus_CFDi();
                obj = pet.getSingle(p => p.id == _id);
                if (pet.bit_error) { throw pet.Error; }

                ConsultaCFDIServiceClient ws = new ConsultaCFDIServiceClient();
                Acuse respuesta = ws.Consulta(obj.impresion);
                if (respuesta == null) { throw new Exception("No se pudo conectar al servicio"); }
                obj.fec_consultado = DateTime.Now;
                obj.bit_consultado = true;
                pet.sumit(obj);
                if (pet.bit_error) { throw pet.Error; }

                Data_Access.XMLCFDi petcfdi = new Data_Access.XMLCFDi();
                //Entidades.XMLCFDi cfdi = obj.XMLCFDi;
                //cfdi.estatusCancelacion = respuesta.EstatusCancelacion;
                //if (respuesta.Estado.Equals("Vigente"))
                //{
                //    cfdi.bit_vigente = true;
                //}
                //else
                //{
                //    cfdi.bit_vigente = false;
                //}
                //petcfdi.sumit(cfdi);
                if (petcfdi.bit_error) { throw petcfdi.Error; }
            }
            catch (Exception ex)
            {//guardar en bitacora
            }
        }
    }
}
