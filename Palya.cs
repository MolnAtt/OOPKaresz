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
		/// A Pálya egy olyan PictureBox-ból származtatott class, amely azon túl, hogy megrajzolja magát pályának,
		/// a rendelkezésre álló robotokat is felrajzolja a pályára. Rendelkezik továbbá azokkal a függvényekkel, amelyek
		/// kiszolgálják a robotok szenzorait és cselekvéseit. Illetve itt helyezkedik el a fájlból pályát betöltő metódus is.
		/// </summary>
		class Pálya : PictureBox
		{
			public const int X = 41;
			public const int Y = 31;
			public const int LX = 24;   // ha fix léptékkel akarunk pályát
			public const int LY = 24;   // ha fix léptékkel akarunk pályát
			public Vektor lépték;
			private Brush[] tollkészlet = new SolidBrush[9];
			private Pen vonalzósceruza = new Pen(new SolidBrush(Color.Gray), 1);
			private Color[] színek = { Color.White, Color.Brown, Color.Black, Color.Red, Color.Green, Color.Yellow, Color.White, Color.Orange, Color.Blue };
			private int[,] tábla = new int[X, Y];
			private int[,] hőtábla = new int[X, Y];
			public Pálya()
			{
				tollkészlet[üres] = new SolidBrush(színek[üres]);
				tollkészlet[fal] = new SolidBrush(színek[fal]);
				tollkészlet[fekete] = new SolidBrush(színek[fekete]);
				tollkészlet[piros] = new SolidBrush(színek[piros]);
				tollkészlet[zöld] = new SolidBrush(színek[zöld]);
				tollkészlet[sárga] = new SolidBrush(színek[sárga]);
				tollkészlet[hó] = new SolidBrush(színek[hó]);
				tollkészlet[láva] = new SolidBrush(színek[láva]);
				tollkészlet[víz] = new SolidBrush(színek[víz]);
				lépték = new Vektor(LX, LY);
				//new Vektor(Width / X, Height / Y);
				Width = X * lépték.X; // maradékos osztás hibái miatt
				Height = Y * lépték.Y;

				BackColor = Color.White;
				Location = new Point(10, 10);
				Margin = new Padding(4, 4, 4, 4);
				Name = "Pálya";
				Size = new Size(Width, Height);
				TabIndex = 0;
				TabStop = false;
			}
			/// <summary>
			/// Megnézi, hogy értelmezhető-e a pályán az adott pont.
			/// </summary>
			/// <param name="V">A pont, amiről megvizsgáljuk, hogy rajta van-e a pályán vagy sem</param>
			/// <returns>Igaz, ha rajta van, hamis, ha nincs.</returns>
			public bool BenneVan(Vektor V) => 0 <= V.X && V.X < X && 0 <= V.Y && V.Y < Y;
			/// <summary>
			/// Visszaadja egy vektorral megadott ponton lévő entitás kódját.
			/// </summary>
			/// <param name="P">A vizsgálandó pozíció</param>
			/// <returns>Az itt lévő entitás kódja.</returns>
			private int Ha_van(Vektor P, int eredmény) => BenneVan(P) ? eredmény : -1;
			public int MiVanItt(Vektor P) => Ha_van(P, tábla[P.X, P.Y]);
			public int Hőmérséklet(Vektor P) => Ha_van(P, hőtábla[P.X, P.Y]);
			/// <summary>
			/// Felülírja a pálya egy adott pontját azzal az értékkel, amit megadunk.
			/// </summary>
			/// <param name="P"> Az itt lévő dolgot írja át</param>
			/// <param name="ez">Erre írja át</param>
			public void LegyenItt(Vektor P, int ez) => tábla[P.X, P.Y] = ez;
			/// <summary>
			/// Ellenőrzi, hogy van-e kavics az adott pozíción.
			/// </summary>
			/// <param name="P">A vizsgálandó pozíció</param>
			/// <returns>igaz, ha van, hamis, ha nincs.</returns>
			public bool VanKavics(Vektor P) => MiVanItt(P) > fal;
			void Négyzetrajz(PaintEventArgs e , int tollszínkód, int x, int y) => e.Graphics.FillRectangle(tollkészlet[tollszínkód], x * lépték.X, y * lépték.Y, lépték.X, lépték.Y);
			void Körrajz(PaintEventArgs e, int tollszínkód, int x, int y) => e.Graphics.FillEllipse(tollkészlet[tollszínkód], x * lépték.X + 2, y * lépték.Y + 2, lépték.X - 4, lépték.Y - 4);
			void Vonalrajz(PaintEventArgs e, int x1, int y1, int x2, int y2) => e.Graphics.DrawLine(vonalzósceruza, x1 * lépték.X, y1 * lépték.Y, x2 * lépték.X, y2 * lépték.Y);
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
							case víz: Négyzetrajz(e, tábla[x, y], x, y); break;
							case fekete: 
							case piros: 
							case zöld: 
							case sárga: 
							case hó: Körrajz(e, tábla[x, y], X, y); break;
						}
				foreach (Robot robot in robotlista)
					e.Graphics.DrawImageUnscaledAndClipped(robot.Iránykép(), new Rectangle(robot.HolVan().X * lépték.X, robot.HolVan().Y * lépték.Y, lépték.X, lépték.Y));
			}
			/// <summary>
			/// A pályaválasztó textboxban szereplő fájlt betölti és újrarajzolja ennek megfelelően a pályát. Üres string esetén üres pályát ad.
			/// </summary>
			/// <param name="fájlnév"></param>
			public void Betölt(string fájlnév = "")
			{
				if (fájlnév.Length > 0)
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
					catch (FileNotFoundException e)
					{
						MessageBox.Show("Nincs meg a pálya!");
					}
				}
				else
				{ // a szélére -1-et rakunk, a belsejébe nullát.
					for (int x = 0; x < X; x++) tábla[x, 0] = -1;
					for (int x = 0; x < X; x++) tábla[x, Y - 1] = -1;
					for (int y = 1; y < Y - 1; y++) tábla[0, y] = -1;
					for (int y = 1; y < Y - 1; y++) tábla[X - 1, y] = -1;
					for (int y = 1; y < Y - 1; ++y)
						for (int x = 1; x < X - 1; ++x)
							tábla[x, y] = 0;
				}

				Hőtérképezés();
				Refresh();
			}
			private bool VanELáva()
			{
				for (int i = 0; i < X; i++)
					for (int j = 0; j < Y; j++)
						if (tábla[i, j] == 7) return true;
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
		}
	}
}