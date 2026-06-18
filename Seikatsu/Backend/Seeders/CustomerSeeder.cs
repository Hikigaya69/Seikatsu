
using Bogus;
using Seikatsu.Backend.Exceptions;
using Seikatsu.Backend.Models;
using Seikatsu.Backend.Services;

public class CustomerSeeder (IAuthService authService)
{
    public async Task SeedAsync(int count = 100)
    {
        var usedNames = new HashSet<string>();
        var usedEmails = new HashSet<string>();
        var seedLog = new List<string>(); // track plain-text credentials

        var faker = new Faker("en");

        int seeded = 0;
        int skipped = 0;

        while (seeded < count)
        {
            string fullName;
            do { fullName = faker.Name.FullName().ToLower(); }
            while (!usedNames.Add(fullName));

            string email;
            do { email = faker.Internet.Email().ToLower(); }
            while (!usedEmails.Add(email));

            // plain text — RegisterAsync will hash it internally
            string plainPassword = $"Pass@{faker.Internet.Password(8)}!Aa1";

            try
            {
                await authService.RegisterAsync(new CustomerDTO
                {
                    FullName = fullName,
                    Email = email,
                    Password = plainPassword   // plain text, service hashes it
                });

                seeded++;

                // log the plain text credentials for testing use
                seedLog.Add($"{seeded},{fullName},{email},{plainPassword}");
                Console.WriteLine($" ({seeded}/{count}) {email} | {plainPassword}");
            }
            catch (ConflictException)
            {
                skipped++;
                Console.WriteLine($"Skipped (conflict)  {email}");
            }
            catch (BadRequestException ex)
            {
                Console.WriteLine($"[✗] Validation failed: {string.Join(", ", ex.Errors.Values)}");
            }
        }

        // save credentials to a CSV so you can log in later
        var lines = new List<string> { "no,fullname,email,plain_password" };
        lines.AddRange(seedLog);
        await File.WriteAllLinesAsync("seeded_customers.csv", lines);

        Console.WriteLine($"\n Done! {seeded} seeded, {skipped} skipped.");
        Console.WriteLine($"Plain-text credentials saved to → seeded_customers.csv");
    }
}