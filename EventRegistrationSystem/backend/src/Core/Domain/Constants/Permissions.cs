namespace EventRegistrationSystem.Domain.Constants;

public static class Permissions
{
    public static class Events
    {
        public const string Create = "Events.Create";
        public const string View = "Events.View";
        public const string ViewAll = "Events.ViewAll"; // For Admin
        public const string Update = "Events.Update";
        public const string Delete = "Events.Delete";
        public const string Register = "Events.Register";
    }

    public static class Organizers
    {
        public const string View = "Organizers.View";
    }
}
