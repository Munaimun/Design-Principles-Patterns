using System;
using System.Collections.Generic;
using System.Linq;
using ECommerce.Ordering.System;

// Solid Principles Demo 
namespace ECommerce.Ordering.System
{
    // ----S----
    // SINGLE RESPONSIBILITY PRINCIPLE (SRP)

    // Responsible ONLY for holding order
    public class Order
    {
        public List<OrderItem> Items { get; } = new List<OrderItem>();

        public void AddItem(OrderItem item) => Items.Add(item);
    }

    // Responsible ONLY for holding item data
    public class OrderItem
    {
        public string ProductName { get; }
        public decimal Price { get; }

        public OrderItem(string productName, decimal price)
        {
            ProductName = productName;
            Price = price;
        }
    }


    // Responsible ONLY for calculating order total
    public class OrderPriceCalculator
    {
        public decimal CalculateTotal(Order order)
        {
            decimal total = 0;
            foreach (var item in order.Items)
            {
                total += item.Price;
            }
            return total;
        }
    }


    // ----O, L, I----
    // OPEN/CLOSED + LISKOV + INTERFACE SEGREGATION

    // Payment abstraction (DIP)
    public interface IPaymentMethod
    {
        void ProcessPayment(decimal amount);
    }

    // Credit Card payment implementation
    public class CreditCardPayment : IPaymentMethod
    {
        public void ProcessPayment(decimal amount) => Console.WriteLine($"Processing credit card payment of {amount}");

    }
    // Paypal payment implementation
    public class PaypalPayment : IPaymentMethod
    {
        public void ProcessPayment(decimal amount) => Console.WriteLine($"Processing Paypal payment of {amount}");
    }

    // Notice:
    // If tomorrow we add StripePayment,
    // we do NOT modify existing classes.
    // We just create a new class implementing IPaymentMethod.
    // This follows OCP.


    // ----D----
    // DEPENDENCY INVERSION PRINCIPLE(DIP)
    // Abstraction for order repository
    public interface IOrderRepository
    {
        void Save(Order order);
    }

    // Concrete implementation (could be DB, API, etc.)
    public class SqlOrderRepository : IOrderRepository
    {
        public void Save(Order order) => Console.WriteLine("Order saved to SQL database.");
    }

    // Abstraction for notification
    public interface INotificationService
    {
        void Send(string message);
    }

    public class EmailNotification : INotificationService
    {
        public void Send(string message) => Console.WriteLine($"Email sent: {message}");
    }

    // ==========================================
    // MAIN SERVICE (High-Level Module)
    // ==========================================

    // This class does NOT depend on concrete classes.
    // It depends on abstractions (interfaces).
    // This is true DIP.
    public class OrderService
    {
        // 1. PRIVATE FIELDS (The "Backpack")
        // These hold onto the dependencies (tools) so the service can use them later.
        // 'readonly' ensures these tools aren't replaced after the service is started.
        private readonly OrderPriceCalculator _calculator;
        private readonly IPaymentMethod _paymentMethod;
        private readonly IOrderRepository _repository;
        private readonly INotificationService _notification;

        // 2. THE CONSTRUCTOR (The "Hand-off")
        // This is where the outside world "injects" the actual tools into the service.
        // Instead of the service creating a 'new' CreditCardPayment, it asks for 
        // "any" IPaymentMethod. This is the heart of Dependency Injection.
        public OrderService(
            OrderPriceCalculator calculator,           // A concrete tool for math
            IPaymentMethod paymentMethod,         // A flexible tool for payment
            IOrderRepository repository,          // A flexible tool for saving
            INotificationService notificationService) // A flexible tool for alerts
        {
            // We take the tools provided in the parameters and store them 
            // in our private fields (the ones starting with underscores).
            _calculator = calculator;
            _paymentMethod = paymentMethod;
            _repository = repository;
            _notification = notificationService;
        }

        // 3. THE ACTION METHOD (The "Work")
        // This method coordinates the process by telling each tool what to do.
        public void PlaceOrder(Order order)
        {
            // Step A: Use the calculator to find out how much the order costs.
            decimal total = _calculator.CalculateTotal(order);

            // Step B: Tell the payment tool to charge that total amount.
            // It doesn't know if it's Credit Card or PayPal; it just calls 'ProcessPayment'.
            _paymentMethod.ProcessPayment(total);

            // Step C: Tell the repository tool to save the order data.
            _repository.Save(order);

            // Step D: Tell the notification tool to let the user know it worked.
            _notification.Send("Order placed successfully!");
        }
    }


    // ==========================================
    // PROGRAM ENTRY POINT
    // ==========================================

    class Program
    {
        static void Main(string[] args)
        {
            // create order
            var order = new Order();
            order.AddItem(new OrderItem("MacBook", 220000));
            order.AddItem(new OrderItem("Iphone Air", 190000));

            // Dependency Injection (manual)
            var calculator = new OrderPriceCalculator();
            IPaymentMethod payment = new PaypalPayment();
            IOrderRepository repository = new SqlOrderRepository();
            INotificationService notification = new EmailNotification();

            var orderService = new OrderService(
                calculator, payment, repository, notification
            );

            orderService.PlaceOrder(order);
        }
    }
}
