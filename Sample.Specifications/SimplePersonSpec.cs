using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sample.Messages;
using Sample.DomainModel.People;
using Sample.Messages.Events.People;

namespace Sample.Specifications
{
	[Specification]
	public class SimplePersonSpec : AggregateRootTestFixture<Person>
	{
		private string newstreet = "NewStreet1";
		private string newstreetnumber = "1";

		protected override IEnumerable<IEvent> Given() 
		{
			return new List<IEvent>() { new PersonCreated(Guid.NewGuid() ,"PersonName","Street","0") };
		}

		protected override void When()
		{
			this.AggregateRoot.MoveToAddress(new Address(newstreet, newstreetnumber));
		}

		[Then]
		public void the_person_should_have_moved_house()
		{
			Assert.That(this.PublishedEvents.Count<IEvent>() == 1);

			Console.WriteLine("count:\t{0}\r\nthing:\t{1}", PublishedEvents.Count<IEvent>(), PublishedEvents);
			Console.WriteLine("lastevent:\t{0}", PublishedEvents.Last<IEvent>());
		}
	}

	[Specification]
	public class PersonShouldHaveMovedHouseTwice : AggregateRootTestFixture<Person>
	{
		private Guid UserId;
		private string newaddressStreet0 = "Street0";
		private string newaddressNumber0 = "0";
		private string newaddressStreet1 = "Street1";
		private string newaddressNumber1 = "1";
		private string newaddressStreet2 = "Street2";
		private string newaddressNumber2 = "2";

		protected override IEnumerable<IEvent> Given()
		{
			UserId = Guid.NewGuid();

			return new List<IEvent>() { 
				new PersonCreated(UserId , "PersonName", "Street", "0"),
 				new PersonMoved(UserId, newaddressStreet0, newaddressNumber0, newaddressStreet1, newaddressNumber1)
			};
		}

		protected override void When()
		{
			AggregateRoot.MoveToAddress(new Address(newaddressStreet2, newaddressNumber2));
		}

		[Then]
		public void the_person_should_have_moved_house_twice()
		{
			var theevents = this.PublishedEvents;
			var finalevent = (PersonMoved)theevents.Last<IEvent>(e => e.GetType() == typeof(PersonMoved));
			Console.WriteLine("Count: {0}", this.PublishedEvents.Count<IEvent>());
			Assert.That(finalevent.GetType() == typeof(PersonMoved));
			Assert.That(finalevent.NewNumber == newaddressNumber2);
			Assert.That(finalevent.NewStreet == newaddressStreet2);
			Assert.That(finalevent.OldNumber == newaddressNumber1);
			Assert.That(finalevent.OldStreet == newaddressStreet1);

			Console.WriteLine(@"type	{0}
id		{1}
newnum	{2}
newst	{3}
oldnum	{4}
oldst	{5}",
				finalevent.GetType().ToString(),
				finalevent.Id,
				finalevent.NewNumber,
				finalevent.NewStreet,
				finalevent.OldNumber,
				finalevent.OldStreet);
		}
	}
}
