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

namespace gbrainy.Core.Services
{
	public interface ICSharpCompiler : IService
	{
		// Evaluates code and stores variables locally
		void EvaluateCode (string c);

		// Gets the value of all variables (after code evaluation)
		string GetAllVariables ();

		// Gets the value of all variables (after code evaluation)
		string GetVariableValue (string _var);
	}
}

