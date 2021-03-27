namespace SkippingCounter
{
    public static class Constants
    {
        public const string SerialLogTemplate = "[{Level}] {Message:l} {Exception:l}";

        public static class PreferenceKeys
        {
            public const string JumpGoal = "JumpGoal";
            public const string JumpThreshold = "JumpThreshold";
            public const string Theme = "Theme";
        }

        public static class Defaults
        {
            public const float JumpThreshold = 3;
        }
    }
}
