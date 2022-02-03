using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using portal_scaCFDi.Models.clienteViewModels;
using Business.Core;

namespace portal_scaCFDi.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        B_Clientes Neg;
        B_logs bitacora;

        public ClienteController()
        {
            bitacora = new B_logs();
        }

        // GET: Cliente
        public ActionResult List()
        {
            Session["idCliente"] = null;
            ListViewModels model = new ListViewModels();
            model.detalles = new List<clientesList>();

            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");


            try
            {
                Neg = new B_Clientes();
                var d_result = Neg.Get_clientesxusuario(Session["userid"].ToString());
                if (Neg.bit_error) { throw Neg.error; }
                d_result.ForEach(t =>
                            {
                                model.detalles.Add(new clientesList
                                {
                                    fechaalta = t.fec_alta.Value.ToString("dd-MM-yyyy"),
                                    rfc = t.RFC,
                                    idcliente = t.id_cliente.ToString(),
                                    fechabaja = (t.baja == null ? "" : t.baja.Value.ToString("dd-MM-YYYY")),
                                    nombrecliente = t.razonsocial
                                }); ;
                            });

            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
 
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");

            CreateViewModels model = new CreateViewModels();
            model.error = false;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModels model)
        {
            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");

            try
            { 
                if (ModelState.IsValid)
                {
                    if (model.pfx.ContentLength > 0)
                    {
                        var _contenido = new byte[model.pfx.ContentLength];
                        model.pfx.InputStream.Read(_contenido, 0, model.pfx.ContentLength);

                        B_Clientes bcliente = new B_Clientes();
                        Entidades.cliente _cliente = new Entidades.cliente();
                        _cliente.RFC = model.RFC.ToUpper().Trim();
                        _cliente.razonsocial = model.razonsocial.ToUpper();
                        _cliente.pass = model.pass;
                        _cliente.pfx = _contenido;
                        _cliente.Id = Session["userid"].ToString();

                        if (bcliente.nuevo(_cliente))
                        {
                            return RedirectToAction("List");
                        }
                        else
                        {
                            model.error = true;
                            model.mensaje = bcliente.error.Message;
                            throw bcliente.error;
                        }
                        // Obtenemos el archivo del request..
                    //    var file = Request.Files[0];
                        // Guardamos el archivo localmente..
                     //   file.SaveAs("C:\\AquiLaRuta");
                        // Continua con tu lógica aquí..
                    }
                    throw new Exception("Error al guardar la información");
                } 
            }
            catch (Exception ex)
            {
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
                
            }
            return View(model);
        }
    }
}