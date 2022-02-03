using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using Business.Core;

namespace portal_scaCFDi.Controllers
{
    [Authorize]
    public class ProgramasController : Controller
    {
        B_Clientes Neg;
        B_logs bitacora;

        public ProgramasController()
        {
            bitacora = new B_logs();
        }
        // GET: Programas
        public ActionResult Centro()
        {
            if (Session["userid"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }


        [HttpGet]
        public ActionResult Download(string fileName,string tipo)
        {
            string rootpath = Server.MapPath("~/programas/");

            string startPath = rootpath + fileName;
            string zipPath = startPath + ".zip";

            if (!System.IO.File.Exists(zipPath))
            {
                ZipFile.CreateFromDirectory(startPath, zipPath);
            }
            string fullName = zipPath;

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName + tipo);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
    }
}