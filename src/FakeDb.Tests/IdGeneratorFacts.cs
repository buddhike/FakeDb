﻿using Xunit;

namespace FakeDb.Tests
{
    public class IdGeneratorFacts
    {
        public class IdentifyMethod
        {
            [Fact]
            public void AssignsAnIdToIdProperty()
            {
                var instance = new Person();
                
                instance = (Person)new IdGenerator().Identify(instance);

                Assert.Equal(1, instance.Id);
            }

            [Fact]
            public void ShouldNotAssignIdToInitialisedIdProperty()
            {
                var instance = new Person{Id = 9};

                instance = (Person)new IdGenerator().Identify(instance);

                Assert.Equal(9, instance.Id);
            }

            [Fact]
            public void DoesNotThrowAnExceptionIfIdPropertyIsNotAvailable()
            {
                new IdGenerator().Identify(new {Name = "Foo"});
            }

            [Fact]
            public void SetsTheIdEvenIfTheIdTypeIsString()
            {
                var a = (Address)new IdGenerator().Identify(new Address());
                Assert.Equal("1", a.Id);
            }
        }
    }
}