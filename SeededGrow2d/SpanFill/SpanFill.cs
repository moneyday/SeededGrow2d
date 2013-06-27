﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SeededGrow2d
{
    public enum ParentDirections
    {
        Y0 = 1, Y2 = 2, Non = 5
    }
    public enum ExtendTypes
    {
        LeftRequired = 1, RightRequired = 2, AllRez = 3, UnRez = 4
    }
    public struct Span
    {
        public int XLeft;
        public int XRight;
        public int Y;
        public ExtendTypes Extended;
        public ParentDirections ParentDirection;
    }
    public class SpanFill2d
    {
        protected int count = 0;
        protected BitMap2d bmp;
        public FlagMap2d flagsMap;
        protected Container<Span> queue;
        public virtual void ExcuteSpanFill_Q(BitMap2d data, Int16Double seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap2d(data.width, data.height);
            queue = new Container_Queue<Span>();
            Process(seed);
            flagsMap.SetFlagOn(seed.X, seed.Y, true);
            Span seedspan = new Span();
            seedspan.XLeft = seed.X;
            seedspan.XRight = seed.X;
            seedspan.Y = seed.Y;
            seedspan.ParentDirection = ParentDirections.Non;
            seedspan.Extended = ExtendTypes.UnRez;
            queue.Push(seedspan);

            while (!queue.Empty())
            {
                Span span = queue.Pop();
                #region AllRez
                if (span.Extended == ExtendTypes.AllRez)
                {
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, ParentDirections.Y2);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, ParentDirections.Y0);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region UnRez
                if (span.Extended == ExtendTypes.UnRez)
                {
                    int xl = FindXLeft(span.XLeft, span.Y);
                    int xr = FindXRight(span.XRight, span.Y);
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y + 1, ParentDirections.Y0);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y + 1, ParentDirections.Y0);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, xr, span.Y + 1, ParentDirections.Y0);
                        if (span.Y - 1 >= 0)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y - 1, ParentDirections.Y2);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y - 1, ParentDirections.Y2);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Non)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, xr, span.Y + 1, ParentDirections.Y0);
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, ParentDirections.Y2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region LeftRequired
                if (span.Extended == ExtendTypes.LeftRequired)
                {
                    int xl = FindXLeft(span.XLeft, span.Y);
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y + 1, ParentDirections.Y0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, span.XRight, span.Y + 1, ParentDirections.Y0);
                        if (span.Y - 1 >= 0 && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y - 1, ParentDirections.Y2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region RightRequired
                if (span.Extended == ExtendTypes.RightRequired)
                {
                    int xr = FindXRight(span.XRight, span.Y);

                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y + 1, ParentDirections.Y0);
                        continue;
                    }

                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, ParentDirections.Y0);
                        if (span.Y - 1 >= 0 && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y - 1, ParentDirections.Y2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
            }
        }
        public virtual void ExcuteSpanFill_S(BitMap2d data, Int16Double seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap2d(data.width, data.height);
            queue = new Container_Stack<Span>();
            Process(seed);
            flagsMap.SetFlagOn(seed.X, seed.Y, true);
            Span seedspan = new Span();
            seedspan.XLeft = seed.X;
            seedspan.XRight = seed.X;
            seedspan.Y = seed.Y;
            seedspan.ParentDirection = ParentDirections.Non;
            seedspan.Extended = ExtendTypes.UnRez;
            queue.Push(seedspan);

            while (!queue.Empty())
            {
                Span span = queue.Pop();
                #region AllRez
                if (span.Extended == ExtendTypes.AllRez)
                {
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, ParentDirections.Y2);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, ParentDirections.Y0);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region UnRez
                if (span.Extended == ExtendTypes.UnRez)
                {
                    int xl = FindXLeft(span.XLeft, span.Y);
                    int xr = FindXRight(span.XRight, span.Y);
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y + 1, ParentDirections.Y0);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y + 1, ParentDirections.Y0);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, xr, span.Y + 1, ParentDirections.Y0);
                        if (span.Y - 1 >= 0)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y - 1, ParentDirections.Y2);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y - 1, ParentDirections.Y2);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Non)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, xr, span.Y + 1, ParentDirections.Y0);
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, ParentDirections.Y2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region LeftRequired
                if (span.Extended == ExtendTypes.LeftRequired)
                {
                    int xl = FindXLeft(span.XLeft, span.Y);
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y + 1, ParentDirections.Y0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, span.XRight, span.Y + 1, ParentDirections.Y0);
                        if (span.Y - 1 >= 0 && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y - 1, ParentDirections.Y2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region RightRequired
                if (span.Extended == ExtendTypes.RightRequired)
                {
                    int xr = FindXRight(span.XRight, span.Y);

                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y + 1, ParentDirections.Y0);
                        continue;
                    }

                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, ParentDirections.Y0);
                        if (span.Y - 1 >= 0 && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y - 1, ParentDirections.Y2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
            }
        }
        private void CheckRange(int xleft, int xright, int y, ParentDirections ptype)
        {
            for (int i = xleft; i <= xright; )
            {
                if ((!flagsMap.GetFlagOn(i, y)) && IncludePredicate(i, y))
                {
                    int lb = i;
                    int rb = i + 1;
                    while (rb <= xright && (!flagsMap.GetFlagOn(rb, y)) && IncludePredicate(rb, y))
                    {
                        rb++;
                    }
                    rb--;

                    Span span = new Span();
                    span.XLeft = lb;
                    span.XRight = rb;
                    span.Y = y;
                    if (lb == xleft && rb == xright)
                    {
                        span.Extended = ExtendTypes.UnRez;
                    }
                    else if (rb == xright)
                    {
                        span.Extended = ExtendTypes.RightRequired;
                    }
                    else if (lb == xleft)
                    {
                        span.Extended = ExtendTypes.LeftRequired;
                    }
                    else
                    {
                        span.Extended = ExtendTypes.AllRez;
                    }
                    span.ParentDirection = ptype;
                    for (int j = lb; j <= rb; j++)
                    {
                        flagsMap.SetFlagOn(j, y, true);
                        Process(new Int16Double(j, y));
                    }
                    queue.Push(span);

                    i = rb + 1;
                }
                else
                {
                    i++;
                }
            }
        }
        private int FindXRight(int x, int y)
        {
            int xright = x + 1;
            while (true)
            {
                if (xright == bmp.width || flagsMap.GetFlagOn(xright, y))
                {
                    break;
                }
                else
                {
                    if (IncludePredicate(xright, y))
                    {
                        Int16Double t = new Int16Double(xright, y);
                        flagsMap.SetFlagOn(xright, y, true);
                        Process(t);
                        xright++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return xright - 1;
        }
        private int FindXLeft(int x, int y)
        {
            int xleft = x - 1;
            while (true)
            {
                if (xleft == -1 || flagsMap.GetFlagOn(xleft, y))
                {
                    break;
                }
                else
                {
                    if (IncludePredicate(xleft, y))
                    {
                        Int16Double t = new Int16Double(xleft, y);
                        flagsMap.SetFlagOn(xleft, y, true);
                        Process(t);
                        xleft--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return xleft + 1;
        }
        bool IncludePredicate(int x, int y)
        {
            byte value = bmp.GetPixel(x, y);
            return value == 255;
        }
        public void Process(Int16Double p)
        {
            count++;
        }
    }
    class SpanFill2d_T : SpanFill2d
    {
        public Stopwatch watch = new Stopwatch();
        public ResultReport report;
        public override void ExcuteSpanFill_Q(BitMap2d data, Int16Double seed)
        {
            watch.Start();
            base.ExcuteSpanFill_Q(data, seed);
            watch.Stop();
            report.time = watch.ElapsedMilliseconds;
            report.result_point_count = count;
            report.bmp_get_count = data.action_get_count;
            report.bmp_set_count = data.action_set_count;
            report.container_pop_count = queue.action_pop_count;
            report.container_push_count = queue.action_push_count;
            report.container_max_size = queue.max_contain_count;
            report.flag_get_count = flagsMap.action_get_count;
            report.flag_set_count = flagsMap.action_set_count;
            return;
        }
        public override void ExcuteSpanFill_S(BitMap2d data, Int16Double seed)
        {
            watch.Start();
            base.ExcuteSpanFill_S(data, seed);
            watch.Stop();
            report.time = watch.ElapsedMilliseconds;
            report.result_point_count = count;
            report.bmp_get_count = data.action_get_count;
            report.bmp_set_count = data.action_set_count;
            report.container_pop_count = queue.action_pop_count;
            report.container_push_count = queue.action_push_count;
            report.container_max_size = queue.max_contain_count;
            report.flag_get_count = flagsMap.action_get_count;
            report.flag_set_count = flagsMap.action_set_count;
            return;
        }
    }
}
