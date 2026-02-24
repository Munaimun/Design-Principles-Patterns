using System;

#region ==================== SINGLETON PATTERN ====================

// ILogger abstraction (good practice: depend on interface)
public interface ILogger
{
    void Log(string message);
}

// Logger implemented as Singleton
public class Logger : ILogger
{
    // Private constructor prevents external instantiation
    private Logger()
    {
        Console.WriteLine("Logger created");
    }

    // Static instance (only one per application)
    private static Logger _instance = null;

    // Lock object for thread safety
    private static readonly object _lock = new object();

    // Global access point
    public static Logger GetInstance()
    {
        // First check (avoid unnecessary locking)
        if (_instance == null)
        {
            lock (_lock)
            {
                // Second check (ensures only one instance is created)
                if (_instance == null)
                {
                    _instance = new Logger();
                }
            }
        }

        return _instance;
    }

    public void Log(string message)
    {
        Console.WriteLine("This is from log - {0}", message);
    }
}

#endregion


#region ==================== STRATEGY PATTERN ====================

// Strategy Interface 1
public interface ISend
{
    void Send();
}

// Strategy Interface 2
public interface ILog
{
    void Log();
}

// Strategy Interface 3
public interface ISave
{
    void Save();
}

// Concrete Strategy: Email
public class EmailNotify : ISend, ILog, ISave
{
    public string Email { get; set; }

    public void Send()
    {
        Console.WriteLine("Sending email to " + Email);
    }

    public void Log()
    {
        // Uses Singleton Logger
        ILogger logger = Logger.GetInstance();
        logger.Log("Logging email to " + Email);
    }

    public void Save()
    {
        Console.WriteLine("Saving db to " + Email);
    }
}

// Concrete Strategy: SMS
public class SMSNotify : ISend, ILog, ISave
{
    public string Phone { get; set; }

    public void Send()
    {
        Console.WriteLine("Sending SMS to " + Phone);
    }

    public void Log()
    {
        ILogger logger = Logger.GetInstance();
        logger.Log("Logging SMS to " + Phone);
    }

    public void Save()
    {
        Console.WriteLine("Saving db to " + Phone);
    }
}

// Concrete Strategy: Push
public class PushNotify : ISend, ILog
{
    public string Token { get; set; }

    public void Send()
    {
        Console.WriteLine("Sending Push to " + Token);
    }

    public void Log()
    {
        ILogger logger = Logger.GetInstance();
        logger.Log("Logging Push to " + Token);
    }
}

// Concrete Strategy: WhatsApp
public class WhatsappNotify : ISend, ILog
{
    public string Phone { get; set; }

    public void Send()
    {
        Console.WriteLine("Sending Whatsapp to " + Phone);
    }

    public void Log()
    {
        ILogger logger = Logger.GetInstance();
        logger.Log("Logging Whatsapp to " + Phone);
    }
}

#endregion


#region ==================== CONTEXT (Strategy Context) ====================

// Context class
// Uses strategy interfaces instead of concrete classes
public class NotifyContext
{
    private readonly ISend _send;
    private readonly ILog _log;
    private readonly ISave _save;

    public NotifyContext(ISend send, ILog log, ISave save)
    {
        _send = send;
        _log = log;
        _save = save;
    }

    public void Process()
    {
        _send.Send();
        _log.Log();

        if (_save != null)
        {
            _save.Save();
        }
    }
}

#endregion


#region ==================== FACTORY METHOD PATTERN ====================

// Factory Interface
// Defines method for creating NotifyContext
public interface INotificationContextCreator
{
    NotifyContext Create();
}

// Concrete Factory: Email
public class EmailNotificationContextCreator : INotificationContextCreator
{
    public NotifyContext Create()
    {
        var email = new EmailNotify { Email = "test@test.com" };
        return new NotifyContext(email, email, email);
    }
}

// Concrete Factory: SMS
public class SMSNotificationContextCreator : INotificationContextCreator
{
    public NotifyContext Create()
    {
        var sms = new SMSNotify { Phone = "123456789" };
        return new NotifyContext(sms, sms, sms);
    }
}

// Concrete Factory: Push
public class PushNotificationContextCreator : INotificationContextCreator
{
    public NotifyContext Create()
    {
        var push = new PushNotify { Token = "123456789" };
        return new NotifyContext(push, push, null);
    }
}

// Concrete Factory: WhatsApp
public class WhatsappNotificationContextCreator : INotificationContextCreator
{
    public NotifyContext Create()
    {
        var whatsapp = new WhatsappNotify { Phone = "123456789" };
        return new NotifyContext(whatsapp, whatsapp, null);
    }
}

#endregion


#region ==================== CLIENT ====================

class Program
{
    public static void Main()
    {
        // Client depends only on factory interface
        INotificationContextCreator creator;

        creator = new EmailNotificationContextCreator();
        creator.Create().Process();

        creator = new SMSNotificationContextCreator();
        creator.Create().Process();

        creator = new PushNotificationContextCreator();
        creator.Create().Process();

        creator = new WhatsappNotificationContextCreator();
        creator.Create().Process();
    }
}

#endregion