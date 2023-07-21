namespace Fold.Motor.Timers;

public class PlayerTimer
{
    private System.Timers.Timer _timer;
    private double _increment;
    private DateTime _dueTime = DateTime.Now;

    public double TimeLeft => (_dueTime - DateTime.Now).TotalMilliseconds;
    public delegate void OnTimeLost();
    public event OnTimeLost OnTimeLostEvent;


    public PlayerTimer(double interval, double increment, OnTimeLost onTimeLostGameResolution)
    {
        _timer = new System.Timers.Timer(increment);
        _timer.AutoReset = false;
        _timer.Elapsed += ( sender, e ) => { OnTimeLostEvent?.Invoke(); };

        _increment = increment;

        _dueTime = DateTime.Now.AddMilliseconds(interval);

        OnTimeLostEvent += onTimeLostGameResolution;
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }

    public void Enable()
    {
        _timer.Enabled = true;
    }

    public void Disable() 
    {
        _timer.Enabled = false;
        _timer.Interval = TimeLeft + _increment;
        _dueTime = DateTime.Now.AddMilliseconds(_timer.Interval);
    }

    public void AddTime(double increment) 
    {
        _timer.Enabled = false;
        _timer.Interval = TimeLeft + increment;
        _dueTime = DateTime.Now.AddMilliseconds(_timer.Interval);
        _timer.Enabled = true;
    }
}