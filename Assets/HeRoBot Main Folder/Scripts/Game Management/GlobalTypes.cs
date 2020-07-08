namespace GlobalTypes

{
    public enum GroundType
    {
        None,
        Ground,
        OneWayPlatform,
        MovingPlatform,
        CollapsablePlatform,
        JumpPad,
    }

    public enum EffectorType
    {
        None,
        rope,
        swing,
        FloatZone
    }

    public enum TagTypes
    {
        MovingPlatform,
        CollapsiblePlatform,
        //Projectile, //spawn an object and add force in a direction
        //SelfPropelledProjectile, //spawn an object but not with added foorce
        //SingleShot, //uses Raycast one tick at a time
        //BurstShot, //uses Raycast hit but with a short burst
        //Laser //continous beam

    }

    public enum SoundEffects
    {
        normalStep1, normalStep2, metalStep1, metalStep2, lazer1, lazer2, lazer3, jump, normalFall, fallOnMetal,
        explode, InAir
    }
}
