namespace EPR.Payment.Portal.Common.Configuration
{
    public class DashboardConfiguration
    {
        public static string SectionName => "Dashboard";

        public Service RPDRootUrl { get; set; } = new Service();
        public Service RepExpRootUrl { get; set; } = new Service();

        public Service MenuUrl { get; set; } = new Service();
        public Service BackUrl { get; set; } = new Service();
        public Service RepExpBackUrl { get; set; } = new Service();
        public Service RegistrationTaskList { get; set; } = new Service();
        public Service FeedbackUrl { get; set; } = new Service();
        public Service OfflinePaymentUrl { get; set; } = new Service();
        public Service SignOutUrl { get; set; } = new Service();
    }

    public class Service
    {
        public string? Url { get; set; }
        public string? Description { get; set; }
    }
}