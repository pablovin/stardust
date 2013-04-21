using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceApps
{
    class StarTrack
    {               
        public void doTracking(String path, String image, String resultPath)
        {
            FileInfo[] list = new DirectoryInfo(path).GetFiles("*.jpg");
            int posicaoImage=0;

            for (int i = 0; i < list.Count(); i++)
                if (image.Equals(list[i].FullName))
                {
                    posicaoImage = i;
                    break;
                }                        
            
            Blob primeiroBlob = getBlobMaisAlto(image);
            Blob trackBlob = primeiroBlob;
            for (int i = posicaoImage; i < list.Count(); i++)
            {
                try
                {                    
                    trackBlob = tracking(trackBlob, list[i].FullName);

                    if (trackBlob == null)
                        break;
                    else
                        new Bitmap(list[i].FullName).Save(resultPath+@"\"+i+".png");
                }
                catch (Exception ex)
                {
                }
            }

            trackBlob = getBlobMaisAlto(image);

            for (int i = posicaoImage; i > 0; i--)
            {
                try
                {
                    trackBlob = tracking(trackBlob, list[i].FullName);

                    if (trackBlob == null)
                        break;
                    else
                        new Bitmap(list[i].FullName).Save(resultPath + @"\" + i + ".png");
                    
                }
                catch (Exception ex)
                {
                }
            }

        }

        private Bitmap hslImage(String path)
        {
            try
            {
                Bitmap image = new Bitmap(path);
              
                HSLFiltering filter = new HSLFiltering();

                //filter.Hue = new AForge.IntRange(335,0);
                filter.UpdateHue = false;
                filter.Luminance = new AForge.Range(0.4f, 1);
                filter.Saturation = new AForge.Range(0.6f, 1);

                image = filter.Apply(image);
                return image;
            }
            catch (Exception ex)
            {
                throw new Exception("Deu erro na conversão");
            }
            
        }

        private Blob tracking(Blob primeiroBlob, String path)
        {
            try
            {
                Bitmap imagem = hslImage(path);

                // process image with blob counter
                BlobCounter blobCounter = new BlobCounter();
                blobCounter.ProcessImage(imagem);
                Blob[] blobs = blobCounter.GetObjectsInformation();

                Blob maisProximo = blobs[0];

                double menorDistancia = -1;

                foreach (Blob blob in blobs)
                {
                    if (menorDistancia == -1)
                    {
                        maisProximo = blob;
                        menorDistancia = distanciaEuclidiana(blob.CenterOfGravity.X, blob.CenterOfGravity.Y, primeiroBlob.CenterOfGravity.X, primeiroBlob.CenterOfGravity.Y);

                    }
                    else
                    {
                        double possivelMenorDistancia = distanciaEuclidiana(blob.CenterOfGravity.X, blob.CenterOfGravity.Y, primeiroBlob.CenterOfGravity.X, primeiroBlob.CenterOfGravity.Y);

                        if (possivelMenorDistancia < menorDistancia)
                        {
                            menorDistancia = possivelMenorDistancia;
                            maisProximo = blob;
                        }

                    }
               }     
           
                double distancia = distanciaEuclidiana(primeiroBlob.CenterOfGravity.X,primeiroBlob.CenterOfGravity.Y,maisProximo.CenterOfGravity.X,maisProximo.CenterOfGravity.Y);
                if (primeiroBlob.Area > 15 && maisProximo.Area > 15 && Math.Abs(maisProximo.Area - primeiroBlob.Area) > 100 || 
                     distancia > 100)
                    maisProximo = null;
                return maisProximo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private double distanciaEuclidiana(double x1, double y1, double x2, double y2)
        {
          //  return Math.Pow((Math.Pow((x1-x2),2) + Math.Pow((y1-y2),2)),1/2);

            return ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        private Blob getBlobMaisAlto(String path)
        {
            try
            {
                Bitmap image = hslImage(path);

                // process image with blob counter
                BlobCounter blobCounter = new BlobCounter();
                blobCounter.ProcessImage(image);
                Blob[] blobs = blobCounter.GetObjectsInformation();

                Blob blobMaisAlto = blobs[0];

                // process each blob
                foreach (Blob blob in blobs)
                {
                    if (blob.CenterOfGravity.Y < blobMaisAlto.CenterOfGravity.Y)
                        blobMaisAlto = blob;
                }

                return blobMaisAlto;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
