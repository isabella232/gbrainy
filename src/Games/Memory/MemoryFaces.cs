/*
 * Copyright (C) 2007-2012 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Cairo;

using gbrainy.Core.Main;
using gbrainy.Core.Toolkit;

namespace gbrainy.Games.Memory
{
	public class MemoryFaces : Core.Main.Memory
	{
		private ArrayListIndicesRandom figures;
		private int rows;
		private int columns;
		private const double start_x_ques = 0.25;
		private const double start_x_ans = 0.25;
		private const double start_y = 0.15;
		private const double figure_size = 0.15;
		private double rect_w, rect_h;
		private int question_pos, question_answer;
		private int figures_active;

		public enum Face
		{
			FaceBoy1,
			FaceBoy2,
			FaceBoy3,
			FaceGirl1,
			FaceGirl2,
			FaceGirl3,
		}

		public override string Name {
			get {return Translations.GetString ("Memorize faces");}
		}

		public override string MemoryQuestion {
			get { 
				return Translations.GetString ("In which cell is the other face like the one shown below? Answer the cell number." );}
		}

		protected override bool SupportsShading {
			get { return false; }
		}

		protected override void Initialize ()
		{
			int fig1, fig2;

			switch (CurrentDifficulty) {
			case GameDifficulty.Easy:
			case GameDifficulty.Medium:
				figures_active = 4;
				rows = columns = 3;
				break;
			case GameDifficulty.Master:
				figures_active = 6;
				rows = 3;
				columns = 4;			
				break;
			}

			rect_w = 0.65 / columns;
			rect_h = 0.65 / rows;
			figures = new ArrayListIndicesRandom (figures_active * 2);
			figures.Initialize ();
			question_pos = random.Next (figures_active * 2);

			for (int figure = 0; figure < figures_active * 2; figure++)
			{	
				if (figure == question_pos)
					continue;
	
				fig1 = figures[figure];
				fig2 = figures[question_pos];

				if (fig1 >= figures_active) fig1 -= figures_active;
				if (fig2 >= figures_active) fig2 -= figures_active;

				if (fig1 == fig2) {
					question_answer = figure + 1;
					break;
				}
			}
			Answer.Correct = question_answer.ToString ();
			base.Initialize ();

			// Answers controls
			int col = 0;
			double y = start_y;

			HorizontalContainer container = new HorizontalContainer (start_x_ans, y, columns * rect_w, rect_h);
			AddWidget (container);

			for (int figure = 0; figure < figures.Count; figure++, col++)
			{
				if (col >= columns) {
					col = 0;
					y += rect_h;

					container = new HorizontalContainer (start_x_ans, y, columns * rect_w, rect_h);
					AddWidget (container);
				}
			
				DrawableArea drawable_area = new DrawableArea (rect_w, rect_h);
				container.AddChild (drawable_area);
				drawable_area.DataEx = (figure + 1).ToString ();

				if (figure == question_pos) {
					int fig = figures[figure];
					if (fig >= figures_active) fig -= figures_active;

					drawable_area.Data = fig;
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						DrawFigure (e.Context, 0, 0, (Face) e.Data);
					};

				} else
				{
					drawable_area.Data = figure + 1;
					drawable_area.DrawEventHandler += delegate (object sender, DrawEventArgs e)
					{
						int n = (int) e.Data;

						e.Context.SetPangoLargeFontSize ();
						e.Context.DrawTextCentered (rect_w / 2, rect_h / 2, (n).ToString ());
						e.Context.Stroke ();
					};
				}
			}
		}

		public override void DrawPossibleAnswers (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			gr.SetSourceColor (new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, 1));

			if (Answer.Draw ==  true) {
				DrawAllFigures (gr, start_x_ans, start_y);
				return;
			}

			DrawGrid (gr, start_x_ans, start_y);
			base.DrawPossibleAnswers (gr, area_width, area_height, rtl);
		}

		public override void DrawObjectToMemorize (CairoContextEx gr, int area_width, int area_height, bool rtl)
		{
			base.DrawObjectToMemorize (gr, area_width, area_height, rtl);
			DrawAllFigures (gr, start_x_ques, start_y);
		}

		private void DrawAllFigures (CairoContextEx gr, double x, double y)
		{
			int col = 1, fig;
			double org_x = x;

			DrawGrid (gr, x, y);
			gr.SetSourceColor (new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha));
			for (int figure = 0; figure < figures.Count; figure++, col++)
			{
				fig = figures[figure];
				if (fig >= figures_active) 
					fig -= figures_active;

				DrawFigure (gr, x, y, (Face) fig);

				if (col >= columns) {
					col = 0;
					y += rect_h;
					x = org_x;
				} else {
					x += rect_w;
				}
			}
		}

		private void DrawFigure (CairoContextEx gr, double x, double y, Face fig)
		{
			double space_x, space_y;
			string image;

			space_x = (rect_w - figure_size) / 2;
			space_y = (rect_h - figure_size) / 2;

			switch (fig) {
			case Face.FaceBoy1:
				image = "faceboy1.svg";
				break;
			case Face.FaceBoy2:
				image = "faceboy2.svg";
				break;
			case Face.FaceBoy3:
				image = "faceboy3.svg";
				break;
			case Face.FaceGirl1:
				image = "facegirl1.svg";
				break;
			case Face.FaceGirl2:
				image = "facegirl2.svg";
				break;
			case Face.FaceGirl3:
				image = "facegirl3.svg";
				break;
			default:
				throw new InvalidOperationException("Invalid value");
			}
			gr.DrawImageFromAssembly (image, x + space_x, y + space_y, figure_size, figure_size);
		}

		private void DrawGrid (CairoContextEx gr, double x, double y)
		{
			gr.SetSourceColor (new Color (DefaultDrawingColor.R, DefaultDrawingColor.G, DefaultDrawingColor.B, alpha));
			for (int column = 0; column < columns; column++) {
				for (int row = 0; row < rows; row++) {
					gr.Rectangle (x + column * rect_w, y + row * rect_h, rect_w, rect_h);
				}
			}
			gr.Stroke ();
		}
	}
}
