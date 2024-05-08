public enum StatType {MoraleStrenght, MeleeAttack, MeleeDefense , Precision, Speed}

public class Stats
{
    readonly StatsMediator mediator;
    readonly UnitTemplate baseStats;

    public StatsMediator Mediator => mediator;

    public int MoraleStrenght
    {
        get
        {
            var q = new Query(StatType.MoraleStrenght, 1);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    public int MeleeAttack
    {
        get
        {
            var q = new Query(StatType.MeleeAttack, baseStats.MeleeAttack);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    public int MeleeDefense
    {
        get
        {
            var q = new Query(StatType.MeleeDefense, baseStats.MeleeDefense);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    public int Precision
    {
        get
        {
            var q = new Query(StatType.Precision, baseStats.Precision);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    public int Speed
    {
        get
        {
            var q = new Query(StatType.Speed, baseStats.Speed);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public Stats(StatsMediator mediator, UnitTemplate baseStats)
    {
        this.mediator = mediator;
        this.baseStats = baseStats;
    }

    public override string ToString() => 
        $"Morale: {MoraleStrenght}, MeleeAttack: {MeleeAttack}, MeleeDefense: {MeleeDefense}";
}