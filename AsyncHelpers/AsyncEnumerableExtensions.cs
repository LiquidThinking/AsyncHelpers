using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncHelpers
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>( this IEnumerable<Task<T>> list )
        {
            var items = list.ToList();
            await Task.WhenAll( items );
            return items.Select( x => x.Result ).ToList();
        }
    }
}
