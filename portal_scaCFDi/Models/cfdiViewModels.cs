using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal_scaCFDi.Models.cfdiViewModels
{

    public class List_peticionViewModels
    {
        public string idcliente { get; set; }
        public string rfc_cliente { get; set; }
        public string fechapet { get; set; }
        public string periodopet { get; set; }
        public bool emitidos { get; set; }
        public string razonsocial { get; set; }
        public List<cfdiList> detalles { get; set; }
    }
    public class cfdiList
    {
        public string id { get; set; }
        public string rfc_emisor { get; set; }
        public string razon_emisor { get; set; }
        public string rfc_receptor { get; set; }
        public string razon_receptor { get; set; }
        public string tipocomprobante { get; set; }
        public string emision { get; set; }
        public string fecha_timbre { get; set; }
        public string uuid { get; set; }
        public string subtotal { get; set; }
        public string descuento { get; set; }
        public string totaltraslados { get; set; }
        public string totalretenidos { get; set; }
        public string total { get; set; }
        public string estatus { get; set; }
    }

    public class cfdi_ERViewModels
    {
        public string rfc_cliente { get; set; }
        public string razonsocial { get; set; }
        public string idcliente { get; set; }
        public Donut emitidos { get; set; }
        public Donut recibidos { get; set; }
    }

    public class Donut
    {
        public string element { get; set; }
        public string[] colors { get; set; }
        public data[] datas { get; set; }

    }

    public class data
    {
        public string label { get; set; }
        public int value { get; set; }
    }

    public class tipocomprobanteViewModels
    {
        public string rfc_cliente { get; set; }
        public string razonsocial { get; set; }
        public string idcliente { get; set; }
        public int ingresos { get; set; }
        public int egresos { get; set; }
        public int pagos { get; set; }
        public int nominas { get; set; }
        public bool emitidos { get; set; }
    }
}