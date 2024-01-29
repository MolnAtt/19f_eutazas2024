using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace eutazas
{
	internal class Program
	{

		class Adat
		{
			public int megallo;
			public DateTime datum_felszallas;
			public string id;
			public string tipus;
			public DateTime datum_berlet;
			public int jegyszam;

			public Adat(int megallo, DateTime datum_felszallas, string id, string tipus, DateTime datum_berlet, int jegyszam)
			{
				this.megallo = megallo;
				this.datum_felszallas = datum_felszallas;
				this.id = id;
				this.tipus = tipus;
				this.datum_berlet = datum_berlet;
				this.jegyszam = jegyszam;
			}
		}

		static void Main(string[] args)
		{
			string[] sorok = File.ReadAllLines("utasadat.txt");

			List<Adat> lista = new List<Adat>();

			foreach (string sor in sorok)
			{
				string[] t = sor.Split(' ');

				string tipus = t[3];

				string datumstr = t[1]; // 20190326-0716
				DateTime datum_felszallas = new DateTime(
					int.Parse(t[1].Substring(0, 4)),
					int.Parse(t[1].Substring(4, 2)),
					int.Parse(t[1].Substring(6, 2)),
					int.Parse(t[1].Substring(9, 2)),
					int.Parse(t[1].Substring(11, 2)),
					0);

				if (tipus == "JGY")
				{
					lista.Add(new Adat(int.Parse(t[0]), datum_felszallas, t[2], t[3], new DateTime(), int.Parse(t[4])));
				}
				else
				{
					lista.Add(new Adat(int.Parse(t[0]), datum_felszallas, t[2], t[3], new DateTime(
						int.Parse(t[4].Substring(0, 4)),
						int.Parse(t[4].Substring(4, 2)),
						int.Parse(t[4].Substring(6, 2)),
						23,59,59,1
						), -1));
				}
			}

            Console.WriteLine($"2. balbabla {lista.Count} ");


			Console.WriteLine($"3. feladat: {lista.Count(x => (x.tipus == "JGY" && x.jegyszam == 0) || (x.tipus != "JGY" && x.datum_berlet < x.datum_felszallas))}");


			Megállónként(lista);

			var nekik_irunk = lista.Where(x => x.tipus !="JGY" &&
				napokszama(x.datum_felszallas.Year, x.datum_felszallas.Month, x.datum_felszallas.Day,x.datum_berlet.Year,x.datum_berlet.Month,x.datum_berlet.Day) <= 3 
			&& 0 <= napokszama(x.datum_felszallas.Year, x.datum_felszallas.Month, x.datum_felszallas.Day, x.datum_berlet.Year, x.datum_berlet.Month, x.datum_berlet.Day));

			using (StreamWriter w = new StreamWriter("figyelmeztetes.txt"))
			{
				foreach (var item in nekik_irunk)
				{
					w.WriteLine($"{item.id} {item.datum_berlet.ToString("yyyy-MM-dd")}");
				}
			}
        }


		/*
		Függvény napokszama(e1:egész, h1:egész, n1: egész, e2:egész,h2: egész, n2: egész): egész
			h1 = (h1 + 9) MOD 12
			e1 = e1 - h1 DIV 10
			d1 = 365*e1 + e1 DIV 4 - e1 DIV 100 + e1 DIV 400 + (h1*306 + 5) DIV 10 + n1 - 1
			h2 = (h2 + 9) MOD 12
			e2 = e2 - h2 DIV 10
			d2 = 365*e2 + e2 DIV 4 - e2 DIV 100 + e2 DIV 400 + (h2*306 + 5) DIV 10 + n2 - 1
			napokszama:= d2-d1
		Függvény vége
		*/

		static int napokszama(int e1, int h1, int n1, int e2, int h2, int n2)
		{
			h1 = (h1 + 9) % 12;
			e1 = e1 - h1 / 10;
			int d1 = 365 * e1 + e1 / 4 - e1 / 100 + e1 / 400 + (h1 * 306 + 5) / 10 + n1 - 1;
			h2 = (h2 + 9) % 12;
			e2 = e2 - h2 / 10;
			int d2 = 365 * e2 + e2 / 4 - e2 / 100 + e2 / 400 + (h2 * 306 + 5) / 10 + n2 - 1;
			return d2 - d1;
		}



		private static void Megállónként(List<Adat> lista)
		{
			// Group by feladat -----> SZÓTÁR

			// megálló : db
			// int : int

			Dictionary<int, int> szotar = new Dictionary<int, int>();

			foreach (Adat adat in lista)
			{
				if (szotar.ContainsKey(adat.megallo))
				{
					szotar[adat.megallo] += 1;
				}
				else
				{
					szotar[adat.megallo]  = 1;
				}
			}

			// valahogy így fog kinézni:
			// 0: 5,
			// 4: 9,
			// 5: 3,
			// ...


			//foreach (int megallo in szotar.Keys)
			//{
			//	Console.WriteLine($"kulcs: {megallo} --> {szotar[megallo]} db");
			//}

			int bestmo = 0;

			foreach (int megallo in szotar.Keys)
			{
				if ( szotar[bestmo] < szotar[megallo])
				{
					bestmo = megallo;
				}
			}


            Console.WriteLine($"A {bestmo} megállóhelyen szálltak fel a legtöbben, mégpedig ennyien: {szotar[bestmo]} fő");

			int kedvezmenyes = lista.Count(x => (x.tipus == "TAB" || x.tipus == "NYB") && x.datum_felszallas <= x.datum_berlet);
			int ingyenes = lista.Count(x => (x.tipus == "NYP" || x.tipus == "RVS" || x.tipus == "GYK") && x.datum_felszallas <= x.datum_berlet);

            Console.WriteLine($"Ingyenesen és kedvezményesen így utaznak: {ingyenes} fő és {kedvezmenyes} fő");



        }
	}
}




