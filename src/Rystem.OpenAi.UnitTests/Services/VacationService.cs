using System.ComponentModel;

namespace Rystem.OpenAi.UnitTests.Services
{
    public sealed class UserSettings
    {
        [Description("These days are the days that the user is not able to get in the request")]
        public List<DateOnly> DatesToAvoid { get; set; }
    }
    public sealed class Approver
    {
        [Description("These are the emails that are able to approve the request")]
        public List<string> Emails { get; set; }
    }
    public sealed class VacationRequest
    {
        [Description("The user that is making the request")]
        public string UserId { get; set; }
        [Description("The email that the user is requesting to be approved")]
        public List<string> Approvers { get; set; }
        [Description("The date that the user is requesting to be approved")]
        public DateOnly From { get; set; }
        [Description("The date that the user is requesting to be approved")]
        public DateOnly To { get; set; }
    }
    internal sealed class VacationService
    {
        public List<DateOnly> GetAvailableDates(string userId)
        {
            //get the current year, and put all sundays and saturdays in a list
            var dates = new List<DateOnly>();
            for (var i = 1; i <= 12; i++)
            {
                for (var j = 1; j <= DateTime.DaysInMonth(DateTime.Now.Year, i); j++)
                {
                    var date = new DateOnly(DateTime.Now.Year, i, j);
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        dates.Add(date);
                }
            }
            return dates;
        }
        public Approver GetApprovers(string userId)
        {
            //create 10 random emails and put them in a list
            var emails = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                var email = $"{Guid.NewGuid()}@gmail.com";
                emails.Add(email);
            }
            return new Approver
            {
                Emails = emails
            };
        }
        public bool MakeRequest(string userId, DateOnly from, DateOnly to, Approver approver)
        {
            //create a request with the userId, from date, to date and approvers
            var request = new VacationRequest
            {
                UserId = userId,
                From = from,
                To = to,
                Approvers = approver.Emails,
            };
            //save the request in the database
            return true;
        }
    }
}
