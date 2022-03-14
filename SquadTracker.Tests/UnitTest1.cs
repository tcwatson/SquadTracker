using System;
using Xunit;

namespace SquadTracker.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(1, 1);
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(1, 0);
        }
    }
}