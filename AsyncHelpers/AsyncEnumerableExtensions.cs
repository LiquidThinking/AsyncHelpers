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
        
        /// <summary>
        /// USAGE NOTE!!! The results will not be ordered
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AsyncEnumerable<T> ToAsyncEnumerable<T>( this IEnumerable<Task<T>> list )
        {
            var asyncEnumberable = new AsyncEnumerable<T>();

            list.ToList().ForEach( t => { t.ContinueWith( r => asyncEnumberable.Add( t.Result ) ); } );

            return asyncEnumberable;
        }
    }
}
