using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPaint.Logica
{
    public class ServicioDibujo
    {
        private int _largo,_alto;
        public const int xTriangulo = 50;
        public const int yTriangulo = 50;
        private Bitmap _triangulo = null;
        private Point _PuntoInicial;

        public ServicioDibujo( int largo , int alto  )
        { 
            _largo =largo;
            _alto = alto;
            _triangulo = CrearTriangulo();
            _PuntoInicial = new Point(_largo / 2, _alto / 2);  

        }

        public Bitmap CrearTriangulo()
        {
            var imagen =new Bitmap(xTriangulo, yTriangulo);
            var graph = Graphics.FromImage(imagen);

            var gp = new GraphicsPath();
            var punto1 = new Point(0, 0);
            var punto2 = new Point(xTriangulo/2 ,yTriangulo );
            var punto3 = new Point(xTriangulo, 0);
            gp.AddLine(punto1, punto2);
            gp.AddLine(punto2, punto3);
            gp.AddLine(punto1, punto3);
            gp.CloseFigure();

            graph.DrawPath(new Pen(Color.Black, 2), gp);
            graph.FillPath(Brushes.Green, gp );
            graph.Flush();
            graph.Save();
            return imagen;
        }


        public Bitmap Avanzar(int pixeles)
        {
            var imagenGeneral = new Bitmap(_largo, _alto);
            var graph2 = Graphics.FromImage(imagenGeneral);
            graph2.FillRectangle(Brushes.Gainsboro, 0, 0, _largo, _alto);
            graph2.Flush();
            _PuntoInicial.Y += pixeles;
            graph2.DrawImage(_triangulo, _PuntoInicial);
            _triangulo.RotateFlip(RotateFlipType.Rotate180FlipX);
            graph2.Flush();
            return imagenGeneral;
        }


        public Bitmap  Iniciar()
        {
           
            var imagenGeneral = new Bitmap( _largo, _alto );
            var graph2 = Graphics.FromImage(imagenGeneral);

            graph2.DrawImage(_triangulo, _PuntoInicial);
            _triangulo.RotateFlip(RotateFlipType.Rotate180FlipX);
            graph2.Save();
            return imagenGeneral;


        }




        public Bitmap  Girar(int p)
        {

            var imagenGeneral = new Bitmap(_largo, _alto);
            var graph2 = Graphics.FromImage(imagenGeneral);
            graph2.FillRectangle(Brushes.Gainsboro, 0, 0, _largo, _alto);
            graph2.Flush();
            graph2.TranslateTransform(_PuntoInicial.X, _PuntoInicial.Y);
            
            graph2.DrawImage(_triangulo, _PuntoInicial);
            graph2.RotateTransform(15);
            
            

            //_triangulo.RotateFlip(RotateFlipType.Rotate90FlipXY);
            
           
            graph2.Flush();
            return imagenGeneral;
        }


        private static Bitmap RotateImage(Image image, PointF offset, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");
        
            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
    
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);
    
            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);
    
            //rotate the image
            g.RotateTransform(angle);
    
            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);
    
            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));
    
            return rotatedBmp;
        }

    }
}
