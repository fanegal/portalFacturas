using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ToPfx
{
    public class PFX
    {
        string cer = "";
        string key = "";
        string clavePrivada = "";
        string ArchivoPFX = "";
        string ArchivoKPEM = "";
        string ArchivoCPEM = "";
        string openssl = "";

        public string error = "";
        public string mensajeExito = "";
        public string detalle = "";
        ///
        //<summary>
        ///
        // </summary>

        /// <param name="cer_">archivo cer a utilizar</param>
        /// <param name="key_">archivo key a utilizar</param>
        /// <param name="clavePrivada_">clave para el archivo pfx</param>
        /// <param name="archivoPfx_">ruta del archivo pfx</param>
        /// <param name="path">ruta para archivos temporales PEM</param>
        public PFX(string cer_, string key_, string clavePrivada_, string archivoPfx_)
        {
            cer = cer_;
            key = key_;
            clavePrivada = clavePrivada_;

            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);

            openssl = directory + "\\openssl\\";

            ArchivoKPEM = directory + "\\k.pem";
            ArchivoCPEM = directory + "\\c.pem";
            ArchivoPFX = archivoPfx_;
        }

        public bool creaPFX()
        {
            bool exito = false;

            //validaciones
            if (!File.Exists(cer))
            {
                error = "No existe el archivo cer en el sistema";
                return false;
            }
            if (!File.Exists(key))
            {
                error = "No existe el archivo key en el sistema";
                return false;
            }
            if (clavePrivada.Trim().Equals(""))
            {
                error = "No existe una clave privada aun en el sistema";
                return false;
            }

            //creamos objetos Process
            Process proc = new Process();
            Process proc2 = new Process();
            Process proc3 = new Process();
            proc.EnableRaisingEvents = false;
            proc2.EnableRaisingEvents = false;
            proc3.EnableRaisingEvents = false;

            detalle = detalle + "\n" + openssl; 

            //openssl x509 -inform DER -in certificado.cer -out certificado.pem
            //proc.StartInfo.FileName = "openssl";
            //proc.StartInfo.Arguments = "x509 -inform DER -in \"" + cer + "\" -out \"" + ArchivoCPEM + "\"";
            //proc.StartInfo.WorkingDirectory = openssl;// @"C:\openssl-win32\bin\";
            string command1 = openssl + "openssl " + "x509 -inform DER -in \"" + cer + "\" -out \"" + ArchivoCPEM + "\"";
            detalle = detalle + "\n" + command1;
            //openssl pkcs8 -inform DER -in llave.key -passin pass:a0123456789 -out llave.pem
            //proc2.StartInfo.FileName = "openssl.exe";
            //proc2.StartInfo.Arguments = "pkcs8 -inform DER -in \"" + key + "\" -passin pass:" + clavePrivada + " -out \"" + ArchivoKPEM + "\"";
            //proc2.StartInfo.WorkingDirectory = openssl;// @"C:\openssl-win32\bin\";
            string command2 = openssl + "openssl " + "pkcs8 -inform DER -in \"" + key + "\" -passin pass:" + clavePrivada + " -out \"" + ArchivoKPEM + "\"";
            detalle = detalle + "\n" + command2;
          //openssl pkcs12 -export -out archivopfx.pfx -inkey llave.pem -in certificado.pem -passout pass:clavedesalida
          //proc3.StartInfo.FileName = "openssl.exe";
          //proc3.StartInfo.Arguments = "pkcs12 -export -out \"" + ArchivoPFX + "\" -inkey \"" + ArchivoKPEM + "\" -in \"" + ArchivoCPEM + "\" -passout pass:" + clavePrivada;
          //proc3.StartInfo.WorkingDirectory = openssl;// @"C:\openssl-win32\bin\";
            string command3 = openssl + "openssl " + "pkcs12 -export -out \"" + ArchivoPFX + "\" -inkey \"" + ArchivoKPEM + "\" -in \"" + ArchivoCPEM + "\" -passout pass:" + clavePrivada;
            detalle = detalle + "\n" + command3;
            try
            {
                proc.StartInfo = new ProcessStartInfo("cmd", "/c " + command1);
                proc.Start(); 
                proc.WaitForExit();

                proc2.StartInfo = new ProcessStartInfo("cmd", "/c " + command2);
                proc2.Start();
                proc2.WaitForExit();

                proc3.StartInfo = new ProcessStartInfo("cmd", "/c " + command3);
                proc3.Start();
                proc3.WaitForExit();


                //enviamos mensaje exitoso
                if (System.IO.File.Exists(ArchivoPFX))
                    mensajeExito = "Se ha creado el archivo PFX ";
                else
                {
                    error = "Error al crear el archivo PFX, puede ser que el cer o el key no sean archivos con formato correcto";
                    return false;
                }

                //eliminamos los archivos pem
                if (System.IO.File.Exists(ArchivoCPEM)) System.IO.File.Delete(ArchivoCPEM);
                if (System.IO.File.Exists(ArchivoKPEM)) System.IO.File.Delete(ArchivoKPEM);

                exito = true;
            }
            catch (Exception ex)
            {
                error = "Error al procesar la información";
                return false;
            }
            finally
            {
                proc.Dispose();
                proc2.Dispose();
                proc3.Dispose();
            }



            return exito;
        }
    }
}
