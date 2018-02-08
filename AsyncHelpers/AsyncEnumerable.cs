using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHelpers
{
	public class AsyncEnumerable<T> : IAsyncEnumerable<T>
	{
		private readonly Queue<T> _items;
		private readonly TaskCompletionSource<bool> _closed = new TaskCompletionSource<bool>();
		private TaskCompletionSource<bool> _pendingItem = new TaskCompletionSource<bool>();

		public AsyncEnumerable( List<T> items = null )
		{
			_items = new Queue<T>( items ?? new List<T>() );
		}

		public void Add( T item )
		{
			_items.Enqueue( item );
			if ( !_pendingItem.Task.IsCompleted )
				_pendingItem.SetResult( true );
		}

		public bool IsOpen { get; private set; } = true;

		public void NoMore()
		{
			IsOpen = false;
			_closed.SetResult( true );
		}

		public async Task<bool> MoveNextAsync( CancellationToken cancellationToken = new CancellationToken() )
		{
			cancellationToken.ThrowIfCancellationRequested();
			while ( _items.Count == 0 )
			{
				if ( !IsOpen && _items.Count == 0 )
					return false;

				if ( !_pendingItem.Task.IsCompleted )
				{
					using ( var cancelTask = new CancellableTask( cancellationToken ) )
					{
						await Task.WhenAny( _pendingItem.Task, _closed.Task, cancelTask.Task );

						if ( !IsOpen && _items.Count == 0 )
							return false;

						cancellationToken.ThrowIfCancellationRequested();
					}
				}

				_pendingItem = new TaskCompletionSource<bool>();
			}

			AdvanceCurrent();

			return true;
		}

		private void AdvanceCurrent()
		{
			Current = _items.Dequeue();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			_items.Clear();
		}

		public bool MoveNext()
		{
			var task = MoveNextAsync();

			try
			{
				task.Wait();
			}
			catch ( AggregateException ae )
			{
				ExceptionDispatchInfo.Capture( ae.InnerException ).Throw();
			}

			return task.Result;
		}

		public void Reset()
		{
			_items.Clear();
		}

		public T Current { get; private set; }

		object IEnumerator.Current => Current;

		public void AddRange( IEnumerable<T> values )
		{
			foreach ( var value in values )
				Add( value );
		}
	}
}