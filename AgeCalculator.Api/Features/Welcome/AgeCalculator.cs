namespace AgeCalculator.Api.Features.Welcome;

public static class AgeCalculation
{
    public static DateTime GetBirthdayInYear(DateTime birthDate, int year)
    {
        if (birthDate.Month == 2 && birthDate.Day == 29 && !DateTime.IsLeapYear(year))
        {
            return new DateTime(year, 2, 28);
        }
        return new DateTime(year, birthDate.Month, birthDate.Day);
    }

    public static (int Years, int Months, int Days, int DaysUntilNextBirthday, bool IsBirthday) Calculate(
        DateTime birthDate, DateTime today)
    {
        int years = today.Year - birthDate.Year;
        int months = today.Month - birthDate.Month;
        int days = today.Day - birthDate.Day;

        if (days < 0)
        {
            months--;
            int prevMonth = today.Month == 1 ? 12 : today.Month - 1;
            int prevYear = today.Month == 1 ? today.Year - 1 : today.Year;
            days += DateTime.DaysInMonth(prevYear, prevMonth);
        }
        if (months < 0)
        {
            years--;
            months += 12;
        }

        var thisBirthday = GetBirthdayInYear(birthDate, today.Year);
        bool isBirthday = today.Date == thisBirthday.Date;

        var nextBirthday = thisBirthday;
        if (nextBirthday < today)
        {
            nextBirthday = GetBirthdayInYear(birthDate, today.Year + 1);
        }

        int daysUntilNextBirthday = (nextBirthday - today).Days;

        return (years, months, days, daysUntilNextBirthday, isBirthday);
    }
}