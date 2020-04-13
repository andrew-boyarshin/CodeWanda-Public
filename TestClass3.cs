using System;

public class Class1
{
	static void Main(string[] args)
	{
		ulong[] tens = new ulong[20];
		tens[0] = 1;
        int i;
		for (i = 1; i < 20; i++)
			tens[i] = tens[i - 1] * 10;

		Console.ReadLine();
	}
}
