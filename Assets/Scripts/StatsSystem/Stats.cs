public enum StatType {MoraleStrenght, MeleeAttack, MeleeDefense , Precision, Speed}

public class Stats
{
    readonly StatsMediator mediator;

    //STATS
    readonly STATS baseStats;
    public struct STATS
    {
        public int MoraleStrenght;
        public int MeleeAttack;
        public int MeleeDefense;
        public int Precision;
        public int Speed;
    }

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

    public Stats(StatsMediator mediator, CompanyTemplate unitTemplate)
    {
        this.mediator = mediator;

        baseStats = new STATS();
        baseStats.MoraleStrenght = unitTemplate.BaseMorale;
        baseStats.MeleeAttack = unitTemplate.MeleeAttack;
        baseStats.MeleeDefense = unitTemplate.MeleeDefense;
        baseStats.Precision = unitTemplate.Precision;
        baseStats.Speed = unitTemplate.Speed;
    }
    public Stats(StatsMediator mediator, ArtilleryBatteryTemplate unitTemplate)
    {
        this.mediator = mediator;

        baseStats = new STATS();
        baseStats.MoraleStrenght = unitTemplate.BaseMorale;
        baseStats.MeleeAttack = unitTemplate.MeleeAttack;
        baseStats.MeleeDefense = unitTemplate.MeleeDefense;
        baseStats.Precision = unitTemplate.Precision;
        baseStats.Speed = unitTemplate.Speed;
    }

    public override string ToString() => 
        $"Morale: {MoraleStrenght}, " +
        $"MeleeAttack: {MeleeAttack}, " +
        $"MeleeDefense: {MeleeDefense}" +
        $"Precision: {Precision}" +
        $"Speed: {Speed}";
}