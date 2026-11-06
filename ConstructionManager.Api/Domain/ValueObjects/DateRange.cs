using ConstructionManager.Api.Domain.Exceptions;

namespace ConstructionManager.Api.Domain.ValueObjects
{
    public class DateRange
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد");

            StartDate = startDate;
            EndDate = endDate;
        }

        public int DurationInDays => (EndDate - StartDate).Days;
    }
}