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
			Form1 szülőform;
			public static int megfigyeltindex = 0;
			public static Robot megfigyelt;
			public static List<Robot> lista = new List<Robot>();
			public string Név { get; private set; }
			public Vektor h;
			public Vektor H { get => h; }		
			private Vektor v = new Vektor(0, 1);
			private int[] kődb;
			private Bitmap[] képkészlet;
			/// <summary>
			/// Teljes konstruktor: Létrehoz egy új robotot a megadott névvel, képkészlettel, pozícióval, iránnyal és induló kövek számával.
			/// </summary>
			/// <param name="név">A robot neve</param>
			/// <param name="képkészlet">A képkészlet a Resources mappából</param>
			/// <param name="H">indulási pozíció</param>
			/// <param name="I">kezdőirány</param>
			/// <param name="kődb">induláskor a zsebeiben lévő kövek száma</param>
			public Robot(string név, Bitmap[] képkészlet, Vektor h, Vektor v, int[] kődb, Form1 szülőform)
			{
				this.Név = név;
				this.h = h;
				this.v = v;
				this.képkészlet = képkészlet;
				this.kődb = kődb;
				this.szülőform = szülőform;

				Robot.lista.Add(this);
			}

			/// <summary>
			/// Létrehoz egy új robotot a megadott névvel és induló kövek számával az 5,28 helyen északra nézvén.
			/// </summary>
			/// <param name="adottnév">A robot neve</param>
			/// <param name="indulókövek">induláskor a zsebeiben lévő kövek száma</param>
			public Robot(string adottnév, int[] indulókövek, Form1 szülőform)
				: this(adottnév, new Bitmap[4]
						{
							Properties.Resources.Karesz0,
							Properties.Resources.Karesz1,
							Properties.Resources.Karesz2,
							Properties.Resources.Karesz3
						},
						new Vektor(5, 28),
						new Vektor(0, -1),
						indulókövek,
						szülőform){}
			/// <summary>
			/// Létrehoz egy új üres zsebű kék robotot a megadott névvel az 5,28 helyen északra nézvén.
			/// </summary>
			/// <param name="adottnév">A robot neve</param>
			public Robot(string adottnév, Form1 szülőform): 
				this(adottnév, new int[5] { 0, 0, 0, 0, 0 }, szülőform){}
			/// <summary>
			/// Elhelyezi a Robotot a megadott helyre.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			public void Teleport(int x, int y) => 
				(h.X, h.Y) = (x, y);
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
				h += v; // Ahova lépni készül.
				if (pálya.BenneVan(H) && pálya.MiVanItt(H) != fal)
					idő++;
				else
				{
					MessageBox.Show(Név + ": Nem tudok lépni!");
					h -= v;
				}
				pálya.Refresh();
				szülőform.Frissít();
			}
			/// <summary>
			/// Elforgatja a robotot a megadott irányban. (Csak normális irányokra reagál.)
			/// </summary>
			/// <param name="forgásirány"></param>
			public void Fordul(int forgásirány)
			{
				Thread.Sleep(várakozás);
				v.Forgat(forgásirány);
				idő++;
				pálya.Refresh();
				szülőform.Frissít();
			}
			/// <summary>
			/// Visszaadja a sebességvektor számkódját, ami a képek kezeléséhez kell.
			/// </summary>
			/// <returns></returns>
			public Bitmap Iránykép() => képkészlet[v.ToInt()];
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
					szülőform.Frissít();
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
				szülőform.Frissít();
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
			public int MiVanElőttem() => MiVanElőttem(H + v);
			public int UltrahangSzenzor()
			{
				int d = 0;
				Vektor J = new Vektor(H);
				while (pálya.MiVanItt(J) != -1 && pálya.MiVanItt(J) != 1)
				{
					J+=v;
					d++;
				}
				return pálya.MiVanItt(J) == 1 ? d : -1;
			}
			public int Hőmérő() => pálya.Hőmérséklet(H);
			static int modulo_add(int honnan, int mennyit, int mod) => 
				(honnan + mennyit) % mod;
			public static void Megfigyelt_léptetése(int l) => 
				Robot.megfigyelt = Robot.lista[modulo_add(megfigyeltindex, l, lista.Count)];
		}
	}
}