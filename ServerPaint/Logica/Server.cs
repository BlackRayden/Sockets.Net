using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ServerPaint.Logica
{
    public delegate void RegistroConexionEvent( EventoConexionArgs e);

    public class EventoConexionArgs :EventArgs
    {
        public Socket conexion {get;set;}
    }

    public class ConexionSocket
    {
        private Socket _cliente;
        private Task _tareaEntrada;
        private CancellationToken _cancellationtoken;
        private readonly List<string> _mensajesEntrada;
        private readonly string _infoCliente;


        private readonly Guid _identificador;


        public string InfoCliente { get { return _infoCliente; } }
        public Guid  Id { get { return _identificador; } }

        public Socket Cliente
        {
            get { return _cliente; }
            set { _cliente = value; }
        }

        public Task TareaCanalEntrada { get { return _tareaEntrada; } }

        public ConexionSocket(Socket socket, CancellationToken token )
        {
            _mensajesEntrada = new List<string>();
            _cancellationtoken = token;
            _cliente = socket;
            _infoCliente = socket.RemoteEndPoint.ToString();
            _identificador = new Guid();
        }

        public IEnumerable<string> GetMensajes()
        {
            foreach (var mensaje in _mensajesEntrada)
            {
                yield return mensaje;
            }
        }

        public void   ListenerCanalEntrada()
        {
            try
            {
                string mensaje = string.Empty;
                byte[] serverBuffer = new byte[10025];
                int bytes = 0;
                while (true)
                {
                    
                    serverBuffer = new byte[10025];
                    bytes = 0;
                    bytes = _cliente.Receive(serverBuffer, serverBuffer.Length, 0);
                    _cancellationtoken.ThrowIfCancellationRequested();
                    var texto = Encoding.ASCII.GetString(serverBuffer, 0, bytes);
                    if (!string.IsNullOrWhiteSpace(texto))
                    {
                        lock (_mensajesEntrada)
                        {

                            _mensajesEntrada.Add(Encoding.ASCII.GetString(serverBuffer, 0, bytes));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DesconectarCliente();
            }
}

        public void   RegistrarCanalEntrada()
        {
            _tareaEntrada =  Task.Run(() => {
                ListenerCanalEntrada();
            }, _cancellationtoken);

        }

        public void   DesconectarCliente()
        {
            try
            {
                _cliente.Disconnect(false);
                _cliente.Close();
                _cliente.Dispose();
                _mensajesEntrada.Clear();
            }
            catch (Exception) { }
        }

        public void   EnviarMensaje(string mensaje)
        {
            if (_cliente.Connected)
            {
                var  serverBuffer = Encoding.ASCII.GetBytes(mensaje);
                _cliente.Send(serverBuffer); 

            }
        }
     
    }

   
    

    public class Server
    {
        private TcpListener _listener;
        private IPAddress _ip;
        private CancellationTokenSource _tokenSource;
        private List<ConexionSocket> _conexiones { get; set; }

        public event RegistroConexionEvent NuevaConexionEvent;

        public IEnumerable<ConexionSocket> GetConexiones()
        {
            foreach (var conexion in _conexiones)
            {
                yield return conexion;
            } 
        }

        public Server()
        {
            _conexiones = new List<ConexionSocket>();
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (  ip.AddressFamily == AddressFamily.InterNetwork )
                {
                    _ip = ip;
                    break;
                }
            }
           
        }

        public Server(CancellationTokenSource _tokenSource)
            :this()
        {
            this._tokenSource = _tokenSource;
        }

        protected void OnNuevaConexion(EventoConexionArgs e)
        {
            RegistroConexionEvent manejador = NuevaConexionEvent;
            if (manejador != null)
            {
                manejador(e);
            }
        }

        private ConexionSocket RegistrarConexion(Socket cliente, CancellationToken token)
        {

            var a = new EventoConexionArgs() { conexion = cliente };
            OnNuevaConexion(a);            
            var conexion = new ConexionSocket(cliente, token ) ;
            conexion.RegistrarCanalEntrada();
            return conexion;
        }

        public void IniciarServidor(int puerto, CancellationToken token)
        {
            try
            {
                _listener = new TcpListener(_ip, puerto);
                _listener.Start();
                while (true)
                {
                    
                    var cliente = _listener.AcceptSocket();
                    token.ThrowIfCancellationRequested();
                    lock ( _conexiones)
                    {
                        _conexiones.Add(RegistrarConexion(cliente, token));
                    }
                }

               

            }
            catch (OperationCanceledException ex)
            {
                _listener.Stop();
            }
            catch (Exception ex)
            {

            }
        }

        public void DetenerServicio()
        {
            foreach (var registro in GetConexiones())
            {
                registro.DesconectarCliente();
            }
        }

        public Socket GetPeticion()
        {
            
            return _listener.AcceptSocket();
        }

        public string GetIp()
        {
            return _ip.ToString();
        }

    }
}
