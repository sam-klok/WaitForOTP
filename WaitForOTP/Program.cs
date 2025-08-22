// 1. generate a random number between 1 and 100

var random = new Random();
int randomNumber = random.Next(1000, 9999); 
Console.WriteLine($"OTP {randomNumber}");

// 2. wait and display seconds left
//for (int i = 10; i > 0; i--)
//{
//    Console.Write($"\rPlease wait... {i} seconds left ");
//    Thread.Sleep(1000);
//}
//Console.WriteLine(); // Move to the next line after countdown

//// 3. ask the user to guess the number
//Console.Write("Enter the OTP: ");
//string? userInput = Console.ReadLine();


async Task CountdownAsync(int seconds)
{
    for (int i = seconds; i > 0; i--)
    {
        Console.Write($"\rPlease wait... {i} seconds left ");
        await Task.Delay(1000);
    }
    Console.WriteLine();
}

async Task<string?> ReadInputAsync()
{
    Console.Write("\nEnter the OTP: ");
    return await Task.Run(() => Console.ReadLine());
}

var countdownTask = CountdownAsync(10);
var inputTask = ReadInputAsync();

await Task.WhenAny(countdownTask, inputTask);

string? userInput = inputTask.IsCompleted ? inputTask.Result : null;

// If input wasn't completed, prompt for input after countdown
if (userInput == null)
{
    Console.WriteLine("you missed time to enter OTP");
    //Console.Write("\nEnter the OTP: ");
    //userInput = Console.ReadLine();
    return;
}


if (userInput == randomNumber.ToString())
{
    Console.WriteLine("OTP is correct!");
}
else
{
    Console.WriteLine("OTP is incorrect.");
}   