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


using Sample.Messages.Events.People;
using Sample.DomainModel.People;

namespace Sample.Specifications
{

	public abstract class AggregateRootTestFixture<T> where T : IAggregate, new()
	{

		protected Guid sutId = Guid.NewGuid();

		protected Guid itemId = Guid.NewGuid();
		protected IAggregate root;
		protected Exception caught = null;
		protected T sut;
		protected List<IEvent> events;
		// protected ICollection events;
		// protected Queue events;

		// protected IEnumerable<IEvent> Given(); //TODO: is this the correct implementation?!
		protected abstract IEnumerable<IEvent> Given();
		protected abstract void When();

		[SetUp]
		public void Setup()
		{
		
			root = new T();
			// root.LoadsFromHistory(Given());
			root.LoadsFromHistory(Given());
			try
			{
				When();
				events = new List<IEvent>((IEnumerable<IEvent>)root.GetUncommittedEvents());
				// events.Add(new PersonDied(itemId));
			
				// events = new Queue(root.GetUncommittedEvents()); //NOTE: Get something better here.
			}
			catch (Exception Ex)
			{
				caught = Ex;
			}
		}


	}
	public static class AggregateHelpers
	{
		public static void LoadsFromHistory(this IAggregate aggregate, IEnumerable<IEvent> history)
		{
			foreach (var @event in history)
			{
				aggregate.ApplyEvent(@event);
			}
		}

 		//TODO: Add a helper method to be able to get the uncommitted eventstream.
	}


	[TestFixture]
	public class FirstPersonUseCase : AggregateRootTestFixture<Person>
	{
		// private Person sut;

		protected override IEnumerable<IEvent> Given()
		{
			// var bob = new PersonCreated(itemId, "Bob", "Carlton Street", "4");

			sut = Person.CreatePerson(itemId, "Bob", "Carlton Street", "4");

			var esource = new IEvent[] { 
				// new PersonCreated(itemId, "Bob", "Carlton Street", "4")
			}.AsEnumerable();

			return esource;
		}

		protected override void When()
		{
			sut.MoveToAddress(new Address("Blockstone Road", "43"));
		}

		[Test]
		public void Then_the_address_should_be_at_blockstone_road()
		{
			Console.WriteLine("Uncommitted Root : {0}",root.GetUncommittedEvents().Count);
			if (events != null)
			{
				Console.WriteLine("Events are NOT null :\\ ");
			}
			else
			{
				Console.WriteLine("Events are a type of {0}", events.GetType().ToString());
			}
			Console.WriteLine("Events are a type of {0}", events.ToString());

			PersonMoved p;
			// events.Where<IEvent>(e => e is PersonMoved).Select(e => e); 
			Assert.That( //1 == 1,"");
				events.Count == 1, "There was 1 event expected but there was {0} Events.", events.Count);
			// Assert.That(events.Where(e => typeof e is PersonMoved && (PersonMoved)e ),"");

			// p = events.Where(e => e is IEnumerable<PersonMoved> ).First<PersonMoved>();
			// p = (PersonMoved)events.Peek();
			if (events[0] != null )
			{
				p = (PersonMoved)events[0];
			
			}
			else
			{
				throw new Exception();
			}


			Assert.That(p.Id == itemId, "Wrong Id"); 
			Assert.That(p.NewNumber == "43", "Wrong NewNumber"); 
			Assert.That(p.NewStreet == "Blockstone Road", "Wrong NewStreet"); 
			Assert.That(p.OldNumber == "4", "Wrong OldNumber"); 
			Assert.That(p.OldStreet == "Carlton Street", "Wrong OldSteet");  
		}
	}
}