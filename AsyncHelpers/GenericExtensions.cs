using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHelpers
{
	public static class GenericExtensions
	{
		public static ValueTask<T> AsValueTask<T>( this T obj )
		{
			return new ValueTask<T>( obj );
		}
	}
}
