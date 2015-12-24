using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace test2
{
    public class Segmentation
    {
        private int _width;
        private int _height;
        private byte[] _foto;
        public double limit = 10;
        List<Versh> versh;
        public static List<Rib> ribs;
        public static Versh[,] v2d;

        public Segmentation(Bitmap foto2D)
        {
            _width = foto2D.Width;
            _height = foto2D.Height;
            versh = new List<Versh>(_width * _height);
            ribs = new List<Rib>(_width * _height * 6);
            _foto = Filters.GetBytes(foto2D);
        }

        public void SortRebr()
        {
            int length = _foto.Length/3;

            var r = new byte[_height, _width];
            var g = new byte[_height, _width];
            var b = new byte[_height, _width];

            int index = 0;
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    r[i, j] = _foto[index++];
                    g[i, j] = _foto[index++];
                    b[i, j] = _foto[index++];
                }
            }
            v2d = new Versh[_height, _width];
           //Versh[,] v2d = new Versh[_height, _width];
            for (int x = 0; x < _height; x++)
            {
                for (int y = 0; y < _width; y++)
                {
                    v2d[x,y] = new Versh(x,y, r[x,y], g[x,y], b[x,y]);
                }
            }
            int[,] rebr = {{0, 1}, {0, 2}, {0, 3}, {1, 0}, {2, 0}, {3, 0}};
            //Array[] arr = Array.CreateInstance()
            // поставить проверку кратности трём массива байтов!
            for (int x = 0, limitX = _height-3; x < limitX; x++)
            {
                for (int y = 0, limitY = _width-3; y < limitY; y++)
                {
                    
                    for (int i = 0; i < 6; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        //тут мы знаем, что следующий пиксель существует, потому что границы в цикле ширина-3, высота-3

                        CreateRib(x, y, positionX, positionY, r, g, b, v2d);
                        // ПРОВЕРКА, НЕ НУЛЕВЫЕ ЛИ ВЕРШИНЫ!
                    }
                }
                for (int y = _width - 3; y < _width; y++)
                {

                    for (int i = 0; i < 6; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        // если следующий пиксел не существует
                        if ((positionX >= _height) || (positionY >= _width))
                            continue;

                        CreateRib(x,y,positionX,positionY, r, g, b, v2d);
                        // ПРОВЕРКА, НЕ НУЛЕВЫЕ ЛИ ВЕРШИНЫ!
                    }
                }
            }
            for (int x = _height - 3; x < _height; x++)
            {
                for (int y = 0; y < _width; y++)
                {

                    for (int i = 0; i < 6; i++)
                    {
                        int positionX = x + rebr[i, 0];
                        int positionY = y + rebr[i, 1];

                        // если следующий пиксел не существует
                        if ((positionX >= _height) || (positionY >= _width))
                            continue;

                        CreateRib(x, y, positionX, positionY, r, g, b, v2d);
                        // ПРОВЕРКА, НЕ НУЛЕВЫЕ ЛИ ВЕРШИНЫ!
                    }
                }
            }
            // ???????? Сработает ли
            // сортировка по дистанции (цене ребра)
            ribs.Sort((first, second) => first._dist.CompareTo(second._dist));
            foreach (Versh versh1 in v2d)
            {
                versh.Add(versh1);
            }
            //v2d = null;
        }

        private void CreateRib(int x1, int y1, int x2, int y2, byte[,] r, byte[,] g, byte[,] b, Versh[,] v2d)
        {
            double dist = Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2) +
                                         Math.Pow((r[x1, y1] - r[x2, y2]), 2) +
                                         Math.Pow((g[x1, y1] - g[x2, y2]), 2) +
                                         Math.Pow((b[x1, y1] - b[x2, y2]), 2));

            Rib rib = new Rib(v2d[x1, y1], v2d[x2, y2], dist);
            ribs.Add(rib);
//            v2d[x1, y1].ribs.Add(rib);
//            v2d[x2, y2].ribs.Add(rib);
        }

        public void Segment()
        {
            int kolSegm = 0;
            //List<int,int> maxSize  = new List<int,int>();
            //Tuple<int, int>[] maxSize = new Tuple<int, int>[_height*_width];
            //List<List<int>> maxSize = new List<List<int>>();
            int[,] maxSize = new int[ _height*_width, 2];
            for (int k = 0; k < ribs.Count; k++)
            {
                Rib r = ribs[k];
                if (r._dist > limit)
                    break;
                //если у них один корень, то они уже в одном сегменте - тогда их не надо соединять снова
                if (r._firstV.Root == r._secondV.Root)
                    continue;
                //если же они в разных сегментах, то мы должны оценить,
                //должны ли мы соединять сегменты
                //одинокий пиксель - сегмент из одной вершины
                double m1, m2;
                //m1 = r._firstV.MaxDist;
                //m2 = r._secondV.MaxDist;
                if (r._dist < limit)
                    r._firstV.MergeSegment(r._secondV);
//                m1 = limit / r._firstV.VershCount;
//                m2 = limit / r._secondV.VershCount;
//                if (m1 == 0 || m2 == 0)
//                {
//                    if (r._dist < limit)
//                        r._firstV.MergeSegment(r._secondV);
//                }
//                else
//                {
//                   // m1 += limit/r._firstV.VershCount;
//                   // m2 += limit/r._secondV.VershCount;
//                    double m = Math.Min(m1, m2);
//                    if (r._dist < m)
//                    {
//                        r._firstV.MergeSegment(r._secondV);
//                    }
//                }
            }
            ///////////////////////////////////////////////////////////////////////////////
            /// тут всё правильно
            byte[] mimimi = new byte[_foto.Length];
            int index = 0;
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    mimimi[index++] = v2d[i, j].Root._r;
                    mimimi[index++] = v2d[i, j].Root._g;
                    mimimi[index++] = v2d[i, j].Root._b;
                }
            }
//            Bitmap zyu = test2.Filters.GetBitmap(mimimi, _width, _height);
//            Bitmap zyuzyu = Filters.GetBitmap(_foto, _width, _height);
//            Application.Run(new Form1(zyuzyu, zyu));
        }
    }
}
    

