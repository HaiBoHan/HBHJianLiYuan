

-- HRTest


select 
	checkin.Department as Department
	,checkin.Status as Status
	,checkin.CurrentOperator as CurrentOperator

	,checkinLine.StaffMember as Staff
	,person.PersonID as StaffCode
	,person.Name as StaffName
	
	,checkinLine.FullTimeDay as FullTimeDay
	,checkinLine.PartTimeDay as PartTimeDay
	,checkinLine.HourlyDay as HourlyDay

from Cust_DayCheckIn checkin
	 inner join Cust_DayCheckInLine checkinLine
	 on checkin.ID = checkinLine.DayCheckIn

	 left join CBO_Person person
	 on checkinLine.StaffMember = person.ID



