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

    public static class Registrations
    {
        public const string Create = "Registrations.Create";
        public const string ViewOwn = "Registrations.ViewOwn";
        public const string CancelOwn = "Registrations.CancelOwn";
        public const string ViewEvent = "Registrations.ViewEvent";
    }
    public static class Users
    {
        public const string ViewAll = "Users.ViewAll";
        public const string UpdateRole = "Users.UpdateRole";
        public const string Delete = "Users.Delete";
    }
}
