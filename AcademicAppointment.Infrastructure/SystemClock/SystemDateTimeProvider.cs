using AcademicAppointment.Application.Interfaces;

namespace AcademicAppointment.Infrastructure.SystemClock
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

