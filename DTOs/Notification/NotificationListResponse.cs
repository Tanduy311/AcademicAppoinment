namespace AcademicAppoinment.DTOs.Notification
{
    public class NotificationListResponse
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public List<NotificationResponse> Items { get; set; }
    }
}
