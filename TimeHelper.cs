using System;
using System.Windows.Threading;

namespace QuizGame1WPF
{
    /// <summary>
    /// Provides timer functionality for quiz questions.
    /// </summary>
    public class TimerHelper
    {
        private DispatcherTimer _timer;
        private int _timeLeft;

        /// <summary>
        /// Occurs every second with the remaining time.
        /// </summary>
        public event Action<int>? OnTick;
        /// <summary>
        /// Occurs when the timer reaches zero.
        /// </summary>
        public event Action? OnTimeUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerHelper"/> class.
        /// </summary>
        public TimerHelper()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Starts the timer with the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds to count down from.</param>
        public void Start(int seconds)
        {
            _timeLeft = seconds;
            _timer.Start();
            OnTick?.Invoke(_timeLeft);
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Handles the timer tick event.
        /// </summary>
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
