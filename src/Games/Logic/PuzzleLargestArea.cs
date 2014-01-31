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
using System.Collections.Generic;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;
using gbrainy.Core.Services;

namespace gbrainy.Games.Logic
{
	public class PuzzleLargestArea : Game
	{
		internal class Figures
		{
			Figure[] figures;

			public Figure[] GetAll()
			{ 
				return figures;
			}

			public Figures ()
			{
				List <Figure> prevfigures = new List <Figure> ();

				prevfigures.Add (new CircleFigure ());
				prevfigures.Add (new RectangleFigure ());
				prevfigures.Add (new SquareFigure ());
				prevfigures.Add (new TriangleFigure ());

				do
				{
					foreach (Figure figure in prevfigures)
					{
						figure.Initialitze (prevfigures);
					}
				}
				while (AreValidFigures (prevfigures) == false);

				figures = StoreRandomOrder (prevfigures);
			}

			public int GetLargerFigure (List <Figure> prevfigures)
			{
				double area = 0;
				int idx = -1;

				for (int i = 0; i < prevfigures.Count; i++)
				{
					if (area == 0 || prevfigures[i].Area() > area)
					{
						area = prevfigures[i].Area();
						idx = i;
					}
				}

				return idx;
			}

			public int GetSmallerFigure (List <Figure> prevfigures)
			{
				double area = Int32.MaxValue;
				int idx = 0;

				for (int i = 0; i < prevfigures.Count; i++)
				{
					if (area == 0 || prevfigures[i].Area() < area)
					{
						area = prevfigures[i].Area();
						idx = i;
					}
				}

				return idx;
			}

			bool AreValidFigures (List <Figure> prevfigures)
			{
				int larger = GetLargerFigure (prevfigures);
				int smaller = GetSmallerFigure (prevfigures);
				const double min_margin = 1.3; // 30%, minimum to make sure that is easy
				const double max_margin = 2.0; // 200%, max to make sure is not trivial

				if (prevfigures[larger].Area () > prevfigures[smaller].Area () * min_margin &&
					prevfigures[larger].Area () < prevfigures[smaller].Area () * max_margin)
				{
					return true;
				}

				return false;
			}

			Figure[] StoreRandomOrder (List <Figure> prevfigures)
			{
				List <Figure> randomfigures = new List <Figure> ();
				ArrayListIndicesRandom random_indices = new ArrayListIndicesRandom (prevfigures.Count);
				random_indices.Initialize ();

				foreach (int idx in random_indices)
				{
					randomfigures.Add(prevfigures[idx]);
				}

				return randomfigures.ToArray ();
			}

		 	public int GetRightAnswerIndex ()
			{
				List <Figure> figs = new List <Figure> ();
				figs.AddRange (figures);
				return GetLargerFigure (figs);
			}
		}

		internal abstract class Figure
		{
			Random random = new Random ();
			const double figure_size = 0.3;
			const double margin = 0.005;

			public double GetRandom (double max)
			{
				const double factor = 1000d;
				int num = (int) (max * factor);
				double r = (double) random.Next (num);
				return r / factor;
			}
			
			public abstract double Area ();
			public abstract void Draw (CairoContextEx gr, double x, double y, double size);
			public abstract void Initialitze (List <Figure> prevfigures);
		}

		internal class CircleFigure : Figure
		{
			private double radius;
	
			public override void Initialitze (List <Figure> prevfigures)
			{
				radius = 0.05 + GetRandom (0.05);
			}

			public override double Area ()
			{
				return Math.PI * radius * radius;
			}

			public override void Draw (CairoContextEx gr, double x, double y, double size)
			{
				gr.Arc (size / 2, size / 2, radius, 0, 2 * Math.PI);
				gr.Stroke();
			}
		}

		internal class RectangleFigure : Figure
		{
			private double width, heigth;

			public override void Initialitze (List <Figure> prevfigures)
			{
				width = 0.15 + GetRandom (0.1);
				heigth = 0.15 + GetRandom (0.1);
			}

			public override double Area ()
			{
				return width * heigth;
			}

			public override void Draw (CairoContextEx gr, double x, double y, double size)
			{
				gr.Rectangle ((size - width) / 2, (size - heigth) / 2, width, heigth);
				gr.Stroke ();
			}
		}

		internal class SquareFigure : Figure
		{
			private double side;

			public override void Initialitze (List <Figure> prevfigures)
			{
				side = 0.1 + GetRandom (0.1);
			}

			public override double Area ()
			{
				return side * side;
			}

			public override void Draw (CairoContextEx gr, double x, double y, double size)
			{
				gr.Rectangle ((size - side) / 2, (size - side) / 2, side, side);
				gr.Stroke ();
			}
		}

		internal class TriangleFigure : Figure
		{
			private double side;

			public override void Initialitze (List <Figure> prevfigures)
			{
				side = 0.15 + GetRandom (0.10);
			}

			public override double Area ()
			{
				return Math.Sqrt (3) / 4d * side * side;
			}

			public override void Draw (CairoContextEx gr, double x, double y, double size)
			{
				gr.DrawEquilateralTriangle ((size - side) / 2, (size - side) / 2, side);
				gr.Stroke ();
			}
		}

		private Figures figures;

		public override string Name {
			get {return Translations.GetString ("Largest area");}
		}

		public override string Question {
			get {return String.Format ( Translations.GetString 
				("Which of the following figures has the largest area? Answer {0}, {1}, {2} or {3}."),
				Answer.GetMultiOption (0), Answer.GetMultiOption (1), Answer.GetMultiOption (2), Answer.GetMultiOption (3));} 
		}

		protected override void Initialize ()
		{
			Answer.CheckAttributes |= GameAnswerCheckAttributes.MultiOption | GameAnswerCheckAttributes.IgnoreSpaces;

			figures = new Figures ();
			SetDrawingAreas ();
			Answer.Correct = Answer.GetMultiOption (figures.GetRightAnswerIndex());
		}

		private void SetDrawingAreas ()
		{
			double x = DrawAreaX, y = DrawAreaY;
			DrawableArea drawable_area;
			const double figure_size = 0.3, margin = 0.1;

			HorizontalContainer container = new HorizontalContainer (x, y, 0.8, 0.4);
			AddWidget (container);

			for (int i = 0; i < figures.GetAll().Length; i++)
			{
				if (i == 2)
				{
					container = new HorizontalContainer (x, y + 0.4, 0.8, 0.4);
					AddWidget (container);
				}

				drawable_area = new DrawableArea (figure_size + margin, 0.35);
				drawable_area.SelectedArea = new Rectangle (0, 0, figure_size, 0.35);
				drawable_area.Data = i;
				drawable_area.DataEx = Answer.GetMultiOption (i);

				drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
				{
					int n = (int) e.Data;
					Figure figure = figures.GetAll()[n];	
					figure.Draw (e.Context, 0, 0, figure_size);
					e.Context.DrawTextCentered (figure_size / 2, 0.32, Answer.GetFigureName(n));
					e.Context.Stroke ();
				};

				container.AddChild (drawable_area);
			}
		}
	}
}
