/*
 * Copyright (C) 2008-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
 * License along with this program; if not, see <http://www.gnu.org/licenses/>."));

			ProgramName = "gbrainy";
			Version = Defines.VERSION;
			Authors = authors;
			Documenters = documenters;
			Logo = LoadFromAssembly ("gbrainy.svg");

			Copyright = Defines.COPYRIGHT;

			Comments = Catalog.GetString ("A brain teaser game for fun and to keep your brain trained.");
			Website = "https://wiki.gnome.org/Apps/gbrainy";
			WebsiteLabel = Catalog.GetString ("gbrainy project web site");
			TranslatorCredits = translators;
			Artists = artists;
			IconName = null;
			License = license.ToString ();
			WrapLicense = true;
			Response += delegate (object o, Gtk.ResponseArgs e) {Destroy ();};
		}

		static Pixbuf LoadFromAssembly (string resource)
		{
			try {
				return new Pixbuf (System.Reflection.Assembly.GetEntryAssembly (), resource);
			} 
			catch (Exception e)
			{
				Console.WriteLine ("AboutDialog.LoadFromAssembly. Could not load resource {0}. Error {1}", resource, e);
				return null;
			}
		}
	}
}
