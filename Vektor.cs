using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karesz
{
	/// <summary>
	/// Egészek fölötti kétdimenziós vektorok (Z^2)
	/// </summary>
	struct Vektor
	{
        #region tulajdonságok

        public int X, Y;

        #endregion

        #region konstruktorok

        public Vektor(int x, int y) =>
			(X, Y) = (x, y);
		public Vektor(Vektor V) :
			this(V.X, V.Y)
		{ }

        #endregion

		#region operátorok (+,-,*,/)

		public static Vektor operator +(Vektor u, Vektor v) =>
			new Vektor(u.X + v.X, u.Y + v.Y);
		public static Vektor operator -(Vektor u, Vektor v) =>
			new Vektor(u.X - v.X, u.Y - v.Y);
		public static int operator *(Vektor u, Vektor v) =>
			u.X * v.X + u.Y * v.Y;
		public static Vektor operator *(Vektor u, int a) =>
			new Vektor(u.X * a, u.Y * a);
		public static Vektor operator *(int a, Vektor u) =>
			u * a;
		public static Vektor operator /(Vektor u, int a) =>
			new Vektor(u.X / a, u.Y / a);

        #endregion

        #region egyéb műveletek

        public int HosszN() =>
			X * X + Y * Y;
		public void Forgat(int i) =>
			(X, Y) = (-i * Y, i * X); // fordított a koordinátarendszer!
		/// <summary>
		/// A négy égtáj fele mutató irányvektort lekódoljuk egy int-be. 
		/// 0: észak
		/// 1: kelet
		/// 2: dél
		/// 3: nyugat
		/// </summary>
		/// <returns></returns>
		public int ToInt() =>
			Y == -1 ? 0 : (X == 1 ? 1 : (Y == 1 ? 2 : 3));
		public int TavN(Vektor Q) =>
			(this - Q).HosszN();
		public Vektor Balra() =>
			new Vektor(X - 1, Y);
		public Vektor Jobbra() =>
			new Vektor(X + 1, Y);
		public Vektor Fent() =>
			new Vektor(X, Y + 1);
		public Vektor Lent() =>
			new Vektor(X, Y - 1);

        #endregion
    }
}
