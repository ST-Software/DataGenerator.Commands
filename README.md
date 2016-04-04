# DataGenerator.Commands

.NET Core utility that search for DataGenerator class in your project and calls Generate method where it passed initialized DbContext.

## Example

__Generator class:__
```csharp
 public class DataGenerator
    {
        public void Generate(AppDbContext dbContext)
        {
            dbContext.Users.Add(new User { Name = "User1" });
            dbContext.Users.Add(new User { Name = "User2" });
            dbContext.SaveChanges();
        }
    }
```    
OR

```csharp
 public class DataGenerator
    {
        public void Generate(AppDbContext dbContext, string argument)
        {
			//argument contains the value from the --argument parameter
            dbContext.Users.Add(new User { Name = "User1" });
            dbContext.Users.Add(new User { Name = "User2" });
            dbContext.SaveChanges();
        }
    }
```    

__project.json:__
```json
  "dependencies": {
    "other dependencies":"...",
    "DataGenerator.Commands": "1.0.*"
  },

  "commands": {
    "datagen": "DataGenerator.Commands"
  },
```

## Parameters
DataGenerator.Commands accept following parameters:

|Parameter              |Description            |
|---------              |-----------            |
|`--dbContextAssembly <DB_CONTEXT_ASSEMBLY_NAME>`       | Assembly name where the DbContext is located. |
|`--generatorType <GENERATOR_TYPE>`                     | Generator class name (this class has to have Generate(DbContext) method). |
|`--generatorAssembly <GENERATOR_ASSEMBLY_NAME>`        | Generator class name (this class has to have Generate(DbContext) method). |
|`--startupAssembly <STARTUP_ASSEMBLY>`                 | Assembly name where the Startup class which initialize DbContext is located. |
|`--environment <ENVIRONMENT>`                          | Used environment passed to Startup class for initialization (default is 'Development'). |
|`--recreate <DATABASE_HOST>|<DATABASE_NAME>`           | Drops and creates database you want to generate data to. The parameters are has to match connection string parameters. Except the '.' in connection string is replaced by machine name in that comparision. |
|`--mode wait`                                          | If mode is 'wait' it waits at the and for user to press enter before leaving a program. |
|`--argument <ARGUMENT>									| This argument is passed as the second argument to the data generator. |
