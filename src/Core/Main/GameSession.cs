/*
 * Copyright (C) 2007 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using Mono.Unix;
using System.Timers;
using System.ComponentModel;

using gbrainy.Core.Views;

namespace gbrainy.Core.Main
{
	public class GameSession : IDrawable, IDrawRequest, IMouseEvent
	{
		[Flags]
		public enum Types
		{	
			None			= 0,
			LogicPuzzles		= 2,
			MemoryTrainers		= 4,
			CalculationTrainers	= 8,
			VerbalAnalogies		= 16,
			Custom			= 32,
			AllGames		= MemoryTrainers | CalculationTrainers | LogicPuzzles | VerbalAnalogies
		}

		public enum SessionStatus
		{
			NotPlaying,
			Playing,
			Answered,
			Finished,
		}

		private TimeSpan game_time;
		private Game current_game;
		private GameManager game_manager;
		private System.Timers.Timer timer;
		private bool paused;
		private string current_time;
		private TimeSpan one_sec = TimeSpan.FromSeconds (1);
		private SessionStatus status;
		private ViewsControler controler;
		private ISynchronizeInvoke synchronize;
		private PlayerHistory player_history;
		private int id;
		private GameSessionHistoryExtended history;

		public event EventHandler DrawRequest;
		public event EventHandler <UpdateUIStateEventArgs> UpdateUIElement;
	
		public GameSession ()
		{
			id = 0;
			game_manager = new GameManager ();
			game_time = TimeSpan.Zero;

			timer = new System.Timers.Timer ();
			timer.Elapsed += TimerUpdater;
			timer.Interval = (1 * 1000); // 1 second

			controler = new ViewsControler (this);
			Status = SessionStatus.NotPlaying;
			player_history = new PlayerHistory ();
			history = new GameSessionHistoryExtended ();
		}

		public int ID {
			get {return id;}
		}

		public GameSessionHistoryExtended History {
			get {return history;}
		}

		public GameTypes AvailableGames {
			get { return game_manager.AvailableGameTypes; }
		}

		public PlayerHistory PlayerHistory { 
			set { player_history = value; }
			get { return player_history; }
		}

		public ISynchronizeInvoke SynchronizingObject { 
			set { synchronize = value; }
			get { return synchronize; }
		}
	
		public Types Type {
			get {return game_manager.GameType; }
			set {game_manager.GameType = value; }
		}

		public GameDifficulty Difficulty {
			get {return game_manager.Difficulty; }
			set {game_manager.Difficulty = value; }
		}
	
		public TimeSpan GameTime {
			get {return game_time; }
			set {game_time = value; }
		}

		public bool Paused {
			get {return paused; }
			set {paused = value; }
		}

		public Game CurrentGame {
			get {return current_game; }
			set {
				current_game = value; 
				controler.Game = value;
			}
		}

		public bool EnableTimer {
			get {return timer.Enabled; }
			set {timer.Enabled = value; }
		}

		public SessionStatus Status {
			get {return status; }
			set {
				status = value;
				controler.Status = value;

				if (status == SessionStatus.Answered && CurrentGame != null)
					CurrentGame.EnableMouseEvents (false);
			}
		}

		public GameManager GameManager {
			get { return game_manager;}
			set { game_manager = value;}
		}

		public string TimePlayed {
			get {
				return (current_time == null) ? TimeSpanToStr (TimeSpan.FromSeconds (0)) : current_time;
			}
		}

		public string TimePerGame {
			get {
				TimeSpan average;

				average = (history.GamesPlayed > 0) ? TimeSpan.FromSeconds (game_time.TotalSeconds / history.GamesPlayed) : game_time;
				return TimeSpanToStr (average);
			}
		}

		public string StatusText {
			get {
				if (Status == SessionStatus.NotPlaying)
					return string.Empty;

				String text;
				text = String.Format (Catalog.GetString ("Games played: {0} (Score: {1})"), history.GamesPlayed, history.TotalScore);
				text += String.Format (Catalog.GetString (" - Time: {0}"), current_time);

				if (CurrentGame != null)
	 				text += " " + String.Format (Catalog.GetString ("- Game: {0}"), CurrentGame.Name);
	
				return text;
			}
		}

		// Summarizes how the game did go
		public string Result {
			get {
				string s;

				if (history.GamesPlayed >= 10) {
					int percentage_won = (int) (100 * history.GamesWon / history.GamesPlayed);
					if (percentage_won >= 90)
						s = Catalog.GetString ("Outstanding results");
					else if (percentage_won >= 70)
						s = Catalog.GetString ("Excellent results");
					else if (percentage_won >= 50)
						s = Catalog.GetString ("Good results");
					else if (percentage_won >= 30)
						s = Catalog.GetString ("Poor results");
					else s = Catalog.GetString ("Disappointing results");
				} else
					s = string.Empty;
	
				return s;
			}
		}
	
		public void New ()
		{
			if (Type == Types.None)
				throw new InvalidOperationException ("You have to setup the GameSession type");

			id++;
			if (Status != SessionStatus.NotPlaying)
				End ();

			current_time = TimeSpanToStr (game_time);

			history.Clear ();
			game_time = TimeSpan.Zero;
			timer.SynchronizingObject = SynchronizingObject;
			EnableTimer = true;
		}

		public void End ()
		{
			// Making a deep copy of GameSessionHistory type (base class) for serialization
			player_history.SaveGameSession (history.Copy ());

			if (CurrentGame != null)
				CurrentGame.Finish ();

			EnableTimer = false;
			timer.SynchronizingObject = null;

			paused = false;
			CurrentGame = null;
			Status = SessionStatus.Finished;
		}

		public void NextGame ()
		{	
			if (CurrentGame != null)
				CurrentGame.Finish ();

			history.GamesPlayed++;
			CurrentGame = game_manager.GetPuzzle ();
			CurrentGame.SynchronizingObject = SynchronizingObject;
			CurrentGame.DrawRequest += GameDrawRequest;
			CurrentGame.UpdateUIElement += GameUpdateUIElement;

			CurrentGame.Begin ();

			CurrentGame.GameTime = TimeSpan.Zero;
			Status = SessionStatus.Playing;
		}

		public void Pause ()
		{
			EnableTimer = false;
			paused = true;
			current_time = Catalog.GetString ("Paused");

			if (CurrentGame != null)
				CurrentGame.EnableMouseEvents (false);
		}

		public void Resume ()
		{
			EnableTimer = true;
			paused = false;

			if (CurrentGame != null)
				CurrentGame.EnableMouseEvents (true);
		}

		public bool ScoreGame (string answer)
		{
			int game_score;

			if (CurrentGame == null || Status == SessionStatus.Answered)
				return false;

			game_score = CurrentGame.Score (answer);
			history.UpdateScore (CurrentGame.Type, Difficulty, game_score);

			Status = SessionStatus.Answered;
			return (game_score > 0 ? true : false);
		}

		private void TimerUpdater (object source, ElapsedEventArgs e)
		{
			lock (this) {
				if (CurrentGame == null)
					return;

				game_time = game_time.Add (one_sec);
				CurrentGame.GameTime = CurrentGame.GameTime + one_sec;
				current_time = TimeSpanToStr (game_time);
			}

			if (UpdateUIElement == null)
				return;

			UpdateUIElement (this, new UpdateUIStateEventArgs (UpdateUIStateEventArgs.EventUIType.Time, null));
		}

		static private string TimeSpanToStr (TimeSpan time)
		{
			string fmt = time.ToString ();
			int i = fmt.IndexOf ('.');
			if (i > 0 && fmt.Length - i > 2)
				fmt = fmt.Substring (0, i);

			return fmt;
		}

		public void GameUpdateUIElement (object obj, UpdateUIStateEventArgs args)
		{
			if (UpdateUIElement != null)
				UpdateUIElement (this, args);
		}

		// A game has requested a redraw, scale the request to the object
		// subscribed to GameSession.GameDrawRequest
		public void GameDrawRequest (object o, EventArgs args)
		{
			if (DrawRequest != null)
				DrawRequest (this, EventArgs.Empty);
		}

		public virtual void Draw (CairoContextEx gr, int width, int height, bool rtl)
		{
			controler.CurrentView.Draw (gr, width, height, rtl);
		}

		public void MouseEvent (object obj, MouseEventArgs args)
		{
			if (controler.CurrentView as IMouseEvent != null)
				(controler.CurrentView as IMouseEvent).MouseEvent (this, args);
		}
	}
}
