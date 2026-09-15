using AgeCalculator.Api.Features.Welcome;

namespace AgeCalculator.Api.Tests;

public class AgeCalculatorTests
{
    [Fact]
    public void Calculate_OrdinaryDate_ReturnsCorrectAge()
    {
        var birthDate = new DateTime(1990, 5, 15);
        var today = new DateTime(2026, 9, 15);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(36, years);
        Assert.Equal(4, months);
        Assert.Equal(0, days);
        Assert.False(isBirthday);
    }

    [Fact]
    public void Calculate_OrdinaryDate_PartialMonth()
    {
        var birthDate = new DateTime(1990, 5, 15);
        var today = new DateTime(2026, 7, 20);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(36, years);
        Assert.Equal(2, months);
        Assert.Equal(5, days);
        Assert.False(isBirthday);
    }

    [Fact]
    public void Calculate_BirthdayToday_ReturnsIsBirthdayTrue()
    {
        var birthDate = new DateTime(1990, 5, 15);
        var today = new DateTime(2026, 5, 15);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(36, years);
        Assert.Equal(0, months);
        Assert.Equal(0, days);
        Assert.Equal(0, daysUntilNextBirthday);
        Assert.True(isBirthday);
    }

    [Fact]
    public void Calculate_BirthdayTomorrow_ReturnsDaysUntilNextBirthdayOne()
    {
        var birthDate = new DateTime(1990, 5, 15);
        var today = new DateTime(2026, 5, 14);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.False(isBirthday);
        Assert.Equal(1, daysUntilNextBirthday);
    }

    [Fact]
    public void Calculate_BirthdayYesterday_ReturnsDaysUntilNextBirthday()
    {
        var birthDate = new DateTime(1990, 5, 15);
        var today = new DateTime(2026, 5, 16);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.False(isBirthday);
        Assert.Equal(364, daysUntilNextBirthday);
    }

    [Fact]
    public void Calculate_BirthdayLastDayOfYear_ReturnsDaysUntilNextBirthday()
    {
        var birthDate = new DateTime(1990, 12, 31);
        var today = new DateTime(2026, 12, 31);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(36, years);
        Assert.Equal(0, months);
        Assert.Equal(0, days);
        Assert.Equal(0, daysUntilNextBirthday);
        Assert.True(isBirthday);
    }

    [Fact]
    public void Calculate_BirthdayFirstDayOfYear()
    {
        var birthDate = new DateTime(1990, 1, 1);
        var today = new DateTime(2026, 1, 1);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(36, years);
        Assert.Equal(0, months);
        Assert.Equal(0, days);
        Assert.Equal(0, daysUntilNextBirthday);
        Assert.True(isBirthday);
    }

    [Fact]
    public void Calculate_FutureBirthDate_ReturnsNegativeYears()
    {
        var birthDate = new DateTime(2030, 5, 15);
        var today = new DateTime(2026, 9, 15);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.True(years < 0);
        Assert.False(isBirthday);
    }

    [Fact]
    public void Calculate_Feb29_BirthInLeapYear_CelebratesOnFeb29()
    {
        var birthDate = new DateTime(2000, 2, 29);
        var today = new DateTime(2024, 2, 29);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(24, years);
        Assert.Equal(0, months);
        Assert.Equal(0, days);
        Assert.Equal(0, daysUntilNextBirthday);
        Assert.True(isBirthday);
    }

    [Fact]
    public void Calculate_Feb29_BirthInLeapYear_CelebratesOnFeb28InNonLeapYear()
    {
        var birthDate = new DateTime(2000, 2, 29);
        var today = new DateTime(2025, 2, 28);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(24, years);
        Assert.Equal(11, months);
        Assert.Equal(30, days);
        Assert.Equal(0, daysUntilNextBirthday);
        Assert.True(isBirthday);
    }

    [Fact]
    public void Calculate_Feb29_DayAfterFeb28InNonLeapYear_ReturnsOneDayUntil()
    {
        var birthDate = new DateTime(2000, 2, 29);
        var today = new DateTime(2025, 3, 1);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.False(isBirthday);
        Assert.Equal(364, daysUntilNextBirthday);
    }

    [Fact]
    public void Calculate_Feb29_DayBeforeFeb28InNonLeapYear_ReturnsOneDayUntil()
    {
        var birthDate = new DateTime(2000, 2, 29);
        var today = new DateTime(2025, 2, 27);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.False(isBirthday);
        Assert.Equal(1, daysUntilNextBirthday);
    }

    [Fact]
    public void Calculate_Feb29_LeapYear_NextBirthdayIsFeb29()
    {
        var birthDate = new DateTime(2000, 2, 29);
        var today = new DateTime(2025, 2, 26);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.False(isBirthday);
        Assert.Equal(2, daysUntilNextBirthday);
    }

    [Fact]
    public void Calculate_SameMonth_BirthAfterToday()
    {
        var birthDate = new DateTime(1990, 9, 20);
        var today = new DateTime(2026, 9, 15);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(35, years);
        Assert.Equal(11, months);
        Assert.Equal(26, days);
        Assert.False(isBirthday);
        Assert.Equal(5, daysUntilNextBirthday);
    }

    [Fact]
    public void GetBirthdayInYear_OrdinaryDate_ReturnsSameDate()
    {
        var birthDate = new DateTime(1990, 5, 15);

        var result = AgeCalculation.GetBirthdayInYear(birthDate, 2026);

        Assert.Equal(new DateTime(2026, 5, 15), result);
    }

    [Fact]
    public void GetBirthdayInYear_Feb29_LeapYear_ReturnsFeb29()
    {
        var birthDate = new DateTime(2000, 2, 29);

        var result = AgeCalculation.GetBirthdayInYear(birthDate, 2024);

        Assert.Equal(new DateTime(2024, 2, 29), result);
    }

    [Fact]
    public void GetBirthdayInYear_Feb29_NonLeapYear_ReturnsFeb28()
    {
        var birthDate = new DateTime(2000, 2, 29);

        var result = AgeCalculation.GetBirthdayInYear(birthDate, 2025);

        Assert.Equal(new DateTime(2025, 2, 28), result);
    }

    [Fact]
    public void Calculate_Jan31_FebHasFewerDays()
    {
        var birthDate = new DateTime(1990, 1, 31);
        var today = new DateTime(2026, 2, 28);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(36, years);
        Assert.Equal(0, months);
        Assert.Equal(28, days);
    }

    [Fact]
    public void Calculate_March1_BornFeb28_CorrectDays()
    {
        var birthDate = new DateTime(1990, 2, 28);
        var today = new DateTime(2026, 3, 1);

        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(birthDate, today);

        Assert.Equal(36, years);
        Assert.Equal(0, months);
        Assert.Equal(1, days);
        Assert.False(isBirthday);
    }
}