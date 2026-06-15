
# Trainee Management System

A Trainee management system API to manage all trainee records by performing CRUD operations through REST APIs. The system is in .NET and the database is In-Memory.

## Tech Stack

ASP.NET, OpenAPI / Swagger, EF Core


## How to Run

Go to the appsettings.json, insert this values for MySQL connection and JWT token information.
```bash
  "ConnectionStrings": {
    "DefaultConnection" : "Server=localhost;Port=3306;Database=<databasename>;User ID=<username>;Password=<password>;SslMode=Required;"
  },
  "JWT":{
    "Key":"128-bit random secret key",
    "Issuer":"<Issuer name or domain>",
    "Audience":"<Frontend name or domain>",
    "ExpiresIn": Expiry time in milliseconds
  }
```
First install all required packages.
```bash
  dotnet restore
```
Then run this command to create all tables and relation in database.
```bash
  dotnet ef database update
```
To launch the project in development with this profile.
```bash
 dotnet run --launch-profile https    
```
To launch in watch mode.
```bash
  dotnet watch --launch-profile https    
```
## Login CRedentials

WHen you launch project, it will first seed a admin user in databse. Go to this path with this request body to get your JWT token for further operations.

```bash
  POST/ /api/auth/login   
```
Request Body:
```bash
  {
    "username" : "admin",
    "password" : "Admin@123456"  
  }    
```


## API Reference

#### Health check of application

```http
  GET /api/Health
```

#### Interactive Swagger UI for testing of routes
```http
  GET /swagger
```

#### Get all Trainees with optional search query 

```http
  GET /api/trainees
```

| query | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `search` | `string` | **Optional** It check whether first name, last name, texh stack, email contains search string.  |

#### Get Trainee by Id

```http
  GET /api/trainees/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `Id`      | `long` | **Required**. Id of trainee to fetch |

#### Add Trainee 
```http
  POST /api/trainees
```
Request Body
| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `firstName`      | `string` | **Required**. First name min 3 max 50. |
| `lastName`      | `string` | **Required**. Last name min 3 max 50 |
| `email`      | `string` | **Required**. Valid email. |
| `techStack`      | `string` | **Required**. |
| `status`      | `string` | **Required**. status in 'Active', 'Inactive','Completed' |

#### Update Trainee 
```http
  PUT /api/trainees/${Id}
```
| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `Id`      | `long` | **Required**. Id of trainee to update |

Request Body
| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `firstName`      | `string` | **Required**. First name min 3 max 50. |
| `lastName`      | `string` | **Required**. Last name min 3 max 50 |
| `email`      | `string` | **Required**. Valid email. |
| `techStack`      | `string` | **Required**. |
| `status`      | `string` | **Required**. status in 'Active', 'Inactive','Completed' |

#### Delete Trainee 
```http
  DELETE /api/trainees/${Id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `Id`      | `long` | **Required**. Id of trainee to delete |
## Sample Request JSON

```bash
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "techStack": "string",
  "status": "string"
}
```

## Sample Response JSON

```bash
{
  "status": "bool",
  "message": "string",
  "data"?: "T",
  "error"?: "object"
}
```

## Known limitations

The database is stored in In Memory, once the application restarts the data get lost. The api lacks security for authentication and authorisation purpose & Error Handling.

