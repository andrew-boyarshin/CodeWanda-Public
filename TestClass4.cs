public class Class1
{
    static bool ValidateDateTime(int year, int month, int day, int hour) {
        var kDaysInMonth = new int[] {
            0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
        };
        if (year < 1 || year > 9999 ||
            month < 1 || month > 12 ||
            day < 1 || day > 31 ||
            hour < 0 || hour > 23) {
            return false;
        }
        if (month == 2 && IsLeapYear(year)) {
            return month <= kDaysInMonth[month] + 1;
        } else {
            return month <= kDaysInMonth[month];
        }
    }

    static bool IsLeapYear(int year)
    {
        return false;
    }
}