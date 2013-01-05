/*
 * Copyright (C) 2013 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;

namespace gbrainy.Games.Logic
{
	public class PuzzleLargestDiameter : Game
	{
		class Arc
		{
			public double X { get; set; }
			public double Y { get; set; }
			public double Size { get; set; }
			public double StartAngle { get; set; }
			public double EndAngle { get; set; }
		}

		private const int num_arcs = 4;
		private const int right_answer = 0;
		private int right_answer_pos = 0;
		private Arc []arcs;
		private ArrayListIndicesRandom arcs_indices;

		public override string Name {
			get {return Translations.GetString ("Largest diameter");}
		}

		public override string Question {
			get {return String.Format (
				Translations.GetString ("If the circles represented by the arcs below were completed, which circle would have the largest diameter? Answer {0}, {1}, {2} or {3}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));
			}
		}

		public override string Rationale {
			get {
				return Translations.GetString ("Less curved is the arc, the larger the circle is.");
			}
		}

		protected override void Initialize ()
		{
			InitArcs ();
			RandomizeArcsOrder ();

			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;		
			Answer.Correct = Answer.GetMultiOption(right_answer_pos);
			SetDrawingAreas ();
		}
	
		private void SetDrawingAreas ()
		{
			double x = DrawAreaX, y = DrawAreaY + 0.05;
			DrawableArea drawable_area;

			HorizontalContainer container = new HorizontalContainer (x, y, 0.8, 0.3);
			AddWidget (container);

			for (int i = 0; i < arcs_indices.Count; i++)
			{
				if (i == 2)
				{
					container = new HorizontalContainer (x, y + 0.4, 0.8, 0.3);
					AddWidget (container);
				}

				drawable_area = new DrawableArea (0.3 + 0.1, 0.3);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;
					Arc arc = arcs[arcs_indices[n]];

					e.Context.Arc (arc.X, arc.Y, arc.Size, ToRadians (arc.StartAngle), ToRadians (arc.EndAngle));
					e.Context.Stroke ();

					e.Context.DrawTextCentered (0.2, 0.28, Translations.GetString (String.Format("Arc {0}",  
						Answer.GetMultiOption(n))));

					e.Context.Stroke();
				};

				container.AddChild (drawable_area);
			}
		}

		private void InitArcs ()
		{
			arcs = new Arc [num_arcs];
			arcs [right_answer] = new Arc () { X = 0.35, Y = 0.42,  Size = 0.35, StartAngle = 215, EndAngle = 260};
			arcs[1] = new Arc () { X = 0.3, Y = 0.40, Size = 0.25, StartAngle = 215, EndAngle = 290};
			arcs[2] = new Arc () { X = 0.3, Y = 0.40, Size = 0.25, StartAngle = 215, EndAngle = 270};
			arcs[3] = new Arc () { X = 0.3, Y = 0.40, Size = 0.25, StartAngle = 215, EndAngle = 270};
		}

		private void RandomizeArcsOrder ()
		{
			arcs_indices = new ArrayListIndicesRandom (num_arcs);
			arcs_indices.Initialize ();
			
			for (int i = 0; i < arcs_indices.Count; i++)
			{
				if (arcs_indices[i] == right_answer)
				{
					right_answer_pos = i;
					break;
				}
			}
		}

		double ToRadians (double degrees)
		{
			return degrees *  Math.PI / 180;
		}
	}
}


