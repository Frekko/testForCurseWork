using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;


namespace test2
{
    public class Segmentation
    {
        private int _width;
        private int _height;
        private byte[] _foto;
        public double limit = 200;
        List<Versh> versh = new List<Versh>();
        private List<Rib> ribs = new List<Rib>();

        public Segmentation(Bitmap foto2D)
        {
            _width = foto2D.Width;
            _height = foto2D.Height;
            _foto = Filters.GetBytes(foto2D);
        }

        public void SortRebr()
        {
            int length = _foto.Length/3;

            var r = new byte[length];
            var g = new byte[length];
            var b = new byte[length];


            bool firstExist = false;
            bool secondExist = false;

            byte number = 0;
            for (int i = 0; i < _foto.Length; i = i + 3)
            {
                r[number] = _foto[i];
                g[number] = _foto[i + 1];
                b[number] = _foto[i + 2];
            }
            int[,] rebr = {{0, 1}, {0, 2}, {0, 3}, {1, 0}, {2, 0}, {3, 0}};
            double dist = 0;
            //Array[] arr = Array.CreateInstance()
            // поставить проверку кратности трём массива байтов!
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    
                    for (int i = 0; i < 6; i++)
                    {

                        firstExist = false;
                        secondExist = false;

                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        // если следующий пиксел не существует
                        if ((positionX >= _width) || (positionY >= _height))
                            continue;

                        dist = Math.Sqrt(Math.Pow((x - positionX), 2) + Math.Pow((y - positionY), 2) +
                                         Math.Pow((r[x*_height + y] - r[positionX*_height + positionY]), 2) +
                                         Math.Pow((g[x*_height + y] - g[positionX*_height + positionY]), 2) +
                                         Math.Pow((b[x*_height + y] - b[positionX*_height + positionY]), 2));

                        Versh first = new Versh();
                        Versh second = new Versh();
                        foreach (Versh v in versh)
                        {
                            if (v._x == x && v._y == y)
                            {
                                first = v;
                                firstExist = true;
                            }
                            if (v._x == positionX && v._y == positionY)
                            {
                                second = v;
                                secondExist = true;
                            }
                        }
                        if (firstExist == false)
                        {
                            first = new Versh(x, y);
                            versh.Add(first);
                        }
                        if (secondExist == false)
                        {
                            second = new Versh(positionX, positionY);
                            versh.Add(second);
                        }

                        // ПРОВЕРКА, НЕ НУЛЕВЫЕ ЛИ ВЕРШИНЫ!

                        Rib H = new Rib( first, second, dist);
                        ribs.Add(H);
                    }
                }
            }
            // ???????? Сработает ли
            // сортировка по дистанции (цене ребра)
            ribs.Sort((first, second) => first._dist.CompareTo(second._dist));

        }

        public void Segment()
        {
            int kolSegm = 0;
            //List<int,int> maxSize  = new List<int,int>();
            //Tuple<int, int>[] maxSize = new Tuple<int, int>[_height*_width];
            //List<List<int>> maxSize = new List<List<int>>();
            int[,] maxSize = new int[ _height*_width, 2];
            foreach (Rib r in ribs)
            {
                //if (r._dist > )
                if (r._firstV._family == r._secondV._family)
                {
                    continue;
                }
                else
                {
                    if (r._firstV._family == 0 || r._secondV._family == 0)
                    {


                    }
                    else
                    {


                        // ?????
                        var min = Math.Min(maxSize[r._firstV._family, 0] + (limit/maxSize[r._firstV._family, 1]),
                            maxSize[r._secondV._family, 0] + (limit/maxSize[r._secondV._family, 1]));
                        if (r._dist < min)
                        {

                        }
                    }
                   
                }
            }
        }
    }
}
    

