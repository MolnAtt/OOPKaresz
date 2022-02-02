using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Karesz
{
	partial class Form1
	{
		/// <summary>
		/// A Pálya egy olyan class, amely azon túl, hogy megrajzolja a pályát,
		/// a rendelkezésre álló robotokat is felrajzolja a pályára. Rendelkezik továbbá azokkal a függvényekkel, amelyek
		/// kiszolgálják a robotok szenzorait és cselekvéseit. Illetve itt helyezkedik el a fájlból pályát betöltő metódus is.
		/// </summary>
		class Pálya
		{
			int X { get; set; }
			int Y { get; set; }
			Vektor l; 
			public Vektor L { get => l; private set => l = value; }
			Brush[] tollkészlet;
			Pen vonalzósceruza;
			int[,] tábla;
			int[,] hőtábla;
			PictureBox képkeret;

			static SolidBrush[] Új_tollkészlet()
			{
				SolidBrush[] tollkészlet = new SolidBrush[9];
				foreach (int szín in színkódok)
					tollkészlet[szín] = new SolidBrush(színek[szín]);
				return tollkészlet;
			}
			static Pen Új_vonalzósceruza() => 
				new Pen(new SolidBrush(Color.Gray), 1);
			Pálya(int X, int Y, Vektor L, Brush[] tollkészlet, Pen vonalzósceruza, int[,] tábla, int[,] hőtábla, PictureBox képkeret)
			{
				this.X = X;
				this.Y = Y;
				this.l = L;
				this.tollkészlet = tollkészlet;
				this.vonalzósceruza = vonalzósceruza;
				this.tábla = tábla;
				this.hőtábla = hőtábla;
				this.képkeret = képkeret;
			}
			Pálya(int X, int Y, int lxy, PictureBox képkeret) :
				this(X, Y, new Vektor(lxy, lxy), Új_tollkészlet(), Új_vonalzósceruza(), new int[X, Y], new int[X, Y], képkeret)
			{ }
			public Pálya(PictureBox képkeret) :
				this(41, 31, 24, képkeret) 
			{ }
			/// <summary>
			/// Megnézi, hogy értelmezhető-e a pályán az adott pont.
			/// </summary>
			/// <param name="V">A pont, amiről megvizsgáljuk, hogy rajta van-e a pályán vagy sem</param>
			/// <returns>Igaz, ha rajta van, hamis, ha nincs.</returns>
			public bool BenneVan(Vektor V) => 
				0 <= V.X && V.X < X && 0 <= V.Y && V.Y < Y;
			/// <summary>
			/// Visszaadja egy vektorral megadott ponton lévő entitás kódját.
			/// </summary>
			/// <param name="P">A vizsgálandó pozíció</param>
			/// <returns>Az itt lévő entitás kódja.</returns>
			private int Ha_van(Vektor P, int eredmény) => 
				BenneVan(P) ? eredmény : -1;
			public int MiVanItt(Vektor P) => 
				Ha_van(P, tábla[P.X, P.Y]);
			public int Hőmérséklet(Vektor P) => 
				Ha_van(P, hőtábla[P.X, P.Y]);
			/// <summary>
			/// Felülírja a pálya egy adott pontját azzal az értékkel, amit megadunk.
			/// </summary>
			/// <param name="P"> Az itt lévő dolgot írja át</param>
			/// <param name="ez">Erre írja át</param>
			public void LegyenItt(Vektor P, int ez) => 
				tábla[P.X, P.Y] = ez;
			/// <summary>
			/// Ellenőrzi, hogy van-e kavics az adott pozíción.
			/// </summary>
			/// <param name="P">A vizsgálandó pozíció</param>
			/// <returns>igaz, ha van, hamis, ha nincs.</returns>
			public bool VanKavics(Vektor P) => 
				MiVanItt(P) > fal;
			void Négyzetrajz(PaintEventArgs e , int tollszínkód, int x, int y) => 
				e.Graphics.FillRectangle(tollkészlet[tollszínkód], x * l.X, y * l.Y, l.X, l.Y);
			void Körrajz(PaintEventArgs e, int tollszínkód, int x, int y) => 
				e.Graphics.FillEllipse(tollkészlet[tollszínkód], x * l.X + 2, y * l.Y + 2, l.X - 4, l.Y - 4);
			void Vonalrajz(PaintEventArgs e, int x1, int y1, int x2, int y2) => 
				e.Graphics.DrawLine(vonalzósceruza, x1 * l.X, y1 * l.Y, x2 * l.X, y2 * l.Y);
			/// <summary>
			/// Lerajzol mindent a pályán, amit csak lehetséges. Robotokat is beleértve.
			/// </summary>
			/// <param name="vászon"></param>
			/// <param name="e"></param>
			public void Rajz(PictureBox vászon, PaintEventArgs e)
			{
				for (int y = 1; y < Y; ++y) Vonalrajz(e, 0, y, X, y); // vízszintes vonalak
				for (int x = 1; x < X; ++x) Vonalrajz(e, x, 0, x, Y); // függőleges vonalak
				for (int y = 0; y < Y; ++y)
					for (int x = 0; x < X; ++x)
						switch (tábla[x, y])
						{
							case fal: 
							case láva: 
							case víz: Négyzetrajz(e, tábla[x, y], x, y); 
								break;
							case fekete: 
							case piros: 
							case zöld: 
							case sárga: 
							case hó: Körrajz(e, tábla[x, y], X, y); 
								break;
						}
				foreach (Robot robot in Robot.lista)
					e.Graphics.DrawImageUnscaledAndClipped(robot.Iránykép(), new Rectangle(robot.H.X * l.X, robot.H.Y * l.Y, l.X, l.Y));
			}
			/// <summary>
			/// Üres pályát generál (valódi betöltés overloaddal történik)
			/// </summary>
			public void Betölt()
			{
				 // a szélére -1-et rakunk, a belsejébe nullát.
					for (int x = 0; x < X; x++) tábla[x, 0] = -1;
					for (int x = 0; x < X; x++) tábla[x, Y - 1] = -1;
					for (int y = 1; y < Y - 1; y++) tábla[0, y] = -1;
					for (int y = 1; y < Y - 1; y++) tábla[X - 1, y] = -1;
					for (int y = 1; y < Y - 1; ++y)
						for (int x = 1; x < X - 1; ++x)
							tábla[x, y] = 0;

				Hőtérképezés();
				Frissít();
			}
			/// <summary>
			/// Betölti a megadott elérési útvonalon lévő pályát. 
			/// </summary>
			/// <param name="fájlnév"></param>
			public void Betölt(string fájlnév)
			{
				try
				{
					StreamReader f = new StreamReader(fájlnév);
					for (int y = 0; y < Y; ++y)
					{
						string[] sor = f.ReadLine().Split('\t');
						for (int x = 0; x < X; ++x)
							tábla[x, y] = Convert.ToInt32(sor[x]);
					}
					f.Close();
				}
				catch (FileNotFoundException)
				{
					MessageBox.Show("Nincs meg a pálya!");
				}

				Hőtérképezés();
				Frissít();
			}
			private bool VanELáva()
			{
				for (int i = 0; i < X; i++)
					for (int j = 0; j < Y; j++)
						if (tábla[i, j] == 7) 
							return true;
				return false;
			}
			private void Melegedés(int i, int j)
			{
				List<int> környezetihőmérséklet = new List<int>();
				if (0 < i && hőtábla[i - 1, j] > 200) környezetihőmérséklet.Add(hőtábla[i - 1, j]);
				if (0 < j && hőtábla[i, j - 1] > 200) környezetihőmérséklet.Add(hőtábla[i, j - 1]);
				if (i < X - 1 && hőtábla[i + 1, j] > 200) környezetihőmérséklet.Add(hőtábla[i + 1, j]);
				if (j < Y - 1 && hőtábla[i, j + 1] > 200) környezetihőmérséklet.Add(hőtábla[i, j + 1]);
				if (környezetihőmérséklet.Count > 0)
				{
					int kihívó = környezetihőmérséklet.Max() - 200;
					if (hőtábla[i, j] < kihívó) hőtábla[i, j] = kihívó;
				}
			}
			private void Hőtérképezés()
			{
				if (VanELáva())
				{// A láva 1000 fokos... ("Inicializálás")
					for (int i = 0; i < X; i++)
						for (int j = 0; j < Y; j++)
							hőtábla[i, j] = (tábla[i, j] == 7 ? 1000 : 0);
					//... és minden szomszédos mezőn 200 fokkal hűvösebb. Tehát 4-szer (1000->800->600->400->200) végigmegyünk, hogy a felmelegedést update-eljük.
					for (int k = 0; k < ((1000 / 200) - 1); k++)
						for (int i = 0; i < X; i++)
							for (int j = 0; j < Y; j++)
								Melegedés(i, j);
				}
			}
			public void Frissít() => képkeret.Refresh();
		}
	}
}