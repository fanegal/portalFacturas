using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal_scaCFDi.Models.PeticionViewModels
{
    public class PeticionList
    {
        public string id { get; set; }
        public string fechaCreacion { get; set; }
        public string inicio { get; set; }
        public string fin { get; set; }
        public string Emitido { get; set; }
        public string terminado { get; set; }
        public string error { get; set; }
        public string numxml { get; set; }
        public string modalidad { get; set; }

    }

    public class ListViewModels
    {
        public string idCliente { get; set; }
        public List<PeticionList> Listado { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public int dia { get; set; }
        public string nombre_cliente { get; set; }
    }

    public class CreateViewModels
    {
        [Display(Name = "Cliente")]
        public string nombreCliente { get; set; }
        [Required]
        [Display(Name = "Fecha de Inicio")]
        public DateTime inicio { get; set; }
        [Required]
        [Display(Name = "Fecha Fin")]
        public DateTime fin { get; set; }
        [Required]
        public string idcliente { get; set; }
        [Required]
        [Display(Name = "Emitido")]
        public bool emitido { get; set; }

        public bool error { get; set; }
        public string mensaje { get; set; }

    }
}