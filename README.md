# Item Proposal Management API

# 🚀 Overview
- NET-based backend API that supports managing items and facilitating a proposal system between different parties/companies, allowing users to create, review, update, and delete proposals with associated payment ratios. The API follows RESTful principles and supports authentication and validation mechanisms.
  
# 📋 Prerequisites
Before running the application, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) - installed
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) - installed and running
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download) - installed

# 🛠️ Setup & Installation
1️⃣ Clone the Repository
```
  git clone https://github.com/nikknez333/ItemProposalAPI.git
  cd ItemProposalAPI
```
2️⃣ Configure the Database
- Edit appsettings.json and update the connection string:
  
```
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ItemProposalDatabase;Trusted_Connection=True;"
  }
```

3️⃣ Install Dependencies
- If you're using Visual Studio, dependencies will be restored automatically.
Otherwise, run:
  ```bash
  dotnet restore

4️⃣ Apply Database Migrations
- Run the following command to apply database migrations:
  ```bash
  dotnet ef database update
  
5️⃣ Run the Application
- Start the API by using following command:
  ```bash
  dotnet run
  
# 📖 API Documentation
- The API is documented using Swagger/OpenAPI.
- Access Swagger UI: https://localhost:7056/swagger
- View endpoints, request & response formats, and test API calls interactively.
  
# 📌 Considerations & Assumptions
- The API is built using .NET 8 with Entity Framework Core and a SQL Server database.
- Users must be authenticated to interact with API
- Only authorized parties can use certain endpoints.
- There are 3 type of Users: Party Owner, Party Employee, Unemployed User.
- Currently public registration allow registration for either Party Employee or Unemployed Users.
- Party Owners are seeded to database after initial run of application.
- For the purpose of testing the application these are theirs credentials:
- 1. username: timCook3 password: CoO3im!TIM
  2. username: sPichai881 password: piCHAI99?8
  3. username: mArkZuck23 password: zuCkie?226
  4. username: saTYaNadl115 password: 4_TYnadALL
  5. username: sAmAlt331 password: AltMAN99!?
    
# 💻 Tech Stack
- .NET 8
- C#
- Entity Framework Core
- SQL Server
- Microsoft Identity & JWT Bearer for Authentication and Authorization
- FluentValidation for input validation
- Open API/Swagger for API documentation
