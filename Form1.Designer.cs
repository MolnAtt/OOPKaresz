using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
//using System.Reflection;
using System.Threading;

// Karesz OOP-bban, több robothoz előkészítve
namespace Karesz
{
	partial class Form1
	{

		#region Konstansok

		const int várakozás = 5;

		//pályatartalom
		const int üres = 0;
		const int fal = 1;
		const int nincs_pálya = -1;
		const int láva = 7;

		//forgásirány
		const int jobbra = 1;
		const int balra = -1;

		//színek				
		const int fekete = 2;
		const int piros = 3;
		const int zöld = 4;
		const int sárga = 5;
		const int hó = 6;
		const int víz = 8;
		#endregion

		#region Statikus objektumok
		// robotok
		static List<Robot> robotlista = new List<Robot>();
		/*{
			new Robot("Karesz", new Bitmap[4]
					{
						Properties.Resources.Karesz0,
						Properties.Resources.Karesz1,
						Properties.Resources.Karesz2,
						Properties.Resources.Karesz3
					},
				new Vektor(5,28),
				new Vektor(0,-1),
				new int[5]{100, 20, 20, 20, 0}
				)
		};*/
		static Óra óra = new Óra();
		static Random véletlen = new Random();
		private static Pálya pálya = new Pálya();
		private static MonitorPanel monitorpanel = new MonitorPanel();
		#endregion

		#region Form-komponensek 
		private Button startgomb = new Button
		{
			Location = new Point(pálya.Size.Width + 20, 10),
			Margin = new Padding(4, 4, 4, 4),
			Name = "startgomb",
			Size = new Size(monitorpanel.Size.Width, 48),
			TabIndex = 9,
			Text = "Start",
			UseVisualStyleBackColor = true
		};
		private Label pályafelirat = new Label
		{
			AutoSize = true,
			Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238))),
			Location = new Point(20 + pálya.Size.Width, pálya.Size.Height - 50),
			Margin = new Padding(4, 0, 4, 0),
			Name = "pályafelirat",
			Size = new Size(50, 20),
			TabIndex = 25,
			Text = "Pálya:",
		};
		private TextBox pályaválasztó = new TextBox
		{
			Location = new Point(20 + pálya.Size.Width + 55, pálya.Size.Height - 50),
			Margin = new Padding(4, 4, 4, 4),
			Name = "pálya",
			Size = new Size(monitorpanel.Size.Width - 55, 30),
			TabIndex = 26,
		};
		private Button Betölt = new Button
		{
			Location = new Point(20 + pálya.Size.Width, pálya.Size.Height - 20),
			Margin = new Padding(4, 4, 4, 4),
			Name = "Betölt",
			Size = new Size(monitorpanel.Size.Width, 30),
			TabIndex = 28,
			Text = "Pályát betölt",
			UseVisualStyleBackColor = true
		};

		#endregion


		public Form1()
		{
			((System.ComponentModel.ISupportInitialize)(pálya)).BeginInit();
			monitorpanel.SuspendLayout();
			SuspendLayout();

			// Controlok
			Controls.Add(Betölt);
			Controls.Add(pályaválasztó);
			Controls.Add(pályafelirat);

			// Form1
			AutoScaleDimensions = new SizeF(8F, 16F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = false;
			ClientSize = new Size(monitorpanel.Size.Width+pálya.Size.Width+35, pálya.Size.Height+15);
			Controls.Add(monitorpanel);
			Controls.Add(startgomb);
			Controls.Add(pálya);
			Margin = new Padding(4, 4, 4, 4);
			Name = "Form1";
			Text = "Karesz2";
			((System.ComponentModel.ISupportInitialize)(pálya)).EndInit();
			monitorpanel.ResumeLayout(false);
			monitorpanel.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

			// Eseménykezelés
			pálya.Paint += new PaintEventHandler(pálya_Paint);
			pálya.MouseDown += new MouseEventHandler(pálya_MouseDown);
			Betölt.Click += new System.EventHandler(Betölt_Click);
			startgomb.Click += new System.EventHandler(startgomb_Click);

			pálya.Betölt();
		}

		/// <summary>
		/// Végrehajtja a feladatot
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void startgomb_Click(object sender, EventArgs e)
		{
			startgomb.Enabled = false;
			monitorpanel.Enged(false);
			FELADAT();
			monitorpanel.Enged(true);
			startgomb.Enabled = true;
			MessageBox.Show("Vége!");
		}

		/// <summary>
		/// Lerajzolja a Pályánál található függvénnyel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pálya_Paint(object sender, PaintEventArgs e){pálya.Rajz(pálya, e);}

		/// <summary>
		/// Ha valaki a pályára kattint, akkor Kareszt leteszi oda.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pálya_MouseDown(object sender, MouseEventArgs e)
		{
			monitorpanel.megfigyeltrobot.Teleport(e.X/pálya.lépték.X, e.Y/ pálya.lépték.Y);
			pálya.Refresh();
			monitorpanel.Frissít();
		}

		private void Betölt_Click(object sender, EventArgs e){pálya.Betölt(pályaválasztó.Text+".txt");}

		


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}


	}
}