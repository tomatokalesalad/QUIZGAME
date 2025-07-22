using System;
using System.Windows.Threading;

namespace QuizGame1WPF
{
    public class TimerHelper
    {
        private DispatcherTimer _timer;
        private int _timeLeft;

        public event Action<int>? OnTick;
        public event Action? OnTimeUp;

        public TimerHelper()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }

        public void Start(int seconds)
        {
            _timeLeft = seconds;
            _timer.Start();
            OnTick?.Invoke(_timeLeft);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timeLeft--;

            if (_timeLeft > 0)
                OnTick?.Invoke(_timeLeft);
            else
            {
                _timer.Stop();
                OnTimeUp?.Invoke();
            }
        }
    }
}
