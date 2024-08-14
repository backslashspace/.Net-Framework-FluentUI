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
            internal static readonly Byte[] PrimaryColor = [AccentPalette[4], AccentPalette[5], AccentPalette[6]];
            internal static SolidColorBrush PrimaryColorAsBrush = new(Color.FromRgb(DarkMode.PrimaryColor[0], DarkMode.PrimaryColor[1], DarkMode.PrimaryColor[2]));

            internal static readonly Byte[] MouseOver = [71, 177, 232];
            internal static SolidColorBrush MouseOverAsBrush = new(Color.FromRgb(MouseOver[0], MouseOver[1], MouseOver[2]));

            internal static readonly Byte[] MouseDown = [66, 161, 210];
            internal static SolidColorBrush MouseDownAsBrush = new(Color.FromRgb(MouseDown[0], MouseDown[1], MouseDown[2]));

            //

            internal static readonly Byte[] BorderColor = [90, 199, 255];
            internal static SolidColorBrush BorderColorAsBrush = new(Color.FromRgb(BorderColor[0], BorderColor[1], BorderColor[2]));

            internal static readonly Byte[] BorderColorMouseOver = [86, 183, 234];
            internal static SolidColorBrush BorderColorMouseOverAsBrush = new(Color.FromRgb(BorderColorMouseOver[0], BorderColorMouseOver[1], BorderColorMouseOver[2]));
        }

        internal static class LightMode
        {
            internal static readonly Byte[] PrimaryColor = [AccentPalette[16], AccentPalette[17], AccentPalette[18]];
            internal static SolidColorBrush PrimaryColorAsBrush = new(Color.FromRgb(PrimaryColor[0], PrimaryColor[1], PrimaryColor[2]));

            internal static readonly Byte[] MouseOver = [25, 117, 197];
            internal static SolidColorBrush MouseOverAsBrush = new(Color.FromRgb(MouseOver[0], MouseOver[1], MouseOver[2]));

            internal static readonly Byte[] MouseDown = [49, 131, 202];
            internal static SolidColorBrush MouseDownAsBrush = new(Color.FromRgb(MouseDown[0], MouseDown[1], MouseDown[2]));

            //

            internal static readonly Byte[] BorderColor = [20, 115, 197];
            internal static SolidColorBrush BorderColorAsBrush = new(Color.FromRgb(BorderColor[0], BorderColor[1], BorderColor[2]));

            internal static readonly Byte[] BorderColorMouseOver = [43, 128, 202];
            internal static SolidColorBrush BorderColorMouseOverAsBrush = new(Color.FromRgb(BorderColorMouseOver[0], BorderColorMouseOver[1], BorderColorMouseOver[2]));
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal delegate void AccentHandler();
        internal static event AccentHandler Changed;

        internal static AccentHandler GetChangedInvoker() => Changed!;

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        internal static readonly Byte[] AccentPalette = // default windows colors
        [0x99, 0xEB, 0xFF, 0x00, 0x4C, 0xC2, 0xFF, 0x00,
         0x00, 0x91, 0xF8, 0x00, 0x00, 0x78, 0xD4, 0x00,
         0x00, 0x67, 0xC0, 0x00, 0x00, 0x3E, 0x92, 0x00,
         0x00, 0x1A, 0x68, 0x00, 0xF7, 0x63, 0x0C, 0x00];

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #

        private const Double MAGIC_NUMBER = 6755399441055744.0d;

        internal static void CalculateDerivedColors()
        {
            #region DarkMode
            DarkMode.PrimaryColor[0] = AccentPalette[4];
            DarkMode.PrimaryColor[1] = AccentPalette[5];
            DarkMode.PrimaryColor[2] = AccentPalette[6];
            DarkMode.PrimaryColorAsBrush = new(Color.FromRgb(DarkMode.PrimaryColor[0], DarkMode.PrimaryColor[1], DarkMode.PrimaryColor[2]));

            // (-10)x = y - 25
            DarkMode.MouseOver[0] = (Byte)(DarkMode.PrimaryColor[0] + (Int32)(((DarkMode.PrimaryColor[0] - 25.1) * -0.1) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.MouseOver[1] = (Byte)(DarkMode.PrimaryColor[1] + (Int32)(((DarkMode.PrimaryColor[1] - 25.1) * -0.1) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.MouseOver[2] = (Byte)(DarkMode.PrimaryColor[2] + (Int32)(((DarkMode.PrimaryColor[2] - 25.1) * -0.1) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.MouseOverAsBrush = new(Color.FromRgb(DarkMode.MouseOver[0], DarkMode.MouseOver[1], DarkMode.MouseOver[2]));

            // (-5)x = y - 27.5
            DarkMode.MouseDown[0] = (Byte)(DarkMode.PrimaryColor[0] + (Int32)(((DarkMode.PrimaryColor[0] - 27.51) * -0.2) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.MouseDown[1] = (Byte)(DarkMode.PrimaryColor[1] + (Int32)(((DarkMode.PrimaryColor[1] - 27.51) * -0.2) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.MouseDown[2] = (Byte)(DarkMode.PrimaryColor[2] + (Int32)(((DarkMode.PrimaryColor[2] - 27.51) * -0.2) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.MouseDownAsBrush = new(Color.FromRgb(DarkMode.MouseDown[0], DarkMode.MouseDown[1], DarkMode.MouseDown[2]));

            // (-12.75)x = y - 255
            DarkMode.BorderColor[0] = (Byte)(DarkMode.PrimaryColor[0] + (Int32)(((DarkMode.PrimaryColor[0] - 255) * -0.0784313725490195) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.BorderColor[1] = (Byte)(DarkMode.PrimaryColor[1] + (Int32)(((DarkMode.PrimaryColor[1] - 255) * -0.0784313725490195) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.BorderColor[2] = (Byte)(DarkMode.PrimaryColor[2] + (Int32)(((DarkMode.PrimaryColor[2] - 255) * -0.0784313725490195) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.BorderColorAsBrush = new(Color.FromRgb(DarkMode.BorderColor[0], DarkMode.BorderColor[1], DarkMode.BorderColor[2]));

            // (-5.8) x = y - 131.2
            DarkMode.BorderColorMouseOver[0] = (Byte)(DarkMode.PrimaryColor[0] + (Int32)(((DarkMode.PrimaryColor[0] - 131.2) * -0.1724137931034483) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.BorderColorMouseOver[1] = (Byte)(DarkMode.PrimaryColor[1] + (Int32)(((DarkMode.PrimaryColor[1] - 131.2) * -0.1724137931034483) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.BorderColorMouseOver[2] = (Byte)(DarkMode.PrimaryColor[2] + (Int32)(((DarkMode.PrimaryColor[2] - 131.2) * -0.1724137931034483) + MAGIC_NUMBER - MAGIC_NUMBER));
            DarkMode.BorderColorMouseOverAsBrush = new(Color.FromRgb(DarkMode.BorderColorMouseOver[0], DarkMode.BorderColorMouseOver[1], DarkMode.BorderColorMouseOver[2]));
            #endregion

            #region LightMode
            LightMode.PrimaryColor[0] = AccentPalette[16];
            LightMode.PrimaryColor[1] = AccentPalette[17];
            LightMode.PrimaryColor[2] = AccentPalette[18];
            LightMode.PrimaryColorAsBrush = new(Color.FromRgb(LightMode.PrimaryColor[0], LightMode.PrimaryColor[1], LightMode.PrimaryColor[2]));

            // (-10)x = y - 245
            LightMode.MouseOver[0] = (Byte)(LightMode.PrimaryColor[0] + (Int32)(((LightMode.PrimaryColor[0] - 245.1) * -0.1) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.MouseOver[1] = (Byte)(LightMode.PrimaryColor[1] + (Int32)(((LightMode.PrimaryColor[1] - 245.1) * -0.1) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.MouseOver[2] = (Byte)(LightMode.PrimaryColor[2] + (Int32)(((LightMode.PrimaryColor[2] - 245.1) * -0.1) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.MouseOverAsBrush = new(Color.FromRgb(LightMode.MouseOver[0], LightMode.MouseOver[1], LightMode.MouseOver[2]));

            // -5 x = y - 243
            LightMode.MouseDown[0] = (Byte)(LightMode.PrimaryColor[0] + (Int32)(((LightMode.PrimaryColor[0] - 243) * -0.2) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.MouseDown[1] = (Byte)(LightMode.PrimaryColor[1] + (Int32)(((LightMode.PrimaryColor[1] - 243) * -0.2) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.MouseDown[2] = (Byte)(LightMode.PrimaryColor[2] + (Int32)(((LightMode.PrimaryColor[2] - 243) * -0.2) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.MouseDownAsBrush = new(Color.FromRgb(LightMode.MouseDown[0], LightMode.MouseDown[1], LightMode.MouseDown[2]));

            // (-12.75)x = y - 255
            LightMode.BorderColor[0] = (Byte)(LightMode.PrimaryColor[0] + (Int32)(((LightMode.PrimaryColor[0] - 255) * -0.0784313725490195) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.BorderColor[1] = (Byte)(LightMode.PrimaryColor[1] + (Int32)(((LightMode.PrimaryColor[1] - 255) * -0.0784313725490195) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.BorderColor[2] = (Byte)(LightMode.PrimaryColor[2] + (Int32)(((LightMode.PrimaryColor[2] - 255) * -0.0784313725490195) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.BorderColorAsBrush = new(Color.FromRgb(LightMode.BorderColor[0], LightMode.BorderColor[1], LightMode.BorderColor[2]));

            // (-5.82) x = y - 248
            LightMode.BorderColorMouseOver[0] = (Byte)(LightMode.PrimaryColor[0] + (Int32)(((LightMode.PrimaryColor[0] - 248) * -0.1718213058419243) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.BorderColorMouseOver[1] = (Byte)(LightMode.PrimaryColor[1] + (Int32)(((LightMode.PrimaryColor[1] - 248) * -0.1718213058419243) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.BorderColorMouseOver[2] = (Byte)(LightMode.PrimaryColor[2] + (Int32)(((LightMode.PrimaryColor[2] - 248) * -0.1718213058419243) + MAGIC_NUMBER - MAGIC_NUMBER));
            LightMode.BorderColorMouseOverAsBrush = new(Color.FromRgb(LightMode.BorderColorMouseOver[0], LightMode.BorderColorMouseOver[1], LightMode.BorderColorMouseOver[2]));
            #endregion
        }
    }
}