using System;

public class Class2
{
	static void Main(string[] args)
	{
		int i = 2, j = 5, n = 7;
        if (i < j && j < n && i < n)
        {
            Console.WriteLine("true");
        }
        else
        {
            Console.WriteLine("false");
        }
	}
}
