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
			#region Láncolt robotgyűrű kialakítása szálkezeléssel együtt.

			public static int ok_száma = 0;
			public static Robot aki_kezd;
            public static Robot akit_kiválasztottak;
			public static Robot First { get => Robot.aki_kezd; }
			public static Robot Last { get => First.prev; }

			public static Robot Get(string n)
			{
				Robot akt_robot = Robot.aki_kezd;
				if (akt_robot.Név == n)
					return akt_robot;
				
				akt_robot = akt_robot.next;
				
				while (!(akt_robot.Név == n || akt_robot == Robot.aki_kezd))
					akt_robot = akt_robot.next;

				if (akt_robot != Robot.aki_kezd)
					return akt_robot;
				throw new Exception("Ilyen nevű robot nincs!");
			}

			Robot prev, next;


			void Beszúr_ez_elé(Robot ez)
			{
				this.next = ez;
				this.prev = ez.next;
				this.next.prev = this;
				this.prev.next = this;
				Robot.ok_száma++;
			}
			/// <summary>
			/// ha már csak egyelemű a lista, akkor hatástalan.
			/// </summary>
			void Kifűz()
			{
				if (this == Robot.aki_kezd)
					Robot.aki_kezd = this.next;
				this.next.prev = this.prev;
				this.prev.next = this.next;
				Robot.ok_száma--;
			}
			void Végére_fűz() => Beszúr_ez_elé(Robot.aki_kezd);

			public static void Megfigyelt_léptetése_előre() => Robot.akit_kiválasztottak = Robot.akit_kiválasztottak.next;
			public static void Megfigyelt_léptetése_hátra() => Robot.akit_kiválasztottak = Robot.akit_kiválasztottak.prev;
			public static void Megfigyelt_léptetése(int l)
			{
				if (0 < l)
					for (int i = 0; i < l; i++)
						Megfigyelt_léptetése_előre();
				else if (l < 0)
					for (int i = 0; i < -l; i++)
						Megfigyelt_léptetése_hátra();
			}

			/// <summary>
			/// foreach-ciklus-szerűség a gyűrűre. Delegate-tel kell használni, aminek egyetlen paramétere egy robot.
			/// </summary>
			/// <param name="a"></param>
			public static void okra_mind(Action<Robot> a)
			{
				if (0 < Robot.ok_száma)
				{
					Robot akt_robot = Robot.aki_kezd;
					a(akt_robot);
					akt_robot = akt_robot.next;
					while (akt_robot != Robot.aki_kezd)
					{
						a(akt_robot);
						akt_robot = akt_robot.next;
					}
				}
			}

			public static Robot Keres(Func<Robot, bool> predikátum)
			{
				if (predikátum(Robot.aki_kezd))
					return Robot.aki_kezd;
				Robot aktuális_robot = Robot.aki_kezd.next;
				while (aktuális_robot != Robot.aki_kezd.next)
				{
					if (predikátum(aktuális_robot))
						return aktuális_robot;
					aktuális_robot = aktuális_robot.next;
				}
				return null;
			}

			public static bool ok_közül_valaki_még_dolgozik() => null != Robot.Keres(r => !r.kész);

			void Cselekvés_vége()
			{
				this.szülőform.Frissít();
				this.körének_vége();
			}
			void körének_vége()
			{
				if (1 < Robot.ok_száma)
					this.körének_átadása();
			}
			void körének_átadása()
			{
				this.vár = true;
				this.next.Te_jössz();
				this.thread.Suspend();

				// valójában itt kezdődnek a körök!
				Thread.Sleep(100); // Ez arra kell, hogy amikor megkapja a körét, akkor hagyjon egy kis időt az előzőnek, hogy felfüggessze magát. 
			}


			void FELADAT_BUROK()
			{
				this.feladat();
				this.kész = true;
				this.Kiszáll();
			}

			private void Kiszáll()
			{
				Robot aki_fog_jönni = this.next;
				this.Kifűz();
				aki_fog_jönni.Te_jössz();
			}



			#endregion

			public override string ToString() => this.Név;

			#region Instanciák tulajdonságai

			public string Név { get; private set; }
			Bitmap[] képkészlet;
			public Vektor h;
			public Vektor H { get => h; }
			Vektor v;
			int[] kődb;
			Form1 szülőform;
			Pálya pálya;
			public Action feladat;
			bool kész;
			bool vár;
			Thread thread;
			#endregion

			#region Konstruktorok

			/// <summary>
			/// Teljes konstruktor: Létrehoz egy új robotot a megadott névvel, képkészlettel, pozícióval, iránnyal és induló kövek számával.
			/// </summary>
			/// <param name="név">A robot neve</param>
			/// <param name="képkészlet">A képkészlet a Resources mappából</param>
			/// <param name="H">indulási pozíció</param>
			/// <param name="I">kezdőirány</param>
			/// <param name="kődb">induláskor a zsebeiben lévő kövek száma</param>
			/// <param name="szülőform">az eredeti form, a visszahivatkozáshoz kell</param>
			/// <param name="pálya">a pálya, amin a robot mozog</param>
			public Robot(string név, Bitmap[] képkészlet, Vektor h, Vektor v, int[] kődb, Form1 szülőform, Pálya pálya)
			{
				this.Név = név;
				this.h = h;
				this.v = v;
				this.képkészlet = képkészlet;
				this.kődb = kődb;
				this.szülőform = szülőform;
				this.pálya = pálya;
				this.kész = false;
				this.vár = false;
				this.thread = new Thread(new ThreadStart(FELADAT_BUROK));

				if (Robot.aki_kezd == null)
					Robot.aki_kezd = this;
				if (Robot.akit_kiválasztottak == null)
					Robot.akit_kiválasztottak = this;

				this.Végére_fűz();
				szülőform.Frissít();
			}



			public Robot(string adottnév, Form1 szülőform, int[] indulókövek, Vektor hely, Vektor sebesség) 
				: this(adottnév, new Bitmap[4]
						{
							Properties.Resources.Karesz0,
							Properties.Resources.Karesz1,
							Properties.Resources.Karesz2,
							Properties.Resources.Karesz3
						},
						hely,
						sebesség,
						indulókövek,
						szülőform,
						szülőform.pálya)
			{ }

			public Robot(string adottnév, Form1 szülőform, int[] indulókövek, int x, int y, int f) :
				this(adottnév, szülőform, indulókövek, new Vektor(x, y), new Vektor(f))
			{ }

			public Robot(string adottnév, Form1 szülőform, int x, int y, int f) :
				this(adottnév, szülőform,  new int[] { 0, 0, 0, 0, 0 }, x, y, f)
			{ }
			public Robot(string adottnév, Form1 szülőform, int x, int y) :
				this(adottnév, szülőform, x, y, 0)
			{ }

			internal static void Játék_elindítása() => Robot.aki_kezd.Te_jössz();

			private void Te_jössz()
			{
				if (!kész)
					Start_or_Resume();
			}

			private void Start_or_Resume()
			{
				if (vár)
					this.thread.Resume();
				else
					this.thread.Start();
			}

			public Robot(string adottnév, Form1 szülőform) :
				this(adottnév, szülőform, 5, 28)
			{ }



			#endregion

			#region Motorok

			/// <summary>
			/// Elhelyezi a Robotot a megadott helyre.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			public void Teleport(int x, int y) =>
				(h.X, h.Y) = (x, y);

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
				Cselekvés_vége();
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
				Cselekvés_vége();
			}

			/// <summary>
			/// Lerakja az adott színű követ a pályán a robot helyére.
			/// </summary>
			/// <param name="szín"></param>
			public void Lerak(int szín = fekete)
			{
				if (pálya.MiVanItt(H) != üres)
					MessageBox.Show($"{Név}: Nem tudom a kavicsot lerakni, mert van lerakva kavics!");
				else if (kődb[szín - 2] <= 0)
					MessageBox.Show($"{Név}: Nem tudom a kavicsot lerakni, mert nincs {színnév[szín]} színű kavicsom!");
				else
				{
					pálya.LegyenItt(H, szín);
					--kődb[szín - 2];
					idő++;
				}
				Cselekvés_vége();
			}


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

				Cselekvés_vége();
			}

			#endregion

			#region Szenzorok

			/// <summary>
			/// Megadja, hogy az adott színből mennyi köve van a robotnak.
			/// </summary>
			/// <param name="szín"></param>
			/// <returns></returns>
			public int Mennyi(int szín) => kődb[szín - 2];

			/// <summary>
			/// Megadja, hogy kavicson áll-e a robot.
			/// </summary>
			/// <returns></returns>
			public bool VanKavics() =>
				pálya.MiVanItt(H) > fal;

			/// <summary>
			/// Megadja, hogy min áll a robot
			/// </summary>
			/// <returns></returns>
			public int MiVanItt() =>
				pálya.MiVanItt(H);

			/// <summary>
			/// Megadja, hogy mi van a robot előtt az adott helyen -- (1 = fal, -1 = kilép)
			/// </summary>
			/// <returns></returns>
			int MiVanElőttem(Vektor Itt) =>
				pálya.BenneVan(Itt) ? pálya.MiVanItt(Itt) : -1;

			/// <summary>
			/// megadja, hogy mi van a robot előtt
			/// </summary>
			/// <returns></returns>
			public int MiVanElőttem() =>
				MiVanElőttem(H + v);

			/// <summary>
			/// megadja, hogy milyen messze van a robot előtti legközelebbi olyan objektum, amely vissza tudja verni a hangot (per pill. másik robot vagy fal)
			/// </summary>
			/// <returns></returns>
			public int UltrahangSzenzor()
			{
				int d = 0;
				Vektor J = new Vektor(H);
				while (pálya.BenneVan(J) && !(pálya.MiVanItt(J) == 1 || Más_robot_van_itt(J)))
				{
					J += v;
					d++;
				}
				return pálya.BenneVan(J)? d : -1;
			}

			private bool Más_robot_van_itt(Vektor v)
			{
				Robot akt_robot = this.next;
				while (akt_robot != this && akt_robot.H != v)
					akt_robot = akt_robot.next;
				return akt_robot != this;
			}

			HashSet<Vektor> Más_robotok_helyei()
			{
				HashSet<Vektor> result = new HashSet<Vektor>();
				Robot akt_robot = this.next;
				while (akt_robot != this)
					result.Add(akt_robot.H);
				return result;
			}
			public int Hőmérő() =>
				pálya.Hőmérséklet(H);
			#endregion

			#region Formkezeléshez szolgáló metódusok

			/// <summary>
			/// Visszaadja a sebességvektor számkódját, ami a képek kezeléséhez kell.
			/// </summary>
			/// <returns></returns>
			public Bitmap Iránykép() => képkészlet[v.ToInt()];

			#endregion


		}
	}
}