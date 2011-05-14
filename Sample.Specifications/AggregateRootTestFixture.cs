using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain;
using CommonDomain.Core;
using Sample.Messages;
using NUnit.Framework;

namespace Sample.Specifications
{

	public class SpecificationAttribute : TestFixtureAttribute { }
	public class GivenAttribute : SetUpAttribute { }
	public class ThenAttribute : TestAttribute { }
	public class AndAttribute : ThenAttribute { }

	public static class AggregateHelpers
	{
		public static void LoadsFromHistory(this IAggregate aggregate, IEnumerable<IEvent> history)
		{
			foreach (var @event in history)
			{
				aggregate.ApplyEvent(@event);
			}
		}
	}

	[Specification]
	public abstract class AggregateRootTestFixture<TAggregateRoot> where TAggregateRoot : IAggregate, new()
	{

		protected TAggregateRoot AggregateRoot { get; set; }

		protected Exception CaughtException { get; private set; }

		protected IEnumerable<IEvent> PublishedEvents { get; private set; }
		// protected ICollection PublishedEvents { get; private set; }

		protected virtual IEnumerable<IEvent> Given()
		{
			return null;
		}

		protected virtual void Finally() { }

		protected abstract void When();

		[Given]
		public void Setup()
		{
		
			AggregateRoot = new TAggregateRoot();
			PublishedEvents = new IEvent[0];

			var history = Given();
			if (history != null)
			{
				AggregateRoot.LoadsFromHistory(history);
			}

			try
			{
				When();
				PublishedEvents = (IEnumerable<IEvent>)AggregateRoot.GetUncommittedEvents();
			}
			catch (Exception exception)
			{
				CaughtException = exception;
			}
			finally
			{
				Finally();
			}
		}
	}
}
