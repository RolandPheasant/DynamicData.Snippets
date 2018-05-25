using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Virtualise
{
	public class PagingListWithVirtualise : AbstractNotifyPropertyChanged, IDisposable
	{
		public IObservableList<Animal> Virtualised { get; }

		private readonly IDisposable _cleanUp;

		public PagingListWithVirtualise(IObservableList<Animal> source, IObservable<IVirtualRequest> requests)
		{
			Virtualised = source.Connect()
				.Virtualise(requests)
				.AsObservableList();

			_cleanUp = Virtualised;
		}

		public void Dispose()
		{
			_cleanUp.Dispose();
		}

	}
}
