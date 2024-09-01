using System;
using System.Windows.Media;

#pragma warning disable IDE0079
#pragma warning disable CS8618

namespace FluentUI
{
    internal static class AccentColors
    {
        internal static class DarkMode
        {
            internal static readonly SolidColorBrush FocusVisualAsBrush = Brushes.White;

            internal static readonly Byte[] Idle = [AccentPalette[4], AccentPalette[5], AccentPalette[6]];
            internal static SolidColorBrush IdleAsBrush = new(Color.FromRgb(Idle[0], Idle[1], Idle[2]));
            internal static readonly Byte[] IdleBorder = [0x5a, 0xc7, 0xff];
            internal static SolidColorBrush IdleBorderAsBrush = new(Color.FromRgb(IdleBorder[0], IdleBorder[1], IdleBorder[2]));
            internal static readonly Byte[] IdleBorderBottom = [0x42, 0xa7, 0xdc];
            internal static SolidColorBrush IdleBorderBottomAsBrush = new(Color.FromRgb(IdleBorderBottom[0], IdleBorderBottom[1], IdleBorderBottom[2]));

            internal static readonly Byte[] MouseOverBorder = [0x55, 0xb7, 0xea];
            internal static SolidColorBrush MouseOverBorderAsBrush = new(Color.FromRgb(MouseOverBorder[0], MouseOverBorder[1], MouseOverBorder[2]));
            internal static readonly Byte[] MouseOverBorderBottom = [0x3d, 0x99, 0xc8];
            internal static SolidColorBrush MouseOverBorderBottomAsBrush = new(Color.FromRgb(MouseOverBorderBottom[0], MouseOverBorderBottom[1], MouseOverBorderBottom[2]));
            internal static readonly Byte[] MouseOverBackground = [0x47, 0xb1, 0xe8];
            internal static SolidColorBrush MouseOverBackgroundAsBrush = new(Color.FromRgb(MouseOverBackground[0], MouseOverBackground[1], MouseOverBackground[2]));

            internal static readonly Byte[] MouseDown = [0x42, 0xa1, 0xd2];
            internal static SolidColorBrush MouseDownAsBrush = new(Color.FromRgb(MouseDown[0], MouseDown[1], MouseDown[2]));
            internal static readonly Byte[] MouseDownBorder = [0x47, 0xb0, 0xe6];
            internal static SolidColorBrush MouseDownBorderAsBrush = new(Color.FromRgb(MouseDownBorder[0], MouseDownBorder[1], MouseDownBorder[2]));
        }

        internal static class LightMode
        {
            internal static readonly SolidColorBrush FocusVisualAsBrush = new(Color.FromRgb(0x1a, 0x1a, 0x1a));

            internal static readonly Byte[] Idle = [AccentPalette[16], AccentPalette[17], AccentPalette[18]];
            internal static SolidColorBrush IdleAsBrush = new(Color.FromRgb(Idle[0], Idle[1], Idle[2]));
            internal static readonly Byte[] IdleBorder = [0x14, 0x73, 0xc5];
            internal static SolidColorBrush IdleBorderAsBrush = new(Color.FromRgb(IdleBorder[0], IdleBorder[1], IdleBorder[2]));
            internal static readonly Byte[] IdleBorderBottom = [0x00, 0x3e, 0x73];
            internal static SolidColorBrush IdleBorderBottomAsBrush = new(Color.FromRgb(IdleBorderBottom[0], IdleBorderBottom[1], IdleBorderBottom[2]));

            internal static readonly Byte[] MouseOverBorder = [0x2b, 0x80, 0xca];
            internal static SolidColorBrush MouseOverBorderAsBrush = new(Color.FromRgb(MouseOverBorder[0], MouseOverBorder[1], MouseOverBorder[2]));
            internal static readonly Byte[] MouseOverBorderBottom = [0x0f, 0x46, 0x76];
            internal static SolidColorBrush MouseOverBorderBottomAsBrush = new(Color.FromRgb(MouseOverBorderBottom[0], MouseOverBorderBottom[1], MouseOverBorderBottom[2]));
            internal static readonly Byte[] MouseOverBackground = [0x19, 0x75, 0xc5];
            internal static SolidColorBrush MouseOverBackgroundAsBrush = new(Color.FromRgb(MouseOverBackground[0], MouseOverBackground[1], MouseOverBackground[2]));

            internal static readonly Byte[] MouseDown = [0x31, 0x83, 0xca];
            internal static SolidColorBrush MouseDownAsBrush = new(Color.FromRgb(MouseDown[0], MouseDown[1], MouseDown[2]));
            internal static readonly Byte[] MouseDownBorder = [0x1e, 0x78, 0xc6];
            internal static SolidColorBrush MouseDownBorderAsBrush = new(Color.FromRgb(MouseDownBorder[0], MouseDownBorder[1], MouseDownBorder[2]));
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal delegate void AccentHandler();
        internal static event AccentHandler Changed;

        internal static void InvokeChanged() => Changed?.Invoke();

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal static readonly Byte[] AccentPalette = // default windows colors
        [0x99, 0xEB, 0xFF, 0x00, 0x4C, 0xC2, 0xFF, 0x00,
         0x00, 0x91, 0xF8, 0x00, 0x00, 0x78, 0xD4, 0x00,
         0x00, 0x67, 0xC0, 0x00, 0x00, 0x3E, 0x92, 0x00,
         0x00, 0x1A, 0x68, 0x00, 0xF7, 0x63, 0x0C, 0x00];

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        /// <summary>Fast rounding</summary>
        private const Double MAGIC_NUMBER = 6755399441055744.0d;

        internal static void CalculateDerivedColors()
        {
            #region DarkMode
            DarkMode.Idle[0] = AccentPalette[4];
            DarkMode.Idle[1] = AccentPalette[5];
            DarkMode.Idle[2] = AccentPalette[6];
            DarkMode.IdleAsBrush = new(Color.FromRgb(DarkMode.Idle[0], DarkMode.Idle[1], DarkMode.Idle[2]));

            // y = 1.08511x − 21.6879
            DarkMode.IdleBorder[0] = (Byte)(((DarkMode.Idle[0] + 21.6879d) * 0.9215655555657952d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.IdleBorder[1] = (Byte)(((DarkMode.Idle[1] + 21.6879d) * 0.9215655555657952d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.IdleBorder[2] = (Byte)(((DarkMode.Idle[2] + 21.6879d) * 0.9215655555657952d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.IdleBorderAsBrush = new(Color.FromRgb(DarkMode.IdleBorder[0], DarkMode.IdleBorder[1], DarkMode.IdleBorder[2]));

            // y = 1.15909x − 0.166667
            DarkMode.IdleBorderBottom[0] = (Byte)(((DarkMode.Idle[0] + 0.166667d) * 0.8627457747025684d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.IdleBorderBottom[1] = (Byte)(((DarkMode.Idle[1] + 0.166667d) * 0.8627457747025684d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.IdleBorderBottom[2] = (Byte)(((DarkMode.Idle[2] + 0.166667d) * 0.8627457747025684d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.IdleBorderBottomAsBrush = new(Color.FromRgb(DarkMode.IdleBorderBottom[0], DarkMode.IdleBorderBottom[1], DarkMode.IdleBorderBottom[2]));

            // y = 1.20853x − 27.7613
            DarkMode.MouseOverBorder[0] = (Byte)(((DarkMode.Idle[0] + 27.7613d) * 0.8274515320265115d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBorder[1] = (Byte)(((DarkMode.Idle[1] + 27.7613d) * 0.8274515320265115d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBorder[2] = (Byte)(((DarkMode.Idle[2] + 27.7613d) * 0.8274515320265115d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBorderAsBrush = new(Color.FromRgb(DarkMode.MouseOverBorder[0], DarkMode.MouseOverBorder[1], DarkMode.MouseOverBorder[2]));

            // y = 1.29441x − 3.83392
            DarkMode.MouseOverBorderBottom[0] = (Byte)(((DarkMode.Idle[0] + 3.83392d) * 0.7725527460387358d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBorderBottom[1] = (Byte)(((DarkMode.Idle[1] + 3.83392d) * 0.7725527460387358d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBorderBottom[2] = (Byte)(((DarkMode.Idle[2] + 3.83392d) * 0.7725527460387358d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBorderBottomAsBrush = new(Color.FromRgb(DarkMode.MouseOverBorderBottom[0], DarkMode.MouseOverBorderBottom[1], DarkMode.MouseOverBorderBottom[2]));

            // y = 1.11354x − 3.3216
            DarkMode.MouseOverBackground[0] = (Byte)(((DarkMode.Idle[0] + 3.3216d) * 0.8980368913554969d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBackground[1] = (Byte)(((DarkMode.Idle[1] + 3.3216d) * 0.8980368913554969d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBackground[2] = (Byte)(((DarkMode.Idle[2] + 3.3216d) * 0.8980368913554969d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseOverBackgroundAsBrush = new(Color.FromRgb(DarkMode.MouseOverBackground[0], DarkMode.MouseOverBackground[1], DarkMode.MouseOverBackground[2]));

            // y = 1.24998x − 7.24741
            DarkMode.MouseDown[0] = (Byte)(((DarkMode.Idle[0] + 7.24741d) * 0.8000128002048033d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseDown[1] = (Byte)(((DarkMode.Idle[1] + 7.24741d) * 0.8000128002048033d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseDown[2] = (Byte)(((DarkMode.Idle[2] + 7.24741d) * 0.8000128002048033d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseDownAsBrush = new(Color.FromRgb(DarkMode.MouseDown[0], DarkMode.MouseDown[1], DarkMode.MouseDown[2]));

            // y = 1.13329x − 5.26112
            DarkMode.MouseDownBorder[0] = (Byte)(((DarkMode.Idle[0] + 5.26112d) * 0.8823866794906864d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseDownBorder[1] = (Byte)(((DarkMode.Idle[1] + 5.26112d) * 0.8823866794906864d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseDownBorder[2] = (Byte)(((DarkMode.Idle[2] + 5.26112d) * 0.8823866794906864d) + MAGIC_NUMBER - MAGIC_NUMBER);
            DarkMode.MouseDownBorderAsBrush = new(Color.FromRgb(DarkMode.MouseDownBorder[0], DarkMode.MouseDownBorder[1], DarkMode.MouseDownBorder[2]));
            #endregion

            #region LightMode
            LightMode.Idle[0] = AccentPalette[16];
            LightMode.Idle[1] = AccentPalette[17];
            LightMode.Idle[2] = AccentPalette[18];
            LightMode.IdleAsBrush = new(Color.FromRgb(LightMode.Idle[0], LightMode.Idle[1], LightMode.Idle[2]));

            // y = 1.08511x − 21.7021
            LightMode.IdleBorder[0] = (Byte)(((LightMode.Idle[0] + 21.7021d) * 0.9215655555657952) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.IdleBorder[1] = (Byte)(((LightMode.Idle[1] + 21.7021d) * 0.9215655555657952) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.IdleBorder[2] = (Byte)(((LightMode.Idle[2] + 21.7021d) * 0.9215655555657952) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.IdleBorderAsBrush = new(Color.FromRgb(LightMode.IdleBorder[0], LightMode.IdleBorder[1], LightMode.IdleBorder[2]));

            // y = 1.66666666x
            LightMode.IdleBorderBottom[0] = (Byte)((LightMode.Idle[0] * 0.6000000024d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.IdleBorderBottom[1] = (Byte)((LightMode.Idle[1] * 0.6000000024d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.IdleBorderBottom[2] = (Byte)((LightMode.Idle[2] * 0.6000000024d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.IdleBorderBottomAsBrush = new(Color.FromRgb(LightMode.IdleBorderBottom[0], LightMode.IdleBorderBottom[1], LightMode.IdleBorderBottom[2]));

            // y = 1.20853x − 51.9318
            LightMode.MouseOverBorder[0] = (Byte)(((LightMode.Idle[0] + 51.9318d) * 0.8274515320265115) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBorder[1] = (Byte)(((LightMode.Idle[1] + 51.9318d) * 0.8274515320265115) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBorder[2] = (Byte)(((LightMode.Idle[2] + 51.9318d) * 0.8274515320265115) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBorderAsBrush = new(Color.FromRgb(LightMode.MouseOverBorder[0], LightMode.MouseOverBorder[1], LightMode.MouseOverBorder[2]));

            // y = 1.8613x − 27.7749
            LightMode.MouseOverBorderBottom[0] = (Byte)(((LightMode.Idle[0] + 27.7749d) * 0.5372589050663515d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBorderBottom[1] = (Byte)(((LightMode.Idle[1] + 27.7749d) * 0.5372589050663515d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBorderBottom[2] = (Byte)(((LightMode.Idle[2] + 27.7749d) * 0.5372589050663515d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBorderBottomAsBrush = new(Color.FromRgb(LightMode.MouseOverBorderBottom[0], LightMode.MouseOverBorderBottom[1], LightMode.MouseOverBorderBottom[2]));

            // y = 1.11354x − 27.8194
            LightMode.MouseOverBackground[0] = (Byte)(((LightMode.Idle[0] + 27.8194d) * 0.8980368913554969d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBackground[1] = (Byte)(((LightMode.Idle[1] + 27.8194d) * 0.8980368913554969d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBackground[2] = (Byte)(((LightMode.Idle[2] + 27.8194d) * 0.8980368913554969d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseOverBackgroundAsBrush = new(Color.FromRgb(LightMode.MouseOverBackground[0], LightMode.MouseOverBackground[1], LightMode.MouseOverBackground[2]));

            // y = 1.24998x − 60.9964
            LightMode.MouseDown[0] = (Byte)(((LightMode.Idle[0] + 60.9964d) * 0.8000128002048033d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseDown[1] = (Byte)(((LightMode.Idle[1] + 60.9964d) * 0.8000128002048033d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseDown[2] = (Byte)(((LightMode.Idle[2] + 60.9964d) * 0.8000128002048033d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseDownAsBrush = new(Color.FromRgb(LightMode.MouseDown[0], LightMode.MouseDown[1], LightMode.MouseDown[2]));

            // y = 1.13329x − 33.5933
            LightMode.MouseDownBorder[0] = (Byte)(((LightMode.Idle[0] + 33.5933d) * 0.8823866794906864d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseDownBorder[1] = (Byte)(((LightMode.Idle[1] + 33.5933d) * 0.8823866794906864d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseDownBorder[2] = (Byte)(((LightMode.Idle[2] + 33.5933d) * 0.8823866794906864d) + MAGIC_NUMBER - MAGIC_NUMBER);
            LightMode.MouseDownBorderAsBrush = new(Color.FromRgb(LightMode.MouseDownBorder[0], LightMode.MouseDownBorder[1], LightMode.MouseDownBorder[2]));
            #endregion
        }
    }
}