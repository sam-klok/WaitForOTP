using System;
using System.IO;
using System.Threading;

class ProgWithThreads
{
    private static int _randomNumber;
    private static string? _userInput;
    private static bool _inputReceived = false;
    private static readonly object _lock = new object();
    private static readonly ManualResetEvent _inputEvent = new ManualResetEvent(false);
    private static readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);

    static void Main()
    {
        // 1. Generate random OTP
        var random = new Random();
        _randomNumber = random.Next(1000, 9999);
        Console.WriteLine($"OTP {_randomNumber}");

        // 2. Start countdown thread
        Thread countdownThread = new Thread(CountdownLoop);
        countdownThread.Start();

        // 3. Start input thread (non-blocking for main)
        Thread inputThread = new Thread(InputLoop);
        inputThread.Start();

        // 4. Wait for input or timeout (10 seconds)
        bool inputTimedOut = !_inputEvent.WaitOne(10000); // Blocks main for 10s max

        // Signal threads to stop
        _stopEvent.Set();

        // Wait for threads to finish
        countdownThread.Join();
        inputThread.Join();

        // 5. Check result
        lock (_lock)
        {
            if (inputTimedOut || !_inputReceived || string.IsNullOrEmpty(_userInput))
            {
                Console.WriteLine("\nYou missed the time to enter OTP");
                return;
            }

            if (_userInput == _randomNumber.ToString())
            {
                Console.WriteLine("\nOTP is correct!");
            }
            else
            {
                Console.WriteLine($"\nOTP is incorrect. (Entered: {_userInput})");
            }
        }
    }

    static void CountdownLoop()
    {
        for (int i = 10; i > 0; i--)
        {
            if (_stopEvent.WaitOne(0)) break; // Check if stop signaled

            lock (_lock)
            {
                Console.Write($"\rPlease wait... {i} seconds left ");
            }

            Thread.Sleep(1000);
        }

        if (!_inputReceived)
        {
            lock (_lock)
            {
                Console.WriteLine(); // Newline after countdown
            }
        }
    }

    static void InputLoop()
    {
        // Wait a tiny bit to let countdown start, then prompt
        Thread.Sleep(100); // Optional: Avoid race with initial output

        lock (_lock)
        {
            Console.WriteLine(); // Ensure newline before prompt
            Console.Write("Enter the OTP: ");
        }

        // Use ReadLineAsync with a timeout via Wait
        var readTask = Console.In.ReadLineAsync();
        bool completedInTime = readTask.Wait(10000); // Wait up to 10 seconds
        string? input = completedInTime ? readTask.Result : null;

        lock (_lock)
        {
            _userInput = input;
            _inputReceived = !string.IsNullOrEmpty(input);
        }

        _inputEvent.Set(); // Always signal completion (success or timeout)
    }
}