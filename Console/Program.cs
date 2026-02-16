using System;


int[] bridge = { 7, 6, 5, 8 };
Console.WriteLine(FlimsyBridge.UsageCount(bridge));

public class FlimsyBridge
{
    public static int UsageCount(int[] bridge)
    {
        int[] temp = (int[])bridge.Clone();

        while (true)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] -= 2;
                if (temp[i] <= 0)
                {
                    return i + 1;
                }
            }
        }
    }
}


