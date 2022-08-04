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
			#region Valami ami nem ez

			public static List<Robot> lista = new List<Robot>();
			public static int ok_száma { get => Robot.lista.Count; }
			public static int megfigyeltindex;
			public static Robot akit_kiválasztottak { get => lista[megfigyeltindex]; }

			public static Robot Get(string n) => Robot.lista.First(x => x.Név == n);

			static int sajatmodulo(int x, int m) => x < 0 ? sajatmodulo(x + m, m) : x % m;

			public static void Megfigyelt_léptetése_előre() => Robot.megfigyeltindex = sajatmodulo(Robot.megfigyeltindex + 1, Robot.lista.Count);
			public static void Megfigyelt_léptetése_hátra() => Robot.megfigyeltindex = sajatmodulo(Robot.megfigyeltindex - 1, Robot.lista.Count);


			public static bool ok_közül_valaki_még_dolgozik() => -1 < Robot.lista.FindIndex(r => !r.Kész);

			void Cselekvés_vége()
			{
				if (1 < Robot.lista.Count)
					this.thread.Suspend();
			}

			#endregion

			public override string ToString() => this.Név;

			#region Instanciák tulajdonságai

			public string Név { get; private set; }
			Bitmap[] képkészlet;
			public Vektor h;
			public Vektor H { get => h; }
			Vektor helyigény;
			Vektor v;
			int[] kődb;
			Form1 szülőform;
			Pálya pálya;
			Action feladat;
			public Action Feladat 
			{
				get => feladat;
				set 
				{
					feladat = value;
					thread = new Thread(new ThreadStart(feladat));
				}
			}
			Thread thread;

			public bool Kész { get => thread.ThreadState == ThreadState.Stopped; }
			public bool Vár { get => thread.ThreadState == ThreadState.Suspended; }

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

				if (0 == Robot.lista.Count)
					Robot.megfigyeltindex = 0;

				Robot.lista.Add(this);
				// szülőform.Frissít();
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
			public Robot(string adottnév, Form1 szülőform) :
				this(adottnév, szülőform, 5, 28)
			{ }


			static void ok_elindítása()
			{
				foreach (Robot robot in Robot.lista)
					if (!robot.Kész)
						robot.Start_or_Resume();
			}

			static void Kör() 
			{

			}
			public static void Játék() 
			{
				const int várakozási_idő = 100;

				Robot.ok_elindítása();

				Thread.Sleep(várakozási_idő);
				while (Robot.ok_közül_valaki_még_nincs_kész())
				{
					if (Robot.ok_mindegyike_várakozik_vagy_kész())
					{
						Robot.lista[0].szülőform.Frissít();
						Robot.ok_elindítása();
					}
					Thread.Sleep(várakozási_idő);
				}
				MessageBox.Show("game over");
			}

			internal static bool ok_közül_valaki_még_nincs_kész()
			{
				foreach ( Robot játékos in Robot.lista)
					if (!játékos.Kész)
						return true;
				return false;

				Robot.lista.Exists(r => !r.Kész);
			}

			internal static bool ok_mindegyike_várakozik_vagy_kész()
			{
				foreach (Robot játékos in Robot.lista)
					if (!(játékos.Vár || játékos.Kész))
						return false;
				return true;
				Robot.lista.TrueForAll(r => r.Kész || r.Vár);
			}

			void Start_or_Resume()
			{
				if (this.thread.ThreadState == ThreadState.Unstarted)
					this.thread.Start();
				else if (this.Vár)
					this.thread.Resume();
			}
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
			public void Lépj()
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
			public void Fordulj(int forgásirány)
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
			public void Tegyél_le_egy_kavicsot(int szín = fekete)
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
			public void Vegyél_fel_egy_kavicsot()
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

			public void Lőjj()
			{

				Robot lövedék = new Robot("lövedék", new Bitmap[] { 
						Properties.Resources.Karesz0,
						Properties.Resources.Karesz1,
						Properties.Resources.Karesz2,
						Properties.Resources.Karesz3}, this.H + this.v, this.v, new int[] { 0,0,0,0,0}, this.szülőform, this.pálya);
				//			public Robot(string név, Bitmap[] képkészlet, Vektor h, Vektor v, int[] kődb, Form1 szülőform, Pálya pálya)
				lövedék.feladat = delegate ()
				{
					while (!lövedék.Előtt_fal_van())
					{
						lövedék.Lépj();
					}
				};
				Cselekvés_vége();
			}

			#endregion

			#region Szenzorok

			/// <summary>
			/// Megadja, hogy az adott színből mennyi köve van a robotnak.
			/// </summary>
			/// <param name="szín"></param>
			/// <returns></returns>
			public int Köveinek_száma_ebből(int szín) => kődb[szín - 2];

			/// <summary>
			/// Megadja, hogy kavicson áll-e a robot.
			/// </summary>
			/// <returns></returns>
			public bool Alatt_van_kavics() =>
				pálya.MiVanItt(H) > fal;

			/// <summary>
			/// Megadja, hogy min áll a robot
			/// </summary>
			/// <returns></returns>
			public int Alatt_ez_van() =>
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
			/// Pontosan akkor igaz, ha a robot előtt fal van.
			/// </summary>
			/// <returns></returns>
			public bool Előtt_fal_van() => this.MiVanElőttem() == fal;
			/// <summary>
			/// Pontosan akkor igaz, ha a robot a pálya szélén van és a következő lépéssel kizuhanna a pályáról.
			/// </summary>
			/// <returns></returns>
			public bool Ki_fog_lépni_a_pályáról() => this.MiVanElőttem() == nincs_pálya;

			/// <summary>
			/// megadja, hogy milyen messze van a robot előtti legközelebbi olyan objektum, amely vissza tudja verni a hangot (per pill. másik robot vagy fal)
			/// </summary>
			/// <returns></returns>
			public int UltrahangSzenzor() => Akadálytávolság(H, v);
			public (int, int, int) SzélesUltrahangSzenzor()
				=> (Akadálytávolság(H + v.Forgatott(balra), v), Akadálytávolság(H, v), Akadálytávolság(H + v.Forgatott(jobbra), v));


			int Akadálytávolság(Vektor hely, Vektor sebesség)
			{
				int d = 0;
				Vektor J = new Vektor(hely);
				while (pálya.BenneVan(J) && !(pálya.MiVanItt(J) == 1 || Más_robot_van_itt(J)))
				{
					J += sebesség;
					d++;
				}
				return pálya.BenneVan(J) ? d : -1;
			}

			private bool Más_robot_van_itt(Vektor v) => false;

			HashSet<Vektor> Más_robotok_helyei() => Robot.lista.Select(x => x.H).ToHashSet();
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