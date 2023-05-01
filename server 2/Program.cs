var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



app.Run();

/*
using System.Timers;

namespace TimerTHing
{
    public class PlayerTimer
    {
        private System.Timers.Timer _timer;
        private double _increment;
        private DateTime _dueTime;

        public double TimeLeft => (this._dueTime - DateTime.Now).TotalMilliseconds;

        public PlayerTimer(double interval, double increment)
        {
            _timer = new System.Timers.Timer(increment);
            _timer.Elapsed += ( sender, e ) => { Console.WriteLine("Tick"); };

            _dueTime = DateTime.Now.AddMilliseconds(increment)
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
        }
    }

Console.WriteLine(timer.Interval);

await Task.Delay(2000);
timer.Enabled = true;


Console.ReadLine();

}
*/