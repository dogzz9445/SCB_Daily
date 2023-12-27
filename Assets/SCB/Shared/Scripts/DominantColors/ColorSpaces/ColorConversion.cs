// Reference from https://patrickwu.space/2016/06/12/csharp-color/#rgb2lab
using System;
using UnityEngine;

namespace SCB.ColorSpaces
{
    public static class ColorConversion
    {
        /// <summary>
        /// Converts a RGB color format to an hexadecimal color.
        /// </summary>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        public static string RGBToHex(int r, int g, int b)
        {
            return String.Format("#{0:x2}{1:x2}{2:x2}", r, g, b).ToUpper();
        }
        /// <summary>
        /// Converts RGB to CIE XYZ (CIE 1931 color space)
        /// </summary>
        public static CIEXYZ RGBtoXYZ(int red, int green, int blue)
        {
            // normalize red, green, blue values
            double rLinear = (double)red / 255.0;
            double gLinear = (double)green / 255.0;
            double bLinear = (double)blue / 255.0;

            // convert to a sRGB form
            double r = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / (
                1 + 0.055), 2.2) : (rLinear / 12.92);
            double g = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / (
                1 + 0.055), 2.2) : (gLinear / 12.92);
            double b = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / (
                1 + 0.055), 2.2) : (bLinear / 12.92);

            // converts
            return new CIEXYZ(
                (r * 0.4124 + g * 0.3576 + b * 0.1805),
                (r * 0.2126 + g * 0.7152 + b * 0.0722),
                (r * 0.0193 + g * 0.1192 + b * 0.9505)
                );
        }

        /// <summary>
        /// XYZ to L*a*b* transformation function.
        /// </summary>
        private static double Fxyz(double t)
        {
            return ((t > 0.008856) ? Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));
        }

        /// <summary>
        /// Converts CIEXYZ to CIELab.
        /// </summary>
        public static CIELab XYZtoLab(double x, double y, double z)
        {
            CIELab lab = CIELab.Empty;

            lab.L = 116.0 * Fxyz(y / CIEXYZ.D65.Y) - 16;
            lab.A = 500.0 * (Fxyz(x / CIEXYZ.D65.X) - Fxyz(y / CIEXYZ.D65.Y));
            lab.B = 200.0 * (Fxyz(y / CIEXYZ.D65.Y) - Fxyz(z / CIEXYZ.D65.Z));

            return lab;
        }

        /// <summary>
        /// Converts CIEXYZ to CIELab.
        /// </summary>
        public static CIELab XYZtoLab(CIEXYZ xyz)
        {
            double x = xyz.X;
            double y = xyz.Y;
            double z = xyz.Z;
            CIELab lab = CIELab.Empty;

            lab.L = 116.0 * Fxyz(y / CIEXYZ.D65.Y) - 16;
            lab.A = 500.0 * (Fxyz(x / CIEXYZ.D65.X) - Fxyz(y / CIEXYZ.D65.Y));
            lab.B = 200.0 * (Fxyz(y / CIEXYZ.D65.Y) - Fxyz(z / CIEXYZ.D65.Z));

            return lab;
        }


        /// <summary>
        /// Converts RGB to CIELab.
        /// </summary>
        public static CIELab RGBtoLab(int red, int green, int blue)
        {
            return XYZtoLab(RGBtoXYZ(red, green, blue));
        }

        /// <summary>
        /// Converts CIELab to CIEXYZ.
        /// </summary>
        public static CIEXYZ LabtoXYZ(double l, double a, double b)
        {
            double delta = 6.0 / 29.0;

            double fy = (l + 16) / 116.0;
            double fx = fy + (a / 500.0);
            double fz = fy - (b / 200.0);

            return new CIEXYZ(
                (fx > delta) ? CIEXYZ.D65.X * (fx * fx * fx) : (fx - 16.0 / 116.0) * 3 * (
                    delta * delta) * CIEXYZ.D65.X,
                (fy > delta) ? CIEXYZ.D65.Y * (fy * fy * fy) : (fy - 16.0 / 116.0) * 3 * (
                    delta * delta) * CIEXYZ.D65.Y,
                (fz > delta) ? CIEXYZ.D65.Z * (fz * fz * fz) : (fz - 16.0 / 116.0) * 3 * (
                    delta * delta) * CIEXYZ.D65.Z
                );
        }
        /// <summary>
        /// Converts CIEXYZ to RGB structure.
        /// </summary>
        public static RGB XYZtoRGB(double x, double y, double z)
        {
            double[] Clinear = new double[3];
            Clinear[0] = x*3.2410 - y*1.5374 - z*0.4986; // red
            Clinear[1] = -x*0.9692 + y*1.8760 - z*0.0416; // green
            Clinear[2] = x*0.0556 - y*0.2040 + z*1.0570; // blue
            
            for(int i=0; i<3; i++)
            {
                Clinear[i] = (Clinear[i]<=0.0031308)? 12.92*Clinear[i] : (
                    1+0.055)* Math.Pow(Clinear[i], (1.0/2.4)) - 0.055;
            }
            
            return new RGB(
                Convert.ToInt32( Double.Parse(String.Format("{0:0.00}",
                    Clinear[0]*255.0)) ),
                Convert.ToInt32( Double.Parse(String.Format("{0:0.00}",
                    Clinear[1]*255.0)) ),
                Convert.ToInt32( Double.Parse(String.Format("{0:0.00}",
                    Clinear[2]*255.0)) )
                );
        }		 
        
        /// <summary>
        /// Converts CIELab to RGB.
        /// </summary>
        public static RGB XYZtoRGB(CIEXYZ xyz)
        {
            return XYZtoRGB(xyz.X, xyz.Y, xyz.Z);
        }		 

        /// <summary>
        /// Converts CIELab to RGB.
        /// </summary>
        public static RGB LabtoRGB(double l, double a, double b)
        {
            return XYZtoRGB(LabtoXYZ(l, a, b));
        }
    }
}