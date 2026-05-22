using System;


int[] numbers = { -4, -2, 1, 4, 8 };
Console.WriteLine(FindClosestNumber(numbers));

// ... existing code ...
int FindClosestNumber(int[] nums)
{
    if (nums == null || nums.Length == 0)
        throw new ArgumentException("El arreglo no puede estar vacío.", nameof(nums));

    int closest = nums[0];          // valor inicial
    int minAbs   = Math.Abs(closest); // mínima magnitud absoluta encontrada

    foreach (var currentNumber in nums.Skip(1))
    {
        int absCurrent = Math.Abs(currentNumber);
        if (absCurrent < minAbs)
        {
            minAbs   = absCurrent;
            closest  = currentNumber;
        }
    }

    return closest;
}
// ... existing code ...