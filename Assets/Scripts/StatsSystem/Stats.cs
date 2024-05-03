public enum StatType { FirePower, Morale, MeleeAttack, MeleeDefense }

public class Stats
{
    readonly StatsMediator mediator;
    readonly BaseStats baseStats;

    public StatsMediator Mediator => mediator;

    public int FirePower
    {
        get
        {
            var q = new Query(StatType.FirePower, baseStats.attack);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    public int Morale
    {
        get
        {
            var q = new Query(StatType.Morale, baseStats.attack);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    public int MeleeAttack
    {
        get
        {
            var q = new Query(StatType.MeleeAttack, baseStats.attack);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    public int MeleeDefense
    {
        get
        {
            var q = new Query(StatType.MeleeDefense, baseStats.attack);
            mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    public Stats(StatsMediator mediator, BaseStats baseStats)
    {
        this.mediator = mediator;
        this.baseStats = baseStats;
    }

    public override string ToString() => 
        $"FirePower: {FirePower}, Morale: {Morale}, MeleeAttack: {MeleeAttack}, MeleeDefense: {MeleeDefense}";
}