using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal_scaCFDi.Models.clienteViewModels
{
    public class CreateViewModels
    {
        [Required]
        [MaxLength(13)]
        [RegularExpression(@"[A-Z&Ñ]{3,4}[0-9]{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])[A-Z0-9]{2}[0-9A]", ErrorMessage = "El RFC no tiene la estructura correcta")]
        public string RFC { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Razón Social")]
        public string razonsocial { get; set; }

        [Required]
        public HttpPostedFileBase pfx { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        [MaxLength(500)]
        public string pass { get; set; }

        public bool error { get; set; }
        public string mensaje { get; set; }
    }

    public class ListViewModels
    {
        public List<clientesList> detalles { get; set; }
    }

    public class clientesList
    {
        public string idcliente { get; set; }
        public string nombrecliente { get; set; }
        public string rfc { get; set; }
        public string fechaalta { get; set; }
        public string fechabaja { get; set; }
    }
}