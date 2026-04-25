namespace MeddlingIdiot.Dispatcher.UnitTests;

public class UnitTests
{
    [Test]
    public async Task Value_ReturnsUnit()
    {
        var unit = Unit.Value;
        await Assert.That(unit).IsEqualTo(Unit.Value);
    }

    [Test]
    public async Task Task_ReturnsCompletedTaskWithUnit()
    {
        var result = await Unit.Task;
        await Assert.That(result).IsEqualTo(Unit.Value);
    }

    [Test]
    public async Task CompareTo_AnotherUnit_ReturnsZero()
    {
        await Assert.That(Unit.Value.CompareTo(Unit.Value)).IsEqualTo(0);
    }

    [Test]
    public async Task CompareTo_AsIComparable_ReturnsZero()
    {
        IComparable unit = Unit.Value;
        await Assert.That(unit.CompareTo(Unit.Value)).IsEqualTo(0);
    }

    [Test]
    public async Task CompareTo_AsIComparable_WithNull_ReturnsZero()
    {
        IComparable unit = Unit.Value;
        await Assert.That(unit.CompareTo(null)).IsEqualTo(0);
    }

    [Test]
    public async Task GetHashCode_ReturnsZero()
    {
        await Assert.That(Unit.Value.GetHashCode()).IsEqualTo(0);
    }

    [Test]
    public async Task Equals_AnotherUnit_ReturnsTrue()
    {
        await Assert.That(Unit.Value.Equals(Unit.Value)).IsTrue();
    }

    [Test]
    public async Task Equals_BoxedUnit_ReturnsTrue()
    {
        await Assert.That(Unit.Value.Equals((object)Unit.Value)).IsTrue();
    }

    [Test]
    public async Task Equals_NonUnit_ReturnsFalse()
    {
        await Assert.That(Unit.Value.Equals("not a unit")).IsFalse();
    }

    [Test]
    public async Task Equals_Null_ReturnsFalse()
    {
        await Assert.That(Unit.Value.Equals(null)).IsFalse();
    }

    [Test]
    public async Task EqualityOperator_TwoUnits_ReturnsTrue()
    {
        await Assert.That(Unit.Value == Unit.Value).IsTrue();
    }

    [Test]
    public async Task InequalityOperator_TwoUnits_ReturnsFalse()
    {
        await Assert.That(Unit.Value != Unit.Value).IsFalse();
    }

    [Test]
    public async Task ToString_ReturnsParentheses()
    {
        await Assert.That(Unit.Value.ToString()).IsEqualTo("()");
    }
}
