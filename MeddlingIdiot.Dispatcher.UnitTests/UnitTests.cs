using FluentAssertions;

namespace MeddlingIdiot.Dispatcher.UnitTests;

public class UnitTests
{
    [Test]
    public void Value_ReturnsUnit()
    {
        var unit = Unit.Value;
        unit.Should().Be(Unit.Value);
    }

    [Test]
    public async Task Task_ReturnsCompletedTaskWithUnit()
    {
        var result = await Unit.Task;
        result.Should().Be(Unit.Value);
    }

    [Test]
    public void CompareTo_AnotherUnit_ReturnsZero()
    {
        Unit.Value.CompareTo(Unit.Value).Should().Be(0);
    }

    [Test]
    public void CompareTo_AsIComparable_ReturnsZero()
    {
        IComparable unit = Unit.Value;
        unit.CompareTo(Unit.Value).Should().Be(0);
    }

    [Test]
    public void CompareTo_AsIComparable_WithNull_ReturnsZero()
    {
        IComparable unit = Unit.Value;
        unit.CompareTo(null).Should().Be(0);
    }

    [Test]
    public void GetHashCode_ReturnsZero()
    {
        Unit.Value.GetHashCode().Should().Be(0);
    }

    [Test]
    public void Equals_AnotherUnit_ReturnsTrue()
    {
        Unit.Value.Equals(Unit.Value).Should().BeTrue();
    }

    [Test]
    public void Equals_BoxedUnit_ReturnsTrue()
    {
        Unit.Value.Equals((object)Unit.Value).Should().BeTrue();
    }

    [Test]
    public void Equals_NonUnit_ReturnsFalse()
    {
        Unit.Value.Equals("not a unit").Should().BeFalse();
    }

    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        Unit.Value.Equals(null).Should().BeFalse();
    }

    [Test]
    public void EqualityOperator_TwoUnits_ReturnsTrue()
    {
        (Unit.Value == Unit.Value).Should().BeTrue();
    }

    [Test]
    public void InequalityOperator_TwoUnits_ReturnsFalse()
    {
        (Unit.Value != Unit.Value).Should().BeFalse();
    }

    [Test]
    public void ToString_ReturnsParentheses()
    {
        Unit.Value.ToString().Should().Be("()");
    }
}
