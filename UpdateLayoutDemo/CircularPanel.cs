using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace UpdateLayoutDemo
{
    public class CircularPanel : Panel
    {
        private static readonly double DefaultArcStart = -Math.PI / 2;

        public static readonly DependencyProperty ArcStartProperty = DependencyProperty.Register(
            "ArcStart",
            typeof(double),
            typeof(CircularPanel),
            new PropertyMetadata(DefaultArcStart, HandleArcStartChanged)
            );

        public static readonly DependencyProperty ArcEndProperty = DependencyProperty.Register(
            "ArcEnd",
            typeof(double),
            typeof(CircularPanel),
            new PropertyMetadata(2 * Math.PI + DefaultArcStart, HandleArcEndChanged)
            );

        public static readonly DependencyProperty ArcEndIncludedProperty = DependencyProperty.Register(
            "ArcEndIncluded",
            typeof(bool),
            typeof(CircularPanel),
            new PropertyMetadata(false, HandleArcEndIncludedChanged)
            );

        private static void HandleArcStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var circularPanel = (CircularPanel)d;
            if (circularPanel.UseUpdateLayout)
            {
                circularPanel.UpdateLayout();
            }
            else
            {
                circularPanel.InvalidateMeasure();
                circularPanel.InvalidateArrange();
            }
        }

        private static void HandleArcEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var circularPanel = (CircularPanel)d;
            if (circularPanel.UseUpdateLayout)
            {
                circularPanel.UpdateLayout();
            }
            else
            {
                circularPanel.InvalidateMeasure();
                circularPanel.InvalidateArrange();
            }
        }

        private static void HandleArcEndIncludedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var circularPanel = (CircularPanel)d;
            if (circularPanel.UseUpdateLayout)
            {
                circularPanel.UpdateLayout();
            }
            else
            {
                circularPanel.InvalidateMeasure();
                circularPanel.InvalidateArrange();
            }
        }

        public bool UseUpdateLayout { get; set; } = true;

        public double ArcStart
        {
            get { return (double)GetValue(ArcStartProperty); }
            set { SetValue(ArcStartProperty, value); }
        }

        public double ArcEnd
        {
            get { return (double)GetValue(ArcEndProperty); }
            set { SetValue(ArcEndProperty, value); }
        }

        public bool ArcEndIncluded
        {
            get { return (bool)GetValue(ArcEndIncludedProperty); }
            set { SetValue(ArcEndIncludedProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Children.Count == 0)
            {
                return new Size(0, 0);
            }

            if (this.Children.Count == 1)
            {
                var child = this.Children[0];
                child.Measure(availableSize);
                return EnforcedSize(child.DesiredSize.Width, child.DesiredSize.Height);
            }

            var arc = Math.Abs(this.ArcEnd - this.ArcStart);
            var halfArc = arc / 2;

            var width = double.IsNaN(this.Width) ? availableSize.Width : this.Width;
            var height = double.IsNaN(this.Height) ? availableSize.Height : this.Height;

            var W = Math.Min(width, height);

            var n = Children.Count;
            var N = this.ArcEndIncluded ? n + 1 : n;

            var k = Math.Sin(halfArc / N);
            var q = k / (2 * (1 + k));
            var r = W * q;

            var childWidth = 2 * r;
            var childHeight = 2 * r;

            double maxChildWidth = 0;
            double maxChildHeight = 0;
            foreach (var child in this.Children)
            {
                child.Measure(new Size(childWidth, childHeight));
                maxChildWidth = Math.Max(maxChildWidth, child.DesiredSize.Width);
                maxChildHeight = Math.Max(maxChildHeight, child.DesiredSize.Height);
            }

            r = Math.Max(maxChildWidth, maxChildHeight) / 2;
            var R = r / k;
            var desiredMeassure = 2 * (R + r);

            return EnforcedSize(desiredMeassure, desiredMeassure);
        }

        private Size AvailableItemSize(Size thisSize)
        {
            var arc = Math.Abs(this.ArcEnd - this.ArcStart);
            var halfArc = arc / 2;

            var W = Math.Min(thisSize.Width, thisSize.Height);

            var n = Children.Count;
            var N = this.ArcEndIncluded ? n + 1 : n;

            var k = Math.Sin(halfArc / N);
            var q = k / (2 * (1 + k));
            var r = W * q;

            return new Size(r * 2, r * 2);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var itemSize = AvailableItemSize(finalSize);

            var width = finalSize.Width - itemSize.Width;
            var height = finalSize.Height - itemSize.Height;

            var measure = Math.Min(width, height);
            var r = measure / 2;

            var arc = this.ArcEnd - this.ArcStart;
            var segments = this.ArcEndIncluded
                ? this.Children.Count - 1
                : this.Children.Count;

            var segmentAngle = arc / segments;

            int index = 0;
            foreach (var child in this.Children)
            {
                var angle = this.ArcStart + (segmentAngle * index);
                var (sin, cos) = Math.SinCos(angle);
                // Relative to centre:
                var x = r * cos;
                var y = r * sin;

                // Translated co-ordinates to top-left of container:
                x += (finalSize.Width / 2);
                y += (finalSize.Height / 2);

                // Translated co-ordinates to top-left of child:
                x -= (child.DesiredSize.Width / 2);
                y -= (child.DesiredSize.Height / 2);

                Point anchorPoint = new Point(x, y);
                child.Arrange(new Rect(anchorPoint, child.DesiredSize));

                index++;
            }

            return base.ArrangeOverride(finalSize);
        }

        private Size EnforcedSize(double width, double height)
        {
            width = double.IsNaN(this.Width) ? width : this.Width;
            height = double.IsNaN(this.Height) ? height : this.Height;
            return new Size(width, height);
        }
    }
}
