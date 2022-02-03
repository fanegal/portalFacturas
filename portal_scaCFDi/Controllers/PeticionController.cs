using System;
using System.Collections.Generic;
using System.Web.Mvc;
using portal_scaCFDi.Models.PeticionViewModels;
using Business.Core;
using System.Linq;
using Entidades;

namespace portal_scaCFDi.Controllers
{
    [Authorize]
    public class PeticionController : Controller
    {
        B_Peticiones Neg;
        B_logs bitacora;


        public PeticionController()
        {
            bitacora = new B_logs();
        }
        // GET: Peticion
        [HttpGet]
        public ActionResult List(string idCliente)
        {
            ListViewModels model = new ListViewModels();
            model.Listado = new List<PeticionList>();
            B_Clientes bclient;
            cliente ocliente;
            try
            {
                Session["idCliente"] = idCliente;
                model.idCliente = idCliente;
                Guid _id = Guid.Parse(idCliente);
                Neg = new B_Peticiones();
                var d_result = Neg.GetPeticionsxCliente(_id);
                if (Neg.bit_error) { throw Neg.error; }
                d_result.Where(b => b.fec_create.Year == DateTime.Now.AddDays(-1).Year && b.fec_create.Month == DateTime.Now.AddDays(-1).Month && b.fec_create.Day == DateTime.Now.AddDays(-1).Day).ToList().ForEach(t =>
                {
                    model.Listado.Add(new PeticionList
                    {
                        fechaCreacion = t.fec_create.ToString("dd-MM-yyyy"),
                        Emitido = (t.bit_emitido ? "SI" : "NO"),
                        inicio = t.fec_inicio.ToString("dd-MM-yyyy"),
                        fin = t.fec_fin.ToString("dd-MM-yyyy"),
                        error = (t.bit_error ? "SI" : "NO"),
                        terminado = (t.bit_terminado ? "SI" : "NO"),
                        numxml = ((t.numxml == null) ? "0" : t.numxml.ToString()),
                        modalidad = (t.bit_automatico ? "AUTOMATICO" : "MANUAL"),
                        id = t.id_peticion.ToString()
                    }); ; ;
                });

                model.anio = DateTime.Now.AddDays(-1).Year;
                model.mes = DateTime.Now.AddDays(-1).Month;
                model.dia = DateTime.Now.AddDays(-1).Day;


                bclient = new B_Clientes();
                ocliente = bclient.getCliente(_id);
                if (bclient.bit_error) { throw bclient.error; }

                model.nombre_cliente = string.Format("{0} - {1}", ocliente.RFC, ocliente.razonsocial);
            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult List(ListViewModels model)
        {

            model.Listado = new List<PeticionList>();
            try
            {
                Session["idCliente"] = model.idCliente;
                Guid _id = Guid.Parse(model.idCliente);
                Neg = new B_Peticiones();
                var d_result = Neg.GetPeticionsxCliente(_id);
                if (Neg.bit_error) { throw Neg.error; }

                if (model.dia == 0)
                {
                    d_result.Where(b => b.fec_create.Year == model.anio && b.fec_create.Month == model.mes).ToList().ForEach(t =>
                  {
                      model.Listado.Add(new PeticionList
                      {
                          fechaCreacion = t.fec_create.ToString("dd-MM-yyyy"),
                          Emitido = (t.bit_emitido ? "SI" : "NO"),
                          inicio = t.fec_inicio.ToString("dd-MM-yyyy"),
                          fin = t.fec_fin.ToString("dd-MM-yyyy"),
                          error = (t.bit_error ? "SI" : "NO"),
                          terminado = (t.bit_terminado ? "SI" : "NO"),
                          numxml = ((t.numxml == null) ? "0" : t.numxml.ToString()),
                          modalidad = (t.bit_automatico ? "AUTOMATICO" : "MANUAL"),
                          id = t.id_peticion.ToString()
                      }); ; ;
                  });
                }
                else
                {

                    d_result.Where(b => b.fec_create.Year == model.anio && b.fec_create.Month == model.mes && b.fec_create.Day == model.dia).ToList().ForEach(t =>
                {
                    model.Listado.Add(new PeticionList
                    {
                        fechaCreacion = t.fec_create.ToString("dd-MM-yyyy"),
                        Emitido = (t.bit_emitido ? "SI" : "NO"),
                        inicio = t.fec_inicio.ToString("dd-MM-yyyy"),
                        fin = t.fec_fin.ToString("dd-MM-yyyy"),
                        error = (t.bit_error ? "SI" : "NO"),
                        terminado = (t.bit_terminado ? "SI" : "NO"),
                        numxml = ((t.numxml == null) ? "0" : t.numxml.ToString()),
                        modalidad = (t.bit_automatico ? "AUTOMATICO" : "MANUAL"),
                        id = t.id_peticion.ToString()
                    }); ; ;
                });
                }

            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
            }
            return View(model);
        }

        // GET: Peticion/Create
        public ActionResult Create()
        {
            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");

            CreateViewModels model = new CreateViewModels();
            try
            {
                model.inicio = DateTime.Now.Date.AddDays(-1);
                model.fin = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                model.idcliente = Session["idCliente"].ToString();
            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);

            }


            return View(model);
        }

        // POST: Peticion/Create
        [HttpPost]
        public ActionResult Create(CreateViewModels model)
        {

            bool valido = false;
            B_Peticiones objneg;
            try
            {
                if (ModelState.IsValid)
                {
                    objneg = new B_Peticiones();
                    objneg.create(Guid.Parse(model.idcliente), model.inicio, model.fin, model.emitido);
                    if (objneg.bit_error)
                        throw objneg.error;

                    valido = true;
                }
            }
            catch (Exception ex)
            {
                // aqui va la bitacora de los errores
                bitacora.new_log_error(Session["userid"].ToString(), ex.ToString(), HttpContext.Request.ServerVariables["REMOTE_ADDR"]);
                valido = false;
            }

            if (valido)
            {
                return RedirectToAction("List", new { idCliente = Session["idCliente"] });
            }
            else
            {
                return View(model);
            }

        }



        // GET: Peticion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Peticion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
