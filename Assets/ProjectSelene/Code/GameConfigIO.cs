namespace ProjectSelene.Code
{
    public static class GameConfigIO
    {
        public static GameConfigData FromAsset(GameConfig cfg) => new GameConfigData {
            GravitationalPull = cfg.gravitationalPull,
            Mass = cfg.mass,
            LinearDamping = cfg.linearDamping,
            MaxTank = cfg.maxTank,
            FuelCost = cfg.fuelCost,
            BaseThrust = cfg.baseThrust,
            MainThrust = cfg.mainThrust,
            SideThrust = cfg.sideThrust,
            SafeLandingSpeed = cfg.safeLandingSpeed,
        };

        public static void Apply(GameConfigData data, GameConfig target)
        {
            target.gravitationalPull = data.GravitationalPull;
            target.mass = data.Mass;
            target.linearDamping = data.LinearDamping;
            target.maxTank = data.MaxTank;
            target.fuelCost = data.FuelCost;
            target.baseThrust = data.BaseThrust;
            target.mainThrust = data.MainThrust;
            target.sideThrust = data.SideThrust;
            target.safeLandingSpeed = data.SafeLandingSpeed;
        }
    }
}