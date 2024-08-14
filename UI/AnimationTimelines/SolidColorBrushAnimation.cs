using System;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;

namespace FluentUI
{
    internal sealed class SolidColorBrushAnimation : AnimationTimeline
    {


        public override Type TargetPropertyType => typeof(SolidColorBrush);

        public SolidColorBrush From
        {
            get => (SolidColorBrush)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof(From), typeof(SolidColorBrush), typeof(SolidColorBrushAnimation));

        public SolidColorBrush To
        {
            get => (SolidColorBrush)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof(To), typeof(SolidColorBrush), typeof(SolidColorBrushAnimation));

        protected override Freezable CreateInstanceCore()
        {
            return new SolidColorBrushAnimation();
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private const Double MAGIC_NUMBER = 6755399441055744.0d;

        public override Object GetCurrentValue(Object defaultOriginValue, Object defaultDestinationValue, AnimationClock animationClock)
        {
            Double currentProgress = animationClock.CurrentProgress.GetValueOrDefault();
            Double currentProgress_Inverted = (currentProgress - 1) * -1;

            Byte alpha = (Byte)((From.Color.A > To.Color.A ? ((From.Color.A - To.Color.A) * currentProgress_Inverted) + To.Color.A : ((To.Color.A - From.Color.A) * currentProgress) + From.Color.A) + MAGIC_NUMBER - MAGIC_NUMBER);
            Byte red = (Byte)((From.Color.R > To.Color.R ? ((From.Color.R - To.Color.R) * currentProgress_Inverted) + To.Color.R : ((To.Color.R - From.Color.R) * currentProgress) + From.Color.R) + MAGIC_NUMBER - MAGIC_NUMBER);
            Byte green = (Byte)((From.Color.G > To.Color.G ? ((From.Color.G - To.Color.G) * currentProgress_Inverted) + To.Color.G : ((To.Color.G - From.Color.G) * currentProgress) + From.Color.G) + MAGIC_NUMBER - MAGIC_NUMBER);
            Byte blue = (Byte)((From.Color.B > To.Color.B ? ((From.Color.B - To.Color.B) * currentProgress_Inverted) + To.Color.B : ((To.Color.B - From.Color.B) * currentProgress) + From.Color.B) + MAGIC_NUMBER - MAGIC_NUMBER);

            return new SolidColorBrush(Color.FromArgb(alpha, red, green, blue));
        }
    }
}
