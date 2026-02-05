namespace Audabit.Service.SoftwarePatterns.App.Testing._9_Testing;

/// <summary>
/// Service implementation for demonstrating Testing Practices and Methodologies.
/// </summary>
public sealed class TestingPatternsService : ITestingPatternsService
{
    public void TestPyramid()
    {
        Console.WriteLine("\n=== Test Pyramid Concept Demo ===\n");
        TestingPractices.TestPyramid.Run();
        Console.WriteLine("\n=== Test Pyramid Concept: Demonstration complete.===\n");
    }

    public void MockStubFake()
    {
        Console.WriteLine("\n=== Mock Stub Fake Technique Demo ===\n");
        TestingPractices.MockStubFake.Run();
        Console.WriteLine("\n=== Mock Stub Fake Technique: Demonstration complete.===\n");
    }

    public void TDD()
    {
        Console.WriteLine("\n=== TDD Methodology Demo ===\n");
        TestingPractices.TDD.Run();
        Console.WriteLine("\n=== TDD Methodology: Demonstration complete.===\n");
    }

    public void BDD()
    {
        Console.WriteLine("\n=== BDD Methodology Demo ===\n");
        TestingPractices.BDD.Run();
        Console.WriteLine("\n=== BDD Methodology: Demonstration complete.===\n");
    }

    public void GoldenMaster()
    {
        Console.WriteLine("\n=== Golden Master Technique Demo ===\n");
        TestingPractices.GoldenMaster.Run();
        Console.WriteLine("\n=== Golden Master Technique: Demonstration complete.===\n");
    }
}