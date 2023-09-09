using TuberTreats.Models;

List<TuberDriver> drivers = new List<TuberDriver>
{
    new TuberDriver
    {
        Id = 1,
        Name = "John Smith"
    },
    new TuberDriver
    {
        Id = 2,
        Name = "Emily Davis"
    },
    new TuberDriver
    {
        Id = 3,
        Name = "Michael Johnson"
    }
};

List<Customer> customers = new List<Customer>
{
    new Customer
    {
        Id = 1,
        Name = "Alice Johnson",
        Address = "123 Main Street"
    },
    new Customer
    {
        Id = 2,
        Name = "Bob Anderson",
        Address = "456 Elm Street"
    },
    new Customer
    {
        Id = 3,
        Name = "Charlie Brown",
        Address = "789 Oak Street"
    },
    new Customer
    {
        Id = 4,
        Name = "David Wilson",
        Address = "101 Pine Street"
    },
    new Customer
    {
        Id = 5,
        Name = "Eve Miller",
        Address = "202 Cedar Street"
    }
};

List<Topping> toppings = new List<Topping>
{
    new Topping
    {
        Id = 1,
        Name = "Sour Cream"
    },
    new Topping
    {
        Id = 2,
        Name = "Cheddar Cheese"
    },
    new Topping
    {
        Id = 3,
        Name = "Bacon Bits"
    },
    new Topping
    {
        Id = 4,
        Name = "Chives"
    },
    new Topping
    {
        Id = 5,
        Name = "Broccoli"
    }
};

List<TuberOrder> tuberOrders = new List<TuberOrder>
{
    new TuberOrder
    {
        Id = 1,
        OrderPlacedOnDate = DateTime.Now.AddDays(-10),
        CustomerId = 1,
        TuberDriverId = 1,
        DeliveredOnDate = DateTime.Now.AddDays(-5),
    },
    new TuberOrder
    {
        Id = 2,
        OrderPlacedOnDate = DateTime.Now.AddDays(-7),
        CustomerId = 4,
        TuberDriverId = 3,
        DeliveredOnDate = DateTime.Now.AddHours(1),
    },
    new TuberOrder
    {
        Id = 3,
        OrderPlacedOnDate = DateTime.Now.AddDays(-2),
        CustomerId = 3,
        TuberDriverId = 2,
        DeliveredOnDate = DateTime.Now.AddHours(4),
    },
    new TuberOrder
    {
        Id = 4,
        OrderPlacedOnDate = DateTime.Now.AddDays(-3),
        CustomerId = 2,
        TuberDriverId = 0,
    },
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping
    {
        Id = 1,
        ToppingId = 1,
        TuberOrderId = 1
    },
    new TuberTopping
    {
        Id = 2,
        ToppingId = 2,
        TuberOrderId = 1
    },
    new TuberTopping
    {
        Id = 3,
        ToppingId = 3,
        TuberOrderId = 2
    },
    new TuberTopping
    {
        Id = 4,
        ToppingId = 4,
        TuberOrderId = 3,
    }
};


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here

// get all tuber orders 
app.MapGet("/tuberorders", () =>
{
    return tuberOrders;
});

// get specific tuberorder by id 
// set up route with paramater of id that is a type integer 
// iterate through TuberOrder collection to find the first matching 
// object where TuberOrder.Id matches with id paramter 
// if no results, return null 
// search for customer and tuberdriver objects that have matching ids 
// to the foundTuberOrder foreign key ids 
// and assign to the foundTuberOrder object
// populate the list of toppings by finding the TuberTopping associated with 
// TuberOrder by TuberOrderId 
// Retrieve ToppingId from TuberTopping object to filter toppings collection
// convert matching toppings to list
app.MapGet("/tuberorders/{id}", (int id) =>
{
    TuberOrder foundTuberOrder = tuberOrders.FirstOrDefault(to => to.Id == id);
    if (foundTuberOrder == null)
    {
        return Results.NotFound();
    }

    foundTuberOrder.TuberDriver = drivers.FirstOrDefault(d => d.Id == foundTuberOrder.TuberDriverId);

    foundTuberOrder.Customer = customers.FirstOrDefault(c => c.Id == foundTuberOrder.CustomerId);

    List<TuberTopping> foundTuberToppings = tuberToppings.Where(tt => tt.TuberOrderId == id).ToList();

    foundTuberOrder.Toppings = foundTuberToppings.Select(tt => toppings.FirstOrDefault(t => t.Id == tt.ToppingId)).ToList();

    return Results.Ok(foundTuberOrder);
});

// create new tuberorder 
// route to tuberorders
// with parameters of type TuberOrder and newOrder object
// method will create new Id 
// create OrderPlacedOnDate property
// add to list of tuber orders
app.MapPost("/tuberorders", (TuberOrder newOrder) =>
{
    newOrder.Id = tuberOrders.Count > 0 ? tuberOrders.Max(to => to.Id) + 1 : 1;
    newOrder.OrderPlacedOnDate = DateTime.Now;
    tuberOrders.Add(newOrder);
    return newOrder;
});

app.MapPut("/tuberorders/{id}", (int id, TuberOrder tuberOrder) =>
{
    TuberOrder orderToUpdate = tuberOrders.FirstOrDefault(to => to.Id == id);
    int orderIndex = tuberOrders.IndexOf(orderToUpdate);
    if (orderToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != tuberOrder.Id)
    {
        return Results.BadRequest();
    }
    tuberOrders[orderIndex] = tuberOrder;
    return Results.Ok();
});

app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    TuberOrder orderToComplete = tuberOrders.FirstOrDefault(to => to.Id == id);
    orderToComplete.DeliveredOnDate = DateTime.Today;
});

app.MapGet("/toppings", () =>
{
    return toppings;
});

app.MapGet("/toppings/{id}", (int id) =>
{
    Topping foundToppings = toppings.FirstOrDefault(t => t.Id == id);
    if (foundToppings == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(foundToppings);
});

app.MapGet("/tubertoppings", () =>
{
    return tuberToppings;
});


// create new post endpoint for tubertopping
app.MapPost("/tubertoppings/{id}", (int id, TuberTopping newTuberTopping) =>
{
    newTuberTopping.Id = tuberToppings.Count > 0 ? tuberToppings.Max(tt => tt.Id) + 1 : 1;
    newTuberTopping.TuberOrderId = id;
    tuberToppings.Add(newTuberTopping);
    return newTuberTopping;
});

app.MapDelete("/tubertoppings/{id}", (int id) =>
{
    TuberTopping toppingToRemove = tuberToppings.FirstOrDefault(tt => tt.Id == id);
    if (toppingToRemove == null)
    {
        return Results.NotFound();
    }

    foreach (TuberOrder tuberOrder in tuberOrders)
    {
        if (tuberOrder.Id == toppingToRemove.TuberOrderId)
        {
            tuberOrder.Toppings.Remove(toppingToRemove.Topping);
        }
    }

    return Results.Ok();
});


app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer foundCustomer = customers.FirstOrDefault(c => c.Id == id);
    if (foundCustomer == null)
    {
        return Results.NotFound();
    }

    foundCustomer.TuberOrders = tuberOrders.Where(o => o.CustomerId == id).ToList();

    return Results.Ok(foundCustomer);
});

app.MapPost("/customers", (Customer newCustomer) =>
{
    newCustomer.Id = customers.Count > 0 ? customers.Max(c => c.Id) + 1 : 1;
    customers.Add(newCustomer);
    return newCustomer;
});

app.MapDelete("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);

    if (customer != null)
    {
        customers.Remove(customer);
    }
});

app.MapGet("/tuberdrivers", () =>
{
    return drivers;
});

app.MapGet("/tuberdrivers/{id}", (int id) =>
{
    TuberDriver foundDriver = drivers.FirstOrDefault(d => d.Id == id);
    if (foundDriver == null)
    {
        return Results.NotFound();
    }

    List<TuberOrder> driverDeliveries = tuberOrders.Where(o => o.TuberDriverId == id).ToList();

    foundDriver.TuberDeliveries = driverDeliveries;

    return Results.Ok(foundDriver);
});

app.Run();
//don't touch or move this!
public partial class Program { }