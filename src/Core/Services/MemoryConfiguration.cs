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

using System;
using System.Collections.Generic;

namespace gbrainy.Core.Services
{
	public class MemoryConfiguration : IConfiguration
	{
		Dictionary <ConfigurationKeys, object> keys;

		public MemoryConfiguration ()
		{
			keys = new Dictionary <ConfigurationKeys, object> ();
		}

		public T Get <T> (ConfigurationKeys key)
		{
			try
			{
				return (T) keys [key];
			}

			catch (KeyNotFoundException)
			{
				throw new KeyNotFoundException (String.Format ("MemoryConfiguration.Get. Key '{0}' not found", key));
			}
		}

		public void Set <T> (ConfigurationKeys key, T val)
		{
			if (keys.ContainsKey (key) == false)
			{
				keys.Add (key, val);
			}
			else
			{
				keys[key] = val;
			}
		}
	}
}

