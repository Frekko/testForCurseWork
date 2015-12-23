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
        static int[,] rebr = { { 0, 1 }, { 0, 2 }, { 0, 3 }, { 0, -1 }, { 0, -2 }, { 0, -3 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { -1, 0 }, { -2, 0 }, { -3, 0 } };
        public readonly int _x;
        public readonly int _y;
        public readonly byte _r;
        public readonly byte _g;
        public readonly byte _b;
        public List<Rib> ribs; 
        public List<Versh> vershList;
        private int _count;
        public int VershCount
        {
            get { return this.GetRoot()._count; }
            set { this.GetRoot()._count = value; }
        }
        private double _maxDist;
        public double MaxDist
        {
            get
            {
                Versh root = this.GetRoot();
                return root._maxDist;
            }
            set
            {
                Versh root = this.GetRoot();
                root._maxDist = value;
            }
        }
        public List<Versh> Children;
        public Versh Parent;

        public Versh(int x1, int y1, byte r, byte g, byte b)
        {
            _count = 1;
            _x = x1;
            _y = y1;
            _r = r;
            _g = g;
            _b = b;
            Children = new List<Versh>();
            Parent = null;
            MaxDist = 0;
            vershList = new List<Versh> { this };
            ribs = new List<Rib>(12);
        }

        public void MergeSegment(Versh v)
        {
            Versh root = this.GetRoot();
            v = v.GetRoot();
            //проверяем, если уже один сегмент
            if (root == v)
            {
                return;
            }
            var vershList1 = root.vershList;
            var vershList2 = v.vershList;
            foreach (Versh v1 in vershList1)
            {
                foreach (Versh v2 in vershList2)
                {
                        double dist = GetRibDist(v1, v2);
                        if (dist > MaxDist)
                            root.MaxDist = dist;
                }
            }

            this.VershCount += v.VershCount;

            root.vershList.AddRange(v.vershList);
            v.vershList = null;

            root.Children.Add(v);
            v.Parent = root;
        }

        public Versh GetRoot()
        {
            Versh root = this;
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            return root;
        }

        public static double GetRibDist(Versh v1, Versh v2)
        {
            foreach (Rib rib in v1.ribs)
            {
                if (rib._firstV == v2 || rib._secondV == v2)
                {
                    return rib._dist;
                }
            }
            return -1;
        }

        private List<Versh> GetSubTree()
        {
            List<Versh> result = new List<Versh>();
            foreach (Versh child in Children)
            {
                result.AddRange(child.GetSubTree());
            }
            result.Add(this);
            return result;
        }
    }
}
