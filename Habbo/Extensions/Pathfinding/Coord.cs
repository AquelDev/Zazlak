using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zazlak.Habbo.Extensions.Pathfinding
{
    public struct Coord
    {
        internal int X;
        internal int Y;

        internal Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal bool IsAround(int x, int y, int ex, int ey)
        {
            if (x-- == ex && y-- == ey)
                return true;
            if (x == ex && y-- == ey)
                return true;
            if (x++ == ex && y-- == ey)
                return true;
            if (x++ == ex && y == ey)
                return true;
            if (x++ == ex && y++ == ey)
                return true;
            if (x == ex && y++ == ey)
                return true;
            if (x-- == ex && y == ey)
                return true;
            return false;
        }

        public static bool operator ==(Coord a, Coord b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Coord a, Coord b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override bool Equals(object obj)
        {
            return base.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}
