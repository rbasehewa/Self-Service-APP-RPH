using StaffServiceAPI.Models;

namespace StaffServiceAPI.Data;

/// <summary>
/// Seeds the database with sample Users and Staff data
/// WHAT THIS DOES:
/// - Creates 100 sample Users (for UsersController)  
/// - Creates 100 sample Staff (for StaffController)
/// - Only runs if tables are empty (won't duplicate data)
/// </summary>
public static class DataSeeder
{
    // Main entry point - called from Program.cs
    public static async Task SeedDatabase(AppDbContext context)
    {
        Console.WriteLine("🔍 Checking database for existing data...");
        
        // Seed Users table
        await SeedUsers(context);
        
        // Seed Staff table  
        await SeedStaff(context);
        
        Console.WriteLine("✅ Database seeding complete!");
    }

    /// <summary>
    /// Seeds Users table with 100 sample users
    /// SKIPS if Users already exist (prevents duplicates)
    /// </summary>
    private static async Task SeedUsers(AppDbContext context)
    {
        // Check if Users data already exists
        if (context.Users.Any())
        {
            Console.WriteLine("   ℹ️  Users table already has data ({0} users). Skipping Users seed.", context.Users.Count());
            return;
        }

        Console.WriteLine("   🌱 Seeding Users table with 100 sample users...");

        // All 100 users from your original file
        var users = new List<User>
        {
            new User { Name = "Alice Adams", ImmuneStatus = "Immune", PhoneNumber = "0400 100 001", YearOfBirth = 1988, VaccineDate = "2023-03-15" },
            new User { Name = "Bob Brown", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 101 002", YearOfBirth = 1985, VaccineDate = "N/A" },
            new User { Name = "Charlie Clark", ImmuneStatus = "Unknown", PhoneNumber = "0400 102 003", YearOfBirth = 1992, VaccineDate = "N/A" },
            new User { Name = "Diana Davis", ImmuneStatus = "Immune", PhoneNumber = "0400 103 004", YearOfBirth = 1990, VaccineDate = "2022-11-02" },
            new User { Name = "Ethan Evans", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 104 005", YearOfBirth = 1983, VaccineDate = "N/A" },
            new User { Name = "Fiona Foster", ImmuneStatus = "Unknown", PhoneNumber = "0400 105 006", YearOfBirth = 1995, VaccineDate = "N/A" },
            new User { Name = "George Green", ImmuneStatus = "Immune", PhoneNumber = "0400 106 007", YearOfBirth = 1979, VaccineDate = "2021-08-19" },
            new User { Name = "Hannah Hill", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 107 008", YearOfBirth = 1991, VaccineDate = "N/A" },
            new User { Name = "Isaac Irwin", ImmuneStatus = "Unknown", PhoneNumber = "0400 108 009", YearOfBirth = 1987, VaccineDate = "N/A" },
            new User { Name = "Jade Jones", ImmuneStatus = "Immune", PhoneNumber = "0400 109 010", YearOfBirth = 1993, VaccineDate = "2024-01-05" },
            new User { Name = "Kevin Kelly", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 110 011", YearOfBirth = 1980, VaccineDate = "N/A" },
            new User { Name = "Lara Lewis", ImmuneStatus = "Unknown", PhoneNumber = "0400 111 012", YearOfBirth = 1986, VaccineDate = "N/A" },
            new User { Name = "Mason Miller", ImmuneStatus = "Immune", PhoneNumber = "0400 112 013", YearOfBirth = 1994, VaccineDate = "2020-09-23" },
            new User { Name = "Nina Nelson", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 113 014", YearOfBirth = 1982, VaccineDate = "N/A" },
            new User { Name = "Oscar Owens", ImmuneStatus = "Unknown", PhoneNumber = "0400 114 015", YearOfBirth = 1990, VaccineDate = "N/A" },
            new User { Name = "Paige Parker", ImmuneStatus = "Immune", PhoneNumber = "0400 115 016", YearOfBirth = 1989, VaccineDate = "2021-06-30" },
            new User { Name = "Quinn Quinn", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 116 017", YearOfBirth = 1978, VaccineDate = "N/A" },
            new User { Name = "Riley Reid", ImmuneStatus = "Unknown", PhoneNumber = "0400 117 018", YearOfBirth = 1996, VaccineDate = "N/A" },
            new User { Name = "Sara Smith", ImmuneStatus = "Immune", PhoneNumber = "0400 118 019", YearOfBirth = 1984, VaccineDate = "2022-03-11" },
            new User { Name = "Tom Taylor", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 119 020", YearOfBirth = 1991, VaccineDate = "N/A" },
            new User { Name = "Uma Underwood", ImmuneStatus = "Unknown", PhoneNumber = "0400 120 021", YearOfBirth = 1987, VaccineDate = "N/A" },
            new User { Name = "Victor Vale", ImmuneStatus = "Immune", PhoneNumber = "0400 121 022", YearOfBirth = 1981, VaccineDate = "2023-07-09" },
            new User { Name = "Wendy Ward", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 122 023", YearOfBirth = 1988, VaccineDate = "N/A" },
            new User { Name = "Xavier Xu", ImmuneStatus = "Unknown", PhoneNumber = "0400 123 024", YearOfBirth = 1992, VaccineDate = "N/A" },
            new User { Name = "Yasmin Young", ImmuneStatus = "Immune", PhoneNumber = "0400 124 025", YearOfBirth = 1995, VaccineDate = "2020-12-14" },
            new User { Name = "Zane Ziegler", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 125 026", YearOfBirth = 1983, VaccineDate = "N/A" },
            new User { Name = "Aaron Archer", ImmuneStatus = "Unknown", PhoneNumber = "0400 126 027", YearOfBirth = 1989, VaccineDate = "N/A" },
            new User { Name = "Bella Brooks", ImmuneStatus = "Immune", PhoneNumber = "0400 127 028", YearOfBirth = 1994, VaccineDate = "2021-02-18" },
            new User { Name = "Cody Carter", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 128 029", YearOfBirth = 1986, VaccineDate = "N/A" },
            new User { Name = "Dana Dawson", ImmuneStatus = "Unknown", PhoneNumber = "0400 129 030", YearOfBirth = 1990, VaccineDate = "N/A" },
            new User { Name = "Eliza East", ImmuneStatus = "Immune", PhoneNumber = "0400 130 031", YearOfBirth = 1986, VaccineDate = "2021-05-22" },
            new User { Name = "Felix Ford", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 131 032", YearOfBirth = 1993, VaccineDate = "N/A" },
            new User { Name = "Grace Gray", ImmuneStatus = "Unknown", PhoneNumber = "0400 132 033", YearOfBirth = 1995, VaccineDate = "N/A" },
            new User { Name = "Harvey Hunt", ImmuneStatus = "Immune", PhoneNumber = "0400 133 034", YearOfBirth = 1982, VaccineDate = "2023-03-18" },
            new User { Name = "Indie Ingram", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 134 035", YearOfBirth = 1990, VaccineDate = "N/A" },
            new User { Name = "Jonah Jenkins", ImmuneStatus = "Unknown", PhoneNumber = "0400 135 036", YearOfBirth = 1987, VaccineDate = "N/A" },
            new User { Name = "Kara Knox", ImmuneStatus = "Immune", PhoneNumber = "0400 136 037", YearOfBirth = 1984, VaccineDate = "2020-10-09" },
            new User { Name = "Liam Lane", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 137 038", YearOfBirth = 1989, VaccineDate = "N/A" },
            new User { Name = "Mila Marsh", ImmuneStatus = "Unknown", PhoneNumber = "0400 138 039", YearOfBirth = 1996, VaccineDate = "N/A" },
            new User { Name = "Noah Neal", ImmuneStatus = "Immune", PhoneNumber = "0400 139 040", YearOfBirth = 1991, VaccineDate = "2022-12-01" },
            new User { Name = "Olivia Oak", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 140 041", YearOfBirth = 1983, VaccineDate = "N/A" },
            new User { Name = "Piper Price", ImmuneStatus = "Unknown", PhoneNumber = "0400 141 042", YearOfBirth = 1994, VaccineDate = "N/A" },
            new User { Name = "Reece Ray", ImmuneStatus = "Immune", PhoneNumber = "0400 142 043", YearOfBirth = 1980, VaccineDate = "2023-08-17" },
            new User { Name = "Sienna Scott", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 143 044", YearOfBirth = 1988, VaccineDate = "N/A" },
            new User { Name = "Theo Trent", ImmuneStatus = "Unknown", PhoneNumber = "0400 144 045", YearOfBirth = 1982, VaccineDate = "N/A" },
            new User { Name = "Violet Voss", ImmuneStatus = "Immune", PhoneNumber = "0400 145 046", YearOfBirth = 1989, VaccineDate = "2021-09-03" },
            new User { Name = "Will West", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 146 047", YearOfBirth = 1986, VaccineDate = "N/A" },
            new User { Name = "Zara Zane", ImmuneStatus = "Unknown", PhoneNumber = "0400 147 048", YearOfBirth = 1995, VaccineDate = "N/A" },
            new User { Name = "Aiden Ash", ImmuneStatus = "Immune", PhoneNumber = "0400 148 049", YearOfBirth = 1984, VaccineDate = "2022-02-26" },
            new User { Name = "Brooke Blake", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 149 050", YearOfBirth = 1993, VaccineDate = "N/A" },
            new User { Name = "Caleb Cole", ImmuneStatus = "Unknown", PhoneNumber = "0400 150 051", YearOfBirth = 1991, VaccineDate = "N/A" },
            new User { Name = "Delia Dean", ImmuneStatus = "Immune", PhoneNumber = "0400 151 052", YearOfBirth = 1988, VaccineDate = "2020-06-14" },
            new User { Name = "Eli Edge", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 152 053", YearOfBirth = 1985, VaccineDate = "N/A" },
            new User { Name = "Freya Frost", ImmuneStatus = "Unknown", PhoneNumber = "0400 153 054", YearOfBirth = 1997, VaccineDate = "N/A" },
            new User { Name = "Gabe Gale", ImmuneStatus = "Immune", PhoneNumber = "0400 154 055", YearOfBirth = 1990, VaccineDate = "2023-10-12" },
            new User { Name = "Hallie Hart", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 155 056", YearOfBirth = 1986, VaccineDate = "N/A" },
            new User { Name = "Ivy Ives", ImmuneStatus = "Unknown", PhoneNumber = "0400 156 057", YearOfBirth = 1994, VaccineDate = "N/A" },
            new User { Name = "Jasper Jude", ImmuneStatus = "Immune", PhoneNumber = "0400 157 058", YearOfBirth = 1982, VaccineDate = "2021-01-28" },
            new User { Name = "Kylie King", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 158 059", YearOfBirth = 1993, VaccineDate = "N/A" },
            new User { Name = "Logan Locke", ImmuneStatus = "Unknown", PhoneNumber = "0400 159 060", YearOfBirth = 1996, VaccineDate = "N/A" },
            new User { Name = "Maddie Moore", ImmuneStatus = "Immune", PhoneNumber = "0400 160 061", YearOfBirth = 1989, VaccineDate = "2022-09-15" },
            new User { Name = "Nolan North", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 161 062", YearOfBirth = 1984, VaccineDate = "N/A" },
            new User { Name = "Opal Orr", ImmuneStatus = "Unknown", PhoneNumber = "0400 162 063", YearOfBirth = 1995, VaccineDate = "N/A" },
            new User { Name = "Penny Page", ImmuneStatus = "Immune", PhoneNumber = "0400 163 064", YearOfBirth = 1992, VaccineDate = "2023-04-30" },
            new User { Name = "Quentin Quick", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 164 065", YearOfBirth = 1987, VaccineDate = "N/A" },
            new User { Name = "Rosa Reed", ImmuneStatus = "Unknown", PhoneNumber = "0400 165 066", YearOfBirth = 1991, VaccineDate = "N/A" },
            new User { Name = "Spencer Shaw", ImmuneStatus = "Immune", PhoneNumber = "0400 166 067", YearOfBirth = 1985, VaccineDate = "2021-03-11" },
            new User { Name = "Tara Tate", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 167 068", YearOfBirth = 1993, VaccineDate = "N/A" },
            new User { Name = "Ulrich Upton", ImmuneStatus = "Unknown", PhoneNumber = "0400 168 069", YearOfBirth = 1988, VaccineDate = "N/A" },
            new User { Name = "Vera Vane", ImmuneStatus = "Immune", PhoneNumber = "0400 169 070", YearOfBirth = 1986, VaccineDate = "2020-07-21" },
            new User { Name = "Wyatt Wood", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 170 071", YearOfBirth = 1990, VaccineDate = "N/A" },
            new User { Name = "Xena Xaio", ImmuneStatus = "Unknown", PhoneNumber = "0400 171 072", YearOfBirth = 1994, VaccineDate = "N/A" },
            new User { Name = "Yuri York", ImmuneStatus = "Immune", PhoneNumber = "0400 172 073", YearOfBirth = 1987, VaccineDate = "2022-05-03" },
            new User { Name = "Zelda Zee", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 173 074", YearOfBirth = 1982, VaccineDate = "N/A" },
            new User { Name = "Adrian Ames", ImmuneStatus = "Unknown", PhoneNumber = "0400 174 075", YearOfBirth = 1989, VaccineDate = "N/A" },
            new User { Name = "Bianca Birch", ImmuneStatus = "Immune", PhoneNumber = "0400 175 076", YearOfBirth = 1991, VaccineDate = "2024-02-10" },
            new User { Name = "Callum Cross", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 176 077", YearOfBirth = 1990, VaccineDate = "N/A" },
            new User { Name = "Demi Dale", ImmuneStatus = "Unknown", PhoneNumber = "0400 177 078", YearOfBirth = 1997, VaccineDate = "N/A" },
            new User { Name = "Eliot Ember", ImmuneStatus = "Immune", PhoneNumber = "0400 178 079", YearOfBirth = 1984, VaccineDate = "2023-06-18" },
            new User { Name = "Farah Flint", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 179 080", YearOfBirth = 1993, VaccineDate = "N/A" },
            new User { Name = "Gus Glover", ImmuneStatus = "Unknown", PhoneNumber = "0400 180 081", YearOfBirth = 1995, VaccineDate = "N/A" },
            new User { Name = "Hazel Hale", ImmuneStatus = "Immune", PhoneNumber = "0400 181 082", YearOfBirth = 1988, VaccineDate = "2021-12-07" },
            new User { Name = "Irwin Ives", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 182 083", YearOfBirth = 1984, VaccineDate = "N/A" },
            new User { Name = "Juno Jett", ImmuneStatus = "Unknown", PhoneNumber = "0400 183 084", YearOfBirth = 1996, VaccineDate = "N/A" },
            new User { Name = "Kendall Kerr", ImmuneStatus = "Immune", PhoneNumber = "0400 184 085", YearOfBirth = 1990, VaccineDate = "2022-03-22" },
            new User { Name = "Lola Lane", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 185 086", YearOfBirth = 1985, VaccineDate = "N/A" },
            new User { Name = "Marco Moss", ImmuneStatus = "Unknown", PhoneNumber = "0400 186 087", YearOfBirth = 1992, VaccineDate = "N/A" },
            new User { Name = "Nadia Neal", ImmuneStatus = "Immune", PhoneNumber = "0400 187 088", YearOfBirth = 1991, VaccineDate = "2020-08-29" },
            new User { Name = "Owen Oaks", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 188 089", YearOfBirth = 1983, VaccineDate = "N/A" },
            new User { Name = "Priya Pike", ImmuneStatus = "Unknown", PhoneNumber = "0400 189 090", YearOfBirth = 1996, VaccineDate = "N/A" },
            new User { Name = "Quincy Quinn", ImmuneStatus = "Immune", PhoneNumber = "0400 190 091", YearOfBirth = 1989, VaccineDate = "2023-04-14" },
            new User { Name = "Ronan Redd", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 191 092", YearOfBirth = 1984, VaccineDate = "N/A" },
            new User { Name = "Skye Snow", ImmuneStatus = "Unknown", PhoneNumber = "0400 192 093", YearOfBirth = 1995, VaccineDate = "N/A" },
            new User { Name = "Trent Troy", ImmuneStatus = "Immune", PhoneNumber = "0400 193 094", YearOfBirth = 1987, VaccineDate = "2022-11-05" },
            new User { Name = "Uma Urban", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 194 095", YearOfBirth = 1986, VaccineDate = "N/A" },
            new User { Name = "Vince Vale", ImmuneStatus = "Unknown", PhoneNumber = "0400 195 096", YearOfBirth = 1993, VaccineDate = "N/A" },
            new User { Name = "Willow Wynn", ImmuneStatus = "Immune", PhoneNumber = "0400 196 097", YearOfBirth = 1988, VaccineDate = "2023-01-17" },
            new User { Name = "Xavier Xeno", ImmuneStatus = "Non-Immune", PhoneNumber = "0400 197 098", YearOfBirth = 1985, VaccineDate = "N/A" },
            new User { Name = "Yara Yule", ImmuneStatus = "Unknown", PhoneNumber = "0400 198 099", YearOfBirth = 1997, VaccineDate = "N/A" },
            new User { Name = "Zack Zorn", ImmuneStatus = "Immune", PhoneNumber = "0400 199 100", YearOfBirth = 1983, VaccineDate = "2021-10-20" }
        };

        // Add all users to database
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"   ✅ Successfully seeded {users.Count} users!");
    }

    /// <summary>
    /// Seeds Staff table with 100 randomly generated staff members
    /// SKIPS if Staff already exist (prevents duplicates)
    /// RANDOM DATA: Names, departments, immune status, birth dates
    /// </summary>
    private static async Task SeedStaff(AppDbContext context)
    {
        // Check if Staff data already exists
        if (context.Staff.Any())
        {
            Console.WriteLine($"   ℹ️  Staff table already has data ({context.Staff.Count()} staff). Skipping Staff seed.");
            return;
        }

        Console.WriteLine("   🌱 Seeding Staff table with 100 sample staff members...");

        // Arrays of sample data for random generation
        var departments = new[] 
        { 
            "Cardiology", "Emergency", "ICU", "Oncology", 
            "Pediatrics", "Surgery", "Radiology", "Neurology",
            "Orthopedics", "Dermatology"
        };
        
        var firstNames = new[] 
        { 
            "James", "Mary", "John", "Patricia", "Robert", "Jennifer", 
            "Michael", "Linda", "William", "Elizabeth", "David", "Barbara",
            "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah",
            "Charles", "Karen", "Christopher", "Nancy", "Daniel", "Lisa"
        };
        
        var lastNames = new[] 
        { 
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia",
            "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez",
            "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore"
        };

        var random = new Random(42); // Fixed seed = same data every time (useful for debugging!)
        var staffList = new List<Staff>();

        // Generate 100 random staff members
        for (int i = 1; i <= 100; i++)
        {
            var staff = new Staff
            {
                // Random name: "FirstName LastName"
                Name = $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}",
                
                // Random birth date: Age between 25 and 65 years old
                DateOfBirth = DateTime.UtcNow.AddYears(-random.Next(25, 66)).AddDays(-random.Next(0, 365)),
                
                // Random gender: 50/50 split
                Gender = random.Next(2) == 0 ? "Male" : "Female",
                
                // Random immune status: 75% immune, 25% non-immune
                ImmuneStatus = random.Next(4) == 0 ? "Non-Immune" : "Immune",
                
                // Random department
                Department = departments[random.Next(departments.Length)]
            };

            staffList.Add(staff);
        }

        // Save all staff to database
        await context.Staff.AddRangeAsync(staffList);
        await context.SaveChangesAsync();

        // Show statistics in console
        var immuneCount = staffList.Count(s => s.ImmuneStatus == "Immune");
        var nonImmuneCount = staffList.Count(s => s.ImmuneStatus == "Non-Immune");
        
        Console.WriteLine($"   ✅ Successfully seeded {staffList.Count} staff members!");
        Console.WriteLine($"      📊 {immuneCount} immune, {nonImmuneCount} non-immune");
        
        // Show department breakdown
        var deptGroups = staffList.GroupBy(s => s.Department)
                                  .OrderByDescending(g => g.Count())
                                  .Take(5); // Show top 5 departments
        
        Console.WriteLine("      🏥 Top departments:");
        foreach (var group in deptGroups)
        {
            Console.WriteLine($"         • {group.Key}: {group.Count()} staff");
        }
    }
}