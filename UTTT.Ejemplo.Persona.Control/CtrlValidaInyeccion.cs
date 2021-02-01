using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UTTT.Ejemplo.Persona.Control
{
    public class CtrlValidaInyeccion
    {
        public bool htmlInyectionValida(string _Info, ref string _mensaje, string _etinquetaR,
            ref System.Web.UI.WebControls.TextBox _control)
        {
            Regex tagPegex = new Regex(@"<\s*([^>]+)[^>]*>.*?<\s*/\s*\1\s*>");
            bool respuesta = tagPegex.IsMatch(_Info);
            if (respuesta)
            {
                _mensaje = "Caracter no valido en" + _etinquetaR.Replace(":", "");
                _control.Focus();
            }
            return respuesta;
        }
        public bool SQLInyectionValida(string _Info, ref string _mensaje, string _etinquetaR,
            ref System.Web.UI.WebControls.TextBox _control)
        {
            Regex tagRegex = new Regex(@"('(''|[^'])*')|(\b(ALTER|alter|Alter|CREATE|create|Create|
            DELETE|Delete|Delete|DROP|Drop|drop|Exec(UTE){0,1}|INSERT( +INTO){0.1}|
        insert( +into){0,1}|Insert( +into){0,1}|MERGE|Union( +all){0,1}\b)");
            bool respuesta = tagRegex.IsMatch(_Info);
            if (respuesta)
            {
                _mensaje = "Cararacter no valido en" + _etinquetaR.Replace(":", "");
                _control.Focus();
            }
            return respuesta;
        }

    }
}
