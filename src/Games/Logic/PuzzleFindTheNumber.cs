/*
 * Copyright (C) 2012-2013 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using gbrainy.Core.Toolkit;


namespace gbrainy.Games.Logic
{
	public class PuzzleFindTheNumber : Game
	{
		const int MAX_DISTANCE = 3;
		const int MAX_NUMBER = 49;

		int [] grid;
		int rows, columns;
		int answer_column, answer_row;
		int pos_ref;

		enum Operation
		{
			Addition,
			Multiplication
		};
		
		public override string Name {
			get {return Translations.GetString ("Find the number");}
		}

		public override string Question {
			get {
				// Translators: {0} is a number between 1 and 3
				return String.Format(
					Translations.GetPluralString (
					"Looking horizontally and vertically to the lines of the grid below, which is the number {0} place away from itself multiplied by 2 and {1} place away from itself plus 2?",
					"Looking horizontally and vertically to the lines of the grid below, which is the number {0} places away from itself multiplied by 2 and {1} places away from itself plus 2?",
					pos_ref),
					pos_ref, pos_ref);
			}
		}

		public override string Rationale {
			get {
				return String.Format(Translations.GetString ("The number is located at row {0}, column {1}."),
					answer_row + 1, answer_column + 1);
			}
		}

		public override string Tip {
			get {
				int where = 1 + rows - MAX_DISTANCE;

				return String.Format(Translations.GetPluralString ("The number is located within the first {0} row of the grid.",
					"The number is located within the first {0} rows of the grid.", where),
					where);
			}
		}

		void PopulateGrid ()
		{
			pos_ref = 1 + random.Next (MAX_DISTANCE);
			for (int i = 0; i < grid.Length; i++)
			{
				grid[i] = (1 + random.Next (MAX_NUMBER));
			}

			answer_row = random.Next (rows - MAX_DISTANCE);
			answer_column = random.Next (columns - MAX_DISTANCE);

			int answer_idx = columns * answer_row + answer_column;
			grid[answer_idx + pos_ref] = grid [answer_idx] * 2;
			grid[answer_idx + pos_ref * columns] = grid [answer_idx] + 2;
		}

		bool ValueMeetConditions (int idx, int hpos, int vpos, Operation hoper, Operation voper)
		{
			if (hoper == voper)
				throw new InvalidOperationException();

			if (idx + hpos >= grid.Length || idx + vpos * columns >= grid.Length ||
				idx + hpos < 0 || idx + vpos * columns < 0)
			{
				return false;
			}
			
			if (hoper == Operation.Addition)
			{
				if (grid[idx + hpos] == grid [idx] + 2)
				{
					return true;
				}
			}
			else	
			{
				if (grid[idx + hpos] == grid [idx] * 2)
				{
					return true;
				}
			}

			if (voper == Operation.Addition)
			{
				if (grid[idx + vpos * columns] == grid [idx] + 2)
				{
					return true;
				}
			}
			else	
			{
				if (grid[idx + vpos * columns] == grid [idx] * 2)
				{
					return true;
				}
			}
			return false;
		}

		bool IsAValidGrid ()
		{
			int ans = grid[columns * answer_row + answer_column];
			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < columns; column++)
				{
					if (column == answer_column && row == answer_row)
						continue;

					int idx = columns * row + column;

					if (grid[idx] == ans)
						return false;

					// Order: horizontal, vertical
					if (ValueMeetConditions (idx, pos_ref, -pos_ref, Operation.Addition, Operation.Multiplication) ||
						ValueMeetConditions (idx, pos_ref, -pos_ref, Operation.Multiplication, Operation.Addition) ||
						ValueMeetConditions (idx, -pos_ref, -pos_ref, Operation.Multiplication, Operation.Addition) ||
						ValueMeetConditions (idx, -pos_ref, -pos_ref, Operation.Addition, Operation.Multiplication) ||
						ValueMeetConditions (idx, pos_ref, pos_ref, Operation.Addition, Operation.Multiplication) ||
						ValueMeetConditions (idx, pos_ref, pos_ref, Operation.Multiplication, Operation.Addition) ||
						ValueMeetConditions (idx, -pos_ref, pos_ref, Operation.Addition, Operation.Multiplication) ||
						ValueMeetConditions (idx, -pos_ref, pos_ref, Operation.Multiplication, Operation.Addition))
					{
						return false;
					}
				}
			}

			return true;
		}

		void SetGridSizeForDifficulty ()
		{
			switch (CurrentDifficulty) {
			case GameDifficulty.Master:
				rows = columns = 8;
				break;		
			case GameDifficulty.Easy:			
			case GameDifficulty.Medium:
			default:
				rows = columns = 6;
				break;		
			}
		}

		protected override void Initialize ()
		{
			SetGridSizeForDifficulty ();
			grid = new int [rows * columns];

			do
			{
				PopulateGrid ();
	
			} while (IsAValidGrid () == false);

			SetDrawingAreas ();
			Answer.Correct = grid[columns * answer_row + answer_column].ToString ();
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
					DrawableArea drawable_area = new DrawableArea (rect_w, rect_h);
					drawable_area.X = DrawAreaX + row * rect_w;
					drawable_area.Y = DrawAreaY + column * rect_h;
					container.AddChild (drawable_area);

					string num = grid[row * columns + column].ToString ();
					drawable_area.Data = num;
					drawable_area.DataEx = num;
									
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						string number = (string) e.Data;

						e.Context.Rectangle (0, 0, e.Width, e.Height);
						e.Context.Stroke ();

						e.Context.SetPangoLargeFontSize ();
						e.Context.DrawTextCentered (e.Width / 2, e.Height / 2, number);
						e.Context.Stroke ();
					};
				}
			}
		}
	}
}
