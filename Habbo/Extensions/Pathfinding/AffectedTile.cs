using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zazlak.Habbo.Extensions.Pathfinding
{
    public class AffectedTile
    {
        int mX;
        int mY;
        int mI;

        public AffectedTile(int x, int y, int i)
        {
            mX = x;
            mY = y;
            mI = i;
        }

        public int X
        {
            get
            {
                return mX;
            }
        }

        public int Y
        {
            get
            {
                return mY;
            }
        }

        public int I
        {
            get
            {
                return mI;
            }
        }
    }
}