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
		class Robot
		{
			private string név;
			private Vektor H;
			private Vektor I = new Vektor(0, 1);
			private int[] kődb;
			private Bitmap[] képkészlet;
			/// <summary>
			/// Létrehoz egy új robotot a megadott névvel, képkészlettel, pozícióval, iránnyal és induló kövek számával.
			/// </summary>
			/// <param name="adottnév">A robot neve</param>
			/// <param name="képek">A képkészlet a Resources mappából</param>
			/// <param name="Pozíció">indulási pozíció</param>
			/// <param name="Irány">kezdőirány</param>
			/// <param name="indulókövek">induláskor a zsebeiben lévő kövek száma</param>
			public Robot(string adottnév, Bitmap[] képek, Vektor Pozíció, Vektor Irány, int[] indulókövek)
			{
				név = adottnév;
				H = Pozíció;
				I = Irány;
				képkészlet = képek;
				kődb = indulókövek;
			}

			/// <summary>
			/// Létrehoz egy új robotot a megadott névvel és induló kövek számával az 5,28 helyen északra nézvén.
			/// </summary>
			/// <param name="adottnév">A robot neve</param>
			/// <param name="indulókövek">induláskor a zsebeiben lévő kövek száma</param>
			public Robot(string adottnév, int[] indulókövek)
			{
				név = adottnév;
				H = new Vektor(5, 28);
				I = new Vektor(0, -1);
				képkészlet = new Bitmap[4]
					{
						Properties.Resources.Karesz0,
						Properties.Resources.Karesz1,
						Properties.Resources.Karesz2,
						Properties.Resources.Karesz3
					};
				kődb = indulókövek;
				robotlista.Add(this);
			}

			/// <summary>
			/// Létrehoz egy új üres zsebű kék robotot a megadott névvel az 5,28 helyen északra nézvén.
			/// </summary>
			/// <param name="adottnév">A robot neve</param>
			public Robot(string adottnév)
			{
				név = adottnév;
				H = new Vektor(5, 28);
				I = new Vektor(0, -1);
				képkészlet = new Bitmap[4]
					{
						Properties.Resources.Karesz0,
						Properties.Resources.Karesz1,
						Properties.Resources.Karesz2,
						Properties.Resources.Karesz3
					};
				kődb = new int[5] { 0, 0, 0, 0, 0 };
				robotlista.Add(this);
			}
			/// <summary>
			/// Elhelyezi a Robotot a megadott helyre.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			public string Neve() { return név; }
			public void Teleport(int x, int y) { H.X = x; H.Y = y; }
			/// <summary>
			/// Visszaadja a robot koordinátáit.
			/// </summary>
			/// <returns></returns>
			public Vektor HolVan() { return new Vektor(H); }
			/// <summary>
			/// Megadja, hogy az adott színből mennyi köve van a robotnak.
			/// </summary>
			/// <param name="szín"></param>
			/// <returns></returns>
			public int Mennyi(int szín) { return kődb[szín - 2]; }
			/// <summary>
			/// Lépteti a robotot a megfelelő irányba.
			/// </summary>
			public void Lép()
			{
				Thread.Sleep(várakozás);

				H.Hozzáad(I); // Ahova lépni készül.

				if (pálya.BenneVan(H) && pálya.MiVanItt(H) != fal) { óra.Tak(); }
				else { MessageBox.Show(név + ": Nem tudok lépni!"); H.Kisebbít(I); }

				pálya.Refresh();
				monitorpanel.Frissít();
			}
			/// <summary>
			/// Elforgatja a robotot a megadott irányban. (Csak normális irányokra reagál.)
			/// </summary>
			/// <param name="forgásirány"></param>
			public void Fordul(int forgásirány)
			{
				Thread.Sleep(várakozás);

				if (forgásirány == balra) I.ForgPoz(); else I.ForgNeg();

				óra.Tak();
				pálya.Refresh();
				monitorpanel.Frissít();
			}
			/// <summary>
			/// Visszaadja a sebességvektor számkódját, ami a képek kezeléséhez kell.
			/// </summary>
			/// <returns></returns>
			public Bitmap Iránykép() { return képkészlet[I.ToInt()]; }
			/// <summary>
			/// Lerakja az adott színű követ a pályán a robot helyére.
			/// </summary>
			/// <param name="szín"></param>
			public void Lerak(int szín = fekete)
			{
				if (pálya.MiVanItt(H) != üres)
					MessageBox.Show(név + ": Nem tudom a kavicsot lerakni, mert van lerakva kavics!");
				else if (kődb[szín - 2] <= 0)
					MessageBox.Show(név + ": Nem tudom a kavicsot lerakni, mert nincs kavicsom!");
				else
				{
					pálya.LegyenItt(H, szín);
					--kődb[szín - 2];

					óra.Tak();
					pálya.Refresh();
					monitorpanel.Frissít();
				}

				pálya.Refresh();
			}
			/*			public void Elhajít(int tav, int szín = hó)
						{
							int d = 0;
							Vektor G = new Vektor(H);
							while (pálya.MiVanItt(G) != -1 && pálya.MiVanItt(G) != 1 && d<tav)
							{
								G.Add(I);
								d++;
							}
							if (d == tav) pálya.LegyenItt(G, szín);
							else if (pálya.MiVanItt(G) == 1)
							{
								G.Rem(I);
								pálya.LegyenItt(G, szín);
							}
							else { }


							pálya.Refresh();
						}
			*/
			/// <summary>
			/// Felveszi azt, amin éppen áll -- feltéve ha az nem fal, stb.
			/// </summary>
			public void Felvesz()
			{
				if (pálya.MiVanItt(H) > fal)
				{
					++kődb[pálya.MiVanItt(H) - 2];
					pálya.LegyenItt(H, üres);
					óra.Tak();
				}
				else
					MessageBox.Show(név + ": Nem tudom a kavicsot felvenni!");

				pálya.Refresh();
				monitorpanel.Frissít();
			}
			/// <summary>
			/// Megadja, hogy kavicson áll-e a robot.
			/// </summary>
			/// <returns></returns>
			public bool VanKavics() { return pálya.MiVanItt(H) > fal; }
			/// <summary>
			/// Megadja, hogy min áll a robot
			/// </summary>
			/// <returns></returns>
			public int MiVanItt() { return pálya.MiVanItt(H); }
			/// <summary>
			/// Megadja, hogy mi van a robot előtt
			/// </summary>
			/// <returns></returns>
			public int MiVanElőttem()
			{
				/*  1, ha fal van előtte
				 * -1, ha kilép a pályáról;*/

				H.Hozzáad(I); // Ahova lépni készül.
				if (pálya.BenneVan(H))
				{
					int result = pálya.MiVanItt(H);
					H.Kisebbít(I);
					return result;
				}
				else { H.Kisebbít(I); return -1; }
			}
			public int UltrahangSzenzor()
			{
				int d = 0;
				Vektor J = new Vektor(H);
				while (pálya.MiVanItt(J) != -1 && pálya.MiVanItt(J) != 1)
				{
					J.Hozzáad(I);
					d++;
				}
				if (pálya.MiVanItt(J) == 1) return d; else return -1;
			}
			public int Hőmérő() { return pálya.Hőmérséklet(H); }
		}
	}
}