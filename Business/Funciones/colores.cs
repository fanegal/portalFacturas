using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Funciones
{
    public class colores
    {
        public colores() { }

        private string amarillo = "#FFFF00";
        private string azul = "#0000FF";
        private string rojo = "#FF0000";
        private string verde = "#008000";
        private string naranja = "#FFA500";
        private string violet = "#EE82EE";
        private string cafe = "#A52A2A";



        public List<string> get()
        {
            List<string> colors = new List<string>();
            colors.Add(amarillo);
            colors.Add(azul);
            colors.Add(rojo);
            colors.Add(verde);
            colors.Add(naranja);
            colors.Add(violet);
            colors.Add(cafe);
            return colors;
        }
    }
}
