using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHelpers
{
	public interface IAsyncEnumerable<out T> : IEnumerable<T>, IEnumerator<T>
	{
		Task<bool> MoveNextAsync( CancellationToken cancellationToken = default( CancellationToken ) );
	}
}