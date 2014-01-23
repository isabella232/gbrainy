/*
 * Copyright (C) 2012 Jordi Mas i Hernàndez <jmas@softcatala.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Calculation
{
	public class CalculationSelectedNumbers : Game
	{
		enum Operation
		{
			Addition,
			Product,
			Total
		}

		const int options_cnt = 4;
		const int correct_pos = 0;
		int []numbers;
		int []options;
		ArrayListIndicesRandom random_indices;
		int correct;
		int total_size;
		int greater_than;
		Operation operation;

		public override string Name {
			get {return Translations.GetString ("Selected numbers");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get {
				switch (operation)
				{
					case Operation.Addition:
						return String.Format (Translations.GetString (
							"In the list of numbers below, what is the sum of all the numbers with a value greater than {0}? Answer {1}, {2}, {3} or {4}."), 
							greater_than, 
							Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));
					case Operation.Product:
						return String.Format (Translations.GetString (
							"In the list of numbers below, what is the product of all the numbers with a value greater than {0}? Answer {1}, {2}, {3} or {4}."),
							greater_than, 
							Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));
				}
				throw new InvalidOperationException();
			}
		}

		bool IsValidSequence (int selected, int result)
		{
			/* Too complex, too many selected */ 
			if (selected > total_size / 3) 
				return false;

			/* Too easy, 3 selected*/
			if (selected < 3 )
				return false;

			/* Too large to be fun */
			if (result > 700)
				return false;

			return true;
		}

		void GetPuzzleNumbersAndAnswer ()
		{
			operation = (Operation)random.Next((int)Operation.Total);
			numbers = new int [total_size];
			greater_than = 2 + random.Next (5);

			int selected;
			do
			{
				switch (operation)
				{
					case Operation.Addition:
						correct = 0;
						break;
					case Operation.Product:
						correct = 1;
						break;
				}

				selected = 0;
				for (int i = 0; i < total_size; i++)
				{
					numbers [i] = 1 + random.Next (8);

					if (numbers [i] > greater_than)
					{
						selected++;

						switch (operation)
						{
							case Operation.Addition:
								correct += numbers [i];
								break;
							case Operation.Product:
								correct *= numbers [i];
								break;
						}
					}
				}

			} while (IsValidSequence (selected, correct) == false);
		}

		protected override void Initialize ()
		{
			bool duplicated;
			int options_next, which = 0;

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
				total_size = 10;
				break;
			case GameDifficulty.Medium:
			case GameDifficulty.Master:
				total_size = 15;
				break;
			}

			GetPuzzleNumbersAndAnswer();

			options = new int [options_cnt];
			options [correct_pos] = correct;
			options_next = correct_pos + 1;

			// Generate possible answers
			while (options_next < options_cnt) 
			{
				int ans;

				ans = correct + random.Next (-correct / 2, correct / 2);
				duplicated = false;

				// No repeated answers
				for (int num = 0; num < options_next; num++)
				{
					if (options [num] == ans) 
					{
						duplicated = true;
						break;
					}
				}

				if (duplicated)
					continue;

				options [options_next] = ans;
				options_next++;
			}

			random_indices = new ArrayListIndicesRandom (options_cnt);
			random_indices.Initialize ();

			for (int i = 0; i < options_cnt; i++)
			{
				if (random_indices [i] == correct_pos) {
					which = i;
					break;
				}
			}

			Answer.SetMultiOptionAnswer (which, options [correct_pos].ToString ());

			// Options
			double x = DrawAreaX + 0.25, y = DrawAreaY + 0.26;
			Container container = new Container (x, y,  1 - (x * 2), 0.6);
			AddWidget (container);

			for (int i = 0; i < options_cnt; i++)
			{
				DrawableArea drawable_area = new DrawableArea (0.3, 0.1);
				drawable_area.X = x;
				drawable_area.Y = y + i * 0.15;
				container.AddChild (drawable_area);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;
					int indx = random_indices[n];

					e.Context.SetPangoLargeFontSize ();
					e.Context.MoveTo (0.02, 0.02);
					e.Context.ShowPangoText (String.Format ("{0}) {1}", Answer.GetMultiOption (n) , options [indx]));
				};
			}
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			const double SEPARATION = 0.05;
			double x = DrawAreaX, y = 0.05;

			base.Draw (gr, area_width, area_height, rtl);
			gr.SetPangoLargeFontSize ();

			gr.MoveTo (0.05, y);
			gr.SetPangoLargeFontSize ();
			gr.ShowPangoText (Translations.GetString ("Numbers"));
			y += 0.08;
		
			x = ((1 - (DrawAreaX * 2) - SEPARATION * numbers.Length) / 2) + DrawAreaX;
			for (int n = 0; n < numbers.Length; n++)
			{
				gr.MoveTo (x, y);
				gr.ShowPangoText (numbers[n].ToString ());
				gr.Stroke ();
				x += SEPARATION;
			}

			gr.MoveTo (0.1, 0.25);
			gr.ShowPangoText (Translations.GetString ("Choose one of the following:"));
		}
	}
}
