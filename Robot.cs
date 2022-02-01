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
			public static List<Robot> lista = new List<Robot>();
			public string Név { get; private set; }
			private Vektor H;
			private Vektor I = new Vektor(0, 1);
			private int[] kődb;
			private Bitmap[] képkészlet;
			/// <summary>
			/// Teljes konstruktor: Létrehoz egy új robotot a megadott névvel, képkészlettel, pozícióval, iránnyal és induló kövek számával.
			/// </summary>
			/// <param name="adottnév">A robot neve</param>
			/// <param name="képek">A képkészlet a Resources mappából</param>
			/// <param name="Pozíció">indulási pozíció</param>
			/// <param name="Irány">kezdőirány</param>
			/// <param name="indulókövek">induláskor a zsebeiben lévő kövek száma</param>
			public Robot(string adottnév, Bitmap[] képek, Vektor Pozíció, Vektor Irány, int[] indulókövek)
			{
				Név = adottnév;
				H = Pozíció;
				I = Irány;
				képkészlet = képek;
				kődb = indulókövek;

				Robot.lista.Add(this);
			}
			/// <summary>
			/// Létrehoz egy új robotot a megadott névvel és induló kövek számával az 5,28 helyen északra nézvén.
			/// </summary>
			/// <param name="adottnév">A robot neve</param>
			/// <param name="indulókövek">induláskor a zsebeiben lévő kövek száma</param>
			public Robot(string adottnév, int[] indulókövek)
				: this(adottnév, new Bitmap[4]
						{
							Properties.Resources.Karesz0,
							Properties.Resources.Karesz1,
							Properties.Resources.Karesz2,
							Properties.Resources.Karesz3
						},
						new Vektor(5, 28),
						new Vektor(0, -1),
						indulókövek){}
			/// <summary>
			/// Létrehoz egy új üres zsebű kék robotot a megadott névvel az 5,28 helyen északra nézvén.
			/// </summary>
			/// <param name="adottnév">A robot neve</param>
			public Robot(string adottnév): this(adottnév, new int[5] { 0, 0, 0, 0, 0 }){}
			/// <summary>
			/// Elhelyezi a Robotot a megadott helyre.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			public void Teleport(int x, int y) => (H.X, H.Y) = (x, y);
			/// <summary>
			/// Visszaadja a robot koordinátáit.
			/// </summary>
			/// <returns></returns>
			public Vektor HolVan() => new Vektor(H);
			/// <summary>
			/// Megadja, hogy az adott színből mennyi köve van a robotnak.
			/// </summary>
			/// <param name="szín"></param>
			/// <returns></returns>
			public int Mennyi(int szín) => kődb[szín - 2];
			/// <summary>
			/// Lépteti a robotot a megfelelő irányba.
			/// </summary>
			public void Lép()
			{
				Thread.Sleep(várakozás);
				H += I; // Ahova lépni készül.
				if (pálya.BenneVan(H) && pálya.MiVanItt(H) != fal)
					idő++;
				else
				{
					MessageBox.Show(Név + ": Nem tudok lépni!");
					H -= I;
				}
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
				I.Forgat(forgásirány);
				idő++;
				pálya.Refresh();
				monitorpanel.Frissít();
			}
			/// <summary>
			/// Visszaadja a sebességvektor számkódját, ami a képek kezeléséhez kell.
			/// </summary>
			/// <returns></returns>
			public Bitmap Iránykép() => képkészlet[I.ToInt()];
			/// <summary>
			/// Lerakja az adott színű követ a pályán a robot helyére.
			/// </summary>
			/// <param name="szín"></param>
			public void Lerak(int szín = fekete)
			{
				if (pálya.MiVanItt(H) != üres)
					MessageBox.Show(Név + ": Nem tudom a kavicsot lerakni, mert van lerakva kavics!");
				else if (kődb[szín - 2] <= 0)
					MessageBox.Show(Név + ": Nem tudom a kavicsot lerakni, mert nincs kavicsom!");
				else
				{
					pálya.LegyenItt(H, szín);
					--kődb[szín - 2];
					idő++;
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
					idő++;
				}
				else
					MessageBox.Show(Név + ": Nem tudom a kavicsot felvenni!");

				pálya.Refresh();
				monitorpanel.Frissít();
			}
			/// <summary>
			/// Megadja, hogy kavicson áll-e a robot.
			/// </summary>
			/// <returns></returns>
			public bool VanKavics() => pálya.MiVanItt(H) > fal;
			/// <summary>
			/// Megadja, hogy min áll a robot
			/// </summary>
			/// <returns></returns>
			public int MiVanItt() => pálya.MiVanItt(H);
			/// <summary>
			/// Megadja, hogy mi van a robot előtt -- (1 = fal, -1 = kilép)
			/// </summary>
			/// <returns></returns>
			int MiVanElőttem(Vektor Itt) => pálya.BenneVan(Itt) ? pálya.MiVanItt(Itt) : -1;
			public int MiVanElőttem() => MiVanElőttem(H + I);
			public int UltrahangSzenzor()
			{
				int d = 0;
				Vektor J = new Vektor(H);
				while (pálya.MiVanItt(J) != -1 && pálya.MiVanItt(J) != 1)
				{
					J+=I;
					d++;
				}
				return pálya.MiVanItt(J) == 1 ? d : -1;
			}
			public int Hőmérő() => pálya.Hőmérséklet(H);
		}
	}
}