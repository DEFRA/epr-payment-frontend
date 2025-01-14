# epr-payment-frontend


## Description
UI to calculate fees and manage payment records for EPR

## Getting Started

### Prerequisites
- EPR Payment Service - https://github.com/DEFRA/epr-payment-service
- EPR Payment Facade - https://github.com/DEFRA/epr-payment-facade
- .NET 8 SDK
- Visual Studio or Visual Studio Code
- Azure ADB2C(Optional for Development - AppSettings: FeatureManagement__EnableAuthenticationFeature:false to disable)
- Redis(Optional for Development - AppSettings: UseLocalSession:true to disable)

- #### Redis

The Portal Optionally requires Redis, the recommended way of runnig Redis locally is to run it via Docker.

```sh
$ docker run -d --name my-redis -p 6379:6379 -p 8001:8001 redis/redis-stack:latest
```

### Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/DEFRA/epr-payment-frontend.git
    ```
2. Navigate to the project directory:
    ```bash
    cd \src\EPR.Payment.Portal
    ```
3. Restore the dependencies:
    ```bash
    dotnet restore
    ```

### Configuration
The application uses appsettings.json for configuration. For development, use appsettings.Development.json.

#### Sample 
appsettings.Development.json

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "FeatureManagement": {
    "EnablePaymentsFeature": true,
    "EnablePaymentStatusInsert": true,
    "EnablePaymentStatusUpdate": true
  }
}
```

### Building the Application
1. Navigate to the project directory:
    ```bash
    cd \src\EPR.Payment.Portal
    ```

2. To build the application:
    ```bash
    dotnet build
    ```

### Running the Application
1. Navigate to the project directory:
    ```bash
    cd \src\EPR.Payment.Portal
    ```
 
2. To run the service locally:
    ```bash
    dotnet run
    ```

3. Launch Browser:
    
    UI:

    [https://localhost:7274/](https://localhost:7274/)
    