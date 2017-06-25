using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerPaint.Modelos;
using System.Windows.Forms;

namespace ServerPaint.Presentacion
{
    public class Controlador
    {
        private Modelo _modelo;

        public Controlador(Modelo modelo)
        {
            _modelo = modelo;
        }

        public  void IniciarServidor(object sender)
        {
            var boton = sender as Button;
            var operacion =   boton.Tag.ToString();
            if (operacion == "1")
            {
                boton.Text = Constantes.DESHABILITAR_SERVIDOR;
                boton.Tag = "2";
                _modelo.IniciarServidor();
            }
            else
            {
                boton.Text = Constantes.HABILITAR_SERVIDOR;
                boton.Tag = "1";
                _modelo.DetenerServidor();
            }
        }

        public void EnviarMensajeCliente()
        {
            _modelo.EnviarMensajeCliente();
        }
    }
}
