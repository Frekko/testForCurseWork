using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace test2
{
    public class Versh
    {
        private static int[,] rebr =
        {
            {0, 1}, {0, 2}, {0, 3}, {0, -1}, {0, -2}, {0, -3}, {1, 0}, {2, 0}, {3, 0}, {-1, 0},
            {-2, 0}, {-3, 0}
        };

        public readonly int _x;
        public readonly int _y;
        public readonly byte _r;
        public readonly byte _g;
        public readonly byte _b;
        private int _count;
        private Versh _parent;
        private double _maxDist;

        public Versh Root
        {
            get
            {
                //если _parent==null, то мы уже в корне
                if (_parent == null)
                    return this;
                //пока _parent - не корень, двигаем его вверх по дереву
                while (_parent._parent != null)
                    _parent = _parent._parent;
                return _parent;
            }
            set
            {
                Versh root = this.Root;
                root._parent = value.Root;
            }
        }

//        public List<Versh> vershList;

        public int VershCount
        {
            get { return Root._count; }
            set { Root._count = value; }
        }


        public double MaxDist
        {
            get { return Root._maxDist; }
            set { Root._maxDist = value; }
        }


        public Versh(int x1, int y1, byte r, byte g, byte b)
        {
            _count = 1;
            _x = x1;
            _y = y1;
            _r = r;
            _g = g;
            _b = b;
            _parent = null;
            _maxDist = 0;
        }

        public void MergeSegment(Versh v)
        {
            //проверяем, если уже один сегмент
            if (this.Root == v.Root)
            {
                return;
            }
            VershCount += v.VershCount;

            v.Root = this.Root;
        }

    }
}
