using Business.Core;
using Data_Access;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Business.HerramientasCFDi
{
    public class DescargaCFDI
    {
        private string urlAutentica;//= "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/Autenticacion/Autenticacion.svc";
        private string urlAutenticaAction;//= "http://DescargaMasivaTerceros.gob.mx/IAutenticacion/Autentica";

        private string urlSolicitud;// = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/SolicitaDescargaService.svc";
        private string urlSolicitudAction;// = "http://DescargaMasivaTerceros.sat.gob.mx/ISolicitaDescargaService/SolicitaDescarga";

        private string urlVerificarSolicitud;//= "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/VerificaSolicitudDescargaService.svc";
        private string urlVerificarSolicitudAction;// = "http://DescargaMasivaTerceros.sat.gob.mx/IVerificaSolicitudDescargaService/VerificaSolicitudDescarga";

        private string urlDescargarSolicitud;//= "https://cfdidescargamasiva.clouda.sat.gob.mx/DescargaMasivaService.svc";
        private string urlDescargarSolicitudAction;//= "http://DescargaMasivaTerceros.sat.gob.mx/IDescargaMasivaTercerosService/Descargar";

        SoapRequestBase soapClass;
        peticion pet;
        cliente cli;
        B_logs bitacora;
        B_parametros param;
        public DescargaCFDI()
        {
            soapClass = new SoapRequestBase();
            bitacora = new B_logs();
            param = new B_parametros();

            urlAutentica = param.GetParametro("urlAutentica").valor;
            if (param.bit_error) { throw param.error; }

            urlAutenticaAction = param.GetParametro("urlAutenticaAction").valor;
            if (param.bit_error) { throw param.error; }

            urlSolicitud = param.GetParametro("urlSolicitud").valor;
            if (param.bit_error) { throw param.error; }

            urlSolicitudAction = param.GetParametro("urlSolicitudAction").valor;
            if (param.bit_error) { throw param.error; }

            urlVerificarSolicitud = param.GetParametro("urlVerificarSolicitud").valor;
            if (param.bit_error) { throw param.error; }

            urlVerificarSolicitudAction = param.GetParametro("urlVerificarSolicitudAction").valor;
            if (param.bit_error) { throw param.error; }

            urlDescargarSolicitud = param.GetParametro("urlDescargarSolicitud").valor;
            if (param.bit_error) { throw param.error; }

            urlDescargarSolicitudAction = param.GetParametro("urlDescargarSolicitudAction").valor;
            if (param.bit_error) { throw param.error; }
        }


        public bool Process_sintoken(Entidades.peticion objpet)
        {
            try
            {
                pet = new peticion();
                cli = new cliente();
                //Obtener Token

                Entidades.cliente objcli = cli.getSingle(h => h.id_cliente == objpet.id_cliente);
                if (cli.bit_error) { throw cli.Error; }

                //Obtener Certificados
                X509Certificate2 certifcate = ObtenerX509Certificado(objcli.pfx, objcli.pass);

                if (string.IsNullOrEmpty(objpet.token))
                {
                    soapClass = new SoapRequestBase(urlAutentica, urlAutenticaAction);
                    soapClass.xml = Autentifica_Generate(soapClass, certifcate);
                    string token = Autentifica_GetResult(soapClass.Send());
                    objpet.token = token;
                    pet.Submit(ref objpet);
                    if (pet.bit_error) { throw pet.Error; }
                }

                return !string.IsNullOrEmpty(objpet.token);

            }
            catch (Exception ex)
            {
                objpet.bit_error = true;
                pet.Submit(ref objpet);

                // aqui va la bitacora de los errores
                bitacora.new_log_error("Process_sintoken", ex.ToString(), "Localhost");
                return false;
            }
        }

        public bool Process_sinsolicitud(Entidades.peticion objpet)
        {
            pet = new peticion();
            cli = new cliente();

            try
            {
                Entidades.cliente objcli = cli.getSingle(h => h.id_cliente == objpet.id_cliente);
                if (cli.bit_error) { throw cli.Error; }
                //Obtener Certificados
                X509Certificate2 certifcate = ObtenerX509Certificado(objcli.pfx, objcli.pass);
                string autorization = String.Format("WRAP access_token=\"{0}\"", HttpUtility.UrlDecode(objpet.token));

                //Generar Solicitud
                if (string.IsNullOrEmpty(objpet.idsolicitud))
                {
                    soapClass = new SoapRequestBase(urlSolicitud, urlSolicitudAction);
                    soapClass.xml = solicita_Generate(soapClass, certifcate, (objpet.bit_emitido ? objcli.RFC : ""), (objpet.bit_emitido ? "" : objcli.RFC), objcli.RFC, objpet.fec_inicio.ToString("yyyy-MM-dd"), objpet.fec_fin.ToString("yyyy-MM-dd"));
                    string idSolicitud = solicita_GetResult(soapClass.Send(autorization));
                    objpet.idsolicitud = idSolicitud;
                    pet.Submit(ref objpet);
                    if (pet.bit_error) { throw pet.Error; }
                }

                return !string.IsNullOrEmpty(objpet.idsolicitud);
            }
            catch (Exception ex)
            {
                objpet.bit_error = true;
                pet.Submit(ref objpet);
                // aqui va la bitacora de los errores
                bitacora.new_log_error("Process_sinsolicitud", ex.ToString(), "Localhost");
                return false;
            }

        }

        public bool Process_validasolicitud(Entidades.peticion objpet)
        {
            try
            {
                pet = new peticion();
                cli = new cliente();

                Entidades.cliente objcli = cli.getSingle(h => h.id_cliente == objpet.id_cliente);
                if (cli.bit_error) { throw cli.Error; }

                //Obtener Certificados
                X509Certificate2 certifcate = ObtenerX509Certificado(objcli.pfx, objcli.pass);
                string autorization = String.Format("WRAP access_token=\"{0}\"", HttpUtility.UrlDecode(objpet.token));

                if (string.IsNullOrEmpty(objpet.idpaquete))
                {
                    soapClass = new SoapRequestBase(urlVerificarSolicitud, urlVerificarSolicitudAction);
                    soapClass.xml = verifica_Generate(soapClass, certifcate, objcli.RFC, objpet.idsolicitud);
                    string idPaquete = verifica_GetResult(soapClass.Send(autorization), objpet.id_peticion);
                    if (!string.IsNullOrEmpty(idPaquete))
                    {
                        objpet.idpaquete = idPaquete;
                        if (idPaquete == "0")
                        {
                            objpet.bit_terminado = true;
                            objpet.numxml = 0;
                        }
                        else if (idPaquete == "-1")
                        {
                            objpet.bit_terminado = true;
                            objpet.bit_error = true;
                        }
                        pet.Submit(ref objpet);
                        if (pet.bit_error) { throw pet.Error; }
                    }
                    else
                    {
                        if (objpet.intentos < 4)
                        {
                            objpet.intentos++;

                            pet.Submit(ref objpet);
                            if (pet.bit_error) { throw pet.Error; }
                        }
                        else
                        {
                            objpet.token = string.Empty;
                            objpet.idsolicitud = string.Empty;
                            objpet.idpaquete = string.Empty;
                            objpet.intentos = 0;

                            pet.Submit(ref objpet);
                            if (pet.bit_error) { throw pet.Error; }

                            B_parametros bparam = new B_parametros();
                            bparam.SetParametro("PAUSA_D_CFDI", "45");
                            if (bparam.bit_error) { throw bparam.error; }
                        }
                    }
                }
                return !string.IsNullOrEmpty(objpet.idpaquete);

            }
            catch (Exception ex)
            {
                objpet.bit_error = true;
                pet.Submit(ref objpet);
                // aqui va la bitacora de los errores
                bitacora.new_log_error("Process_validasolicitud", ex.ToString(), "Localhost");
                return false;
            }


        }

        public bool Process_decargareponse(Entidades.peticion objpet)
        {
            try
            {
                pet = new peticion();
                cli = new cliente();

                Entidades.cliente objcli = cli.getSingle(h => h.id_cliente == objpet.id_cliente);
                if (cli.bit_error) { throw cli.Error; }

                //Obtener Certificados
                X509Certificate2 certifcate = ObtenerX509Certificado(objcli.pfx, objcli.pass);
                string autorization = String.Format("WRAP access_token=\"{0}\"", HttpUtility.UrlDecode(objpet.token));

                if (!string.IsNullOrEmpty(objpet.idpaquete))
                {
                    //Descargar Solicitud
                    if (string.IsNullOrEmpty(objpet.descargaresponse))
                    {
                        soapClass = new SoapRequestBase(urlDescargarSolicitud, urlDescargarSolicitudAction);
                        soapClass.xml = descarga_Generate(soapClass, certifcate, objcli.RFC, objpet.idpaquete);
                        string descargaResponse = descarga_GetResult(soapClass.Send(autorization));

                        objpet.descargaresponse = descargaResponse;
                        pet.Submit(ref objpet);
                        if (pet.bit_error) { throw pet.Error; }
                    }
                }

                return !string.IsNullOrEmpty(objpet.descargaresponse);

            }
            catch (Exception ex)
            {
                objpet.bit_error = true;
                pet.Submit(ref objpet);
                // aqui va la bitacora de los errores
                bitacora.new_log_error("Process_decargareponse", ex.ToString(), "Localhost");
                return false;
            }
        }

        public bool Process_descomprimirdescarga(Entidades.peticion objpet)
        {
            bool respueta = false;
            try
            {
                pet = new peticion();

                if (!string.IsNullOrEmpty(objpet.descargaresponse))
                {
                    string path = "./Paquetes/";
                    byte[] file = Convert.FromBase64String(objpet.descargaresponse);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string pathFinal = path + objpet.idpaquete + ".zip";
                    using (FileStream fs = File.Create(pathFinal, file.Length))
                    {
                        fs.Write(file, 0, file.Length);
                    }

                    tool_Cfdi tcfdi = new tool_Cfdi();
                    objpet.numxml = tcfdi.guardar_CFdis(pathFinal, objpet.id_peticion);

                    objpet.bit_terminado = true;
                    pet.Submit(ref objpet);
                    if (pet.bit_error) { throw pet.Error; }
                    respueta = true;
                }
                respueta = false;
            }
            catch (Exception ex)
            { // aqui va la bitacora de los errores 
                bitacora.new_log_error("Process_descomprimirdescarga", ex.ToString(), "Localhost");
                objpet.bit_error = true;
                pet.Submit(ref objpet);
                if (pet.bit_error) { throw pet.Error; }
                respueta = false;

            }
            return respueta;
        }

        public void process_start(Guid idpeticion)
        {
            pet = new peticion();
            cli = new cliente();


            try
            {

                Entidades.peticion objpet = pet.getSingle(h => h.id_peticion == idpeticion);
                if (pet.bit_error) { throw pet.Error; }

                Entidades.cliente objcli = cli.getSingle(h => h.id_cliente == objpet.id_cliente);
                if (cli.bit_error) { throw cli.Error; }

                //Obtener Certificados
                X509Certificate2 certifcate = ObtenerX509Certificado(objcli.pfx, objcli.pass);

                //Obtener Token
                if (string.IsNullOrEmpty(objpet.token))
                {
                    soapClass = new SoapRequestBase(urlAutentica, urlAutenticaAction);
                    soapClass.xml = Autentifica_Generate(soapClass, certifcate);
                    string token = Autentifica_GetResult(soapClass.Send());
                    objpet.token = token;
                    pet.Submit(ref objpet);
                    if (pet.bit_error) { throw pet.Error; }
                }
                string autorization = String.Format("WRAP access_token=\"{0}\"", HttpUtility.UrlDecode(objpet.token));


                //Generar Solicitud
                if (string.IsNullOrEmpty(objpet.idsolicitud))
                {
                    soapClass = new SoapRequestBase(urlSolicitud, urlSolicitudAction);
                    soapClass.xml = solicita_Generate(soapClass, certifcate, (objpet.bit_emitido ? objcli.RFC : ""), (objpet.bit_emitido ? "" : objcli.RFC), objcli.RFC, objpet.fec_inicio.ToString("yyyy-MM-dd"), objpet.fec_fin.ToString("yyyy-MM-dd"));
                    string idSolicitud = solicita_GetResult(soapClass.Send(autorization));
                    objpet.idsolicitud = idSolicitud;
                    pet.Submit(ref objpet);
                    if (pet.bit_error) { throw pet.Error; }
                }

                //Validar Solicitud
                if (string.IsNullOrEmpty(objpet.idpaquete))
                {
                    soapClass = new SoapRequestBase(urlVerificarSolicitud, urlVerificarSolicitudAction);
                    soapClass.xml = verifica_Generate(soapClass, certifcate, objcli.RFC, objpet.idsolicitud);
                    string idPaquete = verifica_GetResult(soapClass.Send(autorization), objpet.id_peticion);
                    if (!string.IsNullOrEmpty(idPaquete))
                    {
                        objpet.idpaquete = idPaquete;
                        pet.Submit(ref objpet);
                        if (pet.bit_error) { throw pet.Error; }
                    }
                }

                if (!string.IsNullOrEmpty(objpet.idpaquete))
                {
                    //Descargar Solicitud
                    if (string.IsNullOrEmpty(objpet.descargaresponse))
                    {
                        soapClass = new SoapRequestBase(urlDescargarSolicitud, urlDescargarSolicitudAction);
                        soapClass.xml = descarga_Generate(soapClass, certifcate, objcli.RFC, objpet.idpaquete);
                        string descargaResponse = descarga_GetResult(soapClass.Send(autorization));
                        objpet.descargaresponse = descargaResponse;
                        pet.Submit(ref objpet);
                        if (pet.bit_error) { throw pet.Error; }
                    }
                    string path = "./Paquetes/";
                    byte[] file = Convert.FromBase64String(objpet.descargaresponse);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string pathFinal = path + objpet.idpaquete + ".zip";
                    using (FileStream fs = File.Create(pathFinal, file.Length))
                    {
                        fs.Write(file, 0, file.Length);
                    }

                    tool_Cfdi tcfdi = new tool_Cfdi();
                    objpet.numxml = tcfdi.guardar_CFdis(pathFinal, objpet.id_peticion);

                    objpet.bit_terminado = true;
                    pet.Submit(ref objpet);

                }
            }
            catch (WebException e)
            {
                //aqui va un codigo se debe registrar
            }
            catch (Exception ex)
            {
                //objpet.bit_error = true;
                //pet.Submit(objpet);
            }


        }

        private static X509Certificate2 ObtenerX509Certificado(byte[] pfx, string password)
        {
            return new X509Certificate2(pfx, password, X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable);
        }
        private static string Autentifica_Generate(SoapRequestBase soapClass, X509Certificate2 certificate)
        {
            string format = "yyyy-MM-ddTHH:mm:ss.fffZ";
            DateTime date = DateTime.UtcNow;
            string created = date.ToString(format);
            string expires = date.AddSeconds(300).ToString(format);
            string uuid = "uuid-" + Guid.NewGuid().ToString() + "-1";

            string canonicalTimestamp = @"<u:Timestamp xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" u:Id=""_0"">" +
                                            "<u:Created>" + created + "</u:Created>" +
                                            "<u:Expires>" + expires + "</u:Expires>" +
                                         "</u:Timestamp>";
            string digest = soapClass.CreateDigest(canonicalTimestamp);
            string canonicalSignedInfo = @"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                            @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>" +
                                            @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>" +
                                            @"<Reference URI=""#_0"">" +
                                               "<Transforms>" +
                                                  @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>" +
                                               "</Transforms>" +
                                               @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>" +
                                               "<DigestValue>" + digest + "</DigestValue>" +
                                            "</Reference>" +
                                         "</SignedInfo>";
            string signature = soapClass.Sign(canonicalSignedInfo, certificate);
            string soap_request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">" +
                                    @"<s:Header>" +
                                     @"<o:Security s:mustUnderstand=""1"" xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">" +
                                      @"<u:Timestamp u:Id=""_0"">" +
                                       @"<u:Created>" + created + "</u:Created>" +
                                       @"<u:Expires>" + expires + "</u:Expires>" +
                                      @"</u:Timestamp>" +
                                      @"<o:BinarySecurityToken u:Id=""" + uuid + @""" ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">" +
                                       Convert.ToBase64String(certificate.RawData) +
                                      @"</o:BinarySecurityToken>" +
                                      @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                       @"<SignedInfo>" +
                                        @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                        @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
                                        @"<Reference URI=""#_0"">" +
                                         @"<Transforms>" +
                                          @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                         @"</Transforms>" +
                                         @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
                                         @"<DigestValue>" + digest + @"</DigestValue>" +
                                        @"</Reference>" +
                                       @"</SignedInfo>" +
                                       @"<SignatureValue>" + signature + "</SignatureValue>" +
                                       @"<KeyInfo>" +
                                        @"<o:SecurityTokenReference>" +
                                         @"<o:Reference ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" URI=""#" + uuid + @"""/>" +
                                        @"</o:SecurityTokenReference>" +
                                       @"</KeyInfo>" +
                                      @"</Signature>" +
                                     @"</o:Security>" +
                                    @"</s:Header>" +
                                    @"<s:Body>" +
                                     @"<Autentica xmlns=""http://DescargaMasivaTerceros.gob.mx""/>" +
                                    @"</s:Body>" +
                                   @"</s:Envelope>";

            return soap_request;
        }
        private static string Autentifica_GetResult(XmlDocument xmlDoc)
        {
            string s = xmlDoc.GetElementsByTagName("AutenticaResult")[0].InnerXml;
            return s;
        }
        private static string solicita_Generate(SoapRequestBase soapClass, X509Certificate2 certificate, string rfcEmisor, string rfcReceptor, string rfcSolicitante, string fechaInicial = "", string fechaFinal = "", string tipoSolicitud = "CFDI")
        {
            solicita_FixFecha(fechaInicial, fechaFinal, out fechaInicial, out fechaFinal);
            string canonicalTimestamp = "<des:SolicitaDescarga xmlns:des=\"http://DescargaMasivaTerceros.sat.gob.mx\">"
                + "<des:solicitud RfcEmisor=\"" + rfcEmisor + "\" RfcReceptor=\"" + rfcReceptor + "\" RfcSolicitante=\"" + rfcSolicitante + "\" FechaInicial=\"" + fechaInicial + "\" FechaFinal=\"" + fechaFinal + "\" TipoSolicitud=\"CFDI\">"
                + "</des:solicitud>"
                + "</des:SolicitaDescarga>";

            string digest = soapClass.CreateDigest(canonicalTimestamp);

            string canonicalSignedInfo = @"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                            @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>" +
                                            @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>" +
                                            @"<Reference URI=""#_0"">" +
                                               "<Transforms>" +
                                                  @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>" +
                                               "</Transforms>" +
                                               @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>" +
                                               "<DigestValue>" + digest + "</DigestValue>" +
                                            "</Reference>" +
                                         "</SignedInfo>";
            string signature = soapClass.Sign(canonicalSignedInfo, certificate);
            string soap_request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">" +
                        @"<s:Header/>" +
                        @"<s:Body>" +
                            @"<des:SolicitaDescarga>" +
                                @"<des:solicitud RfcEmisor=""" + rfcEmisor +
                                @""" RfcReceptor =""" + rfcReceptor +
                                @""" RfcSolicitante=""" + rfcSolicitante +
                                @""" FechaInicial=""" + fechaInicial +
                                @""" FechaFinal =""" + fechaFinal +
                                @""" TipoSolicitud=""" + tipoSolicitud +
                                @""">" +
                                                    @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                                    @"<SignedInfo>" +
                                                    @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                                    @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
                                                    @"<Reference URI=""#_0"">" +
                                                        @"<Transforms>" +
                                                        @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                                        @"</Transforms>" +
                                                        @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
                                                        @"<DigestValue>" + digest + @"</DigestValue>" +
                                                    @"</Reference>" +
                                                    @"</SignedInfo>" +
                                                    @"<SignatureValue>" + signature + "</SignatureValue>" +
                                                    @"<KeyInfo>" +
                                                        @"<X509Data>" +
                                                            @"<X509IssuerSerial>" +
                                                                @"<X509IssuerName>" + certificate.Issuer +
                                                                @"</X509IssuerName>" +
                                                                @"<X509SerialNumber>" + certificate.SerialNumber +
                                                                @"</X509SerialNumber>" +
                                                            @"</X509IssuerSerial>" +
                                                            @"<X509Certificate>" + Convert.ToBase64String(certificate.RawData) + "</X509Certificate>" +
                                                        @"</X509Data>" +
                                                    @"</KeyInfo>" +
                                                    @"</Signature>" +
                                                    @"</des:solicitud>" +
                                                @"</des:SolicitaDescarga>" +
                                            @"</s:Body>" +
                                            @"</s:Envelope>";

            return soap_request;
        }
        private static void solicita_FixFecha(string fechaInicial1, string fechaFinal1, out string fechaInicial, out string fechaFinal)
        {
            fechaInicial = fechaInicial1 + "T00:00:00";
            fechaFinal = fechaFinal1 + "T23:59:59";
        }
        private static string solicita_GetResult(XmlDocument xmlDoc)
        {
            string s = xmlDoc.GetElementsByTagName("SolicitaDescargaResult")[0].Attributes["IdSolicitud"].Value;
            return s;



        }
        private static string verifica_Generate(SoapRequestBase soapClass, X509Certificate2 certificate, string rfcSolicitante, string idSolicitud)
        {
            string canonicalTimestamp = "<des:VerificaSolicitudDescarga xmlns:des=\"http://DescargaMasivaTerceros.sat.gob.mx\">"
                + "<des:solicitud IdSolicitud=\"" + idSolicitud + "\" RfcSolicitante=\"" + rfcSolicitante + ">"
                + "</des:solicitud>"
                + "</des:VerificaSolicitudDescarga>";

            string digest = soapClass.CreateDigest(canonicalTimestamp);

            string canonicalSignedInfo = @"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                            @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>" +
                                            @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>" +
                                            @"<Reference URI=""#_0"">" +
                                               "<Transforms>" +
                                                  @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>" +
                                               "</Transforms>" +
                                               @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>" +
                                               "<DigestValue>" + digest + "</DigestValue>" +
                                            "</Reference>" +
                                         "</SignedInfo>";
            string signature = soapClass.Sign(canonicalSignedInfo, certificate);
            string soap_request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">" +
                        @"<s:Header/>" +
                        @"<s:Body>" +
                            @"<des:VerificaSolicitudDescarga>" +
                                @"<des:solicitud " +
                                @"IdSolicitud=""" + idSolicitud +
                                @""" RfcSolicitante=""" + rfcSolicitante +
                                @""">" +
                                                    @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                                    @"<SignedInfo>" +
                                                    @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                                    @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
                                                    @"<Reference URI=""#_0"">" +
                                                        @"<Transforms>" +
                                                        @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                                        @"</Transforms>" +
                                                        @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
                                                        @"<DigestValue>" + digest + @"</DigestValue>" +
                                                    @"</Reference>" +
                                                    @"</SignedInfo>" +
                                                    @"<SignatureValue>" + signature + "</SignatureValue>" +
                                                    @"<KeyInfo>" +
                                                        @"<X509Data>" +
                                                            @"<X509IssuerSerial>" +
                                                                @"<X509IssuerName>" + certificate.Issuer +
                                                                @"</X509IssuerName>" +
                                                                @"<X509SerialNumber>" + certificate.SerialNumber +
                                                                @"</X509SerialNumber>" +
                                                            @"</X509IssuerSerial>" +
                                                            @"<X509Certificate>" + Convert.ToBase64String(certificate.RawData) + "</X509Certificate>" +
                                                        @"</X509Data>" +
                                                    @"</KeyInfo>" +
                                                    @"</Signature>" +
                                                    @"</des:solicitud>" +
                                                @"</des:VerificaSolicitudDescarga>" +
                                            @"</s:Body>" +
                                            @"</s:Envelope>";
            return soap_request;
        }
        private static string verifica_GetResult(XmlDocument xmlDoc, Guid id_peticion)
        {
            string s = string.Empty;
            B_logs _bitacora = new B_logs();
            _bitacora.new_log("verifica_GetResult->xml idpeticion:" + id_peticion.ToString(), xmlDoc.OuterXml.ToString(), "Localhost");

            int estado = Convert.ToInt16(xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes["EstadoSolicitud"].Value);

            switch (estado)
            {
                case 0:
                case 1:
                case 2:
                    s = "";
                    break;
                case 3:
                    int tagcfdi = Convert.ToInt32(xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes["NumeroCFDIs"].Value.ToString());

                    s = tagcfdi.ToString();
                    if (tagcfdi != 0)
                    {
                        XmlNodeList tag = xmlDoc.GetElementsByTagName("IdsPaquetes");
                        if (tag.Count > 0)
                        { s = tag[0].InnerXml; }
                    }
                    break;
                case 4:
                case 5:
                case 6:
                    {
                        int codestado = Convert.ToInt16(xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes["CodigoEstadoSolicitud"].Value);
                        switch (codestado)
                        {
                            case 5002:
                                s = "-1";
                                _bitacora.new_log_error("verifica_GetResult->xml idpeticion:" + id_peticion.ToString(), "Se agotó las solicitudes de por vida", "Localhost");
                                break;
                            case 5004:
                                s = "-1";
                                _bitacora.new_log("verifica_GetResult->xml idpeticion:" + id_peticion.ToString(), "No se encontró la información", "Localhost");
                                break;
                            case 5005:
                                s = "-1";
                                _bitacora.new_log_error("verifica_GetResult->xml idpeticion:" + id_peticion.ToString(), "Solicitud duplicada", "Localhost");
                                break;
                        }
                    }
                    break;
            }


            //if (estado != 1 && estado == 3)
            //{

            //    int tagcfdi = Convert.ToInt32(xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes["NumeroCFDIs"].Value.ToString());

            //    s = tagcfdi.ToString();
            //    if (tagcfdi != 0)
            //    {
            //        XmlNodeList tag = xmlDoc.GetElementsByTagName("IdsPaquetes");
            //        if (tag.Count > 0)
            //        { s = tag[0].InnerXml; }
            //    }

            //}
            //else if (estado == 0)
            //{
            //    s = "-1";
            //}
            //else
            //{
            //    int codestado = Convert.ToInt16(xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes["CodigoEstadoSolicitud"].Value);
            //    switch (codestado)
            //    {
            //        case 5002:
            //            s = "-1";
            //            _bitacora.new_log_error("verifica_GetResult->xml idpeticion:" + id_peticion.ToString(), "Se agotó las solicitudes de por vida", "Localhost");
            //            break;
            //        case 5004:
            //            s = "0";
            //            _bitacora.new_log("verifica_GetResult->xml idpeticion:" + id_peticion.ToString(), "No se encontró la información", "Localhost");
            //            break;
            //        case 5005:
            //            s = "-1";
            //            _bitacora.new_log_error("verifica_GetResult->xml idpeticion:" + id_peticion.ToString(), "Solicitud duplicada", "Localhost");
            //            break;
            //    }

            //}
            return s;
        }
        private static string descarga_Generate(SoapRequestBase soapClass, X509Certificate2 certificate, string rfcSolicitante, string idPaquete)
        {
            string canonicalTimestamp = "<des:PeticionDescargaMasivaTercerosEntrada xmlns:des=\"http://DescargaMasivaTerceros.sat.gob.mx\">"
                + "<des:peticionDescarga IdPaquete=\"" + idPaquete + "\" RfcSolicitante=\"" + rfcSolicitante + ">"
                + "</des:peticionDescarga>"
                + "</des:PeticionDescargaMasivaTercerosEntrada>";

            string digest = soapClass.CreateDigest(canonicalTimestamp);

            string canonicalSignedInfo = @"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                            @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>" +
                                            @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>" +
                                            @"<Reference URI=""#_0"">" +
                                               "<Transforms>" +
                                                  @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>" +
                                               "</Transforms>" +
                                               @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>" +
                                               "<DigestValue>" + digest + "</DigestValue>" +
                                            "</Reference>" +
                                         "</SignedInfo>";
            string signature = soapClass.Sign(canonicalSignedInfo, certificate);
            string soap_request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">" +
                        @"<s:Header/>" +
                        @"<s:Body>" +
                            @"<des:PeticionDescargaMasivaTercerosEntrada>" +
                                @"<des:peticionDescarga " +
                                @"IdPaquete=""" + idPaquete +
                                @""" RfcSolicitante=""" + rfcSolicitante +
                                @""">" +
                                                    @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                                    @"<SignedInfo>" +
                                                    @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                                    @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
                                                    @"<Reference URI=""#_0"">" +
                                                        @"<Transforms>" +
                                                        @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                                        @"</Transforms>" +
                                                        @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
                                                        @"<DigestValue>" + digest + @"</DigestValue>" +
                                                    @"</Reference>" +
                                                    @"</SignedInfo>" +
                                                    @"<SignatureValue>" + signature + "</SignatureValue>" +
                                                    @"<KeyInfo>" +
                                                        @"<X509Data>" +
                                                            @"<X509IssuerSerial>" +
                                                                @"<X509IssuerName>" + certificate.Issuer +
                                                                @"</X509IssuerName>" +
                                                                @"<X509SerialNumber>" + certificate.SerialNumber +
                                                                @"</X509SerialNumber>" +
                                                            @"</X509IssuerSerial>" +
                                                            @"<X509Certificate>" + Convert.ToBase64String(certificate.RawData) + "</X509Certificate>" +
                                                        @"</X509Data>" +
                                                    @"</KeyInfo>" +
                                                    @"</Signature>" +
                                                    @"</des:peticionDescarga>" +
                                                @"</des:PeticionDescargaMasivaTercerosEntrada>" +
                                            @"</s:Body>" +
                                            @"</s:Envelope>";
            return soap_request;
        }
        private static string descarga_GetResult(XmlDocument xmlDoc)
        {
            string s = xmlDoc.GetElementsByTagName("Paquete")[0].InnerXml;
            return s;
        }


    }
}
