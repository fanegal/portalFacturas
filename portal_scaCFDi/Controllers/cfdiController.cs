using Business.Core;
using Business.Funciones;
using CrystalDecisions.CrystalReports.Engine;
using portal_scaCFDi.Models.cfdiViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal_scaCFDi.Controllers
{
    [Authorize]
    public class cfdiController : Controller
    {
        B_logs bitacora;
        B_xmlcfdi doc_d;
        B_Clientes cls;
        B_Peticiones pets;

        public cfdiController()
        {
            bitacora = new B_logs();
        }

        public ActionResult List_peticion(string idpeticion)
        {
            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");

            List_peticionViewModels model = new List_peticionViewModels();
            model.detalles = new List<cfdiList>();

            try
            {

                Guid _id = Guid.Parse(idpeticion);

                pets = new B_Peticiones();
                var peticion = pets.getPeticion(_id);
                if (pets.bit_error) { throw pets.error; }

                model.fechapet = peticion.fec_create.ToString("yyyy-MM-dd");
                model.periodopet = "Del " + peticion.fec_inicio.ToString("yyyy-MM-dd") + " al " + peticion.fec_fin.ToString("yyyy-MM-dd");
                model.emitidos = peticion.bit_emitido;

                var cliente = peticion.cliente;
                model.rfc_cliente = cliente.RFC;
                model.razonsocial = cliente.razonsocial;
                model.idcliente = cliente.id_cliente.ToString();

                doc_d = new B_xmlcfdi();
                var d_result = doc_d.cfdixpeticion(_id);
                if (doc_d.bit_error) { throw doc_d.error; }

                d_result.ForEach(t =>
                {
                    model.detalles.Add(new cfdiList
                    {
                        id = t.id_xmlcfdi.ToString(),
                        rfc_emisor = t.rfcemisior,
                        razon_emisor = t.nombreemisor,
                        rfc_receptor = t.rfcreceptor,
                        razon_receptor = t.nombrereceptor,
                        tipocomprobante = t.tipocomprobante,
                        emision = t.fecha.ToString("yyyy-MM-dd HH:mm:ss"),
                        fecha_timbre = t.fechatimbre.ToString("yyyy-MM-ddTHH:mm:ss"),
                        uuid = t.uuidtimbre.ToString(),
                        subtotal = t.subtotal.ToString("c"),
                        descuento = t.descuento.ToString("c"),
                        totaltraslados = t.totaltraslados.ToString("c"),
                        totalretenidos = t.totalretenidos.ToString("c"),
                        total = t.total.ToString("c"),
                        estatus = t.estatusCancelacion
                    });
                });
            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
            }
            return View(model);
        }

        public ActionResult cfdi_ER(string idcliente)
        {
            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");

            B_sp_function sp;
            List<Entidades.sp_cfdiER_Result> emitidos;
            List<Entidades.sp_cfdiER_Result> recibidos;
            cfdi_ERViewModels model = new cfdi_ERViewModels();
            B_Clientes cl;
            try
            {
                cl = new B_Clientes();
                var cliente = cl.getCliente(Guid.Parse(idcliente));
                if (cl.bit_error) { throw cl.error; }

                colores paleta = new colores();
                var lpaleta = paleta.get();

                model.rfc_cliente = cliente.RFC;
                model.razonsocial = cliente.razonsocial;
                model.idcliente = cliente.id_cliente.ToString();

                sp = new B_sp_function();
                emitidos = sp.sp_cfdiER(Guid.Parse(idcliente), true);
                if (sp.bit_error) { throw sp.error; }
                recibidos = sp.sp_cfdiER(Guid.Parse(idcliente), false);
                if (sp.bit_error) { throw sp.error; }

                Donut dona_e = new Donut();
                Donut dona_r = new Donut();

                dona_e.element = "emitidos";
                dona_r.element = "recibidos";

                if (emitidos.Count > 0)
                {
                    dona_e.datas = new data[emitidos.Count];
                    dona_e.colors = new string[emitidos.Count];
                    int i = 0;
                    emitidos.ForEach(u =>
                    {
                        dona_e.datas[i] = new data { label = u.anio.ToString(), value = u.egreso.Value + u.ingreso.Value + u.nomina.Value + u.pagos.Value };
                        dona_e.colors[i] = lpaleta[i];
                        i++;
                    });
                }
                else
                {
                    dona_e.datas = new data[1];
                    dona_e.datas[0] = new data { label = "---", value = 0 };
                }
                if (recibidos.Count > 0)
                {
                    dona_r.datas = new data[recibidos.Count];
                    dona_r.colors = new string[recibidos.Count];
                    int i = 0;
                    recibidos.ForEach(u =>
                    {
                        dona_r.datas[i] = new data { label = u.anio.ToString(), value = u.egreso.Value + u.ingreso.Value + u.nomina.Value + u.pagos.Value };
                        dona_r.colors[i] = lpaleta[i];
                        i++;
                    });
                }
                else
                {
                    dona_r.datas = new data[1];
                    dona_r.datas[0] = new data { label = "---", value = 0 };
                }
                model.emitidos = dona_e;
                model.recibidos = dona_r;
            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
            }
            return View(model);
        }

        public ActionResult List_cfdi(string idpeticion)
        {
            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");

            List_peticionViewModels model = new List_peticionViewModels();
            model.detalles = new List<cfdiList>();

            try
            {

                Guid _id = Guid.Parse(idpeticion);

                pets = new B_Peticiones();
                var peticion = pets.getPeticion(_id);
                if (pets.bit_error) { throw pets.error; }

                model.fechapet = peticion.fec_create.ToString("yyyy-MM-dd");
                model.periodopet = "Del " + peticion.fec_inicio.ToString("yyyy-MM-dd") + " al " + peticion.fec_fin.ToString("yyyy-MM-dd");
                model.emitidos = peticion.bit_emitido;

                var cliente = peticion.cliente;
                model.rfc_cliente = cliente.RFC;
                model.razonsocial = cliente.razonsocial;
                model.idcliente = cliente.id_cliente.ToString();

                doc_d = new B_xmlcfdi();
                var d_result = doc_d.cfdixpeticion(_id);
                if (doc_d.bit_error) { throw doc_d.error; }

                d_result.ForEach(t =>
                {
                    model.detalles.Add(new cfdiList
                    {
                        id = t.id_xmlcfdi.ToString(),
                        rfc_emisor = t.rfcemisior,
                        razon_emisor = t.nombreemisor,
                        rfc_receptor = t.rfcreceptor,
                        razon_receptor = t.nombrereceptor,
                        tipocomprobante = t.tipocomprobante,
                        emision = t.fecha.ToString("yyyy-MM-dd HH:mm:ss"),
                        fecha_timbre = t.fechatimbre.ToString("yyyy-MM-ddTHH:mm:ss"),
                        uuid = t.uuidtimbre.ToString(),
                        subtotal = t.subtotal.ToString("c"),
                        descuento = t.descuento.ToString("c"),
                        totaltraslados = t.totaltraslados.ToString("c"),
                        totalretenidos = t.totalretenidos.ToString("c"),
                        total = t.total.ToString("c"),
                        estatus = t.estatusCancelacion
                    });
                });
            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
            }
            return View(model);
        }

        public ActionResult tipocomprobante(string idcliente, int emitido)
        {
            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");

            tipocomprobanteViewModels model = new tipocomprobanteViewModels();
            B_Clientes cl;
            B_sp_function sp;
            List<Entidades.sp_cfdiER_Result> comprobantes;
            try
            {
                cl = new B_Clientes();
                var cliente = cl.getCliente(Guid.Parse(idcliente));
                if (cl.bit_error) { throw cl.error; }

                model.rfc_cliente = cliente.RFC;
                model.razonsocial = cliente.razonsocial;
                model.idcliente = cliente.id_cliente.ToString();
                model.emitidos = (emitido == 1 ? true : false);

                sp = new B_sp_function();
                comprobantes = sp.sp_cfdiER(Guid.Parse(idcliente), model.emitidos);
                if (sp.bit_error) { throw sp.error; }

                model.egresos = comprobantes.Sum(u => u.egreso).Value;
                model.ingresos = comprobantes.Sum(u => u.ingreso).Value;
                model.pagos = comprobantes.Sum(u => u.pagos).Value;
                model.nominas = comprobantes.Sum(u => u.nomina).Value;
            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
            }

            return View(model);
        }

        public ActionResult Download_PDF(string idpeticion)
        {
            LlenadoCfdi obj;
            try
            {
                ricfdi.factura rd = new ricfdi.factura();
                rd.Load();
                //ReportDocument rd = new ReportDocument();

                //rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "cfdi_ingreso_33.rpt"));
                //rd.SetDataSource(context.emp_table.Select(c => new
                //{
                //    id = c.id,
                //    name = c.name
                //}).ToList());
                ricfdi.metadata base_ = new ricfdi.metadata();
                obj = new LlenadoCfdi();
                obj.setIngresos(ref base_, Guid.Parse(idpeticion));


                rd.SetDataSource(base_);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();


                rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
                rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "CustomerList.pdf");
            }
            catch (Exception ex)
            {
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
                return View();
            }



        }

    }
}