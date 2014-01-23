/*
 * Copyright (C) 2007, 2008, 2013 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using gbrainy.Core.Services;
using gbrainy.Core.Toolkit;

namespace gbrainy.Games.Logic
{
	public class PuzzleGridCircles : Game
	{
		struct Number
		{
			public string NumberString { set; get; }
			public bool Cercled  { set; get; }
		};

		private int [] numbers;
		private int good_pos;
		private const int rows = 4, columns = 4;
		private int divisor;

		public override string Name {
			get {return Translations.GetString ("Circles in a grid");}
		}

		public override string Question {
			get {return Translations.GetString ("One of the numbers in the grid must be circled. Which one?");}
		}

		public override string Tip {
			get { return Translations.GetString ("All circled numbers share an arithmetical property.");}
		}

		public override string Rationale {
			get {
				return String.Format (Translations.GetString ("Every circled number can be divided by {0}."), divisor);
			}
		}

		protected override void Initialize ()
		{
			numbers = new int [rows * columns];
			
			switch (random.Next (2)) {
			case 0:
				divisor = 2;
				break;
			case 1:
				divisor = 3;
				break;
			}
		
			GenerateGridNumbers ();
			SetDrawingAreas ();
			Answer.Correct = numbers[good_pos].ToString ();
		}

		private void GenerateGridNumbers ()
		{
			bool completed = false;
			int count;
			int good = 1 + random.Next (5);
			
			while (completed == false) {
				count = 0;
				for (int n = 0; n < rows; n++) {
					for (int i = 0; i < columns; i++) {
						numbers[(n*rows) + i] = GetUnique ((n*rows) + i);
						if (numbers[(n*rows) + i] % divisor == 0) {
							count++;
							if  (count == good) {
								good_pos =  (n*rows) + i;
							}
						}
					}
				}
			
				if (count > 5 && count < 10)
					completed = true;
			}
		}

		private void SetDrawingAreas ()
		{
			double rect_w = DrawAreaWidth / rows;
			double rect_h = DrawAreaHeight / columns;

			Container container = new Container (DrawAreaX, DrawAreaY, 0.8, 0.8);
			AddWidget (container);

			for (int column = 0; column < columns; column++) 
			{
				for (int row = 0; row < rows; row++) 
				{
					bool cercled;
					DrawableArea drawable_area = new DrawableArea (rect_w, rect_h);
					drawable_area.X = DrawAreaX + row * rect_w;
					drawable_area.Y = DrawAreaY + column * rect_h;
					container.AddChild (drawable_area);

					cercled = numbers[column + (row * 4)] % divisor == 0 && good_pos != column + (row * 4);
					Number number = new Number { NumberString = numbers[column + (row * 4)].ToString (), Cercled = cercled };
					drawable_area.Data = number;
					drawable_area.DataEx = number.NumberString;
									
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						Number num = (Number) e.Data;

						e.Context.Color = DefaultDrawingColor;
						e.Context.Rectangle (0, 0, e.Width, e.Height);
						e.Context.Stroke ();

						e.Context.DrawTextCentered (e.Width / 2, e.Height / 2, num.NumberString);

						if (num.Cercled)
						{
							e.Context.Arc (e.Width / 2, e.Height / 2, 0.05, 0, 2 * Math.PI);
							e.Context.FillGradient (e.Width / 2, e.Height / 2, 0.05, 0.05);
						}

						e.Context.Stroke ();
					};
				}
			}
		}

		private int GetUnique (int max)
		{
			int unique = 0, i;
			bool found = false;

			while (found == false)
			{
				unique = 1 + random.Next (100);
				for (i = 0; i < max; i++) {
					if (unique == numbers [i]) {
						break;
					}
				}
				if (i == max)
					found = true;
			}
			return unique;
		}
	}
}
