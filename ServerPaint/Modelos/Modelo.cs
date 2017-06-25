using ServerPaint.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerPaint.Modelos
{
    public class Modelo
    {

        private Server _servidor;
        private Vista _vista;
        private CancellationTokenSource _tokenSource;
        private ServicioDibujo _servicioDibujo;
        

        public Modelo()
        {
            _tokenSource = new CancellationTokenSource();
            _vista = new Vista(this);
            _servidor = new Server(_tokenSource);
            _servidor.NuevaConexionEvent += _servidor_NuevaConexionEvent;
            _servicioDibujo = new ServicioDibujo(_vista.GetLienzo().Width, _vista.GetLienzo().Height);

        }

        void _servidor_NuevaConexionEvent(EventoConexionArgs e)
        {
            //var texto = e.conexion.RemoteEndPoint.ToString();
            //GetVista().Notificacion(texto);
            var img  = _servicioDibujo.Iniciar();
            GetVista().GetLienzo().Image = img;
            Thread.Sleep(3000);
            var img1 = _servicioDibujo.Avanzar(100);
            Thread.Sleep(3000);
            //var img2= _servicioDibujo.Girar(150);
            GetVista().GetLienzo().Image = img1;

        }

        public void Iniciar()
        {
            _vista.Notificacion("Servidor Detenido");
            _vista.ShowDialog();


        }

        public Vista GetVista()
        {
            return _vista;
        }

        public  void IniciarServidor()
        {
            _tokenSource = new CancellationTokenSource();
            var tokenCancelacion = _tokenSource.Token;
            Task.Factory.StartNew(() =>
            {
                _servidor.IniciarServidor(8000 , tokenCancelacion);
            }, tokenCancelacion);

            MonitorMensajesEntrada();
            GetVista().Notificacion("Servidor Iniciado");
           

        }

        public void EnviarMensajeCliente()
        {
            var v = "pruebas ";
            foreach (var conexion in _servidor.GetConexiones())
            {
                conexion.EnviarMensaje(v);
            }
        }

        public void DetenerServidor()
        {
            _servidor.DetenerServicio();
            _tokenSource.Cancel();
            
            GetVista().Notificacion("Servidor Detenido");
        }

        private void MonitorMensajesEntrada()
        {
            Task.Run(() =>
            {
                var mensajes = new StringBuilder();
                while (true)
                {
                    mensajes = new StringBuilder();
                    foreach (var conexion in _servidor.GetConexiones())
                    {
                        foreach (var mensaje  in  conexion.GetMensajes())
                        {
                            mensajes.AppendLine(string.Format("From:[{0}] Mensaje: {1}", conexion.InfoCliente, mensaje));
                        }
                    }
                    GetVista().PantallaMensajes().Text = mensajes.ToString();
                }
            });
        }
    }
}
