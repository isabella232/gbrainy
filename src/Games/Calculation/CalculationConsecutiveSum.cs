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
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;

using gbrainy.Core.Main;

namespace gbrainy.Games.Calculation
{
	public class CalculationConsecutiveSum : Game
	{
		const int CONSECUTIVE_SIZE = 5;
		const int MIN_START = 5;
		int total_size;	
		int consecutive_sum = 0;
		int consecutive_pos;
		int []numbers;

		public override string Name {
			get {return Translations.GetString ("Consecutive sum");}
		}

		public override GameTypes Type {
			get { return GameTypes.Calculation;}
		}

		public override string Question {
			get { return String.Format (Translations.GetString (
				"In the list of single-digit numbers below, there are 5 consecutive numbers whose sum is {0}. Which numbers are these?"), 
					consecutive_sum);
			}
		}

		bool HasUniqueConsecutive ()
		{
			int found = 0;

			for (int i = 0; i < total_size - CONSECUTIVE_SIZE + 1; i++)
			{
				int sum = 0;
				for (int c = 0; c < CONSECUTIVE_SIZE; c++)
				{
					sum += numbers[i + c];
				}

				if (sum == consecutive_sum)
					found++;
			}

			return found == 1;
		}
	
		protected override void Initialize ()
		{

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
			case GameDifficulty.Medium:
				total_size = 15;
				break;
			case GameDifficulty.Master:
				total_size = 20;
				break;
			}

			numbers = new int [total_size];

			do 
			{	
				for (int i = 0; i < total_size; i++)
				{
					numbers[i] = random.Next (10);
				}

				consecutive_pos = MIN_START + random.Next (total_size - MIN_START - CONSECUTIVE_SIZE);
				consecutive_sum = 0;

				for (int i = 0; i < CONSECUTIVE_SIZE; i++)
				{
					consecutive_sum += numbers[i + consecutive_pos];
				}

			} while (HasUniqueConsecutive () == false);

			string ans = string.Empty;
			string show = string.Empty;
			for (int i = 0; i < CONSECUTIVE_SIZE; i++)
			{
				ans+= numbers[i + consecutive_pos].ToString ();
				show+= numbers[i + consecutive_pos].ToString ();
			}
		
			Answer.CheckAttributes |= GameAnswerCheckAttributes.IgnoreSpaces;
			Answer.CheckExpression = "[-0-9]+";
			Answer.CorrectShow = show;
			Answer.Correct = ans;
		}

		public override void Draw (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			const double SEPARATION = 0.05;
			double x = DrawAreaX, y = 0.4;

			base.Draw (gr, area_width, area_height, rtl);
			gr.SetPangoLargeFontSize ();
		
			x = ((1 - (DrawAreaX * 2) - SEPARATION * numbers.Length) / 2) + DrawAreaX;
			for (int n = 0; n < numbers.Length; n++)
			{
				gr.MoveTo (x, y);
				gr.ShowPangoText (numbers[n].ToString ());
				gr.Stroke ();
				x += SEPARATION;
			}
		}
	}
}
