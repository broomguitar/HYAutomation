using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HYAutomation.BaseView.CustomControls
{
    public class CoverBorder : Border
    {
        public double CoverSize
        {
            get { return (double)GetValue(CoverSizeProperty); }
            set { SetValue(CoverSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoverSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoverSizeProperty =
            DependencyProperty.Register("CoverSize", typeof(double), typeof(CoverBorder), new PropertyMetadata(15d));


        public CoverPlacementEnum CoverPlacement
        {
            get { return (CoverPlacementEnum)GetValue(CoverPlacementProperty); }
            set { SetValue(CoverPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoverPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoverPlacementProperty =
            DependencyProperty.Register("CoverPlacement", typeof(CoverPlacementEnum), typeof(CoverBorder), new PropertyMetadata(CoverPlacementEnum.ALL));



        private double CoverThickness = 7d;

        public Brush CoverStroke
        {
            get { return (Brush)GetValue(CoverStrokeProperty); }
            set { SetValue(CoverStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoverStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoverStrokeProperty =
            DependencyProperty.Register("CoverStroke", typeof(Brush), typeof(CoverBorder), new PropertyMetadata(Brushes.Green));

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (CoverPlacement == CoverPlacementEnum.ALL)
            {
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(CoverThickness / 2, CoverThickness / 2), EnumPlacement.LeftTop));
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(CoverThickness / 2, RenderSize.Height - CoverThickness / 2), EnumPlacement.LeftBottom));
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(RenderSize.Width - CoverThickness / 2, CoverThickness / 2), EnumPlacement.RightTop));
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(RenderSize.Width - CoverThickness / 2, RenderSize.Height - CoverThickness / 2), EnumPlacement.RightBottom));
            }
            else if (CoverPlacement == CoverPlacementEnum.Left)
            {
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(CoverThickness / 2, CoverThickness / 2), EnumPlacement.LeftTop));
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(CoverThickness / 2, RenderSize.Height - CoverThickness / 2), EnumPlacement.LeftBottom));
            }
            else if (CoverPlacement == CoverPlacementEnum.Top)
            {
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(CoverThickness / 2, CoverThickness / 2), EnumPlacement.LeftTop));
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(RenderSize.Width - CoverThickness / 2, CoverThickness / 2), EnumPlacement.RightTop));
            }
            else if (CoverPlacement == CoverPlacementEnum.Right)
            {
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(RenderSize.Width - CoverThickness / 2, CoverThickness / 2), EnumPlacement.RightTop));
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(RenderSize.Width - CoverThickness / 2, RenderSize.Height - CoverThickness / 2), EnumPlacement.RightBottom));
            }
            else if (CoverPlacement == CoverPlacementEnum.Bottom)
            {
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(CoverThickness / 2, RenderSize.Height - CoverThickness / 2), EnumPlacement.LeftBottom));
                dc.DrawGeometry(CoverStroke, new Pen(CoverStroke, CoverThickness), BuildCover(new Point(RenderSize.Width - CoverThickness / 2, RenderSize.Height - CoverThickness / 2), EnumPlacement.RightBottom));
            }
        }
        StreamGeometry BuildCover(Point p, EnumPlacement enumPlacement)
        {
            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                switch (enumPlacement)
                {
                    case EnumPlacement.LeftTop:
                        ctx.BeginFigure(new Point(p.X, p.Y + CoverSize), false, false);
                        ctx.LineTo(new Point(p.X, p.Y), true, false);
                        ctx.LineTo(new Point(p.X + CoverSize, p.Y), true, false);
                        break;
                    case EnumPlacement.LeftBottom:
                        ctx.BeginFigure(new Point(p.X + CoverSize, p.Y), false, false);
                        ctx.LineTo(new Point(p.X, p.Y), true, false);
                        ctx.LineTo(new Point(p.X, p.Y - CoverSize), true, false);
                        break;
                    case EnumPlacement.RightTop:
                        ctx.BeginFigure(new Point(p.X - CoverSize, p.Y), false, false);
                        ctx.LineTo(new Point(p.X, p.Y), true, false);
                        ctx.LineTo(new Point(p.X, p.Y + CoverSize), true, false);
                        break;
                    case EnumPlacement.RightBottom:
                        ctx.BeginFigure(new Point(p.X, p.Y - CoverSize), false, false);
                        ctx.LineTo(new Point(p.X, p.Y), true, false);
                        ctx.LineTo(new Point(p.X - CoverSize, p.Y), true, false);
                        break;
                    default:
                        break;
                }
            }
            geometry.FillRule = FillRule.EvenOdd;
            geometry.Freeze();
            return geometry;
        }
        enum EnumPlacement
        {
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom
        }
    }
    public enum CoverPlacementEnum
    {
        ALL,
        Left,
        Top,
        Right,
        Bottom
    }
}
