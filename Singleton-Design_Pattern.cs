using System;
using System.Threading;  // Needed for creating multiple threads

// This class implements the Singleton Design Pattern
public class Logger
{
    // 1️. Private constructor: Prevents external classes from creating new instances using "new"
    private Logger()
    {
        Console.WriteLine("Logger created!!!");
    }

    // 2️. Static instance variable
    // Static means it belongs to the class, not an object
    // Only ONE copy exists in memory for the entire application
    private static Logger _instance = null;

    // 3️. Lock object for thread safety
    // readonly ensures the reference cannot be changed
    // Used to prevent multiple threads from creating multiple instances
    public static readonly object _lock = new object();

    // 4️. Public static method to access the instance
    // Static because we must call it without creating an object
    public static Logger GetInstance()
    {
        // First check (performance optimization)
        // If instance already created, no need to lock
        if (_instance == null)
        {
            // Lock ensures only ONE thread enters this block at a time
            lock (_lock)
            {
                // Second check (important!)
                // Prevents second thread from creating another instance
                if (_instance == null)
                {
                    _instance = new Logger();  // Instance created only once
                }
            }
        }

        return _instance;  // Always return the same instance
    }

    // Normal instance method
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}

class Program
{
    public static void Main()
    {
        // Creating two separate threads
        Thread thread1 = new Thread(() =>
        {
            Logger log1 = Logger.GetInstance(); // Both threads call this
        });

        Thread thread2 = new Thread(() =>
        {
            Logger log2 = Logger.GetInstance();
        });

        // Start both threads
        thread1.Start();
        thread2.Start();

        // Wait for both threads to finish
        thread1.Join();
        thread2.Join();
    }
}