using System.Diagnostics;
using Interview.Test;
using Interview.Test.Reports;

var csvFilePath = args[0];

// Можно и асинхронно, но т.к. консоль...
// Не приводил к листу, чтоб можно было обе части замерить стопвотчем читая каждый раз и файла
var lines = File.ReadLines(csvFilePath);

// 1 часть
var sessionsReport = new ReportSessionMaxActivityPerDays();
var activities = sessionsReport.Create(lines.ToSessionMaxActivityRequests());

foreach (var result in activities)
{
    Console.WriteLine($"{result.Day} {result.SessionsCount}");
}

// 2 часть
var operatorsInStateReport = new ReportOperatorMinutesPerStates();
var operatorsInState = operatorsInStateReport.Create(lines.ToOperatorMinutesPerStateRequests());
var minutesFormat = "0";

foreach (var result in operatorsInState)
{
    var minutesPerStates = result.TimePerStates.Select(x => x.TotalMinutes.ToString(minutesFormat)).ToList();
    
    Console.WriteLine($"{result.Operator} {string.Join(' ', minutesPerStates)}");
}
