using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace test2
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {

            Bitmap foto1 = new Bitmap("testFoto8.jpg");
            Bitmap foto2 = new Bitmap("testFoto8.jpg");

            int height = foto1.Height;
            int width = foto1.Width;
            
            Segmentation seg = new Segmentation(foto1);
            seg.SortRebr();

            // Размытие Гаусса
            double[,] kernel = {{0, 1, 0}, {1, 4, 1}, {0, 1, 0}};
            // Оператор Собеля
            double[,] kernelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            double[,] kernelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            // Оператор Щарра
            double[,] kernelShX = { { -3, 0, 3 }, { -10, 0, 10 }, { -3, 0, 3 } };
            double[,] kernelShY = { { -3, -10, -3 }, { 0, 0, 0 }, { 3, 10, 3 } };
            //Bitmap fotoAfterSvertka = Svertka(_foto, height, width, kernelY);
            foto2 = Filters.Sobel(foto2);

            

            double[,] kernel2 = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
            
            foto1 = Filters.Svertka(foto1, height, width, kernel);
            //foto2 = Filters.Svertka(foto2, height, width, kernelY);
           //Filters.Svertka(foto1, height, width, kernel);

            //////////////////////////////////////////////////////////////////////
            
            
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(foto1, foto2));
        }
    }

    public static class Filters
    {

        public static void CannyFilter(Bitmap foto)
        {

            int width = foto.Width;
            int height = foto.Height;
            Bitmap linesX, linesY;

            // избавимся от шумов фильтром Гаусса
            double[,] kernel = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
            foto = Svertka(foto, height, width, kernel);
            // Считаем градиент
            foto = Sobel(foto);

            // делаем границы тонкими
            double[,] kernelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            double[,] kernelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            linesX = Filters.Svertka(foto, height, width, kernelX);
            linesY = Filters.Svertka(foto, height, width, kernelY);


            // связываем контуры

        }
        public static Bitmap Sobel2(Bitmap foto)
        {

            int width = foto.Width;
            int height = foto.Height;

            // избавимся от шумов
            double[,] kernel = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
            foto = Svertka(foto, height, width, kernel);


            // а если поднять резкость?
            //double[,] kernel2 = {{ -0.1, -0.1, -0.1}, {-0.1, 2, -0.1}, {-0.1, -0.1, -0.1}};
            //foto = Svertka(foto, height, width, kernel2);
            //foto = Svertka(foto, height, width, kernel2);

            // Оператор Собеля
            double[,] kernelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            double[,] kernelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            // лимит для вычисления границ
            int limit = 128 * 128;

            // проходим свёрткой по X и Y
            Bitmap fotoX = Svertka(foto, height, width, kernelX);
            Bitmap fotoY = Svertka(foto, height, width, kernelY);

            // создаём массивы байтов для свёртки по X и Y, а также итоговый с нужным размером
            byte[] bytesX = GetBytes(fotoX);
            byte[] bytesY = GetBytes(fotoY);
            byte[] outputBytes = new byte[bytesX.Length];


            /*
            for (int i = 0; i < bytesX.Length; i = i + 3)
            {
              int r = bytesX[i] + bytesY[i];
              int g = bytesX[i+1] + bytesY[i+1];
              int b = bytesX[i+2] + bytesY[i+2];
              if (r > 255)
                  outputBytes[i] = 255;
              else
                  outputBytes[i] = (byte)r;

              if (g > 255)
                  outputBytes[i] = 255;
              else
                  outputBytes[i] = (byte)g;

              if (b > 255)
                  outputBytes[i] = 255;
              else
                  outputBytes[i] = (byte)b;
            }
            */
            
            // Конвертируем полученные байты обратно в Битмап
            return GetBitmap(outputBytes, width, height);
        }
        public static Bitmap Sobel(Bitmap foto)
        {

            int width = foto.Width;
            int height = foto.Height;

            // избавимся от шумов
            double[,] kernel = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
            foto = Svertka(foto, height, width, kernel);


            // а если поднять резкость?
            //double[,] kernel2 = {{ -0.1, -0.1, -0.1}, {-0.1, 2, -0.1}, {-0.1, -0.1, -0.1}};
            //foto = Svertka(foto, height, width, kernel2);
            //foto = Svertka(foto, height, width, kernel2);

            // Оператор Собеля
            double[,] kernelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            double[,] kernelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            // лимит для вычисления границ
            int limit = 128*128;

            //int limit = (foto.Height * foto.Width);

            // переводим наше изображение в байты
            byte[] inputBytes = GetBytes(foto);
            // создаём массив для итога с нужным размером
            byte[] outputBytes = new byte[inputBytes.Length];

            // вдруг потом понадобится другой размер, пусть будет так 
            int kernelWidth = kernelX.GetLength(0);
            int kernelHeight = kernelX.GetLength(1);

            double sumRx;
            double sumGx;
            double sumBx;
            double sumRy;
            double sumGy;
            double sumBy;
            double sumKernelx;
            double sumKernely;

            // проходим по изображению, не обрабатывая края
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    sumRx = 0;
                    sumGx = 0;
                    sumBx = 0;
                    sumRy = 0;
                    sumGy = 0;
                    sumBy = 0;
                    sumKernelx = 0;
                    sumKernely = 0;

                    // проходим по ядру
                    for (int i = 0; i < kernelWidth; i++)
                    {
                        for (int j = 0; j < kernelHeight; j++)
                        {
                            int positionX = x + (i - (kernelWidth/2));
                            int positionY = y + (j - (kernelHeight/2));

                            // не обрабатываются края (при их категоричности доработаю)
                            if ((positionX < 0) || (positionX >= width) || (positionY < 0) || (positionY >= height))
                                continue;

                            // т.к. всё лежит подряд в массиве, то и умножаем позицию на 3, получаем 3 палитры подряд
                            byte rX = inputBytes[3*(width*positionY + positionX) + 0];
                            byte gX = inputBytes[3*(width*positionY + positionX) + 1];
                            byte bX = inputBytes[3*(width*positionY + positionX) + 2];

                            byte rY = inputBytes[3*(width*positionY + positionX) + 0];
                            byte gY = inputBytes[3*(width*positionY + positionX) + 1];
                            byte bY = inputBytes[3*(width*positionY + positionX) + 2];

                            double kernelValueX = kernelX[i, j];
                            double kernelValueY = kernelY[i, j];

                            sumRx += rX*kernelValueX;
                            sumGx += gX*kernelValueX;
                            sumBx += bX*kernelValueX;

                            sumRy += rY*kernelValueY;
                            sumGy += gY*kernelValueY;
                            sumBy += bY*kernelValueY;

                            sumKernelx += kernelValueX;
                            sumKernely += kernelValueY;

                        }
                    }
                    //?????????????
                    


                    if (sumRx * sumRx + sumRy * sumRy > limit || sumGx * sumGx + sumGy * sumGy > limit ||
                        sumBx * sumBx + sumBy * sumBy > limit)
                    {
                        outputBytes[3 * (width * y + x) + 0] = 0;
                        outputBytes[3 * (width * y + x) + 1] = 0;
                        outputBytes[3 * (width * y + x) + 2] = 0;
                    }
                    else
                    {
                        outputBytes[3 * (width * y + x) + 0] = 255;
                        outputBytes[3 * (width * y + x) + 1] = 255;
                        outputBytes[3 * (width * y + x) + 2] = 255;
                    }
                    
                    /*
                    if (((byte) Math.Sqrt(sumRx*sumRx + sumRy*sumRy) <= limit) ||
                        ((byte) Math.Sqrt(sumGx*sumGx + sumGy*sumGy) <= limit) ||
                        ((byte) Math.Sqrt(sumBx*sumBx + sumBy*sumBy) <= limit))
                    {
                        outputBytes[3*(width*y + x) + 0] = (byte) Math.Sqrt(sumRx*sumRx + sumRy*sumRy);
                        outputBytes[3*(width*y + x) + 1] = (byte) Math.Sqrt(sumGx*sumGx + sumGy*sumGy);
                        outputBytes[3*(width*y + x) + 2] = (byte) Math.Sqrt(sumBx*sumBx + sumBy*sumBy);
                    }
                    else
                    {
                        outputBytes[3 * (width * y + x) + 0] = 0;
                        outputBytes[3 * (width * y + x) + 1] = 0;
                        outputBytes[3 * (width * y + x) + 2] = 0;
                    }
                     */

                    ///////////////////////////////////////////////////////
                    /*
                    // Нельзя делить на ноль
                    if (sumKernelx <= 0)
                        sumKernelx = 1;

                    // Нельзя выйти за цветовые пределы
                    sumRx = sumRx / sumKernelx;
                    if (sumRx < 0)
                        sumRx = 0;
                    if (sumRx > 255)
                        sumRx = 255;

                    sumGx = sumGx / sumKernelx;
                    if (sumGx < 0)
                        sumGx = 0;
                    if (sumGx > 255)
                        sumGx = 255;

                    sumBx = sumBx / sumKernelx;
                    if (sumBx < 0)
                        sumBx = 0;
                    if (sumBx > 255)
                        sumBx = 255;
                    // Нельзя делить на ноль
                    if (sumKernely <= 0)
                        sumKernely = 1;

                    // Нельзя выйти за цветовые пределы
                    sumRy = sumRy / sumKernely;
                    if (sumRy < 0)
                        sumRy = 0;
                    if (sumRy > 255)
                        sumRy = 255;

                    sumGy = sumGy / sumKernely;
                    if (sumGy < 0)
                        sumGy = 0;
                    if (sumGy > 255)
                        sumGy = 255;

                    sumBy = sumBy / sumKernely;
                    if (sumBy < 0)
                        sumBy = 0;
                    if (sumBy > 255)
                        sumBy = 255;
                   */
                      ///////////////////////////////////////////////////////
                    /*
                    outputBytes[3 * (width * y + x) + 0] = (byte)Math.Sqrt(sumRx * sumRx + sumRy * sumRy);
                    outputBytes[3 * (width * y + x) + 1] = (byte)Math.Sqrt(sumGx * sumGx + sumGy * sumGy);
                    outputBytes[3 * (width * y + x) + 2] = (byte)Math.Sqrt(sumBx * sumBx + sumBy * sumBy);
                   */
                }
            }
            // Конвертируем полученные байты обратно в Битмап
            return GetBitmap(outputBytes, width, height);
        }


        public static byte[] GetBytes(Bitmap input)
        {
            int count = input.Height*input.Width*3; // размер нашего изображения 
            BitmapData inputD = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb); // выделяем память
            var output = new byte[count];
            Marshal.Copy(inputD.Scan0, output, 0, count); // копируем себе в массив
            input.UnlockBits(inputD); // разблокировка памяти
            return output;
        }

        // получение Битмапа из байтов
        public static Bitmap GetBitmap(byte[] input, int width, int height)
        {
            if (input.Length%3 != 0)
                return null;
                    // проверяем сможем ли мы сконвертировать обратно (должно делиться на 3, так хранятся цветные пиксели)
            var output = new Bitmap(width, height);
            BitmapData outputD = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb); // выделяем память

            Marshal.Copy(input, 0, outputD.Scan0, input.Length);
            output.UnlockBits(outputD); // разблокировка памяти
            return output;
        }


        // алгоритм свёртки для фильтров
        public static Bitmap Svertka(Bitmap foto, int height, int width, double[,] kernel) // принимает параметры ядра
        {
            // переводим наше изображение в байты
            byte[] inputBytes = GetBytes(foto);
            // создаём массив для итога с нужным размером
            byte[] outputBytes = new byte[inputBytes.Length];

            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);

            double sumR;
            double sumG;
            double sumB;
            double sumKernel;

            // проходим по изображению, не обрабатывая края
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    sumR = 0;
                    sumG = 0;
                    sumB = 0;
                    sumKernel = 0;

                    // проходим по ядру
                    for (int i = 0; i < kernelWidth; i++)
                    {
                        for (int j = 0; j < kernelHeight; j++)
                        {
                            int positionX = x + (i - (kernelWidth/2));
                            int positionY = y + (j - (kernelHeight/2));

                            // не обрабатываются края (при их категоричности доработаю)
                            if ((positionX < 0) || (positionX >= width) || (positionY < 0) || (positionY >= height))
                                continue;

                            // т.к. всё лежит подряд в массиве, то и умножаем позицию на 3, получаем 3 палитры подряд
                            byte r = inputBytes[3*(width*positionY + positionX) + 0];
                            byte g = inputBytes[3*(width*positionY + positionX) + 1];
                            byte b = inputBytes[3*(width*positionY + positionX) + 2];

                            double kernelValue = kernel[i, j];

                            sumR += r*kernelValue;
                            sumG += g*kernelValue;
                            sumB += b*kernelValue;

                            sumKernel += kernelValue;
                        }
                    }

                    // Нельзя делить на ноль
                    if (sumKernel <= 0)
                        sumKernel = 1;

                    // Нельзя выйти за цветовые пределы
                    sumR = sumR/sumKernel;
                    if (sumR < 0)
                        sumR = 0;
                    if (sumR > 255)
                        sumR = 255;

                    sumG = sumG/sumKernel;
                    if (sumG < 0)
                        sumG = 0;
                    if (sumG > 255)
                        sumG = 255;

                    sumB = sumB/sumKernel;
                    if (sumB < 0)
                        sumB = 0;
                    if (sumB > 255)
                        sumB = 255;

                    // Записываем результат в цвет пикселя
                    outputBytes[3*(width*y + x) + 0] = (byte) sumR;
                    outputBytes[3*(width*y + x) + 1] = (byte) sumG;
                    outputBytes[3*(width*y + x) + 2] = (byte) sumB;
                }
            }
            // Конвертируем полученные байты обратно в Битмап
            return GetBitmap(outputBytes, width, height);
        }



        /*
            Color ourMap = this.ColorMap();
            Color[,] newMap = new Color[height,width];
            return Color newMap[,];
            */

        /*
        // Фильтр Гаусса
        public void Gauss()
        {
            // Использует определенные значения свёртки
            double[,] kernel = {( )};
        }
         * */
    }
}

