using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karesz
{
	/// <summary>
	/// Egészek fölötti kétdimenziós vektorokb (Z^2)
	/// </summary>
	struct Vektor
	{
		public int X, Y;
		public Vektor(int x, int y) => (X, Y) = (x, y);
		public Vektor(Vektor V) : this(V.X, V.Y) { }
		public int HosszN() => X * X + Y * Y;
		public void ForgNeg() => (X, Y) = (-Y, X); // fordított a koordinátarendszer!
		public void ForgPoz() => (X, Y) = (Y, -X);
		public void Hozzáad(Vektor V) { X += V.X; Y += V.Y; }
		public void Felszoroz(int a) { X *= a; Y *= a; }
		public void Kisebbít(Vektor V) { X -= V.X; Y -= V.Y; }
		/// <summary>
		/// A négy égtáj fele mutató irányvektort lekódoljuk egy int-be. 
		/// 0: észak
		/// 1: kelet
		/// 2: dél
		/// 3: nyugat
		/// </summary>
		/// <returns></returns>
		public int ToInt() { return Y == -1 ? 0 : (X == 1 ? 1 : (Y == 1 ? 2 : 3)); }
		public static int operator *(Vektor u, Vektor v) => u.X * v.X + u.Y * v.Y;
		public static Vektor operator *(Vektor u, int a) => new Vektor(u.X * a, u.Y * a);
		public static Vektor operator *(int a, Vektor u) => u * a;
		public static Vektor operator +(Vektor u, Vektor v) => new Vektor(u.X + v.X, u.Y + v.Y);
		public static Vektor operator -(Vektor u, Vektor v) => new Vektor(u.X - v.X, u.Y - v.Y);
		public int TavN(Vektor Q) { return (this - Q).HosszN(); }
	}
}
