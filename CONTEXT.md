# Age Calculator

The Age Calculator helps a registered person understand their age and celebrate their birthday.

## Language

**User**:
A registered person who owns and manages exactly their own profile and birth date.
_Avoid_: Account, customer, member

**Birth Date**:
The calendar date on which a User was born, without a time or time zone.
_Avoid_: Birthday date, DOB

**Birthday**:
The calendar-day anniversary of a User's Birth Date; a February 29 Birth Date is celebrated on February 28 in a non-leap year.
_Avoid_: Birth date

**Age Summary**:
The User's completed years, months, and days since their Birth Date, together with the number of days until their next Birthday.
_Avoid_: Age calculation, age result

**Birthday Celebration**:
The golden presentation state shown to a User throughout their Birthday in the User's browser-local calendar day.
_Avoid_: King mode, luxury mode

**Profile**:
The User-owned record containing the User's Birth Date.
_Avoid_: Account details, settings

**Welcome Page**:
The authenticated landing page that presents a User's Age Summary or directs a User without a Birth Date to their Profile.
_Avoid_: Dashboard, home screen

**Local Calendar Day**:
The calendar day in the User's current browser time zone, used to determine whether a Birthday Celebration is active.
_Avoid_: Server day, UTC day
