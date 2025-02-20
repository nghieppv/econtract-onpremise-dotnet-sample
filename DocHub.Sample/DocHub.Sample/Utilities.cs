namespace DocHub.Sample
{
    static class Utilities
    {
        public static void ConsoleWriteLine(string message, ConsoleColor colors = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = colors;
            Console.WriteLine($"{message}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(1000);
        }

        public static string ReadInput()
        {
            string input;

            do
            {
                input = string.Empty;
                ConsoleKeyInfo key;
                Console.Write("Please enter information OTP (finish entering with [ENTER]): ");
                do
                {
                    key = Console.ReadKey(intercept: true);

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input = input.Remove(input.Length - 1);
                        Console.Write("\b \b");
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        input += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                } while (true);

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Value cannot be empty. Please re-enter OTP.");
                }
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }
    }
}