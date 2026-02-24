// What is Factory Method?
// Ans: Factory Method defines an interface for creating an object, but lets subclasses decide which class to instantiate.

using System;

#region Product (The object that will be created)

// This is the main object that factories will create.
// It contains behavior that varies depending on notification type.
public class NotifyContext
{
    private readonly string _type;

    public NotifyContext(string type)
    {
        _type = type;
    }

    public void Process()
    {
        Console.WriteLine($"Processing {_type} notification...");
    }
}

#endregion


#region Creator (Factory Interface)

// This is the Factory Method interface.
// It declares a method for creating objects.
// Subclasses will decide WHICH concrete object to create.
public interface INotificationContextCreator
{
    NotifyContext Create();   // Factory Method
}

#endregion


#region Concrete Creators (Actual Factories)

// Concrete Factory 1
// Responsible for creating Email notification objects.
public class EmailNotificationCreator : INotificationContextCreator
{
    public NotifyContext Create()
    {
        return new NotifyContext("Email");
    }
}

// Concrete Factory 2
public class SmsNotificationCreator : INotificationContextCreator
{
    public NotifyContext Create()
    {
        return new NotifyContext("SMS");
    }
}

// Concrete Factory 3
public class PushNotificationCreator : INotificationContextCreator
{
    public NotifyContext Create()
    {
        return new NotifyContext("Push");
    }
}

#endregion


#region Client Code

class Program
{
    static void Main()
    {
        // Client depends on abstraction (interface), not concrete class.
        INotificationContextCreator creator;

        // You can switch factory easily without changing client logic.
        creator = new EmailNotificationCreator();
        NotifyContext email = creator.Create();
        email.Process();

        creator = new SmsNotificationCreator();
        NotifyContext sms = creator.Create();
        sms.Process();

        creator = new PushNotificationCreator();
        NotifyContext push = creator.Create();
        push.Process();
    }
}

#endregion