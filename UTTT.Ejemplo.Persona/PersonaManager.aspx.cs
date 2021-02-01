#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UTTT.Ejemplo.Linq.Data.Entity;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Collections;
using UTTT.Ejemplo.Persona.Control;
using UTTT.Ejemplo.Persona.Control.Ctrl;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;


#endregion

namespace UTTT.Ejemplo.Persona
{
    public partial class PersonaManager : System.Web.UI.Page
    {
        #region Variables

        private SessionManager session = new SessionManager();
        private int idPersona = 0;
        private UTTT.Ejemplo.Linq.Data.Entity.Persona baseEntity;
        private DataContext dcGlobal = new DcGeneralDataContext();
        private int tipoAccion = 0;
        string mensaje;

        #endregion

        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Response.Buffer = true;
                this.session = (SessionManager)this.Session["SessionManager"];
                this.idPersona = this.session.Parametros["idPersona"] != null ?
                    int.Parse(this.session.Parametros["idPersona"].ToString()) : 0;
                if (this.idPersona == 0)
                {
                    this.baseEntity = new Linq.Data.Entity.Persona();
                    this.tipoAccion = 1;
                }
                else
                {
                    this.baseEntity = dcGlobal.GetTable<Linq.Data.Entity.Persona>().First(c => c.id == this.idPersona);
                    this.tipoAccion = 2;
                }

                if (!this.IsPostBack)
                {
                    if (this.session.Parametros["baseEntity"] == null)
                    {
                        this.session.Parametros.Add("baseEntity", this.baseEntity);
                    }
                    List<CatSexo> lista = dcGlobal.GetTable<CatSexo>().ToList();
                    CatSexo catTemp = new CatSexo();
                    catTemp.id = -1;
                    catTemp.strValor = "Seleccionar";
                    lista.Insert(0, catTemp);
                    this.ddlSexo.DataTextField = "strValor";
                    this.ddlSexo.DataValueField = "id";
                    this.ddlSexo.DataSource = lista;
                    this.ddlSexo.DataBind();

                    this.ddlSexo.SelectedIndexChanged += new EventHandler(ddlSexo_SelectedIndexChanged);
                    this.ddlSexo.AutoPostBack = true;
                    if (this.idPersona == 0)
                    {
                        this.lblAccion.Text = "Agregar";
                    }
                    else
                    {
                        this.lblAccion.Text = "Editar";
                        this.txtNombre.Text = this.baseEntity.strNombre;
                        this.txtAPaterno.Text = this.baseEntity.strAPaterno;
                        this.txtAMaterno.Text = this.baseEntity.strAMaterno;
                        this.txtClaveUnica.Text = this.baseEntity.strClaveUnica;
                        this.TextBoxHermano.Text = this.baseEntity.intNumeroDeHermanos.ToString();
                        this.TextBoxEmail.Text = this.baseEntity.Correo;
                        this.TextBoxCode_postal.Text = this.baseEntity.Code_postal.ToString();
                        this.TextBoxRFC.Text = this.baseEntity.RFC;
                        
                        this.setItem(ref this.ddlSexo, baseEntity.CatSexo.strValor);
                    }
                }

            }
            catch (Exception _e)
            {
                this.showMessage("Ha ocurrido un problema al cargar la página");
                this.Response.Redirect("~/PersonaPrincipal.aspx", false);
            }

        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                DataContext dcGuardar = new DcGeneralDataContext();
                UTTT.Ejemplo.Linq.Data.Entity.Persona persona = new Linq.Data.Entity.Persona();
                if (this.idPersona == 0)
                {
                    persona.strClaveUnica = this.txtClaveUnica.Text.Trim();
                    persona.strNombre = this.txtNombre.Text.Trim();
                    persona.strAMaterno = this.txtAMaterno.Text.Trim();
                    persona.strAPaterno = this.txtAPaterno.Text.Trim();
                    persona.idCatSexo = int.Parse(this.ddlSexo.Text);
                    persona.intNumeroDeHermanos = int.Parse(this.TextBoxHermano.Text);
                    persona.calendar = DateTime.Parse(Calendar.SelectedDate.ToString());
                    persona.Correo = this.TextBoxEmail.Text.Trim();
                    persona.Code_postal = int.Parse(this.TextBoxCode_postal.Text);
                    persona.RFC = this.TextBoxRFC.Text.Trim();
                    if (!this.ValidarCampos(persona, ref mensaje, TextBoxDia.Text, TextBoxMes.Text, TextBoxAño.Text))
                    {

                        //this.lblMensaje.Text = mensaje;
                        //this.lblMensaje.Visible = true;
                        MessageBox.Show(mensaje);
                        return;
                    }
                    dcGuardar.GetTable<UTTT.Ejemplo.Linq.Data.Entity.Persona>().InsertOnSubmit(persona);
                    dcGuardar.SubmitChanges();
                    this.showMessage("El registro se agrego correctamente.");
                    this.Response.Redirect("~/PersonaPrincipal.aspx", false);

                }
                if (this.idPersona > 0)
                {
                    persona = dcGuardar.GetTable<UTTT.Ejemplo.Linq.Data.Entity.Persona>().First(c => c.id == idPersona);
                    persona.strClaveUnica = this.txtClaveUnica.Text.Trim();
                    persona.strNombre = this.txtNombre.Text.Trim();
                    persona.strAMaterno = this.txtAMaterno.Text.Trim();
                    persona.strAPaterno = this.txtAPaterno.Text.Trim();
                    persona.idCatSexo = int.Parse(this.ddlSexo.Text);
                    persona.intNumeroDeHermanos = int.Parse(this.TextBoxHermano.Text);
                    persona.calendar = DateTime.Parse(Calendar.SelectedDate.ToString());
                    persona.Correo = this.TextBoxEmail.Text.Trim();
                    persona.Code_postal = int.Parse(this.TextBoxCode_postal.Text);
                    persona.RFC = this.TextBoxRFC.Text.Trim();
                    dcGuardar.SubmitChanges();
                    this.showMessage("El registro se edito correctamente.");
                    this.Response.Redirect("~/PersonaPrincipal.aspx", false);
                }
            }
            catch (Exception _e)
            {
                this.showMessageException(_e.Message);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Response.Redirect("~/PersonaPrincipal.aspx", false);
            }
            catch (Exception _e)
            {
                this.showMessage("Ha ocurrido un error inesperado");
            }
        }

        protected void ddlSexo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int idSexo = int.Parse(this.ddlSexo.Text);
                Expression<Func<CatSexo, bool>> predicateSexo = c => c.id == idSexo;
                predicateSexo.Compile();
                List<CatSexo> lista = dcGlobal.GetTable<CatSexo>().Where(predicateSexo).ToList();
                CatSexo catTemp = new CatSexo();
                this.ddlSexo.DataTextField = "strValor";
                this.ddlSexo.DataValueField = "id";
                this.ddlSexo.DataSource = lista;
                this.ddlSexo.DataBind();
            }
            catch (Exception)
            {
                this.showMessage("Ha ocurrido un error inesperado");
            }
        }

        //public bool Validacion(Linq.Data.Entity.Persona _persona, ref String _mensaje) {
        //    if (_persona.idCatSexo.Equals(-1)) {
        //        _mensaje = "Seleccione un sexo";
        //        return false;
        //    }
        //}

        public bool ValidarCampos(Linq.Data.Entity.Persona persona, ref String _mensaje, string dia, string mes, string año)
        {
            if (persona.idCatSexo.Equals(-1))
            {
                Response.Write("Seleccione un sexo");
                return false;
            }
            if (persona.strClaveUnica.Equals(string.Empty)) {
                _mensaje = "clave unica es obligatoria";
                return false;
            }
            if (persona.strClaveUnica.Length !=3)
            {
                _mensaje = "clave unica es obligatoria";
                return false;
            }
            if (persona.strNombre.Length > 50) {
                _mensaje = "le tamaño maximo es de 50 letras";
                return false;
            }
            if (persona.strNombre.Equals(string.Empty))
            {
                _mensaje = "Nombre es obligatoria";
                return false;
            }
            if (persona.strAPaterno.Length > 50)
            {
                _mensaje = "le tamaño maximo es de 50 letras";
                return false;
            }
            if (persona.strAPaterno.Equals(string.Empty))
            {
                _mensaje = "Nombre es obligatoria";
                return false;
            }
            if (persona.strAMaterno.Length > 50)
            {
                _mensaje = "le tamaño maximo es de 50 letras";
                return false;
            }
            if (persona.strAMaterno.Equals(string.Empty))
            {
                _mensaje = "Nombre es obligatoria";
                return false;
            }
            if (persona.intNumeroDeHermanos > 10) {
                _mensaje = "No te cremos que tengas esa cantidad de hermanos";
                return false;
            }
            if (persona.intNumeroDeHermanos < 0) {
                _mensaje = "debes ingresar un nomero de herman@s";
                return false;
            }
            if (persona.Correo == "[a - zA - Z0 - 9_] + ([.][a - zA - Z0 - 9_] +) *@[a-zA - Z0 - 9_]+([.][a - zA - Z0 - 9_] +) *[.][a - zA - Z]{ 1,5}")
            {
                _mensaje = "Correo no valido";
                return false;
            }
            int Validar = (int.Parse(TextBoxDia.Text) * 24 * 60 * 60)+(int.Parse(TextBoxMes.Text)*30*60*60)+((2021-int.Parse(TextBoxAño.Text))*365*24*60*60);
            int may = 568024668;
            if (Validar < may) {
                _mensaje = "que cres que ases crio!, no ere mayor de edad :O ";
                return false;
            }
            if (persona.Code_postal.ToString() == "^d{5}(?:[-s]d{4})?$")
            {
                _mensaje = "codigo postal no es valido";
                return false;
            }
            if (persona.RFC == "^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))((-)?([A-Z]{3}))?")
            {
                _mensaje = "codigo postal no es valido";
                return false;
            }
            return true;
        }

        public new void Error(string error)
        {
            string body = error;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("18300496@uttt.edu.mx", "JOP1226J");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("18300496@uttt.edu.mx", "Error en mi servicodor de somee.com buscate otro XD");
            mail.To.Add(new MailAddress("18300496@uttt.edu.mx"));
            mail.Subject = ("Error");
            mail.Body = body;
            smtp.Send(mail);
        }

        public bool ValidacionSQL(ref String _mensaje)
        {
            CtrlValidaInyeccion valida = new CtrlValidaInyeccion();
            string mensajeFuncion = string.Empty;
            if (valida.SQLInyectionValida(this.txtClaveUnica.Text.Trim(), ref mensajeFuncion, "Clave unica", ref this.txtClaveUnica))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.txtNombre.Text.Trim(), ref mensajeFuncion, "Nombre", ref this.txtNombre))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.txtAPaterno.Text.Trim(), ref mensajeFuncion, "Apellido Paterno", ref this.txtAPaterno))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.txtAMaterno.Text.Trim(), ref mensajeFuncion, "Apellido Materno", ref this.txtAMaterno))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.TextBoxHermano.Text.Trim(), ref mensajeFuncion, "Hermanos", ref this.TextBoxHermano))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.TextBoxEmail.Text.Trim(), ref mensajeFuncion, "Email", ref this.TextBoxEmail))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.TextBoxCode_postal.Text.Trim(), ref mensajeFuncion, "Codigo Postal", ref this.TextBoxCode_postal))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.TextBoxRFC.Text.Trim(), ref mensajeFuncion, "RFC", ref this.TextBoxRFC))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            return true;
        }

        public bool ValidacionHtml(ref String _mensaje)
        {
            CtrlValidaInyeccion valida = new CtrlValidaInyeccion();
            string mensajeFuncion = string.Empty;
            if (valida.SQLInyectionValida(this.txtClaveUnica.Text.Trim(), ref mensajeFuncion, "Clave unica", ref this.txtClaveUnica))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.txtNombre.Text.Trim(), ref mensajeFuncion, "Nombre", ref this.txtNombre))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.txtAPaterno.Text.Trim(), ref mensajeFuncion, "Apellido Paterno", ref this.txtAPaterno))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.txtAMaterno.Text.Trim(), ref mensajeFuncion, "Apellido Materno", ref this.txtAMaterno))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.TextBoxHermano.Text.Trim(), ref mensajeFuncion, "Hermanos", ref this.TextBoxHermano))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.TextBoxEmail.Text.Trim(), ref mensajeFuncion, "Email", ref this.TextBoxEmail))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.TextBoxCode_postal.Text.Trim(), ref mensajeFuncion, "Codigo Postal", ref this.TextBoxCode_postal))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            if (valida.SQLInyectionValida(this.TextBoxRFC.Text.Trim(), ref mensajeFuncion, "RFC", ref this.TextBoxRFC))
            {
                _mensaje = mensajeFuncion;
                return false;
            }
            return true;
        }

        #endregion

        #region Metodos

        public void setItem(ref DropDownList _control, String _value)
        {
            foreach (ListItem item in _control.Items)
            {
                if (item.Value == _value)
                {
                    item.Selected = true;
                    break;
                }
            }
            _control.Items.FindByText(_value).Selected = true;
        }

        #endregion

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void TextBoxTime_TextChanged(object sender, EventArgs e)
        {
            
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            TextBoxDia.Text = Calendar.SelectedDate.Day.ToString();
            TextBoxMes.Text = Calendar.SelectedDate.AddDays(7).Month.ToString();
            TextBoxAño.Text = Calendar.SelectedDate.AddDays(7).Year.ToString();
            

            //int diaCumple = 1;
            //int mesCumple = 10;
            //int anioCumple = 2010;
            //DateTime fechaNacimiento = new DateTime(anioCumple, mesCumple, diaCumple);
            //int edad = (DateTime.Now.Subtract(fechaNacimiento).Days / 365);
            //DateTime proximoCumple;
            //if (DateTime.Now.Month <= mesCumple && DateTime.Now.Day <= diaCumple)
            //    proximoCumple = new DateTime(DateTime.Now.AddYears(1).Year, mesCumple, diaCumple);
            //else
            //proximoCumple = new DateTime(DateTime.Now.Year, mesCumple, diaCumple);
            //TimeSpan faltan = proximoCumple.Subtract(DateTime.Now);
            //StringBuilder sb = new StringBuilder();
            ////sb.AppendFormat("Usted Tiene {0} Años ", edad);
            //if (edad > 18)
            //{
            //    sb.AppendFormat("Usted Tiene {0} Años es mayor de edad ", edad); 
            //}
            //else {
            //    sb.AppendFormat("Usted Tiene {0} Años no es mayo de edad", edad);
            //    MessageBox.Show("alerta",sb.ToString());
            //}
        }

        protected void txtClaveUnica_TextChanged(object sender, EventArgs e)
        {
            
        }
        protected void birthday(object sender, EventArgs e) {
             
        }

        protected void TextBoxAño_TextChanged(object sender, EventArgs e)
        {

        }
    }

}