﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Singularity
{
	internal static class EnumerableExtensions
	{
		[DebuggerStepThrough]
		public static bool TryExecute<TObject>(this IEnumerable<TObject> objects, Action<TObject> action, out IList<Exception> exceptions)
		{
			exceptions = null!;
			foreach (TObject o in objects)
			{
				try
				{
					action(o);
				}
				catch (Exception e)
				{
					if (exceptions == null) exceptions = new List<Exception>();
					exceptions.Add(e);
				}
			}

			return exceptions != null;
		}
	}
}
