namespace ProjectSelene.Code
{
    public static class GameConfigIO
    {
        public static GameConfigData FromAsset(GameConfig cfg) => new GameConfigData {
            GravitationalPull = cfg.gravitationalPull,
            Mass = cfg.mass,
            LinearDamping = cfg.linearDamping,
            MaxTank = cfg.maxFuel,
            FuelCost = cfg.fuelCost,
            MainThrustFuelFactor = cfg.mainThrustFuelFactor,
            MainThrust = cfg.mainThrust,
            SideThrust = cfg.sideThrust,
            SafeLandingSpeed = cfg.safeLandingSpeed,
        };

        public static void Apply(GameConfigData data, GameConfig target)
        {
            target.gravitationalPull = data.GravitationalPull;
            target.mass = data.Mass;
            target.linearDamping = data.LinearDamping;
            target.maxFuel = data.MaxTank;
            target.fuelCost = data.FuelCost;
            target.mainThrustFuelFactor = data.MainThrustFuelFactor;
            target.mainThrust = data.MainThrust;
            target.sideThrust = data.SideThrust;
            target.safeLandingSpeed = data.SafeLandingSpeed;
        }
    }
}