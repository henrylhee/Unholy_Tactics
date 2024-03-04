
public class MainUnit : PlayerUnit
{
    protected override void OnDeath()
    {
        base.OnDeath();
        Combat.Lose();
    }
}
