/*
 * Copyright (C) 2011 Jordi Mas i Hernàndez <jmas@softcatala.org>
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

namespace gbrainy.Core.Services
{
	public interface ITranslations : IService
	{
		int TranslationPercentage { get;}
		string Language { get; set; }

		void Init (string package, string localedir);
		string GetString (string s);
		string GetPluralString (string s, string p, int n);
	}
}

