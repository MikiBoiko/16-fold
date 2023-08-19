namespace Fold.Motor;

public class PlayerTimer
{
    public readonly double interval, increment;

    private System.Timers.Timer _timer;
    public DateTime DueTime { set; get; }

    public double TimeLeft => (DueTime - DateTime.Now).TotalMilliseconds;
    public delegate void OnTimeLost();
    public event OnTimeLost OnTimeLostEvent;


    public PlayerTimer(double interval, double increment, OnTimeLost onTimeLostGameResolution)
    {
        this.interval = interval;
        this.increment = increment;

        _timer = new System.Timers.Timer(interval);
        _timer.AutoReset = false;
        _timer.Elapsed += ( sender, e ) => { OnTimeLostEvent?.Invoke(); };

        DueTime = DateTime.Now.AddMilliseconds(interval);
        OnTimeLostEvent += onTimeLostGameResolution;
    }

    public void Restart()
    {
        _timer.Enabled = false;
        _timer.Interval = interval;
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }

    public void Enable()
    {
        DueTime = DateTime.Now.AddMilliseconds(_timer.Interval);
        _timer.Enabled = true;
    }

    public void Disable() 
    {
        if(_timer.Enabled)
        {
            _timer.Enabled = false;
            _timer.Interval = TimeLeft + increment;
        }
    }

    public void AddTime(double increment) 
    {
        if(_timer.Enabled)
        {
            _timer.Enabled = false;
            _timer.Interval = TimeLeft + increment;
        }
        else 
        {
            _timer.Interval += increment;
        }
        DueTime = DateTime.Now.AddMilliseconds(_timer.Interval);
        _timer.Enabled = true;
    }

    #region State
    public class State
    {
        public double Interval { set; get; }
    }

    public State GetState(bool enabled)
    {
        return new State {
            Interval = enabled ? TimeLeft : interval
        };
    }
    #endregion
}