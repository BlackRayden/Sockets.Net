using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerPaint.Modelos;
using ServerPaint.Presentacion;

namespace ServerPaint
{
    

    public partial class Vista : Form
    {
        private Modelo _modelo;
        private Controlador _controlador; 
        public Vista()
        {
            InitializeComponent();
        }
        public Vista(Modelo modelo)
            :this()
        {
            _controlador = new Controlador(modelo);
            this._modelo = modelo;
        }

        public void Notificacion(string mensaje)
        {
            lblNotificacion.Text = mensaje;
        }

        public TextBox PantallaMensajes()
        {
            return textBox1;
        }

        public PictureBox GetLienzo()
        {
            return Lienzo;
        }

        public void MostrarTexto(StringBuilder  mensaje)
        {
            textBox1.Text = mensaje.ToString();
        }

        private void IniciarServidorAction(object sender, EventArgs e)
        {
            _controlador.IniciarServidor(sender  );
        }

        private void EnviarServidorAction(object sender, EventArgs e)
        {
            _controlador.EnviarMensajeCliente();
        }




        private void Vista_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }
    }
}
