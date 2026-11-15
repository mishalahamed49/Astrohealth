using AstroHealth.Server.Models;

namespace AstroHealth.Server.Services;

public sealed class ThresholdEvaluator
{
    public void Evaluate(HealthReading reading)
    {
        reading.Alerts.Clear();

        if (reading.HeartRate is < 50 or > 110)
            reading.Alerts.Add("Heart rate out of range");

        if (reading.BloodPressureSystolic is < 90 or > 140)
            reading.Alerts.Add("Systolic blood pressure abnormal");

        if (reading.BloodSugar is < 70 or > 180)
            reading.Alerts.Add("Blood sugar out of range");

        if (reading.BodyTemperature is < 36.0 or > 38.5)
            reading.Alerts.Add("Temperature abnormal");

        if (reading.SleepHours is < 6)
            reading.Alerts.Add("Insufficient sleep — consider recovery guidance");

        if (reading.MoodScore is <= 3)
            reading.Alerts.Add("Low mood — behavioral health attention recommended");

        if (reading.OxygenSaturation is < 95)
            reading.Alerts.Add("Low oxygen saturation — check oxygen supply");

        reading.OverallStatus = reading.Alerts.Count switch
        {
            0 => "Normal",
            <= 2 => "Warning",
            _ => "Critical"
        };
    }
}
