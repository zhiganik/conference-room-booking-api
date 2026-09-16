namespace ConferenceRoomBooking.LoadTesting.Traffic;

public sealed class WeightedRandomPicker<T>
{
    private readonly (T Item, double CumulativeWeight)[] _entries;
    private readonly double _totalWeight;

    public WeightedRandomPicker(IReadOnlyList<(T Item, double Weight)> entries)
    {
        if (entries.Count == 0)
        {
            throw new ArgumentException("At least one weighted entry is required.", nameof(entries));
        }

        _entries = new (T, double)[entries.Count];
        var cumulative = 0.0;
        for (var i = 0; i < entries.Count; i++)
        {
            cumulative += entries[i].Weight;
            _entries[i] = (entries[i].Item, cumulative);
        }

        _totalWeight = cumulative;
    }

    public T Pick(Random random)
    {
        var roll = random.NextDouble() * _totalWeight;
        foreach (var (item, cumulativeWeight) in _entries)
        {
            if (roll <= cumulativeWeight)
            {
                return item;
            }
        }

        return _entries[^1].Item;
    }
}
